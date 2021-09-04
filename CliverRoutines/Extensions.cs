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
    public static class Extensions
    {
        public static byte GetNumberOfDigits(this long n)
        {
            byte c = 0;
            n = Math.Abs(n);
            do
            {
                n = n / 10;
                c++;
            }
            while (Math.Abs(n) >= 1);
            return c;
        }

        public static byte GetNumberOfDigits(this int n)
        {
            return GetNumberOfDigits((long)n);
        }

        /// <summary>
        /// Replacement for BeginInvoke() which is not supported in .NET5
        /// </summary>
        /// <param name="delegate"></param>
        /// <param name="ps"></param>
        public static void BeginInvoke(this Delegate @delegate, params object[] ps)
        {
            ThreadRoutines.Start(() => { @delegate.DynamicInvoke(ps); });
        }

        public static O CreateCloneByJson<O>(this O o)
        {
            return Serialization.Json.Clone<O>(o);
        }

        public static object CreateCloneByJson(this object o, Type type)
        {
            return Serialization.Json.Clone(type, o);
        }

        public static bool IsEqualByJson(this object a, object b)
        {
            return Serialization.Json.IsEqual(a, b);
        }

        public static string ToStringByJson(this object o, bool indented = true, bool polymorphic = false, bool ignoreNullValues = true, bool ignoreDefaultValues = false)
        {
            return Serialization.Json.Serialize(o, indented, polymorphic, ignoreNullValues, ignoreDefaultValues);
        }
    }
}