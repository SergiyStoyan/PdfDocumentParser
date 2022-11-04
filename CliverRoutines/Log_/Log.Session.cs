//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cliver
{
    public static partial class Log
    {
        /// <summary>
        /// Log session.
        /// </summary>
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
                string dir;
                if (Log.mode.HasFlag(Mode.FOLDER_PER_SESSION))
                {
                    //string dir0 = RootDir + System.IO.Path.DirectorySeparatorChar + (string.IsNullOrEmpty(NamePrefix) ? "" : NamePrefix + "_") + (string.IsNullOrWhiteSpace(name) ? "" : name + "_") + TimeMark;
                    string dir0 = Log.RootDir + System.IO.Path.DirectorySeparatorChar + NamePrefix + TimeMark + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name);
                    dir = dir0;
                    for (int count = 1; Directory.Exists(dir); count++)
                        dir = dir0 + "_" + count.ToString();
                }
                else //if (Log.mode.HasFlag(Mode.ONE_FOLDER))//default
                {
                    dir = Log.RootDir;
                }
                return dir;
            }

            /// <summary>
            /// Prefix to the session folder name.
            /// </summary>
            public static string NamePrefix = "";

            /// <summary>
            /// Session name.
            /// </summary>
            public string Name
            {
                get
                {
                    lock (names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        return name;
                    }
                }
            }
            string name;

            /// <summary>
            /// Session directory.
            /// </summary>
            public string Dir
            {
                get
                {
                    lock (names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        if (dir == null)
                            dir = getDir(name);
                        return dir;
                    }
                }
            }
            string dir;

            /// <summary>
            /// Time when the session was created.
            /// </summary>
            public DateTime CreatedTime { get; protected set; }

            /// <summary>
            /// Time mark in the session directory of log names.
            /// </summary>
            public string TimeMark
            {
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
            /// Depending on Mode, it is either Main log or Thread log.
            /// </summary>
            public Writer Default
            {
                get
                {
                    if (mode.HasFlag(Mode.DEFAULT_THREAD_LOG))
                        return Thread;
                    else
                        return Main;
                }
            }

            /// <summary>
            /// Close all log files in the session.  
            /// Nevertheless the session can be re-used after.
            /// Make sure that during this call no log of this session is used.
            /// </summary>
            /// <param name="newName">new name</param>
            /// <param name="tryMaxCount">number of attempts if the session foldr is locked</param>
            /// <param name="tryDelayMss">time span between attempts</param>
            public void Rename(string newName, int tryMaxCount = 10, int tryDelayMss = 50)
            {
                lock (names2NamedWriter)
                {
                    lock (threadIds2TreadWriter)
                    {
                        if (newName == Name)
                            return;

                        Write("Renaming session...: '" + Name + "' to '" + newName + "'");

                        lock (names2Session)
                        {
                            if (names2Session.ContainsKey(newName))
                                throw new Exception("Such session already exists.");

                            Close(true);

                            names2Session.Remove(name);
                            name = newName;
                            names2Session[name] = this;
                        }
                        string newDir = getDir(newName);

                        if (Log.mode.HasFlag(Mode.FOLDER_PER_SESSION))
                        {
                            for (int tryCount = 1; ; tryCount++)
                                try
                                {
                                    if (Directory.Exists(dir))
                                        Directory.Move(dir, newDir);
                                    break;
                                }
                                catch (Exception e)//if another thread calls a log in this session then it locks the folder against renaming
                                {
                                    if (tryCount >= tryMaxCount)
                                        throw new Exception("Could not rename log session.", e);
                                    Error(e);
                                    System.Threading.Thread.Sleep(tryDelayMss);
                                }
                            dir = newDir;
                            foreach (Writer w in names2NamedWriter.Values.Select(a => (Writer)a).Concat(threadIds2TreadWriter.Values))
                                w.SetFile();
                        }
                        else //if (Log.mode.HasFlag(Mode.ONE_FOLDER))//default
                        {
                            dir = newDir;
                            foreach (Writer w in names2NamedWriter.Values.Select(a => (Writer)a).Concat(threadIds2TreadWriter.Values))
                            {
                                lock (w)
                                {
                                    w.Close();
                                    string file0 = w.File;
                                    w.SetFile();
                                    if (File.Exists(file0))
                                        File.Move(file0, w.File);
                                }
                            }
                        }
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
                    lock (threadIds2TreadWriter)
                    {
                        if (names2NamedWriter.Values.FirstOrDefault(a => !a.IsClosed) == null && threadIds2TreadWriter.Values.FirstOrDefault(a => !a.IsClosed) == null)
                            return;

                        Write("Closing the log session...");

                        foreach (NamedWriter nw in names2NamedWriter.Values)
                            nw.Close();
                        //names2NamedWriter.Clear(); !!! clearing writers will bring to duplicating them if they are referenced in the calling code.

                        foreach (ThreadWriter tw in threadIds2TreadWriter.Values)
                            tw.Close();
                        //threadIds2TreadWriter.Clear(); !!!clearing writers will bring to duplicating them if they are referenced in the calling code.

                        if (!reuse)
                        {
                            dir = null;
                            CreatedTime = DateTime.MinValue;
                            timeMark = null;
                            //names2Session.Remove(name);!!!removing session will bring to duplicating it if it is referenced in the calling code.
                        }
                    }
                }
            }
        }
    }
}