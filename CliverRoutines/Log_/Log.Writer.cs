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
                            Directory.CreateDirectory(session.Path);
                        }
                        level = value;
                    }
                }
            }
            Level level = Log.level;

            public readonly string Name;

            public string File { get; private set; } = null;

            internal void SetFile()
            {
                lock (this)
                {
                    string file2;
                    switch (Log.mode)
                    {
                        case Log.Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                            file2 = session.Path + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + (string.IsNullOrWhiteSpace(session.Name) ? "" : "_" + session.Name) + "_" + session.TimeMark + (string.IsNullOrWhiteSpace(Name) ? "" : "_" + Name) + "." + FileExtension;
                            break;
                        case Cliver.Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                            file2 = session.Path + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + "_" + session.TimeMark + (string.IsNullOrWhiteSpace(Name) ? "" : "_" + Name) + "." + FileExtension;
                            break;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }

                    if (fileCounter == 1)
                        file2 = Regex.Replace(file2, @"\.[^\.]+$", @"[1]$0");
                    else
                        file2 = Regex.Replace(file2, @"\[" + fileCounter + @"\](\.[^\.]+)$", @"[" + fileCounter + @"]$1");

                    if (File == file2)
                        return;
                    if (logWriter != null)
                        logWriter.Close();
                    File = file2;
                }
            }
            int fileCounter = 0;

            readonly Session session;

            public int MaxFileSize = Log.maxFileSize;

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

                    details = string.IsNullOrWhiteSpace(details) ? "" : "\r\n\r\n" + details;
                    message = (messageType == MessageType.LOG ? "" : messageType.ToString()) + ": " + message + details;
                    logWriter.WriteLine(DateTime.Now.ToString("[dd-MM-yy HH:mm:ss] ") + message);
                    logWriter.Flush();
                }
            }
            TextWriter logWriter = null;

            public delegate void OnWrite(string logWriterName, Log.MessageType messageType, string message, string details);
            static public event OnWrite Writing = null;
        }
    }
}