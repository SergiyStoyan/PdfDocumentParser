//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page
    {
        //internal AnchorActualInfo GetAnchorActualInfo(int anchorId)
        //{
        //    Template.Anchor a = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
        //    if (a == null)
        //        throw new Exception("Anchor[id=" + anchorId + "] does not exist.");
        //    if (!a.IsSet())
        //        throw new Exception("Anchor[id=" + anchorId + "] is not set.");

        //    StringBuilder sb = new StringBuilder(Serialization.Json.Serialize(a, false));
        //    for (int? id = a.ParentAnchorId; id != null;)
        //    {
        //        Template.Anchor pa = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
        //        sb.Append(Serialization.Json.Serialize(pa, false));
        //        id = pa.ParentAnchorId;
        //    }
        //    string sa = sb.ToString();
        //    if (!anchorHashes2anchorActualInfo.TryGetValue(sa, out AnchorActualInfo anchorActualInfo))
        //    {
        //        anchorActualInfo = new AnchorActualInfo(a, this);
        //        anchorHashes2anchorActualInfo[sa] = anchorActualInfo;
        //    }
        //    return anchorActualInfo;
        //}
        //Dictionary<string, AnchorActualInfo> anchorHashes2anchorActualInfo = new Dictionary<string, AnchorActualInfo>();

        internal AnchorActualInfo GetAnchorActualInfo(int anchorId)
        {
            Template.Anchor a = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[id=" + anchorId + "] does not exist.");
            if (!a.IsSet())
                throw new Exception("Anchor[id=" + anchorId + "] is not set.");

            if (!anchorIds2anchorActualInfo.TryGetValue(a.Id, out AnchorActualInfo anchorActualInfo))
            {
                anchorActualInfo = new AnchorActualInfo(a, this);
                anchorIds2anchorActualInfo[a.Id] = anchorActualInfo;
            }
            return anchorActualInfo;
        }
        Dictionary<int, AnchorActualInfo> anchorIds2anchorActualInfo = new Dictionary<int, AnchorActualInfo>();

        internal class AnchorActualInfo
        {
            readonly internal Template.Anchor Anchor;
            public PointF Position
            {
                get
                {
                    return position;
                }
                private set
                {
                    position = value;
                    Found = Position.X >= 0;
                    if (Found)
                        Shift = new SizeF(Position.X - Anchor.Position.X, Position.Y - Anchor.Position.Y);
                }
            }
            PointF position = new PointF(-1, -1);
            internal bool Found;
            internal SizeF Shift { get; private set; }
            internal AnchorActualInfo ParentAnchorActualInfo { get; private set; }

            internal AnchorActualInfo(Template.Anchor anchor, Page page)
            {
                Anchor = anchor;

                for (int? id = anchor.ParentAnchorId; id != null;)
                {
                    Template.Anchor pa = page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                    if (anchor == pa)
                        throw new Exception("Reference loop: anchor[Id=" + anchor.Id + "] is linked by an ancestor anchor.");
                    id = pa.ParentAnchorId;
                }

                findAnchor(page, (PointF p) =>
                {
                    Position = p;
                    return false;
                });
            }

            void findAnchor(Page page, Func<PointF, bool> findNext)
            {
                if (Anchor.ParentAnchorId != null)
                {
                    Template.Anchor pa = page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == Anchor.ParentAnchorId);
                    AnchorActualInfo paai = new AnchorActualInfo(pa);
                    paai.findAnchor(page, (PointF p) =>
                    {
                        bool found = page.findAnchor(Anchor, p, findNext);
                        if (found)
                        {
                            paai.Position = p;
                            ParentAnchorActualInfo = paai;
                        }
                        return !found;
                    });
                }
                else
                    page.findAnchor(Anchor, new PointF(), findNext);
            }

            AnchorActualInfo(Template.Anchor anchor)
            {
                Anchor = anchor;
            }
        }

        bool findAnchor(Template.Anchor a, PointF parentAnchorPoint0, Func<PointF, bool> findNext)
        {
            if (!a.IsSet())
                return false;
            RectangleF rectangle = a.Rectangle();
            RectangleF searchRectangle;
            {
                if (a.ParentAnchorId != null)
                {
                    RectangleF pameir = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == a.ParentAnchorId).Rectangle();
                    rectangle.X += parentAnchorPoint0.X - pameir.X;
                    rectangle.Y += parentAnchorPoint0.Y - pameir.Y;
                }
                if (a.SearchRectangleMargin >= 0)
                    searchRectangle = getSearchRectangle(rectangle, a.SearchRectangleMargin);
                else
                    searchRectangle = new RectangleF();//not used, just stub to compile
            }

            switch (a.Type)
            {
                case Template.Anchor.Types.PdfText:
                    {
                        Template.Anchor.PdfText pt = (Template.Anchor.PdfText)a;
                        List<Template.Anchor.PdfText.CharBox> cbs = pt.CharBoxs;
                        if (pt.IgnoreInvisibleChars)
                            cbs = cbs.Where(x => !Pdf.InvisibleCharacters.Contains(x.Char)).ToList();
                        if (cbs.Count < 1)
                        {
                            int w = Size.Width;
                            int h = Size.Height;
                            for (int i = 0; i < w; i++)
                                for (int j = 0; j < h; j++)
                                {
                                    RectangleF actualR = new RectangleF(i, j, rectangle.Width, rectangle.Height);
                                    if (//check that the rectangle contains nothing
                                        PdfCharBoxs.FirstOrDefault(x => actualR.Contains(x.R) && (!pt.IgnoreInvisibleChars || !Pdf.InvisibleCharacters.Contains(x.Char))) == null
                                        && !findNext(actualR.Location)
                                        )
                                        return true;
                                }
                            return false;
                        }
                        IEnumerable<Pdf.CharBox> tcb0s;
                        if (pt.SearchRectangleMargin < 0)
                            tcb0s = PdfCharBoxs.Where(x => x.Char == cbs[0].Char);
                        else
                            tcb0s = PdfCharBoxs.Where(x => x.Char == cbs[0].Char && searchRectangle.Contains(x.R));
                        List<Pdf.CharBox> tcbs = new List<Pdf.CharBox>();
                        foreach (Pdf.CharBox tcb0 in tcb0s)
                        {
                            tcbs.Clear();
                            tcbs.Add(tcb0);
                            for (int i = 1; i < cbs.Count; i++)
                            {
                                PointF p;
                                if (pt.PositionDeviationIsAbsolute)
                                    p = new PointF(tcb0.R.X + cbs[i].Rectangle.X - cbs[0].Rectangle.X, tcb0.R.Y + cbs[i].Rectangle.Y - cbs[0].Rectangle.Y);
                                else
                                    p = new PointF(tcbs[i - 1].R.X + cbs[i].Rectangle.X - cbs[i - 1].Rectangle.X, tcbs[i - 1].R.Y + cbs[i].Rectangle.Y - cbs[i - 1].Rectangle.Y);
                                foreach (Pdf.CharBox bt in PdfCharBoxs.Where(x => x.Char == cbs[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= pt.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= pt.PositionDeviation)
                                    {
                                        tcbs.Add(bt);
                                        break;
                                    }
                                if (tcbs.Count - 1 < i)
                                    break;
                            }
                            if (tcbs.Count == cbs.Count)
                            {
                                SizeF shift = new SizeF(tcbs[0].R.X - cbs[0].Rectangle.X, tcbs[0].R.Y - cbs[0].Rectangle.Y);
                                RectangleF actualR = new RectangleF(rectangle.X + shift.Width, rectangle.Y + shift.Height, rectangle.Width, rectangle.Height);
                                if (//check that the found rectangle contains only the anchor's boxes
                                 (pt.IgnoreOtherCharsInRectangle || PdfCharBoxs.FirstOrDefault(x => actualR.Contains(x.R) && !tcbs.Contains(x) && (!pt.IgnoreInvisibleChars || !Pdf.InvisibleCharacters.Contains(x.Char))) == null)
                                    && !findNext(actualR.Location)
                                )
                                    return true;
                            }
                        }
                    }
                    return false;
                case Template.Anchor.Types.OcrText:
                    {
                        Template.Anchor.OcrText ot = (Template.Anchor.OcrText)a;
                        List<Template.Anchor.OcrText.CharBox> cbs = ot.CharBoxs;
                        if (cbs.Count < 1)
                        {
                            int w = (int)(ActiveTemplateBitmap.Width * Settings.Constants.Pdf2ImageResolutionRatio - rectangle.Width);
                            int h = (int)(ActiveTemplateBitmap.Height * Settings.Constants.Pdf2ImageResolutionRatio - rectangle.Height);
                            for (int i = 0; i < w; i++)
                                for (int j = 0; j < h; j++)
                                {
                                    RectangleF actualR = new RectangleF(i, j, rectangle.Width, rectangle.Height);
                                    if (ActiveTemplateOcrCharBoxs.FirstOrDefault(x => actualR.Contains(x.R)) == null
                                        && !findNext(actualR.Location)
                                        )
                                        return true;
                                }
                            return false;
                        }
                        List<Ocr.CharBox> searchRectangleOcrCharBoxs;
                        IEnumerable<Ocr.CharBox> tcb0s;
                        if (ot.OcrEntirePage)
                        {
                            searchRectangleOcrCharBoxs = ActiveTemplateOcrCharBoxs;
                            if (ot.SearchRectangleMargin < 0)
                                tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char);
                            else
                                tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char && searchRectangle.Contains(x.R));
                        }
                        else if (ot.SearchRectangleMargin < 0)
                        {
                            searchRectangleOcrCharBoxs = ActiveTemplateOcrCharBoxs;
                            tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char);
                        }
                        else
                        {
                            //RectangleF contaningRectangle = searchRectangle;
                            //for (int i = 1; i < ot.CharBoxs.Count; i++)
                            //    contaningRectangle = RectangleF.Union(contaningRectangle, getSearchRectangle(ot.CharBoxs[i].Rectangle.GetSystemRectangleF(), a.SearchRectangleMargin));
                            //searchRectangleOcrCharBoxs = Ocr.This.GetCharBoxs(GetRectangleFromActiveTemplateBitmap(contaningRectangle.X / Settings.Constants.Pdf2ImageResolutionRatio, contaningRectangle.Y / Settings.Constants.Pdf2ImageResolutionRatio, contaningRectangle.Width / Settings.Constants.Pdf2ImageResolutionRatio, contaningRectangle.Height / Settings.Constants.Pdf2ImageResolutionRatio));
                            //PointF searchRectanglePosition = new PointF(contaningRectangle.X < 0 ? 0 : contaningRectangle.X, contaningRectangle.Y < 0 ? 0 : contaningRectangle.Y);
                            //searchRectangleOcrCharBoxs.ForEach(x => { x.R.X += contaningRectangle.X; x.R.Y += contaningRectangle.Y; });
                            //RectangleF mainElementSearchRectangle = new RectangleF(searchRectangle.X - searchRectanglePosition.X, searchRectangle.Y - searchRectanglePosition.Y, searchRectangle.Width, searchRectangle.Height);
                            //tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char && contaningRectangle.Contains(x.R));
                            using (Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Y / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Width / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Height / Settings.Constants.Pdf2ImageResolutionRatio))
                            {
                                if (b == null)
                                    return false;
                                searchRectangleOcrCharBoxs = Ocr.This.GetCharBoxs(b, PageCollection.ActiveTemplate.TesseractPageSegMode);
                            }
                            PointF searchRectanglePosition = new PointF(searchRectangle.X < 0 ? 0 : searchRectangle.X, searchRectangle.Y < 0 ? 0 : searchRectangle.Y);
                            searchRectangleOcrCharBoxs.ForEach(x => { x.R.X += searchRectanglePosition.X; x.R.Y += searchRectanglePosition.Y; });
                            tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char && searchRectangle.Contains(x.R));
                        }
                        List<Ocr.CharBox> tcbs = new List<Ocr.CharBox>();
                        foreach (Ocr.CharBox tcb0 in tcb0s)
                        {
                            tcbs.Clear();
                            tcbs.Add(tcb0);
                            for (int i = 1; i < cbs.Count; i++)
                            {
                                PointF p;
                                if (ot.PositionDeviationIsAbsolute)
                                    p = new PointF(tcb0.R.X + cbs[i].Rectangle.X - cbs[0].Rectangle.X, tcb0.R.Y + cbs[i].Rectangle.Y - cbs[0].Rectangle.Y);
                                else
                                    p = new PointF(tcbs[i - 1].R.X + cbs[i].Rectangle.X - cbs[i - 1].Rectangle.X, tcbs[i - 1].R.Y + cbs[i].Rectangle.Y - cbs[i - 1].Rectangle.Y);
                                foreach (Ocr.CharBox bt in searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= ot.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= ot.PositionDeviation)
                                    {
                                        tcbs.Add(bt);
                                        break;
                                    }
                                if (tcbs.Count - 1 < i)
                                    break;
                            }
                            if (tcbs.Count == cbs.Count)
                            {
                                SizeF shift = new SizeF(tcbs[0].R.X - cbs[0].Rectangle.X, tcbs[0].R.Y - cbs[0].Rectangle.Y);
                                RectangleF actualR = new RectangleF(rectangle.X + shift.Width, rectangle.Y + shift.Height, rectangle.Width, rectangle.Height);
                                if (//check that the found rectangle contains only the anchor's boxes
                                    searchRectangleOcrCharBoxs.FirstOrDefault(x => actualR.Contains(x.R) && !tcbs.Contains(x)) == null
                                    && !findNext(actualR.Location)
                                )
                                    return true;
                            }
                        }
                    }
                    return false;
                case Template.Anchor.Types.ImageData:
                    {
                        Template.Anchor.ImageData idv = (Template.Anchor.ImageData)a;
                        Point searchRectanglePosition;
                        ImageData id0;
                        if (idv.SearchRectangleMargin < 0)
                        {
                            id0 = ActiveTemplateImageData;
                            searchRectanglePosition = new Point(0, 0);
                        }
                        else
                        {
                            searchRectanglePosition = new Point(searchRectangle.X < 0 ? 0 : (int)searchRectangle.X, searchRectangle.Y < 0 ? 0 : (int)searchRectangle.Y);
                            using (Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Y / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Width / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Height / Settings.Constants.Pdf2ImageResolutionRatio))
                            {
                                if (b == null)
                                    return false;
                                id0 = new ImageData(b);
                            }
                        }
                        Point? p_ = idv.Image.FindWithinImage(id0, idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, idv.FindBestImageMatch);
                        if (p_ == null)
                            return false;
                        Point p = (Point)p_;
                        return !findNext(new PointF(searchRectanglePosition.X + p.X, searchRectanglePosition.Y + p.Y));
                    }
                case Template.Anchor.Types.CvImage:
                    {
                        Template.Anchor.CvImage civ = (Template.Anchor.CvImage)a;
                        Point searchRectanglePosition;
                        CvImage ci0;
                        if (civ.SearchRectangleMargin < 0)
                        {
                            ci0 = ActiveTemplateCvImage;
                            searchRectanglePosition = new Point(0, 0);
                        }
                        else
                        {
                            searchRectanglePosition = new Point(searchRectangle.X < 0 ? 0 : (int)searchRectangle.X, searchRectangle.Y < 0 ? 0 : (int)searchRectangle.Y);
                            using (Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Y / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Width / Settings.Constants.Pdf2ImageResolutionRatio, searchRectangle.Height / Settings.Constants.Pdf2ImageResolutionRatio))
                            {
                                if (b == null)
                                    return false;
                                ci0 = new CvImage(b);
                            }
                        }
                        if (civ.FindBestImageMatch)
                        {
                            //CvImage.Match m = civ.Image.FindFirstMatchWithinImage(ci0, civ.Threshold, civ.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
                            CvImage.Match m = civ.Image.FindBestMatchWithinImage(ci0, civ.Threshold, civ.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
                            if (m == null)
                                return false;
                            Point p = m.Rectangle.Location;
                            return !findNext(new PointF(searchRectanglePosition.X + p.X, searchRectanglePosition.Y + p.Y));
                        }
                        else
                        {//!!!this returns rather the first match than the best match
                            bool found = false;
                            civ.Image.FindMatchesWithinImage(ci0, civ.Threshold, civ.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep,
                                (CvImage.Match m) =>
                                {
                                    found = !findNext(new PointF(searchRectanglePosition.X + m.Rectangle.X, searchRectanglePosition.Y + m.Rectangle.Y));
                                    return !found;
                                }
                                );
                            return found;
                        }
                    }
                default:
                    throw new Exception("Unknown option: " + a.Type);
            }
        }
        RectangleF getSearchRectangle(RectangleF rectangle0, int margin/*, System.Drawing.RectangleF pageRectangle*/)
        {
            RectangleF r = new RectangleF(rectangle0.X - margin, rectangle0.Y - margin, rectangle0.Width + 2 * margin, rectangle0.Height + 2 * margin);
            //r.Intersect(pageRectangle);
            return r;
        }
    }
}