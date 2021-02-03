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

            /// <summary>
            /// Message importance level.
            /// </summary>
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
            Level level = Log.DefaultLevel;

            /// <summary>
            /// Log name.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Log file path.
            /// </summary>
            public string File { get; private set; } = null;

            internal void SetFile()
            {
                lock (this)
                {
                    string file2 = Session.Dir + System.IO.Path.DirectorySeparatorChar;
                    if (Log.mode.HasFlag(Mode.FOLDER_PER_SESSION))
                    {
                        file2 += DateTime.Now.ToString("yyMMddHHmmss");
                    }
                    else //if (Log.mode.HasFlag(Mode.ONE_FOLDER))//default
                    {
                        //file2 += (string.IsNullOrWhiteSpace(Session.Name) ? "" : Session.Name + "_") + Session.TimeMark;//separates session name from log name
                        file2 += Session.TimeMark + (string.IsNullOrWhiteSpace(Session.Name) ? "" : "_" + Session.Name);
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

            /// <summary>
            /// Session to which this log belongs.
            /// </summary>
            public readonly Session Session;

            /// <summary>
            /// Maximum log file length in bytes.
            /// If negative than no effect.
            /// </summary>
            public int MaxFileSize = Log.DefaultMaxFileSize;

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
            /// Base writting log method.
            /// </summary>
            public void Write(Log.MessageType messageType, string message, string details = null)
            {
                lock (this)
                {
                    write(messageType, message, details);
                    if (messageType == Log.MessageType.EXIT)
                        Environment.Exit(0);
                }
            }
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
                    logWriter.WriteLine(DateTime.Now.ToString(Log.TimePattern) + message);
                    logWriter.Flush();
                }
            }
            TextWriter logWriter = null;

            /// <summary>
            /// Called for Writing. 
            /// </summary>
            /// <param name="logWriterName"></param>
            /// <param name="messageType"></param>
            /// <param name="message"></param>
            /// <param name="details"></param>
            public delegate void OnWrite(string logWriterName, Log.MessageType messageType, string message, string details);
            /// <summary>
            /// Triggered before writing message.
            /// </summary>
            static public event OnWrite Writing = null;
        }
    }
}