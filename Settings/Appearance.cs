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
            public System.Drawing.Color AnchorMasterBoxColor = System.Drawing.Color.Violet;
            public System.Drawing.Color AnchorSecondaryBoxColor = System.Drawing.Color.BlueViolet;

            public float SelectionBoxBorderWidth = 1;//MS:You can access the unit of measure of the Graphics object using its PageUnit property. The unit of measure is typically pixels. A Width of 0 will result in the Pen drawing as if the Width were 1.
            public float AnchorMasterBoxBorderWidth = 1;
            public float AnchorSecondaryBoxBorderWidth = 1;

            public override void Loaded()
            {
            }

            public override void Saving()
            {
            }
        }
    }
}