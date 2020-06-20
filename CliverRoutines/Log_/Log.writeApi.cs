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
            mainSession.Error(e);
        }
        public static void Error2(Exception e)
        {
            mainSession.Error2(e);
        }

        public static void Error(string message, Exception e)
        {
            mainSession.Error(message, e);
        }

        static public void Error(string message)
        {
            mainSession.Error(message);
        }

        static public void Error2(string message)
        {
            mainSession.Error2(message);
        }

        static public void Trace(object message = null)
        {
            mainSession.Trace(message);
        }

        static public void Exit(string message)
        {
            mainSession.Error(message);
        }
        static public void Exit2(string message)
        {
            mainSession.Error2(message);
        }

        static public void Exit(Exception e)
        {
            mainSession.Exit(e);
        }

        static public void Warning(string message)
        {
            mainSession.Warning(message);
        }

        static public void Warning2(string message)
        {
            mainSession.Warning2(message);
        }

        static public void Warning(string message, Exception e)
        {
            mainSession.Warning(message, e);
        }

        static public void Warning(Exception e)
        {
            mainSession.Warning(e);
        }

        static public void Warning2(Exception e)
        {
            mainSession.Warning2(e);
        }

        static public void Inform0(string message)
        {
            mainSession.Inform0(message);
        }

        static public void Inform(string message)
        {
            mainSession.Inform(message);
        }

        static public void Write(MessageType messageType, string message, string details = null)
        {
            mainSession.Write(messageType, message, details);
        }

        static public void Write0(string message)
        {
            mainSession.Write0(message);
        }

        static public void Write(string message)
        {
            mainSession.Write(message);
        }
    }
}