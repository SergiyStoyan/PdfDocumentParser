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
        [Cliver.Settings.Obligatory]
        public static readonly GeneralSettings General;

        public class GeneralSettings : Cliver.Settings
        {
            public string InputFolder = @"d:\_d\_projects\PdfDocumentParser\_test_files";
            public bool IgnoreHiddenFiles = true;
            public bool ReadInputFolderRecursively = false;

            public System.Drawing.Color StampColor = System.Drawing.Color.Red;
            
            public List<string> OrderedOutputFieldNames = new List<string>();

            public bool UseActiveSelectPattern = false;
            public bool UseNameSelectPattern = true;
            public bool UseGroupSelectPattern = false;
            public bool UseCommentSelectPattern = false;
            public bool UseOrderWeightPattern = false; 
            public bool SortSelectedUp = true;

            public override void Loaded()
            {
                if (string.IsNullOrWhiteSpace(InputFolder))
                    InputFolder = ProgramRoutines.GetAppDirectory();
            }

            public override void Saving()
            {
            }
        }
    }
}