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
                __Indented = false;
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
                    Template2 t = SerializationRoutines.Json.Clone(InitialTemplate2);
                    t.Template.Name = "";
                    return t;
                }
                return new Template2
                {
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    //SharedFileTemplateNamesRegex = new Regex(@"\@", RegexOptions.IgnoreCase),
                    Template = new Template
                    {
                        AutoDeskew = false,
                        Fields = new List<Template.Field> {
                        new Template.Field.PdfText { Name = "INVOICE#" , Rectangle=new Template.RectangleF(0,0,10,10)},
                        new Template.Field.PdfText { Name = "JOB#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field.PdfText { Name = "PO#", Rectangle=new Template.RectangleF(0,0,10,10) },
                        new Template.Field.PdfText { Name = "COST" , Rectangle=new Template.RectangleF(0,0,10,10)},
                    },
                        Name = "",
                        Anchors = new List<Template.Anchor>(),
                        Marks = new List<Template.Mark>(),
                        PageRotation = PdfDocumentParser.Template.PageRotations.NONE,
                        Editor = new Template.EditorSettings
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
        public Template Template;

        public bool Active = true;
        public string Group;
        public DateTime ModifiedTime;
        public string Comment;
        public float OrderWeight = 1f;
        //public int PdfPageMinNumberToDetectTemplate = 3;
        public uint DetectingTemplateLastPageNumber = 1;
        public Regex FileFilterRegex = null;
        public Regex SharedFileTemplateNamesRegex = null;

        public string GetModifiedTimeAsString()
        {
            return ModifiedTime.ToString("yy-MM-dd HH:mm:ss");
        }
    }
}