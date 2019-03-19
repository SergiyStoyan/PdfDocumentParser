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

/*
TBD: 
- check if tesseract's DetectBestOrientation can perform deskew;
- migrate to WPF;
- tune image recognition by checking brightness deltas
- ?switch to Tesseract.4
- ?provide multiple field extraction on page;
- ?change anchor id->name (involves condition expressions)

 */
/*
 DONE:
 - side anchors added to field;
 - same name field can have multiple instances to look by order;
 - options added to page::GetValue();
 - fields can be marked as columns of a table;
 - space substitution;
 - anchor functioning changed;
     */
namespace Cliver.PdfDocumentParser
{
    public class Program
    {
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

            //Assembly assembly = Assembly.GetExecutingAssembly();
            //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            //Version = new Version(fvi.ProductVersion);
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            Name = ((AssemblyProductAttribute)attributes[0]).Product;

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
    }
}