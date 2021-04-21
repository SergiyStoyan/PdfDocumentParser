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

            if (newTemplate == null
                || newTemplate.PageRotation != PageCollection.ActiveTemplate.PageRotation
                || !Serialization.Json.IsEqual(newTemplate.Deskew, PageCollection.ActiveTemplate.Deskew)
                )
            {
                _activeTemplateImageData = null;
                _activeTemplateBitmap?.Dispose();
                _activeTemplateBitmap = null;
                _activeTemplateOcrCharBoxs = null;
                _activeTemplateCvImage?.Dispose();
                _activeTemplateCvImage = null;
            }
            else if (newTemplate.TesseractPageSegMode != PageCollection.ActiveTemplate.TesseractPageSegMode)
            {
                _activeTemplateOcrCharBoxs = null;
            }

            if (!Serialization.Json.IsEqual(PageCollection.ActiveTemplate, newTemplate))
            {
                anchorIds2anchorActualInfo.Clear();
                fieldNames2fieldActualInfos.Clear();
            }
        }

        internal Bitmap GetRectangleFromActiveTemplateBitmap(float x, float y, float w, float h)
        {
            Rectangle r = new Rectangle(0, 0, ActiveTemplateBitmap.Width, ActiveTemplateBitmap.Height);
            r.Intersect(new Rectangle((int)x, (int)y, (int)w, (int)h));
            if (r.Width < 1 || r.Height < 1)
                return null;
            return ActiveTemplateBitmap.Clone(r, ActiveTemplateBitmap.PixelFormat);
            //return Cliver.Win.ImageRoutines.GetCopy(ActiveTemplateBitmap, r);
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
                    //Bitmap b = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), Bitmap.PixelFormat);//!!!throws from Tesseract: A generic error occurred in GDI+.
                    Bitmap b = new Bitmap(Bitmap);//!!!it sets to 96dpi
                    b.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution);

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
                            int o = 0;
                            try
                            {
                                o = Ocr.This.DetectOrientationAngle(b, out float confidence);
                            }
                            catch (Tesseract.TesseractException e)//on page with no text
                            {
                                Log.Warning2(e);
                            }
                            if (o <= 45) { }
                            else if (o <= 135)
                                b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            else if (o <= 225)
                                b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            else if (o <= 315)
                                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            //Tesseract.Orientation o = Ocr.This.DetectOrientation(b);
                            //switch (o)
                            //{
                            //    case Tesseract.Orientation.PageLeft:
                            //        b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            //        break;
                            //    case Tesseract.Orientation.PageDown:
                            //        b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            //        break;
                            //    case Tesseract.Orientation.PageRight:
                            //        b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            //        break;
                            //    case Tesseract.Orientation.PageUp:
                            //        break;
                            //    default:
                            //        throw new Exception("Unknown option: " + o);
                            //}
                            break;
                        default:
                            throw new Exception("Unknown option: " + PageCollection.ActiveTemplate.PageRotation);
                    }

                    if (PageCollection.ActiveTemplate.Deskew != null)
                    {
                        //using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                        //{
                        //    //image.Density = new PointD(600, 600);
                        //    //image.AutoLevel();
                        //    //image.Negate();
                        //    //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                        //    //image.Negate();
                        //    image.Deskew(new ImageMagick.Percentage(PageCollection.ActiveTemplate.DeskewThreshold));
                        //    //image.AutoThreshold(AutoThresholdMethod.OTSU);
                        //    //image.Despeckle();
                        //    //image.WhiteThreshold(new Percentage(20));
                        //    //image.Trim();
                        //    b.Dispose();
                        //    b = image.ToBitmap();
                        //}
                        Deskewer.Deskew(ref b, PageCollection.ActiveTemplate.Deskew);
                    }

                    b = PageCollection.ActiveTemplate.BitmapPreprocessor.GetProcessed(b);

                    //Template.Anchor.CvImage ai = (Template.Anchor.CvImage)PageCollection.ActiveTemplate.Anchors[0];//!!!test
                    //PageCollection.ActiveTemplate.SubtractingImages = new List<Template.CvImage>() { new Template.CvImage { Image = ai.Image, ScaleDeviation = ai.ScaleDeviation, Threshold = ai.Threshold } };
                    //if (PageCollection.ActiveTemplate.SubtractingImages?.Any() == true)//!!!needs test/debug!
                    //{
                    //    foreach (Template.CvImage cvi in PageCollection.ActiveTemplate.SubtractingImages)
                    //    {
                    //        for (; ; )
                    //        {
                    //            CvImage cvPage = new CvImage(b);
                    //            //List<CvImage.Match> ms = cvi.Image.FindWithinImage(cvPage, new Size(cvi.Image.Width, cvi.Image.Height), cvi.Threshold, cvi.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
                    //            CvImage.Match m = cvi.Image.FindBestMatchWithinImage(cvPage, cvi.Threshold, cvi.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
                    //            if (m == null)
                    //                continue;
                    //            Rectangle r = m.Rectangle;
                    //            for (int x = (int)(r.X / Settings.Constants.Image2PdfResolutionRatio); x < r.Right / Settings.Constants.Image2PdfResolutionRatio; x++)
                    //                for (int y = (int)(r.Y / Settings.Constants.Image2PdfResolutionRatio); y < r.Bottom / Settings.Constants.Image2PdfResolutionRatio; y++)
                    //                    b.SetPixel(x, y, Color.White);
                    //        }
                    //    }
                    //}

                    _activeTemplateBitmap = b;
                    if (PageCollection.ActiveTemplate.ScalingAnchorId > 0)
                    {
                        DetectedImageScale = detectScaleByScalingAnchor();
                        if (DetectedImageScale == 0)
                            Log.Warning("Could not detect image scale.");
                        else if (DetectedImageScale != 1)
                        {
                            _activeTemplateBitmap = Win.ImageRoutines.GetScaled(b, 1f / DetectedImageScale);
                            b.Dispose();
                            _activeTemplateCvImage?.Dispose();
                            _activeTemplateCvImage = null;
                        }
                        Log.Inform("Detected image scale: " + DetectedImageScale);
                    }
                    else
                        DetectedImageScale = -1;
                }
                return _activeTemplateBitmap;
            }
        }
        public float DetectedImageScale { get; private set; } = -1;

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
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(PageCollection.PdfDocument, Number, true);
                return _pdfCharBoxs;
            }
        }
        List<Pdf.CharBox> _pdfCharBoxs;

        internal Size Size
        {
            get
            {
                if (_size == Size.Empty)
                    _size = Pdf.GetPageSize(PageCollection.PdfDocument, Number);
                return _size;
            }
        }
        Size _size = Size.Empty;

        internal List<Ocr.CharBox> ActiveTemplateOcrCharBoxs
        {
            get
            {
                if (_activeTemplateOcrCharBoxs == null)
                {
                    _activeTemplateOcrCharBoxs = Ocr.This.GetCharBoxs(ActiveTemplateBitmap, PageCollection.ActiveTemplate.TesseractPageSegMode);
                }
                return _activeTemplateOcrCharBoxs;
            }
        }
        List<Ocr.CharBox> _activeTemplateOcrCharBoxs;

        float detectScaleByScalingAnchor()
        {
            Template.Anchor.CvImage sda = PageCollection.ActiveTemplate.GetScalingAnchor();
            CvImage searchRectangleCI;
            if (sda.SearchRectangleMargin >= 0)
            {
                RectangleF sr = getSearchRectangle(sda.Rectangle(), sda.SearchRectangleMargin);
                Bitmap b = GetRectangleFromActiveTemplateBitmap(sr.X / Settings.Constants.Image2PdfResolutionRatio, sr.Y / Settings.Constants.Image2PdfResolutionRatio, sr.Width / Settings.Constants.Image2PdfResolutionRatio, sr.Height / Settings.Constants.Image2PdfResolutionRatio);
                if (b == null)
                    throw new Exception("The scaling anchor's rectangle is null.");
                searchRectangleCI = new CvImage(b);
            }
            else
                searchRectangleCI = ActiveTemplateCvImage;
            CvImage.Match m = sda.Image.FindBestMatchWithinImage(searchRectangleCI, sda.Threshold, sda.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
            if (m != null)
                return m.Scale;
            return 0;
        }

        public Color GetColor(float x, float y)
        {
            return ActiveTemplateBitmap.GetPixel((int)Math.Round(x / Settings.Constants.Image2PdfResolutionRatio), (int)Math.Round(y / Settings.Constants.Image2PdfResolutionRatio));
        }
    }
}