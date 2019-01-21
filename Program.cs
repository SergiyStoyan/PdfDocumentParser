//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

/*
TBD: 
- change how char anchors work: allow char anchors to support empty space around selection rectangle;
- check if tesseract's DetectBestOrientation can perform deskew;
- MainForm and TemplateForm to WPF;
- ? !!!in page.cs::_findAnchor() in case Template.Types.ImageData: images are not searched recursively (if a secondary image search failed then search stops). It should be done like it is done for linked anchors;
- ?switch to Tesseract.4
- tune image recognition by checking brightness deltas
- ?provide multiple field extraction on page;
- ?change anchor id->name (involves condition expressions)
- ?store each template in separate file;

 */
/*
 DONE:
 - side anchors added to field;!!!RULE: assigning of rectange and anchors to a field must be done on the same page. 
 - same name field can have multiple instances to look by order;
 - options added to page::GetValue();
     - fields can be marked as columns of a table;
     - space substitution;

    manual: tables can be processed the following ways:
    - get char boxes and do anything;
    - substitute auto-insert spaces with "|" and then split to columns (very unreliabe);
    - create fields as columns
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
                LogMessage.Error(e);
                Environment.Exit(0);
            };

            Message.TopMost = true;

            //Config.Reload();//must be called from the entry projects

            LogMessage.DisableStumblingDialogs = false;
            Log.ShowDeleteOldLogsDialog = false;
            Log.Initialize(Log.Mode.ONLY_LOG, Log.CompanyCommonDataDir, true);
        }

        public static void Initialize()
        {//trigger Program()

        }
    }
}