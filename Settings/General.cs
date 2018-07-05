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

namespace Cliver.PdfDocumentParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly GeneralSettings General;


        public class GeneralSettings : Cliver.Settings
        {
            public string InputFolder = @"d:\_d\_projects\mattpdf\_test_files";
            public string OutputFolder;
            public bool IgnoreHidddenFiles = true;
            public bool ReadInputFolderRecursively = false;

            public System.Drawing.Color SelectionBoxColor = System.Drawing.Color.Red;
            public System.Drawing.Color BoundingBoxColor = System.Drawing.Color.BlueViolet;
            public System.Drawing.Color StampColor = System.Drawing.Color.Red;
            public int PdfPageImageResolution = 300;//tessarct requires at least 300
            public float CoordinateDeviationMargin = 0.001f;
            public decimal TestPictureScale = 1.3m;

            public float Image2PdfResolutionRatio { get { return _Image2PdfResolutionRatio; } }
            float _Image2PdfResolutionRatio;
            public List<string> OrderedOutputFieldNames = new List<string>();

            public Settings.Template InitialTemplate;

            public override void Loaded()
            {
                if (string.IsNullOrWhiteSpace(InputFolder))
                    InputFolder = ProgramRoutines.GetAppDirectory();
                if (string.IsNullOrWhiteSpace(OutputFolder))
                    OutputFolder = InputFolder + "\\Output";// + DateTime.Now.ToString("yyMMddHHmmss");                

                _Image2PdfResolutionRatio = (float)72 / PdfPageImageResolution;//72 is resolution of the most pdf's
            }

            public override void Saving()
            {
            }
        }
    }
}