//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page : IDisposable
    {
        internal Page(PageCollection pageCollection, int pageI)
        {
            PageCollection = pageCollection;
            Number = pageI;
        }
        public readonly int Number;
        public readonly PageCollection PageCollection;

        ~Page()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_pdfCharBoxs != null)
                {
                    _pdfCharBoxs = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                {
                    _activeTemplateOcrCharBoxs = null;
                }
            }
        }

        //class ActualTemplateCash : IDisposable
        //{
        //    internal Dictionary<string, AnchorActualInfo> AnchorHashes2anchorActualInfo = new Dictionary<string, AnchorActualInfo>();
        //    internal ImageData ImageData;
        //    internal Bitmap Bitmap;
        //    internal List<Ocr.CharBox> OcrCharBoxs;

        //    public void Dispose()
        //    {
        //    }
        //}

        internal Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = Pdf.RenderBitmap(PageCollection.PdfFile, Number, Settings.Constants.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        internal void OnActiveTemplateUpdating(Template newTemplate)
        {
            if (PageCollection.ActiveTemplate == null)
                return;

            if (newTemplate == null || newTemplate.PageRotation != PageCollection.ActiveTemplate.PageRotation || newTemplate.AutoDeskew != PageCollection.ActiveTemplate.AutoDeskew)
            {
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                    _activeTemplateOcrCharBoxs = null;
            }
            if (!Serialization.Json.IsEqual(PageCollection.ActiveTemplate, newTemplate))
            {
                anchorIds2anchorActualInfo.Clear();
                fieldNames2fieldActualInfo.Clear();
            }
        }

        internal Bitmap GetRectangleFromActiveTemplateBitmap(float x, float y, float w, float h)
        {
            Rectangle r = new Rectangle(0, 0, ActiveTemplateBitmap.Width, ActiveTemplateBitmap.Height);
            r.Intersect(new Rectangle((int)x, (int)y, (int)w, (int)h));
            return ActiveTemplateBitmap.Clone(r, System.Drawing.Imaging.PixelFormat.Undefined);
            //return ImageRoutines.GetCopy(ActiveTemplateBitmap, new RectangleF(x, y, w, h));
        }

        internal Bitmap ActiveTemplateBitmap
        {
            get
            {
                if (_activeTemplateBitmap == null)
                {
                    if (PageCollection.ActiveTemplate == null)
                        return null;

                    //From stackoverflow:
                    //Using the Graphics class to modify the clone (created with .Clone()) will not modify the original.
                    //Similarly, using the LockBits method yields different memory blocks for the original and clone.
                    //...change one random pixel to a random color on the clone... seems to trigger a copy of all pixel data from the original.
                    //Bitmap b = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.PixelFormat.Undefined);!!!throws from Tesseract: A generic error occurred in GDI+.
                    Bitmap b = new Bitmap(Bitmap);

                    switch (PageCollection.ActiveTemplate.PageRotation)
                    {
                        case Template.PageRotations.NONE:
                            break;
                        case Template.PageRotations.Clockwise90:
                            b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case Template.PageRotations.Clockwise180:
                            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case Template.PageRotations.Clockwise270:
                            b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case Template.PageRotations.AutoDetection:
                            int o = Ocr.This.DetectOrientationAngle(b, out float confidence);
                            if (o <= 45) { }
                            else if (o <= 135)
                                b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            else if (o <= 225)
                                b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            else if (o <= 315)
                                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            //Bitmap b2 = new Bitmap(b.Width, b.Height);
                            //using (var gr = Graphics.FromImage(b2))
                            //{
                            //    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            //    gr.TranslateTransform(b.Width / 2, b.Height / 2);
                            //    gr.RotateTransform(-(float)o);
                            //    gr.DrawImage(b, -b.Width / 2, -b.Height / 2, b.Width, b.Height);
                            //}
                            //b.Dispose();
                            //b = b2;
                            break;
                        default:
                            throw new Exception("Unknown option: " + PageCollection.ActiveTemplate.PageRotation);
                    }

                    if (PageCollection.ActiveTemplate.AutoDeskew)
                    {
                        using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                        {
                            //image.Density = new PointD(600, 600);
                            //image.AutoLevel();
                            //image.Negate();
                            //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                            //image.Negate();
                            image.Deskew(new ImageMagick.Percentage(PageCollection.ActiveTemplate.AutoDeskewThreshold));
                            //image.AutoThreshold(AutoThresholdMethod.OTSU);
                            //image.Despeckle();
                            //image.WhiteThreshold(new Percentage(20));
                            //image.Trim();
                            b.Dispose();
                            b = image.ToBitmap();
                        }
                    }

                    _activeTemplateBitmap = b;
                }
                return _activeTemplateBitmap;
            }
        }

        Bitmap _activeTemplateBitmap = null;

        internal ImageData ActiveTemplateImageData
        {
            get
            {
                if (_activeTemplateImageData == null)
                    _activeTemplateImageData = new ImageData(ActiveTemplateBitmap);
                return _activeTemplateImageData;
            }
        }
        ImageData _activeTemplateImageData;

        internal CvImage ActiveTemplateCvImage
        {
            get
            {
                if (_activeTemplateCvImage == null)
                    _activeTemplateCvImage = new CvImage(ActiveTemplateBitmap);
                return _activeTemplateCvImage;
            }
        }
        CvImage _activeTemplateCvImage;

        internal List<Pdf.CharBox> PdfCharBoxs
        {
            get
            {
                if (_pdfCharBoxs == null)
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(PageCollection.PdfReader, Number, true);
                return _pdfCharBoxs;
            }
        }
        List<Pdf.CharBox> _pdfCharBoxs;

        internal List<Ocr.CharBox> ActiveTemplateOcrCharBoxs
        {
            get
            {
                if (_activeTemplateOcrCharBoxs == null)
                {
                    _activeTemplateOcrCharBoxs = Ocr.This.GetCharBoxs(ActiveTemplateBitmap);
                }
                return _activeTemplateOcrCharBoxs;
            }
        }
        List<Ocr.CharBox> _activeTemplateOcrCharBoxs;

        public Bitmap GetActiveTemplateBitmapCopy()
        {
            return GetScaledImage2Pdf(ActiveTemplateBitmap);
        }
    }
}