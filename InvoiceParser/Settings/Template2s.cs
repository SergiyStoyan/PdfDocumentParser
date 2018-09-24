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
        public static readonly Template2sSettings Template2s;

        public class Template2sSettings : Cliver.Settings
        {
            public Template2 InitialTemplate2;
            public List<Template2> Template2s = new List<Template2>();

            public override void Loaded()
            {
                try
                {
                    __Indented = false;

                    //TEMPORARY: conversion to new format
                    if (Template2s.Count < 1)
                    {
                        if (Settings.Templates == null)
                            Config.ReloadField("Templates");
                        foreach (Template0 t0 in Settings.Templates.Templates)
                        {
                            Template2 t = new Template2
                            {
                                CanShareFileWithAnotherTemplates = t0.CanShareFileWithAnotherTemplates,
                                Comment = t0.Comment,
                                DetectingTemplateLastPageNumber = t0.DetectingTemplateLastPageNumber,
                                FileFilterRegex = t0.FileFilterRegex,
                                ModifiedTime = t0.ModifiedTime,
                                OrderWeight = t0.OrderWeight,
                                Active = t0.Active,
                                Group = t0.Group,
                                Template = new PdfDocumentParser.Template
                                {
                                    Name = t0.Name,
                                    AutoDeskew = t0.AutoDeskew,
                                    AutoDeskewThreshold = t0.AutoDeskewThreshold,
                                    AutoPagesRotation = t0.AutoPagesRotation,
                                    PagesRotation = t0.PagesRotation,
                                    Anchors = new List<PdfDocumentParser.Template.Anchor>(),
                                    Marks = new List<PdfDocumentParser.Template.Mark>(),
                                    Fields = new List<PdfDocumentParser.Template.Field>(),
                                },
                            };
                            t.Template.Editor = t0.Editor;

                            foreach (PdfDocumentParser.Template0.FloatingAnchor fa0 in t0.FloatingAnchors)
                            {
                                PdfDocumentParser.Template.Anchor fa;
                                switch (fa0.ValueType)
                                {
                                    case PdfDocumentParser.Template0.ValueTypes.PdfText:
                                        {
                                            PdfDocumentParser.Template0.FloatingAnchor.PdfTextValue v0 = (PdfDocumentParser.Template0.FloatingAnchor.PdfTextValue)fa0.Value;
                                            PdfDocumentParser.Template.Anchor.PdfText v = new PdfDocumentParser.Template.Anchor.PdfText
                                            {
                                                Id = fa0.Id,
                                                PositionDeviation = fa0.PositionDeviation,
                                                PositionDeviationIsAbsolute = v0.PositionDeviationIsAbsolute,
                                                SearchRectangleMargin = v0.SearchRectangleMargin,
                                                CharBoxs = v0.CharBoxs,
                                            };
                                            fa = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.OcrText:
                                        {
                                            PdfDocumentParser.Template0.FloatingAnchor.OcrTextValue v0 = (PdfDocumentParser.Template0.FloatingAnchor.OcrTextValue)fa0.Value;
                                            PdfDocumentParser.Template.Anchor.OcrText v = new PdfDocumentParser.Template.Anchor.OcrText
                                            {
                                                Id = fa0.Id,
                                                PositionDeviation = fa0.PositionDeviation,
                                                PositionDeviationIsAbsolute = v0.PositionDeviationIsAbsolute,
                                                SearchRectangleMargin = v0.SearchRectangleMargin,
                                                CharBoxs = v0.CharBoxs,
                                            };
                                            fa = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.ImageData:
                                        {
                                            PdfDocumentParser.Template0.FloatingAnchor.ImageDataValue v0 = (PdfDocumentParser.Template0.FloatingAnchor.ImageDataValue)fa0.Value;
                                            PdfDocumentParser.Template.Anchor.ImageData v = new PdfDocumentParser.Template.Anchor.ImageData
                                            {
                                                Id = fa0.Id,
                                                PositionDeviation = fa0.PositionDeviation,
                                                PositionDeviationIsAbsolute = v0.PositionDeviationIsAbsolute,
                                                SearchRectangleMargin = v0.SearchRectangleMargin,
                                                BrightnessTolerance = v0.BrightnessTolerance,
                                                DifferentPixelNumberTolerance = v0.DifferentPixelNumberTolerance,
                                                FindBestImageMatch = v0.FindBestImageMatch,
                                                ImageBoxs = v0.ImageBoxs,
                                            };
                                            fa = v;
                                            break;
                                        }
                                    default: throw new Exception("1");
                                }
                                t.Template.Anchors.Add(fa);
                            }

                            foreach (PdfDocumentParser.Template0.Mark m0 in t0.DocumentFirstPageRecognitionMarks)
                            {
                                PdfDocumentParser.Template.Mark m;
                                switch (m0.Type)
                                {
                                    case PdfDocumentParser.Template0.ValueTypes.PdfText:
                                        {
                                            PdfDocumentParser.Template0.Mark.PdfTextValue v0 = (PdfDocumentParser.Template0.Mark.PdfTextValue)m0.Value;
                                            PdfDocumentParser.Template.Mark.PdfText v = new PdfDocumentParser.Template.Mark.PdfText
                                            {
                                                AnchorId = m0.FloatingAnchorId,
                                                Rectangle = m0.Rectangle,
                                            };
                                            if (v0 != null)
                                            {
                                                v.Text = v0.Text;
                                            }
                                            m = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.OcrText:
                                        {
                                            PdfDocumentParser.Template0.Mark.OcrTextValue v0 = (PdfDocumentParser.Template0.Mark.OcrTextValue)m0.Value;
                                            PdfDocumentParser.Template.Mark.OcrText v = new PdfDocumentParser.Template.Mark.OcrText
                                            {
                                                AnchorId = m0.FloatingAnchorId,
                                                Rectangle = m0.Rectangle,
                                            };
                                            if (v0 != null)
                                            {
                                                v.Text = v0.Text;
                                            }
                                            m = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.ImageData:
                                        {
                                            PdfDocumentParser.Template0.Mark.ImageDataValue v0 = (PdfDocumentParser.Template0.Mark.ImageDataValue)m0.Value;
                                            PdfDocumentParser.Template.Mark.ImageData v = new PdfDocumentParser.Template.Mark.ImageData
                                            {
                                                AnchorId = m0.FloatingAnchorId,
                                                Rectangle = m0.Rectangle,
                                            };
                                            if (v0 != null)
                                            {
                                                v.BrightnessTolerance = v0.BrightnessTolerance;
                                                v.DifferentPixelNumberTolerance = v0.DifferentPixelNumberTolerance;
                                                v.FindBestImageMatch = v0.FindBestImageMatch;
                                                v.ImageData_ = v0.ImageData;
                                            }
                                            m = v;
                                            break;
                                        }
                                    default: throw new Exception("12");
                                }
                                t.Template.Marks.Add(m);
                            }

                            foreach (PdfDocumentParser.Template0.Field f0 in t0.Fields)
                            {
                                PdfDocumentParser.Template.Field f;
                                switch (f0.Type)
                                {
                                    case PdfDocumentParser.Template0.ValueTypes.PdfText:
                                        {
                                            PdfDocumentParser.Template.Field.PdfText v = new PdfDocumentParser.Template.Field.PdfText
                                            {
                                                AnchorId = f0.FloatingAnchorId,
                                                Rectangle = f0.Rectangle,
                                                Name = f0.Name,
                                            };
                                            f = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.OcrText:
                                        {
                                            PdfDocumentParser.Template.Field.OcrText v = new PdfDocumentParser.Template.Field.OcrText
                                            {
                                                AnchorId = f0.FloatingAnchorId,
                                                Rectangle = f0.Rectangle,
                                                Name = f0.Name,
                                            };
                                            f = v;
                                            break;
                                        }
                                    case PdfDocumentParser.Template0.ValueTypes.ImageData:
                                        {
                                            throw new Exception("1233");
                                        }
                                    default: throw new Exception("124");
                                }
                                t.Template.Fields.Add(f);
                            }

                            Template2s.Add(t);
                        }
                        Save();
                        Reload();
                        if (Template2s.Count > 0)
                            if (System.IO.File.Exists(Settings.Templates.__File))
                                System.IO.File.Delete(Settings.Templates.__File);
                    }
                }
                catch (Exception e)
                {
                    Message.Error(e);
                }
            }

            public override void Saving()
            {
                Template2s.RemoveAll(x => string.IsNullOrWhiteSpace(x.Template.Name));
            }

            //public void SaveIfTouched()
            //{
            //    if (!touched)
            //        return;
            //    Save();
            //}

            public void Touch()
            {
                touched = true;
                TouchedChanged?.BeginInvoke(null, null);
            }
            bool touched = false;
            public bool IsTouched()
            {
                return touched;
            }
            public delegate void OnTouchedChanged();
            public event OnTouchedChanged TouchedChanged;

            public override void Saved()
            {
                touched = false;
                TouchedChanged?.BeginInvoke(null, null);
                Settings.SynchronizationSettings.SynchronizeUploadFile(this.__File);
            }

            public Template2 CreateInitialTemplate()
            {
                if (InitialTemplate2 != null && InitialTemplate2.Template != null)
                {
                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate2);
                    Template2 t = SerializationRoutines.Json.Deserialize<Template2>(ts);
                    t.Template.Name = "";
                    return t;
                }
                return new Template2
                {
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    Template = new PdfDocumentParser.Template
                    {
                        AutoDeskew = false,
                        Fields = new List<PdfDocumentParser.Template.Field> {
                        new PdfDocumentParser.Template.Field.PdfText { Name = "INVOICE#" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                        new PdfDocumentParser.Template.Field.PdfText { Name = "JOB#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new PdfDocumentParser.Template.Field.PdfText { Name = "PO#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new PdfDocumentParser.Template.Field.PdfText { Name = "COST" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                    },
                        Name = "",
                        Anchors = new List<PdfDocumentParser.Template.Anchor>(),
                        Marks = new List<PdfDocumentParser.Template.Mark>(),
                        PagesRotation = PdfDocumentParser.Template.PageRotations.NONE,
                        Editor = new PdfDocumentParser.Template.EditorSettings
                        {
                            TestPictureScale = 1.2m,
                            TestFile = "",
                            ExtractFieldsAutomaticallyWhenPageChanged = true,
                        },
                    },
                };
            }
        }
    }

    public class Template2
    {
        public PdfDocumentParser.Template Template;

        public bool Active = true;
        public string Group;
        public DateTime ModifiedTime;
        public string Comment;
        public float OrderWeight = 1f;
        //public int PdfPageMinNumberToDetectTemplate = 3;
        public uint DetectingTemplateLastPageNumber = 1;
        public Regex FileFilterRegex = null;
        public bool CanShareFileWithAnotherTemplates = false;//!!!not engaged!!!

        public string GetModifiedTimeAsString()
        {
            return ModifiedTime.ToString("yy-MM-dd HH:mm:ss");
        }
    }
}