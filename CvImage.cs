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
using System.Collections.Generic;
using System.Linq;

namespace Cliver.PdfDocumentParser
{
    public class CvImage : IDisposable
    {
        public void Dispose()
        {
            lock (this)
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
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
                byte[] hash = new byte[8 + image.Bytes.Length];
                Array.Copy(BitConverter.GetBytes(Width), 0, hash, 0, 4);
                Array.Copy(BitConverter.GetBytes(Height), 0, hash, 4, 4);

                Array.Copy(image.Bytes, 0, hash, 8, image.Bytes.Length);

                return Convert.ToBase64String(hash);
            }
            set
            {
                try
                {
                    byte[] hash = Convert.FromBase64String(value);
                    Width = BitConverter.ToInt32(hash, 0);
                    Height = BitConverter.ToInt32(hash, 4);

                    image = new Image<Gray, byte>(Width, Height);
                    byte[] bs = new byte[hash.Length - 8];
                    Array.Copy(hash, 8, bs, 0, bs.Length);
                    image.Bytes = bs;
                }
                catch
                {
                    Width = 10;
                    Height = 10;
                    image = new Image<Gray, byte>(Width, Height);
                }
            }
        }
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        Image<Gray, byte> image;
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

        public CvImage()// Used only by deserializer!!!
        { }

        public CvImage(Bitmap bitmap)
        {
            bitmap = Page.GetScaledImage2Pdf(bitmap);
            image = getPreprocessedImage(bitmap);
            Width = image.Width;
            Height = image.Height;
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

        public class Match
        {
            //public Point Point;
            public Rectangle Rectangle;
            public float Scale;
            public float Score;
        }

        public Match FindBestMatchWithinImage(CvImage cvImage, float threshold, float scaleDeviation, int scaleStep)
        {
            Match bestMatch = null;
            Tuple<Point, float> p_s = findBestMatchWithinImage(image, cvImage.image);
            if (p_s != null && p_s.Item2 > threshold)
                bestMatch = new Match { Rectangle = new Rectangle(p_s.Item1, new Size(image.Width, image.Height)), Scale = 1, Score = p_s.Item2 };
            //running through pyramid
            int stepCount = Convert.ToInt32(scaleDeviation * Width / scaleStep);
            for (int i = 1; i <= stepCount; i++)
            {
                float scaleDelta = (float)scaleStep * i / Width;
                float scale = 1 + scaleDelta;
                Image<Gray, byte> template = image.Resize(scale, Inter.Linear);
                p_s = findBestMatchWithinImage(template, cvImage.image);
                if (p_s != null && (bestMatch == null && p_s.Item2 > threshold || bestMatch != null && p_s.Item2 > bestMatch.Score))
                    bestMatch = new Match { Rectangle = new Rectangle(p_s.Item1, new Size(template.Width, template.Height)), Scale = scale, Score = p_s.Item2 };
                scale = 1 - scaleDelta;
                template = image.Resize(scale, Inter.Linear);
                p_s = findBestMatchWithinImage(template, cvImage.image);
                if (p_s != null && (bestMatch == null && p_s.Item2 > threshold || bestMatch != null && p_s.Item2 > bestMatch.Score))
                    bestMatch = new Match { Rectangle = new Rectangle(p_s.Item1, new Size(template.Width, template.Height)), Scale = scale, Score = p_s.Item2 };
            }
            return bestMatch;
        }

        public Match FindFirstMatchWithinImage(CvImage cvImage, float threshold, float scaleDeviation, int scaleStep)
        {
            Tuple<Point, float> p_s = findBestMatchWithinImage(image, cvImage.image);
            if (p_s != null && p_s.Item2 > threshold)
                return new Match { Rectangle = new Rectangle(p_s.Item1, new Size(image.Width, image.Height)), Scale = 1, Score = p_s.Item2 };
            //running through pyramid
            int stepCount = Convert.ToInt32(scaleDeviation * Width / scaleStep);
            for (int i = 1; i <= stepCount; i++)
            {
                float scaleDelta = (float)scaleStep * i / Width;
                float scale = 1 + scaleDelta;
                Image<Gray, byte> template = image.Resize(scale, Inter.Linear);
                p_s = findBestMatchWithinImage(template, cvImage.image);
                if (p_s != null && p_s.Item2 > threshold)
                    return new Match { Rectangle = new Rectangle(p_s.Item1, new Size(template.Width, template.Height)), Scale = scale, Score = p_s.Item2 };
                scale = 1 - scaleDelta;
                template = image.Resize(scale, Inter.Linear);
                p_s = findBestMatchWithinImage(template, cvImage.image);
                if (p_s != null && p_s.Item2 > threshold)
                    return new Match { Rectangle = new Rectangle(p_s.Item1, new Size(template.Width, template.Height)), Scale = scale, Score = p_s.Item2 };
            }
            return null;
        }

        static Tuple<Point, float> findBestMatchWithinImage(Image<Gray, byte> template, Image<Gray, byte> image)
        {
            if (template.Width > image.Width || template.Height > image.Height)//otherwise MatchTemplate() throws an exception
                return null;
            using (Image<Gray, float> match = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                match.MinMax(out double[] min, out double[] max, out Point[] minPoint, out Point[] maxPoint);
                return new Tuple<Point, float>(maxPoint[0], (float)max[0]);
            }
        }

        public bool FindMatchesWithinImage(CvImage cvImage, float threshold, float scaleDeviation, int scaleStep, Func<Match, bool> proceedOnMatch)
        {
            foreach (Match m in findMatchsWithinImage(image, 1, cvImage.image, threshold))
                if (!proceedOnMatch(m))
                    return true;
            //running through pyramid
            int stepCount = Convert.ToInt32(scaleDeviation * Width / scaleStep);
            for (int i = 1; i <= stepCount; i++)
            {
                float scaleDelta = (float)scaleStep * i / Width;
                float scale = 1 + scaleDelta;
                Image<Gray, byte> template = image.Resize(scale, Inter.Linear);
                foreach (Match m in findMatchsWithinImage(template, scale, cvImage.image, threshold))
                    if (!proceedOnMatch(m))
                        return true;
                scale = 1 - scaleDelta;
                template = image.Resize(scale, Inter.Linear);
                foreach (Match m in findMatchsWithinImage(template, scale, cvImage.image, threshold))
                    if (!proceedOnMatch(m))
                        return true;
            }
            return false;
        }

        static List<Match> findMatchsWithinImage1(Image<Gray, byte> template, float scale, Image<Gray, byte> image, float threshold)//!!!needs fixing
        {
            List<Match> ms = new List<Match>();
            if (template.Width > image.Width || template.Height > image.Height)//otherwise MatchTemplate() throws an exception
                return ms;
            using (Image<Gray, float> matchData = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                CvInvoke.Threshold(matchData, matchData, threshold, 1, ThresholdType.ToZero);
                //    while (true)
                //    {
                //        double minval = 0, maxval = 0;
                //        Point minloc = new Point(), maxloc = new Point();
                //        CvInvoke.MinMaxLoc(matchData, ref minval, ref maxval, ref minloc, ref maxloc);
                //        if (maxval <= threshold)
                //            break;
                //        ms.Add(new Match { Rectangle = new Rectangle(maxloc, new Size(template.Width, template.Height)), Scale = scale, Score = (float)maxval });
                //        //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                //        Mat outputMask = new Mat(matchData.Rows + 2, matchData.Cols + 2, DepthType.Cv8U, 1);
                //        CvInvoke.FloodFill(matchData, outputMask, maxloc, new MCvScalar(0), out Rectangle r, new MCvScalar(0.1), new MCvScalar(1.0));
                //    }
                for (; ; )
                {
                    matchData.MinMax(out double[] mins, out double[] maxs, out Point[] minPoints, out Point[] maxPoints);
                    if (maxs[0] <= threshold)
                        break;
                    ms.Add(new Match { Rectangle = new Rectangle(maxPoints[0], new Size(template.Width, template.Height)), Scale = scale, Score = (float)maxs[0] });
                    //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                    Mat outputMask = new Mat(matchData.Rows + 2, matchData.Cols + 2, DepthType.Cv8U, 1);
                    CvInvoke.FloodFill(matchData, outputMask, maxPoints[0], new MCvScalar(0), out _, new MCvScalar(1), new MCvScalar(1));
                }
                return ms;
            }
        }

        static List<Match> findMatchsWithinImage(Image<Gray, byte> template, float scale, Image<Gray, byte> image, float threshold)
        {
            List<Match> ms = new List<Match>();
            if (template.Width > image.Width || template.Height > image.Height)//otherwise MatchTemplate() throws an exception
                return ms;

            using (Image<Gray, float> match = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                float[,,] matches = match.Data;
                int xLenght = matches.GetLength(1);
                int yLenght = matches.GetLength(0);
                float[,] extremums = new float[yLenght + 2, xLenght + 2];
                for (int x = 0; x < xLenght; x++)
                    for (int y = 0; y < yLenght; y++)
                        extremums[y + 1, x + 1] = matches[y, x, 0];

                for (int x = 1; x <= xLenght; x++)
                {
                    for (int y = 1; y <= yLenght; y++)
                    {
                        float score = extremums[y, x];
                        if (score <= threshold)
                            continue;
                        if (extremums[y - 1, x - 1] <= score && extremums[y - 1, x] <= score && extremums[y - 1, x + 1] <= score
                            && extremums[y, x - 1] <= score && extremums[y, x + 1] < score
                            && extremums[y + 1, x - 1] <= score && extremums[y + 1, x] <= score && extremums[y + 1, x + 1] <= score
                            )
                            ms.Add(new Match { Rectangle = new Rectangle(new Point(x - 1, y - 1), template.Size), Scale = scale, Score = score });
                    }
                }
            }
            return ms;
        }


        static List<Match> findMatchsWithinImage(Image<Gray, byte> template, Size padding, Image<Gray, byte> image, float threshold)
        {
            if (template.Width > image.Width || template.Height > image.Height)//otherwise MatchTemplate() throws an exception
                return null;

            using (Image<Gray, float> match = image.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                float[,,] matches = match.Data;
                Dictionary<Rectangle, Match> paddedMatchRs2bestMatchP = new Dictionary<Rectangle, Match>();

                for (int x = matches.GetLength(1) - 1; x >= 0; x--)
                {
                    for (int y = matches.GetLength(0) - 1; y >= 0; y--)
                    {
                        float score = matches[y, x, 0];
                        //if (score < 0.003)//SqdiffNormed
                        if (score > threshold)//CcoeffNormed
                        {
                            Rectangle r = new Rectangle(new Point(x, y), template.Size);
                            var kv = paddedMatchRs2bestMatchP.FirstOrDefault(a => a.Key.Contains(r));
                            if (kv.Key == Rectangle.Empty)
                            {
                                Rectangle ar = new Rectangle(r.Location, r.Size);
                                ar.Inflate(padding);
                                paddedMatchRs2bestMatchP[ar] = new Match { Rectangle = r, Score = score };
                            }
                            else
                            {
                                if (kv.Value.Score < score)
                                    paddedMatchRs2bestMatchP[kv.Key] = new Match { Rectangle = r, Score = score };
                            }
                        }
                    }
                }
                return paddedMatchRs2bestMatchP.Values.ToList();
            }
        }

        public Image GetImage()
        {
            return image.ToBitmap();
        }
    }
}