//// ==++==
//// 
////   Copyright (c) Microsoft Corporation.  All rights reserved.
//// 
//// ==--==
///*============================================================
//**
//** Class:  SafeFileMappingHandle
//**
//** <EMAIL>Author: David Gutierrez (Microsoft) </EMAIL>
//**
//** A wrapper for handle to file mappings, returned by 
//** CreateFileMapping and OpenFileMapping.  Used for shared 
//** memory.
//**
//** Date:  July 8, 2002
//** 
//===========================================================*/

//using System;
//using System.Security;
//using System.Security.Permissions;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using Microsoft.Win32;
//using Microsoft.Win32.SafeHandles;
//using System.Runtime.ConstrainedExecution;
//using System.Runtime.Versioning;

//namespace Microsoft.Win32.SafeHandles {
//    [HostProtectionAttribute(MayLeakOnAbort = true)]
//    [SuppressUnmanagedCodeSecurityAttribute]
//    public sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
//    { 
//        // Note that CreateFileMapping returns 0 on failure.

//        // Note that you can pass in -1 for the hFile parameter.
//        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
//        public SafeFileMappingHandle() : base(true) {}
             
//        [DllImport("kernel32.dll", ExactSpelling=true, SetLastError=true)]
//        [ResourceExposure(ResourceScope.None)]
//        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
//        private static extern bool CloseHandle(IntPtr handle);

//        override protected bool ReleaseHandle()
//        {
//            return CloseHandle(handle);
//        }
//    }
//}
