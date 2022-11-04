/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/


using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;
using System.Security.AccessControl;
using System.Collections.Generic;

namespace Cliver.Win
{
    public static partial class ProcessRoutines
    {
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
            bool createdNew;
            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    GLOBAL_SINGLE_PROCESS_MUTEX = new Mutex(false, @"Global\CLIVERSOFT_" + appName + @"_SINGLE_PROCESS", out createdNew, mutexSecurity);
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
            psi.FileName = Application.ExecutablePath;
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

        public static Process GetProcess(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch (System.ArgumentException e)//throws an exception if the process exited
            {
                return null;
            }
        }

        public static bool TryKillProcessTree(int processId, int timeoutMss = 1000, int pollTimeSpanMss = 300)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processId);
            foreach (ManagementObject mo in searcher.Get())
                if (!TryKillProcessTree(Convert.ToInt32(mo["ProcessID"]), timeoutMss, pollTimeSpanMss))
                    return false;

            return TryKillProcess(processId);
        }

        public static bool TryKillProcessTree(this Process process, int timeoutMss = 1000, int pollTimeSpanMss = 300)
        {
            return TryKillProcessTree(process.Id, timeoutMss, pollTimeSpanMss);
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

        /// <summary>
        /// !!! UGLY WAY !!!
        /// Make the host process a system-critical process so that it cannot be terminated without causing a shutdown of the entire system.
        /// </summary>
        //public static class CurrentProcessProtection
        //{
        //    [DllImport("ntdll.dll", SetLastError = true)]
        //    //undocumented functionality making the host process unkillable
        //    private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        //    static object lockObject = new object();

        //    public static bool On
        //    {
        //        get
        //        {
        //            return On;
        //        }
        //        set
        //        {
        //            lock (lockObject)
        //            {
        //                if (value)
        //                {
        //                    if (!on)
        //                    {
        //                        System.Diagnostics.Process.EnterDebugMode();
        //                        RtlSetProcessIsCritical(1, 0, 0);
        //                        on = true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (on)
        //                    {
        //                        RtlSetProcessIsCritical(0, 0, 0);
        //                        on = false;
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    static volatile bool on = false;
        //}

        public static WindowsIdentity GetWindowsIdentityOfProcessUser(Process process = null)
        {
            if (process == null)
                process = Process.GetCurrentProcess();

            IntPtr processHandle = IntPtr.Zero;
            try
            {
                WinApi.Advapi32.OpenProcessToken(process.Handle, WinApi.Advapi32.DesiredAccess.TOKEN_QUERY, out processHandle);
                return new WindowsIdentity(processHandle);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(processHandle);
            }
        }

        public static string GetProcessUserName(Process process = null)
        {
            if (process == null)
                process = Process.GetCurrentProcess();
            WindowsIdentity wi = GetWindowsIdentityOfProcessUser(process);
            string user = wi.Name;
            return Regex.Replace(wi.Name, @".*\\", "");
        }

        public static bool ProcessIsSystem(Process process = null)
        {
            using (var identity = GetWindowsIdentityOfProcessUser(process))
            {
                return identity.IsSystem;
            }
        }

        public static bool ProcessHasElevatedPrivileges(Process process = null)
        {
            if (process == null)
                process = Process.GetCurrentProcess();

            if (IsUacEnabled)
            {
                IntPtr tokenHandle;
                if (!WinApi.Advapi32.OpenProcessToken(process.Handle, WinApi.Advapi32.DesiredAccess.STANDARD_RIGHTS_READ | WinApi.Advapi32.DesiredAccess.TOKEN_QUERY, out tokenHandle))
                    throw new ApplicationException("Could not get process token. Win32 Error Code: " + Marshal.GetLastWin32Error());

                WinApi.Advapi32.TOKEN_ELEVATION_TYPE elevationResult = WinApi.Advapi32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;

                int elevationResultSize = Marshal.SizeOf((int)elevationResult);
                IntPtr tokenInformation = Marshal.AllocHGlobal(elevationResultSize);
                try
                {
                    uint returnedSize = 0;
                    if (!WinApi.Advapi32.GetTokenInformation(tokenHandle, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenElevationType, tokenInformation, (uint)elevationResultSize, out returnedSize))
                        throw new ApplicationException("Unable to determine the current elevation.");
                    return (WinApi.Advapi32.TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(tokenInformation) == WinApi.Advapi32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                }
                finally
                {
                    if (tokenInformation != IntPtr.Zero)
                        Marshal.FreeHGlobal(tokenInformation);
                }
            }
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool IsUacEnabled
        {
            get
            {
                Microsoft.Win32.RegistryKey uacKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
                bool result = uacKey.GetValue(uacRegistryValue).Equals(1);
                return result;
            }
        }
        private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        private const string uacRegistryValue = "EnableLUA";

        public static Process GetParentProcess(int processId)
        {
            Process process = Process.GetProcessById(processId);
            ParentProcessUtilities pbi = new ParentProcessUtilities();
            int status = NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf(pbi), out int returnLength);
            if (status != 0)
                throw new System.ComponentModel.Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);
        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            // These members must match PROCESS_BASIC_INFORMATION
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        }
    }
}