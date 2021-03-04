//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
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
            readonly public Template.Anchor Anchor;
            public PointF Position { get; private set; } = new PointF(-1, -1);
            public bool Found { get { return Position.X >= 0; } }
            readonly public SizeF Shift;

            internal AnchorActualInfo(Template.Anchor anchor, Page page)
            {
                Anchor = anchor;

                //if (anchor.Type == Template.Anchor.Types.Script)//it is a special type of Anchor which is treated separately
                //{
                //    if (anchor.ParentAnchorId != null)
                //        throw new Exception("Anchor [" + anchor.Id + "] cannot be linked to another anchor.");
                //    Template.Anchor a = page.PageCollection.ActiveTemplate.Anchors.FirstOrDefault(x => x.ParentAnchorId == anchor.Id);
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
                    Template.Anchor pa = page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                    if (anchor == pa)
                        throw new Exception("Reference loop: anchor[Id=" + anchor.Id + "] is linked by an ancestor anchor.");
                    id = pa.ParentAnchorId;
                }

                page.findAnchor(Anchor, (PointF p) =>
                {
                    Position = p;
                    return false;
                });

                if (Found)
                    Shift = new SizeF(Position.X - Anchor.Position.X, Position.Y - Anchor.Position.Y);
            }
        }

        void findAnchor(Template.Anchor a, Func<PointF, bool> proceedOnFound)
        {
            if (a.ParentAnchorId != null)
            {
                Template.Anchor pa = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == a.ParentAnchorId);
                findAnchor(pa, (PointF p) =>
                 {
                     return !_findAnchor(a, p, proceedOnFound);
                 });
            }
            else
                _findAnchor(a, new PointF(), proceedOnFound);
        }

        bool _findAnchor(Template.Anchor a, PointF parentAnchorPoint0, Func<PointF, bool> proceedOnFound)
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
                        if (PageCollection.ActiveTemplate.IgnoreInvisiblePdfChars)
                            cbs = cbs.Where(x => !Pdf.InvisibleCharacters.Contains(x.Char)).ToList();
                        if (cbs.Count < 1)
                        {
                            int w = (int)(Bitmap.Width * Settings.Constants.Image2PdfResolutionRatio - rectangle.Width);
                            int h = (int)(Bitmap.Height * Settings.Constants.Image2PdfResolutionRatio - rectangle.Height);
                            for (int i = 0; i < w; i++)
                                for (int j = 0; j < h; j++)
                                {
                                    RectangleF actualR = new RectangleF(i, j, rectangle.Width, rectangle.Height);
                                    if (PdfCharBoxs.FirstOrDefault(x => actualR.Contains(x.R) && (!PageCollection.ActiveTemplate.IgnoreInvisiblePdfChars || !Pdf.InvisibleCharacters.Contains(x.Char))) == null
                                        && !proceedOnFound(actualR.Location)
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
                                    PdfCharBoxs.FirstOrDefault(x => actualR.Contains(x.R) && !tcbs.Contains(x) && (!PageCollection.ActiveTemplate.IgnoreInvisiblePdfChars || !Pdf.InvisibleCharacters.Contains(x.Char))) == null
                                    && !proceedOnFound(actualR.Location)
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
                            int w = (int)(ActiveTemplateBitmap.Width * Settings.Constants.Image2PdfResolutionRatio - rectangle.Width);
                            int h = (int)(ActiveTemplateBitmap.Height * Settings.Constants.Image2PdfResolutionRatio - rectangle.Height);
                            for (int i = 0; i < w; i++)
                                for (int j = 0; j < h; j++)
                                {
                                    RectangleF actualR = new RectangleF(i, j, rectangle.Width, rectangle.Height);
                                    if (ActiveTemplateOcrCharBoxs.FirstOrDefault(x => actualR.Contains(x.R)) == null
                                        && !proceedOnFound(actualR.Location)
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
                            //searchRectangleOcrCharBoxs = Ocr.This.GetCharBoxs(GetRectangleFromActiveTemplateBitmap(contaningRectangle.X / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Height / Settings.Constants.Image2PdfResolutionRatio));
                            //PointF searchRectanglePosition = new PointF(contaningRectangle.X < 0 ? 0 : contaningRectangle.X, contaningRectangle.Y < 0 ? 0 : contaningRectangle.Y);
                            //searchRectangleOcrCharBoxs.ForEach(x => { x.R.X += contaningRectangle.X; x.R.Y += contaningRectangle.Y; });
                            //RectangleF mainElementSearchRectangle = new RectangleF(searchRectangle.X - searchRectanglePosition.X, searchRectangle.Y - searchRectanglePosition.Y, searchRectangle.Width, searchRectangle.Height);
                            //tcb0s = searchRectangleOcrCharBoxs.Where(x => x.Char == cbs[0].Char && contaningRectangle.Contains(x.R));

                            Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Height / Settings.Constants.Image2PdfResolutionRatio);
                            if (b == null)
                                return false;
                            searchRectangleOcrCharBoxs = Ocr.This.GetCharBoxs(b);
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
                                    && !proceedOnFound(actualR.Location)
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
                            Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Height / Settings.Constants.Image2PdfResolutionRatio);
                            if (b == null)
                                return false;
                            using (b)
                            {
                                id0 = new ImageData(b);
                            }
                        }
                        Point? p_ = idv.Image.FindWithinImage(id0, idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, idv.FindBestImageMatch);
                        if (p_ == null)
                            return false;
                        Point p = (Point)p_;
                        return !proceedOnFound(new PointF(searchRectanglePosition.X + p.X, searchRectanglePosition.Y + p.Y));
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
                            Bitmap b = GetRectangleFromActiveTemplateBitmap(searchRectangle.X / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, searchRectangle.Height / Settings.Constants.Image2PdfResolutionRatio);
                            if (b == null)
                                return false;
                            ci0 = new CvImage(b);
                        }
                        CvImage.Match m = civ.Image.FindFirstMatchWithinImage(ci0, civ.Threshold, civ.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep);
                        if (m == null)
                            return false;
                        Point p = m.Rectangle.Location;
                        return !proceedOnFound(new PointF(searchRectanglePosition.X + p.X, searchRectanglePosition.Y + p.Y));
                        //foreach (CvImage.Match m in civ.Image.FindWithinImage(ci0,new Size(20,20), civ.Threshold, civ.ScaleDeviation, PageCollection.ActiveTemplate.CvImageScalePyramidStep))
                        //    return !proceedOnFound(new PointF(searchRectanglePosition.X + m.Rectangle.X, searchRectanglePosition.Y + m.Rectangle.Y));
                        //return false;
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