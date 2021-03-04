//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// parsing rules corresponding to certain pdf document layout
    /// </summary>
    public partial class Template
    {
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
        public int ScaleDetectingAnchorId = -1;

        public Anchor.CvImage GetScaleDetectingAnchor()
        {
            return (Anchor.CvImage)Anchors.FirstOrDefault(a => a.Id == ScaleDetectingAnchorId);
        }

        public List<CvImage> SubtractingImages;
        public class CvImage
        {
            public PdfDocumentParser.CvImage Image;
            public float Threshold = 0.70f;
            public float ScaleDeviation = 0.05f;
        }

        public string BitmapPreprocessorClassDefinition = null;
        public bool PreprocessBitmap = false;

        internal BitmapPreprocessor BitmapPreprocessor
        {
            get
            {
                if (bitmapPreprocessor == null)
                {
                    if (string.IsNullOrWhiteSpace(BitmapPreprocessorClassDefinition) || !PreprocessBitmap)
                        bitmapPreprocessor = new EmptyBitmapPreprocessor();
                    else
                        bitmapPreprocessor = BitmapPreprocessor.CompileBitmapPreprocessor(this);
                }
                return bitmapPreprocessor;
            }
        }
        BitmapPreprocessor bitmapPreprocessor;
        internal class EmptyBitmapPreprocessor : BitmapPreprocessor
        {
            public override Bitmap GetProcessed(Bitmap bitmap)
            {
                return bitmap;
            }
        }
    }
}