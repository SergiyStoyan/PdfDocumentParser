//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cliver
{
    /// <summary>
    /// Used to intercept informative messages in checking scopes.
    /// </summary>
    public class MessageException : System.Exception
    {
        public MessageException(string message) : base(message) { }
    }
}

