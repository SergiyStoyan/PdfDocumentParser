//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Cliver
{
    public static class ImageRoutines
    {
        public static Bitmap GetCopy(Image image, Rectangle r)
        {
            Bitmap b = new Bitmap(r.Width, r.Height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
            {
                g.DrawImage(image, 0, 0, r, GraphicsUnit.Pixel);
            }
            return b;
        }

        public static Bitmap GetCopy(Image image)
        {
            return GetCopy(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        public static Bitmap GetCopy(Image image, RectangleF r)
        {
            return GetCopy(image, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
        }

        public static Bitmap GetResized(Image image, int width, int height)
        {
            var b = new Bitmap(width, height);
            using (var g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, 0, 0, b.Width, b.Height);
            }
            return b;
        }

        public static Bitmap GetScaled(Image image, Size max_size, out float ratio)
        {
            ratio = Math.Min((float)max_size.Width / image.Width, (float)max_size.Height / image.Height);
            return GetResized(image, (int)Math.Round(image.Width * ratio, 0), (int)Math.Round(image.Height * ratio, 0));
        }

        public static Bitmap GetScaled(Image image, Size max_size)
        {
            float ratio;
            return GetScaled(image, max_size, out ratio);
        }

        public static Bitmap GetScaled(Image image, float ratio)
        {
            return GetResized(image, (int)Math.Round(image.Width * ratio, 0), (int)Math.Round(image.Height * ratio, 0));
        }

        public static Bitmap GetCroppedByColor(Image image, Color color)
        {
            Bitmap b = new Bitmap(image);
            int y = b.Height, height = 0, x = b.Width, width = 0;
            for (int h = 0; h < b.Height; h++)
            {
                bool another_color_found = false;
                int wl = 0;
                for (; wl < b.Width; wl++)
                {
                    Color c = b.GetPixel(wl, h);
                    if (c != color)
                    {
                        another_color_found = true;
                        if (x > wl)
                            x = wl;
                        break;
                    }
                }
                for (int wr = b.Width - 1; wr > wl; wr--)
                {
                    Color c = b.GetPixel(wr, h);
                    if (c != color)
                    {
                        another_color_found = true;
                        if (width <= wr)
                            width = wr + 1;
                        break;
                    }
                }
                if (another_color_found)
                {
                    if (height <= h)
                        height = h + 1;
                    if (y > h)
                        y = h;
                }
            }
            if (y >= b.Height)
                return new Bitmap(0, 0);
            b = new Bitmap(width - x, height - y);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, x, y, width, height);
            }
            //b.Save("2.png");
            return b;
        }

        public static Bitmap GetGreyScale(Bitmap b)
        {
            Bitmap b2 = new Bitmap(b);
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    Color c = b.GetPixel(x, y);
                    int gc = (c.R + c.G + c.B) / 3;
                    Color c2 = Color.FromArgb(c.A, gc, gc, gc);
                    b2.SetPixel(x, y, c2);
                }
            }
            return b2;
        }

        public static Bitmap GetInverted(Bitmap b)
        {
            Bitmap b2 = new Bitmap(b);
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    Color c = b.GetPixel(x, y);
                    Color c2 = Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B);
                    b2.SetPixel(x, y, c2);
                }
            }
            return b2;
        }

        public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Icon icon)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        //var g = Convert.ToBase64String(ImageProcessor.GetBitmapHash(new Bitmap(@"d:\temp\b2.png")));
        public static byte[] GetBitmapMd5Hash(Bitmap bitmap, int hashResolution = 16)
        {
            byte[] rawImageData = new byte[hashResolution * hashResolution];
            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, hashResolution, hashResolution), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bd.Scan0, rawImageData, 0, hashResolution * hashResolution * 4);
            bitmap.UnlockBits(bd);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(rawImageData);
        }

        //public static bool BitmapHasSimilarHash(Bitmap bitamp, byte[,] hash, float hashImageRatio, float brightnessTolerance, float differentPixelNumberTolerance)
        //{
        //    return FindBitmapFragmentByHash(bitamp, hash, hashImageRatio, brightnessTolerance, differentPixelNumberTolerance) != null;
        //}

        public static bool BitmapHashesAreSimilar(byte[,] hash1, byte[,] hash2, float brightnessTolerance, float differentPixelNumberTolerance)
        {
            int brightnessMaxDifference = (int)(brightnessTolerance * 255);
            int differentPixelMaxNumber = (int)(hash2.Length * differentPixelNumberTolerance);
            int w2 = hash2.GetLength(0);
            int h2 = hash2.GetLength(1);
            return isHashMatch(hash1, 0, 0, hash2, w2, h2, brightnessMaxDifference, differentPixelMaxNumber);
        }

        public static byte[,] GetBitmapHash(Bitmap bitmap, out float hashImageRatio, int maxHashImageLegnth = 16)
        {
            hashImageRatio = Math.Min((float)maxHashImageLegnth / bitmap.Width, (float)maxHashImageLegnth / bitmap.Height);
            return GetBitmapHash(bitmap, hashImageRatio);
        }

        public static byte[,] GetBitmapHash(Bitmap bitmap, float hashImageRatio)
        {
            int w = (int)(bitmap.Width * hashImageRatio);
            int h = (int)(bitmap.Height * hashImageRatio);
            bitmap = GetResized(bitmap, w, h);
            Int32[] rawImageData = new Int32[w * h];
            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bd.Scan0, rawImageData, 0, w * h);
            bitmap.UnlockBits(bd);
            byte[,] hash = new byte[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = Color.FromArgb(rawImageData[y * w + x]);
                    hash[x, y] = (byte)(c.GetBrightness() * 255);
                }
            }
            return hash;
        }

        public static PointF? FindBitmapFragmentByHash(Bitmap bitmap, byte[,] hash, float hashImageRatio, float brightnessTolerance, float differentPixelNumberTolerance, Point? startPoint = null)
        {
            int brightnessMaxDifference = (int)(brightnessTolerance * 255);
            int differentPixelMaxNumber = (int)(hash.Length * differentPixelNumberTolerance);
            byte[,] bHash = GetBitmapHash(bitmap, hashImageRatio);
            int w = hash.GetLength(0);
            int h = hash.GetLength(1);
            int bw = bHash.GetLength(0) - w;
            int bh = bHash.GetLength(1) - h;
            for (int x = 0; x < bw; x++)
                for (int y = 0; y < bh; y++)
                    if (isHashMatch(bHash, x, y, hash, w, h, brightnessMaxDifference, differentPixelMaxNumber))
                        return new PointF(x/ hashImageRatio, y/ hashImageRatio);
            return null;
        }

        static bool isHashMatch(byte[,] bHash, int x, int y, byte[,] hash, int w, int h, int brightnessMaxDifference, int differentPixelMaxNumber)
        {
            int differentPixelNumber = 0;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    if (Math.Abs(bHash[x + i, y + j] - hash[i, j]) > brightnessMaxDifference)
                        if (++differentPixelNumber > differentPixelMaxNumber)
                            return false;
                }
            return true;
        }
    }
}