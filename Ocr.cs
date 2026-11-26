//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TesseractOCR;
using System.Drawing.Imaging;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// OCR routines
    /// </summary>
    public class Ocr : IDisposable
    {
        public static Ocr This
        {
            get
            {
                if (_This == null)
                    _This = new Ocr(Settings.Constants.OcrConfig);
                return _This;
            }
        }
        static Ocr _This = null;

        public static void DisposeThis()
        {
            _This?.Dispose();
            _This = null;
        }

        //public static Ocr Get(Config config)
        //{
        //    if (!ocrConfigKeys2ocr.TryGetValue(config.Key, out Ocr ocr))
        //    {
        //        ocr = new Ocr(config);
        //        ocrConfigKeys2ocr[config.Key] = ocr;
        //    }
        //    return ocr;
        //}
        //static HandyDictionary<string, Ocr> ocrConfigKeys2ocr = new HandyDictionary<string, Ocr>();

        Ocr(Config config)
        {
            engine = new Engine(@"./tessdata", config.Language, config.EngineMode);
            if (config.VariableNames2value != null)
            {
                string m1 = "Could not set Tesseract variable: ";
                foreach (var v in config.VariableNames2value)
                    if (v.Value is string)
                    {
                        if (!engine.SetVariable(v.Key, (string)v.Value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.Value is int)
                    {
                        if (!engine.SetVariable(v.Key, (int)v.Value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.Value is double)
                    {
                        if (!engine.SetVariable(v.Key, (double)v.Value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.Value is bool)
                    {
                        if (!engine.SetVariable(v.Key, (bool)v.Value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else
                        throw new Exception(m1 + v.ToString() + " Not supported type.");
            }
        }
        public class Config
        {
            public TesseractOCR.Enums.Language Language = TesseractOCR.Enums.Language.English;
            public TesseractOCR.Enums.EngineMode EngineMode = TesseractOCR.Enums.EngineMode.Default;
            public Dictionary<string, object> VariableNames2value = null;

            internal string Key
            {
                get
                {
                    if (key == null)
                        key = this.ToStringByJson(false);
                    return key;
                }
            }
            string key = null;
        }

        TesseractOCR.Engine engine = null;

        ~Ocr()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (engine != null)
                {
                    //if (cachedPage != null)
                    //{
                    //    try
                    //    {
                    //        cachedPage.Dispose();
                    //    }
                    //    catch (Exception e)//for some reason: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
                    //    {
                    //    }
                    //    cachedPageBitmap = null;
                    //    cachedPage = null;
                    //}
                    engine.Dispose();
                    engine = null;
                }
            }
        }

        //Tesseract.Page getPage(Bitmap b)
        //{
        //    if (cachedPageBitmap != b)
        //    {
        //        cachedPage?.Dispose();
        //        cachedPage = engine.Process(b, PageSegMode.SparseTextOsd);
        //        cachedPageBitmap = b;
        //    }
        //    return cachedPage;
        //}
        //Bitmap cachedPageBitmap = null;
        //Tesseract.Page cachedPage = null;

        public static TesseractOCR.Pix.Image GetTesseractImage(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            // PNG is lossless and compact; ImageFormat.Bmp is slightly faster if CPU speed matters
            bitmap.Save(stream, ImageFormat.Png);
            return TesseractOCR.Pix.Image.LoadFromMemory(stream.ToArray());
        }

        public int DetectOrientationAngle(TesseractOCR.Pix.Image b, out float confidence)
        {
            using (TesseractOCR.Page page = engine.Process(b, TesseractOCR.Enums.PageSegMode.OsdOnly))
            {
                page.DetectOrientation(out int o, out confidence);
                return o;
            }
        }

        //!!!abandoned for GetCharBoxsSurroundedByRectangle() because:
        //- it does not give good end lines;
        //- it does not accept expilicitly TextAutoInsertSpace
        //public string GetTextSurroundedByRectangle(Bitmap b, RectangleF r, PageSegMode pageSegMode)
        //{
        //    if (!getScaled(b, ref r))
        //        return string.Empty;
        //    using (Tesseract.Page page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), pageSegMode))
        //    {
        //        return page.GetText();
        //    }
        //}

        public List<CharBox> GetCharBoxsSurroundedByRectangle(TesseractOCR.Pix.Image b, RectangleF r, TesseractOCR.Enums.PageSegMode pageSegMode)
        {
            if (!getScaled(b, ref r))
                return null;
            using (TesseractOCR.Page page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), pageSegMode))
            {
                return getCharBoxs(page);
            }
        }
        bool getScaled(TesseractOCR.Pix.Image b, ref RectangleF r)
        {
            r = new RectangleF(r.X / Settings.Constants.Pdf2ImageResolutionRatio, r.Y / Settings.Constants.Pdf2ImageResolutionRatio, r.Width / Settings.Constants.Pdf2ImageResolutionRatio, r.Height / Settings.Constants.Pdf2ImageResolutionRatio);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
                return false;
            return true;
        }

        public List<CharBox> GetCharBoxs(TesseractOCR.Pix.Image b, TesseractOCR.Enums.PageSegMode pageSegMode)
        {
            using (TesseractOCR.Page page = engine.Process(b, pageSegMode))
            {
                return getCharBoxs(page);
            }
        }
        List<CharBox> getCharBoxs(TesseractOCR.Page page)
        {
            List<CharBox> cbs = new List<CharBox>();

            foreach (var block in page.Layout)
            {
                foreach (var paragraph in block.Paragraphs)
                {
                    foreach (var textLine in paragraph.TextLines)
                    {
                        foreach (var word in textLine.Words)
                        {
                            foreach (var symbol in word.Symbols)
                            {
                                if (symbol.BoundingBox.HasValue)
                                {
                                    var r = symbol.BoundingBox.Value;
                                    cbs.Add(new CharBox
                                    {
                                        Char = symbol.Text,
                                        R = new RectangleF(r.X1 * Settings.Constants.Pdf2ImageResolutionRatio, r.Y1 * Settings.Constants.Pdf2ImageResolutionRatio, r.Width * Settings.Constants.Pdf2ImageResolutionRatio, r.Height * Settings.Constants.Pdf2ImageResolutionRatio)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return cbs;
        }

        //public string GetHtml(Bitmap b, PageSegMode pageSegMode)
        //{
        //    using (Tesseract.Page page = engine.Process(b, pageSegMode))
        //    {
        //        return page.GetHOCRText(0, false);
        //    }
        //}

        public class CharBox : Page.CharBox
        {
            //public bool AutoInserted = false;
        }

        //public static string GetTextSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace, Template.CharFilter charFilter)
        //{
        //    return string.Join("\r\n", GetTextLinesSurroundedByRectangle(cbs, r, textAutoInsertSpace, charFilter));
        //}

        public static List<Page.Line<CharBox>> GetLinesSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace, CharFilter charFilter)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            return Page.GetLines(cbs, textAutoInsertSpace, charFilter);
        }

        public static List<string> GetTextLinesSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace, CharFilter charFilter)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            return Page.GetLines(cbs, textAutoInsertSpace, charFilter).Select(a => a.GetString()).ToList();
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(List<CharBox> cbs, System.Drawing.RectangleF r)
        {
            return cbs.Where(a => /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R)).ToList();
        }

        //public static string GetTextByTopLeftCoordinates(List<CharBox> orderedCbs, RectangleF r)
        //{
        //    orderedCbs = orderedCbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)).ToList();
        //    return orderedCbs.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n)).ToString();
        //}

        //public Bitmap CardinalDeskew(Bitmap b)//!!!debug
        //{
        //    Bitmap b2 = new Bitmap(b.Width, b.Height);
        //    using (Graphics g = Graphics.FromImage(b2))
        //    {
        //        Tesseract.Page page = getPage(b);
        //        Tesseract.PixToBitmapConverter c = new PixToBitmapConverter();
        //        using (var i = page.GetIterator())
        //        {
        //            do
        //            {
        //                string t = i.GetText(PageIteratorLevel.Block);

        //                Pix p = i.GetImage(PageIteratorLevel.Block, 0, out int x, out int y).Deskew(out Scew scew);
        //              //p=  p.Rotate(scew.Angle, RotationMethod.AreaMap);
        //                Bitmap b_ = c.Convert(p);
        //                //System.Drawing.Imaging.BitmapData bd = b2.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //                 //   System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, rawImageData, 0, w * h);
        //                 //bitmap.UnlockBits(bd);                        
        //                    g.DrawImage(b_, x, y);                 

        //                //string t3 = i.GetText(PageIteratorLevel.Para);
        //            } while (i.Next(PageIteratorLevel.Block));
        //        }
        //    }
        //    b.Dispose();
        //    return b2;
        //}
    }
}
