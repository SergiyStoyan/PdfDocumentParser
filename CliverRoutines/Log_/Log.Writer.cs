//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
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
                Session = session;
                SetFile();
            }

            public Level Level
            {
                get
                {
                    return level;
                }
                set
                {
                    lock (this)
                    {
                        if (level == Level.NONE && value > Level.NONE)
                        {
                            setWorkDir(true);
                            Directory.CreateDirectory(Session.Dir);
                        }
                        level = value;
                    }
                }
            }
            Level level = Log.defaultLevel;

            public readonly string Name;

            public string File { get; private set; } = null;

            internal void SetFile()
            {
                lock (this)
                {
                    //string file2 = Session.Dir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName;
                    string file2 = Session.Dir + System.IO.Path.DirectorySeparatorChar;
                    switch (Log.mode)
                    {
                        case Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                            file2 += (string.IsNullOrWhiteSpace(Session.Name) ? "" : Session.Name + "_") + Session.TimeMark;
                            //if (!string.IsNullOrWhiteSpace(Name))//not Main log
                            //    file2 += "_" + DateTime.Now.ToString("yyMMddHHmmss");
                            break;
                        case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                            file2 += DateTime.Now.ToString("yyMMddHHmmss");
                            break;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                    file2 += (string.IsNullOrWhiteSpace(Name) ? "" : "_" + Name) + (fileCounter > 0 ? "[" + fileCounter + "]" : "") + "." + FileExtension;

                    if (File == file2)
                        return;
                    if (logWriter != null)
                        logWriter.Close();
                    File = file2;
                }
            }
            int fileCounter = 0;

            public readonly Session Session;

            public int MaxFileSize = Log.defaultMaxFileSize;

            public const string MAIN_THREAD_LOG_NAME = "";

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

            internal bool IsClosed
            {
                get
                {
                    return logWriter == null;
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
                            write(Log.MessageType.ERROR, GetExceptionMessage(e));
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
            void write(Log.MessageType messageType, string message, string details = null)
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

                    if (MaxFileSize > 0)
                    {
                        FileInfo fi = new FileInfo(File);
                        if (fi.Exists && fi.Length > MaxFileSize)
                        {
                            fileCounter++;
                            SetFile();
                        }
                    }

                    if (logWriter == null)
                    {
                        Directory.CreateDirectory(PathRoutines.GetFileDir(File));
                        logWriter = new StreamWriter(File, true);
                    }

                    message = (messageType == MessageType.LOG ? "" : messageType.ToString() + ": ") + message + (string.IsNullOrWhiteSpace(details) ? "" : "\r\n\r\n" + details);
                    logWriter.WriteLine(DateTime.Now.ToString(Log.timePattern) + message);
                    logWriter.Flush();
                }
            }
            TextWriter logWriter = null;

            public delegate void OnWrite(string logWriterName, Log.MessageType messageType, string message, string details);
            static public event OnWrite Writing = null;
        }
    }
}