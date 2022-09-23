//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Cliver
{
    public class FieldPreparation
    {
        public class IgnoredField : Attribute
        {
        }

        public static string Normalize(string value)
        {
            if (value == null)
                return "";
            value = ReplaceNonPrintableChars(value);
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = value.Trim();
            return value;
        }

        public class Html
        {
            public static string Normalize(string value)
            {
                if (value == null)
                    return "";
                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = ReplaceNonPrintableChars(value);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces
                value = value.Trim();
                return value;
            }

            public static string GetCsvField(string value, bool normalize = true)
            {
                if (value == null)
                    return "";
                if (normalize)
                    value = Normalize(value);
                value = Regex.Replace(value, "\"", "\"\"", RegexOptions.Compiled | RegexOptions.Singleline);
                if (Regex.IsMatch(value, Csv.FieldSeparator, RegexOptions.Compiled | RegexOptions.Singleline))
                    value = "\"" + value + "\"";
                return value;
            }

            public static string GetDbField(string value)
            {
                if (value == null)
                    return "";

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = ReplaceNonPrintableChars(value);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                return value;
            }

            public static string GetCsvLine(dynamic o, bool normalize = true)
            {
                List<string> ss = new List<string>();
                foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
                {
                    if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
                        continue;
                    string s;
                    object p = pi.GetValue(o);
                    if (pi.PropertyType == typeof(string))
                        s = (string)p;
                    else if (p != null)
                        s = p.ToString();
                    else
                        s = null;
                    ss.Add(GetCsvField(s, normalize));
                }
                return string.Join(Csv.FieldSeparator, ss);
            }

            public static string GetCsvLine(IEnumerable<object> values, bool normalize = true)
            {
                List<string> ss = new List<string>();
                foreach (object v in values)
                {
                    string s;
                    if (v is string)
                        s = (string)v;
                    else if (v != null)
                        s = v.ToString();
                    else
                        s = null;
                    ss.Add(GetCsvField(s, normalize));
                }
                return string.Join(Csv.FieldSeparator, ss);
            }

            public static Dictionary<string, object> GetDbObject(dynamic o)
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
                {
                    if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
                        continue;
                    object p = pi.GetValue(o);
                    if (pi.PropertyType == typeof(string))
                        p = GetDbField((string)p);
                    d[pi.Name] = p;
                }
                return d;
            }

            public static string GetDbCellKeepingFormat(string value)
            {
                if (value == null)
                    return "";

                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, @"<(p|br|\/tr)(\s[^>]*>|>)", "\r\n", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = ReplaceNonPrintableChars(value);
                value = Regex.Replace(value, @"[ ]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                return (HttpUtility.HtmlDecode(value)).Trim();
            }
        }

        public class Csv
        {
            public const string FieldSeparator = ",";

            public static string GetField(string value, bool normalize = true)
            {
                return getField(value, FieldSeparator, normalize);
            }

            public static string GetHeaderLine(Type t, bool normalize = true)
            {
                return getHeaderLine(t, FieldSeparator, normalize);
            }

            public static string GetLine(dynamic o, bool normalize = true)
            {
                return getLine(o, FieldSeparator, normalize);
            }

            public static string GetHeaderLine(IEnumerable<string> headers, bool normalize = true)
            {
                return getHeaderLine(headers, FieldSeparator, normalize);
            }

            public static string GetLine(IEnumerable<object> values, bool normalize = true)
            {
                return getLine(values, FieldSeparator, normalize);
            }
        }

        public class Tsv
        {
            public const string FieldSeparator = "\t";

            public static string GetField(string value, bool normalize = true)
            {
                return getField(value, FieldSeparator, normalize);
            }

            public static string GetHeaderLine(Type t, bool normalize = true)
            {
                return getHeaderLine(t, FieldSeparator, normalize);
            }

            public static string GetLine(dynamic o, bool normalize = true)
            {
                return getLine(o, FieldSeparator, normalize);
            }

            public static string GetHeaderLine(IEnumerable<string> headers, bool normalize = true)
            {
                return getHeaderLine(headers, FieldSeparator, normalize);
            }

            public static string GetLine(IEnumerable<object> values, bool normalize = true)
            {
                return getLine(values, FieldSeparator, normalize);
            }
        }

        static string getField(string value, string fieldSeparator, bool normalize = true)
        {
            if (value == null)
                return "";
            if (normalize)
                value = Normalize(value);
            value = Regex.Replace(value, "\"", "\"\"", RegexOptions.Compiled | RegexOptions.Singleline);
            if (Regex.IsMatch(value, fieldSeparator, RegexOptions.Compiled | RegexOptions.Singleline))
                value = "\"" + value + "\"";
            return value;
        }

        static string getHeaderLine(Type t, string fieldSeparator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
                ss.Add(getField(pi.Name, fieldSeparator, normalize));
            return string.Join(fieldSeparator, ss);
        }

        static string getLine(dynamic o, string fieldSeparator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (System.Reflection.FieldInfo pi in o.GetType().GetFields())
            {
                if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
                    continue;
                object p = pi.GetValue(o);
                ss.Add(getField(p?.ToString(), fieldSeparator, normalize));
            }
            return string.Join(fieldSeparator, ss);
        }

        static string getHeaderLine(IEnumerable<string> headers, string fieldSeparator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (string h in headers)
                ss.Add(getField(h, fieldSeparator, normalize));
            return string.Join(fieldSeparator, ss);
        }

        static string getLine(IEnumerable<object> values, string fieldSeparator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (object v in values)
                ss.Add(getField(v?.ToString(), fieldSeparator, normalize));
            return string.Join(fieldSeparator, ss);
        }

        public class Db
        {
            public static string GetField(string value, string default_value = "")
            {
                if (value == null)
                    return default_value;

                value = ReplaceNonPrintableChars(value);
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                if (value == "")
                    return default_value;
                return value;
            }

            public static Dictionary<string, object> GetObject(dynamic o, string default_value = "")
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
                {
                    if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
                        continue;
                    object p = pi.GetValue(o);
                    if (pi.PropertyType == typeof(string))
                        p = GetField((string)p, default_value);
                    d[pi.Name] = p;
                }
                return d;
            }
        }

        public static string ReplaceNonPrintableChars(string s, string substitution = " ")
        {
            return NonPrintableCharsRegex.Replace(s, substitution);
        }
        public static Regex NonPrintableCharsRegex = new Regex(@"\p{C}+", RegexOptions.Singleline);

        public static string Trim(string s, int length, string ending = "...")
        {
            if (s.Length <= length)
                return s;
            return s.Substring(0, length) + ending;
        }
    }
}