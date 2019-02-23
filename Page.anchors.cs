//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page 
    {
        internal PointF? GetAnchorPoint0(int anchorId)
        {
            List<List<RectangleF>> rss = GetAnchorActualInfo(anchorId).Rectangless;
            if (rss == null || rss.Count < 1)
                return null;
            RectangleF r = rss[rss.Count - 1][0];
            return new PointF(r.X, r.Y);
        }

        internal List<List<RectangleF>> GetAnchorRectangless(int anchorId)
        {
            return GetAnchorActualInfo(anchorId).Rectangless;
        }

        internal  AnchorActualInfo GetAnchorActualInfo(int anchorId)
        {
            Template.Anchor a = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[id=" + anchorId + "] does not exist.");
            if (!a.IsSet())
                throw new Exception("Anchor[id=" + anchorId + "] is not set.");

            StringBuilder sb = new StringBuilder(Serialization.Json.Serialize(a, false));
            for (int? id = a.ParentAnchorId; id != null;)
            {
                Template.Anchor pa = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                sb.Append(Serialization.Json.Serialize(pa, false));
                id = pa.ParentAnchorId;
            }
            string sa = sb.ToString();
            if (!anchorHashes2anchorActualInfo.TryGetValue(sa, out AnchorActualInfo anchorActualInfo))
            {
                anchorActualInfo = new AnchorActualInfo(a, this);
                anchorHashes2anchorActualInfo[sa] = anchorActualInfo;
            }
            return anchorActualInfo;
        }
        Dictionary<string, AnchorActualInfo> anchorHashes2anchorActualInfo = new Dictionary<string, AnchorActualInfo>();

        internal class AnchorActualInfo
        {
            readonly public Template.Anchor Anchor;
            readonly public List<List<RectangleF>> Rectangless = new List<List<RectangleF>>();
            public bool Found { get { return Rectangless.Count > 0; } }
            readonly public SizeF Shift;

            internal AnchorActualInfo(Template.Anchor anchor, Page page)
            {
                Anchor = anchor;

                //if (anchor.Type == Template.Anchor.Types.Script)//it is a special type of Anchor which is treated separately
                //{
                //    if (anchor.ParentAnchorId != null)
                //        throw new Exception("Anchor [" + anchor.Id + "] cannot be linked to another anchor.");
                //    Template.Anchor a = page.pageCollection.ActiveTemplate.Anchors.FirstOrDefault(x => x.ParentAnchorId == anchor.Id);
                //    if (a != null)
                //        throw new Exception("Anchor [" + anchor.Id + "] cannot be linked by another anchor but it is linked by anchor[" + a.Id + "]");

                //    Template.Anchor.Script s = (Template.Anchor.Script)anchor;
                //    if (!BooleanEngine.Parse(s.Expression, page))
                //        return;
                //    foreach (int rai in BooleanEngine.GetAnchorIds(s.Expression))
                //    {//RULE OF RESULTING ANCHOR: the first anchor in the expression that is found
                //        AnchorActualInfo aai = page.GetAnchorActualInfo(rai);
                //        if (aai.Found)
                //        {
                //            Rectangless = aai.Rectangless;
                //            Shift = aai.Shift;
                //            break;
                //        }
                //    }
                //    if (Rectangless.Count < 1)
                //        throw new Exception("No resulting anchor found for anchor[" + anchor.Id + "]. This means that its expression is malformed!");
                //return;
                //}
                for (int? id = anchor.ParentAnchorId; id != null;)
                {
                    Template.Anchor pa = page.pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                    if (anchor == pa)
                        throw new Exception("Reference loop: anchor[Id=" + anchor.Id + "] is linked by an ancestor anchor.");
                    id = pa.ParentAnchorId;
                }

                page.findAnchor(Anchor, (IEnumerable<RectangleF> rs) =>
                {
                    Rectangless.Add(rs.ToList());
                    return false;
                }, Rectangless);

                if (Found)
                {
                    RectangleF air = Anchor.MainElementInitialRectangle();
                    RectangleF r = Rectangless[Rectangless.Count - 1][0];
                    Shift = new SizeF(r.X - air.X, r.Y - air.Y);
                }
            }
        }

        void findAnchor(Template.Anchor a, Func<IEnumerable<RectangleF>, bool> proceedOnFound, List<List<RectangleF>> anchorRectangless)
        {
            if (a.ParentAnchorId != null)
            {
                Template.Anchor pa = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == a.ParentAnchorId);
                findAnchor(pa, (IEnumerable<RectangleF> prs) =>
                 {
                     if (_findAnchor(a, prs.First().Location, proceedOnFound))
                     {
                         anchorRectangless.Insert(0, prs.ToList());
                         return false;
                     }
                     return true;
                 }, anchorRectangless);
                return;
            }
            _findAnchor(a, new PointF(), proceedOnFound);
        }

        bool _findAnchor(Template.Anchor a, PointF parentAnchorPoint0, Func<IEnumerable<RectangleF>, bool> proceedOnFound)
        {
            RectangleF mainElementSearchRectangle;
            {
                RectangleF mainElementInitialRectangle = a.MainElementInitialRectangle();
                if (a.ParentAnchorId != null)
                {
                    RectangleF pameir = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == a.ParentAnchorId).MainElementInitialRectangle();
                    if (a.SearchRectangleMargin >= 0)
                        mainElementSearchRectangle = getSearchRectangle(new RectangleF(mainElementInitialRectangle.X + parentAnchorPoint0.X - pameir.X, mainElementInitialRectangle.Y + parentAnchorPoint0.Y - pameir.Y, mainElementInitialRectangle.Width, mainElementInitialRectangle.Height), a.SearchRectangleMargin);
                    else
                        mainElementSearchRectangle = new RectangleF();
                }
                else if (a.SearchRectangleMargin >= 0)
                    mainElementSearchRectangle = getSearchRectangle(mainElementInitialRectangle, a.SearchRectangleMargin);
                else
                    mainElementSearchRectangle = new RectangleF();
            }

            switch (a.Type)
            {
                case Template.Anchor.Types.PdfText:
                    {
                        Template.Anchor.PdfText ptv = (Template.Anchor.PdfText)a;
                        List<Template.Anchor.PdfText.CharBox> aes = ptv.CharBoxs;
                        if (aes.Count < 1)
                            return false;
                        IEnumerable<Pdf.CharBox> bt0s;
                        if (ptv.SearchRectangleMargin < 0)
                            bt0s = PdfCharBoxs.Where(x => x.Char == aes[0].Char);
                        else
                            bt0s = PdfCharBoxs.Where(x => x.Char == aes[0].Char && mainElementSearchRectangle.Contains(x.R));
                        List<Pdf.CharBox> bts = new List<Pdf.CharBox>();
                        foreach (Pdf.CharBox bt0 in bt0s)
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < aes.Count; i++)
                            {
                                PointF p;
                                if (ptv.PositionDeviationIsAbsolute)
                                    p = new PointF(bt0.R.X + aes[i].Rectangle.X - aes[0].Rectangle.X, bt0.R.Y + aes[i].Rectangle.Y - aes[0].Rectangle.Y);
                                else
                                    p = new PointF(bts[i - 1].R.X + aes[i].Rectangle.X - aes[i - 1].Rectangle.X, bts[i - 1].R.Y + aes[i].Rectangle.Y - aes[i - 1].Rectangle.Y);
                                foreach (Pdf.CharBox bt in PdfCharBoxs.Where(x => x.Char == aes[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= ptv.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= ptv.PositionDeviation)
                                    {
                                        bts.Add(bt);
                                        break;
                                    }
                                if (bts.Count - 1 < i)
                                    break;
                            }
                            if (bts.Count == aes.Count)
                                if (!proceedOnFound(bts.Select(x => x.R)))
                                    return true;
                        }
                    }
                    return false;
                case Template.Anchor.Types.OcrText:
                    {
                        Template.Anchor.OcrText ot = (Template.Anchor.OcrText)a;
                        List<Template.Anchor.OcrText.CharBox> aes = ot.CharBoxs;
                        if (aes.Count < 1)
                            return false;
                        List<Ocr.CharBox> contaningOcrCharBoxs;
                        PointF searchRectanglePosition = new PointF(0, 0);
                        IEnumerable<Ocr.CharBox> bt0s;
                        if (ot.OcrEntirePage)
                        {
                            contaningOcrCharBoxs = ActiveTemplateOcrCharBoxs;
                            if (ot.SearchRectangleMargin < 0)
                                bt0s = contaningOcrCharBoxs.Where(x => x.Char == aes[0].Char);
                            else
                                bt0s = contaningOcrCharBoxs.Where(x => x.Char == aes[0].Char && mainElementSearchRectangle.Contains(x.R));
                        }
                        else if (ot.SearchRectangleMargin < 0)
                        {
                            contaningOcrCharBoxs = ActiveTemplateOcrCharBoxs;
                            bt0s = contaningOcrCharBoxs.Where(x => x.Char == aes[0].Char);
                        }
                        else
                        {
                            RectangleF contaningRectangle = mainElementSearchRectangle;
                            for (int i = 1; i < ot.CharBoxs.Count; i++)
                                contaningRectangle = RectangleF.Union(contaningRectangle, getSearchRectangle(ot.CharBoxs[i].Rectangle.GetSystemRectangleF(), a.SearchRectangleMargin));
                            contaningOcrCharBoxs = Ocr.This.GetCharBoxs(GetRectangleFromActiveTemplateBitmap(contaningRectangle.X / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Height / Settings.Constants.Image2PdfResolutionRatio));
                            searchRectanglePosition = new PointF(contaningRectangle.X < 0 ? 0 : contaningRectangle.X, contaningRectangle.Y < 0 ? 0 : contaningRectangle.Y);
                            RectangleF unshiftedMainElementSearchRectangle = new RectangleF(mainElementSearchRectangle.X - searchRectanglePosition.X, mainElementSearchRectangle.Y - searchRectanglePosition.Y, mainElementSearchRectangle.Width, mainElementSearchRectangle.Height);
                            bt0s = contaningOcrCharBoxs.Where(x => x.Char == aes[0].Char && unshiftedMainElementSearchRectangle.Contains(x.R));
                        }
                        List<Ocr.CharBox> bts = new List<Ocr.CharBox>();
                        foreach (Ocr.CharBox bt0 in bt0s)
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < aes.Count; i++)
                            {
                                PointF p;
                                if (ot.PositionDeviationIsAbsolute)
                                    p = new PointF(bt0.R.X + aes[i].Rectangle.X - aes[0].Rectangle.X, bt0.R.Y + aes[i].Rectangle.Y - aes[0].Rectangle.Y);
                                else
                                    p = new PointF(bts[i - 1].R.X + aes[i].Rectangle.X - aes[i - 1].Rectangle.X, bts[i - 1].R.Y + aes[i].Rectangle.Y - aes[i - 1].Rectangle.Y);
                                foreach (Ocr.CharBox bt in contaningOcrCharBoxs.Where(x => x.Char == aes[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= ot.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= ot.PositionDeviation)
                                    {
                                        bts.Add(bt);
                                        break;
                                    }
                                if (bts.Count - 1 < i)
                                    break;
                            }
                            if (bts.Count == aes.Count)
                                if (!proceedOnFound(bts.Select(x => { x.R.X += searchRectanglePosition.X; x.R.Y += searchRectanglePosition.Y; return x.R; })))
                                    return true;
                        }
                    }
                    return false;
                case Template.Anchor.Types.ImageData://TBD:? implement recursive search of images in an anchor (- searching an image combination pixel by pixel will be very long!)
                    {
                        Template.Anchor.ImageData idv = (Template.Anchor.ImageData)a;
                        List<Template.Anchor.ImageData.ImageBox> ibs = idv.ImageBoxs;
                        if (ibs.Count < 1)
                            return false;
                        Point searchRectanglePosition;
                        ImageData id0;
                        if (idv.SearchRectangleMargin < 0)
                        {
                            id0 = ActiveTemplateImageData;
                            searchRectanglePosition = new Point(0, 0);
                        }
                        else
                        {
                            id0 = new ImageData(GetRectangleFromActiveTemplateBitmap(mainElementSearchRectangle.X / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Height / Settings.Constants.Image2PdfResolutionRatio));
                            searchRectanglePosition = new Point(mainElementSearchRectangle.X < 0 ? 0 : (int)mainElementSearchRectangle.X, mainElementSearchRectangle.Y < 0 ? 0 : (int)mainElementSearchRectangle.Y);
                        }
                        List<RectangleF> bestRs = null;
                        float minDeviation = float.MaxValue;
                        ibs[0].ImageData.FindWithinImage(id0, idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, (Point point0, float deviation) =>
                        {
                            point0 = new Point(searchRectanglePosition.X + point0.X, searchRectanglePosition.Y + point0.Y);
                            List<RectangleF> rs = new List<RectangleF>();
                            rs.Add(new RectangleF(point0, new SizeF(ibs[0].Rectangle.Width, ibs[0].Rectangle.Height)));
                            for (int i = 1; i < ibs.Count; i++)
                            {
                                RectangleF r;
                                if (idv.PositionDeviationIsAbsolute)
                                    r = new RectangleF(point0.X + ibs[i].Rectangle.X - ibs[0].Rectangle.X - idv.PositionDeviation, point0.Y + ibs[i].Rectangle.Y - ibs[0].Rectangle.Y - idv.PositionDeviation, ibs[i].Rectangle.Width + 2 * idv.PositionDeviation, ibs[i].Rectangle.Height + 2 * idv.PositionDeviation);
                                else
                                    r = new RectangleF(rs[i - 1].X + ibs[i].Rectangle.X - ibs[i - 1].Rectangle.X - idv.PositionDeviation, rs[i - 1].Y + ibs[i].Rectangle.Y - ibs[i - 1].Rectangle.Y - idv.PositionDeviation, ibs[i].Rectangle.Width + 2 * idv.PositionDeviation, ibs[i].Rectangle.Height + 2 * idv.PositionDeviation);
                                Point p;
                                using (Bitmap rb = GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                                {
                                    Point? p_ = ibs[i].ImageData.FindWithinImage(new ImageData(rb), idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, false);
                                    if (p_ == null)
                                        return true;
                                    p = (Point)p_;
                                }
                                rs.Add(new RectangleF(new PointF(r.X + p.X, r.Y + p.Y), new SizeF(ibs[i].Rectangle.Width, ibs[i].Rectangle.Height)));
                            }
                            if (!idv.FindBestImageMatch)
                            {
                                if (proceedOnFound(rs))
                                    return true;
                                bestRs = rs;
                                return false;
                            }
                            if (deviation < minDeviation)
                            {
                                minDeviation = deviation;
                                bestRs = rs;
                            }
                            return true;
                        });
                        if (bestRs != null && !proceedOnFound(bestRs))
                            return true;
                        return false;
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