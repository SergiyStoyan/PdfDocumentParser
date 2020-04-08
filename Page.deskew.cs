////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        http://www.cliversoft.com
////********************************************************************************************
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Drawing.Drawing2D;

//namespace Cliver.PdfDocumentParser
//{
//    /// <summary>
//    /// pdf page parsing API
//    /// </summary>
//    public partial class Page : IDisposable
//    {
//        static void deskew(Bitmap img, int stripCount, bool isVertical = false)
//        {
//            const int MaxHeight = 600;
//            float Scale = img.Height > MaxHeight ? 1f * MaxHeight / img.Height : 1f;

//            int Height = (int)((isVertical ? img.Width : img.Height) * Scale);
//            int Width = stripCount;

//            var w = isVertical ? Height : Width;
//            var h = isVertical ? Width : Height;

//            using (var bmp = new Bitmap(w, h))
//            using (var gr = Graphics.FromImage(bmp))
//            {
//                gr.InterpolationMode = InterpolationMode.Low;
//                gr.DrawImage(img, 0, 0, w, h);

//                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
//                int stride = bmpData.Stride;
//                byte[] data = new byte[stride * Height];
//                byte getColor(int x, int y)
//                {
//                    var i = (x < 0 || x >= Width || y < 0 || y >= Height) ? -1 : x * 4 + y * stride;
//                    return (i < 0 ? DefaultColor : Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i])).G;
//                }
//                {
//                    int[][] Strips = new int[Width][];
//                    for (int x = 0; x < Strips.Length; x++)
//                    {
//                        var strip = Strips[x] = new int[Height];
//                        for (int y = 0; y < strip.Length; y++) strip[y] = getColor(y, x);
//                    }
//                }
//            }
//            var stripX1 = 2;//take 3-rd strip
//            var stripX2 = 6;//take 7-th strip

//            var angle = findRotateAngle(compact, stripX1, stripX2);
//            angle = (angle * 180 / Math.PI);//to degrees
//        }

//        static double findRotateAngle(Compact compact, int stripX1, int stripX2, int maxSkew = 70)
//        {
//            var shift = 0d;
//            var shift1 = findBestShift(compact, stripX1, stripX2, -maxSkew, maxSkew);
//            var shift2 = findBestShift(compact, stripX1 + 1, stripX2 + 1, -maxSkew, maxSkew);

//            if (Math.Abs(shift1 - shift2) < 2)
//                shift = (shift1 + shift2) / 2d;
//            else
//            if (Math.Abs(shift1) < Math.Abs(shift2))
//                shift = shift1;
//            else
//                shift = shift2;

//            return Math.Atan2(shift / compact.Scale, (stripX2 - stripX1) * compact.SourceWidth / compact.Width);
//        }

//        static int findBestShift(int height, int[] strip1, int[] strip2, int startShift, int endShift)
//        {
//            var bestCorr = double.MinValue;
//            var bestShift = 0;

//            for (int shift = startShift; shift <= endShift; shift++)
//            {
//                var startIndex = shift < 0 ? -shift : 0;
//                var endIndex = shift > 0 ? height - shift - 1 : height - 1;

//                var res = 0;
//                for (int i = startIndex; i <= endIndex; i++)
//                    res += -Math.Abs(strip1[i] - strip2[i + shift]);

//                double corr = 1d * strip1.Length * res / (strip1.Length - Math.Abs(shift));
//                if (corr > bestCorr)
//                {
//                    bestCorr = corr;
//                    bestShift = shift;
//                }
//            }

//            return bestShift;
//        }

//        public static Bitmap Rotate(Bitmap img, double angle)
//        {
//            var rotated = new Bitmap(img.Width, img.Height);
//            using (var gr = Graphics.FromImage(rotated))
//            {
//                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                gr.TranslateTransform(img.Width / 2, img.Height / 2);
//                gr.RotateTransform(-(float)angle);
//                gr.DrawImage(img, -img.Width / 2, -img.Height / 2, img.Width, img.Height);
//            }

//            return rotated;
//        }
//    }
//}