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
            public List<Template> Templates = new List<Template>();//preserving order of matching: only the first match is to be applied

            public override void Loaded()
            {
                //if (Templates.Count < 1)
                //    Templates.Add(CreateInitialTemplate());

                //TEMPORARY: conversion to new format
                foreach (Template t in Templates.Where(x => x.Editor == null))
                    t.Editor = new PdfDocumentParser.Template.EditorSettings
                    {
                        TestPictureScale = 1.2m,
                        TestFile = "",
                        ExtractFieldsAutomaticallyWhenPageChanged = true,
                    };
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

            public Template CreateInitialTemplate()
            {
                if (InitialTemplate != null)
                {
                    string ts = SerializationRoutines.Json.Serialize(InitialTemplate);
                    Template t = SerializationRoutines.Json.Deserialize<Template>(ts);
                    t.Name = "";
                    return t;
                }
                return new Template
                {
                    AutoDeskew = false,
                    Fields = new List<Template.Field> {
                        new Template.Field { Name = "INVOICE#" , Rectangle=new Template.RectangleF(0,0,10,10)},
                        new Template.Field { Name = "JOB#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field { Name = "PO#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field { Name = "COST" , Rectangle=new Template.RectangleF(0,0,10,10)},
                    },
                    Name = "",
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    FloatingAnchors = new List<Template.FloatingAnchor>(),
                    DocumentFirstPageRecognitionMarks = new List<Template.Mark>(),
                    PagesRotation = Template.PageRotations.NONE,
                    Editor = new PdfDocumentParser.Template.EditorSettings
                    {
                        TestPictureScale = 1.2m,
                        TestFile = "",
                        ExtractFieldsAutomaticallyWhenPageChanged = true,
                    },
                };
            }
        }
    }

    public class Template : PdfDocumentParser.Template
    {
        public bool Active = true;
        public string Group;
        public DateTime ModifiedTime;
        public string Comment;
        public float OrderWeight = 1f;
        //public int PdfPageMinNumberToDetectTemplate = 3;
        public uint DetectingTemplateLastPageNumber = 1;
        public bool CanShareFileWithAnotherTemplates = false;

        public string GetModifiedTimeAsString()
        {
            return ModifiedTime.ToString("yy-MM-dd HH:mm:ss");
        }
    }
}
