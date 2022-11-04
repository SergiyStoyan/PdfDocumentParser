//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
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
            /// <summary>
            /// Get log for this thread.
            /// It will be created if not exists.
            /// </summary>
            /// <returns>thread log</returns>
            public ThreadWriter Thread
            {
                get
                {
                    return ThreadWriter.Get(this, System.Threading.Thread.CurrentThread);
                }
            }

            Dictionary<int, ThreadWriter> threadIds2TreadWriter = new Dictionary<int, ThreadWriter>();

            int threadWriterCounter = 0;

            /// <summary>
            /// Thread log.
            /// </summary>
            public partial class ThreadWriter : Writer
            {
                ThreadWriter(Session session, int id, System.Threading.Thread thread)
                    : base(id.ToString(), session)
                {
                    Id = id;
                    Thread = thread;
                }

                /// <summary>
                /// Log ID.
                /// </summary>
                public readonly int Id = 0;

                /// <summary>
                /// Log thread.
                /// </summary>
                public readonly System.Threading.Thread Thread;

                static internal ThreadWriter Get(Session session, System.Threading.Thread thread)
                {
                    lock (session.threadIds2TreadWriter)
                    {
                        ThreadWriter tw;
                        if (!session.threadIds2TreadWriter.TryGetValue(thread.ManagedThreadId, out tw))
                        {
                            //cleanup for dead thread logs
                            List<System.Threading.Thread> oldLogThreads = session.threadIds2TreadWriter.Values.Where(a => !a.Thread.IsAlive).Select(a => a.Thread).ToList();
                            foreach (System.Threading.Thread t in oldLogThreads)
                            {
                                session.threadIds2TreadWriter[t.ManagedThreadId].Close();
                                session.threadIds2TreadWriter.Remove(t.ManagedThreadId);
                            }

                            session.threadWriterCounter++;
                            int logId;
                            if (Log.ReuseThreadLogIndexes)
                            {
                                logId = 1;
                                var ids = session.threadIds2TreadWriter.Values.OrderBy(a => a.Id).Select(a => a.Id);
                                foreach (int id in ids)
                                    if (logId == id)
                                        logId++;
                            }
                            else
                                logId = session.threadWriterCounter;

                            tw = new ThreadWriter(session, logId, thread);
                            session.threadIds2TreadWriter.Add(thread.ManagedThreadId, tw);
                        }
                        return tw;
                    }
                }
            }
        }
    }
}