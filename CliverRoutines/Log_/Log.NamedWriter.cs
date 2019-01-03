//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliver
{
    public partial class Log
    {
        partial class Session
        {
            public class NamedWriter : Writer
            {
                NamedWriter(Session session, string name, string file_name)
                    : base(name, file_name, session)
                {
                }

                public static NamedWriter Get(Session session, string name)
                {
                    return get_named_writer(session, name);
                }

                public static bool IsDefaultOpen(Session session)
                {
                    lock (session.names2nw)
                    {
                        return session.names2nw.ContainsKey(DEFAULT_NAMED_LOG);
                    }
                }

                static NamedWriter get_named_writer(Session session, string name)
                {
                    lock (session.names2nw)
                    {
                        NamedWriter nw = null;
                        if (!session.names2nw.TryGetValue(name, out nw))
                        {
                            try
                            {
                                string log_name = Log.ProcessName + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name) + "_" + session.TimeMark + ".log";
                                nw = new NamedWriter(session, name, log_name);
                                session.names2nw.Add(name, nw);
                            }
                            catch (Exception e)
                            {
                                Cliver.Log.Main.Error(e);
                            }
                        }
                        return nw;
                    }
                }
            }
        }
    }
}