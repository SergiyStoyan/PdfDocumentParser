//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace Cliver.Db
{
    public class DbConnection
    {
        //static DbConnection()
        //{
        //    This = Create();
        //}

        DbConnection()
        {
        }

        public readonly static DbConnection This;

        public static DbConnection Create(string connection_string = null)
        {
            if (connection_string == null)
                throw new Exception("connection_string is null.");

            if (Regex.IsMatch(connection_string, @"\.mdf|\.sdf|Initial\s+Catalog\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline))
                return new MsSqlConnection(connection_string);
            throw new Exception("Could not detect an appropriate wrapper class for " + connection_string);
        }

        public static DbConnection CreateFromNativeConnection(object connection)
        {
            if (connection == null)
                throw new Exception("Connection is null.");

            if (connection is System.Data.SqlClient.SqlConnection)
            {
                System.Data.SqlClient.SqlConnection c = (System.Data.SqlClient.SqlConnection)connection;
                if (c.State != ConnectionState.Open)
                    c.Open();
                return new MsSqlConnection(c);
            }
            throw new Exception("Could not detect an appropriate wrapper class for " + ((System.Data.SqlClient.SqlConnection)connection).ConnectionString);
        }

        protected DbConnection(string connection_string = null)
        {
            this.ConnectionString = connection_string;
        }

        protected DbConnection(System.Data.Common.DbConnection connection)
        {
            this.native_connection = connection;
        }

        public readonly string ConnectionString;

        /// <summary>
        /// Current database
        /// </summary>
        public string Database
        {
            get
            {
                return get_database();
            }
        }
        virtual protected string get_database() { throw new Exception("Stub method is not overriden"); }

        /// <summary>
        /// Native connection that must be casted.
        /// </summary>
        public object NativeConnection
        {
            get
            {
                lock (sqls2commands)
                {
                    return get_refreshed_native_connection();
                }
            }
        }
        protected virtual object get_refreshed_native_connection() { return null; }
        protected object native_connection;

        /// <summary>
        /// Creates and caches a command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Command this[string sql]
        {
            get
            {
                lock (sqls2commands)
                {
                    Command c;
                    if (!sqls2commands.TryGetValue(sql, out c))
                    {
                        c = create_command(sql);
                        sqls2commands[sql] = c;
                    }
                    return c;
                }
            }
        }
        virtual internal Command create_command(string sql) { throw new Exception("Stub method is not overriden"); }
        protected Dictionary<string, Command> sqls2commands = new Dictionary<string, Command>();

        /// <summary>
        /// Creates a not cached command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Command Get(string sql)
        {
            return create_command(sql);
        }
    }

    public class MsSqlConnection : DbConnection
    {
        public MsSqlConnection(string connection_string = null)
            : base(connection_string)
        {
        }

        public MsSqlConnection(System.Data.SqlClient.SqlConnection connection)
            : base(connection)
        {
        }        

        override protected object get_refreshed_native_connection()
        {
            SqlConnection c = (SqlConnection)native_connection;
            if (c == null)
            {
                c = new SqlConnection(ConnectionString);
                native_connection = c;
                c.Open();
            }
            if (c.State != ConnectionState.Open)
            {
                c.Dispose();
                c = new SqlConnection(c.ConnectionString);
                native_connection = c;
                c.Open();
                Dictionary<string, Command> s2cs = new Dictionary<string, Command>();
                foreach (string sql in sqls2commands.Keys)
                    s2cs[sql] = new MsSqlCommand(sql, this);
                sqls2commands = s2cs;
            }
            return c;
        }

        override internal Command create_command(string sql)
        {
            return new MsSqlCommand(sql, this);
        }

        override protected string get_database()
        {
            return ((SqlConnection)native_connection).Database;
        }
    }
}

        //public OdbcConnection Odbc
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (oc == null
        //                || oc.State != ConnectionState.Open
        //                || oc.State == ConnectionState.Broken
        //                || oc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    oc = new OdbcConnection(ConnectionStr);
        //                    oc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return oc;
        //        }
        //    }
        //}
        //OdbcConnection oc = null;

        //public OleDbConnection OleDb
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (lc == null
        //                || lc.State != ConnectionState.Open
        //                || lc.State == ConnectionState.Broken
        //                || lc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    lc = new OleDbConnection(ConnectionStr);
        //                    lc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return lc;
        //        }
        //    }
        //}
        //OleDbConnection lc = null;
        
        //public MySqlConnection MySql
        //{
        //    get
        //    {
        //        lock (lock_variable)
        //        {
        //            if (mc == null
        //                || mc.State != ConnectionState.Open
        //                || mc.State == ConnectionState.Broken
        //                || mc.State == ConnectionState.Closed
        //                )
        //            {
        //                try
        //                {
        //                    mc = new MySqlConnection(ConnectionStr);
        //                    mc.Open();
        //                }
        //                catch (Exception e)
        //                {
        //                    LogMessage.Exit(e);
        //                }
        //            }
        //            return mc;
        //        }
        //    }
        //}
        //MySqlConnection mc = null;
