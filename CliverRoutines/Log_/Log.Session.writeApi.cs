//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
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
#if THREAD_LOG_IS_DEFAULT
                Thread.Error(e);
#else
                Default.Error(e);
#endif
            }

            public void Error(string message, Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Error(message, e);
#else
                Default.Error(message, e);
#endif
            }

            public void Error(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Error(message);
#else
                Default.Error(message);
#endif
            }

            public void Trace(object message = null)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Trace(message);
#else
                Default.Trace(message);
#endif
            }

            public void Exit(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Error(message);
#else
                Default.Error(message);
#endif
            }

            public void Exit(Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Exit(e);
#else
                Default.Exit(e);
#endif
            }

            public void Warning(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Warning(message);
#else
                Default.Warning(message);
#endif
            }

            public void Warning(Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Warning(e);
#else
                Default.Warning(e);
#endif
            }

            public void Inform(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Inform(message);
#else
                Default.Inform(message);
#endif
            }

            public void Write(MessageType messageType, string message, string details = null)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Write(messageType, message, details);
#else
                Default.Write(messageType, message, details);
#endif
            }

            public void Write(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Write(MessageType.LOG, message);
#else
                Default.Write(MessageType.LOG, message);
#endif
            }

            public void Error2(Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Error2(e);
#else
                Default.Error2(e);
#endif
            }

            public void Error2(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Error2(message);
#else
                Default.Error2(message);
#endif
            }

            public void Exit2(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Exit2(message);
#else
                Default.Exit2(message);
#endif
            }

            public void Warning(string message, Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Warning(message, e);
#else
                Default.Warning(message, e);
#endif
            }

            public void Warning2(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Warning2(message);
#else
                Default.Warning2(message);
#endif
            }

            public void Warning2(Exception e)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Warning2(e);
#else
                Default.Warning2(e);
#endif
            }

            public void Inform0(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Inform0(message);
#else
                Default.Inform0(message);
#endif
            }

            public void Write0(string message)
            {
#if THREAD_LOG_IS_DEFAULT
                Thread.Write0(message);
#else
                Default.Write0(message);
#endif
            }
        }
    }
}