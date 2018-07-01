using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace Cliver.InvoiceParser
{
    [Serializable]
    public class ImageData
    {
        public byte[,] Hash;
        public int Width;
        public int Height;

        public string GetAsString()
        {
            //string g;
            //byte[] bs = new byte[2 + Hash.Length];
            //BitConverter.GetBytes((ushort)Width).CopyTo(bs, 0);
            //for (int i = 0; i < Width; i++)
            //    for (int j = 0; j < Height; j++)
            //        bs[2 + i * Height + j] = Hash[i, j];
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (GZipStream compressionStream = new GZipStream(ms, CompressionMode.Compress))
            //    {
            //        compressionStream.Write(bs, 0, bs.Length);
            //    }
            //    g = Encoding.UTF8.GetString(ms.ToArray());
            //}

            byte[] bs = SerializationRoutines.Binary.Serialize(this);//more compact
            return SerializationRoutines.Json.Serialize(bs);
        }
        static public ImageData GetFromString(string value)
        {
            byte[] bs = SerializationRoutines.Json.Deserialize<byte[]>(value);
            return SerializationRoutines.Binary.Deserialize<ImageData>(bs);
            //using (MemoryStream ms = new MemoryStream(bs))
            //{
            //    using (GZipStream compressionStream = new GZipStream(ms, CompressionMode.Decompress))
            //    {
            //        compressionStream.Read(bs, 0, bs.Length);
            //    }
            //    g = Encoding.UTF8.GetString(ms.ToArray());
            //}
        }
        public ImageData(Bitmap bitmap)
        {
            if (bitmap == null)// Used only by deserializer!!!
                return;

            bitmap = ImageRoutines.GetScaled(bitmap, Settings.General.Image2PdfResolutionRatio);

            Hash = getBitmapHash(bitmap);
            Width = Hash.GetLength(0);
            Height = Hash.GetLength(1);
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
        /*!!!ATTENTION!!!
         * tolerance values cannot be 0 even when comparing identical images! Because of separate rescaling of an image and its fragment, some pixels become not same!
         */
        public bool ImageIsSimilar(ImageData imageData, float brightnessTolerance, float differentPixelNumberTolerance)
        {
            int differentPixelNumber;
            return isHashMatch(imageData, 0, 0, (int)(brightnessTolerance * 255), (int)(Hash.Length * differentPixelNumberTolerance), out differentPixelNumber);
        }

        /*!!!ATTENTION!!!
         * tolerance values cannot be 0 even when comparing identical images! Because of separate rescaling of an image and its fragment, some pixels become not same!
         */
        public System.Drawing.PointF? FindWithinImage(ImageData imageData, float brightnessTolerance, float differentPixelNumberTolerance, bool findBestMatch, System.Drawing.PointF? startPoint = null)
        {
            int brightnessMaxDifference = (int)(brightnessTolerance * 255);
            int differentPixelMaxNumber = (int)(Hash.Length * differentPixelNumberTolerance);
            int bw = imageData.Width - Width;
            int bh = imageData.Height - Height;
            PointF? p = null;
            int minDifferentPixelNumber = int.MaxValue;
            for (int x = 0; x < bw; x++)
                for (int y = 0; y < bh; y++)
                {
                    int differentPixelNumber;
                    if (isHashMatch(imageData, x, y, brightnessMaxDifference, differentPixelMaxNumber, out differentPixelNumber))
                    {
                        if (!findBestMatch)
                            return new PointF(x, y);
                        if (minDifferentPixelNumber > differentPixelNumber)
                            p = new PointF(x, y);
                    }
                }
            return p;
        }
        bool isHashMatch(ImageData imageData, int x, int y, int brightnessMaxDifference, int differentPixelMaxNumber, out int differentPixelNumber)
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
    }
}