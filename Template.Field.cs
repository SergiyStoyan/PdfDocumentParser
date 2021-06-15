//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

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

        public class Field
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
                //if (this is PdfText)
                //    Type = Types.PdfText;
                //else if (this is PdfTextLines)
                //    Type = Types.PdfTextLines;
                //else if (this is PdfCharBoxs)
                //    Type = Types.PdfCharBoxs;
                //else if (this is OcrText)
                //    Type = Types.OcrText;
                //else if (this is OcrTextLines)
                //    Type = Types.OcrTextLines;
                //else if (this is OcrCharBoxs)
                //    Type = Types.OcrCharBoxs;
                //else if (this is Image)
                //    Type = Types.Image;
                //else if (this is OcrTextLineImages)
                //    Type = Types.OcrTextLineImages;
                //else
                //    //throw new Exception("Unknown type: " + this.GetType());
                //    Type = Types.Base;
            }

            //[Newtonsoft.Json.JsonIgnore]
            //public readonly Types Type;
            //public enum Types
            //{
            //    /// <summary>
            //    /// for backward compatibility
            //    /// </summary>
            //    Base,
            //    PdfText,
            //    PdfTextLines ,
            //    PdfCharBoxs ,
            //    OcrText ,
            //    OcrTextLines,
            //    OcrCharBoxs ,
            //    Image ,
            //    OcrTextLineImages ,
            //}

            ///// <summary>
            ///// Remains for backward compatibility. It makes sense only for the base type Field.
            ///// </summary>
            public ValueTypes DefaultValueType = ValueTypes.PdfText;

            /// <summary>
            /// Remains for backward compatibility. It makes sense only for the base type Field.
            /// </summary>
            public enum ValueTypes
            {
                PdfText = 0,
                PdfTextLines = 1,
                PdfCharBoxs = 2,
                OcrText = 3,
                OcrTextLines = 4,
                OcrCharBoxs = 5,
                Image = 6,
                OcrTextLineImages = 7,
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
        }

        public class PdfText : Field
        {
        }

        public class PdfTextLines : Field
        {
        }

        public class PdfCharBoxs : Field
        {
        }

        public class OcrText : Field
        {
        }

        public class OcrTextLines : Field
        {
        }

        public class OcrCharBoxs : Field
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