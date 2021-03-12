//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
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
            public int BlockMaxLength = 1000;
            public int BlockMinSpan = 20;
            public Size StructuringElementSize = new Size(30, 1);
            public int ContourMaxCount = 10;
            public float AngleMaxDeviation = 1;
        }
        public enum Modes
        {
            SingleBlock = 1,//default
            ColumnOfBlocks = 2,
            RowOfBlocks = 4,
            ByBlockWithMaxLength = 7,//default
            ByMostUnidirectedBlocks = 8,
        }

        static public void Deskew(ref Bitmap bitmap, Config config)
        {
            switch (config.Mode)
            {
                case Modes.SingleBlock:
                    DeskewAsSingleBlock(ref bitmap, config.StructuringElementSize, config.ContourMaxCount, config.AngleMaxDeviation);
                    break;
                case Modes.ColumnOfBlocks:
                    DeskewAsColumnOfBlocks(ref bitmap, config.BlockMaxLength, config.BlockMinSpan, config.StructuringElementSize, config.ContourMaxCount, config.AngleMaxDeviation);
                    break;
                case Modes.RowOfBlocks:
                    throw new Exception("not implemented");
                    break;
                default:
                    throw new Exception("Unknown option: " + config.Mode);
            }
        }

        static public void DeskewAsSingleBlock(ref Bitmap bitmap, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, int offsetX = 50)//good
        {
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();
                bitmap = deskew(image, structuringElementSize, contourMaxCount, angleMaxDeviation, offsetX)?.ToBitmap();
            }
        }

        static Image<Rgb, byte> deskew(Image<Rgb, byte> image, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, int offsetX)//good
        {//https://becominghuman.ai/how-to-automatically-deskew-straighten-a-text-image-using-opencv-a0c30aed83df
            Image<Gray, byte> image2 = image.Convert<Gray, byte>();
            CvInvoke.BitwiseNot(image2, image2);//to negative
            CvInvoke.GaussianBlur(image2, image2, new Size(9, 9), 0);//remove small spots
            CvInvoke.Threshold(image2, image2, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
            Mat se = CvInvoke.GetStructuringElement(ElementShape.Rectangle, structuringElementSize, new Point(-1, -1));
            CvInvoke.Dilate(image2, image2, se, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            //Emgu.CV.CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(image2, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
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
            else if (cs.Count < 2)//done the most lengthy block
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
            Image<Rgb, byte> image3 = new Image<Rgb, byte>(image.Width + 2 * offsetX, image.Height);
            image3.ROI = new Rectangle(new Point(offsetX, 0), image.Size);
            image.CopyTo(image3);
            image3.ROI = Rectangle.Empty;
            if (angle == 0)
                return image;
            RotationMatrix2D rotationMat = new RotationMatrix2D();
            CvInvoke.GetRotationMatrix2D(new PointF((float)image3.Width / 2, (float)image3.Height / 2), angle, 1, rotationMat);
            CvInvoke.WarpAffine(image3, image3, rotationMat, image3.Size);
            return image3;
        }

        static public void DeskewAsColumnOfBlocks(ref Bitmap bitmap, int blockMaxLength, int blockMinSpan, Size structuringElementSize, int contourMaxCount, double angleMaxDeviation, int offsetX = 50)
        {
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();

                //return image.ToBitmap();
                Image<Rgb, byte> deskewedImage = new Image<Rgb, byte>(new Size(image.Width + 2 * offsetX, image.Height));
                //int lastBlockBottomLeft = -1;
                //int lastBlockBottomRight = -1;
                Image<Gray, byte> image2 = image.Convert<Gray, byte>();
                CvInvoke.BitwiseNot(image2, image2);
                //CvInvoke.Blur(image2, image2, new Size(3, 3), new Point(0, 0));
                CvInvoke.GaussianBlur(image2, image2, new Size(25, 25), 5);//remove small spots
                CvInvoke.Threshold(image2, image2, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
                Mat se = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
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
                            if (c.BoundingRectangle.Bottom + blockMinSpan <= minTop.BoundingRectangle.Top)
                                lastSpan = new Tuple<Contour, Contour>(c, minTop);
                        }

                        if (c.BoundingRectangle.Bottom > blockY + blockMaxLength && lastSpan != null)
                        {
                            blockBottom = lastSpan.Item1.BoundingRectangle.Bottom + blockMinSpan / 2;
                            break;
                        }
                    }

                    Rectangle blockRectangle = new Rectangle(0, blockY, image2.Width, blockBottom + 1 - blockY);
                    Image<Rgb, byte> blockImage = image.Copy(blockRectangle);
                    Image<Rgb, byte> deskewedBlockImage = deskew(blockImage, structuringElementSize, contourMaxCount, angleMaxDeviation, offsetX);
                    //int blockTopLeft = findOffsetLeft(deskewedBlockImage, 10).X;
                    //int blockTopRight = findOffsetRight(deskewedBlockImage, 10).X;
                    //if (lastBlockBottomLeft < 0)
                    //    lastBlockBottomLeft = blockTopLeft;
                    //if (lastBlockBottomRight < 0)
                    //    lastBlockBottomRight = blockTopRight;
                    //deskewedBlockImage.ROI = new Rectangle(blockTopLeft, 0, blockTopRight - blockTopLeft, deskewedBlockImage.Height);
                    //deskewedImage.ROI = new Rectangle(lastBlockBottomLeft - ((blockTopRight - blockTopLeft) - (lastBlockBottomRight - lastBlockBottomLeft)) / 2, blockRectangle.Y, blockTopRight - blockTopLeft, deskewedBlockImage.Height);
                    deskewedImage.ROI = new Rectangle(0, blockRectangle.Y, deskewedBlockImage.Width, deskewedBlockImage.Height);
                    deskewedBlockImage.CopyTo(deskewedImage);
                    deskewedImage.ROI = Rectangle.Empty;
                    //lastBlockBottomLeft = findOffsetLeft(deskewedBlockImage, -10).X;
                    //lastBlockBottomRight = findOffsetRight(deskewedBlockImage, -10).X;
                    // break;
                    blockY = blockBottom + 1;
                }
                int offsetLeft = findOffsetLeft(deskewedImage, deskewedImage.Height).X;
                int offsetRight = findOffsetRight(deskewedImage, deskewedImage.Height).X;
                deskewedImage.ROI = new Rectangle(offsetLeft, 0, offsetRight - offsetLeft, deskewedImage.Height);
                bitmap = deskewedImage?.ToBitmap();
            }
        }

        static Point findOffsetLeft(Image<Rgb, byte> image, int maxHeight)
        {
            int width = image.Data.GetLength(1);
            int height = image.Data.GetLength(0);
            int y1, y2;
            if (maxHeight >= 0)
            {
                y1 = height > maxHeight ? maxHeight : height - 1;
                y2 = 0;
            }
            else
            {
                y1 = height - 1;
                y2 = height > -maxHeight ? height + maxHeight : 0;
            }
            for (int x = 0; x < width; x++)
                for (int y = y1; y >= y2; y--)
                    if (image.Data[y, x, 0] > 0 || image.Data[y, x, 1] > 0 || image.Data[y, x, 2] > 0)
                        return new Point(x, y);
            return Point.Empty;
        }

        static Point findOffsetRight(Image<Rgb, byte> image, int maxHeight)
        {
            int width = image.Data.GetLength(1);
            int height = image.Data.GetLength(0);
            int y1, y2;
            if (maxHeight >= 0)
            {
                y1 = height > maxHeight ? maxHeight : height - 1;
                y2 = 0;
            }
            else
            {
                y1 = height - 1;
                y2 = height > -maxHeight ? height + maxHeight : 0;
            }
            for (int x = width - 1; x >= 0; x--)
                for (int y = y1; y >= y2; y--)
                    if (image.Data[y, x, 0] > 0 || image.Data[y, x, 1] > 0 || image.Data[y, x, 2] > 0)
                        return new Point(x, y);
            return Point.Empty;
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

    public class Contour
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