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
    /// <summary>
    /// Multithreaded logging routines
    /// </summary>
    public static partial class Log
    {
        public static void Initialize(Mode mode, string pre_work_dir = null, bool write_log = true, int delete_logs_older_days = 10, string session_name_prefix = "Session")
        {
            Log.ClearSession();
            //if (work_dir != null)
            //return;
            //    throw new Exception("Initialize should not be called when log is open.");
            Log.mode = mode;
            Log.pre_work_dir = pre_work_dir;
            Log.write_log = write_log;
            Log.delete_logs_older_days = delete_logs_older_days;
            Log.session_name_prefix = session_name_prefix;
        }
        static string pre_work_dir = null;
        static int delete_logs_older_days = 10;
        static bool write_log = true;
        static Mode mode = Mode.ONLY_LOG;
        static string session_name_prefix = "Session";

        public static bool ShowDeleteOldLogsDialog = true;

        public enum Mode
        {
            /// <summary>
            /// Each session creates its own folder.
            /// </summary>
            SESSIONS,
            /// <summary>
            /// Only one session can be at the same time.
            /// </summary>
           // SINGLE_SESSION,
            /// <summary>
            /// Writes only log file without creating session folder.
            /// </summary>
            ONLY_LOG
        }

        public static readonly System.Threading.Thread MainThread = System.Threading.Thread.CurrentThread;

        /// <summary>
        /// Log belonging to the first (main) thread of the process.
        /// </summary>
        public static ThreadWriter Main
        {
            get
            {
                return ThreadWriter.Main;
            }
        }

        public static void CloseAll()
        {
            Log.ThreadWriter.CloseAll();
        }

        /// <summary>
        /// Log beloning to the current thread.
        /// </summary>
        public static ThreadWriter This
        {
            get
            {
                return ThreadWriter.This;
            }
        }

        /// <summary>
        /// Log path
        /// </summary>
        public static string Path
        {
            get
            {
                return ThreadWriter.This.Path;
            }
        }

        /// <summary>
        /// Log id that is used for logging and browsing in GUI
        /// </summary>
        public static int Id
        {
            get
            {
                return ThreadWriter.This.Id;
            }
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        public static void Error(Exception e)
        {
            ThreadWriter.This.Error(e);
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        static public void Error(string message)
        {
            ThreadWriter.This.Error(message);
        }

        /// <summary>
        /// Write the stack information about the caller to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        static public void Trace(object message = null)
        {
            ThreadWriter.This.Trace(message);
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        static public void Exit(string message)
        {
            ThreadWriter.This.Error(message);
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        static public void Exit(Exception e)
        {
            ThreadWriter.This.Exit(e);
        }

        /// <summary>
        /// Write the warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning(string message)
        {
            ThreadWriter.This.Warning(message);
        }

        /// <summary>
        /// Write the exception as warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning(Exception e)
        {
            ThreadWriter.This.Warning(e);
        }

        /// <summary>
        /// Write the notification to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Inform(string message)
        {
            ThreadWriter.This.Inform(message);
        }

        /// <summary>
        /// Write the message to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Write(MessageType type, string message, string details = null)
        {
            ThreadWriter.This.Write(type, message, details);
        }

        static public void Write(string message)
        {
            ThreadWriter.This.Write(MessageType.LOG, message);
        }

        public enum MessageType
        {
            LOG = 0,
            INFORM = 1,
            WARNING = 2,
            ERROR = 3,
            EXIT = 4,
            TRACE = 5,
            //INFORM2 = 11,
            //WARNING2 = 21,
            //ERROR2 = 31,
            //EXIT2 = 41,
        }

        /// <summary>
        /// Return stack information for the caller.
        /// </summary>
        /// <returns></returns>
        public static string GetStackString(int startFrame = 0, int frameCount = 1, bool endOnEmptyFile = true)
        {
            System.Diagnostics.StackTrace st = new StackTrace(true);
            StackFrame sf;
            MethodBase mb = null;
            Type dt = null;
            int frameI = 2;
            for (; ; frameI++)
            {
                sf = st.GetFrame(frameI);
                if (sf == null)
                    break;
                mb = sf.GetMethod();
                dt = mb.DeclaringType;
                if (dt != typeof(Log) && dt != typeof(Log.Writer) && dt != typeof(LogMessage))
                    break;
            }
            List<string> frameSs = new List<string>();
            if (frameCount < 0)
                frameCount = 1000;
            int endFrameI = frameI + frameCount - 1;
            for (frameI += startFrame; frameI <= endFrameI; frameI++)
            {
                sf = st.GetFrame(frameI);
                if (sf == null || endOnEmptyFile && frameSs.Count > 0 && string.IsNullOrEmpty(sf.GetFileName()))//it seems to be passing out of the application
                    break;
                mb = sf.GetMethod();
                dt = mb.DeclaringType;
                frameSs.Add("method: " + dt?.ToString() + "::" + mb?.Name + " \r\nfile: " + sf.GetFileName() + " \r\nline: " + sf.GetFileLineNumber());
            }
            return string.Join("\r\n<=", frameSs);
        }

        public static string GetExceptionMessage(Exception e)
        {
            string m;
            string d;
            GetExceptionMessage(e, out m, out d);
            return m + " \r\n\r\n" + d;
        }

        public static string GetExceptionMessage2(Exception e)
        {
            string m;
            string d;
            GetExceptionMessage(e, out m, out d);
            return m;
        }

        //        static public void GetExceptionMessage(Exception e, out string message, out string details)
        //        {
        //            for (; e.InnerException != null; e = e.InnerException) ;
        //            message = "Exception: \r\n" + e.Message;
        //#if DEBUG            
        //            details = "Module:" + e.TargetSite.Module + " \r\n\r\nStack:" + e.StackTrace;
        //#else       
        //            details = ""; //"Module:" + e.TargetSite.Module + " \r\n\r\nStack:" + e.StackTrace;
        //#endif
        //        }

        static public void GetExceptionMessage(Exception e, out string message, out string details)
        {
            Exception lastInterestingE = null;
            bool passedOutOfApp = false;
            List<string> ms = new List<string>();
            for (; e != null; e = e.InnerException)
            {
                AggregateException ae = e as AggregateException;
                if (ae != null && ae.InnerExceptions.Count > 1)
                    ms.Add("More than 1 exception aggregated! Show only [0]:" + e.Message);
                else
                    ms.Add(e.Message);
                if (!passedOutOfApp)
                {
                    if (lastInterestingE != null && (e.StackTrace == null || e.TargetSite == null))//it seems to be passing out of the application
                        passedOutOfApp = true;
                    else
                        lastInterestingE = e;
                }
            }
            message = string.Join("\r\n<= ", ms);
            details = "Module: " + lastInterestingE?.TargetSite?.Module + " \r\n\r\nStack:" + lastInterestingE?.StackTrace;
        }
        //static void getExceptionMessage(Exception e, ref string message, ref string details)
        //{
        //    for (; e != null; e = e.InnerException)
        //    {
        //        message += "\r\n<= " + e.Message;

        //        AggregateException ae = e as AggregateException;
        //        if (ae != null && ae.InnerExceptions.Count > 1)
        //        {
        //            foreach (Exception ex in ae.InnerExceptions)
        //            {
        //                message += "\r\n ---\r\n ";
        //                getExceptionMessage(ex, ref message, ref details);
        //            }
        //            return;
        //        }
        //    }    
        //    details += "\r\n\r\nModule:" + e.TargetSite?.Module + " \r\n\r\nStack:" + e.StackTrace;
        //}
    }

    public class TerminatingException : Exception
    {
        public TerminatingException(string message)
            : base(message)
        {
            LogMessage.Exit(message);
        }
    }
}
