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
using System.IO;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
//using iTextSharp.text.pdf.parser;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    public class Page : IDisposable
    {
        public Page(PageCollection pageCollection, int pageI)
        {
            this.pageCollection = pageCollection;
            this.pageI = pageI;
        }
        int pageI;
        PageCollection pageCollection;

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
                if (_imageData != null)
                {
                    //imageData.Dispose();
                    _imageData = null;
                }
                if (_pdfCharBoxs != null)
                {
                    _pdfCharBoxs = null;
                }
                if (_ocrCharBoxs != null)
                {
                    _ocrCharBoxs = null;
                }
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = Pdf.RenderBitmap(pageCollection.PdfFile, pageI, Settings.General.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        public void OnActiveTemplateUpdating(Settings.Template newTemplate)
        {
            if (pageCollection.ActiveTemplate == null)
                return;

            if (newTemplate.PagesRotation != pageCollection.ActiveTemplate.PagesRotation || newTemplate.AutoDeskew != pageCollection.ActiveTemplate.AutoDeskew)
            {
                if (_imageData != null)
                {
                    //_imageData.Dispose();
                    _imageData = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_ocrCharBoxs != null)
                    _ocrCharBoxs = null;

                floatingAnchorValueStrings2rectangles.Clear();
            }

            //if (pageCollection.ActiveTemplate.Name != newTemplate.Name)
            //    floatingAnchorValueStrings2rectangles.Clear();

            if (newTemplate.BrightnessTolerance != pageCollection.ActiveTemplate.BrightnessTolerance
                || newTemplate.DifferentPixelNumberTolerance != pageCollection.ActiveTemplate.DifferentPixelNumberTolerance)
                floatingAnchorValueStrings2rectangles.Clear();
        }

        Bitmap getRectangleFromActiveTemplateBitmap(float x, float y, float w, float h)
        {
            return ActiveTemplateBitmap.Clone(new RectangleF(x, y, w, h), System.Drawing.Imaging.PixelFormat.Undefined); 
            //return ImageRoutines.GetCopy(ActiveTemplateBitmap, new RectangleF(x, y, w, h));
        }

        public Bitmap ActiveTemplateBitmap
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

                    if (pageCollection.ActiveTemplate.PagesRotation != Settings.Template.PageRotations.NONE)
                    {
                        //b = ImageRoutines.GetCopy(Bitmap);
                        switch (pageCollection.ActiveTemplate.PagesRotation)
                        {
                            case Settings.Template.PageRotations.NONE:
                                break;
                            case Settings.Template.PageRotations.Clockwise90:
                                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                break;
                            case Settings.Template.PageRotations.Clockwise180:
                                b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case Settings.Template.PageRotations.Clockwise270:
                                b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                            default:
                                throw new Exception("Unknown option: " + pageCollection.ActiveTemplate.PagesRotation);
                        }
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
                            image.Deskew(new ImageMagick.Percentage(10));
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

        public PointF? GetFloatingAnchorPoint0(int floatingAnchorId)
        {
            Settings.Template.FloatingAnchor fa = pageCollection.ActiveTemplate.FloatingAnchors.Find(a => a.Id == floatingAnchorId);
            List<RectangleF> rs = GetFloatingAnchorRectangles(fa);
            if (rs == null || rs.Count < 1)
                return null;
            return new PointF(rs[0].X, rs[0].Y);
        }

        public List<RectangleF> GetFloatingAnchorRectangles(Settings.Template.FloatingAnchor fa)
        {
            List<RectangleF> rs;
            string fas = fa.ValueAsString;
            if (!floatingAnchorValueStrings2rectangles.TryGetValue(fas, out rs))
            {
                rs = findFloatingAnchor(fa);
                floatingAnchorValueStrings2rectangles[fas] = rs;
            }
            return rs;
        }
        Dictionary<string, List<RectangleF>> floatingAnchorValueStrings2rectangles = new Dictionary<string, List<RectangleF>>();

        List<RectangleF> findFloatingAnchor(Settings.Template.FloatingAnchor fa)
        {
            if (fa == null || fa.GetValue() == null)
                return null;

            switch (fa.ValueType)
            {
                case Settings.Template.ValueTypes.PdfText:
                    {
                        List<Settings.Template.FloatingAnchor.PdfTextValue.CharBox> ses = ((Settings.Template.FloatingAnchor.PdfTextValue)fa.GetValue()).CharBoxs;
                        if (ses.Count < 1)
                            return null;
                        List<Pdf.CharBox> bts = new List<Pdf.CharBox>();
                        foreach (Pdf.CharBox bt0 in PdfCharBoxs.Where(a => a.Char == ses[0].Char))
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < ses.Count; i++)
                            {
                                float x = bt0.R.X + ses[i].Rectangle.X - ses[0].Rectangle.X;
                                float y = bt0.R.Y + ses[i].Rectangle.Y - ses[0].Rectangle.Y;
                                foreach (Pdf.CharBox bt in PdfCharBoxs.Where(a => a.Char == ses[i].Char))
                                {
                                    if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                                        continue;
                                    if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                                        continue;
                                    if (bts.Contains(bt))
                                        continue;
                                    bts.Add(bt);
                                }
                            }
                            if (bts.Count == ses.Count)
                                return bts.Select(x => x.R).ToList();
                        }
                    }
                    return null;
                case Settings.Template.ValueTypes.OcrText:
                    {
                        List<Settings.Template.FloatingAnchor.OcrTextValue.CharBox> ses = ((Settings.Template.FloatingAnchor.OcrTextValue)fa.GetValue()).CharBoxs;
                        if (ses.Count < 1)
                            return null;
                        List<Ocr.CharBox> bts = new List<Ocr.CharBox>();
                        foreach (Ocr.CharBox bt0 in OcrCharBoxs.Where(a => a.Char == ses[0].Char))
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < ses.Count; i++)
                            {
                                float x = bt0.R.X + ses[i].Rectangle.X - ses[0].Rectangle.X;
                                float y = bt0.R.Y + ses[i].Rectangle.Y - ses[0].Rectangle.Y;
                                foreach (Ocr.CharBox bt in OcrCharBoxs.Where(a => a.Char == ses[i].Char))
                                {
                                    if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                                        continue;
                                    if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                                        continue;
                                    if (bts.Contains(bt))
                                        continue;
                                    bts.Add(bt);
                                }
                            }
                            if (bts.Count == ses.Count)
                                return bts.Select(x => x.R).ToList();
                        }
                    }
                    return null;
                case Settings.Template.ValueTypes.ImageData:
                    List<Settings.Template.FloatingAnchor.ImageDataValue.ImageBox> ibs = ((Settings.Template.FloatingAnchor.ImageDataValue)fa.GetValue()).ImageBoxs;
                    if (ibs.Count < 1)
                        return null;
                    List<RectangleF> bestRs = null;
                    float minDeviation = 1;
                    ibs[0].ImageData.FindWithinImage(ImageData, pageCollection.ActiveTemplate.BrightnessTolerance, pageCollection.ActiveTemplate.DifferentPixelNumberTolerance, (Point point0, float deviation) =>
                    {
                        List<RectangleF> rs = new List<RectangleF>();
                        rs.Add(new RectangleF(point0, new SizeF(ibs[0].Rectangle.Width, ibs[0].Rectangle.Height)));
                        for (int i = 1; i < ibs.Count; i++)
                        {
                            Settings.Template.RectangleF r = new Settings.Template.RectangleF(ibs[i].Rectangle.X + point0.X, ibs[i].Rectangle.Height + point0.Y, ibs[i].Rectangle.Width, ibs[i].Rectangle.Height);
                            using (Bitmap rb = getRectangleFromActiveTemplateBitmap(r.X, r.Y, r.Width, r.Height))
                            {
                                if (!ibs[i].ImageData.ImageIsSimilar(new ImageData(rb, false), pageCollection.ActiveTemplate.BrightnessTolerance, pageCollection.ActiveTemplate.DifferentPixelNumberTolerance))
                                    return true;
                            }
                            rs.Add(r.GetSystemRectangleF());
                        }
                        if (!pageCollection.ActiveTemplate.FindBestImageMatch)
                        {
                            bestRs = rs;
                            return false;
                        }
                        if (deviation < minDeviation)
                        {
                            minDeviation = deviation;
                            bestRs = rs;
                        }
                        return true;
                    });
                    return bestRs;
                default:
                    throw new Exception("Unknown option: " + fa.ValueType);
            }
        }

        //bool findFloatingAnchorSecondaryElements(PointF point0, Settings.Template.FloatingAnchor fa)
        //{
        //    for (int i = 1; i < fa.Elements.Count; i++)
        //    {
        //        if (!findFloatingAnchorElement(point0, fa.Elements[i]))
        //            return false;
        //    }
        //    return true;
        //}
        //bool findFloatingAnchorElement(PointF point0, Settings.Template.FloatingAnchor.Element e)
        //{
        //    switch (e.ElementType)
        //    {
        //        case Settings.Template.ValueTypes.PdfText:
        //            {
        //                List<Settings.Template.FloatingAnchor.PdfTextElement.CharBox> ses = ((Settings.Template.FloatingAnchor.PdfTextElement)e.Get()).PdfCharBoxs;
        //                List<Pdf.CharBox> bts = new List<Pdf.CharBox>();

        //                bts.Clear();
        //                for (int i = 0; i < ses.Count; i++)
        //                {
        //                    float x = point0.X + ses[i].Rectangle.X - ses[0].Rectangle.X;
        //                    float y = point0.Y + ses[i].Rectangle.Y - ses[0].Rectangle.Y;
        //                    foreach (Pdf.CharBox bt in charBoxLists.Where(a => a.Text == ses[i].Char))
        //                    {
        //                        if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
        //                            continue;
        //                        if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
        //                            continue;
        //                        if (bts.Contains(bt))
        //                            continue;
        //                        bts.Add(bt);
        //                    }
        //                }
        //                return bts.Count == ses.Count;
        //            }
        //        case Settings.Template.ValueTypes.OcrText:
        //            {
        //                return true;
        //            }
        //        case Settings.Template.ValueTypes.ImageData:
        //            {
        //                ImageData id = (ImageData)e.Get();
        //                return id.ImageIsSimilar(new ImageData(GetRectangeFromActiveTemplateBitmap(new Settings.Template.RectangleF(point0.X, point0.Y, id.Width, id.Height))));
        //            }
        //        default:
        //            throw new Exception("Unknown option: " + e.ElementType);
        //    }
        //}

        public ImageData ImageData
        {
            get
            {
                if (_imageData == null)
                    _imageData = new ImageData(ActiveTemplateBitmap);
                return _imageData;
            }
        }
        ImageData _imageData;

        public List<Pdf.CharBox> PdfCharBoxs
        {
            get
            {
                if (_pdfCharBoxs == null)
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(pageCollection.PdfReader, pageI);
                return _pdfCharBoxs;
            }
        }
        List<Pdf.CharBox> _pdfCharBoxs;

        public List<Ocr.CharBox> OcrCharBoxs
        {
            get
            {
                if (_ocrCharBoxs == null)
                {
                    _ocrCharBoxs = Ocr.This.GetCharBoxs(ActiveTemplateBitmap);
                }
                return _ocrCharBoxs;
            }
        }
        List<Ocr.CharBox> _ocrCharBoxs;

        public bool IsDocumentFirstPage()
        {
            string error;
            return IsDocumentFirstPage(out error);
        }

        public bool IsDocumentFirstPage(out string error)
        {
            if (pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks == null || pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks.Count < 1)
            {
                error = "Template '" + pageCollection.ActiveTemplate.Name + "' has no DocumentFirstPageRecognitionMarks defined.";
                return false;
            }
            foreach (Settings.Template.Mark m in pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks)
            {
                if (m.FloatingAnchorId != null && m.GetValue() == null)
                {
                    PointF? p0 = GetFloatingAnchorPoint0((int)m.FloatingAnchorId);
                    if (p0 == null)
                    {
                        error = "FloatingAnchor[" + m.FloatingAnchorId + "] not found.";
                        return false;
                    }
                    continue;
                }
                object v = GetValue(m.FloatingAnchorId, m.Rectangle, m.ValueType, out error);
                if (v == null)
                    return false;
                switch (m.ValueType)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        {
                            string t1 = NormalizeText((string)m.GetValue());
                            string t2 = NormalizeText((string)v);
                            if (t1 == t2)
                                continue;
                            error = "documentFirstPageRecognitionMark[" + pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks.IndexOf(m) + "]:\r\n" + t2 + "\r\n <> \r\n" + t1;
                            return false;
                        }
                    case Settings.Template.ValueTypes.OcrText:
                        {
                            string t1 = NormalizeText((string)m.GetValue());
                            string t2 = NormalizeText((string)v);
                            if (t1 == t2)
                                continue;
                            error = "documentFirstPageRecognitionMark[" + pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks.IndexOf(m) + "]:\r\n" + t2 + "\r\n <> \r\n" + t1;
                            return false;
                        }
                    case Settings.Template.ValueTypes.ImageData:
                        {
                            ImageData id = (ImageData)m.GetValue();
                            if (id.ImageIsSimilar((ImageData)(v), pageCollection.ActiveTemplate.BrightnessTolerance, pageCollection.ActiveTemplate.DifferentPixelNumberTolerance))
                                continue;
                            error = "documentFirstPageRecognitionMark[" + pageCollection.ActiveTemplate.DocumentFirstPageRecognitionMarks.IndexOf(m) + "]: image is not similar.";
                            return false;
                        }
                    default:
                        throw new Exception("Unknown option: " + m.ValueType);
                }
            }
            error = null;
            return true;
        }

        public object GetValue(int? floatingAnchorId, Settings.Template.RectangleF r_, Settings.Template.ValueTypes valueType, out string error)
        {
            //try
            //{
            if (r_ == null)
            {
                error = "Rectangular is not defined.";
                return null;
            }
            if (r_.Width <= Settings.General.CoordinateDeviationMargin || r_.Height <= Settings.General.CoordinateDeviationMargin)
            {
                error = "Rectangular is malformed.";
                return null;
            }
            PointF point0 = new PointF(0, 0);
            if (floatingAnchorId != null)
            {
                PointF? p0;
                p0 = GetFloatingAnchorPoint0((int)floatingAnchorId);
                if (p0 == null)
                {
                    error = "FloatingAnchor[" + floatingAnchorId + "] is not found.";
                    return null;
                }
                point0 = (PointF)p0;
            }
            Settings.Template.RectangleF r = new Settings.Template.RectangleF(r_.X + point0.X, r_.Y + point0.Y, r_.Width, r_.Height);
            error = null;
            switch (valueType)
            {
                case Settings.Template.ValueTypes.PdfText:
                    return Pdf.GetTextByTopLeftCoordinates(PdfCharBoxs, r.GetSystemRectangleF());
                case Settings.Template.ValueTypes.OcrText:
                    //return Ocr.This.GetText(ActiveTemplateBitmap, r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);                    
                    //return Ocr.GetTextByTopLeftCoordinates(OcrCharBoxs, r.GetSystemRectangleF());//sometimes does not work
                    return Ocr.This.GetText(ActiveTemplateBitmap, r.GetSystemRectangleF());
                case Settings.Template.ValueTypes.ImageData:
                    using (Bitmap rb = getRectangleFromActiveTemplateBitmap(r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio))
                    {
                        return new ImageData(rb);
                    }
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            //}
            //catch(Exception e)
            //{
            //    error = Log.GetExceptionMessage(e);
            //}
            //return null;
        }
        static Dictionary<Bitmap, ImageData> bs2id = new Dictionary<Bitmap, ImageData>();

        Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();

        public float Height;

        public static string NormalizeText(string value)
        {
            if (value == null)
                return null;
            value = FieldPreparation.RemoveNonPrintablesRegex.Replace(value, " ");
            value = Regex.Replace(value, @"\s+", " ");
            value = value.Trim();
            return value;
        }
    }
}