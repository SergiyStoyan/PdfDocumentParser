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
        public abstract class Writer
        {
            internal Writer(string name, string file_name, Session session)
            {
                Name = name;
                this.file_name = file_name;
                this.session = session;
            }

            public readonly string Name;
            public string FileName
            {
                get
                {
                    return file_name;
                }
            }
            string file_name = null;

            readonly Session session;

            public static int MaxFileSize = -1;

            public const string MAIN_THREAD_LOG_NAME = "";

            /// <summary>
            /// Log path
            /// </summary>
            public string Path
            {
                get
                {
                    string directory;
                    switch (Log.mode)
                    {
                        case Cliver.Log.Mode.ONLY_LOG:
                            directory = Cliver.Log.WorkDir;
                            break;
                        //case Cliver.Log.Mode.SINGLE_SESSION:
                        case Cliver.Log.Mode.SESSIONS:
                            directory = session.Path;
                            break;
                        default:
                            throw new Exception("Unknown LOGGING_MODE:" + Cliver.Log.mode);
                    }
                    return directory + System.IO.Path.DirectorySeparatorChar + FileName;
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
                    if (log_writer != null)
                        log_writer.Close();
                    log_writer = null;
                    _ErrorCount = 0;
                }
            }

            public Level Level =  Level.ALL;

            /// <summary>
            /// Write the error to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Error(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(Log.MessageType.ERROR, m, e is Exception2 ? null : d);
            }

            public void Error2(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.ERROR, m);
            }

            /// <summary>
            /// Write the error to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Error(string message)
            {
                Write(MessageType.ERROR, message, GetStackString());
            }

            public void Error(string message, Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.ERROR, message, m + (e is Exception2 ? null : "\r\n\r\n" + d));
            }

            public void Error2(string message)
            {
                Write(MessageType.ERROR, message);
            }

            /// <summary>
            /// Write the stack informtion for the caller to the current thread's log
            /// </summary>
            /// <param name="e"></param>
            public void Trace(object message = null)
            {
                Write(MessageType.TRACE, message == null ? null : message.ToString(), GetStackString());
            }

            /// <summary>
            /// Write the error to the current thread's log and terminate the process.
            /// </summary>
            /// <param name="e"></param>
            public void Exit(string message)
            {
                lock (this)
                {
                    if (Name != MAIN_THREAD_LOG_NAME)
                        Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                    Write(MessageType.EXIT, message, Log.GetStackString());
                }
            }

            public void Exit2(string message)
            {
                lock (this)
                {
                    if (Name != MAIN_THREAD_LOG_NAME)
                        Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                    Write(MessageType.EXIT, message);
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
                    if (Name != MAIN_THREAD_LOG_NAME)
                        Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                    string m;
                    string d;
                    GetExceptionMessage(e, out m, out d);
                    Write(MessageType.EXIT, m, e is Exception2 ? null : d);
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
                Write(MessageType.WARNING, message, GetStackString());
            }

            public void Warning(string message, Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, message, m + (e is Exception2 ? null : "\r\n\r\n" + d));
            }

            public void Warning2(string message)
            {
                Write(MessageType.WARNING, message);
            }

            /// <summary>
            /// Write the exception as warning to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Warning(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, m, e is Exception2 ? null : d);
            }

            public void Warning2(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, m);
            }

            /// <summary>
            /// Write the notification to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Inform(string message)
            {
                Write(MessageType.INFORM, message);
            }

            public void Inform0(string message)
            {
                Write(MessageType.INFORM, message, GetStackString());
            }

            public void Write(string line)
            {
                Write(MessageType.LOG, line);
            }

            public void Write0(string message)
            {
                Write(MessageType.LOG, message, GetStackString());
            }

            /// <summary>
            /// Write the message to the current thread's log.
            /// </summary>
            /// <param name="e"></param>
            public void Write(Log.MessageType messageType , string message, string details = null)
            {
                lock (this)
                {
                    if (messageType == Log.MessageType.EXIT)
                    {
                        if (exiting_thread != null)
                            return;
                        write(messageType, message, details);
                        exiting_thread = startThread(() =>
                        {
                            try
                            {
                                if (Exitig != null)
                                    Exitig.Invoke(message);
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
                        });
                    }
                    else
                        write(messageType, message, details);
                }
            }
            void write(Log.MessageType messageType , string message, string details)
            {
                switch(Level)
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

                lock (this)
                {
                    Writing?.Invoke(Name, messageType, message, details);

                    if (Log.writeLog)
                    {
                        if (log_writer == null)
                            log_writer = new StreamWriter(Path, true);

                        details = string.IsNullOrWhiteSpace(details) ? "" : "\r\n\r\n" + details;
                        message = (messageType == MessageType.LOG ? "" : messageType.ToString()) + ": " + message + details;
                        //switch (type)
                        //{
                        //    case Log.MessageType.INFORM:
                        //        message = "INFORM: " + message;
                        //        break;
                        //    case Log.MessageType.WARNING:
                        //        message = "WARNING: " + message;
                        //        break;
                        //    case Log.MessageType.ERROR:
                        //        message = "ERROR: " + message + details;
                        //        _ErrorCount++;
                        //        break;
                        //    case Log.MessageType.EXIT:
                        //        message = "EXIT: " + message + details;
                        //        _ErrorCount++;
                        //        break;
                        //    case Log.MessageType.TRACE:
                        //        message = "TRACE: " + message;
                        //        break;
                        //    case Log.MessageType.LOG:
                        //        break;
                        //    default:
                        //        throw new Exception("No case for " + type.ToString());
                        //}

                        if (MaxFileSize > 0)
                        {
                            FileInfo fi = new FileInfo(Path);
                            if (fi.Length > MaxFileSize)
                            {
                                log_writer.Close();

                                int counter = 0;
                                file_name = Regex.Replace(file_name, @"(\d+_)(\d+)(\.[^\.]+)$",
                                    (Match m) =>
                                    {
                                        counter = int.Parse(m.Groups[2].Value) + 1;
                                        return m.Groups[1].Value + counter + m.Groups[3].Value;
                                    }
                                    );
                                if (counter < 1)
                                    file_name = Regex.Replace(file_name, @"\.[^\.]+$", @"_1$0");

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
            static protected System.Threading.Thread exiting_thread = null;
            //StreamWriter create_next_log_writer()
            //{
            //    for (; ; )
            //    {
            //        int counter = 0;
            //        file_name = Regex.Replace(file_name, @"(\d+_)(\d+)(\.[^\.]+)$",
            //            (Match m) =>
            //            {
            //                counter = int.Parse(m.Groups[2].Value) + 1;
            //                return m.Groups[1].Value + counter + m.Groups[3].Value;
            //            }
            //            );
            //        if (counter < 1)
            //            file_name = Regex.Replace(file_name, @"\.[^\.]+$", @"_1$0");
            //        try
            //        {
            //            return new StreamWriter(Path, true);
            //        }
            //        catch (Exception e)
            //        {
            //            log_writer = null;
            //        }
            //    }
            //}

            public int ErrorCount
            {
                get
                {
                    return _ErrorCount;
                }
            }
            int _ErrorCount = 0;

            public delegate void OnWrite(string logWriterName, Log.MessageType messageType, string message, string details);
            static public event OnWrite Writing = null;
        }
    }
}