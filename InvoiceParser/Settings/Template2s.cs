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
        public static readonly Template2sSettings2 Template2s;

        public class Template2sSettings2 : Cliver.Settings
        {
            public List<Template2> Template2s = new List<Template2>();

            public override void Loaded()
            {
                __Indented = false;

                //to covert from the older format
                foreach (Template2 t in Template2s)
                    if (t.Template.Conditions == null || t.Template.Conditions.Count < 1)
                    {
                        Dictionary<string, List<string>> cns2ce = new Dictionary<string, List<string>>();
                        foreach (Template.Anchor a in t.Template.Anchors.Where(x => !string.IsNullOrWhiteSpace(x.Condition)))
                        {
                            List<string> ce;
                            if (!cns2ce.TryGetValue(a.Condition, out ce))
                            {
                                ce = new List<string>();
                                cns2ce[a.Condition] = ce;
                            }
                            ce.Add(a.Id.ToString());

                        }
                        t.Template.Conditions = new List<Template.Condition>();
                        foreach (string cn in cns2ce.Keys)
                            t.Template.Conditions.Add(new Template.Condition { Name = cn, Value = string.Join(" & ", cns2ce[cn]) });
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
                return new Template2
                {
                    FileFilterRegex = new Regex(@"\.pdf$", RegexOptions.IgnoreCase),
                    Template = new Template
                    {
                        Name = "",
                        PageRotation = PdfDocumentParser.Template.PageRotations.NONE,
                        AutoDeskew = false,
                        Anchors = new List<Template.Anchor>(),
                        Conditions = new List<Template.Condition> { new Template.Condition { Name = Template2.ConditionNames.DocumentFirstPage } },
                        Fields = new List<Template.Field>
                        {
                            new Template.Field.PdfText { Name =Template2.FieldNames.INVOICE , Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field.PdfText { Name = Template2.FieldNames.JOB, Rectangle=new Template.RectangleF(0,0,10,10) },
                            new Template.Field.PdfText { Name = Template2.FieldNames.PO, Rectangle=new Template.RectangleF(0,0,10,10) },
                            new Template.Field.PdfText { Name = Template2.FieldNames.COST, Rectangle=new Template.RectangleF(0,0,10,10)},
                        },
                        Editor = new Template.EditorSettings
                        {
                            TestPictureScale = 1.2m,
                            TestFile = "",
                            CheckConditionsAutomaticallyWhenPageChanged = true,
                            ExtractFieldsAutomaticallyWhenPageChanged = true,
                        },
                    },
                };
            }
        }
    }

    public class Template2
    {
        public class ConditionNames
        {
            public const string DocumentFirstPage = "FirstPageOfDocument";
            public const string DocumentLastPage = "LastPageOfDocument";
        }

        public class FieldNames
        {
            public const string INVOICE = "INVOICE#";
            public const string JOB = "JOB#";
            public const string PO = "PO#";
            public const string COST = "COST";
        }

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