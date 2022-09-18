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

namespace Cliver.Db
{
    public class MsSqlCommand : Command
    {
        public MsSqlCommand(string sql, Connection connection, Cliver.Log.MessageType? logMessageType = null)
            : base(sql, connection, logMessageType)
        {
            command = new SqlCommand(sql, (SqlConnection)connection.RefreshedNativeConnection);
            command.Prepare();
            NativeCommand = command;
        }

        SqlCommand command;

        override protected void @set(params object[] keyValuePairs)
        {
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                command.Parameters.AddWithValue((string)keyValuePairs[i], keyValuePairs[i + 1]);
        }
    }
}