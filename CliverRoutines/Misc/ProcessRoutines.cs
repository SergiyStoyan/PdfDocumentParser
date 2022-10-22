/********************************************************************************************
        Author: Sergiy Stoyan
        systoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace Cliver
{
    public static partial class ProcessRoutines
    {
        static public string GetFile(Process p = null)
        {
            if (p == null)
                p = Process.GetCurrentProcess();
            return p.MainModule.FileName;
        }

        static public string GetNameWithExtension(Process p = null)
        {
            return PathRoutines.GetFileName(GetFile(p));
        }

        /// <summary>
        /// Opens a file in editor/veiwer determined by the file's extension.
        /// </summary>
        /// <param name="file"></param>
        public static void Open(string file)
        {
            Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
        }

        public static IEnumerable<Process> GetProcesses(string exeFile)
        {
            string exeFileDir = PathRoutines.GetFileDir(exeFile).ToLower();
            return Process.GetProcessesByName(PathRoutines.GetFileNameWithoutExtention(exeFile)).Where(p =>
            {
                ProcessModule pm;
                try
                {
                    pm = p.MainModule;
                }
                catch//sometimes it throws exception (if the process exited?)
                {
                    pm = null;
                }
                return pm == null ? false : pm.FileName.StartsWith(exeFileDir, StringComparison.InvariantCultureIgnoreCase);
            }
            );
        }

        public static void RunMeInSingleProcessOnly(Action<string> exitingMessage)
        {
            string appName = ProgramRoutines.GetAppName();
            //MutexSecurity mutexSecurity = new MutexSecurity();
            //mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    GLOBAL_SINGLE_PROCESS_MUTEX = new Mutex(false, @"Global\CLIVERSOFT_" + appName + @"_SINGLE_PROCESS", out _/*, mutexSecurity*/);
                    break;
                }
                catch (Exception e)
                {//An “access denied” while creating a new Mutex can happen in the following situation:
                 //a.Process A running as an administrator creates a named mutex.
                 //b.Process B running as a normal user attempts to access the mutex which fails with “access denied” since only Administrators can access the mutex.
                    if (i == 0)
                    {
                        Thread.Sleep(1000);//wait for some time while contending, if the other instance of the program is still in progress of shutting down.
                        continue;
                    }
                    exitingMessage?.Invoke(Log.GetExceptionMessage(e) + "\r\n\r\nExiting...");
                    Environment.Exit(0);
                }
            }
            if (GLOBAL_SINGLE_PROCESS_MUTEX.WaitOne(1000, false))//wait for some time while contending, if the other instance of the program is still in progress of shutting down.
                return;
            exitingMessage?.Invoke(appName + " is already running, so this instance will exit.");
            Environment.Exit(0);
        }
        static Mutex GLOBAL_SINGLE_PROCESS_MUTEX = null;

        /// <summary>
        /// Must be invoked if this process needs some time to exit.
        /// </summary>
        public static void Exit()
        {
            if (GLOBAL_SINGLE_PROCESS_MUTEX != null)
                GLOBAL_SINGLE_PROCESS_MUTEX.ReleaseMutex();
            Environment.Exit(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as_administarator"></param>
        /// <param name="arguments">if NULL then reuse parameters of the calling process</param>
        public static void Restart(bool as_administarator = false, string arguments = "")
        {
            if (arguments == null)
                arguments = Regex.Replace(Environment.CommandLine, @"^\s*(\'.*?\'|\"".*?\"")\s+", "", RegexOptions.Singleline);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.FileName = ProgramRoutines.GetAppPath();
            if (as_administarator)
                psi.Verb = "runas";
            psi.Arguments = arguments;
            if (GLOBAL_SINGLE_PROCESS_MUTEX != null)
                GLOBAL_SINGLE_PROCESS_MUTEX.ReleaseMutex();
            try
            {
                Process.Start(psi);
            }
            catch
            { //if the user cancelled
            }
            Environment.Exit(0);
        }

        //public static bool IsProcessAlive(int process_id)
        //{
        //    return GetProcess(process_id) != null;
        //}

        //public static bool IsProcessAlive(this Process process)
        //{
        //    return GetProcess(process.Id) != null;
        //}

        public static Process GetProcess(int? processId = null)
        {
            try
            {
                if (processId != null)
                    return Process.GetProcessById(processId.Value);
                return Process.GetCurrentProcess();
            }
            catch (System.ArgumentException e)//throws an exception if the process exited
            {
                return null;
            }
        }

        public static bool TryKillProcess(int processId, int timeoutMss = 1000, int pollTimeSpanMss = 300)
        {
            Process p;
            try
            {
                p = Process.GetProcessById(processId);
            }
            catch
            {
                return true;
            }
            return p.TryKill(timeoutMss, pollTimeSpanMss);
        }

        public static bool TryKill(this Process process, int timeoutMss = 1000, int pollTimeSpanMss = 300)
        {
            return SleepRoutines.WaitForCondition(() =>
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    // Process already exited.
                    return true;
                }
                process.WaitForExit(pollTimeSpanMss);
                return !process.IsRunning();
            },
                timeoutMss
                );
        }

        /// <summary>
        /// Safe method instead of HasExited
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsRunning(this Process p)
        {
            try
            {
                return !p.HasExited;
            }
            catch
            {//it was not started
                return false;
            }
        }
    }
}