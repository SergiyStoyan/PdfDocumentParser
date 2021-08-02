//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

using System.Collections.Generic;

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
            public const int PdfResolution = 72;//It is the basic resolution. All template parameters that depend on bitmap are scaled to it!

            public int InitialSearchRectangleMargin = 100;

            public Ocr.Config OcrConfig;

            [Newtonsoft.Json.JsonIgnore]
            public float Image2PdfResolutionRatio { get; private set; }

            protected override void Loaded()
            {
                Image2PdfResolutionRatio = (float)PdfResolution / PdfPageImageResolution;

                if (OcrConfig == null)
                    OcrConfig = new Ocr.Config
                    {
                        Language = "eng",
                        EngineMode = Tesseract.EngineMode.Default,
                        VariableNames2value = new Dictionary<string, object>
                        {
                            { "load_system_dawg", false },//don't load dictionary
                            { "load_freq_dawg", false },//don't load dictionary
                            //(name: "tessedit_char_whitelist", "0123456789.,"),
                        }
                    };
            }

            protected override void Saving()
            {
            }
        }
    }
}