//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Cliver
{
    public partial class Log
    {
        public interface IWriteApi
        {
            /// <summary>
            /// Write exception with details.
            /// </summary>
            /// <param name="e"></param>
            void Error(Exception e);

            /// <summary>
            /// Write exception without details.
            /// </summary>
            /// <param name="e"></param>
            void Error2(Exception e);

            /// <summary>
            /// Write error with trace details.
            /// </summary>
            /// <param name="message"></param>
            void Error(string message);

            /// <summary>
            /// Write error with exception and details.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="e"></param>
            void Error(string message, Exception e);

            /// <summary>
            /// Write error without trace details.
            /// </summary>
            /// <param name="message"></param>
            void Error2(string message);

            /// <summary>
            /// Write error with exception and without trace details.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="e"></param>
            void Error2(string message, Exception e);

            /// <summary>
            /// Write object with the stack information.
            /// </summary>
            /// <param name="object_"></param>
            void Trace(object object_ = null);

            /// <summary>
            /// Write error with details and terminate the process.
            /// </summary>
            /// <param name="message"></param>
            void Exit(string message);

            /// <summary>
            /// Write error without details and terminate the process.
            /// </summary>
            /// <param name="message"></param>
            void Exit2(string message);

            /// <summary>
            /// Write exception with details and terminate the process.
            /// </summary>
            /// <param name="e"></param>
            void Exit(Exception e);

            /// <summary>
            /// Write warning with stack details.
            /// </summary>
            /// <param name="message"></param>
            void Warning(string message);

            /// <summary>
            /// Write warning with exception and details.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="e"></param>
            void Warning(string message, Exception e);

            /// <summary>
            /// Write warning without details.
            /// </summary>
            /// <param name="message"></param>
            void Warning2(string message);

            /// <summary>
            /// Write warning with exception and without details.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="e"></param>
            void Warning2(string message, Exception e);

            /// <summary>
            /// Write exception with details.
            /// </summary>
            /// <param name="e"></param>
            void Warning(Exception e);

            /// <summary>
            /// Write exception without details.
            /// </summary>
            /// <param name="e"></param>
            void Warning2(Exception e);

            /// <summary>
            /// Write message with details.
            /// </summary>
            /// <param name="message"></param>
            void Inform0(string message);

            /// <summary>
            /// Write message without details.
            /// </summary>
            /// <param name="message"></param>
            void Inform(string message);

            /// <summary>
            /// Write debug message with details.
            /// </summary>
            /// <param name="message"></param>
            void Debug0(string message);

            /// <summary>
            /// Write debug message without details.
            /// </summary>
            /// <param name="message"></param>
            void Debug(string message);

            /// <summary>
            /// Write message without type and with details.
            /// </summary>
            /// <param name="message"></param>
            void Write0(string message);

            /// <summary>
            /// Write message without type and without details.
            /// </summary>
            /// <param name="message"></param>
            void Write(string message);

            /// <summary>
            /// Base writting log method.
            /// </summary>
            /// <param name="messageType"></param>
            /// <param name="message"></param>
            /// <param name="details"></param>
            void Write(Log.MessageType messageType, string message, string details = null);
        }
    }
}