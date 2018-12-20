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
    public static class WindowsUserRoutines
    {
        public static string GetUserNameBySessionId(uint sessionId, bool prependDomain = false)
        {
            string username = null;//"SYSTEM";
            IntPtr buffer = IntPtr.Zero;
            try
            {
                int strLen;
                if (WinApi.Wts.WTSQuerySessionInformation(IntPtr.Zero, sessionId, WinApi.Wts.WTS_INFO_CLASS.WTSUserName, out buffer, out strLen))
                {
                    username = Marshal.PtrToStringAnsi(buffer);
                    if (!prependDomain)
                        return username;
                    WinApi.Wts.WTSFreeMemory(buffer);
                    buffer = IntPtr.Zero;
                    if (WinApi.Wts.WTSQuerySessionInformation(IntPtr.Zero, sessionId, WinApi.Wts.WTS_INFO_CLASS.WTSDomainName, out buffer, out strLen))
                        return Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    WinApi.Wts.WTSFreeMemory(buffer);
            }
            return username;
        }

        //static public string GetUserName()
        //{
        //    uint session_id = WinApi.Wts.WTSGetActiveConsoleSessionId();
        //    if (session_id == 0xFFFFFFFF)
        //        return null;

        //    IntPtr buffer;
        //    int strLen;
        //    if (!WinApi.Wts.WTSQuerySessionInformation(IntPtr.Zero, session_id, WinApi.Wts.WTS_INFO_CLASS.WTSUserName, out buffer, out strLen) || strLen < 1)
        //        return null;

        //    string userName = Marshal.PtrToStringAnsi(buffer);
        //    WinApi.Wts.WTSFreeMemory(buffer);
        //    return userName;
        //}

        static public string GetCurrentUserName2()
        {
            return System.Windows.Forms.SystemInformation.UserName;
        }

        static public string GetCurrentUserName3()
        {
            //return Regex.Replace(Environment.UserName, @".*\\", "");//RunAs ?
            return Regex.Replace(WindowsIdentity.GetCurrent().Name, @".*\\", "");
        }

        static public string GetCurrentUserName4()
        {
            return UserPrincipal.Current.DisplayName;//runs unacceptably long time
        }

        public static bool CurrentUserHasElevatedPrivileges()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        //public static bool IsAdministrator()
        //{
        //    return Win32.IsUserAnAdmin();
        //}

        //private static bool IsAdministrator(string username)//very slow
        //{//https://ayende.com/blog/158401/are-you-an-administrator
        //    PrincipalContext ctx;
        //    try
        //    {
        //        Domain.GetComputerDomain();
        //        try
        //        {
        //            ctx = new PrincipalContext(ContextType.Domain);
        //        }
        //        catch (PrincipalServerDownException)
        //        {
        //            // can't access domain, check local machine instead 
        //            ctx = new PrincipalContext(ContextType.Machine);
        //        }
        //    }
        //    catch (ActiveDirectoryObjectNotFoundException)
        //    {
        //        // not in a domain
        //        ctx = new PrincipalContext(ContextType.Machine);
        //    }
        //    var up = UserPrincipal.FindByIdentity(ctx, username);
        //    if (up != null)
        //    {
        //        PrincipalSearchResult<Principal> authGroups = up.GetAuthorizationGroups();
        //        return authGroups.Any(principal =>
        //                              principal.Sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountDomainAdminsSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountEnterpriseAdminsSid));
        //    }
        //    return false;
        //}

        static public bool CurrentUserIsAdministrator()
        {
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity == null)
                throw new InvalidOperationException("Couldn't get the current user identity");
            var principal = new WindowsPrincipal(windowsIdentity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                return true;

            // If we're not running in Vista onwards, we don't have to worry about checking for UAC.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major < 6)
                // Operating system does not support UAC; skipping elevation check.
                return false;

            int tokenInfLength = Marshal.SizeOf(typeof(int));
            IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);
            try
            {
                uint returnedSize = 0;
                if (!WinApi.Advapi32.GetTokenInformation(windowsIdentity.Token, WinApi.Advapi32.TOKEN_INFORMATION_CLASS.TokenElevationType, tokenInformation, (uint)tokenInfLength, out returnedSize))
                    throw new InvalidOperationException("Couldn't get token information", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

                switch ((WinApi.Advapi32.TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(tokenInformation))
                {
                    case WinApi.Advapi32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault:
                        // TokenElevationTypeDefault - User is not using a split token, so they cannot elevate.
                        return false;
                    case WinApi.Advapi32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull:
                        // TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
                        return true;
                    case WinApi.Advapi32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited:
                        // TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
                        return true;
                    default:
                        // Unknown token elevation type.
                        return false;
                }
            }
            finally
            {
                if (tokenInformation != IntPtr.Zero)
                    Marshal.FreeHGlobal(tokenInformation);
            }
        }
    }
}