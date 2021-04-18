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
            public const int PdfResolution = 72;//72 is resolution of the most pdf's. All template parameters are measured for PdfResolution!

            [Newtonsoft.Json.JsonIgnore]
            public float Image2PdfResolutionRatio { get; private set; }

            protected override void Loaded()
            {
                Image2PdfResolutionRatio = (float)PdfResolution / PdfPageImageResolution;
            }

            protected override void Saving()
            {
            }
        }
    }
}