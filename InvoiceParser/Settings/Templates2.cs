////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        http://www.cliversoft.com
////********************************************************************************************
//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using System.Data.Linq;
//using System.Linq;
//using System.Drawing;
//using System.Collections.Specialized;
//using Cliver.PdfDocumentParser;
//using System.Net;

//namespace Cliver.InvoiceParser
//{
//    public partial class Settings
//    {
//        [Cliver.Settings.Obligatory]
//        public static readonly TemplatesSettings Templates;

//        public class TemplatesSettings : Cliver.Settings
//        {
//            public Template InitialTemplate;
//            public List<Template> Templates = new List<Template>();//preserving order of matching: only the first match is to be applied

//            public override void Loaded()
//            {
//                //if (Templates.Count < 1)
//                //    Templates.Add(CreateInitialTemplate());

//                if (Templates.Count > 0 && Templates[0].BaseTemplate == null)//old format conversion
//                {
//                    FileSystemRoutines.CopyFile(__File, Config.StorageDir + "\\_Templates.Cliver.InvoiceParser.Settings+_TemplatesSettings.json", true);
//                    Settings._Templates.Reload();
//                    Templates.Clear();
//                    foreach (_Template _t in Settings._Templates.Templates)
//                    {
//                        Template t = new Template
//                        {
//                            Active = _t.Active,
//                            Group = _t.Group,
//                            BaseTemplate = new PdfDocumentParser.Template
//                            {
//                                AutoDeskew = _t.AutoDeskew,
//                                AutoDeskewThreshold = _t.AutoDeskewThreshold,
//                                BrightnessTolerance = _t.BrightnessTolerance,
//                                DifferentPixelNumberTolerance = _t.DifferentPixelNumberTolerance,
//                                DocumentFirstPageRecognitionMarks = _t.DocumentFirstPageRecognitionMarks,
//                                Fields = _t.Fields,
//                                FileFilterRegex = _t.FileFilterRegex,
//                                FindBestImageMatch = _t.FindBestImageMatch,
//                                FloatingAnchors = _t.FloatingAnchors,
//                                Name = _t.Name,
//                                PagesRotation = _t.PagesRotation,
//                                TestFile = _t.TestFile,
//                                TestPictureScale = _t.TestPictureScale,
//                            }
//                        };
//                        Templates.Add(t);
//                    }
//                    Save();
//                    Reload();
//                }
//            }

//            public override void Saving()
//            {
//            }

//            public override void Saved()
//            {
//                Settings.SynchronizationSettings.SynchronizeUploadFile(this.__File);
//            }

//            public Template CreateInitialTemplate()
//            {
//                if (InitialTemplate != null)
//                {
//                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate);
//                    return SerializationRoutines.Json.Deserialize<Template>(ts);
//                }
//                return new Template
//                {
//                    Active = true,
//                    BaseTemplate = new PdfDocumentParser.Template
//                    {
//                        AutoDeskew = false,
//                        BrightnessTolerance = 0.4f,
//                        DifferentPixelNumberTolerance = 0.01f,
//                        Fields = new List<PdfDocumentParser.Template.Field> {
//                        new PdfDocumentParser.Template.Field { Name = "INVOICE#" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
//                        new PdfDocumentParser.Template.Field { Name = "JOB#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
//                        new PdfDocumentParser.Template.Field { Name = "PO#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
//                        new PdfDocumentParser.Template.Field { Name = "COST" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
//                    },
//                        Name = "-new-",
//                        FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
//                        FindBestImageMatch = false,
//                        FloatingAnchors = new List<PdfDocumentParser.Template.FloatingAnchor>(),
//                        DocumentFirstPageRecognitionMarks = new List<PdfDocumentParser.Template.Mark>(),
//                        PagesRotation = PdfDocumentParser.Template.PageRotations.NONE,
//                        TestPictureScale = 1.3m,
//                        TestFile = "",
//                    }
//                };
//            }
//        }
//    }

//    public class Template
//    {
//        public bool Active = true;
//        public Cliver.PdfDocumentParser.Template BaseTemplate;
//        public string Group;
//    }
//}
