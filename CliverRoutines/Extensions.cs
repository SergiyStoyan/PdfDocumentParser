//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    public static class Extensions
    {
        /// <summary>
        /// Copy the list of ranges into an output array. A range can be reversed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ps"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static T[] CopyRanges<T>(this T[] ps, params (int I1, int I2)[] ranges)
        {
            T[] ps2 = new T[ranges.Sum(a => Math.Abs(a.I1 - a.I2) + 1)];
            int i2 = -1;
            foreach ((int I1, int I2) r in ranges)
            {
                int i = r.I1;
                for (i2++; i2 < ps2.Length; i2++)
                {
                    ps2[i2] = ps[i];
                    if (r.I1 <= r.I2)
                    {
                        if (++i > r.I2)
                            break;
                    }
                    else
                    {
                        if (--i < r.I2)
                            break;
                    }
                }
            }
            return ps2;
        }

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