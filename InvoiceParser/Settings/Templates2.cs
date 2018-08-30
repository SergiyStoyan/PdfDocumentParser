//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Linq;
using System.Drawing;
using System.Collections.Specialized;
using Cliver.PdfDocumentParser;
using System.Net;

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly Templates2Settings Templates2;

        public class Template2
        {
            public bool Active = true;
            public Cliver.PdfDocumentParser.Template BaseTemplate;
            public string Group;
        }

        public class Templates2Settings : Cliver.Settings
        {
            public Template2 InitialTemplate;
            public List<Template2> Templates = new List<Template2>();//preserving order of matching: only the first match is to be applied

            public override void Loaded()
            {
                //if (Templates.Count < 1)
                //    Templates.Add(CreateInitialTemplate());
            }

            public override void Saving()
            {
            }

            public override void Saved()
            {
                Settings.SynchronizationSettings.SynchronizeUploadFile(this.__File);
            }

            public Template2 CreateInitialTemplate()
            {
                if (InitialTemplate != null)
                {
                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate);
                    return SerializationRoutines.Json.Deserialize<Template2>(ts);
                }
                return new Template2
                {
                    Active = true,
                    BaseTemplate = new PdfDocumentParser.Template
                    {
                        AutoDeskew = false,
                        BrightnessTolerance = 0.4f,
                        DifferentPixelNumberTolerance = 0.01f,
                        Fields = new List<Template.Field> {
                        new Template.Field { Name = "INVOICE#" , Rectangle=new Template.RectangleF(0,0,10,10)},
                        new Template.Field { Name = "JOB#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field { Name = "PO#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field { Name = "COST" , Rectangle=new Template.RectangleF(0,0,10,10)},
                    },
                        Name = "-new-",
                        FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                        FindBestImageMatch = false,
                        FloatingAnchors = new List<Template.FloatingAnchor>(),
                        DocumentFirstPageRecognitionMarks = new List<Template.Mark>(),
                        PagesRotation = Template.PageRotations.NONE,
                        TestPictureScale = 1.3m,
                        TestFile = "",
                    }
                };
            }
        }
    }
}
