//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Cliver
{
    /// <summary>
    /// Writting log methods for the main session
    /// </summary>
    public static partial class Log
    {
        public static void Error(Exception e)
        {
            Main.Error(e);
        }
        public static void Error2(Exception e)
        {
            Main.Error2(e);
        }

        public static void Error(string message, Exception e)
        {
            Main.Error(message, e);
        }

        static public void Error(string message)
        {
            Main.Error(message);
        }

        static public void Error2(string message)
        {
            Main.Error2(message);
        }

        static public void Trace(object message = null)
        {
            Main.Trace(message);
        }

        static public void Exit(string message)
        {
            Main.Error(message);
        }
        static public void Exit2(string message)
        {
            Main.Error2(message);
        }

        static public void Exit(Exception e)
        {
            Main.Exit(e);
        }

        static public void Warning(string message)
        {
            Main.Warning(message);
        }

        static public void Warning2(string message)
        {
            Main.Warning2(message);
        }

        static public void Warning(string message, Exception e)
        {
            Main.Warning(message, e);
        }

        static public void Warning(Exception e)
        {
            Main.Warning(e);
        }

        static public void Warning2(Exception e)
        {
            Main.Warning2(e);
        }

        static public void Inform0(string message)
        {
            Main.Inform0(message);
        }

        static public void Inform(string message)
        {
            Main.Inform(message);
        }

        static public void Write(MessageType messageType, string message, string details = null)
        {
            Main.Write(messageType, message, details);
        }

        static public void Write0(string message)
        {
            Main.Write0(message);
        }

        static public void Write(string message)
        {
            Main.Write(message);
        }
    }
}