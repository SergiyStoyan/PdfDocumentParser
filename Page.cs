//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page : IDisposable
    {
        internal Page(PageCollection pageCollection, int pageI)
        {
            this.pageCollection = pageCollection;
            Number = pageI;
        }
        public readonly int Number;
        readonly PageCollection pageCollection;

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
                    _bitmap = Pdf.RenderBitmap(pageCollection.PdfFile, Number, Settings.Constants.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        internal void OnActiveTemplateUpdating(Template newTemplate)
        {
            if (pageCollection.ActiveTemplate == null)
                return;

            if (newTemplate == null || newTemplate.PageRotation != pageCollection.ActiveTemplate.PageRotation || newTemplate.AutoDeskew != pageCollection.ActiveTemplate.AutoDeskew)
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

                anchorHashes2anchorActualInfo.Clear();
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
                    if (pageCollection.ActiveTemplate == null)
                        return null;

                    //From stackoverflow:
                    //Using the Graphics class to modify the clone (created with .Clone()) will not modify the original.
                    //Similarly, using the LockBits method yields different memory blocks for the original and clone.
                    //...change one random pixel to a random color on the clone... seems to trigger a copy of all pixel data from the original.
                    Bitmap b = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.PixelFormat.Undefined);

                    switch (pageCollection.ActiveTemplate.PageRotation)
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
                            float confidence;
                            Tesseract.Orientation o = Ocr.This.DetectOrientation(b, out confidence);
                            switch (o)
                            {
                                case Tesseract.Orientation.PageUp:
                                    break;
                                case Tesseract.Orientation.PageRight:
                                    b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case Tesseract.Orientation.PageDown:
                                    b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case Tesseract.Orientation.PageLeft:
                                    b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + o);
                            }
                            break;
                        default:
                            throw new Exception("Unknown option: " + pageCollection.ActiveTemplate.PageRotation);
                    }

                    if (pageCollection.ActiveTemplate.AutoDeskew)
                    {
                        using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                        {
                            //image.Density = new PointD(600, 600);
                            //image.AutoLevel();
                            //image.Negate();
                            //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                            //image.Negate();
                            image.Deskew(new ImageMagick.Percentage(pageCollection.ActiveTemplate.AutoDeskewThreshold));
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

        internal List<Pdf.CharBox> PdfCharBoxs
        {
            get
            {
                if (_pdfCharBoxs == null)
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(pageCollection.PdfReader, Number);
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

        public bool IsCondition(string conditionName)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new Exception("Condition name is not specified.");
            var c = pageCollection.ActiveTemplate.Conditions.FirstOrDefault(a => a.Name == conditionName);
            if (string.IsNullOrWhiteSpace(c.Value))
                throw new Exception("Condition '" + conditionName + "' is not set.");
            return BooleanEngine.Parse(c.Value, this);
        }
    }
}