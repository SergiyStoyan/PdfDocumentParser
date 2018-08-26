//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using Cliver.PdfDocumentParser;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Linq;


namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly GeneralSettings General;

        public class GeneralSettings : Cliver.Settings
        {
            public string InputFolder = @"d:\_d\_projects\PdfDocumentParser\_test_files";
            public string OutputFolder;
            public bool IgnoreHidddenFiles = true;
            public bool ReadInputFolderRecursively = false;

            public string TemplatesUrl = "https://www.dropbox.com/s/mhpgr119gg4sqnx/Templates.Cliver.InvoiceParser.Settings%2BTemplatesSettings.json?dl=0";
            public bool UpdateTemplatesOnStart = true;
            public DateTime LastDownloadedTemplatesTimestamp = DateTime.MinValue;

            public System.Drawing.Color StampColor = System.Drawing.Color.Red;

            public List<string> OrderedOutputFieldNames = new List<string>();

            public override void Loaded()
            {
                if (string.IsNullOrWhiteSpace(InputFolder))
                    InputFolder = ProgramRoutines.GetAppDirectory();
                if (string.IsNullOrWhiteSpace(OutputFolder))
                    OutputFolder = InputFolder + "\\Output";// + DateTime.Now.ToString("yyMMddHHmmss");  
            }

            public override void Saving()
            {
            }
        }
    }
}