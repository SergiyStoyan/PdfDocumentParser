//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cliver
{
    /// <summary>
    /// Multithreaded logging routines
    /// </summary>
    public static partial class Log
    {
        /// <summary>
        /// Deletes Log data from disk that is older than the specified threshold
        /// </summary>
        public static void DeleteOldLogs(int deleteLogsOlderDays, Func<string, bool> askYesNo = null)
        {
            lock (lockObject)//TEST!!!! if avoid interlock when writing to log from here
            {
                try
                {
                    if (deleteLogsOlderDays > 0)
                    {
                        DateTime FirstLogDate = DateTime.Now.AddDays(-deleteLogsOlderDays);

                        DirectoryInfo di = new DirectoryInfo(Log.WorkDir);
                        if (!di.Exists)
                            return;

                        string alert;
                        switch (Log.mode)
                        {
                            case Mode.EACH_SESSION_IS_IN_OWN_FORLDER:
                                alert = "Session data including caches and logs older than " + FirstLogDate.ToString() + " are to be deleted.\r\nDelete?";
                                foreach (DirectoryInfo d in di.GetDirectories())
                                {
                                    if (headSession != null && d.FullName.StartsWith(headSession.Dir, StringComparison.InvariantCultureIgnoreCase))
                                        continue;
                                    if (d.LastWriteTime >= FirstLogDate)
                                        continue;
                                    if (alert != null)
                                    {
                                        if (askYesNo == null)
                                            Log.Main.Inform("Deleting session data including caches and logs older than " + FirstLogDate.ToString());
                                        else
                                        if (!askYesNo(alert))
                                            return;
                                        alert = null;
                                    }
                                    Log.Main.Inform("Deleting old directory: " + d.FullName);
                                    try
                                    {
                                        d.Delete(true);
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e);
                                    }
                                }
                                break;
                            case Mode.ALL_LOGS_ARE_IN_SAME_FOLDER:
                                alert = "Logs older than " + FirstLogDate.ToString() + " are to be deleted.\r\nDelete?";
                                foreach (FileInfo f in di.GetFiles())
                                {
                                    if (f.LastWriteTime >= FirstLogDate)
                                        continue;
                                    if (alert != null)
                                    {
                                        if (askYesNo == null)
                                            Log.Main.Inform("Deleting logs older than " + FirstLogDate.ToString());
                                        else
                                        if (!askYesNo(alert))
                                            return;
                                        alert = null;
                                    }
                                    Log.Main.Inform("Deleting old file: " + f.FullName);
                                    try
                                    {
                                        f.Delete();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e);
                                    }
                                }
                                break;
                            default:
                                throw new Exception("Unknown LOGGING_MODE:" + Log.mode);
                        }
                    }
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// Return stack information for the caller.
        /// </summary>
        /// <returns></returns>
        public static string GetStackString(int startFrame = 0, int frameCount = 1, bool endOnEmptyFile = true)
        {
            StackTrace st = new StackTrace(true);
            int frameI = 1;
            for (; ; frameI++)
            {
                StackFrame sf = st.GetFrame(frameI);
                if (sf == null)
                    break;
                MethodBase mb = sf.GetMethod();
                Type dt = mb.DeclaringType;
                if (dt != typeof(Log) && dt != typeof(Log.Writer) && dt != typeof(Log.Session) && TypesExcludedFromStack?.Find(x => x == dt) == null)
                    break;
            }
            List<string> frameSs = new List<string>();
            if (frameCount < 0)
                frameCount = 1000;
            frameI += startFrame;
            int endFrameI = frameI + frameCount - 1;
            for (; frameI <= endFrameI; frameI++)
            {
                StackFrame sf = st.GetFrame(frameI);
                if (sf == null || endOnEmptyFile && frameSs.Count > 0 && string.IsNullOrEmpty(sf.GetFileName()))//it seems to be passing out of the application
                    break;
                MethodBase mb = sf.GetMethod();
                Type dt = mb.DeclaringType;
                frameSs.Add("method: " + dt?.ToString() + "::" + mb?.Name + " \r\nfile: " + sf.GetFileName() + " \r\nline: " + sf.GetFileLineNumber());
            }
            return string.Join("\r\n<=", frameSs);
        }
        static List<Type> TypesExcludedFromStack = null;

        /// <summary>
        /// Get exception message without details.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetExceptionMessage2(Exception e)
        {
          return  GetExceptionMessage(e, false);
        }

        /// <summary>
        /// Get exception message with details.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static public string GetExceptionMessage(Exception e, bool withDetails = true)
        {
            Exception lastInterestingE = null;
            bool passedOutOfApp = false;
            List<string> messages = new List<string>();
            for (; e != null; e = e.InnerException)
            {
                string m = e.Message + (withDetails ? " [" + e.GetType().FullName + "]" : "");
                AggregateException ae = e as AggregateException;
                if (ae != null && ae.InnerExceptions.Count > 1)
                    messages.Add("More than 1 exception aggregated! Show only [0]:" + m);
                else
                    messages.Add(m);
                if (!passedOutOfApp)
                {
                    if (lastInterestingE != null && (e.StackTrace == null || e.TargetSite == null))//it seems to be passing out of the application
                        passedOutOfApp = true;
                    else
                        lastInterestingE = e;
                }
            }
            StringBuilder sb = new StringBuilder(string.Join("\r\n<= ", messages));
            if (withDetails)
                sb.Append("\r\n\r\nModule: " + lastInterestingE?.TargetSite?.Module + " \r\n\r\nStack:" + lastInterestingE?.StackTrace);
            return sb.ToString();
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
}