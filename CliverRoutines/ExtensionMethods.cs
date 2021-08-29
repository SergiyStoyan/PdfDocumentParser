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
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Replacement for BeginInvoke() which is not supported in .NET5
        /// </summary>
        /// <param name="delegate"></param>
        /// <param name="ps"></param>
        public static void BeginInvoke(this Delegate @delegate, params object[] ps)
        {
            ThreadRoutines.Start(() => { @delegate.DynamicInvoke(ps); });
        }
    }
}