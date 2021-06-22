//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        partial class Session
        {
            /// <summary>
            /// Get log by name.
            /// It will be created if not exists.
            /// </summary>
            /// <param name="name">log name</param>
            /// <returns>named log</returns>
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

            /// <summary>
            /// Named log
            /// </summary>
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
                        if (!session.names2NamedWriter.TryGetValue(name, out NamedWriter nw))
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