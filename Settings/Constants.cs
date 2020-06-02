//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

namespace Cliver.PdfDocumentParser
{
    public partial class Settings
    {
        
        public static readonly ConstantsSettings Constants;

        public class ConstantsSettings : Cliver.UserSettings
        {
            [Newtonsoft.Json.JsonIgnore]
            public readonly string HelpFile = @"docs\index.html";

            public int PdfPageImageResolution = 300;//tesseract requires at least 300
            public float CoordinateDeviationMargin = 1f;
            public int PdfResolution = 72;//72 is resolution of the most pdf's

            [Newtonsoft.Json.JsonIgnore]
            public float Image2PdfResolutionRatio { get; private set; }

            public override void Loaded()
            {
                Image2PdfResolutionRatio = (float)PdfResolution / PdfPageImageResolution;
            }

            public override void Saving()
            {
            }
        }
    }
}