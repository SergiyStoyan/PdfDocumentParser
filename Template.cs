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
                    return GetValueAsString(ValueType, value);
                }
                set
                {
                    this.value = GetValueFromString(ValueType, value);
                    ////TEMPORARY: trasferring to a new version
                    //switch (ValueType)
                    //{
                    //    case ValueTypes.PdfText:
                    //        var ptv = (PdfTextValue)this.value;
                    //        if (ptv.Rectangle == null && this.Rectangle!=null)
                    //        {
                    //            ptv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    case ValueTypes.OcrText:
                    //        var otv = (OcrTextValue)this.value;
                    //        if (otv.Rectangle == null && this.Rectangle != null)
                    //        {
                    //            otv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    case ValueTypes.ImageData:
                    //        var idv = (ImageDataValue)this.value;
                    //        if (idv.Rectangle == null && this.Rectangle != null)
                    //        {
                    //            idv.Rectangle = this.Rectangle;
                    //            this.Rectangle = null;
                    //        }
                    //        break;
                    //    default:
                    //        throw new Exception("Unknown option: " + ValueType);
                    //}
                }
            }

            static public string GetValueAsString(ValueType valueType, object value)
            {
                if (value == null)
                    return null;
                switch (valueType)
                {
                    case ValueTypes.PdfText:
                        return SerializationRoutines.Json.Serialize(value, false);
                    case ValueTypes.OcrText:
                        return SerializationRoutines.Json.Serialize(value, false);
                    case ValueTypes.ImageData:
                        return SerializationRoutines.Json.Serialize(value, false);
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }

            static public object GetValueFromString(ValueTypes valueType, string value)
            {
                if (value == null)
                    return null;
                switch (valueType)
                {
                    case ValueTypes.PdfText:
                            return SerializationRoutines.Json.Deserialize<PdfTextValue>(value);
                    case ValueTypes.OcrText:
                            return SerializationRoutines.Json.Deserialize<OcrTextValue>(value);
                    case ValueTypes.ImageData:
                        return SerializationRoutines.Json.Deserialize<ImageDataValue>(value);
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }

            public object GetValue()
            {
                return value;
            }
            object value;

            public class PdfTextValue
            {
                //public RectangleF Rectangle;
                public string Text;
            }
            public class OcrTextValue
            {
                //public RectangleF Rectangle;
                public string Text;
            }
            public class ImageDataValue
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
            public float PositionDeviation = 0.1f;
            //serialize
            public string ValueAsString
            {
                get
                {
                    return GetValueAsString(ValueType, value);
                }
                set
                {
                    this.value = GetValueFromString(ValueType, value);
                    ////TEMPORARY: trasferring to a new version
                    //switch (ValueType)
                    //{
                    //    case ValueTypes.PdfText:
                    //        var ptv = (PdfTextValue)this.value;
                    //        if (ptv.PositionDeviation < this.PositionDeviation)
                    //        {
                    //            ptv.PositionDeviation = this.PositionDeviation;
                    //            this.PositionDeviation = -1;
                    //        }
                    //        break;
                    //case ValueTypes.OcrText:
                    //        var otv = (OcrTextValue)this.value;
                    //        if (otv.PositionDeviation < this.PositionDeviation)
                    //        {
                    //            otv.PositionDeviation = this.PositionDeviation;
                    //            this.PositionDeviation = -1;
                    //        }
                    //        break;
                    //    case ValueTypes.ImageData:
                    //        var idv = (ImageDataValue)this.value;
                    //        if (idv.PositionDeviation < this.PositionDeviation)
                    //        {
                    //            idv.PositionDeviation = this.PositionDeviation;
                    //            this.PositionDeviation = -1;
                    //        }
                    //        break;
                    //    default:
                    //        throw new Exception("Unknown option: " + ValueType);
                    //}
                }
            }

            static public string GetValueAsString(ValueTypes valueType, object value)
            {
                if (value == null)
                    return null;
                switch (valueType)
                {
                    case ValueTypes.PdfText:
                        return SerializationRoutines.Json.Serialize(value, false);
                    case ValueTypes.OcrText:
                        return SerializationRoutines.Json.Serialize(value, false);
                    case ValueTypes.ImageData:
                        //byte[] bs = SerializationRoutines.Binary.Serialize(value);//more compact
                        //s = SerializationRoutines.Json.Serialize(bs, false);
                        return SerializationRoutines.Json.Serialize(value, false);
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }

            static public object GetValueFromString(ValueTypes valueType, string value)
            {
                if (value == null)
                    return null;
                switch (valueType)
                {
                    case ValueTypes.PdfText:
                        return SerializationRoutines.Json.Deserialize<PdfTextValue>(value);
                    case ValueTypes.OcrText:
                        return SerializationRoutines.Json.Deserialize<OcrTextValue>(value);
                    case ValueTypes.ImageData:
                        //byte[] bs = SerializationRoutines.Json.Deserialize<byte[]>(value);
                        //this.value = SerializationRoutines.Binary.Deserialize<ImageDataValue>(bs);
                        return SerializationRoutines.Json.Deserialize<ImageDataValue>(value);
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }

            public object GetValue()
            {
                return value;
            }
            object value;

            public class PdfTextValue
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                //public float PositionDeviation = 0.1f;
                public bool PositionDeviationIsAbsolute = true;
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
            public class OcrTextValue
            {
                public List<CharBox> CharBoxs = new List<CharBox>();
                //public float PositionDeviation = 0.1f;
                //public bool PositionDeviationIsAbsolute = true;
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
            public class ImageDataValue
            {
                public List<ImageBox> ImageBoxs = new List<ImageBox>();
                //public float PositionDeviation = 0.1f;
                //public bool PositionDeviationIsAbsolute = true;
                public class ImageBox
                {
                    public ImageData ImageData;
                    public RectangleF Rectangle;
                }

                public float BrightnessTolerance = 0.4f;
                public float DifferentPixelNumberTolerance = 0.02f;
                public bool FindBestImageMatch = false;
            }
        }
    }
}