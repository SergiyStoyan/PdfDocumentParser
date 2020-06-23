//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Cliver
{
    public partial class Log
    {
        public abstract partial class Writer
        {
            internal Writer(string name, Session session)
            {
                Name = name;
                this.session = session;

                switch (Log.mode)
                {
                    case Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                        FileName = Log.ProcessName + (string.IsNullOrWhiteSpace(session.Name) ? "" : "_" + session.Name) + "_" + session.TimeMark + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name) + "." + FileExtension;
                        break;
                    //case Cliver.Log.Mode.SINGLE_SESSION:
                    case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                        FileName = Log.ProcessName + "_" + session.TimeMark + (string.IsNullOrWhiteSpace(name) ? "" : "_" + name) + "." + FileExtension;
                        break;
                    default:
                        throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                }
            }

            public Level Level
            {
                get
                {
                    return level;
                }
                set
                {
                    if (level == Level.NONE && value > Level.NONE)
                    {
                        setWorkDir(true);
                        Directory.CreateDirectory(session.Path);
                    }
                    level = value;
                }
            }
            Level level = Log.level;

            public readonly string Name;
            public string FileName { get; private set; } = null;

            readonly Session session;

            public int MaxFileSize = Log.maxFileSize;

            public const string MAIN_THREAD_LOG_NAME = "";

            /// <summary>
            /// Log file path.
            /// </summary>
            public string Path
            {
                get
                {
                    return session.Path + System.IO.Path.DirectorySeparatorChar + FileName;
                }
            }

            //public string TimeMark
            //{
            //    get
            //    {
            //        switch (Log.mode)
            //        {
            //            case Cliver.Log.Mode.ONLY_LOG:
            //                return Cliver.Log.TimeMark;
            //            //case Cliver.Log.Mode.SINGLE_SESSION:
            //            case Cliver.Log.Mode.SESSIONS:
            //                return session.TimeMark;
            //            default:
            //                throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
            //        }
            //    }
            //}

            /// <summary>
            /// Close the log
            /// </summary>
            public void Close()
            {
                lock (this)
                {
                    if (logWriter != null)
                        logWriter.Close();
                    logWriter = null;
                }
            }

            /// <summary>
            /// General writting log method.
            /// </summary>
            public void Write(Log.MessageType messageType, string message, string details = null)
            {
                lock (this)
                {
                    write(messageType, message, details);
                    if (messageType == Log.MessageType.EXIT)
                    {
                        if (exiting)
                            return;
                        exiting = true;
                        //if (exitingThread != null)
                        //    return;
                        //exitingThread = ThreadRoutines.Start(() =>
                        //{
                        try
                        {
                            Exitig?.Invoke(message);
                        }
                        catch (Exception e)
                        {
                            string m;
                            string d;
                            GetExceptionMessage(e, out m, out d);
                            write(Log.MessageType.ERROR, m, d);
                        }
                        finally
                        {
                            Environment.Exit(0);
                        }
                        //});
                    }
                }
            }
            //static protected System.Threading.Thread exitingThread = null;
            static protected bool exiting = false;
            void write(Log.MessageType messageType, string message, string details)
            {
                lock (this)
                {
                    Writing?.Invoke(Name, messageType, message, details);

                    switch (Level)
                    {
                        case Level.NONE:
                            return;
                        case Level.ERROR:
                            if (messageType < MessageType.ERROR)
                                return;
                            break;
                        case Level.WARNING:
                            if (messageType < MessageType.WARNING)
                                return;
                            break;
                        case Level.INFORM:
                            if (messageType < MessageType.INFORM)
                                return;
                            break;
                        case Level.ALL:
                            break;
                        default:
                            throw new Exception("Unknown option: " + Level);
                    }

                    if (logWriter == null)
                        logWriter = new StreamWriter(Path, true);

                    details = string.IsNullOrWhiteSpace(details) ? "" : "\r\n\r\n" + details;
                    message = (messageType == MessageType.LOG ? "" : messageType.ToString()) + ": " + message + details;

                    if (MaxFileSize > 0)
                    {
                        FileInfo fi = new FileInfo(Path);
                        if (fi.Length > MaxFileSize)
                        {
                            logWriter.Close();

                            if (fileCounter < 1)
                            {
                                FileName = Regex.Replace(FileName, @"\.[^\.]+$", @"[1]$0");
                                fileCounter = 1;
                            }
                            else
                                FileName = Regex.Replace(FileName, @"\[" + fileCounter + @"\](\.[^\.]+)$", @"[" + (++fileCounter) + @"]$1");

                            logWriter = new StreamWriter(Path, true);
                        }
                    }
                    //if (!string.IsNullOrWhiteSpace(details))
                    //    message += "\r\n\r\n" + details;
                    logWriter.WriteLine(DateTime.Now.ToString("[dd-MM-yy HH:mm:ss] ") + message);
                    logWriter.Flush();
                }
            }
            TextWriter logWriter = null;
            int fileCounter = 0;

            public delegate void OnWrite(string logWriterName, Log.MessageType messageType, string message, string details);
            static public event OnWrite Writing = null;
        }
    }
}