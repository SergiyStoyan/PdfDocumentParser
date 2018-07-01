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
        public static readonly ImageProcessingSettings ImageProcessing;


        public class ImageProcessingSettings : Cliver.Settings
        {
            public float BrightnessTolerance = 0.4f;
            public float DifferentPixelNumberTolerance = 0.01f;
            public bool FindBestImageMatch = false;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}