//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Versioning;

namespace Cliver.WinApi
{
    [HostProtectionAttribute(MayLeakOnAbort = true)]
    [SuppressUnmanagedCodeSecurityAttribute]
    public sealed class SafeLocalMemHandle : SafeHandleZeroOrMinusOneIsInvalid
    { 
        public SafeLocalMemHandle() : base(true) {}
        
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public SafeLocalMemHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle) {
            SetHandle(existingHandle);
        }


        //[DllImport("Advapi32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto, SetLastError=true, BestFitMapping=false)]
        //[ResourceExposure(ResourceScope.None)]
        //public static extern unsafe bool ConvertStringSecurityDescriptorToSecurityDescriptor(string StringSecurityDescriptor, int StringSDRevision, out SafeLocalMemHandle pSecurityDescriptor, IntPtr SecurityDescriptorSize);

        [DllImport("kernel32.dll")]
        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        override protected bool ReleaseHandle()
        {
            return LocalFree(handle) == IntPtr.Zero;
        }

    }
    
    [SuppressUnmanagedCodeSecurityAttribute]
    public sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeThreadHandle() : base(true)
        {
        }

        public void InitialSetHandle(IntPtr h)
        {
            //Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
            base.SetHandle(h);
        }

        override protected bool ReleaseHandle()
        {
            return Kernel32.CloseHandle(handle);
        }
    }

    [HostProtectionAttribute(MayLeakOnAbort = true)]
    [SuppressUnmanagedCodeSecurityAttribute]
    public sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Note that CreateFileMapping returns 0 on failure.

        // Note that you can pass in -1 for the hFile parameter.
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public SafeFileMappingHandle() : base(true) { }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern bool CloseHandle(IntPtr handle);

        override protected bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}





