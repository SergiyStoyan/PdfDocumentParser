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

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly ImageProcessingSettings ImageProcessing;


        public class ImageProcessingSettings : Cliver.Settings
        {
            public float BrightnessTolerance = 0.2f;
            public float DifferentPixelNumberTolerance = 0.05f;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}