using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Cliver.InvoiceParser
{
    class TesseractW : IDisposable
    {
        public static TesseractW This = new TesseractW();

        ~TesseractW()
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

        public string GetText(Bitmap b, float x, float y, float w, float h)
        {
            Rectangle r = new Rectangle((int)x, (int)y, (int)w, (int)h);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.General.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.General.CoordinateDeviationMargin)
                return null;
            using (var page = engine.Process(b, new Tesseract.Rect(r.X, r.Y, r.Width, r.Height), Tesseract.PageSegMode.SingleBlock))
            {
                return page.GetText();
            }
        }

        public void GetChars(Bitmap b)
        {
            using (var page = engine.Process(b, Tesseract.PageSegMode.Auto))
            {
                string t = page.GetHOCRText(1, true);
                //var dfg = page.GetThresholdedImage();                        
                Tesseract.Orientation o;
                float c;
                // page.DetectBestOrientation(out o, out c);
                //  var l = page.AnalyseLayout();
                //var ti =   l.GetBinaryImage(Tesseract.PageIteratorLevel.Para);
                Tesseract.Rect r;
                // l.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
                using (var i = page.GetIterator())
                {
                    int j = 0;
                    i.Begin();
                    do
                    {
                        bool g = i.IsAtBeginningOf(Tesseract.PageIteratorLevel.Block);
                        bool v = i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
                        var bt = i.BlockType;
                        //if (Regex.IsMatch(bt.ToString(), @"image", RegexOptions.IgnoreCase))
                        //{
                        //    //i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block,out r);
                        //    Tesseract.Pix p = i.GetBinaryImage(Tesseract.PageIteratorLevel.Block);
                        //    Bitmap b = Tesseract.PixConverter.ToBitmap(p);
                        //    b.Save(Log.AppDir + "\\test" + (j++) + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        //}
                    } while (i.Next(Tesseract.PageIteratorLevel.Block));
                }
            }
        }
    }
}
