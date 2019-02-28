//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;

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

        public abstract class Field
        {
            public string Name;

            //public int? AnchorId//conversion from the old format; to be removed
            //{
            //    set
            //    {
            //        if (value == null)
            //            return;
            //        LeftAnchor = new SideAnchor { Id = (int)value };
            //        TopAnchor = new SideAnchor { Id = (int)value };
            //        RightAnchor = new SideAnchor { Id = (int)value };
            //        BottomAnchor = new SideAnchor { Id = (int)value };
            //    }
            //}
            //public int? LeftAnchorId//conversion from the old format; to be removed
            //{
            //    set
            //    {
            //        if (value == null)
            //            return;
            //        LeftAnchor = new SideAnchor { Id = (int)value };
            //    }
            //}
            //public int? TopAnchorId//conversion from the old format; to be removed
            //{
            //    set
            //    {
            //        if (value == null)
            //            return;
            //        TopAnchor = new SideAnchor { Id = (int)value };
            //    }
            //}
            //public int? RightAnchorId//conversion from the old format; to be removed
            //{
            //    set
            //    {
            //        if (value == null)
            //            return;
            //        RightAnchor = new SideAnchor { Id = (int)value };
            //    }
            //}
            //public int? BottomAnchorId//conversion from the old format; to be removed
            //{
            //    set
            //    {
            //        if (value == null)
            //            return;
            //        BottomAnchor = new SideAnchor { Id = (int)value };
            //    }
            //}

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
                public float Shift;// { get { return 0; } set { } }
            }

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

            /// <summary>
            /// set by the custom application. When set, the field is considered as a table column intercepted with the field specified by ColumnOf. 
            /// In this way, all the fields which are columns of the same field always contain the same number of lines. 
            /// Not all field types supported!
            /// </summary>
            public string ColumnOfTable = null;

            public class PdfText : Field
            {
            }

            public class OcrText : Field
            {
            }

            public class ImageData : Field
            {
            }
        }
    }
}