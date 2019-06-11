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
using System.Windows.Media.Imaging;

namespace Cliver.PdfDocumentParser
{
    public class ImageData
    {
        public byte[,] Hash;
        public int Width;
        public int Height;

        //public string GetAsString()
        //{
        //    //string g;
        //    //byte[] bs = new byte[2 + Hash.Length];
        //    //BitConverter.GetBytes((ushort)Width).CopyTo(bs, 0);
        //    //for (int i = 0; i < Width; i++)
        //    //    for (int j = 0; j < Height; j++)
        //    //        bs[2 + i * Height + j] = Hash[i, j];
        //    //using (MemoryStream ms = new MemoryStream())
        //    //{
        //    //    using (GZipStream compressionStream = new GZipStream(ms, CompressionMode.Compress))
        //    //    {
        //    //        compressionStream.Write(bs, 0, bs.Length);
        //    //    }
        //    //    g = Encoding.UTF8.GetString(ms.ToArray());
        //    //}

        //    //byte[] bs = Serialization.Binary.Serialize(this);//more compact
        //    //return Serialization.Json.Serialize(bs, false);
        //    return Serialization.Json.Serialize(this, false);
        //}
        //static public ImageData GetFromString(string s)
        //{
        //    //byte[] bs = Serialization.Json.Deserialize<byte[]>(s);
        //    //return Serialization.Binary.Deserialize<ImageData>(bs);
        //    return Serialization.Json.Deserialize<ImageData>(s);

        //    //using (MemoryStream ms = new MemoryStream(bs))
        //    //{
        //    //    using (GZipStream compressionStream = new GZipStream(ms, CompressionMode.Decompress))
        //    //    {
        //    //        compressionStream.Read(bs, 0, bs.Length);
        //    //    }
        //    //    g = Encoding.UTF8.GetString(ms.ToArray());
        //    //}
        //}

        public ImageData(Bitmap bitmap, bool scaleBitmap = true)
        {
            if (bitmap == null)// Used only by deserializer!!!
                return;

            if (scaleBitmap)
                bitmap = GetScaled(bitmap, Settings.Constants.Image2PdfResolutionRatio);

            Hash = getBitmapHash(bitmap);
            Width = Hash.GetLength(0);
            Height = Hash.GetLength(1);
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
        static byte[,] getBitmapHash(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;
            bitmap = ImageRoutines.GetResized(bitmap, w, h);
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
                    //hash[x, y] = (byte)(c.GetBrightness() * 255);
                    hash[x, y] = (byte)((c.R + c.G + c.B) / 3);
                    //hash[x, y] = (byte)((c.GetBrightness() < 0.9 ? 0 : 1) * 255);
                }
            }
            return hash;
        }

        public bool EqualTo(ImageData id)
        {
            if (Width != id.Width || Height != id.Height)
                return false;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                    if (Hash[x, y] != id.Hash[x, y])
                        return false;
            }
            return true;
        }

        /*!!!ATTENTION!!!
         * tolerance values cannot be 0 even when comparing identical images! Because of separate rescaling of an image and its fragment, some pixels become not same!
         */
        public bool IsSimilarTo(ImageData imageData, float brightnessTolerance, float differentPixelNumberTolerance)
        {
            if (Width != imageData.Width || Height != imageData.Height)
                throw new Exception("Images have different sizes.");
            //return isAbsolutizedBrightnessMatchOf(imageData, 0, 0, (int)(brightnessTolerance * 255 * absolutizedBrightnessToleranceFactor), (int)(Hash.Length * differentPixelNumberTolerance), out differentPixelNumber);
            return isMatchOf(imageData, 0, 0, (int)(brightnessTolerance * 255), (int)(Hash.Length * differentPixelNumberTolerance), out int differentPixelNumber);
        }

        /*!!!ATTENTION!!!
         * tolerance values cannot be 0 even when comparing identical images! Because of separate rescaling of an image and its fragment, some pixels become not equal!
         */
        public System.Drawing.Point? FindWithinImage(ImageData imageData, float brightnessTolerance, float differentPixelNumberTolerance, bool findBestMatch)
        {
            Point? bestP = null;
            float minDeviation = 1;
            FindWithinImage(imageData, brightnessTolerance, differentPixelNumberTolerance, (Point p, float deviation) =>
             {
                 if (!findBestMatch)
                 {
                     bestP = p;
                     return false;
                 }
                 if (deviation < minDeviation)
                 {
                     minDeviation = deviation;
                     bestP = p;
                 }
                 return true;
             });
            return bestP;
        }

        public void FindWithinImage(ImageData imageData, float brightnessTolerance, float differentPixelNumberTolerance, Func<Point, float, bool> proceedOnFound)
        {
            int brightnessMaxDifference = (int)(brightnessTolerance * 255);
            int differentPixelMaxNumber = (int)(Hash.Length * differentPixelNumberTolerance);
            int bw = imageData.Width - Width;
            int bh = imageData.Height - Height;
            for (int x = 0; x <= bw; x++)
                for (int y = 0; y <= bh; y++)
                {
                    if (isMatchOf(imageData, x, y, brightnessMaxDifference, differentPixelMaxNumber, out int differentPixelNumber))
                    {
                        //int absolutizedBrightnessMaxDifference = (int)(brightnessTolerance * 255 * absolutizedBrightnessToleranceFactor);
                        //if (!isAbsolutizedBrightnessMatchOf(imageData, x, y, absolutizedBrightnessMaxDifference, differentPixelMaxNumber, out differentPixelNumber))
                        //    continue;
                        if (!proceedOnFound(new Point(x, y), differentPixelMaxNumber == 0 ? 0 : (float)differentPixelNumber / differentPixelMaxNumber))
                            return;
                    }
                }
        }
        bool isMatchOf(ImageData imageData, int x, int y, int brightnessMaxDifference, int differentPixelMaxNumber, out int differentPixelNumber)
        {
            differentPixelNumber = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    if (Math.Abs(imageData.Hash[x + i, y + j] - Hash[i, j]) > brightnessMaxDifference)
                        if (++differentPixelNumber > differentPixelMaxNumber)
                            return false;
                }
            return true;
        }

        //#region    with taking image brightness to account

        //ImageData absolutizedBrightnessImageData = null;
        //float absolutizedBrightnessToleranceFactor = 0.3f;
        ////float brightnessOptimumThreashold = 0.02f;

        //bool isAbsolutizedBrightnessMatchOf(ImageData imageData, int x, int y, int brightnessMaxDifference, int differentPixelMaxNumber, out int differentPixelNumber)
        //{
        //    //!!!still works bad
        //    if (absolutizedBrightnessImageData == null)
        //        absolutizedBrightnessImageData = getAbsolutizedImageData(0, 0, Width, Height);
        //    ImageData absolutizedBrightnessImageData2 = imageData.getAbsolutizedImageData(x, y, Width, Height);
        //    return absolutizedBrightnessImageData.isMatchOf(absolutizedBrightnessImageData2, 0, 0, brightnessMaxDifference, differentPixelMaxNumber, out differentPixelNumber);
        //}

        //void getMinMaxBrightnessOptimums(int x, int y, int width, int height, out byte minBrightness, out byte maxBrightness)
        //{
        //    int[] brightnesses2pointCount = new int[256];
        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //            brightnesses2pointCount[Hash[x + i, y + j]] = brightnesses2pointCount[Hash[x + i, y + j]] + 1;
        //    }
        //    //int minBrightnessPointCount = 0;
        //    //minBrightness = 0;
        //    //for (byte i = 0; i < 128; i++)
        //    //    if (minBrightnessPointCount < brightnesses2pointCount[i])
        //    //    {
        //    //        minBrightnessPointCount = brightnesses2pointCount[i];
        //    //        minBrightness = i;
        //    //    }
        //    //int maxBrightnessPointCount = 0;
        //    //maxBrightness = 255;
        //    //for (byte i = 127; 127 <= i; i++)
        //    //    if (maxBrightnessPointCount < brightnesses2pointCount[i])
        //    //    {
        //    //        maxBrightnessPointCount = brightnesses2pointCount[i];
        //    //        maxBrightness = i;
        //    //    }

        //    //float brightnessOptimumThreasholdCount = brightnessOptimumThreashold * width * height;
        //    //minBrightness = 0;
        //    //for (byte i = 0; i < brightnesses2pointCount.Length - 1; i++)
        //    //    if (brightnesses2pointCount[i] > brightnessOptimumThreasholdCount)
        //    //    {
        //    //        minBrightness = i;
        //    //        break;
        //    //    }
        //    //maxBrightness = 255;
        //    //for (byte i = (byte)(brightnesses2pointCount.Length - 1); i > 0; i--)
        //    //    if (brightnesses2pointCount[i] > brightnessOptimumThreasholdCount)
        //    //    {
        //    //        maxBrightness = i;
        //    //        break;
        //    //    }

        //    minBrightness = 0;
        //    for (byte i = 0; i < brightnesses2pointCount.Length - 1; i++)
        //        if (brightnesses2pointCount[i] > brightnesses2pointCount[i + 1])
        //        {
        //            minBrightness = i;
        //            break;
        //        }
        //    maxBrightness = 255;
        //    for (byte i = (byte)(brightnesses2pointCount.Length - 1); i > 0; i--)
        //        if (brightnesses2pointCount[i] > brightnesses2pointCount[i - 1])
        //        {
        //            maxBrightness = i;
        //            break;
        //        }
        //}

        //ImageData getAbsolutizedImageData(int x, int y, int width, int height)
        //{
        //    ImageData aid = new ImageData(null);
        //    aid.Width = width;
        //    aid.Height = height;

        //    getMinMaxBrightnessOptimums(x, y, width, height, out byte minOptimum, out byte maxOptimum);
        //    float brightnessFactor = (float)255 / (maxOptimum - minOptimum);

        //    aid.Hash = new byte[width, height];
        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {
        //            if (Hash[x + i, y + j] <= minOptimum)
        //                aid.Hash[i, j] = 0;
        //            else if (Hash[x + i, y + j] < maxOptimum)
        //                aid.Hash[i, j] = (byte)(brightnessFactor * (Hash[x + i, y + j] - minOptimum));
        //            else
        //                aid.Hash[i, j] = 255;
        //        }
        //    }
        //    return aid;
        //}
        //#endregion

        //#region    with summing image brightness deltas
        //public bool ImageIsSimilar(ImageData imageData, float brightnessTolerance, float brightnessTotalDifferenceTolerance)
        //{
        //    if (Width != imageData.Width || Height != imageData.Height)
        //        throw new Exception("Images have different sizes.");
        //    uint brightnessDifferenceNumber;
        //    return isHashMatch(imageData, 0, 0, (uint)(brightnessTolerance * 255), (uint)(Width * Height * 255 * brightnessTotalDifferenceTolerance), out brightnessDifferenceNumber);
        //}

        //public System.Drawing.Point? FindWithinImage(ImageData imageData, float brightnessTolerance, float brightnessTotalDifferenceTolerance, bool findBestMatch)
        //{
        //    Point? bestP = null;
        //    float minDeviation = 1;
        //    FindWithinImage(imageData, brightnessTolerance, brightnessTotalDifferenceTolerance, (Point p, float deviation) =>
        //    {
        //        if (!findBestMatch)
        //        {
        //            bestP = p;
        //            return false;
        //        }
        //        if (deviation < minDeviation)
        //        {
        //            minDeviation = deviation;
        //            bestP = p;
        //        }
        //        return true;
        //    });
        //    return bestP;
        //}

        //public void FindWithinImage(ImageData imageData, float brightnessTolerance, float brightnessDifferenceTolerance, Func<Point, float, bool> onFound)
        //{
        //    uint brightnessMaxDifference = (uint)(brightnessTolerance * 255);
        //    uint brightnessTotalMaxDifference = (uint)(Width * Height * 255 * brightnessDifferenceTolerance);
        //    int bw = imageData.Width - Width;
        //    int bh = imageData.Height - Height;
        //    for (int x = 0; x <= bw; x++)
        //        for (int y = 0; y <= bh; y++)
        //        {
        //            uint brightnessTotalDifference;
        //            if (isHashMatch(imageData, x, y, brightnessMaxDifference, brightnessTotalMaxDifference, out brightnessTotalDifference))
        //            {
        //                if (!onFound(new Point(x, y), brightnessTotalMaxDifference == 0 ? 0 : (float)brightnessTotalDifference / brightnessTotalMaxDifference))
        //                    return;
        //            }
        //        }
        //}

        //bool isHashMatch(ImageData imageData, int x, int y, uint brightnessMaxDifference, uint brightnessTotalMaxDifference, out uint brightnessTotalDifference)
        //{
        //    brightnessTotalDifference = 0;
        //    for (int i = 0; i < Width; i++)
        //        for (int j = 0; j < Height; j++)
        //        {
        //            uint d = (uint)Math.Abs(imageData.Hash[x + i, y + j] - Hash[i, j]);
        //            if (d > brightnessMaxDifference)
        //            {
        //                brightnessTotalDifference += d;
        //                if (brightnessTotalDifference > brightnessTotalMaxDifference)
        //                    return false;
        //            }
        //        }
        //    return true;
        //}
        //#endregion

        public Image GetImage()
        {
            Bitmap b = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(Hash[x, y], Hash[x, y], Hash[x, y]));
            return b;
        }

        //public BitmapImage GetBitmapImage()
        //{
            //using (MemoryStream memory = new MemoryStream())
            //{
            //    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            //    memory.Position = 0;
            //    BitmapImage bitmapimage = new BitmapImage();
            //    bitmapimage.BeginInit();
            //    bitmapimage.pi.CopyPixels(.StreamSource = memory;
            //    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapimage.EndInit();

            //    return bitmapimage;
            //}
        //}

        public System.Windows.Media.ImageSource GetImageSource()
        {
            return ImageRoutines.ToImageSource((Bitmap)GetImage());
        }
    }
}