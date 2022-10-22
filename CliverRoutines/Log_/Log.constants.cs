//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Cliver
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Log
    {
        static Log()
        {
            {//this block works on Windows desktop, XamarinMAC, NT service, Android
                Assembly headAssembly = Assembly.GetEntryAssembly();
                //!!!when using WCF or Android, GetEntryAssembly() == NULL 
                if (headAssembly == null)
                    headAssembly = Assembly.GetCallingAssembly();
                ProgramName = headAssembly.GetName(false).Name;

                //AppDir = AppDomain.CurrentDomain.BaseDirectory?.TrimEnd(Path.DirectorySeparatorChar);!!!gives not an app's dir on WCF or Android
                if (headAssembly.Location != null)
                    AppDir = PathRoutines.GetFileDir(headAssembly.Location);
                else//just in case. It hardly can come here
                {
                    Uri u = new Uri(headAssembly.CodeBase);
                    AppDir = PathRoutines.GetFileDir(u.LocalPath);
                }

                AssemblyRoutines.AssemblyInfo ai = new AssemblyRoutines.AssemblyInfo(headAssembly);
                CompanyName = ai.Company;
            }

            //{
            //    HashSet<Assembly> assemblies = new HashSet<Assembly>();
            //    Assembly a = null;
            //    StackTrace stackTrace = new StackTrace();
            //    foreach (StackFrame st in stackTrace.GetFrames())
            //    {
            //        Assembly b = st.GetMethod().DeclaringType.Assembly;
            //        if (b == null)
            //            break;
            //        a = b;
            //        assemblies.Add(a);
            //    }
            //    if (a == null)
            //        a = Assembly.GetEntryAssembly();

            //    AssemblyName = a.FullName;
            //    Process p = Process.GetCurrentProcess();
            //    AppDir = PathRoutines.GetFileDir(p.MainModule.FileName);
            //    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(p.MainModule.FileName);
            //    if (fvi != null)
            //    {
            //        CompanyName = fvi.CompanyName;
            //        //ProductName = fvi.ProductName;
            //    }
            //}

            //!!!No write permission on macOS
            //CompanyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + CompanyName;
            //!!!No write permission on macOS
            //AppCompanyCommonDataDir = CompanyCommonDataDir + Path.DirectorySeparatorChar + ProcessName;
            //CompanyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + CompanyName;
            //AppCompanyUserDataDir = CompanyUserDataDir + Path.DirectorySeparatorChar + ProcessName;
        }

        /// <summary>
        /// Name of the assembly considered first in the program.
        /// </summary>
        public static readonly string ProgramName;

        /// <summary>
        /// Product name of the executing file.
        /// </summary>
        //public static readonly string ProductName;

        /// <summary>
        /// Company name of the executing file.
        /// </summary>
        public static readonly string CompanyName;

        /// <summary>
        /// User-independent company data directory.
        /// (!)No write permission on macOS
        /// </summary>
        public static string CompanyCommonDataDir//it is property to avoid forced setting if crashes on certain platform
        {
            get
            {
                if (companyCommonDataDir == null)
                    //!!!No write permission on macOS
                    companyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + CompanyName;
                return companyCommonDataDir;
            }
        }
        static string companyCommonDataDir;

        /// <summary>
        /// User-independent company-application data directory.
        /// (!)No write permission on macOS
        /// </summary>
        public static string AppCompanyCommonDataDir//it is property to avoid forced setting if crashes on certain platform
        {
            get
            {
                if (appCompanyCommonDataDir == null)
                    //!!!No write permission on macOS
                    appCompanyCommonDataDir = CompanyCommonDataDir + Path.DirectorySeparatorChar + ProgramName;
                return appCompanyCommonDataDir;
            }
        }
        static string appCompanyCommonDataDir;

        /// <summary>
        /// User-specific company data directory.
        /// </summary>
        public static string CompanyUserDataDir//it is property to avoid forced setting if crashes on certain platform
        {
            get
            {
                if (companyUserDataDir == null)
                {
                    if (Assembly.GetEntryAssembly() == null)//on Android, macOS
                        companyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);//it contains the app's name already so company name is a redundant
                    else
                        companyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + CompanyName;
                }
                return companyUserDataDir;
            }
        }
        static string companyUserDataDir;

        /// <summary>
        /// User-specific company-application data directory.
        /// </summary>
        public static string AppCompanyUserDataDir//it is property to avoid forced setting if crashes on certain platform
        {
            get
            {
                if (appCompanyUserDataDir == null)
                {
                    if (Assembly.GetEntryAssembly() == null)//on Android, macOS
                        appCompanyUserDataDir = CompanyUserDataDir;//it contains the app's name already
                    else
                        appCompanyUserDataDir = CompanyUserDataDir + Path.DirectorySeparatorChar + ProgramName;
                }
                return appCompanyUserDataDir;
            }
        }
        static string appCompanyUserDataDir;

        /// <summary>
        /// Directory where the application binary is located.
        /// </summary>
        public readonly static string AppDir;
    }
}

