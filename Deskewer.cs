//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.Features2D;

namespace Cliver.PdfDocumentParser
{
    public class Deskewer
    {
        public class Config
        {
            public Modes Mode = Modes.SingleBlock;
            public int BlockMaxLength = 400;
            public int BlockMinGap = 6;
            public Size StructuringElementSize = new Size(30, 1);
            public int ContourMaxCount = 10;
            public float AngleMaxDeviation = 1;
            public Color MarginColor = Color.FromArgb(255, 210, 210);
        }
        public enum Modes
        {
            SingleBlock = 1,//default
            ColumnOfBlocks = 2,
            RowOfBlocks = 4,
            //ByBlockWithMaxLength = 7,//default
            //ByMostUnidirectedBlocks = 8,
        }

        static public void Deskew(ref Bitmap bitmap, Config config)
        {
            int blockMaxLength = (int)(config.BlockMaxLength / Settings.Constants.Pdf2ImageResolutionRatio);
            int blockMinGap = (int)(config.BlockMinGap / Settings.Constants.Pdf2ImageResolutionRatio);
            Size structuringElementSize = new Size((int)(config.StructuringElementSize.Width / Settings.Constants.Pdf2ImageResolutionRatio), (int)(config.StructuringElementSize.Height / Settings.Constants.Pdf2ImageResolutionRatio));
            switch (config.Mode)
            {
                case Modes.SingleBlock:
                    DeskewAsSingleBlock(ref bitmap, structuringElementSize, config.ContourMaxCount, config.AngleMaxDeviation, config.MarginColor);
                    break;
                case Modes.ColumnOfBlocks:
                    DeskewAsColumnOfBlocks(ref bitmap, blockMaxLength, blockMinGap, structuringElementSize, config.ContourMaxCount, config.AngleMaxDeviation, config.MarginColor);
                    break;
                case Modes.RowOfBlocks:
                    throw new Exception("not implemented");
                    break;
                default:
                    throw new Exception("Unknown option: " + config.Mode);
            }
        }

        static public void DeskewAsSingleBlock(ref Bitmap bitmap, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, Color marginColor)//good
        {
            (float H, float V) dpi = (H: bitmap.HorizontalResolution, V: bitmap.VerticalResolution);
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();
                Rgb marginRgb = new Rgb(marginColor);
                Image<Rgb, byte> deskewedImage = deskew(image, structuringElementSize, contourMaxCount, angleMaxDeviation, getScaledBlockMargin(), marginRgb);
                deskewedImage.ROI = getCropped(deskewedImage, marginRgb);//crop
                bitmap = deskewedImage?.ToBitmap();
            }
            bitmap.SetResolution(dpi.H, dpi.V);
        }

        /// <summary>
        /// Margin added to a block while deskewing it. It is cropped in the end.
        /// </summary>
        static public Size BlockMargin = new Size(5, 5);
        static Size getScaledBlockMargin()
        {
            return new Size((int)(BlockMargin.Width / Settings.Constants.Pdf2ImageResolutionRatio), (int)(BlockMargin.Height / Settings.Constants.Pdf2ImageResolutionRatio));
        }

        static Image<Rgb, byte> deskew(Image<Rgb, byte> image, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, Size margin, Rgb marginRgb)//good
        {//https://becominghuman.ai/how-to-automatically-deskew-straighten-a-text-image-using-opencv-a0c30aed83df
            Image<Gray, byte> image2 = image.Convert<Gray, byte>();
            CvInvoke.BitwiseNot(image2, image2);//to negative
            //CvInvoke.GaussianBlur(image2, image2, new Size((int)(9f / Settings.Constants.Pdf2ImageResolutionRatio), (int)(9f / Settings.Constants.Pdf2ImageResolutionRatio)), 0);//remove small spots
            CvInvoke.Threshold(image2, image2, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
            Mat se = CvInvoke.GetStructuringElement(MorphShapes.Rectangle, structuringElementSize, new Point(-1, -1));
            CvInvoke.Dilate(image2, image2, se, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            //Emgu.CV.CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            Image<Rgb, byte> image3 = new Image<Rgb, byte>(image.Width + 2 * margin.Width, image.Height + 2 * margin.Height, marginRgb);
            image3.ROI = new Rectangle(new Point(margin.Width, margin.Height), image.Size);
            image.CopyTo(image3);
            image3.ROI = Rectangle.Empty;

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(image2, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            if (contours.Size < 1)
                return null;

            double angle = 0;
            //when contourMaxCount == 1, it just looks by the most lengthy block
            List<(double angle, int w)> cs = new List<(double angle, int w)>();
            for (int i = 0; i < contours.Size; i++)
            {
                RotatedRect rr = CvInvoke.MinAreaRect(contours[i]);
                Rectangle r = rr.MinAreaRect();
                int w = r.Width > r.Height ? r.Width : r.Height;
                double a = rr.Angle;
                if (a > 45)
                    a -= 90;
                else if (a < -45)
                    a += 90;
                cs.Add((angle: a, w: w));
            }
            cs = cs.OrderByDescending(a => a.w).Take(contourMaxCount).OrderBy(a => a.angle).ToList();
            if (cs.Count < 1)
                angle = 0;
            else if (cs.Count < 2)//use the most lengthy block
                angle = cs[0].angle;
            else
            {
                List<List<int>> dss = new List<List<int>>();
                List<int> ds = new List<int>();
                for (int i = 1; i < cs.Count; i++)
                    if (Math.Abs(cs[i].angle - cs[i - 1].angle) < angleMaxDeviation)
                        ds.Add(i);
                    else
                    {
                        dss.Add(ds);
                        ds = new List<int>();
                    }
                dss.Add(ds);
                ds = dss.OrderByDescending(a => a.Count).FirstOrDefault();
                if (ds.Count < 1)
                    angle = 0;
                else
                    // angle = as_[ds.OrderBy(a => Math.Abs(as_[a].angle - as_[a - 1].angle)).FirstOrDefault()].angle;
                    angle = (cs[ds[0] - 1].angle + ds.Sum(a => cs[a].angle)) / (1 + ds.Count);
            }
            if (angle == 0)
                return image3;

            RotationMatrix2D rotationMat = new RotationMatrix2D();
            CvInvoke.GetRotationMatrix2D(new PointF((float)image3.Width / 2, (float)image3.Height / 2), angle, 1, rotationMat);
            //image3.ROI = new Rectangle(new Point(offset.Width, offset.Height), image.Size);
            CvInvoke.WarpAffine(image3, image3, rotationMat, image3.Size, borderValue: marginRgb.MCvScalar);
            return image3;
        }

        static public void DeskewAsColumnOfBlocks(ref Bitmap bitmap, int blockMaxLength, int blockMinGap, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, Color marginColor)
        {
            (float H, float V) dpi = (H: bitmap.HorizontalResolution, V: bitmap.VerticalResolution);
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();
                Size margin = getScaledBlockMargin();
                Rgb marginRgb = new Rgb(marginColor);
                Image<Rgb, byte> deskewedImage = new Image<Rgb, byte>(image.Width + 2 * margin.Width, image.Height /*+ 2 * margin.Height*/, marginRgb);

                //int lastBlockBottomLeft = -1;
                //int lastBlockBottomRight = -1;
                Image<Gray, byte> image2 = image.Convert<Gray, byte>();
                CvInvoke.BitwiseNot(image2, image2);
                //CvInvoke.Blur(image2, image2, new Size(3, 3), new Point(0, 0));
                CvInvoke.GaussianBlur(image2, image2, new Size(25, 25), 5);//remove small spots
                CvInvoke.Threshold(image2, image2, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
                Mat se = CvInvoke.GetStructuringElement(MorphShapes.Rectangle, new Size(5, 5), new Point(-1, -1));
                CvInvoke.Dilate(image2, image2, se, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
                //CvInvoke.Erode(image2, image2, se, new Point(-1, -1), 5, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

                //CvInvoke.BitwiseNot(image2, image2);
                //return image2.ToBitmap();

                VectorOfVectorOfPoint cs = new VectorOfVectorOfPoint();
                Mat h = new Mat();
                CvInvoke.FindContours(image2, cs, h, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
                if (cs.Size < 1)
                    return;

                Array hierarchy = h.GetData();
                List<Contour> contours = new List<Contour>();
                for (int i = 0; i < cs.Size; i++)
                {
                    int p = (int)hierarchy.GetValue(0, i, Contour.HierarchyKey.Parent);
                    if (p < 1)
                        contours.Add(new Contour(hierarchy, i, cs[i]));
                }
                if (contours.Where(a => a.ParentId < 0).Count() < 2)//the only parent is the whole page frame
                    contours.RemoveAll(a => a.ParentId < 0);
                else
                    contours.RemoveAll(a => a.ParentId >= 0);

                contours = contours.OrderBy(a => a.BoundingRectangle.Bottom).ToList();
                for (int blockY = 0; blockY < image.Height;)
                {
                    int blockBottom = image.Height - 1;
                    Tuple<Contour, Contour> lastSpan = null;
                    for (; contours.Count > 0;)
                    {
                        Contour c = contours[0];
                        contours.RemoveAt(0);
                        if (contours.Count > 0)
                        {
                            Contour minTop = contours.Aggregate((a, b) => a.BoundingRectangle.Top < b.BoundingRectangle.Top ? a : b);
                            if (c.BoundingRectangle.Bottom + blockMinGap <= minTop.BoundingRectangle.Top)
                                lastSpan = new Tuple<Contour, Contour>(c, minTop);
                        }

                        if (c.BoundingRectangle.Bottom > blockY + blockMaxLength && lastSpan != null)
                        {
                            blockBottom = lastSpan.Item1.BoundingRectangle.Bottom + blockMinGap / 2;
                            break;
                        }
                    }

                    Rectangle blockRectangle = new Rectangle(0, blockY, image2.Width, blockBottom + 1 - blockY);
                    Image<Rgb, byte> blockImage = image.Copy(blockRectangle);
                    Image<Rgb, byte> deskewedBlockImage = deskew(blockImage, structuringElementSize, contourMaxCount, angleMaxDeviation, margin, marginRgb);
                    //int blockTopLeft = findOffsetLeft(deskewedBlockImage, 10).X;
                    //int blockTopRight = findOffsetRight(deskewedBlockImage, 10).X;
                    //if (lastBlockBottomLeft < 0)
                    //    lastBlockBottomLeft = blockTopLeft;
                    //if (lastBlockBottomRight < 0)
                    //    lastBlockBottomRight = blockTopRight;
                    //deskewedBlockImage.ROI = new Rectangle(blockTopLeft, 0, blockTopRight - blockTopLeft, deskewedBlockImage.Height);
                    //deskewedImage.ROI = new Rectangle(lastBlockBottomLeft - ((blockTopRight - blockTopLeft) - (lastBlockBottomRight - lastBlockBottomLeft)) / 2, blockRectangle.Y, blockTopRight - blockTopLeft, deskewedBlockImage.Height);
                    deskewedBlockImage.ROI = new Rectangle(0, margin.Height, deskewedBlockImage.Width, deskewedBlockImage.Height - 2 * margin.Height);
                    deskewedImage.ROI = new Rectangle(0, blockRectangle.Y, deskewedBlockImage.ROI.Width, deskewedBlockImage.ROI.Height);
                    deskewedBlockImage.CopyTo(deskewedImage);
                    deskewedImage.ROI = Rectangle.Empty;
                    //lastBlockBottomLeft = findOffsetLeft(deskewedBlockImage, -10).X;
                    //lastBlockBottomRight = findOffsetRight(deskewedBlockImage, -10).X;
                    // break;
                    blockY = blockBottom + 1;
                }
                deskewedImage.ROI = getCropped(deskewedImage, marginRgb);//crop
                bitmap = deskewedImage?.ToBitmap();
            }
            bitmap.SetResolution(dpi.H, dpi.V);
        }

        static Rectangle getCropped(Image<Rgb, byte> image, Rgb cropRgb)
        {
            int offsetLeft = findOffset(image, Side.Left, image.Height, cropRgb).X;
            int offsetRight = findOffset(image, Side.Right, image.Height, cropRgb).X;
            int offsetTop = findOffset(image, Side.Top, image.Width, cropRgb).Y;
            int offsetBottom = findOffset(image, Side.Bottom, image.Width, cropRgb).Y;
            return new Rectangle(offsetLeft, offsetTop, offsetRight - offsetLeft + 1, offsetBottom - offsetTop + 1);
        }

        static Point findOffset(Image<Rgb, byte> image, Side side, int maxLenght, Rgb cropRgb)
        {
            switch (side)
            {
                case Side.Left:
                case Side.Right:
                    int y1, y2;
                    if (maxLenght >= 0)
                    {
                        y1 = 0;
                        y2 = image.Height > maxLenght ? maxLenght : image.Height;
                    }
                    else
                    {
                        y1 = image.Height > -maxLenght ? image.Height + maxLenght : 0 - 1;
                        y2 = image.Height;
                    }
                    if (side == Side.Left)
                    {
                        for (int x = 0; x < image.Width; x++)
                            for (int y = y1; y < y2; y++)
                                if (image.Data[y, x, 0] != cropRgb.Red || image.Data[y, x, 1] != cropRgb.Green || image.Data[y, x, 2] != cropRgb.Blue)
                                    return new Point(x, y);
                    }
                    else
                        for (int x = image.Width - 1; x >= 0; x--)
                            for (int y = y1; y < y2; y++)
                                if (image.Data[y, x, 0] != cropRgb.Red || image.Data[y, x, 1] != cropRgb.Green || image.Data[y, x, 2] != cropRgb.Blue)
                                    return new Point(x, y);
                    break;
                case Side.Top:
                case Side.Bottom:
                    int x1, x2;
                    if (maxLenght >= 0)
                    {
                        x1 = 0;
                        x2 = image.Width > maxLenght ? maxLenght : image.Width;
                    }
                    else
                    {
                        x1 = image.Width > -maxLenght ? image.Width + maxLenght : 0 - 1;
                        x2 = image.Width;
                    }
                    if (side == Side.Top)
                    {
                        for (int y = 0; y < image.Height; y++)
                            for (int x = x1; x < x2; x++)
                                if (image.Data[y, x, 0] != cropRgb.Red || image.Data[y, x, 1] != cropRgb.Green || image.Data[y, x, 2] != cropRgb.Blue)
                                    return new Point(x, y);
                    }
                    else
                        for (int y = image.Height - 1; y >= 0; y--)
                            for (int x = x1; x < x2; x++)
                                if (image.Data[y, x, 0] != cropRgb.Red || image.Data[y, x, 1] != cropRgb.Green || image.Data[y, x, 2] != cropRgb.Blue)
                                    return new Point(x, y);
                    break;
                default:
                    throw new Exception("Uknown option: " + side);

            }
            return Point.Empty;
        }
        enum Side
        {
            Left,
            Right,
            Top,
            Bottom
        }

        //public Bitmap DeskewByBlocks(Size blockMaxSize, int minBlockSpan)//!!!not completed
        //{
        //    Image<Gray, byte> image2 = image.Clone();
        //    CvInvoke.BitwiseNot(image2, image2);
        //    //CvInvoke.Blur(image2, image2, new Size(3, 3), new Point(0, 0));
        //    CvInvoke.GaussianBlur(image2, image2, new Size(25, 25), 5);//remove small spots
        //    CvInvoke.Threshold(image2, image2, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
        //    Mat se = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
        //    CvInvoke.Dilate(image2, image2, se, new Point(-1, -1), 5, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
        //    //CvInvoke.Erode(image2, image2, se, new Point(-1, -1), 5, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

        //    CvInvoke.BitwiseNot(image2, image2);
        //    //return image2.ToBitmap();

        //    VectorOfVectorOfPoint cs = new VectorOfVectorOfPoint();
        //    Mat h = new Mat();
        //    CvInvoke.FindContours(image2, cs, h, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
        //    if (cs.Size < 1)
        //        return null;

        //    Array hierarchy = h.GetData();
        //    List<Contour> contours = new List<Contour>();
        //    for (int i = 0; i < cs.Size; i++)
        //    {
        //        int p = (int)hierarchy.GetValue(0, i, Contour.HierarchyKey.Parent);
        //        if (p < 1)
        //            contours.Add(new Contour(hierarchy, i, cs[i]));
        //    }
        //    if (contours.Where(a => a.ParentId < 0).Count() < 2)//the only parent is the whole page frame
        //        contours.RemoveAll(a => a.ParentId < 0);
        //    else
        //        contours.RemoveAll(a => a.ParentId >= 0);

        //    int x = 0;
        //    int y = 0;
        //    for (Rectangle br = new Rectangle(-2, -2, -1, -1); ;)
        //    {
        //        x = br.Right + 1;
        //        if (x >= image.Width)
        //        {
        //            x = 0;
        //            y = br.Bottom + 1;
        //            if (y >= image.Height)
        //                break;
        //        }
        //        int blockWidth = blockMaxSize.Width;
        //        if (x + blockWidth > image.Width)
        //            blockWidth = image.Width - x;
        //        int blockHight = blockMaxSize.Height;
        //        if (y + blockHight > image.Height)
        //            blockHight = image.Height - y;
        //        br = new Rectangle(new Point(x, y), new Size(blockWidth, blockHight));

        //        int lastId;
        //        for (int j = 0; j < contours.Count; j++)
        //        {
        //            if (br.Contains(contours[j].BoundingRectangle))
        //            {
        //                contours.RemoveAt(j);
        //                j--;
        //            }

        //        }
        //        //deskew()
        //    }
        //    return image2.ToBitmap();
        //}
    }

    internal class Contour
    {
        public Contour(Array hierarchy, int i, VectorOfPoint points)
        {
            I = i;
            Points = points;
            NextSiblingId = (int)hierarchy.GetValue(0, i, HierarchyKey.NextSibling);
            PreviousSiblingId = (int)hierarchy.GetValue(0, i, HierarchyKey.PreviousSibling);
            FirstChildId = (int)hierarchy.GetValue(0, i, HierarchyKey.FirstChild);
            ParentId = (int)hierarchy.GetValue(0, i, HierarchyKey.Parent);
        }
        public class HierarchyKey
        {
            public const int NextSibling = 0;
            public const int PreviousSibling = 1;
            public const int FirstChild = 2;
            public const int Parent = 3;
        }

        public readonly int I;
        public readonly VectorOfPoint Points;

        public readonly int NextSiblingId = 0;
        public readonly int PreviousSiblingId = 1;
        public readonly int FirstChildId = 2;
        public readonly int ParentId = 3;

        public float Angle
        {
            get
            {
                if (_Angle < -400)
                {
                    if (RotatedRect.Size.Width > RotatedRect.Size.Height)
                        _Angle = 90 + RotatedRect.Angle;
                    else
                        _Angle = RotatedRect.Angle;
                }
                return _Angle;
            }
        }
        float _Angle = -401;

        public PointF[] RotatedRectPoints
        {
            get
            {
                if (_RotatedRectPoints == null)
                    _RotatedRectPoints = RotatedRect.GetVertices();
                return _RotatedRectPoints;
            }
        }
        PointF[] _RotatedRectPoints = null;

        public RotatedRect RotatedRect
        {
            get
            {
                if (_RotatedRect.Size == RotatedRect.Empty.Size)
                    _RotatedRect = Emgu.CV.CvInvoke.FitEllipse(Points);
                return _RotatedRect;
            }
        }
        RotatedRect _RotatedRect = RotatedRect.Empty;

        public float Length
        {
            get
            {
                if (_Length < 0)
                    _Length = Math.Max(RotatedRect.Size.Width, RotatedRect.Size.Height);
                return _Length;
            }
        }
        float _Length = -1;

        public double Area
        {
            get
            {
                if (_Area < 0)
                    _Area = Emgu.CV.CvInvoke.ContourArea(Points);
                return _Area;
            }
        }
        double _Area = -1;

        public RectangleF MinAreaRectF
        {
            get
            {
                if (_MinAreaRectF == RectangleF.Empty)
                    _MinAreaRectF = RotatedRect.MinAreaRect();
                return _MinAreaRectF;
            }
        }
        RectangleF _MinAreaRectF = RectangleF.Empty;

        public Rectangle BoundingRectangle
        {
            get
            {
                if (_BoundingRectangle == Rectangle.Empty)
                    _BoundingRectangle = CvInvoke.BoundingRectangle(Points);
                return _BoundingRectangle;
            }
        }
        Rectangle _BoundingRectangle = Rectangle.Empty;

    }
}