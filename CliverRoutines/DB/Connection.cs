//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************

//################################
// 
// (!!!) When using a certain database type include the respective implementation files into the project and add the required reference.
//
//################################

using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cliver.Db
{
    public abstract class Connection : IDisposable
    {
        ~Connection()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                ClearCachedCommands();
                NativeConnection?.Dispose();
            }
        }

        public virtual void ClearCachedCommands()
        {
            lock (sqls2command)
            {
                while (sqls2command.Count > 0)
                {
                    string k = sqls2command.Keys.First();
                    sqls2command[k].Dispose();
                    sqls2command.Remove(k);
                }
            }
        }

        //static Connection()
        //{
        //    This = Create();
        //}

        //public static Connection This { get; protected set; }

        //public static Connection Create(string connectionString = null)
        //{
        //    if (connectionString == null)
        //        throw new Exception("connectionString is null.");

        //    if (Regex.IsMatch(connectionString, @"\.mdf|\.sdf|Initial\s+Catalog\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
        //        return new MsSqlConnection(connectionString);
        //    if (Regex.IsMatch(connectionString, @"\.mdf|\.sdf|Initial\s+Catalog\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
        //        return new MySqlConnection(connectionString);
        //    throw new Exception("Could not detect an appropriate wrapper class for " + connectionString);
        //}

        //public static Connection CreateFromNativeConnection(object connection)
        //{
        //    if (connection == null)
        //        throw new Exception("Connection is null.");

        //    if (connection is System.Data.SqlClient.SqlConnection)
        //    {
        //        System.Data.SqlClient.SqlConnection c = (System.Data.SqlClient.SqlConnection)connection;
        //        if (c.State != ConnectionState.Open)
        //            c.Open();
        //        return new MsSqlConnection(c);
        //    }
        //    throw new Exception("Could not detect an appropriate wrapper class for " + ((System.Data.SqlClient.SqlConnection)connection).ConnectionString);
        //}

        protected Connection(string connectionString = null, Log.MessageType logDefaultMessageType = Log.MessageType.DEBUG)
        {
            ConnectionString = connectionString;
            LogDefaultMessageType = logDefaultMessageType;
        }

        protected Connection(System.Data.Common.DbConnection nativeConnection, Log.MessageType logDefaultMessageType = Log.MessageType.DEBUG)
        {
            NativeConnection = nativeConnection;
            ConnectionString = nativeConnection.ConnectionString;
            LogDefaultMessageType = logDefaultMessageType;
        }

        public readonly string ConnectionString;
        public Log.MessageType LogDefaultMessageType = Log.MessageType.DEBUG;

        /// <summary>
        /// Current database
        /// </summary>
        public string Database
        {
            get
            {
                return NativeConnection.Database;
            }
        }

        /// <summary>
        /// Native connection that must be casted.
        /// </summary>
        public System.Data.Common.DbConnection RefreshedNativeConnection
        {
            get
            {
                lock (sqls2command)
                {
                    return getRefreshedNativeConnection();
                }
            }
        }
        protected abstract System.Data.Common.DbConnection getRefreshedNativeConnection();

        /// <summary>
        /// Use it carefully, only when unavoidable, because it can interfere with the wraper class.
        /// </summary>
        public System.Data.Common.DbConnection NativeConnection { get; protected set; }

        /// <summary>
        /// Creates and caches/retrieves a command.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public Command this[string sql, Cliver.Log.MessageType? logMessageType = null]
        {
            get
            {
                lock (sqls2command)
                {
                    Command c;
                    if (!sqls2command.TryGetValue(sql, out c))
                    {
                        c = createCommand(sql);
                        sqls2command[sql] = c;
                    }
                    if (logMessageType != null)
                        c.LogMessageType = (Cliver.Log.MessageType)logMessageType;
                    return c;
                }
            }
        }
        abstract protected Command createCommand(string sql, Cliver.Log.MessageType? logMessageType = null);
        protected Dictionary<string, Command> sqls2command = new Dictionary<string, Command>();

        /// <summary>
        /// Creates a not cached command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Command Get(string sql, Cliver.Log.MessageType? logMessageType = null)
        {
            return createCommand(sql, logMessageType);
        }

        public void Close()
        {
            lock (this)
            {
                ClearCachedCommands();
                NativeConnection.Close();
            }
        }
    }
}