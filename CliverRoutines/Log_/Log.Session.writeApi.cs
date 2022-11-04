//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

//#define THREAD_LOG_IS_DEFAULT

using System;
using System.Collections.Generic;
using System.IO;

namespace Cliver
{
    public static partial class Log
    {
        public partial class Session : IWriteApi
        {
            public void Error(Exception e)
            {
                Default.Error(e);
            }

            public void Error(string message, Exception e)
            {
                Default.Error(message, e);
            }

            public void Error(string message)
            {
                Default.Error(message);
            }

            public void Trace(object object_ = null)
            {
                Default.Trace(object_);
            }

            public void Exit(string message)
            {
                Default.Exit(message);
            }

            public void Exit(Exception e)
            {
                Default.Exit(e);
            }

            public void Warning(string message)
            {
                Default.Warning(message);
            }

            public void Warning(Exception e)
            {
                Default.Warning(e);
            }

            public void Inform(string message)
            {
                Default.Inform(message);
            }

            public void Write(MessageType messageType, string message, string details = null)
            {
                Default.Write(messageType, message, details);
            }

            public void Write(string message)
            {
                Default.Write(message);
            }

            public void Error2(Exception e)
            {
                Default.Error2(e);
            }

            public void Error2(string message)
            {
                Default.Error2(message);
            }

            public void Error2(string message, Exception e)
            {
                Default.Error2(message, e);
            }

            public void Exit2(string message)
            {
                Default.Exit2(message);
            }

            public void Warning(string message, Exception e)
            {
                Default.Warning(message, e);
            }

            public void Warning2(string message)
            {
                Default.Warning2(message);
            }

            public void Warning2(Exception e)
            {
                Default.Warning2(e);
            }

            public void Warning2(string message, Exception e)
            {
                Default.Warning2(message, e);
            }

            public void Inform0(string message)
            {
                Default.Inform0(message);
            }

            public void Write0(string message)
            {
                Default.Write0(message);
            }

            public void Debug(string message)
            {
                Default.Debug(message);
            }

            public void Debug0(string message)
            {
                Default.Debug0(message);
            }
        }
    }
}