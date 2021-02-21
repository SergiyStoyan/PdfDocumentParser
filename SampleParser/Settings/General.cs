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


namespace Cliver.SampleParser
{
    public partial class Settings
    {
        public static GeneralSettings General;

        public class GeneralSettings : Cliver.UserSettings
        {
            public string InputFolder;
            public string OutputFolder;

            protected override void Loaded()
            {
                if (string.IsNullOrWhiteSpace(InputFolder))
                    InputFolder = ProgramRoutines.GetAppDirectory();
                if (string.IsNullOrWhiteSpace(OutputFolder))
                    OutputFolder = ProgramRoutines.GetAppDirectory();
            }

            protected override void Saving()
            {
            }
        }
    }
}