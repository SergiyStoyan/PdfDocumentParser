//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
        public static object GetDefault(this Type type)
        {
            return typeof(ReflectionRoutines).GetRuntimeMethod(nameof(getDefaultGeneric), new Type[] { }).MakeGenericMethod(type).Invoke(null, null);
        }
        static T getDefaultGeneric<T>()
        {
            return default(T);
        }

        static public bool IsSubclassOfRawGeneric(Type type, Type genericType)
        {
            for (; type != null && type != typeof(object); type = type.BaseType)
                if (genericType == (type.IsGenericType ? type.GetGenericTypeDefinition() : type))
                    return true;
            return false;
        }

        public static Dictionary<string, FieldInfo> GetFieldInfos(object o, BindingFlags bindingFlags)
        {
            Dictionary<string, FieldInfo> ns2fi = new Dictionary<string, FieldInfo>();
            foreach (FieldInfo fi in o.GetType().GetFields(bindingFlags))
                ns2fi[fi.Name] = fi;
            return ns2fi;
        }

        public static Dictionary<string, object> GetFieldValues(object o, BindingFlags bindingFlags)
        {
            Dictionary<string, object> ns2fi = new Dictionary<string, object>();
            foreach (FieldInfo fi in o.GetType().GetFields(bindingFlags))
                ns2fi[fi.Name] = fi.GetValue(o);
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

        /// <summary>
        /// !!!not debugged! It will not work for Release
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static string GetNameOfVariablePassedInAsParameter(string parameterName, int frame = 1)
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

        //public static MethodInfo GetCallingStackMethodInfo(Regex namespaceRegex)
        //{
        //    Type myType = typeof(MyClass);
        //    var n = myType.Namespace;
        //    StackTrace st = new StackTrace(true);
        //    int frameI = 1;
        //    for (; ; frameI++)
        //    {
        //        StackFrame sf = st.GetFrame(frameI);
        //        if (sf == null)
        //            break;
        //        MethodBase mb = sf.GetMethod();
        //        Type dt = mb.DeclaringType;
        //        if (dt != typeof(Log) && dt != typeof(Log.Writer) && dt != typeof(Log.Session) && TypesExcludedFromStack?.Find(x => x == dt) == null)
        //            break;
        //    }
        //    List<string> frameSs = new List<string>();
        //    if (frameCount < 0)
        //        frameCount = 1000;
        //    frameI += startFrame;
        //    int endFrameI = frameI + frameCount - 1;
        //    for (; frameI <= endFrameI; frameI++)
        //    {
        //        StackFrame sf = st.GetFrame(frameI);
        //        if (sf == null || endOnEmptyFile && frameSs.Count > 0 && string.IsNullOrEmpty(sf.GetFileName()))//it seems to be passing out of the application
        //            break;
        //        MethodBase mb = sf.GetMethod();
        //        Type dt = mb.DeclaringType;
        //        frameSs.Add("method: " + dt?.ToString() + "::" + mb?.Name + " \r\nfile: " + sf.GetFileName() + " \r\nline: " + sf.GetFileLineNumber());
        //    }
        //    return string.Join("\r\n<=", frameSs);
        //}
        //static List<Type> TypesExcludedFromStack = null;
    }
}
