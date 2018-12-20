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

namespace Win32
{
    static public class Functions
    {
        public delegate bool EnumProc(IntPtr hwnd, int lParam);
        //public delegate IntPtr HookProc(IntPtr nCode, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hook, HookProc callback, IntPtr hMod, uint dwThreadId);
        

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        } 
        
        [DllImport("user32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);
        
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        
        //[DllImport("user32.dll")]
        //public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, CustomWindowProc dwNewLong);
        
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        
        //[DllImport("user32.dll")] 
        //public static extern IntPtr SetClassLong(IntPtr hWnd, int nIndex, CustomWindowProc dwNewLong); 
        
        //[DllImport("user32.dll")] 
        //public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam,IntPtr lParam); 
        
        [DllImport("user32.dll")]
        public static extern IntPtr DefDlgProc(IntPtr hDlg, uint Msg, IntPtr wParam, IntPtr lParam);


        //[DllImport("User32.dll")]
        //public static extern UIntPtr SetTimer(IntPtr hwnd, UIntPtr nIDEvent, uint uElapse, CallBack cbf);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);



        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hwnd, string str);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int InternalGetWindowText(IntPtr hwnd, StringBuilder text, int nMaxCount);

        [DllImport("User32.Dll")]
        public static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern int FindWindowEx(IntPtr parent_h, IntPtr child_h, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern int GetWindow(IntPtr hwnd, int flag);
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int GW_MAX = 5;


        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern int GetAncestor(IntPtr hwnd, int flag);
        public const int GA_PARENT = 1;
        public const int GA_ROOT = 2;
        public const int GA_ROOTOWNER = 3;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SetActiveWindow")]
        public static extern int SetActiveWindow(IntPtr hwnd);

        [DllImport("user32")]
        public static extern int EnumWindows(EnumProc cbf, int lParam);

        [DllImport("user32")]
        public static extern int EnumChildWindows(IntPtr hwnd, EnumProc cbf, int lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("User32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumProc cbf, int lParam);

        [DllImport("Wininet.dll")]
        public static extern bool InternetGetCookie(string Url, string CookieName, StringBuilder CookieData, ref int Size);

        //InternetSetCookie

        [DllImport("Wininet.dll", SetLastError = true)]
        //public static extern bool GetUrlCacheEntryInfo(string Url, StringBuilder CacheFile, ref int Size);
        public static extern bool GetUrlCacheEntryInfo(string Url, IntPtr lpCacheEntryInfo, ref int Size);
    }

    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public struct CWPRETSTRUCT
    {
        public IntPtr lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNET_CACHE_ENTRY_INFO
    {
        public UInt32 dwStructSize;
        public string lpszSourceUrlName;
        public string lpszLocalFileName;
        public UInt32 CacheEntryType;
        public UInt32 dwUseCount;
        public UInt32 dwHitRate;
        public UInt32 dwSizeLow;
        public UInt32 dwSizeHigh;
        public FILETIME LastModifiedTime;
        public FILETIME ExpireTime;
        public FILETIME LastAccessTime;
        public FILETIME LastSyncTime;
        public IntPtr lpHeaderInfo;
        public UInt32 dwHeaderInfoSize;
        public string lpszFileExtension;
        public UInt32 dwExemptDelta;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME
    {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }
}
