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
            if (x < 0)
            {
                w += x;
                x = 0;
            }
            if (y < 0)
            {
                h += y;
                y = 0;
            }
            if (b.Width < x + w)
                w = b.Width - x;
            if (b.Height < y + h)
                h = b.Height - y;
            if (Math.Abs(w) < Settings.General.CoordinateDeviationMargin || Math.Abs(h) < Settings.General.CoordinateDeviationMargin)
                return null;
            using (var page = engine.Process(b, new Tesseract.Rect((int)x, (int)y, (int)w, (int)h), Tesseract.PageSegMode.SingleBlock))
            {
                return page.GetText();
            }
        }
    }
}