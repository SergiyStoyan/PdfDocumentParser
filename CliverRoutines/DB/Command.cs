//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************

//################################
// 
// (!!!) When using a certain database type include the respective implementation files into the project and add the required reference.
//
//################################
using System;
using System.Data;
using System.Collections.Generic;

namespace Cliver.Db
{
    public abstract class Command : IDisposable
    {
        ~Command()
        {
            Dispose();
        }

        public void Dispose()
        {
            NativeCommand?.Dispose();
        }

        public readonly string Sql;
        protected readonly Connection connection;

        protected Command(string sql, Connection connection, Cliver.Log.MessageType? logMessageType = null)
        {
            this.connection = connection;
            this.Sql = sql;
            this.LogMessageType = logMessageType == null ? connection.LogDefaultMessageType : LogMessageType;
        }

        /// <summary>
        /// Use it carefully, only when unavoidable, because it can interfere with the wraper class.
        /// </summary>
        public System.Data.Common.DbCommand NativeCommand { get; protected set; }

        public int Execute(params object[] keyValuePairs)
        {
            lock (connection.RefreshedNativeConnection)
            {
                return execute(keyValuePairs);
            }
        }
        virtual protected int execute(params object[] keyValuePairs)
        {
            int rc = 0;
            try
            {
                Set(keyValuePairs);
                rc = NativeCommand.ExecuteNonQuery();
            }
            finally
            {
                NativeCommand.Parameters.Clear();
            }
            return rc;
        }

        public object GetSingleValue(params object[] keyValuePairs)
        {
            lock (connection.RefreshedNativeConnection)
            {
                return getSingleValue(keyValuePairs);
            }
        }
        virtual protected object getSingleValue(params object[] keyValuePairs)
        {
            object o = null;
            try
            {
                Set(keyValuePairs);
                o = NativeCommand.ExecuteScalar();
            }
            finally
            {
                NativeCommand.Parameters.Clear();
            }
            return o;
        }

        public IDataReader GetReader(params object[] keyValuePairs)
        {
            lock (connection.RefreshedNativeConnection)
            {
                return getReader(keyValuePairs);
            }
        }
        virtual protected IDataReader getReader(params object[] keyValuePairs)
        {
            IDataReader r = null;
            try
            {
                this.Set(keyValuePairs);
                r = NativeCommand.ExecuteReader();
            }
            finally
            {
                NativeCommand.Parameters.Clear();
            }
            return r;
        }

        public Record GetFirstRecord(params object[] keyValuePairs)
        {
            lock (connection.RefreshedNativeConnection)
            {
                return getFirstRecord(keyValuePairs);
            }
        }

        virtual protected Record getFirstRecord(params object[] keyValuePairs)
        {
            Record a = null;
            IDataReader r = null;
            try
            {
                Set(keyValuePairs);
                r = NativeCommand.ExecuteReader();
            }
            finally
            {
                NativeCommand.Parameters.Clear();
            }
            try
            {
                if (r.Read())
                {
                    a = new Record();
                    //object[] os = new object[r.FieldCount];
                    //int t = r.GetValues(os);
                    for (int i = 0; i < r.FieldCount; i++)
                        a[r.GetName(i)] = r.IsDBNull(i) ? null : r[i];
                }
            }
            finally
            {
                r.Close();
            }
            return a;
        }

        public void Set(params object[] keyValuePairs)
        {
            for (int i = 0; i < keyValuePairs.Length; i += 2)
            {
                if (keyValuePairs[i + 1] == null)
                    keyValuePairs[i + 1] = DBNull.Value;
                string k = ((string)keyValuePairs[i]).Trim();
                if (!k.StartsWith("@"))
                    keyValuePairs[i] = "@" + k;
            }
            log(keyValuePairs);
            lock (connection.RefreshedNativeConnection)
            {
                @set(keyValuePairs);
            }
        }

        abstract protected void @set(params object[] keyValuePairs);

        public Recordset GetRecordset(params object[] keyValuePairs)
        {
            lock (connection.RefreshedNativeConnection)
            {
                return getRecordset(keyValuePairs);
            }
        }
        virtual protected Recordset getRecordset(params object[] keyValuePairs)
        {
            this.Set(keyValuePairs);
            Recordset rs = new Recordset(NativeCommand.ExecuteReader());
            return rs;
        }

        public string CommandText
        {
            get
            {
                lock (connection.RefreshedNativeConnection)
                {
                    return NativeCommand.CommandText;
                }
            }
        }

        public Cliver.Log.MessageType LogMessageType = Log.MessageType.DEBUG;

        void log(params object[] keyValuePairs)
        {
            if (!Log.Main.Is2BeLogged(LogMessageType))
                return;
            string m = "[" + connection.Database + "]...:\r\n" + Sql;
            List<string> kvs = new List<string>();
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                kvs.Add(keyValuePairs[i].ToString() + "=" + keyValuePairs[i + 1].ToString());
            if (kvs.Count > 0)
                m += "\r\n" + string.Join("\r\n", kvs);
            Cliver.Log.Write(LogMessageType, m);
            //Cliver.Log.Inform("[" + connection.Database + "]:\r\n" + NativeCommand.CommandText);
            //logNext = false;
        }
    }
}