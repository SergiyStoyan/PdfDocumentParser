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
using Sdcb.SimdPaddleOCR;
using Sdcb.SimdPaddleOCR.Models.ChineseV6Medium;
using System.Drawing.Imaging;
using Sdcb.SimdPaddleOCR.ModelProvider;
using System.Runtime.InteropServices;
using System.IO;


namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// OCR routines
    /// </summary>
    public class Ocr : IDisposable
    {
        public void Dispose()
        {
            lock (this)
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

                engine?.Dispose();
                engine = null;
                detector?.Dispose();
                detector = null;
                //detectorStream?.Dispose();
                //detectorStream = null;
            }
        }
        ~Ocr()
        {
            Dispose();
        }

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

        public string RecognizeImage(Bitmap bitmap)
        {
            // Ensure bitmap is 24-bit BGR format (SimdPaddleOCR's native default)
            using (Bitmap bgrBitmap = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? (Bitmap)bitmap.Clone()
                : bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb))
            {
                var rect = new Rectangle(0, 0, bgrBitmap.Width, bgrBitmap.Height);
                BitmapData bmpData = bgrBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                try
                {
                    // Copy locked pixel buffer into a managed byte array
                    int byteCount = Math.Abs(bmpData.Stride) * bgrBitmap.Height;
                    byte[] pixels = new byte[byteCount];
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);

                    // Run OCR passing width, height, and row stride
                    PaddleOcrResult result = engine.Run(
                        pixels,
                        bgrBitmap.Width,
                        bgrBitmap.Height,
                        bmpData.Stride,
                        ImagePixelFormat.Bgr24);

                    return result.Text;
                }
                finally
                {
                    bgrBitmap.UnlockBits(bmpData);
                }
            }
        }

        ///// <summary>
        ///// (!!!)After use, call UnlockBits() and Dispose() on bgrBitmap
        ///// </summary>
        ///// <param name="bitmap"></param>
        ///// <returns></returns>
        //(byte[] pixels, Bitmap bgrBitmap, BitmapData bmpData) getImageBytes(Bitmap bitmap)
        //{
        //    var bgrBitmap = bitmap.PixelFormat == PixelFormat.Format24bppRgb
        //         ? (Bitmap)bitmap.Clone()
        //         : bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
        //    var rect = new Rectangle(0, 0, bgrBitmap.Width, bgrBitmap.Height);
        //    var bmpData = bgrBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        //    // Copy locked pixel buffer into a managed byte array
        //    int byteCount = Math.Abs(bmpData.Stride) * bgrBitmap.Height;
        //    byte[] pixels = new byte[byteCount];
        //    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);
        //    return (pixels, bgrBitmap, bmpData);
        //}
        //void ungetImageBytes((byte[] pixels, Bitmap bgrBitmap, BitmapData bmpData) imageBytes)
        //{
        //    imageBytes.bgrBitmap.UnlockBits(imageBytes.bmpData);
        //    imageBytes.bgrBitmap.Dispose();
        //}

        Ocr(Config config)
        {
            //// Omit the Classification model so upside-down (180°) text is not auto-flipped
            //// and scores noticeably lower than upright text
            //var noFlipBundle = new PaddleOcrModelBundle(
            //    config.PaddleOcrModelBundle.Name,
            //    config.PaddleOcrModelBundle.LanguageCode,
            //    config.PaddleOcrModelBundle.Detection,
            //    config.PaddleOcrModelBundle.Recognition,
            //    config.PaddleOcrModelBundle.Dictionary
            //    );
            engine = PaddleOcrAll.Load(config.PaddleOcrModelBundle);
            detector = new PaddleOcrDetector(config.PaddleOcrModelBundle.Detection.OpenRead());
        }
        public class Config
        {
            public PaddleOcrModelBundle PaddleOcrModelBundle = ChineseV6MediumModels.Default;
        }

        PaddleOcrAll engine = null;
        PaddleOcrDetector detector = null;
        //Stream detectorStream = null;

        private static byte[] extractBgraPixels(Bitmap bitmap, out int stride, Rectangle? region = null)
        {
            Rectangle r = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            if (region != null)
            {
                r = Rectangle.Intersect(r, region.Value);
                if (r.Width <= 0 || r.Height <= 0)
                    throw new ArgumentOutOfRangeException(nameof(region), "Region does not overlap the bitmap.");
            }
            BitmapData data = bitmap.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                stride = data.Stride;
                byte[] buffer = new byte[stride * r.Height];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                return buffer;
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        public int DetectOrientationAngle(Bitmap bitmap/*, out float confidence*/)
        {
            // 1. Detect boxes to check if the page is horizontal (0°/180°) or sideways (90°/270°)
            byte[] pixels = extractBgraPixels(bitmap, out int stride);
            PaddleOcrDetectionResult detection = detector.Detect(pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);

            var boxes = detection?.Boxes;
            if (boxes == null || boxes.Length == 0)
                return 0;

            int baseAngle = 0;
            int verticalCount = boxes.Count(b => b.Height > b.Width * 1.2f);
            if (verticalCount > boxes.Length / 2)
            {
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                baseAngle = 90;
                pixels = extractBgraPixels(bitmap, out stride);
            }

            // 2. Score at baseAngle (0° or 90°) using RecognitionScore
            PaddleOcrResult firstPass = engine.Run(pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);
            float firstScore = firstPass.Lines.Length > 0 ? firstPass.Lines.Average(l => l.RecognitionScore) : 0f;

            // 3. Rotate 180° and score again to check if the page was upside-down
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pixels = extractBgraPixels(bitmap, out stride);
            PaddleOcrResult secondPass = engine.Run(pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);
            float secondScore = secondPass.Lines.Length > 0 ? secondPass.Lines.Average(l => l.RecognitionScore) : 0f;

            if (secondScore > firstScore)
            {
                // Keep the 180°-rotated bitmap; total rotation was (baseAngle + 180) % 360
                return (baseAngle + 180) % 360;
            }

            // First angle was better — rotate back 180°
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return baseAngle;
        }

        //public Orientation DetectOrientation(Bitmap b)
        //{
        //    using (Tesseract.Page page = engine.Process(b, PageSegMode.OsdOnly))
        //    {
        //        return page.AnalyseLayout().GetProperties().Orientation;
        //    }
        //}

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

        public List<CharBox> GetCharBoxsSurroundedByRectangle(Bitmap bitmap, RectangleF r/*, PageSegMode pageSegMode*/)
        {
            if (!getScaled(bitmap, ref r))
                return null;
            byte[] pixels = extractBgraPixels(bitmap, out int stride, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
            PaddleOcrResult por = engine.Run(pixels, (int)r.Width, (int)r.Height, stride, ImagePixelFormat.Bgra32);
            return getCharBoxs(por);
        }
        bool getScaled(Bitmap b, ref RectangleF r)
        {
            r = new RectangleF(r.X / Settings.Constants.Pdf2ImageResolutionRatio, r.Y / Settings.Constants.Pdf2ImageResolutionRatio, r.Width / Settings.Constants.Pdf2ImageResolutionRatio, r.Height / Settings.Constants.Pdf2ImageResolutionRatio);
            r.Intersect(new Rectangle(0, 0, b.Width, b.Height));
            if (Math.Abs(r.Width) < Settings.Constants.CoordinateDeviationMargin || Math.Abs(r.Height) < Settings.Constants.CoordinateDeviationMargin)
                return false;
            return true;
        }

        public List<CharBox> GetCharBoxs(Bitmap bitmap/*, PageSegMode pageSegMode*/)
        {
            byte[] pixels = extractBgraPixels(bitmap, out int stride);
            PaddleOcrResult por = engine.Run(pixels, bitmap.Width, bitmap.Height, stride, ImagePixelFormat.Bgra32);
            return getCharBoxs(por);
        }

        List<CharBox> getCharBoxs(PaddleOcrResult por)
        {
            var charBoxes = new List<CharBox>();

            if (por?.Lines == null)
                return charBoxes;

            foreach (var line in por.Lines)
            {
                foreach (var c in line.EstimateCharacterBoxes())
                    charBoxes.Add(new CharBox
                    {
                        Char = c.Text,
                        R = new RectangleF(c.X1, c.Y1, c.X2 - c.X1, c.Y2 - c.Y1)
                    });
            }
            return charBoxes;
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
