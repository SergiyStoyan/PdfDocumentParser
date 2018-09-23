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
            public string TestFile;
            public decimal TestPictureScale = 1.2m;
        }
        public EditorSettings Editor;

        public string Name;

        //public int ImageResolution = 300;//tesseract requires at least 300

        public bool AutoPagesRotation = false;//!!!not implemented!!! to do by tesseract
        public PageRotations PagesRotation = 0;
        public enum PageRotations
        {
            NONE,
            Clockwise90,
            Clockwise180,
            Clockwise270,
        }

        public bool AutoDeskew = false;
        public int AutoDeskewThreshold = 100;

        public List<FloatingAnchor> FloatingAnchors;

        public List<Mark> DocumentFirstPageRecognitionMarks;

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

        public abstract class Mark
        {
            public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public RectangleF Rectangle;

            public Mark()
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
                return FloatingAnchorId != null || Rectangle != null;
            }

            public class PdfText : Mark
            {
                public string Text;
            }

            public class OcrText : Mark
            {
                public string Text;
            }

            public class ImageData : Mark
            {
                public PdfDocumentParser.ImageData ImageData_;
                public float BrightnessTolerance = 0.4f;
                public float DifferentPixelNumberTolerance = 0.02f;
                public bool FindBestImageMatch = false;
            }
        }

        public enum Types
        {
            PdfText,
            OcrText,
            ImageData
        }

        public abstract class FloatingAnchor
        {
            public int Id;
            public int SearchRectangleMargin = -1;//px
            public float PositionDeviation = 0.1f;
            public bool PositionDeviationIsAbsolute = true;

            public FloatingAnchor()
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

          virtual  public bool IsSet()
            {
                return Id > 0;
            }

            public static System.Drawing.RectangleF GetSearchRectangle(RectangleF rectangle0, int margin/*, System.Drawing.RectangleF pageRectangle*/)
            {
                System.Drawing.RectangleF r = new System.Drawing.RectangleF(rectangle0.X - margin, rectangle0.Y - margin, rectangle0.Width + 2 * margin, rectangle0.Height + 2 * margin);
                //r.Intersect(pageRectangle);
                return r;
            }

            public class PdfText : FloatingAnchor
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }

                override public bool IsSet()
                {
                    return base.IsSet() && CharBoxs.Count > 0;
                }
            }

            public class OcrText : FloatingAnchor
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }

                override public bool IsSet()
                {
                    return base.IsSet() && CharBoxs.Count > 0;
                }
            }

            public class ImageData : FloatingAnchor
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
                    return base.IsSet() && ImageBoxs.Count > 0;
                }
            }
        }

        public abstract class Field
        {
            public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public string Name;
            public RectangleF Rectangle;//when FloatingAnchor is set, Rectangle.X,Y are bound to location of the anchor as to zero point

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