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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Features2D;

namespace Cliver.PdfDocumentParser
{
    public class CvImage:IDisposable
    {
        public void Dispose()
        {
            lock (this)
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }
                //if (Bitmap != null)
                //{
                //    Bitmap.Dispose();
                //    Bitmap = null;
                //}
            }
        }

        ~CvImage()
        {
            Dispose();
        }

        /// <summary>
        /// used only by serializer
        /// </summary>
        public string Data
        {
            get
            {
                byte[] hash = new byte[8 + Image.Bytes.Length];
                Array.Copy(BitConverter.GetBytes(Width), 0, hash, 0, 4);
                Array.Copy(BitConverter.GetBytes(Height), 0, hash, 4, 4);

                Array.Copy(Image.Bytes, 0, hash, 8, Image.Bytes.Length);
                //int w = Bitmap.Width;
                //int h = Bitmap.Height;
                //Bitmap = Win.ImageRoutines.GetResized(Bitmap, w, h);
                //Int32[] rawImageData = new Int32[w * h];
                //BitmapData bd = Bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                //Marshal.Copy(bd.Scan0, rawImageData, 0, w * h);
                //Bitmap.UnlockBits(bd);
                //int p = 8;
                //for (int x = 0; x < w; x++)
                //{
                //    for (int y = 0; y < h; y++)
                //    {
                //        Color c = Color.FromArgb(rawImageData[p]);
                //        hash[p] = (byte)((c.R + c.G + c.B) / 3);
                //        p++;
                //    }
                //}

                return Convert.ToBase64String(hash);
            }
            set
            {
                try
                {
                    byte[] hash = Convert.FromBase64String(value);
                    Width = BitConverter.ToInt32(hash, 0);
                    Height = BitConverter.ToInt32(hash, 4);

                    //Bitmap = new Bitmap(Width, Height);
                    //int p = 0;
                    //for (int x = 0; x < Width; x++)
                    //    for (int y = 0; y < Height; y++)
                    //    {
                    //        Bitmap.SetPixel(x, y, Color.FromArgb(hash[p], hash[p], hash[p]));
                    //        p++;
                    //    }

                    Image = new Image<Gray, byte>(Width, Height);
                    byte[] bs = new byte[hash.Length - 8];
                    Array.Copy(hash, 8, bs, 0, bs.Length);
                    Image.Bytes = bs;
                }
                catch
                {
                    Width = 10;
                    Height = 10;
                    Image = new Image<Gray, byte>(Width, Height);
                }
            }
        }
        internal int Width;
        internal int Height;
        internal Image<Gray, byte> Image;
        //internal Bitmap Bitmap
        //{
        //    set
        //    {
        //        _bitmap = value;
        //        Image<Gray, byte> Image = _bitmap.ToImage<Gray, byte>();
        //        Emgu.CV.CvInvoke.Blur(Image, Image, new Size(10, 10), new Point(0, 0));
        //        //Emgu.CV.CvInvoke.Threshold(image, image, 60, 255, ThresholdType.Otsu | ThresholdType.Binary);
        //        //Emgu.CV.CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        //        //CvInvoke.Dilate(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        //        //CvInvoke.Canny(image, image, 100, 30, 3);
        //    }
        //    get
        //    {
        //        return _bitmap;
        //    }
        //}
        //Bitmap _bitmap;

        public CvImage(Bitmap bitmap, bool scaleBitmap = true)
        {
            if (bitmap == null)// Used only by deserializer!!!
                return;

            if (scaleBitmap)
                bitmap = GetScaled(bitmap, Settings.Constants.Image2PdfResolutionRatio);

            Image = getPreprocessedImage(bitmap);
            Width = Image.Width;
            Height = Image.Height;
        }

        public static Bitmap GetScaled(Image image, float ratio)
        {
            var b = new Bitmap((int)Math.Round(image.Width * ratio, 0), (int)Math.Round(image.Height * ratio, 0), PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(b))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, 0, 0, b.Width, b.Height);
            }
            return b;
        }

        static private Image<Gray, byte> getPreprocessedImage(Bitmap bitmap)
        {
            Image<Gray, byte> image = bitmap.ToImage<Gray, byte>();
            //Emgu.CV.CvInvoke.Blur(image, image, new Size(10, 10), new Point(0, 0));
            //Emgu.CV.CvInvoke.Threshold(image, image, 60, 255, ThresholdType.Otsu | ThresholdType.Binary);
            //Emgu.CV.CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            //CvInvoke.Dilate(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            //CvInvoke.Canny(image, image, 100, 30, 3);
            return image;
        }

        public System.Drawing.Point? FindWithinImage(CvImage cvImage, float threshold, float scaleDeviation)
        {
            System.Drawing.Point? p;
            p = findWithinImage(Image, cvImage.Image, threshold);
            if (p != null)
                return p;
            //running though pyramid
            const int scaleStep = 2;
            int stepCount = Convert.ToInt32(scaleDeviation * Width / scaleStep);
            for (int i = 1; i <= stepCount; i++)
            {
                double scaleDelta = scaleStep * i / Width;
                Image<Gray, byte> template = Image.Resize(1 + scaleDelta, Inter.Linear);
                p = findWithinImage(template, cvImage.Image, threshold);
                if (p != null)
                    return p;
                template = Image.Resize(1 - scaleDelta, Inter.Linear);
                p = findWithinImage(template, cvImage.Image, threshold);
                if (p != null)
                    return p;
            }
            return null;
        }

       static System.Drawing.Point? findWithinImage(Image<Gray, byte> template, Image<Gray, byte> image, float threshold = 0.7f)
        {
            using (Image<Gray, float> match = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                match.MinMax(out double[] min, out double[] max, out Point[] minPoint, out Point[] maxPoint);

                if (max[0] > threshold)
                    return maxPoint[0];
                return null;
            }
        }

        public Image GetImage()
        {
            return Image.ToBitmap();
        }
    }
}