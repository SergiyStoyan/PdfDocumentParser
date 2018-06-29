using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Linq;
using System.Drawing;
using System.Collections.Specialized;

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly TemplatesSettings Templates;

        public class TemplatesSettings : Cliver.Settings
        {
            public List<Template> Templates = new List<Template>();//preserving order of matching: only the first match is to be applied

            internal const string InitialTemplate = @"{
""Name"":"""",
""FileFilterRegex"":{
  ""Pattern"": ""\\.pdf$"",
  ""Options"": 1
},
""InvoiceFirstPageRecognitionMarks"":"""",
""Active"": true,
""TestFile"":"""",
""Fields"":[
    {
        ""Name"":""JOB#"",
        ""Rectangle"":{
             ""X"": 245.0,
            ""Y"": 510.0,
            ""Width"": 190.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""PO#"",
        ""Rectangle"":{
            ""X"": 435.0,
            ""Y"": 510.0,
            ""Width"": 165.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""INVOICE#"",
        ""Rectangle"":{
            ""X"": 381.0,
            ""Y"": 723.0,
            ""Width"": 100.0,
            ""Height"": 12.0
        },
    },
    {
        ""Name"":""COST"",
        ""Rectangle"":{
            ""X"": 381.0,
            ""Y"": 723.0,
            ""Width"": 100.0,
            ""Height"": 12.0
        },
    },
]
            }";

            public override void Loaded()
            {
                if (Templates.Count < 1)
                    Templates.Add(SerializationRoutines.Json.Deserialize<Template>(InitialTemplate));
            }

            public override void Saving()
            {
            }
        }

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

            public List<Mark> InvoiceFirstPageRecognitionMarks;

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
                public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
                public string Value;
                public RectangleF Rectangle;
                public ValueTypes ValueType = ValueTypes.PdfText;
                //public byte[] ValueAsBytes
                //{
                //    get
                //    {
                //        if (value == null)
                //            return null;
                //        return SerializationRoutines.Binary.Serialize(value);
                //    }
                //    set
                //    {
                //        this.value = SerializationRoutines.Binary.Deserialize(value);
                //    }
                //}

                //public object GetValue()
                //{
                //    return value;
                //}
                //object value;
                //public string GetValueAsString()
                //{
                //    if (value == null)
                //        return null;
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
                //serialized
                public int Id;
                //serialized
                public ValueTypes ValueType = ValueTypes.PdfText;
                //serialized
                public byte[] ValueAsBytes
                {
                    get
                    {
                        if (value == null)
                            return null;
                        return value.GetAsBytes();
                    }
                    set
                    {
                        this.value = Value.GetFromBytes(value);
                    }
                }

                public Value GetValue()
                {
                    return value;
                }
                Value value;
                public string GetValueAsString()
                {
                    if (value == null)
                        return null;
                    return value.GetAsString();
                }
                public FloatingAnchor(int id, ValueTypes valueType, string valueAsString)
                {
                    Id = id;
                    ValueType = valueType;
                    if (valueAsString != null)
                        value = Value.GetFromString(ValueType, valueAsString);
                    else
                        value = null;
                }
                public FloatingAnchor()//!!!used only by serializer!!!
                { 
                }
                [Serializable]
                public class Value
                {
                    public byte[] GetAsBytes()
                    {
                        return SerializationRoutines.Binary.Serialize(this);
                    }
                    public string GetAsString()
                    {
                        return SerializationRoutines.Json.Serialize(this);
                    }
                    static public Value GetFromBytes(byte[] elementAsBytes)
                    {
                        return SerializationRoutines.Binary.Deserialize<Value>(elementAsBytes);
                    }
                    static public Value GetFromString(ValueTypes valueType, string elementAsString)
                    {
                        switch(valueType)
                        {
                            case ValueTypes.PdfText:
                                return SerializationRoutines.Json.Deserialize<PdfTextValue>(elementAsString);
                            case ValueTypes.OcrText:
                                return SerializationRoutines.Json.Deserialize<OcrTextValue>(elementAsString);
                            case ValueTypes.ImageData:
                                return SerializationRoutines.Json.Deserialize<ImageDataValue>(elementAsString);
                            default:
                                throw new Exception("Unknown option: " + valueType);
                        }
                    }
                }

                [Serializable]
                public class PdfTextValue : Value
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
                public class ImageDataValue : Value
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
                public class OcrTextValue : Value
                {
                    public List<TextBox> TextBoxs;
                    [Serializable]
                    public class TextBox
                    {
                        public string Text;
                        public RectangleF Rectangle;
                    }
                }
            }
        }
    }
}
