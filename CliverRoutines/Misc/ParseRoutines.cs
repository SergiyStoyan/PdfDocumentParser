//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Text.RegularExpressions;

namespace Cliver
{
    static public class ParseRoutines
    {
        public static T ParseEnum<T>(this string text) where T : struct
        {
            string t = Regex.Replace(text, @".+\.", "");
            if (Enum.TryParse<T>(t, out T r))
                return r;
            throw new Exception("Could not parse: " + text);
        }
    }
}

