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
        public class EditorSettings
        {
            public bool ExtractFieldsAutomaticallyWhenPageChanged = true;
            public bool ShowFieldTextLineSeparators = true;
            public bool CheckConditionsAutomaticallyWhenPageChanged = true;
            public decimal TestPictureScale = 1.2m;
        }
        public EditorSettings Editor;

        public string Name;

        public TextAutoInsertSpace TextAutoInsertSpace = new TextAutoInsertSpace();

        //public bool IgnoreDuplicatedPdfChars = true;//used but not edited

        public List<Anchor> Anchors;

        public List<Condition> Conditions;

        public List<Field> Fields;

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

        public class PointF
        {
            public float X;
            public float Y;
        }

        public class SizeF
        {
            public float Width;
            public float Height;
        }

        public class Size
        {
            public int Width;
            public int Height;
        }

        public class Condition
        {
            public string Name;
            public string Value;

            public bool IsSet()
            {
                return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Value);
            }
        }
    }

    public class CharFilter
    {
        public float MinWidth;
        //{
        //    get { return minWidth; }
        //    set { minWidth = value > 0 ? value : 0; }
        //}
        //float minWidth;
        public float MaxWidth;
        //{
        //    get { return maxWidth; }
        //    set { maxWidth = value > 0 ? value : 0; }
        //}
        //float maxWidth;
        public float MinHeight;
        //{
        //    get { return minHeight; }
        //    set { minHeight = value > 0 ? value : 0; }
        //}
        //float minHeight;
        public float MaxHeight;
        //{
        //    get { return maxHeight; }
        //    set { maxHeight = value > 0 ? value : 0; }
        //}
        //float maxHeight;

        public override string ToString()//used only by debugging tips
        {
            return this.ToStringByJson();
        }
    }

    public class TextAutoInsertSpace
    {
        public float Threshold = 6;
        public string Representative = " ";
        //public string Substitute = " ";
        public bool IgnoreSourceSpaces = false;
    }
}