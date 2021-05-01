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

namespace Cliver.SampleParser
{
    public partial class Settings
    {
        public static readonly Template2sSettings3 Template2s;

        public class Template2sSettings3 : Cliver.UserSettings
        {
            public List<Template2> Template2s = new List<Template2>();

            protected override void Loaded()
            {
                __Info.Indented = false;
            }

            protected override void Saving()
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

            protected override void Saved()
            {
                touched = false;
                TouchedChanged?.BeginInvoke(null, null);
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
                        Deskew = null,
                        Anchors = new List<Template.Anchor>(),
                        Conditions = new List<Template.Condition> {
                            new Template.Condition { Name = Template2.ConditionNames.DocumentFirstPage },
                            new Template.Condition { Name = Template2.ConditionNames.DocumentLastPage }
                        },
                        Fields = new List<Template.Field>
                        {
                            new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfText, Name = Template2.FieldNames.InvoiceId , Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfText, Name = Template2.FieldNames.TotalAmount, Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfText, Name = Template2.FieldNames.ProductTable, Rectangle=new Template.RectangleF(0,0,10,10)},
                            new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfTextLines, Name = Template2.FieldNames.ProductNames, Rectangle=new Template.RectangleF(0,0,10,10), ColumnOfTable=Template2.FieldNames.ProductTable},
                            new Template.Field { DefaultValueType = Template.Field.ValueTypes.PdfTextLines, Name = Template2.FieldNames.ProductCosts, Rectangle=new Template.RectangleF(0,0,10,10), ColumnOfTable=Template2.FieldNames.ProductTable},
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
            public const string InvoiceId = "InvoiceId";
            public const string TotalAmount = "TotalAmount";
            public const string ProductTable = "ProductTable";
            public const string ProductNames = "ProductNames";
            public const string ProductCosts = "ProductCosts";
        }

        public Template Template;

        public bool Active = true;
        public DateTime ModifiedTime;
        public string Comment;
        public float OrderWeight = 1f;
        public Regex FileFilterRegex = null;

        public string GetModifiedTimeAsString()
        {
            return ModifiedTime.ToString("yy-MM-dd HH:mm:ss");
        }
    }
}