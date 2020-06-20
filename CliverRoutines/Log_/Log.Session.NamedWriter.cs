//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        partial class Session
        {
            public NamedWriter this[string name]
            {
                get
                {
                    return NamedWriter.Get(this, name);
                }
            }

            /// <summary>
            /// Main log of the session.
            /// </summary>
            public NamedWriter Main
            {
                get
                {
                    if (_Main == null)
                        _Main = NamedWriter.Get(this, MAIN_WRITER_NAME);
                    return _Main;
                }
            }
            NamedWriter _Main = null;
            public const string MAIN_WRITER_NAME = "";

            Dictionary<string, NamedWriter> names2NamedWriter = new Dictionary<string, NamedWriter>();

            public class NamedWriter : Writer
            {
                NamedWriter(Session session, string name)
                    : base(name, session)
                {
                }

                static internal NamedWriter Get(Session session, string name)
                {
                    lock (session.names2NamedWriter)
                    {
                        NamedWriter nw = null;
                        if (!session.names2NamedWriter.TryGetValue(name, out nw))
                        {
                            nw = new NamedWriter(session, name);
                            session.names2NamedWriter.Add(name, nw);
                        }
                        return nw;
                    }
                }
            }
        }
    }
}