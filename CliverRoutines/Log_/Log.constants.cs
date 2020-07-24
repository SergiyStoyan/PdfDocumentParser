//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
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
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            //!!!when using WCF hapenned that GetEntryAssembly() is NULL 
            if (a == null)
                a = System.Reflection.Assembly.GetCallingAssembly();
            ProcessName = System.Reflection.Assembly.GetEntryAssembly().GetName(false).Name;

            AppDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar);

            AssemblyRoutines.AssemblyInfo ai = new AssemblyRoutines.AssemblyInfo(Assembly.GetEntryAssembly());
            CompanyName = string.IsNullOrWhiteSpace(ai.Company) ? "CliverSoft" : ai.Company;

            //!!!no write permission on macOS!!!
            CompanyCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + System.IO.Path.DirectorySeparatorChar + CompanyName;
            AppCommonDataDir = CompanyCommonDataDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName;

            CompanyUserDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + CompanyName;
            AppUserDataDir = CompanyUserDataDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName;
        }

        /// <summary>
        /// Normalized name of this process
        /// </summary>
        public static readonly string ProcessName;

        public static readonly string CompanyName;

        /// <summary>
        /// Directory where the company's application data independent on user are located.
        /// </summary>
        public static readonly string CompanyCommonDataDir;

        /// <summary>
        /// Directory where the application's data files independent on user are located.
        /// </summary>
        public static readonly string AppCommonDataDir;

        /// <summary>
        /// Directory where the CliverSoft's application data dependent on user are located.
        /// </summary>
        public static readonly string CompanyUserDataDir;

        /// <summary>
        /// Directory where the application's data files dependent on user are located.
        /// </summary>
        public static readonly string AppUserDataDir;

        /// <summary>
        /// Directory where the application binary is located.
        /// </summary>
        public readonly static string AppDir;
    }
}

