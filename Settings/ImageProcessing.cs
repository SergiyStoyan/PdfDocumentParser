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
        public static readonly ImageProcessingSettings ImageProcessing;


        public class ImageProcessingSettings : Cliver.Settings
        {
            public int PdfPageImageResolution = 300;//tesseract requires at least 300
            public float CoordinateDeviationMargin = 0.001f;
            public float Image2PdfResolutionRatio { get { return _Image2PdfResolutionRatio; } }
            float _Image2PdfResolutionRatio;
            
            public override void Loaded()
            {
                _Image2PdfResolutionRatio = (float)72 / PdfPageImageResolution;//72 is resolution of the most pdf's
            }

            public override void Saving()
            {
            }
        }
    }
}