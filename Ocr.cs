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
                    _This = new Ocr();
                return _This;
            }
        }
        static Ocr _This = null;

        Ocr()
        {
            engine = new TesseractEngine(@"./tessdata", config.language, config.engineMode);
            if (config.variables != null)
            {
                string m1 = "Could not set Tesseract variable: ";
                foreach (var v in config.variables)
                    if (v.value is string)
                    {
                        if (!engine.SetVariable(v.name, (string)v.value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.value is int)
                    {
                        if (!engine.SetVariable(v.name, (int)v.value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.value is double)
                    {
                        if (!engine.SetVariable(v.name, (double)v.value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else if (v.value is bool)
                    {
                        if (!engine.SetVariable(v.name, (bool)v.value))
                            throw new Exception(m1 + v.ToString());
                    }
                    else
                        throw new Exception(m1 + v.ToString() + " Not supported type.");
            }
        }
        static Config config = //new Config();
        new Config
        {
            language = "eng",
            engineMode = EngineMode.TesseractOnly,
            variables = new List<(string name, object value)> {
                        (name: "load_system_dawg", value: false),
                        (name: "load_freq_dawg", value: false),
                        //(name: "tessedit_char_whitelist", "0123456789.,"),
                    }
        };
        public class Config
        {
            public string language = "eng";
            //public string language = "superior";
            public EngineMode engineMode = EngineMode.Default;
            public List<(string name, object value)> variables = null;
        }

        static void Initialize(string language = "eng", EngineMode engineMode = EngineMode.Default, List<(string name, object value)> variables = null)
        {
            Config newConfig = new Config { language = language, engineMode = engineMode, variables = variables };
            if (newConfig.IsEqualByJson(config))
                return;
            config = newConfig;
            _This?.Dispose();
            _This = null;
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

        public float DetectDeskewAngle(Bitmap b)
        {
            using (Tesseract.Page page = engine.Process(b, PageSegMode.OsdOnly))
            {
                return page.AnalyseLayout().GetProperties().DeskewAngle;
            }
        }

        public string GetTextSurroundedByRectangle(Bitmap b, RectangleF r, PageSegMode pageSegMode)
        {
            if (!getScaled(b, ref r))
                return string.Empty;
            using (Tesseract.Page page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), pageSegMode))
            {
                return page.GetText();
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

        public List<CharBox> GetCharBoxsSurroundedByRectangle(Bitmap b, RectangleF r, PageSegMode pageSegMode)
        {
            if (!getScaled(b, ref r))
                return null;
            using (Tesseract.Page page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), pageSegMode))
            {
                return getCharBoxs(page);
            }
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

        public static List<string> GetTextLines(List<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs, textAutoInsertSpace))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return ls;
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

        public static string GetTextSurroundedByRectangle(List<CharBox> cbs, RectangleF r, TextAutoInsertSpace textAutoInsertSpace)
        {
            return string.Join("\r\n", GetTextLinesSurroundedByRectangle(cbs, r, textAutoInsertSpace));
        }

        public static List<string> GetTextLinesSurroundedByRectangle(List<CharBox> cbs, System.Drawing.RectangleF r, TextAutoInsertSpace textAutoInsertSpace)
        {
            cbs = GetCharBoxsSurroundedByRectangle(cbs, r);
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs, textAutoInsertSpace))
                ls.Add(l.ToString());
            return ls;
        }

        public static List<Line> GetLines(IEnumerable<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            bool spaceAutoInsert = textAutoInsertSpace != null && textAutoInsertSpace.Threshold > 0;
            cbs = cbs.OrderBy(a => a.R.X).ToList();
            List<Line> lines = new List<Line>();
            foreach (CharBox cb in cbs)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (cb.R.Bottom < lines[i].Top)
                    {
                        Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                        l.CharBoxs.Add(cb);
                        lines.Insert(i, l);
                        goto CONTINUE;
                    }
                    if (cb.R.Bottom - cb.R.Height / 2 <= lines[i].Bottom)
                    {
                        if (spaceAutoInsert && /*cb.Char != " " &&*/ lines[i].CharBoxs.Count > 0)
                        {
                            CharBox cb0 = lines[i].CharBoxs[lines[i].CharBoxs.Count - 1];
                            if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (/*cb0.R.Width*/0.8 / cb0.R.Height + /*cb.R.Width*/0.8 / cb.R.Height) * textAutoInsertSpace.Threshold)
                            {
                                float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                                int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                                for (int j = 0; j < spaceNumber; j++)
                                    lines[i].CharBoxs.Add(new CharBox { Char = textAutoInsertSpace.Representative, R = new RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
                            }
                        }
                        lines[i].CharBoxs.Add(cb);
                        if (lines[i].Top > cb.R.Top)
                            lines[i].Top = cb.R.Top;
                        if (lines[i].Bottom < cb.R.Bottom)
                            lines[i].Bottom = cb.R.Bottom;
                        goto CONTINUE;
                    }
                }
                {
                    Line l = new Line { Top = cb.R.Top, Bottom = cb.R.Bottom };
                    l.CharBoxs.Add(cb);
                    lines.Add(l);
                }
            CONTINUE:;
            }
            return lines;
        }

        internal static List<Line> GetLinesWithAdjacentBorders(IEnumerable<CharBox> cbs, RectangleF ar)
        {
            List<Line> ls = GetLines(cbs, null);
            for (int i = 0; i < ls.Count; i++)
            {
                Line l = ls[i];
                if (ar.Top > l.Top)
                    continue;
                if (ar.Bottom < l.Bottom)
                    continue;
                if (i == 0)
                    l.Top = (l.Bottom - l.Top) < (l.Top - ar.Top) ? (l.Bottom + l.Top) / 2 : ar.Top;
                if (i < ls.Count - 1)
                {
                    l.Bottom = (ls[i + 1].Top + l.Bottom) / 2;
                    ls[i + 1].Top = l.Bottom;
                }
                else
                    l.Bottom = (l.Bottom - l.Top) < (ar.Bottom - l.Bottom) ? (l.Bottom + l.Top) / 2 : ar.Bottom;
            }
            return ls;
        }

        public class Line
        {
            public float Left { get { return CharBoxs[0].R.Left; } }
            public float Right { get { return CharBoxs[CharBoxs.Count - 1].R.Right; } }
            public float Top;
            public float Bottom;
            public List<CharBox> CharBoxs = new List<CharBox>();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in CharBoxs)
                    sb.Append(cb.Char);
                return sb.ToString();
            }
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
