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
            public string HelpFile = @"help\index.html";

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}