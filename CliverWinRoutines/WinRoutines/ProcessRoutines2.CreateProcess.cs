////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        sergey_stoyan@yahoo.com
////        http://www.cliversoft.com
////        26 September 2006
////Copyright: (C) 2006, Sergey Stoyan
////********************************************************************************************
//using Cliver.WinApi;
//using Microsoft.Win32.SafeHandles;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Runtime.InteropServices;
//using Microsoft.Win32;

//namespace Cliver.Win
//{

//    public static partial class ProcessRoutines2
//    {

//        //public static void CreateProcess(SafeFileHandle hStdInput)//works
//        //{
//        //    IntPtr hNewProcessToken = IntPtr.Zero;
//        //    IntPtr hProcessToken = IntPtr.Zero;
//        //    SafeFileHandle stdInWrite = null;//, stdOutRead = null, stdErrRead = null;
//        //    try
//        //    {
//        //        NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();

//        //        //si.hStdInput =new SafeFileHandle( NativeMethods.GetStdHandle(Kernel32.STD_INPUT_HANDLE),true);
//        //        si.hStdOutput = new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE), true);
//        //        si.hStdError = new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_ERROR_HANDLE), true);
//        //        si.dwFlags = 256;// Advapi32.dwFlags.STARTF_USESTDHANDLES;// | Advapi32.dwFlags.STARTF_USESHOWWINDOW;

//        //        NativeMethods.SECURITY_ATTRIBUTES sa = new NativeMethods.SECURITY_ATTRIBUTES();
//        //        sa.bInheritHandle = true;
//        //        //if (!NativeMethods.CreatePipe(out si.hStdInput, out stdInWrite, sa, 0))
//        //        //    throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//        //        ////Ensure the write handle to the pipe for STDIN is not inherited. 
//        //        //if (!NativeMethods.SetHandleInformation(stdInWrite, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//        //        //    throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//        //        //if (!NativeMethods.SetHandleInformation(stdInRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//        //        //    throw new Exception("!SetHandleInformation()2", ErrorRoutines.GetLastError());
//        //        si.hStdInput = hStdInput;

//        //        SafeNativeMethods.PROCESS_INFORMATION pi = new SafeNativeMethods.PROCESS_INFORMATION();
//        //        if (!NativeMethods.CreateProcess(
//        //            null,//Environment.SystemDirectory + "\\cmd.exe", // file to execute
//        //            new System.Text.StringBuilder(Environment.SystemDirectory + "\\cmd.exe"), //  @"C:\Far\Far.exe",// null, // command line
//        //            null, // pointer to process SECURITY_ATTRIBUTES
//        //            null, // pointer to thread SECURITY_ATTRIBUTES
//        //            true, // handles are inheritable
//        //            0,//   Advapi32.CreationFlags.CREATE_NEW_CONSOLE, // |Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT // creation flags
//        //            IntPtr.Zero, // pointer to new environment block;  NULL = use parent's environment
//        //            Environment.CurrentDirectory, // name of current directory; NULL = use parent's current directory 
//        //            si, // pointer to STARTUPINFO structure
//        //            pi // receives information about new process
//        //            ))
//        //            throw new Exception("!CreateProcess()", ErrorRoutines.GetLastError());
//        //    }
//        //    finally
//        //    {
//        //        if (hProcessToken != IntPtr.Zero)
//        //            Kernel32.CloseHandle(hProcessToken);
//        //        if (hNewProcessToken != IntPtr.Zero)
//        //            Kernel32.CloseHandle(hNewProcessToken);
//        //    }

//        //    //StreamWriter standardInput = new StreamWriter(new FileStream(stdInWrite, FileAccess.Write, 4096, isAsync: false), Console.InputEncoding, 4096);
//        //    //standardInput.AutoFlush = true;

//        //    //standardInput.WriteLine("qwerty###");
//        //}

//        //public static void CreateProcess()//STDIN redirection works
//        //{
//        //    IntPtr hNewProcessToken = IntPtr.Zero;
//        //    IntPtr hProcessToken = IntPtr.Zero;
//        //    SafeFileHandle stdInWrite = null;//, stdOutRead = null, stdErrRead = null;
//        //    try
//        //    {
//        //        NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();

//        //        //si.hStdInput =new SafeFileHandle( NativeMethods.GetStdHandle(Kernel32.STD_INPUT_HANDLE),true);
//        //        si.hStdOutput = new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE), true);
//        //        si.hStdError = new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_ERROR_HANDLE), true);
//        //        si.dwFlags = 256;// Advapi32.dwFlags.STARTF_USESTDHANDLES;// | Advapi32.dwFlags.STARTF_USESHOWWINDOW;

//        //        NativeMethods.SECURITY_ATTRIBUTES sa = new NativeMethods.SECURITY_ATTRIBUTES();
//        //        sa.bInheritHandle = true;
//        //        if (!NativeMethods.CreatePipe(out si.hStdInput, out stdInWrite, sa, 0))
//        //            throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//        //        ////Ensure the write handle to the pipe for STDIN is not inherited. 
//        //        //if (!NativeMethods.SetHandleInformation(stdInWrite, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//        //        //    throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//        //        //if (!NativeMethods.SetHandleInformation(stdInRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//        //        //    throw new Exception("!SetHandleInformation()2", ErrorRoutines.GetLastError());

//        //        SafeNativeMethods.PROCESS_INFORMATION pi = new SafeNativeMethods.PROCESS_INFORMATION();
//        //        if (!NativeMethods.CreateProcess(
//        //         null,//Environment.SystemDirectory + "\\cmd.exe", // file to execute
//        //    new System.Text.StringBuilder(Environment.SystemDirectory + "\\cmd.exe"), //  @"C:\Far\Far.exe",// null, // command line
//        //            null, // pointer to process SECURITY_ATTRIBUTES
//        //            null, // pointer to thread SECURITY_ATTRIBUTES
//        //            true, // handles are inheritable
//        //      0,//   Advapi32.CreationFlags.CREATE_NEW_CONSOLE, // |Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT // creation flags
//        //           IntPtr.Zero, // pointer to new environment block;  NULL = use parent's environment
//        //            Environment.CurrentDirectory, // name of current directory; NULL = use parent's current directory 
//        //            si, // pointer to STARTUPINFO structure
//        //            pi // receives information about new process
//        //            ))
//        //            throw new Exception("!CreateProcess()", ErrorRoutines.GetLastError());
//        //    }
//        //    finally
//        //    {
//        //        if (hProcessToken != IntPtr.Zero)
//        //            Kernel32.CloseHandle(hProcessToken);
//        //        if (hNewProcessToken != IntPtr.Zero)
//        //            Kernel32.CloseHandle(hNewProcessToken);
//        //    }

//        //    StreamWriter standardInput = new StreamWriter(new FileStream(stdInWrite, FileAccess.Write, 4096, isAsync: false), Console.InputEncoding, 4096);
//        //    standardInput.AutoFlush = true;

//        //    standardInput.WriteLine("qwerty###");
//        //}



//        public class ProcessInfo : IDisposable
//        {
//            public readonly uint Id = default;
//            public IntPtr StdIn { get; internal set; } = default;
//            public IntPtr StdOut { get; internal set; } = default;
//            public IntPtr StdErr { get; internal set; } = default;

//            public readonly Process Process;

//            internal ProcessInfo(uint id)
//            {
//                Id = id;
//                try
//                {
//                    Process = Process.GetProcessById((int)Id);
//                }
//                catch (System.ArgumentException e)//throws an exception if the process is not running
//                { }
//            }

//            public bool WriteIn(byte[] buffer, int offset, int count)
//            {
//                lock (this)
//                {
//                    if (fileStreamIn == null)
//                    {
//                        if (StdIn == default)
//                            throw new Exception("StdIn is not redirected.");
//                        SafeFileHandle h = new SafeFileHandle(StdIn, true);
//                        fileStreamIn = new FileStream(h, FileAccess.Write, BufferLength);
//                    }
//                    if (!fileStreamIn.CanWrite)
//                        return false;
//                    fileStreamIn.Write(buffer, offset, count);
//                    fileStreamIn.Flush();
//                    return true;
//                }
//            }
//            FileStream fileStreamIn = null;
//            public int BufferLength = 4096;

//            public bool WriteIn2(string m)
//            {
//                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(m);
//                uint dwWritten;
//                System.Threading.NativeOverlapped o = new System.Threading.NativeOverlapped();
//                bool r = Kernel32.WriteFile(new SafeFileHandle(StdIn, true), buffer, (uint)buffer.Length, out dwWritten, ref o);
//                WinApi.Kernel32.CloseHandle(StdIn);
//                return r;
//            }

//            public bool WriteIn(string m)
//            {
//                //byte[] bs = Process.StandardInput.Encoding.GetBytes(m);!!! does not work
//                byte[] bs = System.Text.Encoding.ASCII.GetBytes(m);
//                return WriteIn(bs, 0, bs.Length);
//            }

//            public int ReadOut(byte[] buffer, int offset, int count)
//            {
//                lock (this)
//                {
//                    if (fileStreamOut == null)
//                    {
//                        if (StdOut == default)
//                            throw new Exception("StdOut is not redirected.");
//                        SafeFileHandle h = new SafeFileHandle(StdOut, true);
//                        fileStreamOut = new FileStream(h, FileAccess.Read, BufferLength, false);
//                    }
//                    if (!fileStreamOut.CanRead)
//                        return -1;
//                    return fileStreamOut.Read(buffer, offset, count);
//                }
//            }
//            FileStream fileStreamOut = null;

//            System.Text.Encoding encoding
//            {
//                get
//                {
//                    if (_encoding == null)
//                        _encoding = System.Text.Encoding.GetEncoding(Kernel32.GetConsoleOutputCP());
//                    return _encoding;
//                }
//            }
//            System.Text.Encoding _encoding = null;

//            public string ReadOut(int count)
//            {
//                byte[] bs = new byte[count];
//                int r = ReadOut(bs, 0, count);
//                return encoding.GetString(bs, 0, r);
//            }

//            //public string ReadOut(int count)
//            //{
//            //    byte[] bs = new byte[count];
//            //    uint r;
//            //    System.Threading.NativeOverlapped o = new System.Threading.NativeOverlapped();
//            //    //Kernel32.ReadFile(new SafeFileHandle(StdOut, true), bs, 100, out r, ref o);
//            //    bool c = Kernel32.ReadFile(StdOut, bs, (uint)count, out r, ref o);
//            //    return System.Text.Encoding.ASCII.GetString(bs, 0, (int)r);
//            //}

//            public int ReadErr(byte[] buffer, int offset, int count)
//            {
//                lock (this)
//                {
//                    if (fileStreamErr == null)
//                        fileStreamErr = new FileStream(new SafeFileHandle(StdErr, true), FileAccess.Read);
//                    if (!fileStreamErr.CanRead)
//                        return -1;
//                    return fileStreamErr.Read(buffer, offset, count);
//                }
//            }
//            FileStream fileStreamErr = null;

//            public string ReadErr(int count)
//            {
//                byte[] bs = new byte[count];
//                int r = ReadErr(bs, 0, count);
//                return encoding.GetString(bs, 0, r);
//            }

//            ~ProcessInfo()
//            {
//                Dispose();
//            }

//            public void Dispose()
//            {
//                lock (this)
//                {
//                    if (StdIn != default)
//                    {
//                        Kernel32.CloseHandle(StdIn);
//                        if (fileStreamIn != null)
//                        {
//                            fileStreamIn.Close();
//                            fileStreamIn = null;
//                        }
//                    }
//                    if (StdOut != default)
//                    {
//                        Kernel32.CloseHandle(StdOut);
//                        if (fileStreamOut != null)
//                        {
//                            fileStreamOut.Close();
//                            fileStreamOut = null;
//                        }
//                    }
//                    if (StdErr != default)
//                    {
//                        Kernel32.CloseHandle(StdErr);
//                        if (fileStreamErr != null)
//                        {
//                            fileStreamErr.Close();
//                            fileStreamErr = null;
//                        }
//                    }
//                }
//            }
//        }


//        static int CreateProcess(ProcessParameters processParameters)
//        {
//            IntPtr hNewProcessToken = IntPtr.Zero;
//            IntPtr hProcessToken = IntPtr.Zero;
//            try
//            {
//                NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();
//                si.hStdInput = processParameters.StdIn != null ? processParameters.StdIn : new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_INPUT_HANDLE), true);
//                si.hStdOutput = processParameters.StdOut != null ? processParameters.StdOut : new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE), true);
//                si.hStdError = processParameters.StdErr != null ? processParameters.StdErr : new SafeFileHandle(NativeMethods.GetStdHandle(Kernel32.STD_ERROR_HANDLE), true);
//                if (processParameters.StdIn != null || processParameters.StdOut != null || processParameters.StdErr != null)
//                    si.dwFlags = (int)Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                si.dwFlags |= Advapi32.dwFlags.STARTF_USESHOWWINDOW; //it is needed if hiding the window when CREATE_NEW_CONSOLE
//                si.wShowWindow = (short)(!showWindow ? User32.SW.SW_HIDE : User32.SW.SW_SHOW);
//                //si.lpDesktop = "winsta0\\default";

//                NativeMethods.SECURITY_ATTRIBUTES sa = new NativeMethods.SECURITY_ATTRIBUTES();
//                sa.bInheritHandle = true;

//                Advapi32.CreationFlags creationFlags = Advapi32.CreationFlags.CREATE_NEW_CONSOLE;
//                //|=Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT
//                if (!showWindow)
//                    creationFlags |= Advapi32.CreationFlags.CREATE_NO_WINDOW;

//                SafeNativeMethods.PROCESS_INFORMATION pi = new SafeNativeMethods.PROCESS_INFORMATION();
//                if (!NativeMethods.CreateProcess(
//                    null,//Environment.SystemDirectory + "\\cmd.exe", // file to execute
//                    new StringBuilder(commandLine),// new System.Text.StringBuilder(Environment.SystemDirectory + "\\cmd.exe"), //  @"C:\Far\Far.exe",// null, // command line
//                    null, // pointer to process SECURITY_ATTRIBUTES
//                    null, // pointer to thread SECURITY_ATTRIBUTES
//                    true, // handles are inheritable
//                    (int)creationFlags, // creation flags
//                    IntPtr.Zero, // pointer to new environment block;  NULL = use parent's environment
//                    Environment.CurrentDirectory, // name of current directory; NULL = use parent's current directory 
//                    si, // pointer to STARTUPINFO structure
//                    pi // receives information about new process
//                    ))
//                    throw new Win.LastErrorException("!CreateProcess()");
//                return pi.dwProcessId;
//            }
//            finally
//            {
//                if (hProcessToken != IntPtr.Zero)
//                    Kernel32.CloseHandle(hProcessToken);
//                if (hNewProcessToken != IntPtr.Zero)
//                    Kernel32.CloseHandle(hNewProcessToken);
//            }
//        }

//        public static ProcessInfo CreateProcess(ProcessParameters processParameters)
//        {
//            IntPtr hNewProcessToken = IntPtr.Zero;
//            IntPtr hProcessToken = IntPtr.Zero;
//            IntPtr stdInRead = default, stdOutWrite = default, stdErrWrite = default;
//            try
//            {
//                Advapi32.STARTUPINFO si;
//                if (processParameters.StartupInfo != null)
//                    si = (Advapi32.STARTUPINFO)processParameters.StartupInfo;
//                else
//                    si = new Advapi32.STARTUPINFO();
//                si.cb = Marshal.SizeOf(si);
//                //si.lpDesktop = "winsta0\\default";
//                //si.wShowWindow = (short)(processParameters.CreationFlags.HasFlag(Advapi32.CreationFlags.CREATE_NO_WINDOW) ? User32.SW.SW_HIDE : User32.SW.SW_SHOW);
//                //Advapi32.CreationFlags dwCreationFlags = Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT | (visible ? Advapi32.CreationFlags.CREATE_NEW_CONSOLE : Advapi32.CreationFlags.CREATE_NO_WINDOW);
//                //processParameters.CreationFlags |= Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT;

//                si.hStdInput = Kernel32.GetStdHandle(Kernel32.STD_INPUT_HANDLE);
//                si.hStdOutput = Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);
//                si.hStdError = Kernel32.GetStdHandle(Kernel32.STD_ERROR_HANDLE);

//                IntPtr stdInWrite = default, stdOutRead = default, stdErrRead = default;

//                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
//                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDIN))
//                {
//                    sa.bInheritHandle = true;
//                    if (processParameters.StdIn != default)
//                        si.hStdInput = processParameters.StdIn;
//                    else
//                    {
//                        if (!Kernel32.CreatePipe(out stdInRead, out stdInWrite, ref sa, processParameters.BufferLength))
//                            throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                        //Ensure the write handle to the pipe for STDIN is not inherited. 
//                        if (!Kernel32.SetHandleInformation(stdInWrite, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                            throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                        si.hStdInput = stdInRead;
//                    }
//                    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                }

//                //if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDOUT))
//                //{
//                //    sa.bInheritHandle = true;
//                //    if (!Kernel32.CreatePipe(out stdOutRead, out stdOutWrite, ref sa, processParameters.BufferLength))
//                //        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                //    // Ensure the read handle to the pipe for STDOUT is not inherited.
//                //    if (!Kernel32.SetHandleInformation(stdOutRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                //        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                //    si.hStdOutput = stdOutWrite;
//                //    if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR_TO_STDOUT))
//                //        si.hStdError = stdOutWrite;
//                //    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                //}

//                //if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR))
//                //{
//                //    sa.bInheritHandle = true;
//                //    if (!Kernel32.CreatePipe(out stdErrRead, out stdErrWrite, ref sa, processParameters.BufferLength))
//                //        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                //    // Ensure the read handle to the pipe for STDERR is not inherited.
//                //    if (!Kernel32.SetHandleInformation(stdErrRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                //        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                //    si.hStdError = stdErrWrite;
//                //    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                //}

//                //IntPtr pEnv = default;
//                //if (!Userenv.CreateEnvironmentBlock(ref pEnv, hNewProcessToken, false))
//                //    throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");

//                bool bInheritHandle = sa.bInheritHandle;
//                sa = new SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
//                Advapi32.PROCESS_INFORMATION pi;
//                if (!Advapi32.CreateProcess(
//                 processParameters.Executable, // file to execute
//                 processParameters.CommandLine, // command line
//                    ref sa, // pointer to process SECURITY_ATTRIBUTES
//                    ref sa, // pointer to thread SECURITY_ATTRIBUTES
//                    bInheritHandle, // handles are inheritable
//                Advapi32.CreationFlags.CREATE_NEW_CONSOLE, //0,//  processParameters.CreationFlags, // creation flags
//                    IntPtr.Zero, // pointer to new environment block 
//                    processParameters.CurrentDirectory, // name of current directory 
//                    ref si, // pointer to STARTUPINFO structure
//                    out pi // receives information about new process
//                    ))
//                    throw new Exception("!CreateProcess()", ErrorRoutines.GetLastError());
//                return new ProcessInfo(pi.dwProcessId) { StdIn = stdInWrite, BufferLength = (int)processParameters.BufferLength };
//            }
//            finally
//            {
//                if (hProcessToken != IntPtr.Zero)
//                    Kernel32.CloseHandle(hProcessToken);
//                if (hNewProcessToken != IntPtr.Zero)
//                    Kernel32.CloseHandle(hNewProcessToken);

//                if (stdInRead != default)
//                    Kernel32.CloseHandle(stdInRead);
//                if (stdOutWrite != default)
//                    Kernel32.CloseHandle(stdOutWrite);
//                if (stdErrWrite != default)
//                    Kernel32.CloseHandle(stdErrWrite);
//            }
//        }

//        /// <summary>
//        /// Run a process in the specified logon session as the user of the current process.
//        /// </summary>
//        /// <param name="dwSessionId">ID of the logon session where the process is to run</param>
//        /// <param name="processParameters"></param>
//        /// <returns></returns>
//        public static ProcessInfo CreateProcessAsUserOfCurrentProcess(uint dwSessionId, ProcessParameters processParameters)
//        {
//            IntPtr hNewProcessToken = IntPtr.Zero;
//            IntPtr hProcessToken = IntPtr.Zero;
//            IntPtr stdInRead = default, stdOutWrite = default, stdErrWrite = default;
//            IntPtr stdInWrite = default, stdOutRead = default, stdErrRead = default;
//            try
//            {
//                WinApi.Advapi32.STARTUPINFO si;
//                if (processParameters.StartupInfo != null)
//                    si = (WinApi.Advapi32.STARTUPINFO)processParameters.StartupInfo;
//                else
//                    si = new WinApi.Advapi32.STARTUPINFO();
//                si.cb = Marshal.SizeOf(si);
//                si.lpDesktop = "winsta0\\default";
//                si.wShowWindow = (short)(processParameters.CreationFlags.HasFlag(WinApi.Advapi32.CreationFlags.CREATE_NO_WINDOW) ? User32.SW.SW_HIDE : User32.SW.SW_SHOW);

//                if (!WinApi.Advapi32.OpenProcessToken(Process.GetCurrentProcess().Handle, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, out hProcessToken))
//                    throw new Exception("!OpenProcessToken()", ErrorRoutines.GetLastError());

//                var sa = new WinApi.SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
//                //sa.bInheritHandle = 1;
//                if (!WinApi.Advapi32.DuplicateTokenEx(hProcessToken, WinApi.Advapi32.DesiredAccess.MAXIMUM_ALLOWED, sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, out hNewProcessToken))
//                    throw new Exception("!DuplicateTokenEx()", ErrorRoutines.GetLastError());

//                //is it needed to specify in which session the process is to run
//                //!!!it usually needs to run with SYSTEM priviledges
//                //if (!WinApi.Advapi32.SetTokenInformation(hNewProcessToken, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, (uint)IntPtr.Size))
//                //    throw new Exception("!SetTokenInformation()", ErrorRoutines.GetLastError());

//                si.hStdInput = Kernel32.GetStdHandle(Kernel32.STD_INPUT_HANDLE);
//                si.hStdOutput = Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);
//                si.hStdError = Kernel32.GetStdHandle(Kernel32.STD_ERROR_HANDLE);

//                sa = new SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
//                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDIN))
//                {
//                    sa.bInheritHandle = true;
//                    //if (!Kernel32.CreatePipe(out stdInRead, out stdInWrite, ref sa, processParameters.BufferLength))
//                    //    throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                    stdInRead = Kernel32.CreateNamedPipe("test123",
//                           Kernel32.PipeOpenModeFlags.FILE_FLAG_OVERLAPPED | Kernel32.PipeOpenModeFlags.PIPE_ACCESS_INBOUND,
//                           Kernel32.PipeModeFlags.PIPE_TYPE_MESSAGE | Kernel32.PipeModeFlags.PIPE_READMODE_MESSAGE | Kernel32.PipeModeFlags.PIPE_WAIT,
//                           1,
//                           processParameters.BufferLength,
//                           processParameters.BufferLength,
//                            100000,
//                            ref sa
//                            );
//                    stdInRead = Kernel32.CreateNamedPipe("test123",
//                           0,
//                          0,
//                           1,
//                           processParameters.BufferLength,
//                           processParameters.BufferLength,
//                            100000,
//                            ref sa
//                            );
//                    if (stdInRead == Kernel32.INVALID_HANDLE_VALUE)
//                        throw new Exception("!CreateNamedPipe()", ErrorRoutines.GetLastError());
//                    // Ensure the write handle to the pipe for STDIN is not inherited. 
//                    //if (!Kernel32.SetHandleInformation(stdInWrite, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                    //    throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                    si.hStdInput = stdInRead;
//                    //si.dwFlags |= WinApi.Advapi32.dwFlags.STARTF_USESTDHANDLES; //hides output even if it is not handled/redirected
//                }

//                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDOUT))
//                {
//                    sa.bInheritHandle = true;
//                    if (!Kernel32.CreatePipe(out stdOutRead, out stdOutWrite, ref sa, processParameters.BufferLength))
//                        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                    // Ensure the read handle to the pipe for STDOUT is not inherited.
//                    if (!Kernel32.SetHandleInformation(stdOutRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                    si.hStdOutput = stdOutWrite;
//                    if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR_TO_STDOUT))
//                        si.hStdError = stdOutWrite;
//                    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                }

//                if (processParameters.CreationFlags2.HasFlag(CreationFlags2.REDIRECT_STDERR))
//                {
//                    sa.bInheritHandle = true;
//                    if (!Kernel32.CreatePipe(out stdErrRead, out stdErrWrite, ref sa, processParameters.BufferLength))
//                        throw new Exception("!CreatePipe()", ErrorRoutines.GetLastError());
//                    // Ensure the read handle to the pipe for STDERR is not inherited.
//                    if (!Kernel32.SetHandleInformation(stdErrRead, Kernel32.HANDLE_FLAGS.INHERIT, Kernel32.HANDLE_FLAGS.INHERIT))
//                        throw new Exception("!SetHandleInformation()", ErrorRoutines.GetLastError());
//                    si.hStdError = stdErrWrite;
//                    si.dwFlags |= Advapi32.dwFlags.STARTF_USESTDHANDLES;
//                }

//                //IntPtr pEnv = default;
//                //if (!Userenv.CreateEnvironmentBlock(ref pEnv, hNewProcessToken, false))
//                //    throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");

//                bool bInheritHandle = sa.bInheritHandle;
//                sa = new SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
//                Advapi32.PROCESS_INFORMATION pi;
//                if (!Advapi32.CreateProcessAsUser(hNewProcessToken, // client's access token
//                    null, // file to execute
//                    processParameters.CommandLine, // command line
//                    sa, // pointer to process SECURITY_ATTRIBUTES
//                    sa, // pointer to thread SECURITY_ATTRIBUTES
//                    bInheritHandle, // handles are inheritable
//                  processParameters.CreationFlags, // creation flags
//                    default, // pointer to new environment block 
//                    processParameters.CurrentDirectory, // name of current directory 
//                    ref si, // pointer to STARTUPINFO structure
//                    out pi // receives information about new process
//                    ))
//                    throw new Exception("!CreateProcessAsUser()", ErrorRoutines.GetLastError());
//                return new ProcessInfo(pi.dwProcessId) { BufferLength = (int)processParameters.BufferLength };
//            }
//            finally
//            {
//                if (hProcessToken != IntPtr.Zero)
//                    WinApi.Kernel32.CloseHandle(hProcessToken);
//                if (hNewProcessToken != IntPtr.Zero)
//                    WinApi.Kernel32.CloseHandle(hNewProcessToken);

//                if (stdInRead != default)
//                    Kernel32.CloseHandle(stdInRead);
//                if (stdOutWrite != default)
//                    Kernel32.CloseHandle(stdOutWrite);
//                if (stdErrWrite != default)
//                    Kernel32.CloseHandle(stdErrWrite);
//            }
//        }

//        public static uint CreateProcessAsUserOfCurrentSession(string appCmdLine /*,int processId*/)
//        {
//            IntPtr hToken = IntPtr.Zero;
//            IntPtr envBlock = IntPtr.Zero;
//            try
//            {
//                //Either specify the processID explicitly 
//                //Or try to get it from a process owned by the user. 
//                //In this case assuming there is only one explorer.exe 
//                Process[] ps = Process.GetProcessesByName("explorer");
//                int processId = -1;//=processId 
//                if (ps.Length > 0)
//                    processId = ps[0].Id;
//                if (processId <= 1)
//                    throw new Exception("processId <= 1: " + processId);

//                hToken = getPrimaryToken(processId);
//                if (hToken == IntPtr.Zero)
//                    throw new Exception("!GetPrimaryToken()", ErrorRoutines.GetLastError());

//                if (!Cliver.WinApi.Userenv.CreateEnvironmentBlock(ref envBlock, hToken, false))
//                    throw new Exception("!CreateEnvironmentBlock()", ErrorRoutines.GetLastError());

//                return createProcessAsUser(appCmdLine, hToken, envBlock);
//            }
//            //catch(Exception e)
//            //{

//            //}
//            finally
//            {
//                if (envBlock != IntPtr.Zero)
//                    Cliver.WinApi.Userenv.DestroyEnvironmentBlock(envBlock);
//                if (hToken != IntPtr.Zero)
//                    WinApi.Kernel32.CloseHandle(hToken);
//            }
//        }
//        static uint createProcessAsUser(string cmdLine, IntPtr hToken, IntPtr envBlock)
//        {
//            try
//            {
//                WinApi.Advapi32.PROCESS_INFORMATION pi = new WinApi.Advapi32.PROCESS_INFORMATION();
//                WinApi.SECURITY_ATTRIBUTES saProcess = new WinApi.SECURITY_ATTRIBUTES();
//                WinApi.SECURITY_ATTRIBUTES saThread = new WinApi.SECURITY_ATTRIBUTES();
//                saProcess.nLength = Marshal.SizeOf(saProcess);
//                saThread.nLength = Marshal.SizeOf(saThread);

//                WinApi.Advapi32.STARTUPINFO si = new WinApi.Advapi32.STARTUPINFO();
//                si.cb = Marshal.SizeOf(si);

//                //if this member is NULL, the new process inherits the desktop 
//                //and window station of its parent process. If this member is 
//                //an empty string, the process does not inherit the desktop and 
//                //window station of its parent process; instead, the system 
//                //determines if a new desktop and window station need to be created. 
//                //If the impersonated user already has a desktop, the system uses the 
//                //existing desktop. 

//                si.lpDesktop = @"WinSta0\Default"; //Modify as needed 
//                si.dwFlags = Advapi32.dwFlags.STARTF_USESHOWWINDOW | Advapi32.dwFlags.STARTF_FORCEONFEEDBACK;
//                si.wShowWindow = WinApi.User32.SW_SHOW;

//                if (!WinApi.Advapi32.CreateProcessAsUser(
//                    hToken,
//                    null,
//                    cmdLine,
//                     saProcess,
//                     saThread,
//                    false,
//                    WinApi.Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT,
//                    envBlock,
//                    null,
//                    ref si,
//                    out pi
//                    ))
//                    throw new Exception("!CreateProcessAsUser()", ErrorRoutines.GetLastError());
//                return pi.dwProcessId;
//            }
//            //catch(Exception e)
//            //{

//            //}
//            finally
//            {
//            }
//        }
//        static IntPtr getPrimaryToken(int processId)
//        {
//            IntPtr hToken = IntPtr.Zero;
//            try
//            {
//                IntPtr primaryToken = IntPtr.Zero;
//                Process p = Process.GetProcessById(processId);

//                //Gets impersonation token 
//                if (!WinApi.Advapi32.OpenProcessToken(p.Handle, WinApi.Advapi32.DesiredAccess.TOKEN_DUPLICATE, out hToken))
//                    throw new Exception("!OpenProcessToken()", ErrorRoutines.GetLastError());

//                WinApi.SECURITY_ATTRIBUTES sa = new WinApi.SECURITY_ATTRIBUTES();
//                sa.nLength = Marshal.SizeOf(sa);

//                //Convert the impersonation token into Primary token 
//                if (!WinApi.Advapi32.DuplicateTokenEx(
//                    hToken,
//                    WinApi.Advapi32.DesiredAccess.TOKEN_ASSIGN_PRIMARY | WinApi.Advapi32.DesiredAccess.TOKEN_DUPLICATE | WinApi.Advapi32.DesiredAccess.TOKEN_QUERY,
//                    sa,
//                    WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
//                    WinApi.Advapi32.TOKEN_TYPE.TokenPrimary,
//                    out primaryToken
//                    ))
//                    throw new Exception("!DuplicateTokenEx()", ErrorRoutines.GetLastError());
//                return primaryToken;
//            }
//            //catch(Exception e)
//            //{

//            //}
//            finally
//            {
//                if (hToken != IntPtr.Zero)
//                    WinApi.Kernel32.CloseHandle(hToken);
//            }
//        }





//        /// <summary>
//        /// !!! to be checked
//        /// </summary>
//        /// <param name="appPath"></param>
//        /// <param name="cmdLine"></param>
//        /// <param name="workDir"></param>
//        /// <param name="visible"></param>
//        /// <returns></returns>
//        public static bool StartProcessAsCurrentUser(string appPath, string cmdLine = null, string workDir = null, bool visible = true)
//        {
//            var hUserToken = IntPtr.Zero;
//            Advapi32.STARTUPINFO startInfo = new Advapi32.STARTUPINFO();
//            WinApi.Advapi32.PROCESS_INFORMATION procInfo = default;
//            var pEnv = IntPtr.Zero;
//            int iResultOfCreateProcessAsUser;
//            startInfo.cb = Marshal.SizeOf(typeof(Advapi32.STARTUPINFO));

//            try
//            {
//                if (!getActiveSessionUserToken(ref hUserToken))
//                    throw new Exception("StartProcessAsCurrentUser: GetSessionUserToken failed.");

//                Advapi32.CreationFlags dwCreationFlags = Advapi32.CreationFlags.CREATE_UNICODE_ENVIRONMENT | (visible ? Advapi32.CreationFlags.CREATE_NEW_CONSOLE : Advapi32.CreationFlags.CREATE_NO_WINDOW);
//                startInfo.wShowWindow = (short)(visible ? User32.SW.SW_SHOW : User32.SW.SW_HIDE);
//                startInfo.lpDesktop = "winsta0\\default";

//                if (!Userenv.CreateEnvironmentBlock(ref pEnv, hUserToken, false))
//                    throw new Exception("StartProcessAsCurrentUser: CreateEnvironmentBlock failed.");

//                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
//                if (!Advapi32.CreateProcessAsUser(hUserToken,
//                    appPath, // Application Name
//                    cmdLine, // Command Line
//                     sa,
//                     sa,
//                    false,
//                    dwCreationFlags,
//                    pEnv,
//                    workDir, // Working directory
//                    ref startInfo,
//                    out procInfo))
//                {
//                    iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
//                    throw new Exception("StartProcessAsCurrentUser: CreateProcessAsUser failed.  Error Code -" + iResultOfCreateProcessAsUser);
//                }

//                iResultOfCreateProcessAsUser = Marshal.GetLastWin32Error();
//            }
//            finally
//            {
//                Kernel32.CloseHandle(hUserToken);
//                if (pEnv != IntPtr.Zero)
//                {
//                    Userenv.DestroyEnvironmentBlock(pEnv);
//                }
//                Kernel32.CloseHandle(procInfo.hThread);
//                Kernel32.CloseHandle(procInfo.hProcess);
//            }

//            return true;
//        }
//        private static bool getActiveSessionUserToken(ref IntPtr phUserToken)
//        {
//            var bResult = false;
//            var hImpersonationToken = IntPtr.Zero;
//            var activeSessionId = Wts.INVALID_SESSION_ID;
//            var pSessionInfo = IntPtr.Zero;
//            var sessionCount = 0;

//            // Get a handle to the user access token for the current active session.
//            if (Wts.WTSEnumerateSessions(Wts.WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
//            {
//                var arrayElementSize = Marshal.SizeOf(typeof(Wts.WTS_SESSION_INFO));
//                var current = pSessionInfo;

//                for (var i = 0; i < sessionCount; i++)
//                {
//                    var si = (Wts.WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(Wts.WTS_SESSION_INFO));
//                    current += arrayElementSize;

//                    if (si.State == Wts.WTS_CONNECTSTATE_CLASS.WTSActive)
//                        activeSessionId = si.SessionID;
//                }
//            }

//            // If enumerating did not work, fall back to the old method
//            if (activeSessionId == Wts.INVALID_SESSION_ID)
//            {
//                activeSessionId = Wts.WTSGetActiveConsoleSessionId();
//            }

//            if (Wts.WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
//            {
//                // Convert the impersonation token to a primary token
//                var sa = new WinApi.SECURITY_ATTRIBUTES();
//                bResult = Advapi32.DuplicateTokenEx(hImpersonationToken, 0, sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, out phUserToken);
//                Kernel32.CloseHandle(hImpersonationToken);
//            }

//            return bResult;
//        }
//    }
//}