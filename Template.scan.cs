//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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

        public CharFilter CharFilter;

        public Field.OcrModes OcrMode = Field.OcrModes.SingleFieldFromFieldImage;
        internal bool AdjustLineBorders
        {
            get { return OcrMode.HasFlag(Field.OcrModes.AdjustLineBorders); }
            set
            {
                if (value) OcrMode |= Field.OcrModes.AdjustLineBorders;
                else OcrMode &= ~Field.OcrModes.AdjustLineBorders;
            }
        }
        internal bool SingleFieldFromFieldImage
        {
            get { return OcrMode.HasFlag(Field.OcrModes.SingleFieldFromFieldImage); }
            set
            {
                if (value) OcrMode |= Field.OcrModes.SingleFieldFromFieldImage;
                else OcrMode &= ~Field.OcrModes.SingleFieldFromFieldImage;
            }
        }
        internal bool ColumnCellFromCellImage
        {
            get { return OcrMode.HasFlag(Field.OcrModes.ColumnCellFromCellImage); }
            set
            {
                if (value) OcrMode |= Field.OcrModes.ColumnCellFromCellImage;
                else OcrMode &= ~Field.OcrModes.ColumnCellFromCellImage;
            }
        }

        public int LinePaddingY = 1;

        public PageRotations PageRotation = PageRotations.NONE;
        public enum PageRotations
        {
            NONE,
            Clockwise90,
            Clockwise180,
            Clockwise270,
            AutoDetection
        }

        public Deskewer.Config Deskew = null;

        public int CvImageScalePyramidStep = 2;

        public int ScalingAnchorId = -1;

        public Anchor.CvImage GetScalingAnchor()
        {
            return (Anchor.CvImage)Anchors?.FirstOrDefault(a => a.Id == ScalingAnchorId);
        }

        //public List<CvImage> SubtractingImages;
        //public class CvImage
        //{
        //    public PdfDocumentParser.CvImage Image;
        //    public float Threshold = 0.70f;
        //    public float ScaleDeviation = 0.05f;
        //}

        public string BitmapPreprocessorClassDefinition = null;
        //public bool PreprocessBitmap { get { return !string.IsNullOrWhiteSpace(Compiler.GetOnlyCode(BitmapPreprocessorClassDefinition)); } }

        internal BitmapPreprocessor BitmapPreprocessor
        {
            get
            {
                if (bitmapPreprocessor == null)
                {
                    if (string.IsNullOrWhiteSpace(Compiler.RemoveComments(BitmapPreprocessorClassDefinition)))
                        bitmapPreprocessor = new EmptyBitmapPreprocessor();
                    else
                        bitmapPreprocessor = BitmapPreprocessor.CreateBitmapPreprocessor(this);
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