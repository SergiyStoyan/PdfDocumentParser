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
    public class Page : IDisposable
    {
        internal Page(PageCollection pageCollection, int pageI)
        {
            this.pageCollection = pageCollection;
            Number = pageI;
        }
        public readonly int Number;
        readonly PageCollection pageCollection;

        ~Page()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_pdfCharBoxs != null)
                {
                    _pdfCharBoxs = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                {
                    _activeTemplateOcrCharBoxs = null;
                }
            }
        }

        internal Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = Pdf.RenderBitmap(pageCollection.PdfFile, Number, Settings.Constants.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        internal void OnActiveTemplateUpdating(Template newTemplate)
        {
            if (pageCollection.ActiveTemplate == null)
                return;

            if (newTemplate == null || newTemplate.PageRotation != pageCollection.ActiveTemplate.PageRotation || newTemplate.AutoDeskew != pageCollection.ActiveTemplate.AutoDeskew)
            {
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                    _activeTemplateOcrCharBoxs = null;

                anchorHashes2anchorActualInfo.Clear();
            }
        }

        internal Bitmap GetRectangleFromActiveTemplateBitmap(float x, float y, float w, float h)
        {
            Rectangle r = new Rectangle(0, 0, ActiveTemplateBitmap.Width, ActiveTemplateBitmap.Height);
            r.Intersect(new Rectangle((int)x, (int)y, (int)w, (int)h));
            return ActiveTemplateBitmap.Clone(r, System.Drawing.Imaging.PixelFormat.Undefined);
            //return ImageRoutines.GetCopy(ActiveTemplateBitmap, new RectangleF(x, y, w, h));
        }

        internal Bitmap ActiveTemplateBitmap
        {
            get
            {
                if (_activeTemplateBitmap == null)
                {
                    if (pageCollection.ActiveTemplate == null)
                        return null;

                    //From stackoverflow:
                    //Using the Graphics class to modify the clone (created with .Clone()) will not modify the original.
                    //Similarly, using the LockBits method yields different memory blocks for the original and clone.
                    //...change one random pixel to a random color on the clone... seems to trigger a copy of all pixel data from the original.
                    Bitmap b = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.PixelFormat.Undefined);

                    switch (pageCollection.ActiveTemplate.PageRotation)
                    {
                        case Template.PageRotations.NONE:
                            break;
                        case Template.PageRotations.Clockwise90:
                            b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case Template.PageRotations.Clockwise180:
                            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case Template.PageRotations.Clockwise270:
                            b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case Template.PageRotations.AutoDetection:
                            float confidence;
                            Tesseract.Orientation o = Ocr.This.DetectOrientation(b, out confidence);
                            switch (o)
                            {
                                case Tesseract.Orientation.PageUp:
                                    break;
                                case Tesseract.Orientation.PageRight:
                                    b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case Tesseract.Orientation.PageDown:
                                    b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case Tesseract.Orientation.PageLeft:
                                    b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + o);
                            }
                            break;
                        default:
                            throw new Exception("Unknown option: " + pageCollection.ActiveTemplate.PageRotation);
                    }

                    if (pageCollection.ActiveTemplate.AutoDeskew)
                    {
                        using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                        {
                            //image.Density = new PointD(600, 600);
                            //image.AutoLevel();
                            //image.Negate();
                            //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                            //image.Negate();
                            image.Deskew(new ImageMagick.Percentage(pageCollection.ActiveTemplate.AutoDeskewThreshold));
                            //image.AutoThreshold(AutoThresholdMethod.OTSU);
                            //image.Despeckle();
                            //image.WhiteThreshold(new Percentage(20));
                            //image.Trim();
                            b.Dispose();
                            b = image.ToBitmap();
                        }
                    }

                    _activeTemplateBitmap = b;
                }
                return _activeTemplateBitmap;
            }
        }

        Bitmap _activeTemplateBitmap = null;

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
                throw new Exception("Anchor[id=" + anchorId + "] is not defined.");

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

        internal ImageData ActiveTemplateImageData
        {
            get
            {
                if (_activeTemplateImageData == null)
                    _activeTemplateImageData = new ImageData(ActiveTemplateBitmap);
                return _activeTemplateImageData;
            }
        }
        ImageData _activeTemplateImageData;

        internal List<Pdf.CharBox> PdfCharBoxs
        {
            get
            {
                if (_pdfCharBoxs == null)
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(pageCollection.PdfReader, Number);
                return _pdfCharBoxs;
            }
        }
        List<Pdf.CharBox> _pdfCharBoxs;

        internal List<Ocr.CharBox> ActiveTemplateOcrCharBoxs
        {
            get
            {
                if (_activeTemplateOcrCharBoxs == null)
                {
                    _activeTemplateOcrCharBoxs = Ocr.This.GetCharBoxs(ActiveTemplateBitmap);
                }
                return _activeTemplateOcrCharBoxs;
            }
        }
        List<Ocr.CharBox> _activeTemplateOcrCharBoxs;

        public bool IsCondition(string conditionName)
        {
            if (string.IsNullOrWhiteSpace(conditionName))
                throw new Exception("Condition name is not specified.");
            var c = pageCollection.ActiveTemplate.Conditions.FirstOrDefault(a => a.Name == conditionName);
            if (string.IsNullOrWhiteSpace(c.Value))
                throw new Exception("Condition '" + conditionName + "' is not defined.");
            return BooleanEngine.Parse(c.Value, this);
        }

        public object GetValue(string fieldName)
        {
            object o = false;
            foreach (Template.Field f in pageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName))
            {
                o = getValue(f);
                if (o != null)
                    return o;
            }
            if (o == null)
                return null;
            throw new Exception("These is no field[name=" + fieldName + "]");
        }

        object getValue(Template.Field field)
        {
            if (!field.IsSet())
                throw new Exception("Field is not set.");
            if (field.Rectangle.Width <= Settings.Constants.CoordinateDeviationMargin || field.Rectangle.Height <= Settings.Constants.CoordinateDeviationMargin)
                throw new Exception("Rectangle is malformed.");
            RectangleF r = field.Rectangle.GetSystemRectangleF();
            RectangleF r0 = r;
            if (field.LeftAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.LeftAnchor.Id);
                if (!aai.Found)
                    return null;
                r.X += aai.Shift.Width - field.LeftAnchor.Shift;
            }
            if (field.TopAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.TopAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Y += aai.Shift.Height - field.TopAnchor.Shift;
            }
            if (field.RightAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.RightAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Width += r0.X - r.X + aai.Shift.Width - field.RightAnchor.Shift;
                if (r.Width <= 0)
                    return null;
            }
            if (field.BottomAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.BottomAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Height += r0.Y - r.Y + aai.Shift.Height - field.BottomAnchor.Shift;
                if (r.Height <= 0)
                    return null;
            }
            switch (field.Type)
            {
                case Template.Field.Types.PdfText:
                    return Pdf.GetTextByTopLeftCoordinates(PdfCharBoxs, r, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold);
                case Template.Field.Types.OcrText:
                    return Ocr.This.GetText(ActiveTemplateBitmap, r);
                case Template.Field.Types.ImageData:
                    using (Bitmap rb = GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                    {
                        return ImageData.GetScaled(rb, Settings.Constants.Image2PdfResolutionRatio);
                    }
                default:
                    throw new Exception("Unknown option: " + field.Type);
            }
        }    

        public static string NormalizeText(string value)
        {
            if (value == null)
                return null;
            value = FieldPreparation.RemoveNonPrintablesRegex.Replace(value, " ");
            value = Regex.Replace(value, @"\s+", " ");
            value = value.Trim();
            return value;
        }
    }
}