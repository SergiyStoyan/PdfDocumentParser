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

            //Settings.test.template2s.Add(new Template2 { DocumentFirstPageRecognitionMarks = new System.Collections.Generic.List<Template2.Mark> { new Template2.Mark.PdfText(), new Template2.Mark.OcrText(), new Template2.Mark.ImageData(), } });
            //Settings.test.Save();
        }

        public static void Initialize()
        {//trigger Program()

        }
    }
}