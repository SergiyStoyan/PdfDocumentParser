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
    //OLD FORMAT,TO BE REMOVED
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly Templates0Settings Templates0;

        public class Templates0Settings : Cliver.Settings
        {
            public Template0 InitialTemplate;
            public List<Template0> Templates = new List<Template0>();

            public override void Loaded()
            {
                //if (Templates.Count < 1)
                //    Templates.Add(CreateInitialTemplate());

                //TEMPORARY: conversion to new format
                //foreach (Template t in Templates.Where(x => x.Editor == null))
                //    t.Editor = new PdfDocumentParser.Template.EditorSettings
                //    {
                //        TestPictureScale = 1.2m,
                //        TestFile = "",
                //        ExtractFieldsAutomaticallyWhenPageChanged = true,
                //    };
            }

            public override void Saving()
            {
                Templates.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name));
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

            public Template0 CreateInitialTemplate()
            {
                if (InitialTemplate != null)
                {
                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate);
                    Template0 t = SerializationRoutines.Json.Deserialize<Template0>(ts);
                    t.Name = "";
                    return t;
                }
                return new Template0
                {
                    AutoDeskew = false,
                    Fields = new List<Template0.Field> {
                        new Template0.Field { Name = "INVOICE#" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                        new Template0.Field { Name = "JOB#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new Template0.Field { Name = "PO#", Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10) },
                        new Template0.Field { Name = "COST" , Rectangle=new PdfDocumentParser.Template.RectangleF(0,0,10,10)},
                    },
                    Name = "",
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    FloatingAnchors = new List<Template0.FloatingAnchor>(),
                    DocumentFirstPageRecognitionMarks = new List<Template0.Mark>(),
                    PagesRotation = PdfDocumentParser.Template.PageRotations.NONE,
                    Editor = new PdfDocumentParser.Template0.EditorSettings
                    {
                        TestPictureScale = 1.2m,
                        TestFile = "",
                        ExtractFieldsAutomaticallyWhenPageChanged = true,
                    },
                };
            }
        }
    }

    public class Template0 : PdfDocumentParser.Template0
    {
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