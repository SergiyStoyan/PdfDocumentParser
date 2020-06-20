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
        public abstract partial class Writer : IWriteApi
        {
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
            /// Write the stack information for the caller.
            /// </summary>
            /// <param name="e"></param>
            public void Trace(object message = null)
            {
                Write(MessageType.TRACE, message == null ? null : message.ToString(), GetStackString());
            }

            /// <summary>
            /// Write the error to the log and terminate the process.
            /// </summary>
            /// <param name="e"></param>
            public void Exit(string message)
            {
                lock (this)
                {
                    if (Name != MAIN_THREAD_LOG_NAME)
                        Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                    Write(MessageType.EXIT, message, GetStackString());
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
            /// Write the error to the log and terminate the process.
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
            /// Write warning with stack details.
            /// </summary>
            /// <param name="message"></param>
            public void Warning(string message)
            {
                Write(MessageType.WARNING, message, GetStackString());
            }

            /// <summary>
            /// Write warning with exception with details.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="e"></param>
            public void Warning(string message, Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, message, m + (e is Exception2 ? null : "\r\n\r\n" + d));
            }

            /// <summary>
            /// Write warning without stack details.
            /// </summary>
            /// <param name="message"></param>
            public void Warning2(string message)
            {
                Write(MessageType.WARNING, message);
            }

            /// <summary>
            /// Write exception with details.
            /// </summary>
            /// <param name="e"></param>
            public void Warning(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, m, e is Exception2 ? null : d);
            }

            /// <summary>
            /// Write exception without details.
            /// </summary>
            /// <param name="e"></param>
            public void Warning2(Exception e)
            {
                string m;
                string d;
                GetExceptionMessage(e, out m, out d);
                Write(MessageType.WARNING, m);
            }

            public void Inform(string message)
            {
                Write(MessageType.INFORM, message);
            }

            public void Inform0(string message)
            {
                Write(MessageType.INFORM, message, GetStackString());
            }

            public void Write(string message)
            {
                Write(MessageType.LOG, message);
            }

            public void Write0(string message)
            {
                Write(MessageType.LOG, message, GetStackString());
            }

            public void Debug(string message)
            {
                Write(MessageType.DEBUG, message);
            }

            public void Debug0(string message)
            {
                Write(MessageType.DEBUG, message, GetStackString());
            }
        }
    }
}