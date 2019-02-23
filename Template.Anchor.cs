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

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// parsing rules corresponding to certain pdf document layout
    /// </summary>
    public partial class Template
    {
        public abstract class Anchor
        {
            public int Id;
            public int SearchRectangleMargin = -1;//px
            public float PositionDeviation = 1f;
            public bool PositionDeviationIsAbsolute = false;
            virtual public int? ParentAnchorId { get; set; } = null;

            public Anchor()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is OcrText)
                    Type = Types.OcrText;
                else if (this is ImageData)
                    Type = Types.ImageData;
                //else if (this is Script)
                //    Type = Types.Script;
                else
                    throw new Exception("Unknown type: " + this.GetType());
            }
            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;
            public enum Types
            {
                PdfText,
                OcrText,
                ImageData,
                //Script
            }

            abstract public bool IsSet();

            abstract public System.Drawing.RectangleF MainElementInitialRectangle();

            public class PdfText : Anchor
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
                //public List<TextBox> TextBoxs = new List<TextBox>();
                //public class TextBox
                //{
                //    public List<CharBox> CharBoxs = new List<CharBox>();
                //    public RectangleF Rectangle;
                //}

                override public bool IsSet()
                {
                    return CharBoxs.Count > 0;
                }

                override public System.Drawing.RectangleF MainElementInitialRectangle()
                {
                    if (CharBoxs == null || CharBoxs.Count < 1)
                        return new System.Drawing.RectangleF();
                    return CharBoxs[0].Rectangle.GetSystemRectangleF();
                }
            }

            public class OcrText : Anchor
            {
                public bool OcrEntirePage = false;//Tesseract recongnition of a big fragment and a small one gives different results!
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }                
                //public List<TextBox> TextBoxs = new List<TextBox>();
                //public class TextBox
                //{
                //    public List<CharBox> CharBoxs = new List<CharBox>();
                //    public RectangleF Rectangle;
                //}

                override public bool IsSet()
                {
                    return CharBoxs.Count > 0;
                }

                override public System.Drawing.RectangleF MainElementInitialRectangle()
                {
                    if (CharBoxs == null || CharBoxs.Count < 1)
                        return new System.Drawing.RectangleF();
                    return CharBoxs[0].Rectangle.GetSystemRectangleF();
                }
            }

            public class ImageData : Anchor
            {
                public List<ImageBox> ImageBoxs = new List<ImageBox>();
                public class ImageBox
                {
                    public PdfDocumentParser.ImageData ImageData;
                    public RectangleF Rectangle;
                }

                public float BrightnessTolerance = 0.20f;
                public float DifferentPixelNumberTolerance = 0.15f;
                public bool FindBestImageMatch = false;

                override public bool IsSet()
                {
                    return ImageBoxs.Count > 0;
                }

                override public System.Drawing.RectangleF MainElementInitialRectangle()
                {
                    if (ImageBoxs == null || ImageBoxs.Count < 1)
                        return new System.Drawing.RectangleF();
                    return ImageBoxs[0].Rectangle.GetSystemRectangleF();
                }
            }

            //public class Script : Anchor
            //{
            //    override public int? ParentAnchorId
            //    {
            //        get { return null; }
            //        set { if (value != null) throw new Exception("Should not be used in this type Anchor!"); }
            //    }

            //    public string Expression = null;

            //    override public bool IsSet()
            //    {
            //        return !string.IsNullOrWhiteSpace(Expression);
            //    }

            //    override public System.Drawing.RectangleF MainElementInitialRectangle()
            //    {
            //        throw new Exception("Should not be called in this type Anchor!");
            //    }
            //}
        }
    }
}