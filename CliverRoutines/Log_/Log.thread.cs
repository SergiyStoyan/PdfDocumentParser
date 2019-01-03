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
    public static partial class Log
    {
        public class Thread
        {
            static Thread()
            {
                //to avoid looping if calling 
                Log.DeleteOldLogs();
            }

            Thread(int id, string log_file)
            {
                this.Id = id;
                this.path = log_file;
            }

            public static int MaxFileSize = -1;

            const int MAIN_THREAD_LOG_ID = -1;

            static Dictionary<System.Threading.Thread, Thread> thread2tls = new Dictionary<System.Threading.Thread, Thread>();

            static Thread get_thread_log(System.Threading.Thread thread)
            {
                lock (thread2tls)
                {
                    Log.Thread tl;
                    if (!thread2tls.TryGetValue(thread, out tl))
                    {
                        try
                        {
                            //cleanup for dead thread logs
                            List<System.Threading.Thread> old_log_keys = (from t in thread2tls.Keys where !t.IsAlive select t).ToList();
                            foreach (System.Threading.Thread t in old_log_keys)
                            {
                                if (t.ThreadState != System.Threading.ThreadState.Stopped)
                                {
                                    thread2tls[t].Error("This thread is detected as not alive. Aborting it...");
                                    t.Abort();
                                }
                                thread2tls[t].Close();
                                thread2tls.Remove(t);
                            }

                            int log_id;
                            if (thread == Log.MainThread)
                                log_id = MAIN_THREAD_LOG_ID;
                            else
                            {
                                log_id = 1;
                                var ids = from x in thread2tls.Keys orderby thread2tls[x].Id select thread2tls[x].Id;
                                foreach (int id in ids)
                                    if (log_id == id) log_id++;
                            }

                            string log_file;
                            switch (Log.MODE)
                            {
                                case Log.Mode.ONLY_LOG:
                                    log_file = Log.WorkDir + @"\" + Log.EntryAssemblyName;
                                    break;
                                case Log.Mode.SESSIONS:
                                    log_file = Log.SessionDir + @"\" + Log.EntryAssemblyName;
                                    break;
                                default:
                                    throw new Exception("Unknown LOGGING_MODE:" + Log.MODE);
                            }
                            if (log_id < 0)
                                log_file += "_" + Log.TimeMark + ".log";
                            else
                                log_file += "_" + log_id.ToString() + "_" + Log.TimeMark + ".log";

                            tl = new Thread(log_id, log_file);
                            thread2tls.Add(thread, tl);
                        }
                        catch (Exception e)
                        {
                            Log.Main.Error(e);
                        }
                    }
                    return tl;
                }
            }

            /// <summary>
            /// Log belonging to the first (main) thread of the process.
            /// </summary>
            public static Log.Thread Main
            {
                get
                {
                    return get_thread_log(Log.MainThread);
                }
            }

            /// <summary>
            /// Log beloning to the current thread.
            /// </summary>
            public static Thread This
            {
                get
                {
                    return get_thread_log(System.Threading.Thread.CurrentThread);
                }
            }

            public static void CloseAll()
            {
                lock (thread2tls)
                {
                    foreach (Thread tl in thread2tls.Values)
                        tl.Close();
                    thread2tls.Clear();

                    exiting_thread = null;
                }
            }

            public static int TotalErrorCount
            {
                get
                {
                    lock (thread2tls)
                    {
                        int ec = 0;
                        foreach (Thread tl in thread2tls.Values)
                            ec += tl.ErrorCount;
                        return ec;
                    }
                }
            }

            /// <summary>
            /// Log id that is used for logging and browsing in GUI
            /// </summary>
            public readonly int Id;

            /// <summary>
            /// Log path
            /// </summary>
            public string Path
            {
                get
                {
                    return path;
                }
            }
            public string path;

            /// <summary>
            /// Used to close Log
            /// </summary>
            public void Close()
            {
                lock (this)
                {
                    if (log_writer != null)
                        log_writer.Close();
                    log_writer = null;
                    _ErrorCount = 0;
                }
            }

            /// <summary>
            /// Write the error to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Error(Exception e)
            {
                string m;
                string d;
                Log.GetExceptionMessage(e, out m, out d);
                Write(Log.MessageType.ERROR, m, d);
            }

            /// <summary>
            /// Write the error to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Error(string message)
            {
                Write(Log.MessageType.ERROR, message, Log.GetStackString());
            }

            public void Error2(string message)
            {
                Write(Log.MessageType.ERROR, message);
            }

            /// <summary>
            /// Write the stack informtion for the caller to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Trace(object message = null)
            {
                if (message != null)
                    Write(Log.MessageType.TRACE, message.ToString(), Log.GetStackString());
                else
                    Write(Log.MessageType.TRACE, null, Log.GetStackString());
            }

            /// <summary>
            /// Write the error to the current thread's log and terminate the process.
            /// </summary>
            /// <param name="e"></param>
            public void Exit(string message)
            {
                lock (this)
                {
                    if (Id >= 0)
                        Log.Main.Write("EXITING: due to thread #" + Id.ToString() + ". See the respective Log");
                    Write(Log.MessageType.EXIT, message, Log.GetStackString());
                }
            }

            public void Exit2(string message)
            {
                lock (this)
                {
                    if (Id >= 0)
                        Log.Main.Write("EXITING: due to thread #" + Id.ToString() + ". See the respective Log");
                    Write(Log.MessageType.EXIT, message);
                }
            }

            /// <summary>
            /// Write the error to the current thread's log and terminate the process.
            /// </summary>
            /// <param name="e"></param>
            public void Exit(Exception e)
            {
                lock (this)
                {
                    if (Id >= 0)
                        Log.Main.Write("EXITING: due to thread #" + Id.ToString() + ". See the respective Log");
                    string m;
                    string d;
                    Log.GetExceptionMessage(e, out m, out d);
                    Write(Log.MessageType.EXIT, m, d);
                }
            }

            public delegate void OnExitig(string message);
            static public event OnExitig Exitig = null;

            /// <summary>
            /// Write the warning to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Warning(string message)
            {
                Write(Log.MessageType.WARNING, message);
            }

            /// <summary>
            /// Write the exception as warning to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Warning(Exception e)
            {
                string m;
                string d;
                Log.GetExceptionMessage(e, out m, out d);
                Write(Log.MessageType.WARNING, m, d);
            }

            /// <summary>
            /// Write the notification to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Inform(string message)
            {
                Write(Log.MessageType.INFORM, message);
            }

            public void Write(string line)
            {
                Write(Log.MessageType.LOG, line);
            }

            /// <summary>
            /// Write the message to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Write(Log.MessageType type, string message, string details = null)
            {
                lock (this)
                {
                    if (type == Log.MessageType.EXIT)
                    {
                        if (exiting_thread != null)
                            return;
                        exiting_thread = new System.Threading.Thread(() =>
                        {
                            if (Exitig != null)
                                Exitig.Invoke(message);
                            write(type, message, details);
                            Environment.Exit(0);
                        });
                        exiting_thread.IsBackground = true;
                        exiting_thread.Start();
                    }
                    else
                        write(type, message, details);
                }
            }
            void write(Log.MessageType type, string message, string details)
            {
                lock (this)
                {
                    if (Writing != null)
                        Writing.Invoke(type, message, details);

                    if (Log.write_log)
                    {
                        if (log_writer == null)
                            log_writer = new StreamWriter(Path, true);

                        details = string.IsNullOrWhiteSpace(details) ? "" : "\r\n\r\n" + details;

                        switch (type)
                        {
                            case Log.MessageType.INFORM: message = "INFORM: " + message; break;
                            case Log.MessageType.WARNING: message = "WARNING: " + message; break;
                            case Log.MessageType.ERROR:
                                message = "ERROR: " + message + details;
                                _ErrorCount++;
                                break;
                            case Log.MessageType.EXIT:
                                message = "EXIT: " + message + details;
                                _ErrorCount++;
                                break;
                            case Log.MessageType.TRACE: message = "TRACE: " + message; break;
                            case Log.MessageType.LOG: break;
                            default: throw new Exception("No case for " + type.ToString());
                        }

                        if (MaxFileSize > 0)
                        {
                            FileInfo fi = new FileInfo(Path);
                            if (fi.Length > MaxFileSize)
                            {
                                log_writer.Close();

                                int counter = 0;
                                path = Regex.Replace(path, @"(\d+_)(\d+)(\.[^\.]+)$",
                                    (Match m) =>
                                    {
                                        counter = int.Parse(m.Groups[2].Value) + 1;
                                        return m.Groups[1].Value + counter + m.Groups[3].Value;
                                    },
                                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
                                    );
                                if (counter < 1)
                                    path = Regex.Replace(path, @"\.[^\.]+$", @"_1$0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                                log_writer = new StreamWriter(Path, true);
                            }
                        }
                        //if (!string.IsNullOrWhiteSpace(details))
                        //    message += "\r\n\r\n" + details;
                        log_writer.WriteLine(DateTime.Now.ToString("[dd-MM-yy HH:mm:ss] ") + message);
                        log_writer.Flush();
                    }
                }
            }
            TextWriter log_writer = null;
            static System.Threading.Thread exiting_thread = null;

            public int ErrorCount
            {
                get
                {
                    return _ErrorCount;
                }
            }
            int _ErrorCount = 0;

            public delegate void OnWrite(Log.MessageType type, string message, string details);
            static public event OnWrite Writing = null;
        }
    }
}