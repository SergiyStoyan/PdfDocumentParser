//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Cliver.Db
{
    public class MsSqlConnection : Connection
    {
        public MsSqlConnection(string connectionString = null, Log.MessageType logDefaultMessageType = Log.MessageType.DEBUG)
            : base(connectionString, logDefaultMessageType)
        {
            NativeConnection = new SqlConnection(ConnectionString);
        }

        public MsSqlConnection(SqlConnection NativeConnection, Log.MessageType logDefaultMessageType = Log.MessageType.DEBUG)
            : base(NativeConnection, logDefaultMessageType)
        {
        }

        override protected System.Data.Common.DbConnection getRefreshedNativeConnection()
        {
            if (NativeConnection.State != ConnectionState.Open)
            {
                //if (NativeConnection.State != ConnectionState.Closed)
                //{
                //    NativeConnection.Dispose();
                //    NativeConnection = new SqlConnection(NativeConnection.ConnectionString);
                //}
                NativeConnection.Open();
                Dictionary<string, Command> s2cs = new Dictionary<string, Command>();
                foreach (string sql in sqls2command.Keys)
                    s2cs[sql] = new MsSqlCommand(sql, this);
                sqls2command = s2cs;
            }
            return NativeConnection;
        }

        override protected Command createCommand(string sql, Cliver.Log.MessageType? logMessageType = null)
        {
            var c = new MsSqlCommand(sql, this, logMessageType);
            return c;
        }
    }
}