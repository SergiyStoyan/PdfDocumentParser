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
        public Tesseract.PageSegMode TesseractPageSegMode = Tesseract.PageSegMode.SingleBlock;

        public PageRotations PageRotation = PageRotations.NONE;
        public enum PageRotations
        {
            NONE,
            Clockwise90,
            Clockwise180,
            Clockwise270,
            AutoDetection
        }

        public bool Deskew = false;
        public int DeskewBlockMaxHeight = 3000;
        public int DeskewBlockMinSpan = 30;
        //public int DeskewThreshold = 1;

        public int CvImageScalePyramidStep = 2;
        public int ScalingAnchorId = -1;

        public Anchor.CvImage GetScalingAnchor()
        {
            return (Anchor.CvImage)Anchors.FirstOrDefault(a => a.Id == ScalingAnchorId);
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