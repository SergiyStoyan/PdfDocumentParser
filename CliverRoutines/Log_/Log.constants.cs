//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Threading;
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
        static object lock_object = new object();

        static Log()
        {
            /*if (ProgramRoutines.IsWebContext) - crashes on Xamarin
                throw new Exception("Log is disabled in web context.");

            if (ProgramRoutines.IsWebContext)
                ProcessName = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).Name;
            else*/
                ProcessName = System.Reflection.Assembly.GetEntryAssembly().GetName(false).Name;

            AppDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            CompanyName = string.IsNullOrWhiteSpace(fvi.CompanyName) ? "CliverSoft" : fvi.CompanyName;

            CompanyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder./*CommonApplicationData - no write permission on macOS*/LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + CompanyName;
            AppCommonDataDir = CompanyCommonDataDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName;
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
                if (workDir == null)
                {
                    Thread deletingOldLogsThread = null;
                    lock (lock_object)
                    {
                        if (preWorkDir != null && System.IO.Path.IsPathRooted(preWorkDir))
                        {
                            workDir = preWorkDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + WorkDirPrefix;
                            if (writeLog && !Directory.Exists(workDir))
                                try
                                {
                                    //Directory.CreateDirectory(workDir);
                                    FileSystemRoutines.CreateDirectory(workDir);
                                }
                                catch(Exception e)
                                {
                                    preWorkDir = null;
                                }
                        }
                        if (string.IsNullOrWhiteSpace(workDir) || !Directory.Exists(workDir))
                        {
                            foreach (string baseDir in new string[] {
                                Log.AppDir,
                                CompanyCommonDataDir,
                                System.IO.Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + CompanyName + System.IO.Path.DirectorySeparatorChar,
                                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                            })
                            {
                                workDir = baseDir + System.IO.Path.DirectorySeparatorChar + preWorkDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + WorkDirPrefix;
                                if (!writeLog)
                                    break;
                                if (Directory.Exists(workDir))
                                    break;
                                try
                                {
                                    //Directory.CreateDirectory(workDir);
                                    FileSystemRoutines.CreateDirectory(workDir);
                                    if (Directory.Exists(workDir))
                                        break;
                                }
                                catch (Exception e)
                                { }
                            }
                        }
                        if (writeLog)
                            if (Directory.Exists(workDir) && deleteLogsOlderDays >= 0)
                                deletingOldLogsThread = startThread(() => { Log.DeleteOldLogs(deleteLogsOlderDays, ShowDeleteOldLogsDialog); });//to avoid a concurrent loop while accessing the log file from the same thread 
                            else
                                throw new Exception("Could not create log folder!");
                    }
                    workDir= PathRoutines.GetNormalizedPath(workDir, false);
                    // delete_old_logs?.Join();
                }
                return workDir;
            }
        }
        static string workDir = null;
        public const string WorkDirPrefix = @"_Sessions";
        static Thread startThread(ThreadStart code, bool background = true, ApartmentState state = ApartmentState.Unknown)
        {
            Thread t = new Thread(code);
            if (state != ApartmentState.Unknown)
                t.SetApartmentState(state);
            t.IsBackground = background;
            t.Start();
            return t;
        }
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
                if (mainSession == null)
                    mainSession = new Session();
                return mainSession;
            }
        }
        static Session mainSession = null;

        public static bool IsMainSessionOpen
        {
            get
            { 
                return mainSession != null;
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
                
                workDir = null;
                if (mainSession != null)
                    mainSession.Close();
                mainSession = null;

                GC.Collect();
            }
        }

        /// <summary>
        /// Deletes Log data from disk that is older than the specified threshold
        /// </summary>
        public static void DeleteOldLogs(int deleteLogsOlderDays, bool show_dialog)
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
                if (deleteLogsOlderDays > 0)
                {
                    DateTime FirstLogDate = DateTime.Now.AddDays(-deleteLogsOlderDays);

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
                                if (mainSession != null && d.FullName.StartsWith(mainSession.Path, StringComparison.InvariantCultureIgnoreCase))
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
        //        return System.IO.Path.GetFullPath(Log.AppDir + System.IO.Path.DirectorySeparatorChar + path);
        //    }
        //    catch (Exception e)
        //    {
        //        LogMessage.Exit(e);
        //    }

        //    return null;
        //}
    }
}

