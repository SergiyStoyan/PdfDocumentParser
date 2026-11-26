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
using PaddleOCRSharp;

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
            if (config == null)
                throw new Exception($"{nameof(Config)} is not set!");

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

            //!!! Paddle.Runtime.win_x64 nuget may be needed
            engine2 = new PaddleOCREngine(config.OCRModelConfig, config.OCRParameter);
        }
        public class Config
        {
            public TesseractOCR.Enums.Language Language = TesseractOCR.Enums.Language.English;
            public TesseractOCR.Enums.EngineMode EngineMode = TesseractOCR.Enums.EngineMode.Default;
            public Dictionary<string, object> VariableNames2value = null;

            //internal string Key
            //{
            //    get
            //    {
            //        if (key == null)
            //            key = this.ToStringByJson(false);
            //        return key;
            //    }
            //}
            //string key = null;

            public OCRModelConfig OCRModelConfig = null; // Uses default embedded models
            public OCRParameter OCRParameter = new OCRParameter
            {
                cls = true,            // Runs the classifier during inference
                enable_mkldnn = true,
                //det = false,           // Skip text detection; treat the entire crop as one text line
                rec = true,            // Run recognition directly
                //use_angle_cls = true,  // Enables text-line orientation classification (0°/180°)
                use_angle_cls = false  // Skip angle classification if orientation is already known
            };
        }

        TesseractOCR.Engine engine = null;
        PaddleOCRSharp.PaddleOCREngine engine2 = null;

        ~Ocr()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                engine?.Dispose();
                engine = null;
                engine2?.Dispose();
                engine2 = null;
            }
        }

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

        public OCRResult GetOCRResultSurroundedByRectangle2(Bitmap b, RectangleF r, int padding)
        {
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (r.Width <= 0 || r.Height <= 0)
                return new OCRResult();

            var rr = new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
            using var paddedCrop = new Bitmap(rr.Width + padding * 2, rr.Height + padding * 2);
            using (var g = Graphics.FromImage(paddedCrop))
            {
                g.Clear(Color.White);
                g.DrawImage(b, destRect: new Rectangle(padding, padding, rr.Width, rr.Height), srcRect: r, srcUnit: GraphicsUnit.Pixel);
            }

            OCRResult result = engine2.DetectText(paddedCrop);
            // 4. Translate detected BoxPoints back to full-page coordinates
            if (result?.TextBlocks != null)
            {
                int offsetX = rr.X - padding;
                int offsetY = rr.Y - padding;

                foreach (var block in result.TextBlocks)
                {
                    if (block.BoxPoints == null) continue;
                    for (int i = 0; i < block.BoxPoints.Count; i++)
                    {
                        var pt = block.BoxPoints[i];
                        block.BoxPoints[i] = new OCRPoint(pt.X + offsetX, pt.Y + offsetY);
                    }
                }
            }
            return result;
        }

        public string GetTextSurroundedByRectangle2(Bitmap b, RectangleF r, int padding)
        {
            if (!getScaled(b, ref r))
                return null;
            var or = GetOCRResultSurroundedByRectangle2(b, r, padding);
            return or.Text;
        }
        bool getScaled(Bitmap b, ref RectangleF r)
        {
            r = new RectangleF(r.X / Settings.Constants.Pdf2ImageResolutionRatio, r.Y / Settings.Constants.Pdf2ImageResolutionRatio, r.Width / Settings.Constants.Pdf2ImageResolutionRatio, r.Height / Settings.Constants.Pdf2ImageResolutionRatio);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
                return false;
            return true;
        }

        public List<string> GetTextLinesSurroundedByRectangle2(Bitmap b, RectangleF r, int padding)
        {
            if (!getScaled(b, ref r))
                return null;
            var or = GetOCRResultSurroundedByRectangle2(b, r, padding);
            return or.TextBlocks.Select(a => a.Text).ToList();
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
