//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************


using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cliver
{
    /// <summary>
    /// Show MessageForm with predefined features
    /// </summary>
    public static partial class Message
    {
        /// <summary>
        /// Whether the message box must be displayed in the Windows taskbar.
        /// </summary>
        public static bool ShowInTaskbar = true;

        /// <summary>
        /// Whether the message box must be displayed topmost.
        /// </summary>
        public static bool TopMost = false;

        /// <summary>
        /// Owner that is used by default
        /// </summary>
        public static Form Owner;
        //{
        //    set
        //    {
        //        _Owner = value;
        //    }
        //    get
        //    {
        //        if (_Owner != null)
        //            return _Owner;
        //        return Application.OpenForms.Cast<Form>().Last();
        //    }
        //}
        //static Form _Owner = null;

        /// <summary>
        /// Autosize buttons by text
        /// </summary>
        public static bool ButtonAutosize = false;

        /// <summary>
        /// Display only one message box for all same messages throuwn. When the first one is being diplayed, the rest are ignored.
        /// </summary>
        public static bool NoDuplicate = true;

        public readonly static string AppName = getAppName();
        static string getAppName()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string n = a.GetProduct();
            if (n != null)
                return n;
            n = a.GetTitle();
            if (n != null)
                return n;
            n = ProgramRoutines.GetAppName();
            return n;
        }

        public static void Inform(string message, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Information), message, new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim(string message, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Exclamation), message, new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim0(Exception e, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Exclamation), /*GetExceptionDetails(e)*/Log.GetExceptionMessage(e, !(e is Exception2)), new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim(Exception e, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Exclamation), Log.GetExceptionMessage2(e), new string[1] { "OK" }, 0, owner);
        }

        public static void Warning(string message, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Warning), message, new string[1] { "OK" }, 0, owner);
        }

        public static void Warning(Exception e, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Warning), /*GetExceptionDetails(e)*/Log.GetExceptionMessage(e, !(e is Exception2)), new string[1] { "OK" }, 0, owner);
        }

        public static void Warning2(Exception e, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Warning), Log.GetExceptionMessage2(e), new string[1] { "OK" }, 0, owner);
        }

        public static void Error(Exception e, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Error), /*GetExceptionDetails(e)*/Log.GetExceptionMessage(e, !(e is Exception2)), new string[1] { "OK" }, 0, owner);
        }

        public static void Error2(Exception e, Form owner = null)
        {
            Error(Log.GetExceptionMessage2(e), owner);
        }

        //public static string GetExceptionDetails(Exception e)
        //{
        //    List<string> ms = new List<string>();
        //    bool stack_trace_added = false;
        //    for (; e != null; e = e.InnerException)
        //    {
        //        string s = e.Message;
        //        if (!stack_trace_added && e.StackTrace != null)
        //        {
        //            stack_trace_added = true;
        //            s += "\r\n" + e.StackTrace;
        //        }
        //        ms.Add(s);
        //    }
        //    return string.Join("\r\n=>\r\n", ms);
        //}

        public static void Error(string subtitle, Exception e, Form owner = null)
        {
            Error(subtitle + "\r\n\r\n" + Log.GetExceptionMessage(e, !(e is Exception2)), owner);
        }

        public static void Error2(string subtitle, Exception e, Form owner = null)
        {
            Error(subtitle + "\r\n\r\n" + Log.GetExceptionMessage2(e), owner);
        }

        public static void Error(string message, Form owner = null)
        {
            ShowDialog(AppName, getIcon(Icons.Error), message, new string[1] { "OK" }, 0, owner);
        }

        public static bool YesNo(string question, Form owner = null, Icons icon = Icons.Question, bool defaultIsYes = true)
        {
            return ShowDialog(AppName, getIcon(icon), question, new string[2] { "Yes", "No" }, defaultIsYes ? 0 : 1, owner) == 0;
        }

        public static int ShowDialog(string title, Icons icon, string message, string[] buttons, int defaultButton, Form owner = null, bool? buttonAutosize = null, bool? noDuplicate = null, bool? topmost = null)
        {
            return ShowDialog(title, getIcon(icon), message, buttons, defaultButton, owner, buttonAutosize, noDuplicate, topmost);
        }

        public static int ShowDialog(string title, Icon icon, string message, string[] buttons, int defaultButton, Form owner, bool? buttonAutosize = null, bool? noDuplicate = null, bool? topmost = null)
        {
            if (title == null)
                title = AppName;
            owner = owner ?? Owner;
            if (owner != null && !owner.Visible)
                owner = null;
            if (owner == null || !owner.InvokeRequired)
                return show_dialog(title, icon, message, buttons, defaultButton, owner, buttonAutosize, noDuplicate, topmost);

            return (int)owner.Invoke(() =>
            {
                return show_dialog(title, icon, message, buttons, defaultButton, owner, buttonAutosize, noDuplicate, topmost);
            });
        }

        static int show_dialog(string title, Icon icon, string message, string[] buttons, int defaultButton, Form owner, bool? buttonAutosize = null, bool? noDuplicate = null, bool? top_most = null)
        {
            string caller = null;
            if (noDuplicate ?? NoDuplicate)
            {
                StackTrace st = new StackTrace(true);
                StackFrame sf = null;
                for (int i = 1; i < st.FrameCount; i++)
                {
                    sf = st.GetFrame(i);
                    string file_name = sf.GetFileName();
                    if (file_name == null || !Regex.IsMatch(file_name, @"\\Message\.cs$"))
                        break;
                }
                caller = sf.GetMethod().Name + "," + sf.GetNativeOffset().ToString();
                lock (callers2message)
                {
                    if (callers2message.TryGetValue(caller, out string m) && m == message)
                        return -1;
                    callers2message[caller] = message;
                }
            }

            MessageForm mf = new MessageForm(title, icon, message, buttons, defaultButton, owner, buttonAutosize ?? ButtonAutosize);
            mf.ShowInTaskbar = ShowInTaskbar;
            mf.TopMost = top_most ?? TopMost;
            int result = mf.ShowDialog();

            if (noDuplicate ?? NoDuplicate)
                lock (callers2message)
                {
                    callers2message.Remove(caller);
                }

            return result;
        }
        //public static void AttachedThreadInputAction(Action action)
        //{
        //    uint pid;
        //    var foreThread = WinApi.User32.GetWindowThreadProcessId(WinApi.User32.GetForegroundWindow(), out pid);
        //    var appThread = WinApi.Kernel32.GetCurrentThreadId();
        //    bool threadsAttached = false;
        //    try
        //    {
        //        threadsAttached = foreThread == appThread || WinApi.User32.AttachThreadInput(foreThread, appThread, true);
        //        if (threadsAttached)
        //            action();
        //        else
        //            throw new Exception("AttachThreadInput failed.");
        //    }
        //    finally
        //    {
        //        if (threadsAttached)
        //            WinApi.User32.AttachThreadInput(foreThread, appThread, false);
        //    }
        //}
        static Dictionary<string, string> callers2message = new Dictionary<string, string>();
        public enum Icons
        {
            Information,
            Warning,
            Error,
            Question,
            Exclamation,
        }
        //static Icon get_icon(Icons icon)
        //{
        //    switch (icon)
        //    {
        //        case Icons.Information:
        //            return SystemIcons.Information;
        //        case Icons.Warning:
        //            return SystemIcons.Warning;
        //        case Icons.Error:
        //            return SystemIcons.Error;
        //        case Icons.Question:
        //            return SystemIcons.Question;
        //        case Icons.Exclamation:
        //            return SystemIcons.Exclamation;
        //        default: throw new Exception("No option: " + icon);
        //    }
        //}
        static public Icon getIcon(Icons icon)
        {
            WinApi.Shell32.SHSTOCKICONID iId;
            switch (icon)
            {
                case Icons.Information:
                    iId = WinApi.Shell32.SHSTOCKICONID.SIID_INFO;
                    break;
                case Icons.Warning:
                    iId = WinApi.Shell32.SHSTOCKICONID.SIID_WARNING;
                    break;
                case Icons.Error:
                    iId = WinApi.Shell32.SHSTOCKICONID.SIID_ERROR;
                    break;
                case Icons.Question:
                    iId = WinApi.Shell32.SHSTOCKICONID.SIID_HELP;
                    break;
                case Icons.Exclamation:
                    iId = WinApi.Shell32.SHSTOCKICONID.SIID_WARNING;
                    break;
                default: throw new Exception("No option: " + icon);
            }
            var sii = new WinApi.Shell32.SHSTOCKICONINFO();
            sii.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinApi.Shell32.SHSTOCKICONINFO));
            WinApi.Shell32.SHGetStockIconInfo(iId, WinApi.Shell32.SHGSI.SHGSI_ICON, ref sii);
            return Icon.FromHandle(sii.hIcon);
        }
    }
}

