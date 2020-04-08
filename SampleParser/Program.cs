//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cliver.SampleParser
{
    /*
 TBD:

     */
    class Program
    {
        [DllImport("Shcore.dll")]
        static extern int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);

        // According to https://msdn.microsoft.com/en-us/library/windows/desktop/dn280512(v=vs.85).aspx
        private enum DpiAwareness
        {
            None = 0,
            SystemAware = 1,
            PerMonitorAware = 2
        }

        static Program()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                Win.LogMessage.Error(e);
                Environment.Exit(0);
            };

            //SetProcessDpiAwareness((int)DpiAwareness.PerMonitorAware);

            Version = AssemblyRoutines.GetAssemblyCompiledTime(Assembly.GetEntryAssembly()).ToString("yyMMdd-HHmmss"); //String.Format("Version {0}", AssemblyVersion);
            Name = Application.ProductName;

            Message.TopMost = true;

            Config.Reload();
            Win.LogMessage.DisableStumblingDialogs = false;
            Log.DeleteOldLogsDialog = null;
            Log.Initialize(Log.Mode.ONLY_LOG, Log.CompanyCommonDataDir, true);
        }
        public static readonly string Version;
        public static readonly string Name;

        [STAThread]
        static void Main()
        {
            try
            {
                PdfDocumentParser.Program.Initialize();
                
                Application.Run(MainForm.This);
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
            Environment.Exit(0);
        }
    }
}