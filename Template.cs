using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Linq;
using System.Drawing;
using System.Collections.Specialized;

namespace Cliver.PdfDocumentParser
{
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

        public bool AutoDeskew = false;

        public float BrightnessTolerance = 0.4f;
        public float DifferentPixelNumberTolerance = 0.01f;
        public bool FindBestImageMatch = false;

        public List<FloatingAnchor> FloatingAnchors;

        public List<Mark> DocumentFirstPageRecognitionMarks;

        public List<Field> Fields;//preserving order for output

        public bool Active = true;

        public string TestFile;
        public decimal TestPictureScale = 1.3m;

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
                    if (value == null)
                        return null;
                    switch (ValueType)
                    {
                        case ValueTypes.PdfText:
                            return (string)value;
                        case ValueTypes.OcrText:
                            return (string)value;
                        case ValueTypes.ImageData:
                            return SerializationRoutines.Json.Serialize(value, false);
                        default:
                            throw new Exception("Unknown option: " + ValueType);
                    }
                }
                set
                {
                    if (value != null)
                        switch (ValueType)
                        {
                            case ValueTypes.PdfText:
                                this.value = value;
                                break;
                            case ValueTypes.OcrText:
                                this.value = value;
                                break;
                            case ValueTypes.ImageData:
                                this.value = SerializationRoutines.Json.Deserialize<ImageData>(value);
                                break;
                            default:
                                throw new Exception("Unknown option: " + ValueType);
                        }
                    else
                        value = null;
                }
            }

            public object GetValue()
            {
                return value;
            }
            object value;

            //public string GetValueAsString()
            //{
            //    if (value == null)
            //        return null;
            //    if (value is string)
            //        return (string)value;
            //    return SerializationRoutines.Json.Serialize(value);
            //}
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
            //public Regex Regex;//if set the rest settings are ignored; it is applied to the page text
        }

        public partial class FloatingAnchor
        {
            //serialize
            public int Id;
            //serialize
            public ValueTypes ValueType = ValueTypes.PdfText;
            //serialize
            public string ValueAsString
            {
                get
                {
                    if (value == null)
                        return null;
                    switch (ValueType)
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
                            throw new Exception("Unknown option: " + ValueType);
                    }
                }
                set
                {
                    if (value != null)
                        switch (ValueType)
                        {
                            case ValueTypes.PdfText:
                                this.value = SerializationRoutines.Json.Deserialize<PdfTextValue>(value);
                                return;
                            case ValueTypes.OcrText:
                                this.value = SerializationRoutines.Json.Deserialize<OcrTextValue>(value);
                                return;
                            case ValueTypes.ImageData:
                                //byte[] bs = SerializationRoutines.Json.Deserialize<byte[]>(value);
                                //this.value = SerializationRoutines.Binary.Deserialize<ImageDataValue>(bs);
                                this.value = SerializationRoutines.Json.Deserialize<ImageDataValue>(value);
                                return;
                            default:
                                throw new Exception("Unknown option: " + ValueType);
                        }
                    else
                        value = null;
                }
            }

            static public string GetValueAsString(object value)
            {
                if (value == null)
                    return null;
                return SerializationRoutines.Json.Serialize(value, false);
            }

            //public string GetValueAsString()
            //{
            //    return FloatingAnchor.GetValueAsString(value);
            //}

            public object GetValue()
            {
                return value;
            }
            object value;

            [Serializable]
            public class PdfTextValue
            {
                public List<CharBox> CharBoxs;
                [Serializable]
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
            [Serializable]
            public class ImageDataValue
            {
                public List<ImageBox> ImageBoxs;
                [Serializable]
                public class ImageBox
                {
                    public ImageData ImageData;
                    public RectangleF Rectangle;
                }
            }
            [Serializable]
            public class OcrTextValue
            {
                public List<CharBox> CharBoxs;
                [Serializable]
                public class CharBox
                {
                    public string Char;
                    public RectangleF Rectangle;
                }
            }
        }
    }
}