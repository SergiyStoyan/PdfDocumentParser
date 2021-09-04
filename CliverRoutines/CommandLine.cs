//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cliver
{
    //public interface ICommandLineParameter
    //{
    //    string Name { get; }
    //    string Value { get; }
    //    //string EscapedValue { get; }
    //}

    //public class EnumCommandLineParameter : ICommandLineParameter
    //{
    //    protected EnumCommandLineParameter(string name)
    //    {
    //        Name = name;
    //    }
    //    public string Name { get; }
    //    public string Value { get; protected set; }
    //    //public string EscapedValue { get; internal set; }
    //}

    //public class ValiableCommandLineParameter : ICommandLineParameter
    //{
    //    public ValiableCommandLineParameter(string name, string value = null)
    //    {
    //        Name = name;
    //        Value = value;
    //    }
    //    public string Name { get; }
    //    public string Value { get; set; }
    //    //public string EscapedValue { get; internal set; }
    //}

    public class CommandLine
    {
        //static public bool IsParameterSet(ICommandLineParameter parameter)
        //{
        //    return Regex.IsMatch(Environment.CommandLine, @"\s" + Regex.Escape(parameter.Name) + @"([^\w]|$)", RegexOptions.IgnoreCase);
        //}

        static public bool IsParameterSet(string parameter, string commandLine = null)
        {
            if (commandLine == null)
                commandLine = Environment.CommandLine;
            return Regex.IsMatch(commandLine, @"\s" + Regex.Escape(parameter) + @"([^\w]|$)", RegexOptions.IgnoreCase);
        }

        public static string ParseParameter(string parameter, string defaultValue = "__EXCEPTION__", string commandLine = null)
        {
            if (commandLine == null)
                commandLine = Environment.CommandLine;
            Match m = Regex.Match(commandLine, @"(?:^|\s|&)" + Regex.Escape(parameter) + @"\s*=\s*(.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!m.Success)
            {
                if (defaultValue != "__EXCEPTION__")
                    return defaultValue;
                throw new Exception("Parameter '" + parameter + "' was not found in the command line: \r\n" + Environment.CommandLine);
            }
            string value = m.Groups[1].Value.Trim();

            //!!!ATTENTION: generally such unescaping does not work on Windows
            m = Regex.Match(value, @"^""(.*?)[^\\]"".*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                value = m.Groups[1].Value.Replace(@"\""", @"""").Replace(@"\\", @"\");
                return value.Trim();
            }
            return Regex.Replace(value, @"(?<=.*?)\s.*$", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        //public static T Parse<T>(string commandLine = null) where T: CommandLine, new()
        //{
        //    if (commandLine == null)
        //        commandLine = Environment.CommandLine;
        //    T cl = new T();
        //    foreach (FieldInfo fi in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.FieldType.IsSubclassOf(typeof(ICommandLineParameter))))
        //    {
        //        if(fi.FieldType.IsSubclassOf(typeof(ValiableCommandLineParameter)))

        //    }

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">values are expected to be unescaped</param>
        /// <returns></returns>
        //public static string GetCommandLine(params ICommandLineParameter[] parameters)
        //{
        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        ICommandLineParameter p = parameters[i];
        //        if (p.Value.Contains("="))
        //            throw new Exception("The value of command line parameter '" + p.Name + "' contains '=': " + p.Value);
        //        string v = Regex.Escape(p.Value.Trim());
        //        if (v.Contains("\""))
        //            p.EscapedValue = "\"" + p.EscapedValue + "\"";
        //    }
        //    return parameters.Aggregate(new StringBuilder(), (s, p) => s.Append(p.Name + (p.EscapedValue == null ? null : "=" + p.EscapedValue)).Append(" ")).ToString();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">values are expected to be unescaped</param>
        /// <returns></returns>
        public static string ToString(params (string name, string value)[] parameters)
        {
            if (parameters.Length < 1)
                return Regex.Replace(Environment.CommandLine, @"(\"".*?\""\s*|.*?\s*)$", "");

            List<(string name, string value)> ps = new List<(string name, string value)>();
            foreach ((string name, string value) p0 in parameters)
            {
                (string name, string value) p = p0;
                if (p.value.Contains("="))
                    throw new Exception("The value of command line parameter '" + p.name + "' contains '=': " + p.value);
                if (Regex.IsMatch(p.value, @"\""|\s"))
                {
                    //!!!ATTENTION: generally such escaping does not work on Windows
                    p.value = p.value.Replace(@"\", @"\\").Replace(@"""", @"\""");
                    p.value = "\"" + p.value + "\"";
                }
                ps.Add(p);
            }
            return ps.Aggregate(new StringBuilder(), (s, p) => s.Append(p.name + (p.value == null ? null : "=" + p.value)).Append(" ")).ToString();
        }
        //static PlatformID platform = Environment.OSVersion.Platform;

        //public static string ToString()
        //{
        //    List<(string Name, string Value)> ps = GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.FieldType.IsSubclassOf(typeof(ICommandLineParameter))).Select(a => (ICommandLineParameter)a.GetValue(this)).Select(a => (a.Name, a.Value)).ToList();

        //    for (int i = 0; i < ps.Count; i++)
        //    {
        //        (string Name, string Value) p = ps[i];
        //        if (p.Value.Contains("="))
        //            throw new Exception("The value of command line parameter '" + p.Name + "' contains '=': " + p.Value);
        //        p.Value = Regex.Escape(p.Value.Trim());
        //        if (p.Value.Contains("\""))
        //            p.Value = "\"" + p.Value + "\"";
        //    }
        //    return ps.Aggregate(new StringBuilder(), (s, p) => s.Append(p.Name + (p.Value == null ? null : "=" + p.Value)).Append(" ")).ToString();
        //}
    }
}