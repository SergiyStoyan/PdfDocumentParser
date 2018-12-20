//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        15 December 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cliver.WinApi
{
    public partial class Secur32
    {
        [DllImport("secur32.dll", SetLastError = false)]
        public static extern uint LsaFreeReturnBuffer(IntPtr buffer);

        [DllImport("Secur32.dll", SetLastError = false)]
        public static extern uint LsaEnumerateLogonSessions(out UInt64 LogonSessionCount, out IntPtr LogonSessionList);

        [DllImport("Secur32.dll", SetLastError = false)]
        public static extern uint LsaGetLogonSessionData(IntPtr luid, out IntPtr ppLogonSessionData);

        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public UInt32 LowPart;
            public UInt32 HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_LOGON_SESSION_DATA
        {
            public UInt32 Size;
            public LUID LoginID;
            public LSA_UNICODE_STRING Username;
            public LSA_UNICODE_STRING LoginDomain;
            public LSA_UNICODE_STRING AuthenticationPackage;
            public UInt32 LogonType;
            public UInt32 Session;
            public IntPtr PSiD;
            public UInt64 LoginTime;
            public LSA_UNICODE_STRING LogonServer;
            public LSA_UNICODE_STRING DnsDomainName;
            public LSA_UNICODE_STRING Upn;
        }

        public enum SECURITY_LOGON_TYPE : uint
        {
            Interactive = 2,        //The security principal is logging on interactively.
            Network,                //The security principal is logging using a network.
            Batch,                  //The logon is for a batch process.
            Service,                //The logon is for a service account.
            Proxy,                  //Not supported.
            Unlock,                 //The logon is an attempt to unlock a workstation.
            NetworkCleartext,       //The logon is a network logon with cleartext credentials.
            NewCredentials,         //Allows the caller to clone its current token and specify new credentials for outbound connections.
            RemoteInteractive,      //A terminal server session that is both remote and interactive.
            CachedInteractive,      //Attempt to use the cached credentials without going out across the network.
            CachedRemoteInteractive,// Same as RemoteInteractive, except used internally for auditing purposes.
            CachedUnlock            // The logon is an attempt to unlock a workstation.
        }
    }
}