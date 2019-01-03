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
        public class ThreadWriter : Writer
        {
            ThreadWriter(int id, string file_name)
                : base(id.ToString(), file_name, Log.MainSession)
            {
                this.Id = id;
            }
            
            internal const int MAIN_THREAD_LOG_ID = -1;

            /// <summary>
            /// Log belonging to the first (main) thread of the process.
            /// </summary>
            public static ThreadWriter Main
            {
                get
                {
                    return get_thread_writer(Log.MainThread);
                }
            }

            public static bool IsAnythingOpen
            {
                get
                {
                    lock (thread2tws)
                    {
                        return thread2tws.Count > 0;
                    }
                }
            }

            ///// <summary>
            ///// Log belonging to the first (main) thread of the process.
            ///// </summary>
            //public static ThreadWriter Main
            //{
            //    get
            //    {
            //        return Session.Main;
            //    }
            //}

            ///// <summary>
            ///// Log beloning to the current thread.
            ///// </summary>
            //public static ThreadWriter This
            //{
            //    get
            //    {
            //        return get_log_thread(System.Threading.Thread.CurrentThread);
            //    }
            //}

            /// <summary>
            /// Log beloning to the current thread.
            /// </summary>
            public static ThreadWriter This
            {
                get
                {
                    return get_thread_writer(System.Threading.Thread.CurrentThread);
                }
            }

            public static int TotalErrorCount
            {
                get
                {
                    lock (thread2tws)
                    {
                        int ec = 0;
                        foreach (Writer tl in thread2tws.Values)
                            ec += tl.ErrorCount;
                        return ec;
                    }
                }
            }

            public static void CloseAll()
            {
                lock (thread2tws)
                {
                    foreach (Writer tl in thread2tws.Values)
                        tl.Close();
                    thread2tws.Clear();

                    exiting_thread = null;
                }
            }

            /// <summary>
            /// Log id that is used for logging and browsing in GUI
            /// </summary>
            public readonly int Id = MAIN_THREAD_LOG_ID;

            static Dictionary<System.Threading.Thread, ThreadWriter> thread2tws = new Dictionary<System.Threading.Thread, ThreadWriter>();

            static ThreadWriter get_thread_writer(System.Threading.Thread thread)
            {
                lock (thread2tws)
                {
                    ThreadWriter tw;
                    if (!thread2tws.TryGetValue(thread, out tw))
                    {
                        try
                        {
                            //cleanup for dead thread logs
                            List<System.Threading.Thread> old_log_keys = (from t in thread2tws.Keys where !t.IsAlive select t).ToList();
                            foreach (System.Threading.Thread t in old_log_keys)
                            {
                                if (t.ThreadState != System.Threading.ThreadState.Stopped)
                                {
                                    thread2tws[t].Error("This thread is detected as not alive. Aborting it...");
                                    t.Abort();
                                }
                                thread2tws[t].Close();
                                thread2tws.Remove(t);
                            }

                            int log_id;
                            if (thread == Log.MainThread)
                                log_id = MAIN_THREAD_LOG_ID;
                            else
                            {
                                log_id = 1;
                                var ids = from x in thread2tws.Keys orderby thread2tws[x].Id select thread2tws[x].Id;
                                foreach (int id in ids)
                                    if (log_id == id)
                                        log_id++;
                            }

                            string log_name = Log.ProcessName;
                            if (log_id < 0)
                                log_name += "_" + MainSession.TimeMark + ".log";
                            else
                                log_name += "_" + MainSession.TimeMark + "_" + log_id.ToString() + ".log";

                            tw = new ThreadWriter(log_id, log_name);
                            thread2tws.Add(thread, tw);
                        }
                        catch (Exception e)
                        {
                            Log.Main.Error(e);
                        }
                    }
                    return tw;
                }
            }
        }
    }
}