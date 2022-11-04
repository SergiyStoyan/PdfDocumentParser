/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using Cliver.WinApi;
using System;
using System.Runtime.InteropServices;

namespace Cliver.Win
{
    public static class SessionRoutines
    {
        /// <summary>
        /// Returns the Logon Session string
        /// CREDIT: https://stackoverflow.com/questions/6061143/how-to-get-a-unique-id-for-the-current-users-logon-session-in-windows-c-sharp
        /// </summary>
        /// <returns></returns>
        public static string GetLogonSession()
        {
            string sidString = "";
            IntPtr hdesk = User32.GetThreadDesktop(Kernel32.GetCurrentThreadId());
            byte[] buf = new byte[100];
            uint lengthNeeded;
            User32.GetUserObjectInformation(hdesk, User32.UOI_USER_SID, buf, 100, out lengthNeeded);
            IntPtr ptrSid;
            if (!Advapi32.ConvertSidToStringSid(buf, out ptrSid))
                throw new System.ComponentModel.Win32Exception();
            try
            {
                sidString = Marshal.PtrToStringAuto(ptrSid);
            }
            catch
            {
            }
            return sidString;
        }

        //public static bool GetSessionUserToken(ref IntPtr phUserToken)
        //{
        //    var bResult = false;
        //    var hImpersonationToken = IntPtr.Zero;
        //    var activeSessionId = INVALID_SESSION_ID;
        //    var pSessionInfo = IntPtr.Zero;
        //    var sessionCount = 0;

        //    // Get a handle to the user access token for the current active session.
        //    if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
        //    {
        //        var arrayElementSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
        //        var current = pSessionInfo;

        //        for (var i = 0; i < sessionCount; i++)
        //        {
        //            var si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
        //            current += arrayElementSize;

        //            if (si.State == WTS_CONNECTSTATE_CLASS.WTSActive)
        //            {
        //                activeSessionId = si.SessionID;
        //            }
        //        }
        //    }

        //    // If enumerating did not work, fall back to the old method
        //    if (activeSessionId == INVALID_SESSION_ID)
        //    {
        //        activeSessionId = WTSGetActiveConsoleSessionId();
        //    }

        //    if (WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
        //    {
        //        // Convert the impersonation token to a primary token
        //        bResult = DuplicateTokenEx(hImpersonationToken, 0, IntPtr.Zero,
        //            (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, (int)TOKEN_TYPE.TokenPrimary,
        //            ref phUserToken);

        //        CloseHandle(hImpersonationToken);
        //    }

        //    return bResult;
        //}
    }
}

