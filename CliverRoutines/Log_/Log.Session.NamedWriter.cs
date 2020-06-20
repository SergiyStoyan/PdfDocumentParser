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
            /// Main log in the session.
            /// </summary>
            public NamedWriter Noname
            {
                get
                {
                    if (_Noname == null)
                        _Noname = NamedWriter.Get(this);
                    return _Noname;
                }
            }
            NamedWriter _Noname = null;

            Dictionary<string, NamedWriter> names2NamedWriter = new Dictionary<string, NamedWriter>();

            public class NamedWriter : Writer
            {
                NamedWriter(Session session, string name)
                    : base(name, session)
                {
                }

                public const string DEFAULT_WRITER_NAME = "";
                static internal NamedWriter Get(Session session, string name = DEFAULT_WRITER_NAME)
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