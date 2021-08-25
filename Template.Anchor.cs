//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;

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
            public PointF Position;
            public int SearchRectangleMargin = -1;//px
            virtual public int? ParentAnchorId { get; set; } = null;

            public Anchor()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is OcrText)
                    Type = Types.OcrText;
                else if (this is ImageData)
                    Type = Types.ImageData;
                else if (this is CvImage)
                    Type = Types.CvImage;
                else
                    throw new Exception("Unknown option: " + this.GetType());
            }
            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;
            public enum Types
            {
                PdfText,
                CvImage,
                OcrText,
                ImageData,
            }

            abstract public bool IsSet();
            abstract public System.Drawing.RectangleF Rectangle();

            public class PdfText : Anchor
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
                public SizeF Size;
                public float PositionDeviation = 1f;
                public bool PositionDeviationIsAbsolute = false;
                public bool IgnoreInvisibleChars = true;
                public bool IgnoreOtherCharsInRectangle = true;

                override public bool IsSet()
                {
                    return /*CharBoxs.Count > 0 &&*/ Size != null;
                }

                override public System.Drawing.RectangleF Rectangle()
                {
                    return new System.Drawing.RectangleF(Position.X, Position.Y, Size.Width, Size.Height);
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
                public SizeF Size;
                public float PositionDeviation = 1f;
                public bool PositionDeviationIsAbsolute = false;
                public OcrSettings OcrSettings;//TBI

                override public bool IsSet()
                {
                    return /*CharBoxs.Count > 0 &&*/ Size != null;
                }

                override public System.Drawing.RectangleF Rectangle()
                {
                    return new System.Drawing.RectangleF(Position.X, Position.Y, Size.Width, Size.Height);
                }
            }

            public class OcrSettings
            {
                public Tesseract.PageSegMode TesseractPageSegMode = Tesseract.PageSegMode.SingleBlock;
            }

            public class CvImage : Anchor
            {
                public PdfDocumentParser.CvImage Image;

                public float Threshold = 0.70f;
                public float ScaleDeviation = 0.05f;//when automatic rescaling it is not needed
                public bool FindBestImageMatch = false;

                override public bool IsSet()
                {
                    return Image != null && Image.Width > 0 && Image.Height > 0;
                }

                override public System.Drawing.RectangleF Rectangle()
                {
                    return new System.Drawing.RectangleF(Position.X, Position.Y, Image.Width, Image.Height);
                }
            }

            public class ImageData : Anchor
            {
                public PdfDocumentParser.ImageData Image;

                public float BrightnessTolerance = 0.20f;
                public float DifferentPixelNumberTolerance = 0.15f;
                public bool FindBestImageMatch = false;

                override public bool IsSet()
                {
                    return Image != null && Image.Width > 0 && Image.Height > 0;
                }

                override public System.Drawing.RectangleF Rectangle()
                {
                    return new System.Drawing.RectangleF(Position.X, Position.Y, Image.Width, Image.Height);
                }
            }
        }
    }
}