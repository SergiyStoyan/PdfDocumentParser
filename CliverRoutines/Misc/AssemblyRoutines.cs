﻿//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace Cliver
{
    public static class AssemblyRoutines
    {
        public static Assembly GetPreviousAssemblyInCallStack()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrames().Select(f => f.GetMethod().DeclaringType.Assembly).Where(a => a != thisAssembly).FirstOrDefault();
        }

        public static DateTime GetAssemblyCompiledTime(Assembly assembly)
        {
            byte[] bs = new byte[2048];
            System.IO.Stream s = new System.IO.FileStream(assembly.Location, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            s.Read(bs, 0, bs.Length);
            s.Close();
            int i = System.BitConverter.ToInt32(bs, 60);
            int secs_since_1970 = System.BitConverter.ToInt32(bs, i + 8);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secs_since_1970);
            return dt.ToLocalTime();
        }

        public static string GetAppCompilationVersion()
        {
            DateTime dt = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetEntryAssembly());
            DateTime dt2 = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetCallingAssembly());
            dt = dt > dt2 ? dt : dt2;
            return dt.ToString("yy-MM-dd-HH-mm-ss");
        }

        public static Version GetExecutingAssemblyVersion()
        {
            AssemblyInfo ai = new AssemblyInfo(Assembly.GetCallingAssembly());
            return ai.Version;
        }

        public static string GetExecutingAssemblyName()
        {
            AssemblyInfo ai = new AssemblyInfo(Assembly.GetCallingAssembly());
            return ai.Name;
        }

        //public static System.Drawing.Icon GetAppIcon(Assembly assembly = null)
        //{
        //    return System.Drawing.Icon.ExtractAssociatedIcon((assembly != null ? assembly : Assembly.GetEntryAssembly()).Location);
        //}

        //public static System.Windows.Media.ImageSource GetAppIconImageSource()
        //{
        //    return GetAppIcon().ToImageSource();
        //}

        public class AssemblyInfo
        {
            public AssemblyInfo(string file)
            {
                if (file == null)
                    Assembly = Assembly.GetCallingAssembly();
                else
                    Assembly = Assembly.LoadFile(file);
            }

            public Assembly Assembly { get; }

            public AssemblyInfo(Assembly a = null)
            {
                if (a == null)
                    Assembly = Assembly.GetCallingAssembly();
                else
                    Assembly = a;
            }

            public string CompilationVersion
            {
                get
                {
                    DateTime dt = GetAssemblyCompiledTime(Assembly);
                    return dt.ToString("yy-MM-dd-HH-mm-ss");
                }
            }

            public string Title
            {
                get
                {
                    object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length < 1)
                        return null;
                    return ((AssemblyTitleAttribute)attributes[0]).Title;
                }
            }

            //public string AssemblyVersion//does not give auto build part
            //{
            //    get
            //    {
            //        return a.GetName().Version.ToString();
            //    }
            //}

            public Version Version
            {
                get
                {
                    return Assembly.GetName().Version;
                }
            }

            public Version FileVersion
            {
                get
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.Location);
                    return new Version(fvi.ProductVersion);
                }
            }

            public string Description
            {
                get
                {
                    object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length <1)
                        return null;
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }

            public string Product
            {
                get
                {
                    object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length < 1)
                        return null;
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }

            public string Copyright
            {
                get
                {
                    object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length < 1)
                        return null;
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }

            public string Company
            {
                get
                {
                    object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attributes.Length < 1)
                        return null;
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
            }

            public string Name
            {
                get
                {
                    return Assembly.GetName().Name;
                }
            }
        }
    }
}