//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
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

        public class Html
        {
            public static string Normalize(string value)
            {
                if (value == null)
                    return "";
                value = Regex.Replace(value, "<!--.*?-->|<script .*?</script>", "", RegexOptions.Compiled | RegexOptions.Singleline);
                value = Regex.Replace(value, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                value = HttpUtility.HtmlDecode(value);
                value = RemoveNonPrintablesRegex.Replace(value, " ");
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces
                value = value.Trim();
                return value;
            }

            public static string GetCsvField(string value, FieldSeparator separator, bool normalize = true)
            {
                if (value == null)
                    return "";
                if (normalize)
                    value = Normalize(value);
                value = Regex.Replace(value, "\"", "\"\"", RegexOptions.Compiled | RegexOptions.Singleline);
                if (Regex.IsMatch(value, separator.Value, RegexOptions.Compiled | RegexOptions.Singleline))
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
                value = RemoveNonPrintablesRegex.Replace(value, " ");
                value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                return value;
            }

            public static string GetCsvLine(dynamic o, FieldSeparator separator, bool normalize = true)
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
                    ss.Add(GetCsvField(s, separator, normalize));
                }
                return string.Join(separator.Value, ss);
            }

            public static string GetCsvLine(IEnumerable<object> values, FieldSeparator separator, bool normalize = true)
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
                    ss.Add(GetCsvField(s, separator, normalize));
                }
                return string.Join(separator.Value, ss);
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
                value = RemoveNonPrintablesRegex.Replace(value, " ");
                value = Regex.Replace(value, @"[ ]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
                value = value.Trim();
                return (HttpUtility.HtmlDecode(value)).Trim();
            }
        }

        public static string Normalize(string value)
        {
            if (value == null)
                return "";
            value = RemoveNonPrintablesRegex.Replace(value, " ");
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            value = value.Trim();
            return value;
        }

        public static string GetCsvField(string value, FieldSeparator separator, bool normalize = true)
        {
            if (value == null)
                return "";
            if (normalize)
                value = Normalize(value);
            value = Regex.Replace(value, "\"", "\"\"", RegexOptions.Compiled | RegexOptions.Singleline);
            if (Regex.IsMatch(value, separator.Value, RegexOptions.Compiled | RegexOptions.Singleline))
                value = "\"" + value + "\"";
            return value;
        }

        public class FieldSeparator : Cliver.Enum<string>
        {
            FieldSeparator(string value) : base(value) { }

            public readonly static FieldSeparator COMMA = new FieldSeparator(",");
            public readonly static FieldSeparator TAB = new FieldSeparator("\t");
        }

        public static string GetDbField(string value, string default_value = "")
        {
            if (value == null)
                return default_value;

            value = RemoveNonPrintablesRegex.Replace(value, " ");
            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
            value = value.Trim();
            if (value == "")
                return default_value;
            return value;
        }

        public static string GetCsvHeaderLine(Type t, FieldSeparator separator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
                ss.Add(GetCsvField(pi.Name, separator, normalize));
            return string.Join(separator.Value, ss);
        }

        public static string GetCsvLine(dynamic o, FieldSeparator separator, bool normalize = true)
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
                ss.Add(GetCsvField(s, separator, normalize));
            }
            return string.Join(separator.Value, ss);
        }

        public static string GetCsvHeaderLine(IEnumerable<string> headers, FieldSeparator separator, bool normalize = true)
        {
            List<string> ss = new List<string>();
            foreach (string h in headers)
                ss.Add(GetCsvField(h, separator, normalize));
            return string.Join(separator.Value, ss);
        }

        public static string GetCsvLine(IEnumerable<object> values, FieldSeparator separator, bool normalize = true)
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
                ss.Add(GetCsvField(s, separator, normalize));
            }
            return string.Join(separator.Value, ss);
        }

        public static Dictionary<string, object> GetDbObject(dynamic o, string default_value = "")
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (System.Reflection.PropertyInfo pi in o.GetType().GetProperties())
            {
                if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
                    continue;
                object p = pi.GetValue(o);
                if (pi.PropertyType == typeof(string))
                    p = GetDbField((string)p, default_value);
                d[pi.Name] = p;
            }
            return d;
        }

        //static Regex RemoveNonPrintablesRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled | RegexOptions.Singleline);
        public readonly static Regex RemoveNonPrintablesRegex = new Regex(@"[^\u0000-\u00b0]", RegexOptions.Compiled | RegexOptions.Singleline);
        //static Regex RemoveNonPrintablesRegex = new Regex(@"[^\x20-\x7E]", RegexOptions.Compiled | RegexOptions.Singleline);

        public static string Trim(string s, int length, string ending = "...")
        {
            if (s.Length <= length)
                return s;
            return s.Substring(0, length) + ending;
        }
    }
}