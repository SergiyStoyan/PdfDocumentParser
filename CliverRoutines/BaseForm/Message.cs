//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace Cliver
{
    /// <summary>
    /// Show MessageForm with predefined features
    /// </summary>
    public static class Message
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

        public readonly static string AppName = ProgramRoutines.GetAppName();

        public static bool ShowDetailsOnException =
#if DEBUG
            true
#else
            false
#endif
            ;

        public static void Inform(string message, Form owner = null)
        {
            ShowDialog(AppName, SystemIcons.Information, message, new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim(string message, Form owner = null)
        {
            ShowDialog(AppName, SystemIcons.Exclamation, message, new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim(Exception e, Form owner = null)
        {
            if (!ShowDetailsOnException)
                ShowDialog(AppName, SystemIcons.Exclamation, e.Message, new string[1] { "OK" }, 0, owner);
            else
                ShowDialog(AppName, SystemIcons.Exclamation, GetExceptionDetails(e), new string[1] { "OK" }, 0, owner);
        }

        public static void Warning(string message, Form owner = null)
        {
            ShowDialog(AppName, SystemIcons.Warning, message, new string[1] { "OK" }, 0, owner);
        }

        public static void Warning(Exception e, Form owner = null)
        {
            if (!ShowDetailsOnException)
                ShowDialog(AppName, SystemIcons.Warning, e.Message, new string[1] { "OK" }, 0, owner);
            else
                ShowDialog(AppName, SystemIcons.Warning, GetExceptionDetails(e), new string[1] { "OK" }, 0, owner);
        }

        public static void Error(Exception e, Form owner = null)
        {
            if(!ShowDetailsOnException)
                Error(e.Message, owner);
            else
                ShowDialog(AppName, SystemIcons.Error, GetExceptionDetails(e), new string[1] { "OK" }, 0, owner);
        }

        public static string GetExceptionDetails(Exception e)
        {
            List<string> ms = new List<string>();
            bool stack_trace_added = false;
            for (; e != null; e = e.InnerException)
            {
                string s = e.Message;
                if (!stack_trace_added && e.StackTrace != null)
                {
                    stack_trace_added = true;
                    s += "\r\n" + e.StackTrace;
                }
                ms.Add(s);
            }
            return string.Join("\r\n=>\r\n", ms);
        }

        /// <summary>
        /// Display exception without tracing inforamation.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="owner"></param>
        public static void Error2(Exception e, Form owner = null)
        {
            Error(e.Message, owner);
        }

        /// <summary>
        /// Display exception without tracing inforamation.
        /// </summary>
        /// <param name="subtitle"></param>
        /// <param name="e"></param>
        /// <param name="owner"></param>
        public static void Error2(string subtitle, Exception e, Form owner = null)
        {
            Error(subtitle + "\r\n\r\n" + e.Message, owner);
        }

        public static void Error(string message, Form owner = null)
        {
            ShowDialog(AppName, SystemIcons.Error, message, new string[1] { "OK" }, 0, owner);
        }

        public static bool YesNo(string question, Form owner = null, Icons icon = Icons.Question, bool default_yes = true)
        {
            return ShowDialog(AppName, get_icon(icon), question, new string[2] { "Yes", "No" }, default_yes ? 0 : 1, owner) == 0;
        }

        public static int ShowDialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner, bool? button_autosize = null, bool? no_duplicate = null, bool? topmost = null)
        {
            owner = owner ?? Owner;
            if (owner != null && !owner.Visible)
                owner = null;
            if (owner == null || !owner.InvokeRequired)
                return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate, topmost);

            return (int)owner.Invoke(() =>
           {
               return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate, topmost);
           });
        }

        static int show_dialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner, bool? button_autosize = null, bool? no_duplicate = null, bool? top_most = null)
        {
            string caller = null;
            if (no_duplicate ?? NoDuplicate)
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
                string m = null;
                lock (callers2message)
                {
                    if (callers2message.TryGetValue(caller, out m) && m == message)
                        return -1;
                    callers2message[caller] = message;
                }
            }

            MessageForm mf = new MessageForm(title, icon, message, buttons, default_button, owner, button_autosize ?? ButtonAutosize);
            mf.ShowInTaskbar = ShowInTaskbar;
            mf.TopMost = top_most ?? TopMost;
            int result = mf.ShowDialog();

            if (no_duplicate ?? NoDuplicate)
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
        static Icon get_icon(Icons icon)
        {
            switch(icon)
            {
                case Icons.Information:
                    return SystemIcons.Information;
                case Icons.Warning:
                    return SystemIcons.Warning;
                case Icons.Error:
                    return SystemIcons.Error;
                case Icons.Question:
                    return SystemIcons.Question;
                case Icons.Exclamation:
                    return SystemIcons.Exclamation;
                default: throw new Exception("No option: " + icon);
            }
        }
    }
}

