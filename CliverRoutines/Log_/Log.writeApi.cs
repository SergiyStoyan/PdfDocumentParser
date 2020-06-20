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
    /// Writting log methods for the head session
    /// </summary>
    public static partial class Log
    {
        public static void Error(Exception e)
        {
            Head.Error(e);
        }
        public static void Error2(Exception e)
        {
            Head.Error2(e);
        }

        public static void Error(string message, Exception e)
        {
            Head.Error(message, e);
        }

        static public void Error(string message)
        {
            Head.Error(message);
        }

        static public void Error2(string message)
        {
            Head.Error2(message);
        }

        static public void Trace(object message = null)
        {
            Head.Trace(message);
        }

        static public void Exit(string message)
        {
            Head.Error(message);
        }
        static public void Exit2(string message)
        {
            Head.Error2(message);
        }

        static public void Exit(Exception e)
        {
            Head.Exit(e);
        }

        static public void Warning(string message)
        {
            Head.Warning(message);
        }

        static public void Warning2(string message)
        {
            Head.Warning2(message);
        }

        static public void Warning(string message, Exception e)
        {
            Head.Warning(message, e);
        }

        static public void Warning(Exception e)
        {
            Head.Warning(e);
        }

        static public void Warning2(Exception e)
        {
            Head.Warning2(e);
        }

        static public void Inform0(string message)
        {
            Head.Inform0(message);
        }

        static public void Inform(string message)
        {
            Head.Inform(message);
        }

        static public void Write(MessageType messageType, string message, string details = null)
        {
            Head.Write(messageType, message, details);
        }

        static public void Write0(string message)
        {
            Head.Write0(message);
        }

        static public void Write(string message)
        {
            Head.Write(message);
        }
    }
}