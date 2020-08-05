//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        public partial class Session
        {
            public ThreadWriter Thread
            {
                get
                {
                    return ThreadWriter.Get(this, System.Threading.Thread.CurrentThread);
                }
            }

            Dictionary<System.Threading.Thread, ThreadWriter> threads2treadWriter = new Dictionary<System.Threading.Thread, ThreadWriter>();

            int threadWriterCounter = 0;

            public class ThreadWriter : Writer
            {
                ThreadWriter(Session session, int id)
                    : base(id.ToString(), session)
                {
                    this.Id = id;
                }

                /// <summary>
                /// Log id that is used for logging and browsing in GUI
                /// </summary>
                public readonly int Id = 0;

                static internal ThreadWriter Get(Session session, System.Threading.Thread thread)
                {
                    lock (session.threads2treadWriter)
                    {
                        ThreadWriter tw;
                        if (!session.threads2treadWriter.TryGetValue(thread, out tw))
                        {
                            //cleanup for dead thread logs
                            List<System.Threading.Thread> oldLogThreads = (from t in session.threads2treadWriter.Keys where !t.IsAlive select t).ToList();
                            foreach (System.Threading.Thread t in oldLogThreads)
                            {
                                if (t.ThreadState != System.Threading.ThreadState.Stopped)
                                {
                                    session.threads2treadWriter[t].Error("This thread is detected as not alive. Aborting it...");
                                    t.Abort();
                                }
                                session.threads2treadWriter[t].Close();
                                session.threads2treadWriter.Remove(t);
                            }

                            session.threadWriterCounter++;
                            int logId;
                            if (Log.ReuseThreadLogIndexes)
                            {
                                logId = 1;
                                var ids = session.threads2treadWriter.Values.OrderBy(a => a.Id).Select(a => a.Id);
                                foreach (int id in ids)
                                    if (logId == id)
                                        logId++;
                            }
                            else
                                logId = session.threadWriterCounter;

                            tw = new ThreadWriter(session, logId);
                            session.threads2treadWriter.Add(thread, tw);
                        }
                        return tw;
                    }
                }
            }
        }
    }
}