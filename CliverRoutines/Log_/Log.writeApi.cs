//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
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
        /// <summary>
        /// Write exception with details.
        /// </summary>
        /// <param name="e"></param>
        public static void Error(Exception e)
        {
            Head.Error(e);
        }

        /// <summary>
        /// Write exception without details.
        /// </summary>
        /// <param name="e"></param>
        public static void Error2(Exception e)
        {
            Head.Error2(e);
        }

        /// <summary>
        /// Write error with trace details.
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Head.Error(message);
        }

        /// <summary>
        /// Write error with exception and details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public static void Error(string message, Exception e)
        {
            Head.Error(message, e);
        }

        /// <summary>
        /// Write error without trace details.
        /// </summary>
        /// <param name="message"></param>
        static public void Error2(string message)
        {
            Head.Error2(message);
        }

        /// <summary>
        /// Write error with exception and without trace details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        static public void Error2(string message, Exception e)
        {
            Head.Error2(message, e);
        }

        /// <summary>
        /// Write message with the stack information.
        /// </summary>
        /// <param name="message"></param>
        static public void Trace(object message = null)
        {
            Head.Trace(message);
        }

        /// <summary>
        /// Write error with details and terminate the process.
        /// </summary>
        /// <param name="message"></param>
        static public void Exit(string message)
        {
            Head.Exit(message);
        }

        /// <summary>
        /// Write error without details and terminate the process.
        /// </summary>
        /// <param name="message"></param>
        static public void Exit2(string message)
        {
            Head.Exit2(message);
        }

        /// <summary>
        /// Write exception with details and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        static public void Exit(Exception e)
        {
            Head.Exit(e);
        }

        /// <summary>
        /// Write warning with stack details.
        /// </summary>
        /// <param name="message"></param>
        static public void Warning(string message)
        {
            Head.Warning(message);
        }

        /// <summary>
        /// Write warning with exception and details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        static public void Warning(string message, Exception e)
        {
            Head.Warning(message, e);
        }

        /// <summary>
        /// Write warning without details.
        /// </summary>
        /// <param name="message"></param>
        static public void Warning2(string message)
        {
            Head.Warning2(message);
        }

        /// <summary>
        /// Write warning with exception and without details.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        static public void Warning2(string message, Exception e)
        {
            Head.Warning2(message, e);
        }

        /// <summary>
        /// Write exception with details.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning(Exception e)
        {
            Head.Warning(e);
        }

        /// <summary>
        /// Write exception without details.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning2(Exception e)
        {
            Head.Warning2(e);
        }

        /// <summary>
        /// Write message with details.
        /// </summary>
        /// <param name="message"></param>
        static public void Inform0(string message)
        {
            Head.Inform0(message);
        }

        /// <summary>
        /// Write message without details.
        /// </summary>
        /// <param name="message"></param>
        static public void Inform(string message)
        {
            Head.Inform(message);
        }

        /// <summary>
        /// Write debug message with details.
        /// </summary>
        /// <param name="message"></param>
        static public void Debug0(string message)
        {
            Head.Debug0(message);
        }

        /// <summary>
        /// Write debug message without details.
        /// </summary>
        /// <param name="message"></param>
        static public void Debug(string message)
        {
            Head.Debug(message);
        }

        /// <summary>
        /// Write message without type and with details.
        /// </summary>
        /// <param name="message"></param>
        static public void Write0(string message)
        {
            Head.Write0(message);
        }

        /// <summary>
        /// Write message without type and without details.
        /// </summary>
        /// <param name="message"></param>
        static public void Write(string message)
        {
            Head.Write(message);
        }

        /// <summary>
        /// General writting log method.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <param name="details"></param>
        static public void Write(MessageType messageType, string message, string details = null)
        {
            Head.Write(messageType, message, details);
        }
    }
}