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

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly TestFilesSettings TestFiles;

        public class TestFilesSettings : Cliver.Settings
        {
            public Dictionary<string,string> TemplateNames2TestFile;//the customer asked this

            public override void Loaded()
            {
                if (TemplateNames2TestFile == null)
                    TemplateNames2TestFile = new Dictionary<string, string>();
            }

            public override void Saving()
            {
            }
        }
    }
}