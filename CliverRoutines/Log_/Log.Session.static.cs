//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;

namespace Cliver
{
    public static partial class Log
    {
        public partial class Session
        {
            static Dictionary<string, Session> names2Session = new Dictionary<string, Session>();

            internal static void CloseAll()
            {
                lock (names2Session)
                {
                    foreach (Session s in names2Session.Values)
                        s.Close(false);
                    names2Session.Clear();
                }
            }

            public static Session Get(string sessionName)
            {
                lock (names2Session)
                {
                    if (!names2Session.TryGetValue(sessionName, out Session s))
                    {
                        s = new Session(sessionName);
                        names2Session[sessionName] = s;
                    }
                    return s;
                }
            }
        }
    }
}