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
    }
}