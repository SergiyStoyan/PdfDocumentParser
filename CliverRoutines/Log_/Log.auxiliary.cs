//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Linq;
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
            lock (lockObject)//TEST??? to avoid interlock when writing to log from here
            {
                try
                {
                    if (deleteLogsOlderDays < 0)
                        return; 
                        DateTime firstLogTime = DateTime.Now.AddDays(-deleteLogsOlderDays);
                    //!!!sometimes no session is created yet by this moment or this function is called without log at all, so the session list can be empty
                    DateTime currentLogTime = Session.GetAll().Select(a => a.CreatedTime).DefaultIfEmpty(DateTime.Now.AddHours(-1)).Min();
                    if (firstLogTime > currentLogTime)
                        firstLogTime = currentLogTime;

                    DirectoryInfo di = new DirectoryInfo(Log.RootDir);
                    if (!di.Exists)
                        return;

                    string alert;
                    if (Log.mode.HasFlag(Mode.FOLDER_PER_SESSION))
                    {
                        alert = "Session data including caches and logs older than " + firstLogTime.ToString() + " are to be deleted.\r\nDelete?";
                        foreach (DirectoryInfo d in di.GetDirectories())
                        {
                            if (headSession != null && d.FullName.StartsWith(headSession.Dir, StringComparison.InvariantCultureIgnoreCase))
                                continue;
                            if (d.LastWriteTime >= firstLogTime)
                                continue;
                            if (alert != null)
                            {
                                if (askYesNo == null)
                                    Log.Main.Inform("Deleting session data including caches and logs older than " + firstLogTime.ToString());
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
                    }
                    else //if (Log.mode.HasFlag(Mode.ONE_FOLDER))//default
                    {
                        alert = "Logs older than " + firstLogTime.ToString() + " are to be deleted.\r\nDelete?";
                        foreach (FileInfo f in di.GetFiles())
                        {
                            if (f.LastWriteTime >= firstLogTime)
                                continue;
                            if (alert != null)
                            {
                                if (askYesNo == null)
                                    Log.Main.Inform("Deleting logs older than " + firstLogTime.ToString());
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

                    }
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// Get stack information for the caller.
        /// </summary>
        /// <param name="startFrame">frame to start with</param>
        /// <param name="frameCount">number of frames to take</param>
        /// <param name="endOnEmptyFile">if true, stop when going out of the app</param>
        /// <returns>stack info</returns>
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
        /// Get exception message without stack info.
        /// </summary>
        /// <param name="e">exception</param>
        /// <returns>exception message chain</returns>
        public static string GetExceptionMessage2(Exception e)
        {
            return GetExceptionMessage(e, false);
        }

        /// <summary>
        /// Get the exception message chain.
        /// </summary>
        /// <param name="e">exception</param>
        /// <param name="withDetails">add stack info</param>
        /// <returns>exception message chain</returns>
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

        /// <summary>
        /// Return name of the method which called this function
        /// </summary>
        /// <param name="name">don't change it</param>
        /// <returns>name of the calling method</returns>
        static public string GetThisMethodName([System.Runtime.CompilerServices.CallerMemberName] string name = "undefined")
        {
            return name;
        }
    }
}