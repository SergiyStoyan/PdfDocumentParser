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

namespace Cliver.PdfDocumentParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly ConstantsSettings Constants;

        public class ConstantsSettings : Cliver.Settings
        {
            [Newtonsoft.Json.JsonIgnore]
            public readonly string HelpFile = @"docs\index.html";

            public float TextAutoInsertSpaceThreshold = 6;
            public int PdfPageImageResolution = 300;//tesseract requires at least 300
            public float CoordinateDeviationMargin = 1f;

            [Newtonsoft.Json.JsonIgnore]
            public float Image2PdfResolutionRatio { get; private set; }

            public override void Loaded()
            {
                Image2PdfResolutionRatio = (float)72 / PdfPageImageResolution;//72 is resolution of the most pdf's
            }

            public override void Saving()
            {
            }
        }
    }
}