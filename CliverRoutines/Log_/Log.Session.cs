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
                string path = Path;//initialize
            }

            string getPath(string name)
            {
                lock (this.names2NamedWriter)
                {
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                            return WorkDir;
                        //case Cliver.Log.Mode.SINGLE_SESSION:
                        case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                            string path0 = WorkDir + System.IO.Path.DirectorySeparatorChar + NamePrefix + "_" + TimeMark + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name);
                            string path = path0;
                            for (int count = 1; Directory.Exists(path); count++)
                                path = path0 + "_" + count.ToString();
                            return path;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
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

            public string Path
            {
                get
                {
                    lock (this.names2NamedWriter)//this lock is needed if Session::Close(string new_name) is being performed
                    {
                        if (path == null)
                        {
                            path = getPath(name);
                            if (writeLog)
                                Directory.CreateDirectory(path);
                        }
                        return path;
                    }
                }
            }
            string path;

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
            /// Close all log files in the session.  
            /// Nevertheless the session can be called and used again.
            /// </summary>
            /// <param name="newName"></param>
            public void Rename(string newName)
            {
                lock (this.names2NamedWriter)
                {
                    if (Log.mode == Mode.ALL_LOGS_ARE_IN_SAME_FOLDER)
                        throw new Exception("Cannot rename log folder in mode: " + Log.mode);

                    if (newName == Name)
                    {
                        Close(true);
                        return;
                    }

                    string newPath = getPath(newName);
                    Write("Renaming session...: '" + Path + "' to '" + newName + "'");

                    Close(true);

                    try
                    {
                        Directory.Move(Path, newPath);
                        path = newPath;

                        lock (names2Session)
                        {
                            names2Session.Remove(name);
                            name = newName;
                            names2Session[name] = this;
                        }
                    }
                    catch (Exception e)
                    {
                        Error(e);
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
                    //names2NamedWriter.Clear(); !!! clearing writers will bring to duplicating of them if they are referenced alongside also.

                    lock (threads2treadWriter)
                    {
                        foreach (ThreadWriter tw in threads2treadWriter.Values)
                            tw.Close();
                        threads2treadWriter.Clear();
                    }

                    if (!reuse)
                    {
                        path = null;
                        CreatedTime = DateTime.MinValue;
                        timeMark = null;
                    }
                }
            }
        }
    }
}