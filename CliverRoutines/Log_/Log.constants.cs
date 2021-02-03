//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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
            /*if (ProgramRoutines.IsWebContext) - !!!crashes on Xamarin!!!
                throw new Exception("Log is disabled in web context.");

            if (ProgramRoutines.IsWebContext)
                ProcessName = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).Name;
            else*/
            /*
            {//this block works on Windows desktop, XamarinMAC, NT service
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                //!!!when using WCF, GetEntryAssembly() is NULL 
                if (entryAssembly == null)
                    entryAssembly = Assembly.GetCallingAssembly();
                ProcessName = entryAssembly.GetName(false).Name;

                AssemblyRoutines.AssemblyInfo ai = new AssemblyRoutines.AssemblyInfo(entryAssembly);
                CompanyName = string.IsNullOrWhiteSpace(ai.Company) ? "CliverSoft" : ai.Company;
                ProductName = ai.Product;

                AppDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            }
            */
            {//this block works on Windows desktop, NT service. 
                //!!!Needs testing on XamarinMAC
                Process p = Process.GetCurrentProcess();
                ProcessName = p.ProcessName;
                AppDir = PathRoutines.GetFileDir(p.MainModule.FileName);
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(p.MainModule.FileName);
                if (fvi != null)
                {
                    CompanyName = fvi.CompanyName;
                    //ProductName = fvi.ProductName;
                }
            }            

            //!!!No write permission on macOS
            //CompanyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + CompanyName;
            //!!!No write permission on macOS
            //AppCompanyCommonDataDir = CompanyCommonDataDir + Path.DirectorySeparatorChar + ProcessName;
            //CompanyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + CompanyName;
            //AppCompanyUserDataDir = CompanyUserDataDir + Path.DirectorySeparatorChar + ProcessName;
        }

        /// <summary>
        /// Name of this process.
        /// </summary>
        public static readonly string ProcessName;

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
                    appCompanyCommonDataDir = CompanyCommonDataDir + Path.DirectorySeparatorChar + ProcessName;
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
                    companyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + CompanyName;
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
                    appCompanyUserDataDir = CompanyUserDataDir + Path.DirectorySeparatorChar + ProcessName;
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

