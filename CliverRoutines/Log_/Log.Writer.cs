//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        /// <summary>
        /// The base log writer. 
        /// </summary>
        abstract public partial class Writer
        {
            internal Writer(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Log name.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Message importance level.
            /// </summary>
            abstract public Level Level { get; set; }
            protected Level level = Log.DefaultLevel;

            /// <summary>
            /// Log file path.
            /// </summary>
            public string File { get; protected set; } = null;

            abstract internal void SetFile();
            protected int fileCounter = 0;

            /// <summary>
            /// Maximum log file length in bytes.
            /// If negative than no effect.
            /// </summary>
            public int MaxFileSize = Log.DefaultMaxFileSize;

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

                    if (!Is2BeLogged(messageType))
                        return;

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
            protected TextWriter logWriter = null;

            public bool Is2BeLogged(Log.MessageType messageType)
            {
                switch (Level)
                {
                    case Level.NONE:
                        return false;
                    case Level.ERROR:
                        if (messageType < MessageType.ERROR)
                            return false;
                        break;
                    case Level.WARNING:
                        if (messageType < MessageType.WARNING)
                            return false;
                        break;
                    case Level.INFORM:
                        if (messageType < MessageType.INFORM)
                            return false;
                        break;
                    case Level.ALL:
                        break;
                    default:
                        throw new Exception("Unknown option: " + Level);
                }
                return true;
            }

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