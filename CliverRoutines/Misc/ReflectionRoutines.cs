//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace Cliver
{
    public static class ReflectionRoutines
    {
        static public bool IsSubclassOfRawGeneric(this Type type, Type generic_type)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic_type == cur)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        public static Dictionary<string, FieldInfo> GetFields(this object o, BindingFlags bindingFlags = BindingFlags.Public)
        {
            Dictionary<string, FieldInfo> ns2fi = new Dictionary<string, FieldInfo>();
            foreach (FieldInfo fi in o.GetType().GetFields(bindingFlags))
                ns2fi[fi.Name] = fi;
            return ns2fi;
        }

        //public static HandyDictionary<string, object> GetFields(this object o, BindingFlags bindingFlags = BindingFlags.Public)
        //{
        //    Dictionary<string, FieldInfo> ns2fi = new Dictionary<string, FieldInfo>();
        //    foreach (FieldInfo fi in o.GetType().GetFields(bindingFlags))
        //        ns2fi[fi.Name] = fi;

        //    return new HandyDictionary<string, object>(
        //        (string name) =>
        //        {
        //            return ns2fi[name].GetValue(o);
        //        });
        //} public Dictionary<string, string> nameOfAlreadyAcessed = new Dictionary<string, string>();

        public static string GetNameOfVariablePassedInAsParameter(string parameterName, int frame = 1)//!!!not debugged! It will not work for Release
        {
            MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
            int parameterNumber = 0;
            foreach (ParameterInfo pi in mb.GetParameters())
            {
                if (pi.Name == parameterName)
                    break;
                parameterNumber++;
            }
            StackFrame stackFrame = new StackTrace(true).GetFrame(frame);
            string fileName = stackFrame.GetFileName();
            int lineNumber = stackFrame.GetFileLineNumber();
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            for (int i = 0; i < lineNumber - 1; i++)
                file.ReadLine();
            for (string g = file.ReadLine(); !file.EndOfStream; g += file.ReadLine())
            {
                Match m = Regex.Match(g, @"[^\w\d\_\-]" + mb.Name + @"\s*\(?:([^\,\)]+\,){" + parameterNumber + @"}(?'VariableName'[^\,\)])+(?:\,\))", RegexOptions.Singleline);
                if (m.Success)
                    return m.Groups["VariableName"].Value;
            }
            return null;
        }
    }
}
