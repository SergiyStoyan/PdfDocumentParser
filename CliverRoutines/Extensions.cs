//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
        //public static object Convert(this object value, Type type)
        //{
        //    if (value == null)
        //        return type.GetDefault();
        //    type = Nullable.GetUnderlyingType(type) ?? type;
        //    return System.Convert.ChangeType(value, type);
        //}

        //public static T Convert<T>(this object value)
        //{
        //    if (value == null)
        //        return default(T);
        //    Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        //    return (T)System.Convert.ChangeType(value, type);
        //}

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

        public static string ToStringByJson(this object o, bool indented = true, bool polymorphic = false, bool ignoreNullValues = true, bool ignoreDefaultValues = false/*, bool ignoreReferenceLoop = true !!!can go to the endless cycle*/)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o,
                    indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                   new Newtonsoft.Json.JsonSerializerSettings
                   {
                       TypeNameHandling = polymorphic ? Newtonsoft.Json.TypeNameHandling.Auto : Newtonsoft.Json.TypeNameHandling.None,
                       NullValueHandling = ignoreNullValues ? Newtonsoft.Json.NullValueHandling.Ignore : Newtonsoft.Json.NullValueHandling.Include,
                       DefaultValueHandling = ignoreDefaultValues ? Newtonsoft.Json.DefaultValueHandling.Ignore : Newtonsoft.Json.DefaultValueHandling.Include,
                       //ReferenceLoopHandling = ignoreReferenceLoop ? Newtonsoft.Json.ReferenceLoopHandling.Serialize : Newtonsoft.Json.ReferenceLoopHandling.Error
                   }
                   );
        }

        public static string ToStringByJson(this object o, Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings)
        {
            return Serialization.Json.Serialize(o, jsonSerializerSettings);
        }
    }
}