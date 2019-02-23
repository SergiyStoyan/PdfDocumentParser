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
    public partial class Wininet
    {
        [DllImport("Wininet.dll", SetLastError = true)]
        //public static extern bool GetUrlCacheEntryInfo(string Url, StringBuilder CacheFile, ref int Size);
        public static extern bool GetUrlCacheEntryInfo(string Url, IntPtr lpCacheEntryInfo, ref int Size);
        
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32 dwFlags, IntPtr lpReserved);
        public const Int32 InternetCookieHttponly = 0x2000;

        [DllImport("wininet.dll")]
        public static extern bool InternetGetCookie(string Url, string CookieName, StringBuilder CookieData, ref int Size);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        
        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }
    }
}