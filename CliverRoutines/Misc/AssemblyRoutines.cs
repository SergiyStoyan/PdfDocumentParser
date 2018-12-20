//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Reflection;

namespace Cliver
{
    public static class AssemblyRoutines
    {
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

        public static string GetAppVersion()
        {
            DateTime dt = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetEntryAssembly());
            DateTime dt2 = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetCallingAssembly());
            dt = dt > dt2 ? dt : dt2;
            return dt.ToString("yy-MM-dd-HH-mm-ss");
        }

        public static System.Drawing.Icon GetAppIcon(Assembly assembly = null)
        {
            return System.Drawing.Icon.ExtractAssociatedIcon((assembly != null ? assembly : Assembly.GetEntryAssembly()).Location);
        }

        public static System.Windows.Media.ImageSource GetAppIconImageSource()
        {
            return GetAppIcon().ToImageSource();
        }

        public class AssemblyInfo
        {
            public AssemblyInfo(string file)
            {
                if (file == null)
                    a = Assembly.GetEntryAssembly();
                else
                    a = Assembly.LoadFile(file);
            }
            readonly Assembly a;

            public AssemblyInfo(Assembly a = null)
            {
                if (a == null)
                    this.a = Assembly.GetEntryAssembly();
                else
                    this.a = a;
            }

            public string AssemblyCompilationVersion
            {
                get
                {
                    DateTime dt = AssemblyRoutines.GetAssemblyCompiledTime(a);
                    return dt.ToString("yy-MM-dd-HH-mm-ss");
                }
            }

            public string AssemblyTitle
            {
                get
                {
                    object[] attributes = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
            }

            public string AssemblyVersion
            {
                get
                {
                    return a.GetName().Version.ToString();
                }
            }

            public string AssemblyDescription
            {
                get
                {
                    object[] attributes = a.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }

            public string AssemblyProduct
            {
                get
                {
                    object[] attributes = a.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }

            public string AssemblyCopyright
            {
                get
                {
                    object[] attributes = a.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }

            public string AssemblyCompany
            {
                get
                {
                    object[] attributes = a.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
            }
        }
    }
}