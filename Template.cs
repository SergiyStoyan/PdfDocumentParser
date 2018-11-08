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
    public class Template
    {
        public class EditorSettings
        {
            public bool ExtractFieldsAutomaticallyWhenPageChanged = true;
            public bool CheckConditionsAutomaticallyWhenPageChanged = true;
            public string TestFile;
            public decimal TestPictureScale = 1.2m;
        }
        public EditorSettings Editor;

        public string Name;

        //public int ImageResolution = 300;//tesseract requires at least 300

        public PageRotations PageRotation = PageRotations.NONE;
        public enum PageRotations
        {
            NONE,
            Clockwise90,
            Clockwise180,
            Clockwise270,
            AutoDetection
        }

        public bool AutoDeskew = false;
        public int AutoDeskewThreshold = 100;

        public List<Anchor> Anchors;

        public List<Condition> Conditions;

        public List<Field> Fields;

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

            static public RectangleF GetFromSystemRectangleF(System.Drawing.RectangleF r)
            {
                return new RectangleF(r.X, r.Y, r.Width, r.Height);
            }

            public System.Drawing.Rectangle GetSystemRectangle()
            {
                return new System.Drawing.Rectangle((int)Math.Round(X, 0), (int)Math.Round(Y, 0), (int)Math.Round(Width, 0), (int)Math.Round(Height, 0));
            }

            public System.Drawing.RectangleF GetSystemRectangleF()
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

        public enum Types
        {
            PdfText,
            OcrText,
            ImageData
        }

        public abstract class Anchor
        {
            public int Id;
            public int SearchRectangleMargin = -1;//px
            public float PositionDeviation = 1f;
            public bool PositionDeviationIsAbsolute = false;
            public int? ParentAnchorId = null;
            //public string Condition = null;//to be removed in the next release

            public Anchor()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is OcrText)
                    Type = Types.OcrText;
                else if (this is ImageData)
                    Type = Types.ImageData;
                else
                    throw new Exception("Unknown type: " + this.GetType());
            }
            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;

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
        }

        public class Condition
        {
            public string Name;
            public string Value;

            public bool IsSet()
            {
                return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Value);
            }
        }

        public abstract class Field
        {
            public int? AnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public string Name;
            public RectangleF Rectangle;//when Anchor is set, Rectangle.X,Y are bound to location of the anchor as to zero point

            public Field()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is OcrText)
                    Type = Types.OcrText;
                else if (this is ImageData)
                    Type = Types.ImageData;
                else
                    throw new Exception("Unknown type: " + this.GetType());
            }
            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;

            public bool IsSet()
            {
                return Rectangle != null;
            }

            public class PdfText : Field
            {
            }

            public class OcrText : Field
            {
            }

            public class ImageData : Field
            {
            }
        }
    }
}