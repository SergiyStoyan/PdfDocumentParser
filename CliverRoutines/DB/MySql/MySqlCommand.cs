//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Cliver.Db
{
    public class MySqlCommand : Command
    {
        public MySqlCommand(string sql, Connection connection, Cliver.Log.MessageType? logMessageType = null)
            : base(sql, connection,  logMessageType)
        {
            command = new MySql.Data.MySqlClient.MySqlCommand(sql, (MySql.Data.MySqlClient.MySqlConnection)connection.RefreshedNativeConnection);
            //command.Prepare();!!!gives error 'Parameter ... was not found during prepare.' when a command contains parameter
            NativeCommand = command;
        }

        MySql.Data.MySqlClient.MySqlCommand command;

        override protected void @set(params object[] keyValuePairs)
        {
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                command.Parameters.AddWithValue((string)keyValuePairs[i], keyValuePairs[i + 1]);
        }
    }
}