//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Tesseract;

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
            engine = new TesseractEngine(@"./tessdata", config.Language, config.EngineMode);
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
            public string Language = "eng";
            public EngineMode EngineMode = EngineMode.Default;
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

        TesseractEngine engine = null;

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

        public int DetectOrientationAngle(Bitmap b, out float confidence)
        {
            using (Tesseract.Page page = engine.Process(b, PageSegMode.OsdOnly))
            {
                page.DetectBestOrientation(out int o, out confidence);
                return o;
            }
        }

        public Orientation DetectOrientation(Bitmap b)
        {
            using (Tesseract.Page page = engine.Process(b, PageSegMode.OsdOnly))
            {
                return page.AnalyseLayout().GetProperties().Orientation;
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

        public List<CharBox> GetCharBoxsSurroundedByRectangle(Bitmap b, RectangleF r, PageSegMode pageSegMode)
        {
            if (!getScaled(b, ref r))
                return null;
            using (Tesseract.Page page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), pageSegMode))
            {
                return getCharBoxs(page);
            }
        }
        bool getScaled(Bitmap b, ref RectangleF r)
        {
            r = new RectangleF(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
                return false;
            return true;
        }

        public List<CharBox> GetCharBoxs(Bitmap b, PageSegMode pageSegMode)
        {
            using (Tesseract.Page page = engine.Process(b, pageSegMode))
            {
                return getCharBoxs(page);
            }
        }
        List<CharBox> getCharBoxs(Tesseract.Page page)
        {
            List<CharBox> cbs = new List<CharBox>();
            //string t = page.GetHOCRText(1, true);
            //var dfg = page.GetThresholdedImage();                        
            //Tesseract.Orientation o;
            //float c;
            // page.DetectBestOrientation(out o, out c);
            //  var l = page.AnalyseLayout();
            //var ti =   l.GetBinaryImage(Tesseract.PageIteratorLevel.Para);
            //Tesseract.Rect r;
            // l.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
            using (var i = page.GetIterator())
            {
                //int j = 0;
                //i.Begin();
                //do
                //{
                //    bool g = i.IsAtBeginningOf(Tesseract.PageIteratorLevel.Block);
                //    bool v = i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
                //    var bt = i.BlockType;
                //    //if (Regex.IsMatch(bt.ToString(), @"image", RegexOptions.IgnoreCase))
                //    //{
                //    //    //i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block,out r);
                //    //    Tesseract.Pix p = i.GetBinaryImage(Tesseract.PageIteratorLevel.Block);
                //    //    Bitmap b = Tesseract.PixConverter.ToBitmap(p);
                //    //    b.Save(Log.AppDir + "\\test" + (j++) + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //}
                //} while (i.Next(Tesseract.PageIteratorLevel.Block));
                //do
                //{
                //    do
                //    {
                //        do
                //        {
                //            do
                //        {
                do
                {
                    //if (i.IsAtBeginningOf(PageIteratorLevel.Block))
                    //{
                    //}
                    //if (i.IsAtBeginningOf(PageIteratorLevel.Para))
                    //{
                    //}
                    //if (i.IsAtBeginningOf(PageIteratorLevel.TextLine))
                    //{
                    //}

                    Rect r;
                    if (i.TryGetBoundingBox(PageIteratorLevel.Symbol, out r))
                    {
                        //if (i.IsAtBeginningOf(PageIteratorLevel.Word))
                        //{
                        //if (i.IsAtBeginningOf(PageIteratorLevel.Para))
                        //{
                        //    cbs.Add(new CharBox
                        //    {
                        //        Char = "\r\n",
                        //        AutoInserted = true,
                        //        R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                        //    });
                        //}//seems to work not well

                        //cbs.Add(new CharBox//worked well before autoinsert was moved
                        //{
                        //    Char = " ",
                        //    AutoInserted = true,
                        //    R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                        //});
                        //}
                        cbs.Add(new CharBox
                        {
                            Char = i.GetText(PageIteratorLevel.Symbol),
                            R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                        });
                    }
                } while (i.Next(PageIteratorLevel.Symbol));
                //            } while (i.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));
                //        } while (i.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                //    } while (i.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                //} while (i.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
            }
            return cbs;
        }

        public string GetHtml(Bitmap b, PageSegMode pageSegMode)
        {
            using (Tesseract.Page page = engine.Process(b, pageSegMode))
            {
                return page.GetHOCRText(0, false);
            }
        }

        public class CharBox : Page.CharBox
        {
            //public bool AutoInserted = false;
        }

        //public static string GetTextByTopLeftCoordinates(List<CharBox> orderedCbs, RectangleF r)
        //{
        //    orderedCbs = orderedCbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)).ToList();
        //    return orderedCbs.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n)).ToString();
        //}

        public static string GetTextSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace, Template.SizeF ignoreCharsBiggerThan)
        {
            return string.Join("\r\n", GetTextLinesSurroundedByRectangle(cbs, r, textAutoInsertSpace, ignoreCharsBiggerThan));
        }

        public static List<string> GetTextLinesSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace, Template.SizeF ignoreCharsBiggerThan)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            List<string> ls = new List<string>();
            foreach (Page.Line<CharBox> l in Page.GetLines(cbs, textAutoInsertSpace, ignoreCharsBiggerThan))
                ls.Add(l.GetString());
            return ls;
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(List<CharBox> cbs, System.Drawing.RectangleF r)
        {
            return cbs.Where(a => /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R)).ToList();
        }

        public static List<CharBox> GetOrdered(List<CharBox> orderedContainerCbs, List<CharBox> cbs)
        {
            List<CharBox> orderedCbs = new List<CharBox>();
            foreach (CharBox cb in orderedContainerCbs)
            {
                if (orderedCbs.Count == cbs.Count)
                    break;
                if (cbs.Contains(cb))
                    orderedCbs.Add(cb);
            }
            return orderedCbs;
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
