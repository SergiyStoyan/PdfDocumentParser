//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
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
        abstract public class Field
        {
            public string Name;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            public RectangleF Rectangle;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            /// <summary>
            /// when set, Left shifts as the anchor shifts
            /// </summary>
            public SideAnchor LeftAnchor;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            /// <summary>
            /// when set, Top shifts as the anchor shifts
            /// </summary>
            public SideAnchor TopAnchor;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            /// <summary>
            /// when set, Right shifts as the anchor shifts
            /// </summary>
            public SideAnchor RightAnchor;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            /// <summary>
            /// when set, Bottom shifts as the anchor shifts
            /// </summary>
            public SideAnchor BottomAnchor;

            public class SideAnchor //: Definition.SideAnchor
            {
                public int Id;
                public float Shift;// { get { return 0; } set { } }
            }

            public Field()
            {
                if (this is PdfText)
                    Type = Types.PdfText;
                else if (this is PdfTextLines)
                    Type = Types.PdfTextLines;
                else if (this is PdfCharBoxs)
                    Type = Types.PdfCharBoxs;
                else if (this is OcrText2)
                    Type = Types.OcrText2;
                else if (this is OcrTextLines2)
                    Type = Types.OcrTextLines2;
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

            /// <summary>
            /// The idea is that it must possible to get value of any type independently on the field's type.
            /// 1)it is necessary due to internal needs (at least value type must be independent on the field's type);
            /// 2)it is allowed for custom code;
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public readonly Types Type;//can be replaced with ValueType. It was declined because it will narrow possibilites while neither adding advantages nor simplifying the code.
            public enum Types
            {
                PdfText,
                PdfTextLines,
                PdfCharBoxs,
                /// <summary>
                /// PaddleOCRSharp-based
                /// </summary>
                OcrText2,
                /// <summary>
                /// PaddleOCRSharp-based
                /// </summary>
                OcrTextLines2,
                /// <summary>
                /// TesseractOCR-based
                /// </summary>
                OcrText,
                /// <summary>
                /// TesseractOCR-based
                /// </summary>
                OcrTextLines,
                /// <summary>
                /// TesseractOCR-based
                /// </summary>
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

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            public CharFilter CharFilter = null;

            public int? LinePaddingY = null;

            [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            public TextAutoInsertSpace TextAutoInsertSpace = null;

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

            public enum OcrModes
            {
                AdjustLineBorders = 0b0000001,
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

            /// <summary>
            /// TesseractOCR
            /// </summary>
            abstract public class Ocr : Field
            {
                public TesseractOCR.Enums.PageSegMode? TesseractPageSegMode = null;
                public OcrModes? Mode = null;

                internal bool? AdjustLineBorders
                {
                    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.AdjustLineBorders) : (bool?)null; }
                    set
                    {
                        if (value == null)
                        {
                            Mode = null;
                            return;
                        }
                        if (Mode == null)
                            Mode = 0;
                        Mode = value.Value ? Mode.Value | OcrModes.AdjustLineBorders : Mode.Value & ~OcrModes.AdjustLineBorders;
                    }
                }
                internal bool? SingleFieldFromFieldImage
                {
                    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.SingleFieldFromFieldImage) : (bool?)null; }
                    set
                    {
                        if (value == null)
                        {
                            Mode = null;
                            return;
                        }
                        if (Mode == null)
                            Mode = 0;
                        Mode = value.Value ? Mode.Value | OcrModes.SingleFieldFromFieldImage : Mode.Value & ~OcrModes.SingleFieldFromFieldImage;
                    }
                }
                internal bool? ColumnCellFromCellImage
                {
                    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.ColumnCellFromCellImage) : (bool?)null; }
                    set
                    {
                        if (value == null)
                        {
                            Mode = null;
                            return;
                        }
                        if (Mode == null)
                            Mode = 0;
                        Mode = value.Value ? Mode.Value | OcrModes.ColumnCellFromCellImage : Mode.Value & ~OcrModes.ColumnCellFromCellImage;
                    }
                }
            }

            public class OcrText2 : Ocr2, Text
            {
            }

            public class OcrTextLines2 : Ocr2, TextLines
            {
            }

            /// <summary>
            /// PaddleOCRSharp
            /// </summary>
            abstract public class Ocr2 : Field
            {
                public int? CropPadding = null;
                public OcrModes? Mode = null;

                internal bool? AdjustLineBorders
                {
                    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.AdjustLineBorders) : (bool?)null; }
                    set
                    {
                        if (value == null)
                        {
                            Mode = null;
                            return;
                        }
                        if (Mode == null)
                            Mode = 0;
                        Mode = value.Value ? Mode.Value | OcrModes.AdjustLineBorders : Mode.Value & ~OcrModes.AdjustLineBorders;
                    }
                }
                //internal bool? SingleFieldFromFieldImage
                //{
                //    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.SingleFieldFromFieldImage) : (bool?)null; }
                //    set
                //    {
                //        if (value == null)
                //        {
                //            Mode = null;
                //            return;
                //        }
                //        if (Mode == null)
                //            Mode = 0;
                //        Mode = value.Value ? Mode.Value | OcrModes.SingleFieldFromFieldImage : Mode.Value & ~OcrModes.SingleFieldFromFieldImage;
                //    }
                //}
                //internal bool? ColumnCellFromCellImage
                //{
                //    get { return Mode != null ? Mode.Value.HasFlag(OcrModes.ColumnCellFromCellImage) : (bool?)null; }
                //    set
                //    {
                //        if (value == null)
                //        {
                //            Mode = null;
                //            return;
                //        }
                //        if (Mode == null)
                //            Mode = 0;
                //        Mode = value.Value ? Mode.Value | OcrModes.ColumnCellFromCellImage : Mode.Value & ~OcrModes.ColumnCellFromCellImage;
                //    }
                //}
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