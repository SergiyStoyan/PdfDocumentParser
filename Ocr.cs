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
    /// Auxiliary OCR routines
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
            if (_engine != null)
            {
                _engine.Dispose();
                _engine = null;
            }
        }

        Tesseract.TesseractEngine engine
        {
            get
            {
                if (_engine == null)
                    _engine = new Tesseract.TesseractEngine(@"./tessdata", "eng", Tesseract.EngineMode.Default);
                return _engine;
            }
        }
        Tesseract.TesseractEngine _engine = null;

        public string GetText(Bitmap b, RectangleF r)
        {
            r = new RectangleF(r.X / Settings.ImageProcessing.Image2PdfResolutionRatio, r.Y / Settings.ImageProcessing.Image2PdfResolutionRatio, r.Width / Settings.ImageProcessing.Image2PdfResolutionRatio, r.Height / Settings.ImageProcessing.Image2PdfResolutionRatio);            
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.ImageProcessing.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.ImageProcessing.CoordinateDeviationMargin)
                return null;
            using (var page = engine.Process(b, new Rect((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height), PageSegMode.SingleBlock))
            {
                return page.GetText();
            }
        }

        public string GetHtml(Bitmap b)
        {
            using (var page = engine.Process(b, PageSegMode.Auto))
            {
                return page.GetHOCRText(0, false);
            }
        }

        public List<CharBox> GetCharBoxs(Bitmap b)
        {
            List<CharBox> cbs = new List<CharBox>();
            using (var page = engine.Process(b, PageSegMode.Auto))
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
                    //            {
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
                        //if (i.IsAtBeginningOf(PageIteratorLevel.Word))
                        //{
                        //}

                        Rect r;
                        if (i.TryGetBoundingBox(PageIteratorLevel.Symbol, out r))
                            cbs.Add(new CharBox
                            {
                                Char = i.GetText(PageIteratorLevel.Symbol),
                                R = new RectangleF(r.X1 * Settings.ImageProcessing.Image2PdfResolutionRatio, r.Y1 * Settings.ImageProcessing.Image2PdfResolutionRatio, r.Width * Settings.ImageProcessing.Image2PdfResolutionRatio, r.Height * Settings.ImageProcessing.Image2PdfResolutionRatio)
                            });
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
            public System.Drawing.RectangleF R;
        }

        public static string GetTextByTopLeftCoordinates(List<CharBox> bts, RectangleF r)
        {
            //r = new RectangleF(r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);
            bts = RemoveDuplicatesAndOrder(bts.Where(a => (r.Contains(a.R) /*|| d.IntersectsWith(a.R)*/)));
            StringBuilder sb = new StringBuilder(bts.Count > 0 ? bts[0].Char : "");
            for (int i = 1; i < bts.Count; i++)
            {
                if (Math.Abs(bts[i - 1].R.Y - bts[i].R.Y) > bts[i - 1].R.Height / 2)
                    sb.Append("\r\n");
                else if (Math.Abs(bts[i - 1].R.Right - bts[i].R.X) > Math.Min(bts[i - 1].R.Width, bts[i].R.Width) / 2)
                    sb.Append(" ");
                sb.Append(bts[i].Char);
            }
            return sb.ToString();
        }

        public static List<CharBox> RemoveDuplicatesAndOrder(IEnumerable<CharBox> bts)
        {
            List<CharBox> bs = bts.Where(a => a.R.Width >= 0 && a.R.Height >= 0).ToList();//some symbols are duplicated with negative width and height
            for (int i = 0; i < bs.Count; i++)
                for (int j = bs.Count - 1; j > i; j--)
                {
                    if (Math.Abs(bs[i].R.X - bs[j].R.X) > Settings.ImageProcessing.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (Math.Abs(bs[i].R.Y - bs[j].R.Y) > Settings.ImageProcessing.CoordinateDeviationMargin)//some symbols are duplicated in [almost] same position
                        continue;
                    if (bs[i].Char != bs[j].Char)
                        continue;
                    bs.RemoveAt(j);
                }
            return bs.OrderBy(a => a.R.Y).OrderBy(a => a.R.X).ToList();
        }

        public static List<CharBox> GetCharBoxsSurroundedByRectangle(List<CharBox> bts, System.Drawing.RectangleF r)
        {
            //r = new RectangleF(r.X / Settings.General.Image2PdfResolutionRatio, r.Y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);
            return bts.Where(a => /*selectedR.IntersectsWith(a.R) || */r.Contains(a.R)).ToList();
        }
    }
}
