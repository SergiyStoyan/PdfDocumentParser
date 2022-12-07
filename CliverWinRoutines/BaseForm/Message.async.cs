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
using System.Threading.Tasks;

namespace Cliver
{
    public static partial class Message
    {
        public static async Task InformAsync(string message, Form owner = null)
        {
            await Task.Run(() => { Message.Inform(message, owner); });
        }

        public static async Task ExclaimAsync(string message, Form owner = null)
        {
            await Task.Run(() => { Message.Exclaim(message, owner); });
        }

        public static async Task Exclaim0Async(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Exclaim0(e, owner); });
        }

        public static async Task ExclaimAsync(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Exclaim(e, owner); });
        }

        public static async Task WarningAsync(string message, Form owner = null)
        {
            await Task.Run(() => { Message.Warning(message, owner); });
        }

        public static async Task WarningAsync(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Warning(e, owner); });
        }

        public static async Task Warning2Async(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Warning2(e, owner); });
        }

        public static async Task ErrorAsync(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Error(e, owner); });
        }

        public static async Task Error2Async(Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Error2(e, owner); });
        }

        public static async Task ErrorAsync(string subtitle, Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Error(subtitle, e, owner); });
        }

        public static async Task Error2Async(string subtitle, Exception e, Form owner = null)
        {
            await Task.Run(() => { Message.Error2(subtitle, e, owner); });
        }

        public static async Task ErrorAsync(string message, Form owner = null)
        {
            await Task.Run(() => { Message.Error(message, owner); });
        }

        public static async Task<bool> YesNoAsync(string question, Form owner = null, Message.Icons icon = Message.Icons.Question, bool defaultIsYes = true)
        {
            return await Task.Run(() => { return Message.YesNo(question, owner, icon, defaultIsYes); });
        }

        public static async Task<int> ShowDialogAsync(string title, Message.Icons icon, string message, string[] buttons, int defaultButton, Form owner = null, bool? buttonAutosize = null, bool? noDuplicate = null, bool? topmost = null)
        {
            return await Task.Run(() => { return Message.ShowDialog(title, icon, message, buttons, defaultButton, owner, buttonAutosize, noDuplicate, topmost); });

        }

        public static async Task<int> ShowDialogAsync(string title, Icon icon, string message, string[] buttons, int defaultButton, Form owner, bool? buttonAutosize = null, bool? noDuplicate = null, bool? topmost = null)
        {
            return await Task.Run(() => { return Message.ShowDialog(title, icon, message, buttons, defaultButton, owner, buttonAutosize, noDuplicate, topmost); });
        }
    }
}

