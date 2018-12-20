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

namespace Cliver
{
    public static partial class ProcessRoutines
    {
        public static uint CreateProcessAsUserOfCurrentProcess(uint dwSessionId, String commandLine, WinApi.Advapi32.CreationFlags dwCreationFlags = 0, WinApi.Advapi32.STARTUPINFO? startupInfo = null)
        {
            Log.Main.Inform("Launching (in session " + dwSessionId + "):\r\n" + commandLine);

            IntPtr hNewProcessToken = IntPtr.Zero;
            IntPtr hProcessToken = IntPtr.Zero;
            try
            {
                WinApi.Advapi32.STARTUPINFO si;
                if (startupInfo != null)
                    si = (WinApi.Advapi32.STARTUPINFO)startupInfo;
                else
                    si = new WinApi.Advapi32.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                si.lpDesktop = "winsta0\\default";

                if (!WinApi.Advapi32.OpenProcessToken(Process.GetCurrentProcess().Handle, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, out hProcessToken))
                    throw new Exception("!OpenProcessToken. " + ErrorRoutines.GetLastError());

                var sa = new WinApi.Advapi32.SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);
                if (!WinApi.Advapi32.DuplicateTokenEx(hProcessToken, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, ref sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, ref hNewProcessToken))
                    throw new Exception("!DuplicateTokenEx. " + ErrorRoutines.GetLastError());

                if (!WinApi.Advapi32.SetTokenInformation(hNewProcessToken, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, (uint)IntPtr.Size))
                    throw new Exception("!SetTokenInformation. " + ErrorRoutines.GetLastError());

                WinApi.Advapi32.PROCESS_INFORMATION pi;
                if (!WinApi.Advapi32.CreateProcessAsUser(hNewProcessToken, // client's access token
                    null, // file to execute
                    commandLine, // command line
                    ref sa, // pointer to process SECURITY_ATTRIBUTES
                    ref sa, // pointer to thread SECURITY_ATTRIBUTES
                    false, // handles are not inheritable
                    dwCreationFlags, // creation flags
                    IntPtr.Zero,//pEnv, // pointer to new environment block 
                    null, // name of current directory 
                    ref si, // pointer to STARTUPINFO structure
                    out pi // receives information about new process
                    ))
                    throw new Exception("!CreateProcessAsUser. " + ErrorRoutines.GetLastError());
                return pi.dwProcessId;
            }
            //catch(Exception e)
            //{

            //}
            finally
            {
                if (hProcessToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hProcessToken);
                if (hNewProcessToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hNewProcessToken);
            }
        }

        public static uint CreateProcessAsUserOfCurrentSession(string appCmdLine /*,int processId*/)
        {
            IntPtr hToken = IntPtr.Zero;
            IntPtr envBlock = IntPtr.Zero;
            try
            {
                //Either specify the processID explicitly 
                //Or try to get it from a process owned by the user. 
                //In this case assuming there is only one explorer.exe 
                Process[] ps = Process.GetProcessesByName("explorer");
                int processId = -1;//=processId 
                if (ps.Length > 0)
                    processId = ps[0].Id;
                if (processId <= 1)
                    throw new Exception("processId <= 1: " + processId);

                hToken = getPrimaryToken(processId);
                if (hToken == IntPtr.Zero)
                    throw new Exception("!GetPrimaryToken. " + ErrorRoutines.GetLastError());

                if (!Cliver.WinApi.Userenv.CreateEnvironmentBlock(ref envBlock, hToken, false))
                    throw new Exception("!CreateEnvironmentBlock. " + ErrorRoutines.GetLastError());

                return createProcessAsUser(appCmdLine, hToken, envBlock);
            }
            //catch(Exception e)
            //{

            //}
            finally
            {
                if (envBlock != IntPtr.Zero)
                    Cliver.WinApi.Userenv.DestroyEnvironmentBlock(envBlock);
                if (hToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hToken);
            }
        }
        static uint createProcessAsUser(string cmdLine, IntPtr hToken, IntPtr envBlock)
        {
            try
            {
                WinApi.Advapi32.PROCESS_INFORMATION pi = new WinApi.Advapi32.PROCESS_INFORMATION();
                WinApi.Advapi32.SECURITY_ATTRIBUTES saProcess = new WinApi.Advapi32.SECURITY_ATTRIBUTES();
                WinApi.Advapi32.SECURITY_ATTRIBUTES saThread = new WinApi.Advapi32.SECURITY_ATTRIBUTES();
                saProcess.Length = Marshal.SizeOf(saProcess);
                saThread.Length = Marshal.SizeOf(saThread);

                WinApi.Advapi32.STARTUPINFO si = new WinApi.Advapi32.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);

                //if this member is NULL, the new process inherits the desktop 
                //and window station of its parent process. If this member is 
                //an empty string, the process does not inherit the desktop and 
                //window station of its parent process; instead, the system 
                //determines if a new desktop and window station need to be created. 
                //If the impersonated user already has a desktop, the system uses the 
                //existing desktop. 

                si.lpDesktop = @"WinSta0\Default"; //Modify as needed 
                si.dwFlags = WinApi.Advapi32.STARTF.USESHOWWINDOW | WinApi.Advapi32.STARTF.FORCEONFEEDBACK;
                si.wShowWindow = WinApi.User32.SW_SHOW;

                if (!WinApi.Advapi32.CreateProcessAsUser(
                    hToken,
                    null,
                    cmdLine,
                    ref saProcess,
                    ref saThread,
                    false,
                    WinApi.Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT,
                    envBlock,
                    null,
                    ref si,
                    out pi
                    ))
                    throw new Exception("!CreateProcessAsUser. " + ErrorRoutines.GetLastError());
                return pi.dwProcessId;
            }
            //catch(Exception e)
            //{

            //}
            finally
            {
            }
        }
        static IntPtr getPrimaryToken(int processId)
        {
            IntPtr hToken = IntPtr.Zero;
            try
            {
                IntPtr primaryToken = IntPtr.Zero;
                Process p = Process.GetProcessById(processId);

                //Gets impersonation token 
                if (!WinApi.Advapi32.OpenProcessToken(p.Handle, WinApi.Advapi32.DesiredAccess.TOKEN_DUPLICATE, out hToken))
                    throw new Exception("!OpenProcessToken. " + ErrorRoutines.GetLastError());

                WinApi.Advapi32.SECURITY_ATTRIBUTES sa = new WinApi.Advapi32.SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);

                //Convert the impersonation token into Primary token 
                if (!WinApi.Advapi32.DuplicateTokenEx(
                    hToken,
                    WinApi.Advapi32.DesiredAccess.TOKEN_ASSIGN_PRIMARY | WinApi.Advapi32.DesiredAccess.TOKEN_DUPLICATE | WinApi.Advapi32.DesiredAccess.TOKEN_QUERY,
                    ref sa,
                    WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                    WinApi.Advapi32.TOKEN_TYPE.TokenPrimary,
                    ref primaryToken
                    ))
                    throw new Exception("!DuplicateTokenEx. " + ErrorRoutines.GetLastError());
                return primaryToken;
            }
            //catch(Exception e)
            //{

            //}
            finally
            {
                if (hToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hToken);
            }
        }
    }
}