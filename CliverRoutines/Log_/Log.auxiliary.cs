//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
                if (dt != typeof(Log) && dt != typeof(Log.Writer) && TypesExcludedFromStack?.Find(x => x == dt) == null)
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

        public static string GetExceptionMessage(Exception e)
        {
            GetExceptionMessage(e, out string m, out string d);
            return m + " \r\n\r\n" + d;
        }

        public static string GetExceptionMessage2(Exception e)
        {
            GetExceptionMessage(e, out string m, out string d);
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
}