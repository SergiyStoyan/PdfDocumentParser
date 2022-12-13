//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cliver;

namespace Cliver
{
    /// <summary>
    /// Trace info for such Exception is not logged. 
    /// It is intended for foreseen errors.
    /// </summary>
    public class Exception2 : Exception
    {
        public Exception2(string message)
            : base(message)
        {
        }

        public Exception2(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Informative Exception with controllable logging features.
    /// </summary>
    public class MessageException : Exception
    {
        public Log.MessageType MessageType;
        public bool PrintDetails;

        public MessageException(Log.MessageType messageType, bool printDetails, string message) : base(message)
        {
            MessageType = messageType;
            PrintDetails = printDetails;
        }
    }

    /// <summary>
    /// Used to intercept informative messages in checking scopes.
    /// </summary>
    public class InformException : MessageException
    {
        public InformException(string message) : base(Log.MessageType.INFORM, false, message) { }
    }

    /// <summary>
    /// Used to intercept informative messages in checking scopes. Logged with trace details.
    /// </summary>
    public class InformException0 : MessageException
    {
        public InformException0(string message) : base(Log.MessageType.INFORM, true, message) { }
    }

    /// <summary>
    /// Used to intercept warning messages in checking scopes. Logged with trace details.
    /// </summary>
    public class WarningException : MessageException
    {
        public WarningException(string message) : base(Log.MessageType.WARNING, true, message) { }
    }

    /// <summary>
    /// Used to intercept informative messages in checking scopes. Logged without trace details.
    /// </summary>
    public class WarningException2 : MessageException
    {
        public WarningException2(string message) : base(Log.MessageType.WARNING, false, message) { }
    }
}

