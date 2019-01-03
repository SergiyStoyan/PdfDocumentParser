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
using System.Reflection;

namespace Cliver
{
    public static partial class Log
    {
        public partial class Session
        {
            const string MAIN_SESSION_NAME = "";

            public Session(string name = MAIN_SESSION_NAME)
            {
                TimeMark = CreatedTime.ToString("yyMMddHHmmss");
                this.name = name;

                //if (Log.mode == Mode.ONLY_LOG)
                //    throw new Exception("SessionDir cannot be used in Log.Mode.ONLY_LOG");

                path = get_path(name);
                if(Log.mode == Mode.SESSIONS && write_log)
                    Directory.CreateDirectory(path);
            }

            string get_path(string name)
            {
                lock (this.names2nw)
                {
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ONLY_LOG:
                            return WorkDir;
                        //case Cliver.Log.Mode.SINGLE_SESSION:
                        case Cliver.Log.Mode.SESSIONS:
                            string path0 = WorkDir + "\\" + session_name_prefix + "_" + (string.IsNullOrWhiteSpace(name) ? "" : name + "_") + TimeMark;
                            string path = path0;
                            for (int count = 1; Directory.Exists(path); count++)
                                path = path0 + "_" + count.ToString();
                            return path;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                }
            }

            public string Name
            {
                get
                {
                    lock (this.names2nw)//this lock is needed if Session::Close(string new_name) is performing
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
                    lock (this.names2nw)//this lock is needed if Session::Close(string new_name) is performing
                    {
                        return path;
                    }
                }
            }
            string path;

            public readonly DateTime CreatedTime = DateTime.Now;
            public readonly string TimeMark;

            Dictionary<string, NamedWriter> names2nw = new Dictionary<string, NamedWriter>();

            /// <summary>
            /// Close all writing streams and rename session and its directory. Using the session can be continued after that.
            /// </summary>
            /// <param name="new_name"></param>
            public void Close(string new_name)
            {
                lock (this.names2nw)
                {
                    if (Log.mode == Mode.ONLY_LOG)
                        throw new Exception("Cannot rename log folder in mode: " + Log.mode);

                    if (new_name == Name)
                    {
                        Close();
                        return;
                    }

                    string new_path = get_path(new_name);
                    Default.Write("Renaming session: '" + Path + "' to '" + new_path + "'");

                    Close();

                    try
                    {
                        Directory.Move(Path, new_path);
                        path = new_path;
                        name = new_name;
                    }
                    catch (Exception e)
                    {
                        Log.Main.Error(e);
                    }
                }
            }

            /// <summary>
            /// Close all writing streams. The session can be used after.
            /// </summary>
            public void Close()
            {
                lock (this.names2nw)
                {
                    if (names2nw.Values.Count < 1 && !ThreadWriter.IsAnythingOpen)
                        return;

                    Default.Write("Closing the log session");

                    //any ThreadWriter can belong only to MainSession
                    if(Log.IsMainSessionOpen && this == Log.MainSession)
                        Log.ThreadWriter.CloseAll();

                    foreach (NamedWriter nw in names2nw.Values)
                        nw.Close();
                    //names2nw.Clear(); !!! clearing writers will bring to duplicating of them if they are referenced alongside also.
                }
            }

            public int TotalErrorCount
            {
                get
                {
                    lock (this.names2nw)
                    {
                        int ec = 0;
                        foreach (NamedWriter nw in names2nw.Values)
                            ec += nw.ErrorCount;
                        return ec;
                    }
                }
            }
            
            public NamedWriter this[string name]
            {
                get
                {
                    return NamedWriter.Get(this, name);
                }
            }

            ///// <summary>
            ///// Output directory for current session
            ///// </summary>
            //public static string OutputDir
            //{
            //    get
            //    {
            //        if (output_dir == null)
            //        {
            //            lock (lock_object)
            //            {
            //                output_dir = SessionDir + @"\" + OutputDirName;

            //                DirectoryInfo di = new DirectoryInfo(output_dir);
            //                if (!di.Exists)
            //                    di.Create();
            //            }
            //        }
            //        return output_dir;
            //    }
            //}
            //static string output_dir = null;

            ///// <summary>
            ///// Output folder name
            ///// </summary>
            //public static string OutputDirName = @"output";

            ///// <summary>
            ///// Download directory for session. 
            ///// This dir can be used to calculate value of downloaded bytes.
            ///// </summary>
            //public static string DownloadDir
            //{
            //    get
            //    {
            //        if (download_dir == null)
            //        {
            //            lock (lock_object)
            //            {
            //                download_dir = SessionDir + "\\" + DownloadDirName;

            //                DirectoryInfo di = new DirectoryInfo(download_dir);
            //                if (!di.Exists)
            //                    di.Create();
            //            }
            //        }
            //        return download_dir;
            //    }
            //}
            //static string download_dir = null;
            //public const string DownloadDirName = "cache";

            public Writer Default
            {
                get
                {
                    if (this == Log.MainSession)
                        return ThreadWriter.Main;
                    return NamedWriter.Get(this, DEFAULT_NAMED_LOG);
                }
            }

            internal const string DEFAULT_NAMED_LOG = "_";//to differ from ThreadWriter.Main

            public void Error(Exception e)
            {
                Default.Error(e);
            }

            public void Error(string message)
            {
                Default.Error(message);
            }

            public void Trace(object message = null)
            {
                Default.Trace(message);
            }

            public void Exit(string message)
            {
                Default.Error(message);
            }

            public void Exit(Exception e)
            {
                Default.Exit(e);
            }

            public void Warning(string message)
            {
                Default.Warning(message);
            }

            public void Warning(Exception e)
            {
                Default.Warning(e);
            }

            public void Inform(string message)
            {
                Default.Inform(message);
            }

            public void Write(MessageType type, string message, string details = null)
            {
                Default.Write(type, message, details);
            }

            public void Write(string message)
            {
                Default.Write(MessageType.LOG, message);
            }
        }
    }
}