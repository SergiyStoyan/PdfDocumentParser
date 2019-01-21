//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tesseract;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// OCR routines
    /// </summary>
    internal class Ocr : IDisposable
    {
        public static Ocr This = new Ocr();

        ~Ocr()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_engine != null)
                {
                    _engine.Dispose();
                    _engine = null;
                }
            }
        }

        Tesseract.TesseractEngine engine
        {
            get
            {
                if (_engine == null)
                    _engine = new Tesseract.TesseractEngine(@"./tessdata", "eng", Tesseract.EngineMode.Default);
                //if(!_engine.SetVariable("preserve_interword_spaces", 1))
                //    throw new Exception("Not set!");
                return _engine;
            }
        }
        Tesseract.TesseractEngine _engine = null;

        public Orientation DetectOrientation(Bitmap b, out float confidence)
        {
            using (var page = engine.Process(b, PageSegMode.OsdOnly))
            {
                Orientation o;
                page.DetectBestOrientation(out o, out confidence);
                return o;
            }
        }

        public string GetText(Bitmap b, RectangleF r)
        {
            r = new RectangleF(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
                return null;
            using (var page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), PageSegMode.SingleBlock))
            {
                return page.GetText();
            }
        }

        public string GetHtml(Bitmap b)
        {
            using (var page = engine.Process(b, PageSegMode.SingleBlock))
            {
                return page.GetHOCRText(0, false);
            }
        }

        public static string GetText(List<CharBox> cbs)
        {
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return string.Join("\r\n", ls);
        }

        //public static string GetText(List<CharBox> orderedCbs)
        //{
        //    return orderedCbs.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n.Char)).ToString();
        //}

        public List<CharBox> GetCharBoxs(Bitmap b)
        {
            List<CharBox> cbs = new List<CharBox>();
            using (var page = engine.Process(b, PageSegMode.SingleBlock))
            {
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
                            if (i.IsAtBeginningOf(PageIteratorLevel.Word))
                            {
                                //if (i.IsAtBeginningOf(PageIteratorLevel.Para))
                                //{
                                //    cbs.Add(new CharBox
                                //    {
                                //        Char = "\r\n",
                                //        AutoInserted = true,
                                //        R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                                //    });
                                //}//seems to work not well
                                cbs.Add(new CharBox
                                {
                                    Char = " ",
                                    AutoInserted = true,
                                    R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                                });
                            }
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
            }
            return cbs;
        }

        public class CharBox
        {
            public string Char;
            public bool AutoInserted = false;
            public System.Drawing.RectangleF R;
        }

        //public static string GetTextByTopLeftCoordinates(List<CharBox> orderedCbs, RectangleF r)
        //{
        //    orderedCbs = orderedCbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)).ToList();
        //    return orderedCbs.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n)).ToString();
        //}

        public static string GetTextByTopLeftCoordinates(List<CharBox> cbs, RectangleF r)
        {
            cbs = cbs.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)).ToList();
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return string.Join("\r\n", ls);
        }

        public static List<Line> GetLines(IEnumerable<CharBox> cbs)
        {
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
            //foreach (Line l in lines)
            //    l.CharBoxs = l.CharBoxs.OrderBy(a => a.R.X).ToList();
            return lines;
        }
        public class Line
        {
            public float Top;
            public float Bottom;
            public List<CharBox> CharBoxs = new List<CharBox>();
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(List<CharBox> cbs, System.Drawing.RectangleF r)
        {
            return cbs.Where(a => !a.AutoInserted && /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R)).ToList();
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
    }
}
