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
    /// //OLD FORMAT,TO BE REMOVED
    public class Template0
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
        public Template.PageRotations PagesRotation = 0;

        public bool AutoDeskew = false;
        public int AutoDeskewThreshold = 100;

        public List<FloatingAnchor> FloatingAnchors;

        public List<Mark> DocumentFirstPageRecognitionMarks;

        public List<Field> Fields;

        public class Mark
        {
            //serialize
            public int? FloatingAnchorId;//when set, Rectangle.X,Y are bound to location of the anchor as to zero point
            //serialize
            public Template.RectangleF Rectangle;//to be removed in next update
            //serialize
            public ValueTypes Type = ValueTypes.PdfText;
            //serialize
            public string ValueAsString
            {
                get
                {
                    if (Value == null)
                        return null;
                    switch (Type)
                    {
                        case ValueTypes.PdfText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.OcrText:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        case ValueTypes.ImageData:
                            return SerializationRoutines.Json.Serialize(Value, false);
                        default:
                            throw new Exception("Unknown option: " + Type);
                    }
                }
                set
                {
                    if (value == null)
                    {
                        Value = null;
                        return;
                    }
                    switch (Type)
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
                            throw new Exception("Unknown option: " + Type);
                    }

                    ////TEMPORARY: trasferring to a new version
                    //switch (Type)
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
                    //        throw new Exception("Unknown option: " + Type);
                    //}
                }
            }

            [Newtonsoft.Json.JsonIgnore]
            public BaseValue Value { get; set; }

            public abstract class BaseValue
            {
                //public RectangleF Rectangle;
            }
            public class PdfTextValue: BaseValue
            {
                public string Text;
            }
            public class OcrTextValue: BaseValue
            {
                public string Text;
            }
            public class ImageDataValue: BaseValue
            {
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
            public Template.RectangleF Rectangle;//when FloatingAnchor is set, Rectangle.X,Y are bound to location of the anchor as to zero point
            public ValueTypes Type = ValueTypes.PdfText;
        }
        
        public partial class FloatingAnchor
        {
            //serialize
            public int Id;
            //serialize
            public ValueTypes Type = ValueTypes.PdfText;
            //serialize
            public float PositionDeviation = 0.1f;//to be removed after update
            //serialize
            public string ValueAsString
            {
                get
                {
                    if (Value == null)
                        return null;
                    switch (Type)
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
                            throw new Exception("Unknown option: " + Type);
                    }
                }
                set
                {
                    if (value == null)
                    {
                        Value = null;
                        return;
                    }
                    switch (Type)
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
                            throw new Exception("Unknown option: " + Type);
                    }
                }
            }

            [Newtonsoft.Json.JsonIgnore]
            public BaseValue Value { get; set; }

            public abstract class BaseValue
            {
                public int SearchRectangleMargin = -1;//px
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
                public List<Template.FloatingAnchor.PdfText.CharBox> CharBoxs = new List<Template.FloatingAnchor.PdfText.CharBox>();
            }
            public class OcrTextValue: BaseValue
            {
                public List<Template.FloatingAnchor.OcrText.CharBox> CharBoxs = new List<Template.FloatingAnchor.OcrText.CharBox>();
            }
            public class ImageDataValue: BaseValue
            {
                public List<Template.FloatingAnchor.ImageData.ImageBox> ImageBoxs = new List<Template.FloatingAnchor.ImageData.ImageBox>();

                public float BrightnessTolerance = 0.20f;
                public float DifferentPixelNumberTolerance = 0.15f;
                public bool FindBestImageMatch = false;
            }
        }
    }
}