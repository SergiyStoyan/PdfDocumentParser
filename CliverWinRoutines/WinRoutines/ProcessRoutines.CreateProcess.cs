/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using Cliver.WinApi;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cliver.Win
{
    public static partial class ProcessRoutines
    {
        public class ProcessParameters
        {
            public bool ShowWindow = true;
            public bool NewConsole = true;
            public string Executable;
            public string Arguments;
            public string CurrentDirectory;
            public SafeFileHandle StdIn = default;
            public SafeFileHandle StdOut = default;
            public SafeFileHandle StdErr = default;

            public string GetCommandLine()
            {
                string fileName = Executable.Trim();
                bool fileNameIsQuoted = (fileName.StartsWith("\"", StringComparison.Ordinal) && fileName.EndsWith("\"", StringComparison.Ordinal));
                string commandLine = fileName;
                if (!fileNameIsQuoted)
                    commandLine = "\"" + commandLine + "\"";
                if (!string.IsNullOrEmpty(Arguments))
                    commandLine += " " + Arguments;
                return commandLine;
            }
        }

        //public class ProcessInfo : IDisposable
        //{
        //    public readonly uint Id = default;
        //    public IntPtr StdIn { get; internal set; } = default;
        //    public IntPtr StdOut { get; internal set; } = default;
        //    public IntPtr StdErr { get; internal set; } = default;

        //    public readonly Process Process;

        //    internal ProcessInfo(uint id)
        //    {
        //        Id = id;
        //        try
        //        {
        //            Process = Process.GetProcessById((int)Id);
        //        }
        //        catch (System.ArgumentException e)//throws an exception if the process is not running
        //        { }
        //    }

        //    public bool WriteIn(byte[] buffer, int offset, int count)
        //    {
        //        lock (this)
        //        {
        //            if (fileStreamIn == null)
        //            {
        //                if (StdIn == default)
        //                    throw new Exception("StdIn is not redirected.");
        //                SafeFileHandle h = new SafeFileHandle(StdIn, true);
        //                fileStreamIn = new FileStream(h, FileAccess.Write, BufferLength);
        //            }
        //            if (!fileStreamIn.CanWrite)
        //                return false;
        //            fileStreamIn.Write(buffer, offset, count);
        //            fileStreamIn.Flush();
        //            return true;
        //        }
        //    }
        //    FileStream fileStreamIn = null;
        //    public int BufferLength = 4096;

        //    public bool WriteIn2(string m)
        //    {
        //        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(m);
        //        uint dwWritten;
        //        System.Threading.NativeOverlapped o = new System.Threading.NativeOverlapped();
        //        bool r = Kernel32.WriteFile(new SafeFileHandle(StdIn, true), buffer, (uint)buffer.Length, out dwWritten, ref o);
        //        WinApi.Kernel32.CloseHandle(StdIn);
        //        return r;
        //    }

        //    public bool WriteIn(string m)
        //    {
        //        //byte[] bs = Process.StandardInput.Encoding.GetBytes(m);!!! does not work
        //        byte[] bs = System.Text.Encoding.ASCII.GetBytes(m);
        //        return WriteIn(bs, 0, bs.Length);
        //    }

        //    public int ReadOut(byte[] buffer, int offset, int count)
        //    {
        //        lock (this)
        //        {
        //            if (fileStreamOut == null)
        //            {
        //                if (StdOut == default)
        //                    throw new Exception("StdOut is not redirected.");
        //                SafeFileHandle h = new SafeFileHandle(StdOut, true);
        //                fileStreamOut = new FileStream(h, FileAccess.Read, BufferLength, false);
        //            }
        //            if (!fileStreamOut.CanRead)
        //                return -1;
        //            return fileStreamOut.Read(buffer, offset, count);
        //        }
        //    }
        //    FileStream fileStreamOut = null;

        //    System.Text.Encoding encoding
        //    {
        //        get
        //        {
        //            if (_encoding == null)
        //                _encoding = System.Text.Encoding.GetEncoding(Kernel32.GetConsoleOutputCP());
        //            return _encoding;
        //        }
        //    }
        //    System.Text.Encoding _encoding = null;

        //    public string ReadOut(int count)
        //    {
        //        byte[] bs = new byte[count];
        //        int r = ReadOut(bs, 0, count);
        //        return encoding.GetString(bs, 0, r);
        //    }

        //    //public string ReadOut(int count)
        //    //{
        //    //    byte[] bs = new byte[count];
        //    //    uint r;
        //    //    System.Threading.NativeOverlapped o = new System.Threading.NativeOverlapped();
        //    //    //Kernel32.ReadFile(new SafeFileHandle(StdOut, true), bs, 100, out r, ref o);
        //    //    bool c = Kernel32.ReadFile(StdOut, bs, (uint)count, out r, ref o);
        //    //    return System.Text.Encoding.ASCII.GetString(bs, 0, (int)r);
        //    //}

        //    public int ReadErr(byte[] buffer, int offset, int count)
        //    {
        //        lock (this)
        //        {
        //            if (fileStreamErr == null)
        //                fileStreamErr = new FileStream(new SafeFileHandle(StdErr, true), FileAccess.Read);
        //            if (!fileStreamErr.CanRead)
        //                return -1;
        //            return fileStreamErr.Read(buffer, offset, count);
        //        }
        //    }
        //    FileStream fileStreamErr = null;

        //    public string ReadErr(int count)
        //    {
        //        byte[] bs = new byte[count];
        //        int r = ReadErr(bs, 0, count);
        //        return encoding.GetString(bs, 0, r);
        //    }

        //    ~ProcessInfo()
        //    {
        //        Dispose();
        //    }

        //    public void Dispose()
        //    {
        //        lock (this)
        //        {
        //            if (StdIn != default)
        //            {
        //                Kernel32.CloseHandle(StdIn);
        //                if (fileStreamIn != null)
        //                {
        //                    fileStreamIn.Close();
        //                    fileStreamIn = null;
        //                }
        //            }
        //            if (StdOut != default)
        //            {
        //                Kernel32.CloseHandle(StdOut);
        //                if (fileStreamOut != null)
        //                {
        //                    fileStreamOut.Close();
        //                    fileStreamOut = null;
        //                }
        //            }
        //            if (StdErr != default)
        //            {
        //                Kernel32.CloseHandle(StdErr);
        //                if (fileStreamErr != null)
        //                {
        //                    fileStreamErr.Close();
        //                    fileStreamErr = null;
        //                }
        //            }
        //        }
        //    }
        //}


        public static int CreateProcess(ProcessParameters processParameters)
        {
            try
            {
                Advapi32.STARTUPINFO si = new Advapi32.STARTUPINFO();
                si.hStdInput = processParameters.StdIn != null ? processParameters.StdIn : Kernel32.GetStdHandle(Kernel32.StdHandles.STD_INPUT_HANDLE);
                si.hStdOutput = processParameters.StdOut != null ? processParameters.StdOut :Kernel32.GetStdHandle(Kernel32.StdHandles.STD_OUTPUT_HANDLE);
                si.hStdError = processParameters.StdErr != null ? processParameters.StdErr : Kernel32.GetStdHandle(Kernel32.StdHandles.STD_ERROR_HANDLE);
                if (processParameters.StdIn != null || processParameters.StdOut != null || processParameters.StdErr != null)
                    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
                si.dwFlags |= Advapi32.dwFlags.STARTF_USESHOWWINDOW; //it is needed if hiding the window when CREATE_NEW_CONSOLE
                si.wShowWindow = (short)(!processParameters.ShowWindow ? User32.SW.SW_HIDE : User32.SW.SW_SHOW);//what does it do???
                //si.lpDesktop = "winsta0\\default";

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                if (processParameters.StdIn != null || processParameters.StdOut != null || processParameters.StdErr != null)
                    sa.bInheritHandle = true;

                Advapi32.CreationFlags creationFlags = 0;
                //|=Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT
                if (processParameters.NewConsole)
                    creationFlags |= Advapi32.CreationFlags.CREATE_NEW_CONSOLE;
                if (!processParameters.ShowWindow)
                    creationFlags |= Advapi32.CreationFlags.CREATE_NO_WINDOW;

                Advapi32.PROCESS_INFORMATION pi = new Advapi32.PROCESS_INFORMATION();
                if (!Advapi32.CreateProcess(
                    processParameters.Executable, // file to execute
                    processParameters.Arguments, // command line 
                    null, // pointer to process SECURITY_ATTRIBUTES
                    null, // pointer to thread SECURITY_ATTRIBUTES
                    true, // handles are inheritable
                    creationFlags, // creation flags
                    IntPtr.Zero, // pointer to new environment block;  NULL = use parent's environment
                    Environment.CurrentDirectory, // name of current directory; NULL = use parent's current directory 
                    si, // pointer to STARTUPINFO structure
                    pi // receives information about new process
                    ))
                    throw new LastErrorException("!CreateProcess()");
                return pi.dwProcessId;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Run a process in the specified logon session as the user of the current process.
        /// </summary>
        /// <param name="dwSessionId">ID of the logon session where the process is to run</param>
        /// <param name="processParameters"></param>
        /// <returns></returns>
        public static int CreateProcessAsUserOfCurrentProcess(uint dwSessionId, ProcessParameters processParameters)
        {
            IntPtr hNewProcessToken = IntPtr.Zero;
            IntPtr hProcessToken = IntPtr.Zero;
            try
            {
                if (!Advapi32.OpenProcessToken(Process.GetCurrentProcess().Handle, Advapi32.DesiredAccess.MAXIMUM_ALLOWED, out hProcessToken))
                    throw new LastErrorException("!OpenProcessToken()");

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.bInheritHandle = true;
                if (!Advapi32.DuplicateTokenEx(hProcessToken, Advapi32.DesiredAccess.MAXIMUM_ALLOWED, sa, Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, Advapi32.TOKEN_TYPE.TokenPrimary, out hNewProcessToken))
                    throw new LastErrorException("!DuplicateTokenEx()");

                //is it needed to specify in which logon session the process is to run
                //!!!it usually needs to run with SYSTEM priviledges
                if (!Advapi32.SetTokenInformation(hNewProcessToken, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, (uint)IntPtr.Size))
                    throw new LastErrorException("!SetTokenInformation()");

                Advapi32.STARTUPINFO si = new Advapi32.STARTUPINFO();
                //si.lpDesktop = "winsta0\\default";
                si.wShowWindow = (short)(processParameters.ShowWindow ? User32.SW.SW_SHOW : User32.SW.SW_HIDE);//what does it do???

                Advapi32.CreationFlags creationFlags = processParameters.ShowWindow ? 0 : Advapi32.CreationFlags.CREATE_NO_WINDOW;

                Advapi32.PROCESS_INFORMATION pi = new Advapi32.PROCESS_INFORMATION();
                if (!Advapi32.CreateProcessAsUser(
                    hNewProcessToken, // client's access token
                    processParameters.Executable, // file to execute
                    processParameters.Arguments, // command line
                    null, // process SECURITY_ATTRIBUTES
                    null, // thread SECURITY_ATTRIBUTES
                    false, // You cannot inherit handles across sessions. Additionally, if this parameter is TRUE, you must create the process in the same session as the caller.
                    creationFlags, // creation flags
                    IntPtr.Zero, // pointer to new environment block 
                    processParameters.CurrentDirectory, // name of current directory 
                    si, // pointer to STARTUPINFO structure
                    pi // receives information about new process
                    ))
                    throw new Exception("!CreateProcessAsUser()", ErrorRoutines.GetLastError());
                return pi.dwProcessId;
            }
            finally
            {
                if (hProcessToken != IntPtr.Zero)
                    Kernel32.CloseHandle(hProcessToken);
                if (hNewProcessToken != IntPtr.Zero)
                    Kernel32.CloseHandle(hNewProcessToken);
            }
        }

        public static int CreateProcessAsUserOfCurrentSession(string appCmdLine /*,int processId*/)
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
        static int createProcessAsUser(string cmdLine, IntPtr hToken, IntPtr envBlock)
        {
            try
            {
                SECURITY_ATTRIBUTES saProcess = new SECURITY_ATTRIBUTES();
                SECURITY_ATTRIBUTES saThread = new SECURITY_ATTRIBUTES();
                saProcess.nLength = Marshal.SizeOf(saProcess);
                saThread.nLength = Marshal.SizeOf(saThread);

                Advapi32.STARTUPINFO si = new Advapi32.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);

                //if this member is NULL, the new process inherits the desktop 
                //and window station of its parent process. If this member is 
                //an empty string, the process does not inherit the desktop and 
                //window station of its parent process; instead, the system 
                //determines if a new desktop and window station need to be created. 
                //If the impersonated user already has a desktop, the system uses the 
                //existing desktop. 

                //si.lpDesktop = @"WinSta0\Default"; //Modify as needed 
                si.dwFlags = Advapi32.dwFlags.STARTF_USESHOWWINDOW | Advapi32.dwFlags.STARTF_FORCEONFEEDBACK;
                si.wShowWindow = WinApi.User32.SW_SHOW;

                Advapi32.PROCESS_INFORMATION pi = new Advapi32.PROCESS_INFORMATION();
                if (!Advapi32.CreateProcessAsUser(
                    hToken,
                    null,
                    cmdLine,
                    saProcess,
                    saThread,
                    false,
                    WinApi.Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT,
                    envBlock,
                    null,
                    si,
                    pi
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
                sa.nLength = Marshal.SizeOf(sa);

                //Convert the impersonation token into Primary token 
                if (!WinApi.Advapi32.DuplicateTokenEx(
                    hToken,
                    WinApi.Advapi32.DesiredAccess.TOKEN_ASSIGN_PRIMARY | WinApi.Advapi32.DesiredAccess.TOKEN_DUPLICATE | WinApi.Advapi32.DesiredAccess.TOKEN_QUERY,
                    sa,
                    WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                    WinApi.Advapi32.TOKEN_TYPE.TokenPrimary,
                    out primaryToken
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





        /// <summary>
        /// !!! to be checked
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="cmdLine"></param>
        /// <param name="workDir"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null, bool visible = true)
        {
            var hUserToken = IntPtr.Zero;
            Advapi32.STARTUPINFO startInfo = new Advapi32.STARTUPINFO();
            WinApi.Advapi32.PROCESS_INFORMATION procInfo = default;
            var pEnv = IntPtr.Zero;
            int iResultOfCreateProcessAsUser;
            startInfo.cb = Marshal.SizeOf(typeof(Advapi32.STARTUPINFO));

            try
            {
                if (!getActiveSessionUserToken(ref hUserToken))
                    throw new Exception("StartProcessAsCurrentUser: GetSessionUserToken failed.");

                Advapi32.CreationFlags dwCreationFlags = Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT | (visible ? Advapi32.CreationFlags.CREATE_NEW_CONSOLE : Advapi32.CreationFlags.CREATE_NO_WINDOW);
                startInfo.wShowWindow = (short)(visible ? User32.SW.SW_SHOW : User32.SW.SW_HIDE);
                //startInfo.lpDesktop = "winsta0\\default";

                if (!Userenv.CreateEnvironmentBlock(ref pEnv, hUserToken, false))
                    throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");

                if (!Advapi32.CreateProcessAsUser(hUserToken,
                    appPath, // Application Name
                    cmdLine, // Command Line
                    null,
                    null,
                    false,
                    dwCreationFlags,
                    pEnv,
                    workDir, // Working directory
                    startInfo,
                    procInfo))
                {
                    iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
                    throw new Exception("StartProcessAsCurrentUser: CreateProcessAsUser failed.  Error Code -" + iResultOfCreateProcessAsUser);
                }

                iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
            }
            finally
            {
                Kernel32.CloseHandle(hUserToken);
                if (pEnv != IntPtr.Zero)
                {
                    Userenv.DestroyEnvironmentBlock(pEnv);
                }
                Kernel32.CloseHandle(procInfo.hThread);
                Kernel32.CloseHandle(procInfo.hProcess);
            }

            return true;
        }
        private static bool getActiveSessionUserToken(ref IntPtr phUserToken)
        {
            var bResult = false;
            var hImpersonationToken = IntPtr.Zero;
            var activeSessionId = Wts.INVALID_SESSION_ID;
            var pSessionInfo = IntPtr.Zero;
            var sessionCount = 0;

            // Get a handle to the user access token for the current active session.
            if (Wts.WTSEnumerateSessions(Wts.WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
            {
                var arrayElementSize = Marshal.SizeOf(typeof(Wts.WTS_SESSION_INFO));
                var current = pSessionInfo;

                for (var i = 0; i < sessionCount; i++)
                {
                    var si = (Wts.WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(Wts.WTS_SESSION_INFO));
                    current += arrayElementSize;

                    if (si.State == Wts.WTS_CONNECTSTATE_CLASS.WTSActive)
                        activeSessionId = si.SessionID;
                }
            }

            // If enumerating did not work, fall back to the old method
            if (activeSessionId == Wts.INVALID_SESSION_ID)
            {
                activeSessionId = Wts.WTSGetActiveConsoleSessionId();
            }

            if (Wts.WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
            {
                // Convert the impersonation token to a primary token
                var sa = new WinApi.SECURITY_ATTRIBUTES();
                bResult = Advapi32.DuplicateTokenEx(hImpersonationToken, 0, sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, out phUserToken);
                Kernel32.CloseHandle(hImpersonationToken);
            }

            return bResult;
        }
    }
}