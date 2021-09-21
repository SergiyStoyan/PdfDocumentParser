/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;


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

        static public string GetAppPath()
        {
            string p = Regex.Replace(System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase, @"file:///", "");
            return Path.GetFullPath(p);
        }

        static public string GetAppTempDirectory()
        {
            return Path.GetTempPath() + "\\" + GetAppName();
        }

        static public string GetAppDirectory()
        {
            return PathRoutines.GetFileDir(GetAppPath()).TrimEnd('\\', '/');
        }

        static public string GetAppName()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName(false).Name;
        }
    }
}