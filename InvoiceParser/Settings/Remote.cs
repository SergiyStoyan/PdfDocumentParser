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
        public static readonly RemoteSettings Remote;

        public class RemoteSettings : Cliver.Settings
        {
            readonly public string TemplatesPath = "/Templates.Cliver.InvoiceParser.Settings+TemplatesSettings.json";
            public string AccessToken = "";
            public bool UpdateTemplatesOnStart = true;
            public DateTime LastDownloadedTemplatesTimestamp = DateTime.MinValue;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}