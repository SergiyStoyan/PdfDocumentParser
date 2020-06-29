//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using Cliver.WinApi;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Cliver.Win
{
    public static partial class ProcessRoutines
    {
        public enum CreationFlags2
        {
            REDIRECT_STDIN = 0x00000001,
            REDIRECT_STDOUT = 0x00000002,
            REDIRECT_STDERR = 0x00000004,
            REDIRECT_STDERR_TO_STDOUT = 0x00000005,
        }
        public class ProcessParameters
        {
            public Advapi32.CreationFlags CreationFlags = 0;
            public CreationFlags2 CreationFlags2 = 0;
            public Advapi32.STARTUPINFO? StartupInfo = null;
            public string CommandLine;
        }
        public class ProcessInfo : IDisposable
        {
            public uint Id = default;
            public IntPtr StdIn = default;
            public IntPtr StdOut = default;
            public IntPtr StdErr = default;
            public Process Process
            {
                get
                {
                    if (process == null && Id > 0)
                        process = Process.GetProcessById((int)Id);
                    return process;
                }
            }
            Process process = null;

            public bool Write(byte[] buffer)
            {
                if (fileStreamIn == null)
                    fileStreamIn = new FileStream(new SafeFileHandle(StdIn, true), FileAccess.Write);
                if (!fileStreamIn.CanWrite)
                    return false;
                fileStreamIn.Write(buffer, 0, buffer.Length);
                fileStreamIn.Flush();//!!! denied !!!
                return true;
            }
            FileStream fileStreamIn = null;

            public void Dispose()
            {
                if (StdIn != default)
                {
                    Kernel32.CloseHandle(StdIn);
                    StdIn = default;
                    if (fileStreamIn != null)
                    {
                        fileStreamIn.Close();
                        fileStreamIn = null;
                    }
                }
                if (StdOut != default)
                {
                    Kernel32.CloseHandle(StdOut);
                    StdOut = default;
                }
                if (StdErr != default)
                {
                    Kernel32.CloseHandle(StdErr);
                    StdErr = default;
                }
            }
        }
        public static ProcessInfo CreateProcessAsUserOfCurrentProcess(uint dwSessionId, ProcessParameters processParameters)
        {
            IntPtr hNewProcessToken = IntPtr.Zero;
            IntPtr hProcessToken = IntPtr.Zero;
            try
            {
                WinApi.Advapi32.STARTUPINFO si;
                if (processParameters.StartupInfo != null)
                    si = (WinApi.Advapi32.STARTUPINFO)processParameters.StartupInfo;
                else
                    si = new WinApi.Advapi32.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                si.lpDesktop = "winsta0\\default";

                if (!WinApi.Advapi32.OpenProcessToken(Process.GetCurrentProcess().Handle, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, out hProcessToken))
                    throw new Exception("!OpenProcessToken()", ErrorRoutines.GetLastError());

                var sa = new WinApi.SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);
                if (!WinApi.Advapi32.DuplicateTokenEx(hProcessToken, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, ref sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, ref hNewProcessToken))
                    throw new Exception("!DuplicateTokenEx()", ErrorRoutines.GetLastError());

                //!!! is it needed ???
                //if (!WinApi.Advapi32.SetTokenInformation(hNewProcessToken, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, (uint)IntPtr.Size))
                //    throw new Exception("!SetTokenInformation()", ErrorRoutines.GetLastError());

                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDIN))
                {
                    if (!WinApi.Kernel32.CreatePipe(out IntPtr stdInRead, out IntPtr stdInWrite, ref sa, 0))
                        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
                    // Ensure the write handle to the pipe for STDIN is not inherited. 
                    if (!WinApi.Kernel32.SetHandleInformation(stdInWrite, WinApi.Kernel32.HANDLE_FLAG_INHERIT, 0))
                        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
                    si.hStdInput = stdInRead;
                    si.dwFlags |= WinApi.Advapi32.STARTF.USESTDHANDLES;
                }

                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDOUT))
                {
                    if (!WinApi.Kernel32.CreatePipe(out IntPtr stdOutRead, out IntPtr stdOutWrite, ref sa, 0))
                        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
                    // Ensure the write handle to the pipe for STDIN is not inherited. 
                    if (!WinApi.Kernel32.SetHandleInformation(stdOutRead, WinApi.Kernel32.HANDLE_FLAG_INHERIT, 0))
                        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
                    si.hStdOutput = stdOutWrite;
                    if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR_TO_STDOUT))
                        si.hStdError = stdOutWrite;
                    si.dwFlags |= WinApi.Advapi32.STARTF.USESTDHANDLES;
                }

                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR))
                {
                    if (!WinApi.Kernel32.CreatePipe(out IntPtr stdErrRead, out IntPtr stdErrWrite, ref sa, 0))
                        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
                    // Ensure the write handle to the pipe for STDIN is not inherited. 
                    if (!WinApi.Kernel32.SetHandleInformation(stdErrRead, WinApi.Kernel32.HANDLE_FLAG_INHERIT, 0))
                        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
                    si.hStdError = stdErrWrite;
                    si.dwFlags |= WinApi.Advapi32.STARTF.USESTDHANDLES;
                }

                WinApi.Advapi32.PROCESS_INFORMATION pi;
                if (!WinApi.Advapi32.CreateProcessAsUser(hNewProcessToken, // client's access token
                    null, // file to execute
                    processParameters.CommandLine, // command line
                    ref sa, // pointer to process SECURITY_ATTRIBUTES
                    ref sa, // pointer to thread SECURITY_ATTRIBUTES
                    false, // handles are not inheritable
                  processParameters.CreationFlags, // creation flags
                    IntPtr.Zero,//pEnv, // pointer to new environment block 
                    null, // name of current directory 
                    ref si, // pointer to STARTUPINFO structure
                    out pi // receives information about new process
                    ))
                    throw new Exception("!CreateProcessAsUser()", ErrorRoutines.GetLastError());
                return new ProcessInfo { Id = pi.dwProcessId, StdIn = si.hStdInput, StdOut = si.hStdOutput, StdErr = si.hStdError };
            }
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
                    throw new Exception("!GetPrimaryToken()", ErrorRoutines.GetLastError());

                if (!Cliver.WinApi.Userenv.CreateEnvironmentBlock(ref envBlock, hToken, false))
                    throw new Exception("!CreateEnvironmentBlock()", ErrorRoutines.GetLastError());

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
                WinApi.SECURITY_ATTRIBUTES saProcess = new WinApi.SECURITY_ATTRIBUTES();
                WinApi.SECURITY_ATTRIBUTES saThread = new WinApi.SECURITY_ATTRIBUTES();
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
                    throw new Exception("!CreateProcessAsUser()", ErrorRoutines.GetLastError());
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
                    throw new Exception("!OpenProcessToken()", ErrorRoutines.GetLastError());

                WinApi.SECURITY_ATTRIBUTES sa = new WinApi.SECURITY_ATTRIBUTES();
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
                    throw new Exception("!DuplicateTokenEx()", ErrorRoutines.GetLastError());
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