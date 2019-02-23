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
    public partial class User32
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static public extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static public extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        static public extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        public class WindowPositionFlags
        {
            public const uint SWP_ASYNCWINDOWPOS = 0x4000;
            public const uint SWP_DEFERERASE = 0x2000;
            public const uint SWP_DRAWFRAME = 0x0020;
            public const uint SWP_FRAMECHANGED = 0x0020;
            public const uint SWP_HIDEWINDOW = 0x0080;
            public const uint SWP_NOACTIVATE = 0x0010;
            public const uint SWP_NOCOPYBITS = 0x0100;
            public const uint SWP_NOMOVE = 0x0002;
            public const uint SWP_NOOWNERZORDER = 0x0200;
            public const uint SWP_NOREDRAW = 0x0008;
            public const uint SWP_NOREPOSITION = 0x0200;
            public const uint SWP_NOSENDCHANGING = 0x0400;
            public const uint SWP_NOSIZE = 0x0001;
            public const uint SWP_NOZORDER = 0x0004;
            public const uint SWP_SHOWWINDOW = 0x0040;
        }

        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        //public class MemoryProtection
        //{
        //    public const int PAGE_READWRITE = 0x04;
        //    public const int PAGE_NOACCESS = 0x01;
        //    public const int PAGE_GUARD = 0x100;
        //}

        //public class MemoryState
        //{
        //    public const int MEM_COMMIT = 0x00001000;
        //}

        //public class ProcessRights
        //{
        //    public const int PROCESS_QUERY_INFORMATION = 0x0400;
        //    public const int PROCESS_WM_READ = 0x0010;
        //}

        public const short SW_SHOW = 5;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetForegroundWindow(IntPtr h);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern byte VkKeyScan(char c);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hook, HookProc callback, IntPtr hMod, uint dwThreadId);
        public enum HookType : uint
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
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

        [DllImport("user32.dll")]
        public static extern int GetDlgItem(IntPtr hWnd, int itemId);

        [DllImport("user32.dll")]
        public static extern int GetDlgItemText(IntPtr hWnd, int itemId, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hwnd, string str);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int InternalGetWindowText(IntPtr hwnd, StringBuilder text, int nMaxCount);

        [DllImport("User32.Dll")]
        public static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);
        public enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorCmd uCmd);
        public enum GetAncestorCmd : uint
        {
            GA_PARENT = 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3
        }

        //[DllImport("user32.dll")]
        //public static extern int GetWindowLong(IntPtr hWnd, GetAncestorCmd uCmd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern int FindWindowEx(IntPtr parent_h, IntPtr child_h, string lpClassName, string lpWindowName);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, Int64 lParam);

        [DllImport("User32.dll", EntryPoint = "SetActiveWindow")]
        public static extern int SetActiveWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumProc cbf, int lParam);
        public delegate bool EnumProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hwnd, EnumProc cbf, int lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("User32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumProc cbf, IntPtr lParam);
        
        [DllImport("User32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumProc cbf, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MONITORINFOEX
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x16,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
    }
}