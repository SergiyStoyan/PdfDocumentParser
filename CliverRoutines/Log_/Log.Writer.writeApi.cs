//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Cliver
{
    public partial class Log
    {
        public partial class Writer : IWriteApi
        {
            public void Error(Exception e)
            {
                Write(MessageType.ERROR, GetExceptionMessage(e, !(e is Exception2)));
            }

            public void Error2(Exception e)
            {
                Write(MessageType.ERROR, GetExceptionMessage2(e));
            }

            public void Error(string message)
            {
                Write(MessageType.ERROR, message, GetStackString());
            }

            public void Error(string message, Exception e)
            {
                Write(MessageType.ERROR, message, GetExceptionMessage(e, !(e is Exception2)));
            }

            public void Error2(string message)
            {
                Write(MessageType.ERROR, message);
            }

            public void Error2(string message, Exception e)
            {
                Write(MessageType.ERROR, message, GetExceptionMessage2(e));
            }

            public void Trace(object message = null)
            {
                Write(MessageType.TRACE, message == null ? null : message.ToString(), GetStackString());
            }

            virtual public void Exit(string message)
            {
                Write(MessageType.EXIT, message, GetStackString());
            }

            virtual public void Exit2(string message)
            {
                Write(MessageType.EXIT, message);
            }

            virtual public void Exit(Exception e)
            {
                Write(MessageType.EXIT, GetExceptionMessage(e, !(e is Exception2)));
            }

            public void Warning(string message)
            {
                Write(MessageType.WARNING, message, GetStackString());
            }

            public void Warning(string message, Exception e)
            {
                Write(MessageType.WARNING, message, GetExceptionMessage(e, !(e is Exception2)));
            }

            public void Warning2(string message)
            {
                Write(MessageType.WARNING, message);
            }

            public void Warning2(string message, Exception e)
            {
                Write(MessageType.WARNING, message, GetExceptionMessage2(e));
            }

            public void Warning(Exception e)
            {
                Write(MessageType.WARNING, GetExceptionMessage(e, !(e is Exception2)));
            }

            public void Warning2(Exception e)
            {
                Write(MessageType.WARNING, GetExceptionMessage2(e));
            }

            public void Inform0(string message)
            {
                Write(MessageType.INFORM, message, GetStackString());
            }

            public void Inform(string message)
            {
                Write(MessageType.INFORM, message);
            }

            public void Debug0(string message)
            {
                Write(MessageType.DEBUG, message, GetStackString());
            }

            public void Debug(string message)
            {
                Write(MessageType.DEBUG, message);
            }

            public void Write0(string message)
            {
                Write(MessageType.LOG, message, GetStackString());
            }

            public void Write(string message)
            {
                Write(MessageType.LOG, message);
            }
        }
    }
}