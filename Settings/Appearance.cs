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
        public static readonly AppearanceSettings Appearance;

        public class AppearanceSettings : Cliver.Settings
        {
            public System.Drawing.Color SelectionBoxColor = System.Drawing.Color.Red;
            public System.Drawing.Color FloatingAnchorMasterBoxColor = System.Drawing.Color.Violet;
            public System.Drawing.Color FloatingAnchorSecondaryBoxColor = System.Drawing.Color.BlueViolet;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}