//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Security.AccessControl;

namespace Cliver
{
    public static partial class ProcessRoutines
    {
        public static bool IsProgramRunningAlready()
        {
            Process p = Process.GetCurrentProcess();
            return (from x in Process.GetProcessesByName(p.ProcessName) where x.MainModule.FileName == p.MainModule.FileName select x).Count() > 1;
        }

        public static void RunSingleProcessOnly(bool silent = false)
        {
            string app_name = ProgramRoutines.GetAppName();
            bool createdNew;
            MutexSecurity mutexSecurity = new System.Security.AccessControl.MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    GLOBAL_SINGLE_PROCESS_MUTEX = new Mutex(false, @"Global\CLIVERSOFT_" + app_name + @"_SINGLE_PROCESS", out createdNew, mutexSecurity);
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
                    if (!silent)
                        LogMessage.Exit(e);
                    else
                        Environment.Exit(0);
                }
            }
            if (GLOBAL_SINGLE_PROCESS_MUTEX.WaitOne(1000, false))//wait for some time while contending, if the other instance of the program is still in progress of shutting down.
                return;
            if (!silent)
                LogMessage.Exit2(app_name + " is already running, so this instance will exit.");
            else
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

        public static void Restart(bool as_administarator = false, string arguments = null)
        {
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

        public static bool IsProcessAlive(int process_id)
        {
            return GetProcess(process_id) != null;
        }

        public static bool IsProcessAlive(Process process)
        {
            return GetProcess(process.Id) != null;
        }

        public static Process GetProcess(int process_id)
        {
            try
            {
                return Process.GetProcessById(process_id);
            }
            catch
            {
                return null;
            }
        }

        public static bool KillProcessTree(int process_id)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + process_id);
            foreach (ManagementObject mo in searcher.Get())
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));

            Process p;
            try
            {
                p = Process.GetProcessById(process_id);
            }
            catch
            {
                return true;
            }
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    p.Kill();
                }
                catch
                {
                    // Process already exited.
                    return true;
                }
                p.WaitForExit(1000);
                if (!IsProcessRunning(p))
                    return true;
            }
            return false;
        }

        public static bool IsProcessRunning(Process p)
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
                throw e;
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
    }
}