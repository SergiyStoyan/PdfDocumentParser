//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

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
            public string TestFile;
            public decimal TestPictureScale = 1.2m;
        }
        public EditorSettings Editor;

        public string Name;

        //public float TextAutoInsertSpaceThreshold = 6;
        //public string TextAutoInsertSpaceSubstitute = "\t";
        public TextAutoInsertSpace TextAutoInsertSpace;

        public bool IgnoreInvisiblePdfChars = true;//used but not edited
        public bool IgnoreDuplicatedPdfChars = true;//used but not edited

        public PageRotations PageRotation = PageRotations.NONE;
        public enum PageRotations
        {
            NONE,
            Clockwise90,
            Clockwise180,
            Clockwise270,
            AutoDetection
        }

        public bool AutoDeskew = false;
        public int AutoDeskewThreshold = 1;
        public int CvImageScalePyramidStep = 2;

        public List<CvImage> SubtractingImages;
        public class CvImage
        {
            public PdfDocumentParser.CvImage Image;
            public float Threshold = 0.70f;
            public float ScaleDeviation = 0.05f;
        }

        public string BitmapPreprocessorClassDefinition = null;
        public bool BitmapPreprocessorClassDefinitionIsActive = false;

        internal BitmapPreprocessor BitmapPreprocessor
        {
            get
            {
                if (bitmapPreprocessor == null)
                {
                    if (string.IsNullOrWhiteSpace(BitmapPreprocessorClassDefinition) || !BitmapPreprocessorClassDefinitionIsActive)
                        bitmapPreprocessor = new DefaultBitmapPreprocessor();
                    else
                        bitmapPreprocessor = BitmapPreprocessor.CompileBitmapPreprocessor(this);
                }
                return bitmapPreprocessor;
            }
        }
        BitmapPreprocessor bitmapPreprocessor;
        internal class DefaultBitmapPreprocessor : BitmapPreprocessor
        {
            public override Bitmap GetProcessed(Bitmap bitmap)
            {
                return bitmap;
            }
        }

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

    public class TextAutoInsertSpace
    {
        public float Threshold = 6;
        public string Representative = "\t";
        //public string Substitute = " ";
    }
}