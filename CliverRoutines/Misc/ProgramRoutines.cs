//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;


namespace Cliver
{
    public static class ProgramRoutines
    {
        public static Dictionary<string, string> GetCommandLineParameters(char equal = '=')
        {
            Dictionary<string, string> clps = new Dictionary<string, string>();
            if (equal == ' ')
            {
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 2; i < args.Length; i += 2)
                    clps[args[i - 1]] = args[i];
            }
            else
            {
                string args = Regex.Replace(Environment.CommandLine, @"^(\"".*?\""|\'.*?\'|.*?)\s+", "");
                for (Match m = Regex.Match(args, @"(?'Key'\w+?)" + Regex.Escape(equal.ToString()) + @"(?'Value'\"".*?\""|\'.*?\'|.*?)(\s|$)"); m.Success; m = m.NextMatch())
                {
                    string v = m.Groups["Value"].Value;
                    if (v.Length > 1)
                        if (v[0] == '"')
                        {
                            v = v.Trim('"');
                            v = Regex.Replace(v, @"\\""", "\"");
                        }
                        else if (v[0] == '\'')
                        {
                            v = v.Trim('\'');
                            v = Regex.Replace(v, @"\\\'", "'");
                        }
                    clps[m.Groups["Key"].Value] = v;
                }
            }
            return clps;
        }

        public class CommandLineParameters
        {
            public static readonly CommandLineParameters NOT_SET = new CommandLineParameters(null);

            public override string ToString()
            {
                return Value;
            }

            protected CommandLineParameters(string value)
            {
                this.Value = value;
            }

            public string Value { get; private set; }
        }

        static public bool IsParameterSet<T>(T parameter) where T : CommandLineParameters
        {
            return Regex.IsMatch(Environment.CommandLine, @"\s" + parameter.Value + @"([^\w]|$)", RegexOptions.IgnoreCase);
        }

        public static bool IsWebContext
        {
            get
            {
                return HttpRuntime.AppDomainAppId != null;
            }
        }

        static public string GetAppPath()
        {
            string p;
            if (ProgramRoutines.IsWebContext)
                p = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).CodeBase;
            else
                p = Regex.Replace(System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase, @"file:///", "");
            return Path.GetFullPath(p);
        }

        static public string GetAppTempDirectory()
        {
            return Path.GetTempPath() + "\\" + GetAppName();
        }

        static public string GetAppDirectory()
        {
            return PathRoutines.GetDirFromPath(GetAppPath()).TrimEnd('\\', '/');
        }

        static public string GetAppName()
        {
            string an = Application.ProductName;
            if (!string.IsNullOrWhiteSpace(an))
                return an;
            if (IsWebContext)
                return System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).Name;
            else
                return System.Reflection.Assembly.GetEntryAssembly().GetName(false).Name;
        }
    }
}