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

        public float TextAutoInsertSpaceThreshold = 6;
        public string TextAutoInsertSpaceSubstitute = "\t";
        //public TextAutoInsertSpace TextAutoInsertSpace;
        //public struct TextAutoInsertSpace
        //{
        //    public float Threshold = 6;
        //    public string Substitute = "\t";
        //    public string SubstituteReplacement = " ";
        //}

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
            public string Name;

            public int? AnchorId//conversion from the old format; to be removed
            {
                set
                {
                    if (value == null)
                        return;
                    LeftAnchor = new SideAnchor { Id = (int)value };
                    TopAnchor = new SideAnchor { Id = (int)value };
                }
            }
            public int? LeftAnchorId//conversion from the old format; to be removed
            {
                set
                {
                    if (value == null)
                        return;
                    LeftAnchor = new SideAnchor { Id = (int)value };
                }
            }
            public int? TopAnchorId//conversion from the old format; to be removed
            {
                set
                {
                    if (value == null)
                        return;
                    TopAnchor = new SideAnchor { Id = (int)value };
                }
            }
            public int? RightAnchorId//conversion from the old format; to be removed
            {
                set
                {
                    if (value == null)
                        return;
                    RightAnchor = new SideAnchor { Id = (int)value };
                }
            }
            public int? BottomAnchorId//conversion from the old format; to be removed
            {
                set
                {
                    if (value == null)
                        return;
                    BottomAnchor = new SideAnchor { Id = (int)value };
                }
            }

            public RectangleF Rectangle;
            /// <summary>
            /// when set, Left shifts as the anchor shifts
            /// </summary>
            public SideAnchor LeftAnchor;
            /// <summary>
            /// when set, Top shifts as the anchor shifts
            /// </summary>
            public SideAnchor TopAnchor;
            /// <summary>
            /// when set, Right shifts as the anchor shifts
            /// </summary>
            public SideAnchor RightAnchor;
            /// <summary>
            /// when set, Bottom shifts as the anchor shifts
            /// </summary>
            public SideAnchor BottomAnchor;
            public class SideAnchor
            {
                public int Id;
                public float Shift = 0;
            }

            /// <summary>
            /// set by the custom application. When set, the field is considered as a table column. 
            /// This field is retrived and split on lines together with the rest columns 
            /// so that their lines match together. Thus every field contains the same number of lines. 
            /// </summary>
            //public string Table = null;

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
            public enum Types
            {
                PdfText,
                OcrText,
                ImageData,
            }

            public bool IsSet()
            {
                return Rectangle != null && Rectangle.Width > 0 && Rectangle.Height > 0;
            }

            public class PdfText : Field
            {
                public bool ValueAsCharBoxes = false;
            }

            public class OcrText : Field
            {
                public bool ValueAsCharBoxes = false;
            }

            public class ImageData : Field
            {
            }
        }
    }
}