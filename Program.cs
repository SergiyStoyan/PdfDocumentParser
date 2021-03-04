//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

/*
TBD: 

for scan preprocessing:
- invoices still have hand marks on them which hinder OCR. Solution: a)filter out colors control, b)manual eraser;
- line filtering (when test overlaps a line);

- ?implement anchor row-separator which requires multiple anchor matching (needed for GCG statement) - verdict: it's better to have multiple field extraction and use it as a separator in a custom processor;
- move to IText7;
- provide multiple field extraction on page;

- ?migrate to WPF;
- ?change anchor id->name (involves condition expressions)
 */
namespace Cliver.PdfDocumentParser
{
    public class Program
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

            AssemblyRoutines.AssemblyInfo ai = new AssemblyRoutines.AssemblyInfo();
            Version = ai.Version;
            Name = ai.Product;

            FullName = Name + " " + Version.ToString(3);

            //Log.Initialize(Log.Mode.ONLY_LOG, Log.CompanyCommonDataDir, true);//must be called from the entry projects
            //Log.ShowDeleteOldLogsDialog = false;//must be called from the entry projects
            //Message.TopMost = true;//must be called from the entry projects
            //Config.Reload();//must be called from the entry projects
            //Win.LogMessage.DisableStumblingDialogs = false;//must be called from the entry projects
            //Win.LogMessage.ShowDialog = ((string title, Icon icon, string message, string[] buttons, int default_button, Form owner) => { return Message.ShowDialog(title, icon, message, buttons, default_button, owner); });
        }

        public static void Initialize()
        {//trigger Program()

        }

        public static readonly Version Version;
        public static readonly string Name;
        public static readonly string FullName;
    }
}