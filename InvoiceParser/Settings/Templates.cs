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
        public static readonly TemplatesSettings Templates;

        public class TemplatesSettings : Cliver.Settings
        {
            public Template InitialTemplate;
            public List<Template> Templates = new List<Template>();

            public override void Loaded()
            {
                //TEMPORARY: conversion to new format
                if (Templates.Count > 0 && Templates[0].Base == null)
                {
                    FileSystemRoutines.CopyFile(__File, Config.StorageDir + "\\Templates0.Cliver.InvoiceParser.Settings+Templates0Settings.json", true);
                    Settings.Templates0.Reload();
                    Templates.Clear();
                    foreach (Template0 t0 in Settings.Templates0.Templates)
                    {
                        Template t = new Template
                        { CanShareFileWithAnotherTemplates = t0.CanShareFileWithAnotherTemplates,
                            Comment = t0.Comment,
                            DetectingTemplateLastPageNumber = t0.DetectingTemplateLastPageNumber,
                            FileFilterRegex = t0.FileFilterRegex,
                            ModifiedTime = t0.ModifiedTime,
                            OrderWeight = t0.OrderWeight,
                            Active = t0.Active,
                            Group = t0.Group,
                            Base = new PdfDocumentParser.Template
                            {
                                Name = t0.Name,
                                AutoDeskew = t0.AutoDeskew,
                                AutoDeskewThreshold = t0.AutoDeskewThreshold,
                                AutoPagesRotation = t0.AutoPagesRotation,
                                PagesRotation = t0.PagesRotation,
                                FloatingAnchors = new List<PdfDocumentParser.Template.FloatingAnchor>(),
                                DocumentFirstPageRecognitionMarks = new List<PdfDocumentParser.Template.Mark>(),
                                Fields = new List<PdfDocumentParser.Template.Field>() ,
                            },                            
                        };
                        t.Base.Editor.ExtractFieldsAutomaticallyWhenPageChanged = t0.Editor.ExtractFieldsAutomaticallyWhenPageChanged;
                        t.Base.Editor.TestFile = t0.Editor.TestFile;
                        t.Base.Editor.TestPictureScale = t0.Editor.TestPictureScale;

                        foreach (PdfDocumentParser.Template0.FloatingAnchor fa0 in t0.FloatingAnchors)
                        {
                            PdfDocumentParser.Template.FloatingAnchor fa;
                            switch (fa0.Type)
                            {
                                case PdfDocumentParser.Template0.ValueTypes.PdfText:
                                    {
                                        PdfDocumentParser.Template0.FloatingAnchor.PdfTextValue v0 = (PdfDocumentParser.Template0.FloatingAnchor.PdfTextValue)fa0.Value;
                                        PdfDocumentParser.Template.FloatingAnchor.PdfText v = new PdfDocumentParser.Template.FloatingAnchor.PdfText
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
                                        PdfDocumentParser.Template.FloatingAnchor.OcrText v = new PdfDocumentParser.Template.FloatingAnchor.OcrText
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
                                        PdfDocumentParser.Template.FloatingAnchor.ImageData v = new PdfDocumentParser.Template.FloatingAnchor.ImageData
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
                            t.Base.FloatingAnchors.Add(fa);
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
                                            FloatingAnchorId = m0.FloatingAnchorId,
                                            Rectangle = m0.Rectangle,
                                            Text = v0.Text,
                                        };
                                        m = v;
                                        break;
                                    }
                                case PdfDocumentParser.Template0.ValueTypes.OcrText:
                                    {
                                        PdfDocumentParser.Template0.Mark.OcrTextValue v0 = (PdfDocumentParser.Template0.Mark.OcrTextValue)m0.Value;
                                        PdfDocumentParser.Template.Mark.OcrText v = new PdfDocumentParser.Template.Mark.OcrText
                                        {
                                            FloatingAnchorId = m0.FloatingAnchorId,
                                            Rectangle = m0.Rectangle,
                                            Text = v0.Text,
                                        };
                                        m = v;
                                        break;
                                    }
                                case PdfDocumentParser.Template0.ValueTypes.ImageData:
                                    {
                                        PdfDocumentParser.Template0.Mark.ImageDataValue v0 = (PdfDocumentParser.Template0.Mark.ImageDataValue)m0.Value;
                                        PdfDocumentParser.Template.Mark.ImageData v = new PdfDocumentParser.Template.Mark.ImageData
                                        {
                                            FloatingAnchorId = m0.FloatingAnchorId,
                                            Rectangle = m0.Rectangle,
                                            BrightnessTolerance = v0.BrightnessTolerance,
                                            DifferentPixelNumberTolerance = v0.DifferentPixelNumberTolerance,
                                            FindBestImageMatch = v0.FindBestImageMatch,
                                            ImageData_ = v0.ImageData,
                                        };
                                        m = v;
                                        break;
                                    }
                                default: throw new Exception("12");
                            }
                            t.Base.DocumentFirstPageRecognitionMarks.Add(m);
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
                                            FloatingAnchorId = f0.FloatingAnchorId,
                                            Rectangle = f0.Rectangle,
                                             Name=f0.Name,
                                        };
                                        f = v;
                                        break;
                                    }
                                case PdfDocumentParser.Template0.ValueTypes.OcrText:
                                    {
                                        PdfDocumentParser.Template.Field.OcrText v = new PdfDocumentParser.Template.Field.OcrText
                                        { 
                                            FloatingAnchorId = f0.FloatingAnchorId,
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
                            t.Base.Fields.Add(f);
                        }

                        Templates.Add(t);
                    }
                    Save();
                    Reload();
                }
            }

            public override void Saving()
            {
                Templates.RemoveAll(x => string.IsNullOrWhiteSpace(x.Base.Name));
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

            public Template CreateInitialTemplate()
            {
                if (InitialTemplate != null)
                {
                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate);
                    Template t = SerializationRoutines.Json.Deserialize<Template>(ts);
                    t.Base.Name = "";
                    return t;
                }
                return new Template
                {
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    Base = new PdfDocumentParser.Template
                    {
                        AutoDeskew = false,
                        Fields = new List<PdfDocumentParser.Template.Field> {
                        new PdfDocumentParser.Template.Field.PdfText { Name = "INVOICE#" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                        new PdfDocumentParser.Template.Field.PdfText { Name = "JOB#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new PdfDocumentParser.Template.Field.PdfText { Name = "PO#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new PdfDocumentParser.Template.Field.PdfText { Name = "COST" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                    },
                        Name = "",
                        FloatingAnchors = new List<PdfDocumentParser.Template.FloatingAnchor>(),
                        DocumentFirstPageRecognitionMarks = new List<PdfDocumentParser.Template.Mark>(),
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

    public class Template
    {
        public PdfDocumentParser.Template Base;

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