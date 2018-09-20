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

        [Serializable]
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
        [Serializable]
        public class PointF
        {
            public float X;
            public float Y;
        }
        [Serializable]
        public class SizeF
        {
            public float Width;
            public float Height;
        }

        public class Mark
        {
            //serialize
            public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            //serialize
            public RectangleF Rectangle;
            //serialize
            public ValueTypes ValueType = ValueTypes.PdfText;
            //serialize
            public string ValueAsString
            {
                get
                {
                    if (Value == null)
                        return null;
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.OcrText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.ImageData:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }
                }
                set
                {
                    if (value == null)
                    {
                        Value = null;
                        return;
                    }
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            Value = SerializationRoutines.Json.Deserialize<PdfTextValue>(value);
                            break;
                        case ValueTypes.OcrText:
                            Value = SerializationRoutines.Json.Deserialize<OcrTextValue>(value);
                            break;
                        case ValueTypes.ImageData:
                            Value = SerializationRoutines.Json.Deserialize<ImageDataValue>(value);
                            break;
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }

                    ////TEMPORARY: trasferring to a new version
                    //switch (ValueType)
                    //{
                    //    case ValueTypes.PdfText:
                    //        if (this.Rectangle != null)
                    //        {
                    //            var ptv = (PdfTextValue)Value;
                    //            ptv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    case ValueTypes.OcrText:
                    //        if (this.Rectangle != null)
                    //        {
                    //            var otv = (OcrTextValue)Value;
                    //            otv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    case ValueTypes.ImageData:
                    //        if (this.Rectangle != null)
                    //        {
                    //            var idv = (ImageDataValue)Value;
                    //            idv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    default:
                    //        throw new Exception("Unknown option: " + ValueType);
                    //}
                }
            }
            
            //not to serialize!
            internal BaseValue Value { get; set; }

            public class BaseValue
            {
                //public RectangleF Rectangle
                //{
                //    get { return; }
                //}
                //RectangleF rectangle;
            }
            public class PdfTextValue: BaseValue
            {
                //public RectangleF Rectangle;
                public string Text;
            }
            public class OcrTextValue: BaseValue
            {
                //public RectangleF Rectangle;
                public string Text;
            }
            public class ImageDataValue: BaseValue
            {
                //public RectangleF Rectangle;
                public ImageData ImageData;
                public float BrightnessTolerance = 0.4f;
                public float DifferentPixelNumberTolerance = 0.02f;
                public bool FindBestImageMatch = false;
            }
        }

        public enum ValueTypes
        {
            PdfText,
            OcrText,
            ImageData
        }

        public class Field
        {
            public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public string Name;
            public RectangleF Rectangle;//when FloatingAnchor is set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public ValueTypes ValueType = ValueTypes.PdfText;
        }
        
        public partial class FloatingAnchor
        {
            //serialize
            public int Id;
            //serialize
            public ValueTypes ValueType = ValueTypes.PdfText;
            //serialize
            public float PositionDeviation = 0.1f;//to be removed after update
            //serialize
            public string ValueAsString
            {
                get
                {
                    if (Value == null)
                        return null;
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.OcrText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.ImageData:
                            //byte[] bs = SerializationRoutines.Binary.Serialize(value);//more compact
                            //s = SerializationRoutines.Json.Serialize(bs, false);
                            return SerializationRoutines.Json.Serialize(Value, false);
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }
                }
                set
                {
                    if (value == null)
                    {
                        Value = null;
                        return;
                    }
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            Value = SerializationRoutines.Json.Deserialize<PdfTextValue>(value);
                            break;
                        case ValueTypes.OcrText:
                            Value = SerializationRoutines.Json.Deserialize<OcrTextValue>(value);
                            break;
                        case ValueTypes.ImageData:
                            //byte[] bs = SerializationRoutines.Json.Deserialize<byte[]>(value);
                            //this.value = SerializationRoutines.Binary.Deserialize<ImageDataValue>(bs);
                            Value = SerializationRoutines.Json.Deserialize<ImageDataValue>(value);
                            break;
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }

                    //TEMPORARY: transferring to a new version
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            var ptv = (PdfTextValue)this.Value;
                            if (this.PositionDeviation > 0)
                                ptv.PositionDeviation = this.PositionDeviation;
                            break;
                        case ValueTypes.OcrText:
                            var otv = (OcrTextValue)this.Value;
                            if (this.PositionDeviation > 0)
                                otv.PositionDeviation = this.PositionDeviation;
                            break;
                        case ValueTypes.ImageData:
                            var idv = (ImageDataValue)this.Value;
                            if (this.PositionDeviation > 0)
                                idv.PositionDeviation = this.PositionDeviation;
                            break;
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }
                    this.PositionDeviation = -1;
                }
            }

            //not to serialize!
            internal BaseValue Value { get; set; }

            public abstract class BaseValue
            {
                public int SearchRectangleMargin = -1;//px
                public float PositionDeviation = 0.1f;
                public bool PositionDeviationIsAbsolute = true;

                public static System.Drawing.RectangleF GetSearchRectangle(RectangleF rectangle0, int margin/*, System.Drawing.RectangleF pageRectangle*/)
                {
                    System.Drawing.RectangleF r = new System.Drawing.RectangleF(rectangle0.X - margin, rectangle0.Y - margin, rectangle0.Width + 2 * margin, rectangle0.Height + 2 * margin);
                    //r.Intersect(pageRectangle);
                    return r;
                }
            }
            public class PdfTextValue: BaseValue
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
            public class OcrTextValue: BaseValue
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
            public class ImageDataValue: BaseValue
            {
                public List<ImageBox> ImageBoxs = new List<ImageBox>();
                public class ImageBox
                {
                    public ImageData ImageData;
                    public RectangleF Rectangle;
                }

                public float BrightnessTolerance = 0.20f;
                public float DifferentPixelNumberTolerance = 0.15f;
                public bool FindBestImageMatch = false;
            }
        }
    }
}