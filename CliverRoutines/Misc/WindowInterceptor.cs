using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
//using Cliver.Win32;
using System.Windows.Forms;
using System.Collections;
using Cliver;

namespace Cliver
{    
    /// <summary>
    /// Intercept dialog box creation
    /// </summary>
    static public class WindowInterceptor
    {
        static object static_lock_variable = new object();

        static IntPtr hook_id = IntPtr.Zero;
        static WinApi.User32.HookProc cbf = new WinApi.User32.HookProc(wnd_hook_proc);

        static IntPtr[] owner_windows = new IntPtr[0];
        static Dictionary<IntPtr, Cliver.Log.Writer> owner_window_logs = new Dictionary<IntPtr, Cliver.Log.Writer>();

        /// <summary>
        /// Add new owner window to be traced for dialog box creating
        /// </summary>
        /// <param name="Log">Log of owner window</param>
        /// <param name="owner_window">owner window</param>
        static public void AddOwnerWindow(IntPtr owner_window)
        {
            lock (static_lock_variable)
            {
                try
                {
                    if (owner_window_logs.ContainsKey(owner_window))
                        return;
                    ICollection a = (ICollection)owner_windows.Clone();
                    ArrayList al = new ArrayList((ICollection)a);
                    al.Add(owner_window);
                    owner_windows = (IntPtr[])al.ToArray(typeof(IntPtr));

                    owner_window_logs[owner_window] = Log.This;

                    if (hook_id == IntPtr.Zero)
                        hook_id = WinApi.User32.SetWindowsHookEx(WinApi.User32.HookType.WH_CALLWNDPROCRET, cbf, IntPtr.Zero, WinApi.Kernel32.GetCurrentThreadId());
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        /// <summary>
        /// Remove owner window from hook tracing
        /// </summary>
        /// <param name="owner_window">owner window</param>
        static public void RemoveOwnerWindow(IntPtr owner_window)
        {
            lock (static_lock_variable)
            {
                try
                {
                    ICollection a = (ICollection)owner_windows.Clone();
                    ArrayList al = new ArrayList((ICollection)a);
                    al.Remove(owner_window);
                    owner_windows = (IntPtr[])al.ToArray(typeof(IntPtr));

                    owner_window_logs.Remove(owner_window);

                    if (owner_windows.Length < 1 && hook_id != IntPtr.Zero)
                    {
                        WinApi.User32.UnhookWindowsHookEx(hook_id);
                        hook_id = IntPtr.Zero;
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        // <summary>
        // Start dialog box interception for the specified owner window
        // </summary>
        // <param name="Log">Log of owner window</param>
        // <param name="owner_window">owner window</param>
        //static public void Start(Log Log, IntPtr owner_window)
        //{
        //    lock (static_lock_variable)
        //    {
        //        try
        //        {
        //            Stop();
        //            AddOwnerWindow(Log, owner_window);

        //            if (hook_id == IntPtr.Zero)
        //                hook_id = Win32.Functions.SetWindowsHookEx(Win32.HookType.WH_CALLWNDPROCRET, cbf, IntPtr.Zero, Win32.Functions.GetCurrentThreadId());
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Main.Exit(e);
        //        }
        //    }
        //}

        /// <summary>
        /// Stop dialog box interception
        /// </summary>
        static public void Stop()
        {
            lock (static_lock_variable)
            {
                try
                {
                    owner_windows = new IntPtr[0];
                    owner_window_logs.Clear();

                    if (hook_id != IntPtr.Zero)
                    {
                        WinApi.User32.UnhookWindowsHookEx(hook_id);
                        hook_id = IntPtr.Zero;
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        static private IntPtr wnd_hook_proc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            lock (static_lock_variable)
            {
                try
                {
                    if (nCode < 0)
                        return WinApi.User32.CallNextHookEx(hook_id, nCode, wParam, lParam);

                    WinApi.Wininet.CWPRETSTRUCT msg = (WinApi.Wininet.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.Wininet.CWPRETSTRUCT));

                    if (msg.message == (uint)WinApi.Messages.WM_SHOWWINDOW)
                    {
                        //check if owner is that was specified
                        IntPtr h = WinApi.User32.GetWindow(msg.hwnd, WinApi.User32.GetWindowCmd.GW_OWNER);
                        foreach (IntPtr owner_window in owner_windows)
                        {
                            if (owner_window != h)
                            {
                                StringBuilder text2 = new StringBuilder(255);
                                WinApi.User32.GetWindowText(h, text2, 255);
                                if (!text2.ToString().Contains("WindowsFormsParkingWindow"))
                                    continue;
                            }

                            StringBuilder text = new StringBuilder(255);
                            WinApi.User32.GetWindowText(msg.hwnd, text, 255);
                            owner_window_logs[owner_window].Write("Intercepted dialog box: " + text.ToString());

                            //short dw = (short)Win32.Functions.SendMessage(msg.hwnd, (uint)Win32.Messages.DM_GETDEFID, 0, 0);
                            //Win32.Functions.EndDialog(msg.hwnd, (IntPtr)dw);
                            WinApi.User32.SendMessage(msg.hwnd, (uint)WinApi.Messages.WM_CLOSE, 0, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }

                return WinApi.User32.CallNextHookEx(hook_id, nCode, wParam, lParam);
            }
        }
    }
}
