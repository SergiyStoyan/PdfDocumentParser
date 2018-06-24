using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Linq;
using System.Drawing;

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly TemplatesSettings Templates;

        public class TemplatesSettings : Cliver.Settings
        {            
            public List<Template> Templates = new List<Template>();//preserving order of matching: only the first match is to be applied

            internal const string InitialTemplate = @"{
""Name"":"""",
""FileFilterRegex"":{
  ""Pattern"": ""\\.pdf$"",
  ""Options"": 1
},
""InvoiceFirstPageRecognitionMarks"":"""",
""Active"": true,
""TestFile"":"""",
""Fields"":[
    {
        ""Name"":""JOB#"",
        ""Rectangle"":{
             ""X"": 245.0,
            ""Y"": 510.0,
            ""Width"": 190.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""PO#"",
        ""Rectangle"":{
            ""X"": 435.0,
            ""Y"": 510.0,
            ""Width"": 165.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""INVOICE#"",
        ""Rectangle"":{
            ""X"": 381.0,
            ""Y"": 723.0,
            ""Width"": 100.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""COST"",
        ""Rectangle"":{
            ""X"": 381.0,
            ""Y"": 723.0,
            ""Width"": 100.0,
            ""Height"": 12.0
        },
    },
]
            }";

            public override void Loaded()
            {
                if (Templates.Count < 1)
                    Templates.Add(SerializationRoutines.Json.Deserialize<Template>(InitialTemplate));                
            }

            public override void Saving()
            {
            }
        }

        public partial class Template
        {
            public string Name;

            public Regex FileFilterRegex;

            //public int ImageResolution = 300;//tessarct requires at least 300

            public PageRotations PagesRotation = 0;
            public enum PageRotations
            {
                NONE,
                Clockwise90,
                Clockwise180,
                Clockwise270,
            }
            
            public ImageData Position0Anchor;//when set, all the absolute cooordinates in the template are counted from this point

            public bool AutoDeskew = false;

            public List<Mark> InvoiceFirstPageRecognitionMarks;

            public List<Field> Fields;//preserving order for output

            public bool Active = true;

            public string TestFile;
            public decimal TestPictureScale = 1.3m;

            public class RectangleF
            {
                public float X;
                public float Y;
                public float Width;
                public float Height;

                public RectangleF(float x, float y, float w, float h)
                {
                    X = x;
                    Y = y;
                    Width = w;
                    Height = h;
                }

                public System.Drawing.RectangleF Convert()
                {
                    return new System.Drawing.RectangleF(X, Y, Width, Height);
                }
            }
            public class PointF
            {
                public float X;
                public float Y;
            }
            public class SizeF
            {
                public float Width;
                public float Height;
            }

            public class Mark
            {
                public string Value;
                public RectangleF Rectangle;
                public ValueTypes ValueType = ValueTypes.PdfText;
            }
            public enum ValueTypes
            {
                PdfText,
                OcrText,
                ImageData
            }

            public class Field
            {
                public string Name;
                public RectangleF Rectangle;//when FloatingAnchor is set, Rectangle.X,Y are bound to location of the anchor as to zero point
                public bool Ocr = false;
                //public List<ImageMark> PageRecognitionImageMarks;//not used do far
                //public List<TextMark> PageRecognitionTextMarks;
                public FloatingAnchor FloatingAnchor;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
                public Regex Regex;//if set the rest settings are ignored; it is applied to the page text
            }
            public class FloatingAnchor
            {
                public List<Element> Elements = new List<Element>();//first Element has undefined X,Y, the rest Elements have X,Y related on the first's Element X,Y
                public class Element
                {
                    public String Text;
                    public RectangleF Rectangle;
                    //public Object Value;
                    //public ValueTypes ValueType = ValueTypes.PdfText;

                    //public object GetTyped()
                    //{
                    //    switch (ValueType)
                    //    {
                    //        case ValueTypes.ImageData:
                    //            return ImageData.GetFromString(Value);
                    //        case ValueTypes.OcrText:
                    //        case ValueTypes.PdfText:
                    //            return (TextElement)SerializationRoutines.Json.Deserialize<TextElement>(Value);
                    //        default:
                    //            throw new Exception("Unknown option: " + ValueType);
                    //    }
                    //}
                }

                //public System.Drawing.PointF? FindFloatingAnchor(List<BoxText> boxTexts, Bitmap page)
                //{
                //    List<BoxText> bts = new List<BoxText>();

                //    foreach (BoxText bt0 in boxTexts.Where(a => a.Text == fa.Elements[0].Text))
                //    {
                //        bts.Clear();
                //        bts.Add(bt0);
                //        for (int i = 1; i < fa.Elements.Count; i++)
                //        {
                //            float x = bt0.R.X + fa.Elements[i].Rectangle.X - fa.Elements[0].Rectangle.X;
                //            float y = bt0.R.Y + fa.Elements[i].Rectangle.Y - fa.Elements[0].Rectangle.Y;
                //            foreach (BoxText bt in boxTexts.Where(a => a.Text == fa.Elements[i].Text))
                //            {
                //                if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                //                    continue;
                //                if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                //                    continue;
                //                if (bts.Contains(bt))
                //                    continue;
                //                bts.Add(bt);
                //            }
                //        }
                //        if (bts.Count == fa.Elements.Count)
                //            return bts;
                //    }
                //    return null;
                //}

                //public List<BoxText> FindFloatingAnchor(List<BoxText> boxTexts, Bitmap page)
                //{
                //    List<BoxText> bts = new List<BoxText>();
                //    foreach (BoxText bt0 in boxTexts.Where(a => a.Text == fa.Elements[0].Text))
                //    {
                //        bts.Clear();
                //        bts.Add(bt0);
                //        for (int i = 1; i < fa.Elements.Count; i++)
                //        {
                //            float x = bt0.R.X + fa.Elements[i].Rectangle.X - fa.Elements[0].Rectangle.X;
                //            float y = bt0.R.Y + fa.Elements[i].Rectangle.Y - fa.Elements[0].Rectangle.Y;
                //            foreach (BoxText bt in boxTexts.Where(a => a.Text == fa.Elements[i].Text))
                //            {
                //                if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                //                    continue;
                //                if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                //                    continue;
                //                if (bts.Contains(bt))
                //                    continue;
                //                bts.Add(bt);
                //            }
                //        }
                //        if (bts.Count == fa.Elements.Count)
                //            return bts;
                //    }
                //    return null;
                //}
            }
            public class TextElement
            {
                public String Text;
                public RectangleF Rectangle;
            }
        }
    }
}
