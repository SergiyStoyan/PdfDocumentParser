//// ==++==
//// 
////   Copyright (c) Microsoft Corporation.  All rights reserved.
//// 
//// ==--==
///*============================================================
//**
//** Class: UnsafeNativeMethods
//**
//============================================================*/
//namespace Microsoft.Win32_ {

//    using Microsoft.Win32;
//    using Microsoft.Win32.SafeHandles;
//    using System;
//    using System.Configuration.Assemblies;
//    using System.Runtime.CompilerServices;
//    using System.Runtime.ConstrainedExecution;
//    using System.Runtime.InteropServices;
//    using System.Runtime.Remoting;
//    using System.Security.Principal;
//    using System.Runtime.Serialization;
//    using System.Threading;
//    using System.Runtime.Versioning;
//    using System.Security;
//    using System.Security.Permissions;
//    using System.Text;
//    using System.Diagnostics.Eventing;
//    using System.Diagnostics.Eventing.Reader;

//    [SuppressUnmanagedCodeSecurityAttribute()]
//    public static class UnsafeNativeMethods {

//        public const String KERNEL32 = "kernel32.dll";
//        public const String ADVAPI32 = "advapi32.dll";
//        public const String WEVTAPI  = "wevtapi.dll";
//        public static readonly IntPtr NULL = IntPtr.Zero;

//        //
//        // Win32 IO
//        //
//        public const int CREDUI_MAX_USERNAME_LENGTH = 513;
     
       
//        // WinError.h codes:

//        public const int ERROR_SUCCESS                 = 0x0;
//        public const int ERROR_FILE_NOT_FOUND          = 0x2;
//        public const int ERROR_PATH_NOT_FOUND          = 0x3;
//        public const int ERROR_ACCESS_DENIED           = 0x5;
//        public const int ERROR_INVALID_HANDLE          = 0x6;

//        // Can occurs when filled buffers are trying to flush to disk, but disk IOs are not fast enough. 
//        // This happens when the disk is slow and event traffic is heavy. 
//        // Eventually, there are no more free (empty) buffers and the event is dropped.
//        public const int ERROR_NOT_ENOUGH_MEMORY       = 0x8;
        
//        public const int ERROR_INVALID_DRIVE           = 0xF;
//        public const int ERROR_NO_MORE_FILES           = 0x12;
//        public const int ERROR_NOT_READY               = 0x15;
//        public const int ERROR_BAD_LENGTH              = 0x18;
//        public const int ERROR_SHARING_VIOLATION       = 0x20;
//        public const int ERROR_LOCK_VIOLATION          = 0x21;  // 33
//        public const int ERROR_HANDLE_EOF              = 0x26;  // 38
//        public const int ERROR_FILE_EXISTS             = 0x50;
//        public const int ERROR_INVALID_PARAMETER       = 0x57;  // 87
//        public const int ERROR_BROKEN_PIPE             = 0x6D;  // 109
//        public const int ERROR_INSUFFICIENT_BUFFER     = 0x7A;  // 122
//        public const int ERROR_INVALID_NAME            = 0x7B;
//        public const int ERROR_BAD_PATHNAME            = 0xA1;
//        public const int ERROR_ALREADY_EXISTS          = 0xB7;        
//        public const int ERROR_ENVVAR_NOT_FOUND        = 0xCB;
//        public const int ERROR_FILENAME_EXCED_RANGE    = 0xCE;  // filename too long
//        public const int ERROR_PIPE_BUSY               = 0xE7;  // 231
//        public const int ERROR_NO_DATA                 = 0xE8;  // 232
//        public const int ERROR_PIPE_NOT_CONNECTED      = 0xE9;  // 233
//        public const int ERROR_MORE_DATA               = 0xEA;
//        public const int ERROR_NO_MORE_ITEMS           = 0x103;  // 259
//        public const int ERROR_PIPE_CONNECTED          = 0x217;  // 535
//        public const int ERROR_PIPE_LISTENING          = 0x218;  // 536
//        public const int ERROR_OPERATION_ABORTED       = 0x3E3;  // 995; For IO Cancellation
//        public const int ERROR_IO_PENDING              = 0x3E5;  // 997
//        public const int ERROR_NOT_FOUND               = 0x490;  // 1168      
   
//        // The event size is larger than the allowed maximum (64k - header).
//        public const int ERROR_ARITHMETIC_OVERFLOW     = 0x216;  // 534

//        public const int ERROR_RESOURCE_LANG_NOT_FOUND = 0x717;  // 1815


//        // Event log specific codes:

//        public const int ERROR_EVT_MESSAGE_NOT_FOUND = 15027;
//        public const int ERROR_EVT_MESSAGE_ID_NOT_FOUND = 15028;
//        public const int ERROR_EVT_UNRESOLVED_VALUE_INSERT = 15029;
//        public const int ERROR_EVT_UNRESOLVED_PARAMETER_INSERT = 15030;
//        public const int ERROR_EVT_MAX_INSERTS_REACHED = 15031;
//        public const int ERROR_EVT_MESSAGE_LOCALE_NOT_FOUND = 15033;
//        public const int ERROR_MUI_FILE_NOT_FOUND = 15100;


//        public const int SECURITY_SQOS_PRESENT = 0x00100000;
//        public const int SECURITY_ANONYMOUS = 0 << 16;
//        public const int SECURITY_IDENTIFICATION = 1 << 16;
//        public const int SECURITY_IMPERSONATION = 2 << 16;
//        public const int SECURITY_DELEGATION = 3 << 16;

//        public const int GENERIC_READ = unchecked((int)0x80000000);
//        public const int GENERIC_WRITE = 0x40000000;

//        public const int STD_INPUT_HANDLE = -10;
//        public const int STD_OUTPUT_HANDLE = -11;
//        public const int STD_ERROR_HANDLE = -12;

//        public const int DUPLICATE_SAME_ACCESS = 0x00000002;

//        public const int PIPE_ACCESS_INBOUND = 1;
//        public const int PIPE_ACCESS_OUTBOUND = 2;
//        public const int PIPE_ACCESS_DUPLEX = 3;
//        public const int PIPE_TYPE_BYTE = 0;
//        public const int PIPE_TYPE_MESSAGE = 4;
//        public const int PIPE_READMODE_BYTE = 0;
//        public const int PIPE_READMODE_MESSAGE = 2;
//        public const int PIPE_UNLIMITED_INSTANCES = 255;

//        public const int FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;
//        public const int FILE_SHARE_READ = 0x00000001;
//        public const int FILE_SHARE_WRITE = 0x00000002;
//        public const int FILE_ATTRIBUTE_NORMAL = 0x00000080;

//        public const int FILE_FLAG_OVERLAPPED = 0x40000000;

//        public const int OPEN_EXISTING = 3;        

//        // From WinBase.h
//        public const int FILE_TYPE_DISK = 0x0001;
//        public const int FILE_TYPE_CHAR = 0x0002;
//        public const int FILE_TYPE_PIPE = 0x0003;       

//        // Memory mapped file constants
//        public const int MEM_COMMIT = 0x1000;
//        public const int MEM_RESERVE = 0x2000;
//        public const int INVALID_FILE_SIZE = -1;
//        public const int PAGE_READWRITE = 0x04;
//        public const int PAGE_READONLY = 0x02;
//        public const int PAGE_WRITECOPY = 0x08;
//        public const int PAGE_EXECUTE_READ = 0x20;
//        public const int PAGE_EXECUTE_READWRITE = 0x40;

//        public const int FILE_MAP_COPY = 0x0001;
//        public const int FILE_MAP_WRITE = 0x0002;
//        public const int FILE_MAP_READ = 0x0004;
//        public const int FILE_MAP_EXECUTE = 0x0020;

//        //[StructLayout(LayoutKind.Sequential)]
//        //public class SECURITY_ATTRIBUTES
//        //{
//        //    public int nLength;
//        //    [SecurityCritical]
//        //    public unsafe byte* pSecurityDescriptor;
//        //    public int bInheritHandle;
//        //} 

//        //!!!imported from NativeMethods.cs which differs from the native one!!!
//        [StructLayout(LayoutKind.Sequential)]
//        public class SECURITY_ATTRIBUTES
//        {
//            public int nLength = 12;
//            public SafeLocalMemHandle lpSecurityDescriptor = new SafeLocalMemHandle(IntPtr.Zero, false);
//            public bool bInheritHandle = false;
//        }

//        [DllImport(KERNEL32)]
//        [SecurityCritical]
//        public static extern int GetFileType(SafeFileHandle handle);

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //public static unsafe extern int WriteFile(SafeFileHandle handle, byte* bytes, int numBytesToWrite,
//        //                                            out int numBytesWritten, NativeOverlapped* lpOverlapped);

//        // Disallow access to all non-file devices from methods that take
//        // a String.  This disallows DOS devices like "con:", "com1:", 
//        // "lpt1:", etc.  Use this to avoid security problems, like allowing
//        // a web client asking a server for "http://server/com1.aspx" and
//        // then causing a worker process to hang.
//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        private static extern SafeFileHandle CreateFile(String lpFileName,
//            int dwDesiredAccess, System.IO.FileShare dwShareMode,
//            SECURITY_ATTRIBUTES securityAttrs, System.IO.FileMode dwCreationDisposition,
//            int dwFlagsAndAttributes, IntPtr hTemplateFile);

     


//        // From WinBase.h
//        public const int SEM_FAILCRITICALERRORS = 1;

//        [DllImport(KERNEL32, SetLastError = false)]
//        [ResourceExposure(ResourceScope.Process)]
//        [SecurityCritical]
//        public static extern int SetErrorMode(int newMode);

//        //[DllImport(KERNEL32, SetLastError = true, EntryPoint = "SetFilePointer")]
//        //[ResourceExposure(ResourceScope.None)]
//        //[SecurityCritical]
//        //private unsafe static extern int SetFilePointerWin32(SafeFileHandle handle, int lo, int* hi, int origin);

//        //[ResourceExposure(ResourceScope.None)]
//        //[SecurityCritical]
//        //public unsafe static long SetFilePointer(SafeFileHandle handle, long offset, System.IO.SeekOrigin origin, out int hr) {
//        //    hr = 0;
//        //    int lo = (int)offset;
//        //    int hi = (int)(offset >> 32);
//        //    lo = SetFilePointerWin32(handle, lo, &hi, (int)origin);

//        //    if (lo == -1 && ((hr = Marshal.GetLastWin32Error()) != 0))
//        //        return -1;
//        //    return (long)(((ulong)((uint)hi)) << 32) | ((uint)lo);
//        //}

//        //
//        // ErrorCode & format 
//        //

//        // Use this to translate error codes like the above into HRESULTs like

//        // 0x80070006 for ERROR_INVALID_HANDLE
//        public static int MakeHRFromErrorCode(int errorCode) {
//            return unchecked(((int)0x80070000) | errorCode);
//        }

//        // for win32 error message formatting
//        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
//        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
//        private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;


//        [DllImport(KERNEL32, CharSet = CharSet.Auto, BestFitMapping = false)]
//        [SecurityCritical]
//        public static extern int FormatMessage(int dwFlags, IntPtr lpSource,
//            int dwMessageId, int dwLanguageId, StringBuilder lpBuffer,
//            int nSize, IntPtr va_list_arguments);

//        // Gets an error message for a Win32 error code.
//        [SecurityCritical]
//        public static String GetMessage(int errorCode) {
//            StringBuilder sb = new StringBuilder(512);
//            int result = UnsafeNativeMethods.FormatMessage(FORMAT_MESSAGE_IGNORE_INSERTS |
//                FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ARGUMENT_ARRAY,
//                UnsafeNativeMethods.NULL, errorCode, 0, sb, sb.Capacity, UnsafeNativeMethods.NULL);
//            if (result != 0) {
//                // result is the # of characters copied to the StringBuilder on NT,
//                // but on Win9x, it appears to be the number of MBCS buffer.
//                // Just give up and return the String as-is...
//                String s = sb.ToString();
//                return s;
//            }
//            else {
//                return "UnknownError_Num " + errorCode;
//            }
//        }



//        // 
//        // Pipe
//        //

//        [DllImport(KERNEL32, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        [SecurityCritical]
//        public static extern bool CloseHandle(IntPtr handle);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        public static extern IntPtr GetCurrentProcess();

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
//            SafePipeHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafePipeHandle lpTargetHandle,
//            uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

//        [DllImport(KERNEL32)]
//        [SecurityCritical]
//        public static extern int GetFileType(SafePipeHandle handle);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool CreatePipe(out SafePipeHandle hReadPipe,
//            out SafePipeHandle hWritePipe, SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);


//        [DllImport(KERNEL32, EntryPoint="CreateFile", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
//        [SecurityCritical]
//        public static extern SafePipeHandle CreateNamedPipeClient(String lpFileName,
//            int dwDesiredAccess, System.IO.FileShare dwShareMode,
//            UnsafeNativeMethods.SECURITY_ATTRIBUTES securityAttrs, System.IO.FileMode dwCreationDisposition,
//            int dwFlagsAndAttributes, IntPtr hTemplateFile);

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //[return: MarshalAs(UnmanagedType.Bool)]
//        //unsafe public static extern bool ConnectNamedPipe(SafePipeHandle handle, NativeOverlapped* overlapped);

//        [DllImport(KERNEL32, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool ConnectNamedPipe(SafePipeHandle handle, IntPtr overlapped);

//        [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool WaitNamedPipe(String name, int timeout);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, out int lpState,
//            IntPtr lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout,
//            IntPtr lpUserName, int nMaxUserNameSize);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, IntPtr lpState,
//            out int lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout,
//            IntPtr lpUserName, int nMaxUserNameSize);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, IntPtr lpState,
//            IntPtr lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout,
//            StringBuilder lpUserName, int nMaxUserNameSize);

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe,
//          out int lpFlags,
//          IntPtr lpOutBufferSize,
//          IntPtr lpInBufferSize,
//          IntPtr lpMaxInstances
//        );

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe,
//          IntPtr lpFlags,
//          out int lpOutBufferSize,
//          IntPtr lpInBufferSize,
//          IntPtr lpMaxInstances
//        );

//        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe,
//          IntPtr lpFlags,
//          IntPtr lpOutBufferSize,
//          out int lpInBufferSize,
//          IntPtr lpMaxInstances
//        );

//        //[DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
//        //[SecurityCritical]
//        //[return: MarshalAs(UnmanagedType.Bool)]
//        //public static unsafe extern bool SetNamedPipeHandleState(
//        //  SafePipeHandle hNamedPipe,
//        //  int* lpMode,
//        //  IntPtr lpMaxCollectionCount,
//        //  IntPtr lpCollectDataTimeout
//        //);

//        [DllImport(KERNEL32, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool DisconnectNamedPipe(SafePipeHandle hNamedPipe);

//        [DllImport(KERNEL32, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool FlushFileBuffers(SafePipeHandle hNamedPipe);

//        [DllImport(ADVAPI32, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
//        public static extern bool RevertToSelf();

//        [DllImport(ADVAPI32, SetLastError = true)]
//        [SecurityCritical]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
//        public static extern bool ImpersonateNamedPipeClient(SafePipeHandle hNamedPipe);

//        [DllImport(KERNEL32, SetLastError = true, BestFitMapping = false, CharSet = CharSet.Auto)]
//        [SecurityCritical]
//        public static extern SafePipeHandle CreateNamedPipe(string pipeName,
//            uint openMode, int pipeMode, int maxInstances,
//            int outBufferSize, int inBufferSize, int defaultTimeout,
//            SECURITY_ATTRIBUTES securityAttributes);

//        // Note there are two different ReadFile prototypes - this is to use 
//        // the type system to force you to not trip across a "feature" in 
//        // Win32's async IO support.  You can't do the following three things
//        // simultaneously: overlapped IO, free the memory for the overlapped 
//        // struct in a callback (or an EndRead method called by that callback), 
//        // and pass in an address for the numBytesRead parameter.  
//        // <

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //unsafe public static extern int ReadFile(SafePipeHandle handle, byte* bytes, int numBytesToRead,
//        //    IntPtr numBytesRead_mustBeZero, NativeOverlapped* overlapped);

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //unsafe public static extern int ReadFile(SafePipeHandle handle, byte* bytes, int numBytesToRead,
//        //    out int numBytesRead, IntPtr mustBeZero);

//        // Note there are two different WriteFile prototypes - this is to use 
//        // the type system to force you to not trip across a "feature" in 
//        // Win32's async IO support.  You can't do the following three things
//        // simultaneously: overlapped IO, free the memory for the overlapped 
//        // struct in a callback (or an EndWrite method called by that callback),
//        // and pass in an address for the numBytesRead parameter.  
//        // <

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //public static unsafe extern int WriteFile(SafePipeHandle handle, byte* bytes, int numBytesToWrite,
//        //    IntPtr numBytesWritten_mustBeZero, NativeOverlapped* lpOverlapped);

//        //[DllImport(KERNEL32, SetLastError = true)]
//        //[SecurityCritical]
//        //public static unsafe extern int WriteFile(SafePipeHandle handle, byte* bytes, int numBytesToWrite,
//        //    out int numBytesWritten, IntPtr mustBeZero);

//        [DllImport(KERNEL32, SetLastError = true)]
//        [SecurityCritical]
//        public static extern bool SetEndOfFile(IntPtr hNamedPipe);



//        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "EventUnregister", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
//        [SecurityCritical]
//        public static extern int EventUnregister([In] long registrationHandle);


//        //
//        // Control (Is Enabled) APIs
//        //
//        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "EventEnabled", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
//        [SecurityCritical]
//        public static extern int EventEnabled([In] long registrationHandle, [In] ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor);

//        [DllImport(ADVAPI32, ExactSpelling = true, EntryPoint = "EventProviderEnabled", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
//        [SecurityCritical]
//        public static extern int EventProviderEnabled([In] long registrationHandle, [In] byte level, [In] long keywords);

     

//        //
//        // EventLog
//        // 
//        [Flags]
//        public enum EvtQueryFlags {
//            EvtQueryChannelPath = 0x1,
//            EvtQueryFilePath = 0x2,
//            EvtQueryForwardDirection = 0x100,
//            EvtQueryReverseDirection = 0x200,
//            EvtQueryTolerateQueryErrors = 0x1000
//        }

//        [Flags]
//        public enum EvtSubscribeFlags {
//            EvtSubscribeToFutureEvents = 1,
//            EvtSubscribeStartAtOldestRecord = 2,
//            EvtSubscribeStartAfterBookmark = 3,
//            EvtSubscribeTolerateQueryErrors = 0x1000,
//            EvtSubscribeStrict = 0x10000
//        }
        
//        /// <summary>
//        /// Evt Variant types
//        /// </summary>
//        public enum EvtVariantType {
//            EvtVarTypeNull = 0,
//            EvtVarTypeString = 1,
//            EvtVarTypeAnsiString = 2,
//            EvtVarTypeSByte = 3,
//            EvtVarTypeByte = 4,
//            EvtVarTypeInt16 = 5,
//            EvtVarTypeUInt16 = 6,
//            EvtVarTypeInt32 = 7,
//            EvtVarTypeUInt32 = 8,
//            EvtVarTypeInt64 = 9,
//            EvtVarTypeUInt64 = 10,
//            EvtVarTypeSingle = 11,
//            EvtVarTypeDouble = 12,
//            EvtVarTypeBoolean = 13,
//            EvtVarTypeBinary = 14,
//            EvtVarTypeGuid = 15,
//            EvtVarTypeSizeT = 16,
//            EvtVarTypeFileTime = 17,
//            EvtVarTypeSysTime = 18,
//            EvtVarTypeSid = 19,
//            EvtVarTypeHexInt32 = 20,
//            EvtVarTypeHexInt64 = 21,
//            // these types used internally
//            EvtVarTypeEvtHandle = 32,
//            EvtVarTypeEvtXml = 35,
//            //Array = 128
//            EvtVarTypeStringArray = 129,
//            EvtVarTypeUInt32Array = 136
//        }

//        public enum EvtMasks {
//            EVT_VARIANT_TYPE_MASK = 0x7f,
//            EVT_VARIANT_TYPE_ARRAY = 128
//        }

//        [StructLayout(LayoutKind.Sequential)]
//        public struct SystemTime {
//            [MarshalAs(UnmanagedType.U2)]
//            public short Year;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Month;
//            [MarshalAs(UnmanagedType.U2)]
//            public short DayOfWeek;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Day;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Hour;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Minute;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Second;
//            [MarshalAs(UnmanagedType.U2)]
//            public short Milliseconds;
//        }

//        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
//#pragma warning disable 618 // Ssytem.Core still uses SecurityRuleSet.Level1
//        [SecurityCritical(SecurityCriticalScope.Everything)]
//#pragma warning restore 618
//        public struct EvtVariant {
//            [FieldOffset(0)]
//            public UInt32 UInteger;
//            [FieldOffset(0)]
//            public Int32 Integer;
//            [FieldOffset(0)]
//            public byte UInt8;
//            [FieldOffset(0)]
//            public short Short;
//            [FieldOffset(0)]
//            public ushort UShort;
//            [FieldOffset(0)]
//            public UInt32 Bool;
//            [FieldOffset(0)]
//            public Byte ByteVal;
//            [FieldOffset(0)]
//            public byte SByte;
//            [FieldOffset(0)]
//            public UInt64 ULong;
//            [FieldOffset(0)]
//            public Int64 Long;
//            [FieldOffset(0)]
//            public Single Single;            
//            [FieldOffset(0)]
//            public Double Double;
//            [FieldOffset(0)]
//            public IntPtr StringVal;
//            [FieldOffset(0)]
//            public IntPtr AnsiString;
//            [FieldOffset(0)]
//            public IntPtr SidVal;
//            [FieldOffset(0)]
//            public IntPtr Binary;
//            [FieldOffset(0)]
//            public IntPtr Reference;
//            [FieldOffset(0)]
//            public IntPtr Handle;
//            [FieldOffset(0)]
//            public IntPtr GuidReference;
//            [FieldOffset(0)]
//            public UInt64 FileTime;
//            [FieldOffset(0)]
//            public IntPtr SystemTime;
//            [FieldOffset(0)]
//            public IntPtr SizeT;
//            [FieldOffset(8)]
//            public UInt32 Count;   // number of elements (not length) in bytes.
//            [FieldOffset(12)]
//            public UInt32 Type;
//        }

//        public enum EvtEventPropertyId {
//            EvtEventQueryIDs = 0,
//            EvtEventPath = 1
//        }

//        /// <summary>
//        /// The query flags to get information about query
//        /// </summary>
//        public enum EvtQueryPropertyId {
//            EvtQueryNames = 0,   //String;   //Variant will be array of EvtVarTypeString
//            EvtQueryStatuses = 1 //UInt32;   //Variant will be Array of EvtVarTypeUInt32
//        }

//        /// <summary>
//        /// Publisher Metadata properties
//        /// </summary>
//        public enum EvtPublisherMetadataPropertyId {
//            EvtPublisherMetadataPublisherGuid = 0,      // EvtVarTypeGuid
//            EvtPublisherMetadataResourceFilePath = 1,       // EvtVarTypeString
//            EvtPublisherMetadataParameterFilePath = 2,      // EvtVarTypeString
//            EvtPublisherMetadataMessageFilePath = 3,        // EvtVarTypeString
//            EvtPublisherMetadataHelpLink = 4,               // EvtVarTypeString
//            EvtPublisherMetadataPublisherMessageID = 5,     // EvtVarTypeUInt32

//            EvtPublisherMetadataChannelReferences = 6,      // EvtVarTypeEvtHandle, ObjectArray
//            EvtPublisherMetadataChannelReferencePath = 7,   // EvtVarTypeString
//            EvtPublisherMetadataChannelReferenceIndex = 8,  // EvtVarTypeUInt32
//            EvtPublisherMetadataChannelReferenceID = 9,     // EvtVarTypeUInt32
//            EvtPublisherMetadataChannelReferenceFlags = 10,  // EvtVarTypeUInt32
//            EvtPublisherMetadataChannelReferenceMessageID = 11, // EvtVarTypeUInt32

//            EvtPublisherMetadataLevels = 12,                 // EvtVarTypeEvtHandle, ObjectArray
//            EvtPublisherMetadataLevelName = 13,              // EvtVarTypeString
//            EvtPublisherMetadataLevelValue = 14,             // EvtVarTypeUInt32
//            EvtPublisherMetadataLevelMessageID = 15,         // EvtVarTypeUInt32

//            EvtPublisherMetadataTasks = 16,                  // EvtVarTypeEvtHandle, ObjectArray
//            EvtPublisherMetadataTaskName = 17,               // EvtVarTypeString
//            EvtPublisherMetadataTaskEventGuid = 18,          // EvtVarTypeGuid
//            EvtPublisherMetadataTaskValue = 19,              // EvtVarTypeUInt32
//            EvtPublisherMetadataTaskMessageID = 20,          // EvtVarTypeUInt32

//            EvtPublisherMetadataOpcodes = 21,                // EvtVarTypeEvtHandle, ObjectArray
//            EvtPublisherMetadataOpcodeName = 22,             // EvtVarTypeString
//            EvtPublisherMetadataOpcodeValue = 23,            // EvtVarTypeUInt32
//            EvtPublisherMetadataOpcodeMessageID = 24,        // EvtVarTypeUInt32

//            EvtPublisherMetadataKeywords = 25,               // EvtVarTypeEvtHandle, ObjectArray
//            EvtPublisherMetadataKeywordName = 26,            // EvtVarTypeString
//            EvtPublisherMetadataKeywordValue = 27,           // EvtVarTypeUInt64
//            EvtPublisherMetadataKeywordMessageID = 28//,       // EvtVarTypeUInt32
//            //EvtPublisherMetadataPropertyIdEND
//        }

//        public enum EvtChannelReferenceFlags {
//            EvtChannelReferenceImported = 1
//        }

//        public enum EvtEventMetadataPropertyId {
//            EventMetadataEventID,       // EvtVarTypeUInt32
//            EventMetadataEventVersion,  // EvtVarTypeUInt32
//            EventMetadataEventChannel,  // EvtVarTypeUInt32
//            EventMetadataEventLevel,    // EvtVarTypeUInt32
//            EventMetadataEventOpcode,   // EvtVarTypeUInt32
//            EventMetadataEventTask,     // EvtVarTypeUInt32
//            EventMetadataEventKeyword,  // EvtVarTypeUInt64
//            EventMetadataEventMessageID,// EvtVarTypeUInt32
//            EventMetadataEventTemplate // EvtVarTypeString
//            //EvtEventMetadataPropertyIdEND
//        }

//        //CHANNEL CONFIGURATION 
//        public enum EvtChannelConfigPropertyId {
//            EvtChannelConfigEnabled = 0,            // EvtVarTypeBoolean
//            EvtChannelConfigIsolation,              // EvtVarTypeUInt32, EVT_CHANNEL_ISOLATION_TYPE
//            EvtChannelConfigType,                   // EvtVarTypeUInt32, EVT_CHANNEL_TYPE
//            EvtChannelConfigOwningPublisher,        // EvtVarTypeString
//            EvtChannelConfigClassicEventlog,        // EvtVarTypeBoolean
//            EvtChannelConfigAccess,                 // EvtVarTypeString
//            EvtChannelLoggingConfigRetention,       // EvtVarTypeBoolean
//            EvtChannelLoggingConfigAutoBackup,      // EvtVarTypeBoolean
//            EvtChannelLoggingConfigMaxSize,         // EvtVarTypeUInt64
//            EvtChannelLoggingConfigLogFilePath,     // EvtVarTypeString
//            EvtChannelPublishingConfigLevel,        // EvtVarTypeUInt32
//            EvtChannelPublishingConfigKeywords,     // EvtVarTypeUInt64
//            EvtChannelPublishingConfigControlGuid,  // EvtVarTypeGuid
//            EvtChannelPublishingConfigBufferSize,   // EvtVarTypeUInt32
//            EvtChannelPublishingConfigMinBuffers,   // EvtVarTypeUInt32
//            EvtChannelPublishingConfigMaxBuffers,   // EvtVarTypeUInt32
//            EvtChannelPublishingConfigLatency,      // EvtVarTypeUInt32
//            EvtChannelPublishingConfigClockType,    // EvtVarTypeUInt32, EVT_CHANNEL_CLOCK_TYPE
//            EvtChannelPublishingConfigSidType,      // EvtVarTypeUInt32, EVT_CHANNEL_SID_TYPE
//            EvtChannelPublisherList,                // EvtVarTypeString | EVT_VARIANT_TYPE_ARRAY
//            EvtChannelConfigPropertyIdEND
//        }

//        //LOG INFORMATION
//        public enum EvtLogPropertyId {
//            EvtLogCreationTime = 0,             // EvtVarTypeFileTime
//            EvtLogLastAccessTime,               // EvtVarTypeFileTime
//            EvtLogLastWriteTime,                // EvtVarTypeFileTime
//            EvtLogFileSize,                     // EvtVarTypeUInt64
//            EvtLogAttributes,                   // EvtVarTypeUInt32
//            EvtLogNumberOfLogRecords,           // EvtVarTypeUInt64
//            EvtLogOldestRecordNumber,           // EvtVarTypeUInt64
//            EvtLogFull,                         // EvtVarTypeBoolean
//        }

//        public enum EvtExportLogFlags {
//            EvtExportLogChannelPath = 1,
//            EvtExportLogFilePath = 2,
//            EvtExportLogTolerateQueryErrors = 0x1000
//        }

//        //RENDERING    
//        public enum EvtRenderContextFlags {
//            EvtRenderContextValues = 0,      // Render specific properties
//            EvtRenderContextSystem = 1,      // Render all system properties (System)
//            EvtRenderContextUser = 2         // Render all user properties (User/EventData)
//        }

//        public enum EvtRenderFlags {
//            EvtRenderEventValues = 0,       // Variants
//            EvtRenderEventXml = 1,          // XML
//            EvtRenderBookmark = 2           // Bookmark
//        }

//        public enum EvtFormatMessageFlags {
//            EvtFormatMessageEvent = 1,
//            EvtFormatMessageLevel = 2,
//            EvtFormatMessageTask = 3,
//            EvtFormatMessageOpcode = 4,
//            EvtFormatMessageKeyword = 5,
//            EvtFormatMessageChannel = 6,
//            EvtFormatMessageProvider = 7,
//            EvtFormatMessageId = 8,
//            EvtFormatMessageXml = 9
//        }

//        public enum EvtSystemPropertyId {
//            EvtSystemProviderName = 0,          // EvtVarTypeString             
//            EvtSystemProviderGuid,              // EvtVarTypeGuid  
//            EvtSystemEventID,                   // EvtVarTypeUInt16  
//            EvtSystemQualifiers,                // EvtVarTypeUInt16
//            EvtSystemLevel,                     // EvtVarTypeUInt8
//            EvtSystemTask,                      // EvtVarTypeUInt16
//            EvtSystemOpcode,                    // EvtVarTypeUInt8
//            EvtSystemKeywords,                  // EvtVarTypeHexInt64
//            EvtSystemTimeCreated,               // EvtVarTypeFileTime
//            EvtSystemEventRecordId,             // EvtVarTypeUInt64
//            EvtSystemActivityID,                // EvtVarTypeGuid
//            EvtSystemRelatedActivityID,         // EvtVarTypeGuid
//            EvtSystemProcessID,                 // EvtVarTypeUInt32
//            EvtSystemThreadID,                  // EvtVarTypeUInt32
//            EvtSystemChannel,                   // EvtVarTypeString 
//            EvtSystemComputer,                  // EvtVarTypeString 
//            EvtSystemUserID,                    // EvtVarTypeSid
//            EvtSystemVersion,                   // EvtVarTypeUInt8
//            EvtSystemPropertyIdEND
//        }



//            [StructLayout(LayoutKind.Sequential)]
//            public struct SYSTEM_INFO {
//                public int dwOemId;    // This is a union of a DWORD and a struct containing 2 WORDs.
//                public int dwPageSize;
//                public IntPtr lpMinimumApplicationAddress;
//                public IntPtr lpMaximumApplicationAddress;
//                public IntPtr dwActiveProcessorMask;
//                public int dwNumberOfProcessors;
//                public int dwProcessorType;
//                public int dwAllocationGranularity;
//                public short wProcessorLevel;
//                public short wProcessorRevision;
//            }

//            [DllImport(KERNEL32, SetLastError = true)]
//            [SecurityCritical]
//            public static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

//            [DllImport(KERNEL32, ExactSpelling = true)]
//            [SecurityCritical]
//            [return: MarshalAs(UnmanagedType.Bool)]
//            public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

//            [DllImport(KERNEL32, SetLastError = true)]
//            [SecurityCritical]
//            public static extern int GetFileSize(
//                                SafeMemoryMappedFileHandle hFile, 
//                                out int highSize
//                                );
    
    

//            [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
//            [SecurityCritical]
//            public static extern SafeMemoryMappedFileHandle CreateFileMapping(
//                                SafeFileHandle hFile, 
//                                SECURITY_ATTRIBUTES lpAttributes, 
//                                int fProtect, 
//                                int dwMaximumSizeHigh, 
//                                int dwMaximumSizeLow, 
//                                String lpName
//                                );

       

//            [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
//            [SecurityCritical]
//            public static extern SafeMemoryMappedFileHandle OpenFileMapping(
//                                int dwDesiredAccess, 
//                                [MarshalAs(UnmanagedType.Bool)] 
//                                bool bInheritHandle, 
//                                string lpName
//                                );

//            [DllImport(KERNEL32, SetLastError = true, ExactSpelling = true)]
//            [SecurityCritical]
//            public static extern SafeMemoryMappedViewHandle MapViewOfFile(
//                                SafeMemoryMappedFileHandle handle,
//                                int dwDesiredAccess, 
//                                uint dwFileOffsetHigh, 
//                                uint dwFileOffsetLow, 
//                                UIntPtr dwNumberOfBytesToMap
//                                );

       

//            [SecurityCritical]
//            public static bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer)
//            {
//                lpBuffer.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
//                return GlobalMemoryStatusExNative(ref lpBuffer);
//            }
                                
//            [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "GlobalMemoryStatusEx")]
//            [SecurityCritical]
//            [return: MarshalAs(UnmanagedType.Bool)]
//            private static extern bool GlobalMemoryStatusExNative([In, Out] ref MEMORYSTATUSEX lpBuffer);

         
//            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//            public struct MEMORYSTATUSEX {
//                public uint dwLength;
//                public uint dwMemoryLoad;
//                public ulong ullTotalPhys;
//                public ulong ullAvailPhys;
//                public ulong ullTotalPageFile;
//                public ulong ullAvailPageFile;
//                public ulong ullTotalVirtual;
//                public ulong ullAvailVirtual;
//                public ulong ullAvailExtendedVirtual;
//            }
//    }
//}
