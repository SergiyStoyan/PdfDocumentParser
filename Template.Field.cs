//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
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
        //public IEnumerable<Field> GetFieldDefinitions(string fieldName)
        //{
        //    return Fields.Where(x => x.Name == fieldName);
        //}

        abstract public class Field
        {
            public string Name;

            //public List<Definition> Definitions = new List<Definition>();

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

            public class SideAnchor //: Definition.SideAnchor
            {
                public int Id;
                public float Shift;// { get { return 0; } set { } }
            }

            //public class Definition
            //{
            //    public RectangleF Rectangle;
            //    /// <summary>
            //    /// when set, Left shifts as the anchor shifts
            //    /// </summary>
            //    public SideAnchor LeftAnchor;
            //    /// <summary>
            //    /// when set, Top shifts as the anchor shifts
            //    /// </summary>
            //    public SideAnchor TopAnchor;
            //    /// <summary>
            //    /// when set, Right shifts as the anchor shifts
            //    /// </summary>
            //    public SideAnchor RightAnchor;
            //    /// <summary>
            //    /// when set, Bottom shifts as the anchor shifts
            //    /// </summary>
            //    public SideAnchor BottomAnchor;

            //    public class SideAnchor
            //    {
            //        public int Id;
            //        public float Shift;// { get { return 0; } set { } }
            //    }
            //}

            public Field()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is PdfTextLines)
                    Type = Types.PdfTextLines;
                else if (this is PdfCharBoxs)
                    Type = Types.PdfCharBoxs;
                else if (this is OcrText)
                    Type = Types.OcrText;
                else if (this is OcrTextLines)
                    Type = Types.OcrTextLines;
                else if (this is OcrCharBoxs)
                    Type = Types.OcrCharBoxs;
                else if (this is Image)
                    Type = Types.Image;
                else if (this is OcrTextLineImages)
                    Type = Types.OcrTextLineImages;
                else
                    throw new Exception("Unknown option: " + this.GetType());
            }

            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;
            public enum Types
            {
                PdfText,
                PdfTextLines,
                PdfCharBoxs,
                OcrText,
                OcrTextLines,
                OcrCharBoxs,
                Image,
                OcrTextLineImages,
            }

            public bool IsSet()
            {
                return Rectangle != null && Rectangle.Width > 0 && Rectangle.Height > 0;
            }

            /// <summary>
            /// set by the custom application. When set, the field is considered as a table column intercepted with the field specified by ColumnOf. 
            /// In this way, all the fields which are columns of the same field always contain the same number of lines. 
            /// Not all field types supported!
            /// </summary>
            public string ColumnOfTable = null;

            //virtual internal object GetValue()
            //{
            //    return null;
            //}

            public interface Text
            {
            }

            public interface TextLines
            {
            }

            public interface CharBoxs
            {
            }

            abstract public class Pdf : Field
            {
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            internal TextAutoInsertSpace GetTextAutoInsertSpace(Template t)
            {
                Field.Pdf f = this as Template.Field.Pdf;
                return f?.TextAutoInsertSpace != null ? f.TextAutoInsertSpace : t.TextAutoInsertSpace;
            }

            public class PdfText : Pdf, Text
            {
            }

            public class PdfTextLines : Pdf, TextLines
            {
            }

            public class PdfCharBoxs : Pdf, CharBoxs
            {
            }

            abstract public class Ocr : Field
            {
                public OcrSettings OcrSettings;
                //public TextAutoInsertSpace TextAutoInsertSpace;
            }

            internal OcrSettings GetOcrSettings(Template t)
            {
                Field.Ocr f = this as Template.Field.Ocr;
                return f?.OcrSettings != null ? f.OcrSettings : t.OcrSettings;
            }

            public class OcrSettings
            {
                public OcrModes Mode;
                public Tesseract.PageSegMode TesseractPageSegMode = Tesseract.PageSegMode.SingleBlock;
                public CharFilter CharFilter;

                internal bool AdjustColumnCellBorders
                {
                    get { return Mode.HasFlag(OcrModes.AdjustColumnCellBorders); }
                    set
                    {
                        if (value) Mode |= OcrModes.AdjustColumnCellBorders;
                        else Mode &= ~OcrModes.AdjustColumnCellBorders;
                    }
                }
                internal bool SingleFieldFromFieldImage
                {
                    get { return Mode.HasFlag(OcrModes.SingleFieldFromFieldImage); }
                    set
                    {
                        if (value) Mode |= OcrModes.SingleFieldFromFieldImage;
                        else Mode &= ~OcrModes.SingleFieldFromFieldImage;
                    }
                }
                internal bool ColumnCellFromCellImage
                {
                    get { return Mode.HasFlag(OcrModes.ColumnCellFromCellImage); }
                    set
                    {
                        if (value) Mode |= OcrModes.ColumnCellFromCellImage;
                        else Mode &= ~OcrModes.ColumnCellFromCellImage;
                    }
                }
            }

            public enum OcrModes
            {
                AdjustColumnCellBorders = 0b0000001,
                //SingleFieldFromPageCharBoxs = 0b0000001,//default
                SingleFieldFromFieldImage = 0b0000010,
                //TableFieldFromPageCharBoxs = 0b00100,//default
                //TableFieldFromFieldImage = 0b0001000,
                //ColumnCellFromTableCharBoxs = 0b0010000,//default
                ColumnCellFromCellImage = 0b0100000,
            }

            public class OcrText : Ocr, Text
            {
            }

            public class OcrTextLines : Ocr, TextLines
            {
            }

            public class OcrCharBoxs : Ocr, CharBoxs
            {
            }

            public class Image : Field
            {
            }

            public class OcrTextLineImages : Field
            {
            }
        }
    }
}