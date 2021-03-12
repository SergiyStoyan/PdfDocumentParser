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
            public int BlockMaxHeight = 1000;
            public int BlockMinSpan = 20;
            public Size StructuringElementSize = new Size(30, 1);
        }
        public enum Modes
        {
            SingleBlock,
            ColumnOfBlocks,
            RowOfBlocks,
        }

        static public void Deskew(ref Bitmap bitmap, Config config)
        {
            switch (config.Mode)
            {
                case Modes.SingleBlock:
                    DeskewAsSingleBlock(ref bitmap, config.StructuringElementSize);
                    break;
                case Modes.ColumnOfBlocks:
                    DeskewAsColumnOfBlocks(ref bitmap, config.BlockMaxHeight, config.BlockMinSpan, config.StructuringElementSize);
                    break;
                case Modes.RowOfBlocks:
                    throw new Exception("not implemented");
                    break;
                default:
                    throw new Exception("Unknown option: " + config.Mode);
            }
        }

        static public void DeskewAsSingleBlock(ref Bitmap bitmap, Size structuringElementSize)//good
        {
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();
                bitmap = deskew(image, structuringElementSize)?.ToBitmap();
            }
        }

        static Image<Rgb, byte> deskew(Image<Rgb, byte> image, Size structuringElementSize)//good
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
            int maxW = 0;
            double angle = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                RotatedRect rr = CvInvoke.MinAreaRect(contours[i]);
                Rectangle r = rr.MinAreaRect();
                int w = r.Width > r.Height ? r.Width : r.Height;
                if (maxW < w)
                {
                    maxW = w;
                    angle = rr.Angle;
                }
            }
            if (angle > 45)
                angle -= 90;
            else if (angle < -45)
                angle += 90;
            RotationMatrix2D rotationMat = new RotationMatrix2D();
            CvInvoke.GetRotationMatrix2D(new PointF((float)image.Width / 2, (float)image.Height / 2), angle, 1, rotationMat);
            Image<Rgb, byte> image3 = new Image<Rgb, byte>(image.Size);
            CvInvoke.WarpAffine(image, image3, rotationMat, image.Size);
            return image3;
        }

        static public void DeskewAsColumnOfBlocks(ref Bitmap bitmap, int blockMaxHeight, int blockMinSpan, Size structuringElementSize)
        {
            using (Image<Rgb, byte> image = bitmap.ToImage<Rgb, byte>())
            {
                bitmap.Dispose();

                //return image.ToBitmap();
                Image<Rgb, byte> deskewedimage = new Image<Rgb, byte>(image.Size);
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

                        if (c.BoundingRectangle.Bottom > blockY + blockMaxHeight && lastSpan != null)
                        {
                            blockBottom = lastSpan.Item1.BoundingRectangle.Bottom + blockMinSpan / 2;
                            break;
                        }
                    }

                    Rectangle blockRectangle = new Rectangle(0, blockY, image2.Width, blockBottom + 1 - blockY);
                    Image<Rgb, byte> blockImage = image.Copy(blockRectangle);
                    blockImage = deskew(blockImage, structuringElementSize);
                    deskewedimage.ROI = blockRectangle;
                    blockImage.CopyTo(deskewedimage);
                    deskewedimage.ROI = Rectangle.Empty;
                    // break;
                    blockY = blockBottom + 1;
                }
                bitmap = deskewedimage?.ToBitmap();
            }
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