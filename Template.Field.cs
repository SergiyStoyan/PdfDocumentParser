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

            public class PdfText : Field
            {
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            public class PdfTextLines : Field
            {
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            public class PdfCharBoxs : Field
            {
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            public class OcrText : Field
            {
                public OcrSettings OcrSettings;
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            public class OcrSettings
            {
                public OcrModes OcrMode = OcrModes.SingleFieldFromFieldImage | OcrModes.ColumnFieldFromTableCharBoxs;
                
                public Ocr.Config TesseractConfig;

                internal string TesseractConfigKey
                {
                    get
                    {
                        if (tesseractConfigKey == null)
                            tesseractConfigKey = TesseractConfig?.ToStringByJson(false);
                        return tesseractConfigKey;
                    }
                }
                string tesseractConfigKey = null;
            }
            //internal Template.Field.TesseractConfig GetOcrSpecialParsingSettingsFromGUI()
            //{
            //    Template.Field.OcrModes ocrMode = 0;
            //    if (SingleFieldFromFieldImage.Checked)
            //        ocrMode |= Template.Field.OcrModes.SingleFieldFromFieldImage;
            //    if (ColumnFieldFromFieldImage.Checked)
            //        ocrMode |= Template.Field.OcrModes.ColumnFieldFromFieldImage;
            //    TextAutoInsertSpace textAutoInsertSpace = new TextAutoInsertSpace
            //    {
            //        Threshold = (float)textAutoInsertSpaceThreshold.Value,
            //        Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text)
            //    };
            //    Ocr.Config ocrConfig = new Ocr.Config { };
            //    return new Template.Field.TesseractConfig { OcrMode = ocrMode, TextAutoInsertSpace = textAutoInsertSpace, TesseractConfig = ocrConfig };
            //}

            public enum OcrModes
            {
                SingleFieldFromPageCharBoxs = 0b0000001,//default
                SingleFieldFromFieldImage = 0b0000010,
                //TableFieldFromPageCharBoxs = 0b00100,//default
                //TableFieldFromFieldImage = 0b0001000,
                ColumnFieldFromTableCharBoxs = 0b0010000,//default
                ColumnFieldFromFieldImage = 0b0100000,
            }

            public class OcrTextLines : Field
            {
                public OcrSettings OcrSettings;
                public TextAutoInsertSpace TextAutoInsertSpace;
            }

            public class OcrCharBoxs : Field
            {
                public OcrSettings OcrSettings;
                public TextAutoInsertSpace TextAutoInsertSpace;
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