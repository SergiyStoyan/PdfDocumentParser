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

        public string GetTextSurroundedByRectangle(Bitmap b, RectangleF r)
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
            using (Tesseract.Page page = engine.Process(b, PageSegMode.SingleBlock))
            {
                return page.GetHOCRText(0, false);
            }
        }

        public List<CharBox> GetCharBoxs(Bitmap b)
        {
            List<CharBox> cbs = new List<CharBox>();
            using (Tesseract.Page page = engine.Process(b, PageSegMode.SingleBlock))
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

                                //cbs.Add(new CharBox//worked well before autoinsert was moved
                                //{
                                //    Char = " ",
                                //    AutoInserted = true,
                                //    R = new RectangleF(r.X1 * Settings.Constants.Image2PdfResolutionRatio - Settings.Constants.CoordinateDeviationMargin * 2, r.Y1 * Settings.Constants.Image2PdfResolutionRatio, r.Width * Settings.Constants.Image2PdfResolutionRatio, r.Height * Settings.Constants.Image2PdfResolutionRatio)
                                //});
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

        //public static string GetText(List<CharBox> orderedCbs)
        //{
        //    return orderedCbs.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n.Char)).ToString();
        //}

        public static string GetText(List<CharBox> cbs, TextAutoInsertSpace textAutoInsertSpace)
        {
            List<string> ls = new List<string>();
            foreach (Line l in GetLines(cbs, textAutoInsertSpace))
            {
                StringBuilder sb = new StringBuilder();
                foreach (CharBox cb in l.CharBoxs)
                    sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return string.Join("\r\n", ls);
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
                            if (/*cb0.Char != " " && */cb.R.Left - cb0.R.Right > (cb0.R.Width + cb0.R.Height + cb.R.Width + cb.R.Height) * textAutoInsertSpace.Threshold / 100)
                            {
                                float spaceWidth = (cb0.R.Width + cb.R.Width) / 2;
                                int spaceNumber = (int)Math.Ceiling((cb.R.Left - cb0.R.Right) / spaceWidth);
                                for (int j = 0; j < spaceNumber; j++)
                                    lines[i].CharBoxs.Add(new CharBox { Char = textAutoInsertSpace.Representative, R = new System.Drawing.RectangleF(cb0.R.Right + spaceWidth * j, cb0.R.Y, spaceWidth, cb.R.Height) });
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

        public class Line
        {
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
