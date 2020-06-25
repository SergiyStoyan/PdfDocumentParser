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
using System.IO;
using System.Linq;

namespace Cliver
{
    public static partial class Log
    {
        public partial class Session
        {
            Session(string name)
            {
                this.name = name;
                CreatedTime = DateTime.MinValue;
                timeMark = null;
                string dir = Dir;//initialize
            }

            string getDir(string name)
            {
                lock (this.names2NamedWriter)
                {
                    string dir;
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                            dir = WorkDir;
                            break;
                        case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                            string dir0 = WorkDir + System.IO.Path.DirectorySeparatorChar + NamePrefix + "_" + TimeMark + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name);
                            dir = dir0;
                            for (int count = 1; Directory.Exists(dir); count++)
                                dir = dir0 + "_" + count.ToString();
                            break;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                    return dir;
                }
            }

            public static string NamePrefix = "Session";

            public string Name
            {
                get
                {
                    lock (this.names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        return name;
                    }
                }
            }
            string name;

            public string Dir
            {
                get
                {
                    lock (this.names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        if (dir == null)
                            dir = getDir(name);
                        return dir;
                    }
                }
            }
            string dir;

            public DateTime CreatedTime { get; protected set; }
            public string TimeMark {
                get
                {
                    lock (names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        if (timeMark == null)
                        {
                            CreatedTime = DateTime.Now;
                            timeMark = CreatedTime.ToString("yyMMddHHmmss");
                        }
                        return timeMark;
                    }
                }
            }
            string timeMark = null;

            /// <summary>
            /// Default log of the session.
            /// Depending on condition THREAD_LOG_IS_DEFAULT, it is either Main log or Thread log.
            /// </summary>
            public Writer Default
            {
                get
                {
#if THREAD_LOG_IS_DEFAULT
                    return Thread;
#else
                    return Main;
#endif
                }
            }

            /// <summary>
            /// Close all log files in the session.  
            /// Nevertheless the session can be re-used after.
            /// </summary>
            /// <param name="newName"></param>
            public void Rename(string newName)
            {
                lock (this.names2NamedWriter)
                {
                    if (newName == Name)
                        return;

                    Write("Renaming session...: '" + Name + "' to '" + newName + "'");

                    Close(true);
                    string newDir = getDir(newName);
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                            dir = newDir;
                            foreach (Writer w in names2NamedWriter.Values.Select(a => (Writer)a).Concat(threads2treadWriter.Values))
                            {
                                string file0 = w.File;
                                w.SetFile();
                                if (File.Exists(file0))
                                    File.Move(file0, w.File);
                            }
                            break;
                        case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                            if (Directory.Exists(dir))
                                Directory.Move(dir, newDir);
                            dir = newDir;
                            break;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                    lock (names2Session)
                    {
                        names2Session.Remove(name);
                        name = newName;
                        names2Session[name] = this;
                    }
                }
            }

            /// <summary>
            /// Close all log files in the session. 
            /// </summary>
            /// <param name="reuse">if true, the same session folder can be used again, otherwise a new folder will be created for this session</param>
            public void Close(bool reuse)
            {
                lock (names2NamedWriter)
                {
                    if (names2NamedWriter.Values.Count < 1 && threads2treadWriter.Values.Count < 1)
                        return;

                    Write("Closing the log session...");

                    foreach (NamedWriter nw in names2NamedWriter.Values)
                        nw.Close();
                    //names2NamedWriter.Clear(); !!! clearing writers will bring to duplicating of them if they are referenced in the custom code.

                    lock (threads2treadWriter)
                    {
                        foreach (ThreadWriter tw in threads2treadWriter.Values)
                            tw.Close();
                        //threads2treadWriter.Clear(); !!!clearing writers will bring to duplicating of them if they are referenced in the custom code.
                    }

                    if (!reuse)
                    {
                        dir = null;
                        CreatedTime = DateTime.MinValue;
                        timeMark = null;
                    }
                }
            }
        }
    }
}