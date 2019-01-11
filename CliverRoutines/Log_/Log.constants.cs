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
        static object lock_object = new object();

        static Log()
        {
            if (ProgramRoutines.IsWebContext)
                throw new Exception("Log is disabled in web context.");

            if (ProgramRoutines.IsWebContext)
                ProcessName = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).Name;
            else
                ProcessName = System.Reflection.Assembly.GetEntryAssembly().GetName(false).Name;

            AppDir = AppDomain.CurrentDomain.BaseDirectory;
            
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            CompanyName = string.IsNullOrWhiteSpace(fvi.CompanyName) ? "CliverSoft" : fvi.CompanyName;

            CompanyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + CompanyName;
            AppCommonDataDir = CompanyCommonDataDir + "\\" + Log.ProcessName;
            //Log.DeleteOldLogs();
        }

        /// <summary>
        /// Normalized name of this process
        /// </summary>
        public static readonly string ProcessName;

        public static readonly string CompanyName;

        /// <summary>
        /// Directory where the CliverSoft's application data independent on user are located.
        /// </summary>
        public static readonly string CompanyCommonDataDir;

        /// <summary>
        /// Directory where the application's data files independent on user are located.
        /// </summary>
        public static readonly string AppCommonDataDir;

        /// <summary>
        /// Directory where the application binary is located.
        /// </summary>
        public readonly static string AppDir;
        
        /// <summary>
        ///Parent Log directory where logs are recorded
        /// </summary>
        public static string WorkDir
        {
            get
            {
                if (work_dir == null)
                {
                    Thread deleting_old_logs_t = null;
                    lock (lock_object)
                    {
                        if (pre_work_dir != null && pre_work_dir.Contains(":"))
                        {
                            work_dir = pre_work_dir + @"\" + Log.ProcessName + WorkDirPrefix;
                            if (write_log && !Directory.Exists(work_dir))
                                try
                                {
                                    Directory.CreateDirectory(work_dir);
                                }
                                catch
                                {
                                    pre_work_dir = null;
                                }
                        }
                        if (string.IsNullOrWhiteSpace(work_dir) || !Directory.Exists(work_dir))
                        {
                            foreach (string base_dir in new string[] {
                                Log.AppDir,
                                CompanyCommonDataDir,
                                System.IO.Path.GetTempPath() + "\\" + CompanyName + "\\"
                            })
                            {
                                work_dir = base_dir + @"\" + pre_work_dir + @"\" + Log.ProcessName + WorkDirPrefix;
                                if (!write_log)
                                    break;
                                if (Directory.Exists(work_dir))
                                    break;
                                try
                                {
                                    Directory.CreateDirectory(work_dir);
                                    if (Directory.Exists(work_dir))
                                        break;
                                }
                                catch { }
                            }
                        }
                        if (write_log)
                            if (Directory.Exists(work_dir) && delete_logs_older_days >= 0)
                                deleting_old_logs_t = ThreadRoutines.StartTry(() => { Log.DeleteOldLogs(delete_logs_older_days, ShowDeleteOldLogsDialog); });//to avoid a concurrent loop while accessing the log file from the same thread 
                            else
                                throw new Exception("Could not create log folder!");
                    }
                    // delete_old_logs?.Join();
                }
                return work_dir;
            }
        }
        static string work_dir = null;
        public const string WorkDirPrefix = @"_Sessions";
        //static bool HaveWritePermissionForDir(string dir)
        //{
        //    var writeAllow = false;
        //    var writeDeny = false;
        //    var accessControlList = Directory.GetAccessControl(dir);
        //    if (accessControlList == null)
        //        return false;
        //    var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
        //    if (accessRules == null)
        //        return false;

        //    foreach (System.Security.AccessControl.FileSystemAccessRule rule in accessRules)
        //    {
        //        if ((System.Security.AccessControl.FileSystemRights.Write & rule.FileSystemRights) != System.Security.AccessControl.FileSystemRights.Write)
        //            continue;
        //        if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Allow)
        //            writeAllow = true;
        //        else if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Deny)
        //            writeDeny = true;
        //    }

        //    return writeAllow && !writeDeny;
        //}

        /// <summary>
        /// Directory of the current main session
        /// </summary>
        public static string SessionDir
        {
            get
            {
                return MainSession.Path;
            }
        }

        public static Session MainSession
        {
            get
            {
                if (main_session == null)
                    main_session = new Session();
                return main_session;
            }
        }
        static Session main_session = null;

        public static bool IsMainSessionOpen
        {
            get
            { 
                return main_session != null;
            }
        }

        /// <summary>
        /// Output folder name
        /// </summary>
        public static string OutputDirName = @"output";
        
        /// <summary>
        /// Used to clear all session parameters in order to start a new session
        /// </summary>
        public static void ClearSession()
        {
            lock (lock_object)
            {
                Log.CloseAll();
                
                work_dir = null;
                if (main_session != null)
                    main_session.Close();
                main_session = null;

                GC.Collect();
            }
        }

        /// <summary>
        /// Deletes Log data from disk that is older than the specified threshold
        /// </summary>
        public static void DeleteOldLogs(int delete_logs_older_days, bool show_dialog)
        {
            //ThreadWriter tw = Log.Main;
            //Log.Main.Inform("test");
            if (delete_old_logs_running)
                return;
            //lock (lock_object)//no lock to avoid interlock when writing to log from here
            //{
            delete_old_logs_running = true;
            try
            {
                if (delete_logs_older_days > 0)
                {
                    DateTime FirstLogDate = DateTime.Now.AddDays(-delete_logs_older_days);

                    DirectoryInfo di = new DirectoryInfo(Log.WorkDir);
                    if (!di.Exists)
                        return;

                    string alert;
                    switch (Log.mode)
                    {
                        case Mode.SESSIONS:
                            //case Mode.SINGLE_SESSION:
                            alert = "Session data including caches and logs older than " + FirstLogDate.ToString() + " are to be deleted.\r\nDelete?";
                            foreach (DirectoryInfo d in di.GetDirectories())
                            {
                                if (main_session != null && d.FullName.StartsWith(main_session.Path, StringComparison.InvariantCultureIgnoreCase))
                                    continue;
                                if (d.LastWriteTime >= FirstLogDate)
                                    continue;
                                if (alert != null)
                                {
                                    if (!show_dialog)
                                        Log.Main.Inform("Deleting session data including caches and logs older than " + FirstLogDate.ToString());
                                    else
                                    if (!LogMessage.AskYesNo(alert, true))
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
                                    LogMessage.Error(e);
                                }
                            }
                            break;
                        case Mode.ONLY_LOG:
                            alert = "Logs older than " + FirstLogDate.ToString() + " are to be deleted.\r\nDelete?";
                            foreach (FileInfo f in di.GetFiles())
                            {
                                if (f.LastWriteTime >= FirstLogDate)
                                    continue;
                                if (alert != null)
                                {
                                    if (!show_dialog)
                                        Log.Main.Inform("Deleting logs older than " + FirstLogDate.ToString());
                                    else
                                    if (!LogMessage.AskYesNo(alert, true))
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
                                    LogMessage.Error(e);
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
                delete_old_logs_running = false;
            }
            //}
        }
        static bool delete_old_logs_running = false;

        /// <summary>
        /// Create absolute path from app directory and relative path
        /// </summary>
        /// <param name="file">file path or name</param>
        /// <returns>absolute path</returns>
        //public static string GetAbsolutePath(string path)
        //{
        //    try
        //    {
        //        if (path.Contains(":"))
        //            return path;
        //        return System.IO.Path.GetFullPath(Log.AppDir + "\\" + path);
        //    }
        //    catch (Exception e)
        //    {
        //        LogMessage.Exit(e);
        //    }

        //    return null;
        //}
    }
}

