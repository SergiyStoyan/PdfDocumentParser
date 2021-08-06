//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public partial class Page
    {
        public string GetText(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            if (fai.ActualField is Template.Field.Pdf)
                return (string)fai.GetValue(Template.Field.Types.PdfText);
            return (string)fai.GetValue(Template.Field.Types.OcrText);
        }

        public List<string> GetTextLines(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            if (fai.ActualField is Template.Field.Pdf)
                return (List<string>)fai.GetValue(Template.Field.Types.PdfTextLines);
            return (List<string>)fai.GetValue(Template.Field.Types.OcrTextLines);
        }

        public List<CharBox> GetCharBoxes(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            if (fai.ActualField is Template.Field.Pdf)
                return (List<CharBox>)fai.GetValue(Template.Field.Types.PdfCharBoxs);
            return (List<CharBox>)fai.GetValue(Template.Field.Types.OcrCharBoxs);
        }

        public Bitmap GetImage(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            return (Bitmap)fai.GetValue(Template.Field.Types.Image);
        }

        //public List<Image> GetTextLineImages(string fieldName)
        //{
        //    FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
        //    if (!fai.Found)
        //        return null;
        //    if (fai.ActualField.DefaultValueType.ToString().StartsWith("Pdf"))
        //        return (List<string>)fai.GetValue(Template.Field.Types.PdfTextLines);
        //    return (List<Image>)fai.GetValue(Template.Field.Types.OcrTextLineImages);
        //}

        /// <summary>
        /// Tries field definitions of the given name in turn until some is found on the page.
        /// </summary>
        /// <param name="fieldName">field is referenced by name because there may be several field-definitions for the same name</param>
        /// <param name="type">if not set then DefaultValueType is used</param>
        /// <returns></returns>
        public object GetValue(string fieldName, Template.Field.Types? type = null)
        {
            return GetValue(fieldName, out _, type);
        }

        /// <summary>
        /// Tries field definitions of the given name in turn until some is found on the page.
        /// </summary>
        /// <param name="fieldName">field is referenced by name because there may be several field-definitions for the same name</param>
        /// <param name="actualField">actual field definition which was found on the page</param>
        /// <param name="type">if not set then DefaultValueType is used</param>
        /// <returns></returns>
        public object GetValue(string fieldName, out Template.Field actualField, Template.Field.Types? type = null)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
            {
                actualField = null;
                return null;
            }
            actualField = fai.ActualField;
            return fai.GetValue(type == null ? fai.ActualField.Type : (Template.Field.Types)type);
        }

        /// <summary>
        /// It is helpful when a field has more than 1 definition and its image is required.
        /// !!!Only must be used for the field returned by GetValue(string fieldName, out Template.Field actualField, Template.Field.Types? type = null)
        /// ATTENTION: using Template.Field as a parameter may be deceitful for these reasons:
        /// - it may belong to another template than ActiveTemplate;
        /// - it implies that a Template.Field object is equivalent to a field while it is just one of its defintions;
        /// - it may be the same by logic while being another as an object;
        /// </summary>
        /// <param name="exactField"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //public Bitmap GetImage(Template.Field exactField)
        //{
        //    RectangleF? ar = getFieldActualRectangle(exactField);
        //    FieldActualInfo fai = new FieldActualInfo(this, exactField, ar, exactField.ColumnOfTable != null ? getFoundFieldActualInfo(exactField.ColumnOfTable) : null);
        //    if (!fai.Found)
        //        return null;
        //    return (Bitmap)fai.GetValue(Template.Field.Types.Image);
        //}

        internal FieldActualInfo GetFieldActualInfo(Template.Field field)
        {
            List<FieldActualInfo> fais = getFieldActualInfos(field.Name);
            //FieldActualInfo fai = fais.Find(a => a.ActualField == field);!!!while the fields can be same by logic, they are different objects!
            string fs = Serialization.Json.Serialize(field, false, true);
            FieldActualInfo fai = fais.Find(a => Serialization.Json.Serialize(a.ActualField, false, true) == fs);
            if (fai == null)
            {
                RectangleF? ar = getFieldActualRectangle(field);
                fai = new FieldActualInfo(this, field, ar, field.ColumnOfTable != null ? getFoundFieldActualInfo(field.ColumnOfTable) : null);
                fais.Add(fai);
            }
            return fai;
        }

        FieldActualInfo getFoundFieldActualInfo(string fieldName)
        {
            return getFieldActualInfos(fieldName)[0];
        }

        List<FieldActualInfo> getFieldActualInfos(string fieldName)
        {
            if (!fieldNames2fieldActualInfos.TryGetValue(fieldName, out List<FieldActualInfo> fais))
            {
                fais = new List<FieldActualInfo>();
                fieldNames2fieldActualInfos[fieldName] = fais;
                if (PageCollection.ActiveTemplate == null)
                    throw new Exception("ActiveTemplate is not set for the PageCollection.");
                foreach (Template.Field f in PageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName))
                {
                    RectangleF? ar = getFieldActualRectangle(f);
                    FieldActualInfo fai = new FieldActualInfo(this, f, ar, f.ColumnOfTable != null ? getFoundFieldActualInfo(f.ColumnOfTable) : null);
                    if (fai.Found)
                    {
                        fais.Insert(0, fai);
                        break;
                    }
                    fais.Add(fai);
                }
                if (fais.Count < 1)
                    throw new Exception("Field[name=" + fieldName + "] does not exist.");
            }
            return fais;
        }
        HandyDictionary<string, List<FieldActualInfo>> fieldNames2fieldActualInfos = new HandyDictionary<string, List<FieldActualInfo>>(
            disposeValue: (List<FieldActualInfo> v) =>
            {
                if (v == null)
                    return;
                foreach (FieldActualInfo fai in v)
                    fai?.Dispose();
            }
        );

        internal class FieldActualInfo : IDisposable
        {
            ~FieldActualInfo()
            {
                Dispose();
            }

            public void Dispose()
            {
                types2cachedValue.Dispose();
            }

            readonly internal Template.Field ActualField;
            readonly internal RectangleF? ActualRectangle;
            readonly internal FieldActualInfo TableFieldActualInfo = null;
            readonly internal bool Found;

            internal object GetValue(Template.Field.Types type)
            {
                //    return getValue(type, true);//test; if works, remove
                //}

                //object getValue(Template.Field.Types type, bool cached)
                //{
                //    if (!cached)
                //        return getValue_(type);
                if (!types2cachedValue.TryGetValue(type, out object o))
                {//!!!to cache Table field values to re-use them internally only!!!
                    o = getValue_(type);
                    types2cachedValue[type] = o;
                }
                return o;
            }
            HandyDictionary<Template.Field.Types, object> types2cachedValue = new HandyDictionary<Template.Field.Types, object>(
               disposeValue: (object v) =>
                {
                    if (v == null)
                        return;
                    if (v is Bitmap)
                        ((Bitmap)v).Dispose();
                    else if (v is List<Bitmap>)
                        foreach (Bitmap b in (List<Bitmap>)v)
                            b?.Dispose();
                }
            );//!!!cache Table field values for internal reuse only!!! 
            object getValue_(Template.Field.Types type)
            {
                if (ActualRectangle == null || TableFieldActualInfo?.Found == false)
                    return null;
                switch (type)
                {
                    case Template.Field.Types.PdfText:
                        {
                            List<string> ss = getPdfTextLines();
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.Types.PdfTextLines:
                        return getPdfTextLines();
                    case Template.Field.Types.PdfCharBoxs:
                        return getPdfCharBoxs();
                    case Template.Field.Types.OcrText:
                        {
                            List<string> ss = getOcrTextLines();
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.Types.OcrTextLines:
                        return getOcrTextLines();
                    case Template.Field.Types.OcrCharBoxs:
                        return getOcrCharBoxs();
                    case Template.Field.Types.Image:
                        return getImage();
                    case Template.Field.Types.OcrTextLineImages:
                        return getOcrTextLineImages();
                    default:
                        throw new Exception("Unknown option: " + type);
                }
            }
            readonly Page page;

            List<string> getPdfTextLines()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                TextAutoInsertSpace textAutoInsertSpace = ActualField.GetTextAutoInsertSpace(page.PageCollection.ActiveTemplate);

                if (ActualField.ColumnOfTable == null)
                    return Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, ar, textAutoInsertSpace);

                List<string> ls = new List<string>();
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.PdfCharBoxs);
                foreach (Line<Pdf.CharBox> l in GetLines(cbs, textAutoInsertSpace, null))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Pdf.CharBox cb in l.CharBoxs)
                        if (cb.R.Left >= ar.Left && cb.R.Right <= ar.Right && cb.R.Top >= ar.Top && cb.R.Bottom <= ar.Bottom)
                            sb.Append(cb.Char);
                    ls.Add(sb.ToString());
                }
                return ls;
            }

            List<Pdf.CharBox> getPdfCharBoxs()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;

                if (ActualField.ColumnOfTable == null)
                    return Pdf.GetCharBoxsSurroundedByRectangle(page.PdfCharBoxs, ar);

                if (!TableFieldActualInfo.Found)
                    return null;
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.PdfCharBoxs);
                return cbs.Where(a => a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
            }

            List<string> getOcrTextLines()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                TextAutoInsertSpace textAutoInsertSpace = ActualField.GetTextAutoInsertSpace(page.PageCollection.ActiveTemplate);
                Template.Field.OcrSettings ocrSettings = ActualField.GetOcrSettings(page.PageCollection.ActiveTemplate);

                if (ActualField.ColumnOfTable == null)
                {
                    if (ocrSettings.SingleFieldFromFieldImage)
                    {
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, ocrSettings.TesseractPageSegMode);
                        if (cs == null)
                            return null;
                        return GetTextLines(cs, textAutoInsertSpace, ocrSettings.CharFilter);
                    }
                    else
                        return Ocr.GetTextLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar, textAutoInsertSpace, ocrSettings.CharFilter);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                List<string> ls = new List<string>();
                if (ocrSettings.ColumnCellFromCellImage)
                {
                    //List<Line<Ocr.CharBox>> ols = GetLinesWithAdjacentBorders(cbs, TableFieldActualInfo.ActualRectangle.Value);
                    List<Line<Ocr.CharBox>> ols = GetLines(cbs, null, ocrSettings.CharFilter);
                    foreach (Line<Ocr.CharBox> l in ols)
                    {
                        float x = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                        RectangleF r = new RectangleF(
                            x,
                            l.Top,
                           (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - x,
                           l.Bottom - l.Top
                            );
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, r, ocrSettings.TesseractPageSegMode);
                        ls.Add(cs != null ? string.Join("", GetTextLines(cs, textAutoInsertSpace, ocrSettings.CharFilter)) : "");
                    }
                }
                else
                {
                    foreach (Line<Ocr.CharBox> l in GetLines(cbs, textAutoInsertSpace, ocrSettings.CharFilter))
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Ocr.CharBox cb in l.CharBoxs)
                            if (cb.R.Left >= ar.Left && cb.R.Right <= ar.Right && cb.R.Top >= ar.Top && cb.R.Bottom <= ar.Bottom)
                                sb.Append(cb.Char);
                        ls.Add(sb.ToString());
                    }
                }
                return ls;
            }

            List<Ocr.CharBox> getOcrCharBoxs()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                Template.Field.OcrSettings ocrSettings = ActualField.GetOcrSettings(page.PageCollection.ActiveTemplate);

                if (ActualField.ColumnOfTable == null)
                {
                    if (ocrSettings.SingleFieldFromFieldImage)
                        return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, ocrSettings.TesseractPageSegMode);
                    else
                        return Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                if (ocrSettings.ColumnCellFromCellImage)
                {
                    float x = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                    float y = ar.Top > TableFieldActualInfo.ActualRectangle.Value.Top ? ar.Top : TableFieldActualInfo.ActualRectangle.Value.Top;
                    RectangleF r = new RectangleF(
                        x,
                        y,
                       (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - x,
                       (ar.Bottom < TableFieldActualInfo.ActualRectangle.Value.Bottom ? ar.Bottom : TableFieldActualInfo.ActualRectangle.Value.Bottom) - y
                        );
                    return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, ocrSettings.TesseractPageSegMode);
                }
                else
                {
                    List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                    return cbs.Where(a => /*!check: ar.Contains(a.R)*/ a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
                }
            }

            Bitmap getImage()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = ActualRectangle.Value;
                RectangleF r;

                if (ActualField.ColumnOfTable != null)
                {
                    if (!TableFieldActualInfo.Found)
                        return null;

                    float x = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                    float y = ar.Top > TableFieldActualInfo.ActualRectangle.Value.Top ? ar.Top : TableFieldActualInfo.ActualRectangle.Value.Top;
                    r = new RectangleF(
                        x,
                        y,
                       (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - x,
                       (ar.Bottom < TableFieldActualInfo.ActualRectangle.Value.Bottom ? ar.Bottom : TableFieldActualInfo.ActualRectangle.Value.Bottom) - y
                        );
                }
                else
                    r = ar;

                using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                {
                    if (b == null)
                        return null;
                    return GetImageScaled2Pdf(b);
                }
            }

            List<Bitmap> getOcrTextLineImages()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                Template.Field.OcrSettings ocrSettings = ActualField.GetOcrSettings(page.PageCollection.ActiveTemplate);

                List<Line<Ocr.CharBox>> ols;
                float left, width;
                if (ActualField.ColumnOfTable == null)
                {
                    if (ocrSettings.SingleFieldFromFieldImage)
                    {
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, ocrSettings.TesseractPageSegMode);
                        if (cs == null)
                            return null;
                        //List<Line<Ocr.CharBox>> ols = GetLinesWithAdjacentBorders(cbs, TableFieldActualInfo.ActualRectangle.Value);
                        ols = GetLines(cs, null, ocrSettings.CharFilter);
                    }
                    else
                        ols = Ocr.GetLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar, null, ocrSettings.CharFilter);
                    left = ar.Left;
                    width = ar.Width;
                }
                else
                {
                    if (!TableFieldActualInfo.Found)
                        return null;
                    List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                    if (ocrSettings.ColumnCellFromCellImage)
                        //List<Line<Ocr.CharBox>> ols = GetLinesWithAdjacentBorders(cbs, TableFieldActualInfo.ActualRectangle.Value);
                        ols = GetLines(cbs, null, ocrSettings.CharFilter);
                    else
                        ols = GetLines(cbs, null, ocrSettings.CharFilter);
                    left = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                    width = (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - left;
                }

                List<Bitmap> ls = new List<Bitmap>();
                foreach (Line<Ocr.CharBox> l in ols)
                {
                    RectangleF r = new RectangleF(left, l.Top, width, l.Bottom - l.Top);
                    using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                    {
                        ls.Add(b == null ? b : GetImageScaled2Pdf(b));
                    }
                }
                return ls;
            }

            internal FieldActualInfo(Page page, Template.Field actualfield, RectangleF? actualRectangle, FieldActualInfo tableFieldActualInfo)
            {
                this.page = page;
                ActualField = actualfield;
                ActualRectangle = actualRectangle;
                TableFieldActualInfo = tableFieldActualInfo;
                Found = ActualRectangle != null;
            }
        }

        RectangleF? getFieldActualRectangle(Template.Field field)
        {
            if (!field.IsSet())
                throw new Exception("Field is not set.");
            if (field.Rectangle.Width <= Settings.Constants.CoordinateDeviationMargin || field.Rectangle.Height <= Settings.Constants.CoordinateDeviationMargin)
                throw new Exception("Rectangle is malformed.");
            RectangleF r = field.Rectangle.GetSystemRectangleF();
            if (field.LeftAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.LeftAnchor.Id);
                if (!aai.Found)
                    return null;
                float right = r.Right;
                r.X += aai.Shift.Width - field.LeftAnchor.Shift;
                r.Width = right - r.X;
            }
            if (field.TopAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.TopAnchor.Id);
                if (!aai.Found)
                    return null;
                float bottom = r.Bottom;
                r.Y += aai.Shift.Height - field.TopAnchor.Shift;
                r.Height = bottom - r.Y;
            }
            if (field.RightAnchor != null)
            {
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.RightAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Width += aai.Shift.Width - field.RightAnchor.Shift;
            }
            if (field.BottomAnchor != null)
            {
                AnchorActualInfo aai = GetAnchorActualInfo(field.BottomAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Height += aai.Shift.Height - field.BottomAnchor.Shift;
            }
            //!!!???when all the anchors found then not null even if it is collapsed
            //if (r.Width <= 0 || r.Height <= 0)
            //    return null;
            return r;
        }

        internal static Bitmap GetImageScaled2Pdf(Image image)
        {
            int w = (int)Math.Round(image.Width * Settings.Constants.Image2PdfResolutionRatio, 0);
            int h = (int)Math.Round(image.Height * Settings.Constants.Image2PdfResolutionRatio, 0);
            if (w == 0)
                w = 1;
            if (h == 0)
                h = 1;
            Bitmap b = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(b))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, 0, 0, b.Width, b.Height);
            }
            b.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            return b;
        }
    }
}