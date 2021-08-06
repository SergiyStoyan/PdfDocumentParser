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

for scan processing:
- invoices still have hand marks on them which hinder OCR. Solution: filter out colors control;
- line filtering (when text overlaps a line);
- each field has its own tesseract config;
- OcrColumnLines - provide cropping by anchors;
- to preserve normal lines, provide column removal from table;

- ?implement anchor row-separator which requires multiple anchor matching (needed for GCG statement) - verdict: it's better to have multiple field extraction and use it as a separator in a custom processor;
- provide multiple field extraction on page;

- ?migrate to iText7 (!!! 7.1.15 has the following bugs: -some pdf's are read in wrong encoding; -wrong and sometimes very wrong GetAscentLine(); -new PdfReader(file) locks file forever if read error;)

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
                Log.Error(e);
                Message.Error(e);
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
        }

        /// <summary>
        /// First it must be called before any use of the PdfDocumentParser. 
        /// Then it can be called to re-initialize.
        /// </summary>
        /// <param name="ocrConfig"></param>
        public static void Initialize(Ocr.Config ocrConfig)//trigger Program()
        {
            Ocr.DisposeThis();
            Settings.Constants.OcrConfig = ocrConfig;
        }

        public static readonly Version Version;
        public static readonly string Name;
        public static readonly string FullName;
    }
}