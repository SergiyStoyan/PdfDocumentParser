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
            if (fai.ActualField.DefaultValueType.ToString().StartsWith("Pdf"))
                return (string)fai.GetValue(Template.Field.ValueTypes.PdfText);
            return (string)fai.GetValue(Template.Field.ValueTypes.OcrText);
        }

        public List<string> GetTextLines(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            if (fai.ActualField.DefaultValueType.ToString().StartsWith("Pdf"))
                return (List<string>)fai.GetValue(Template.Field.ValueTypes.PdfTextLines);
            return (List<string>)fai.GetValue(Template.Field.ValueTypes.OcrTextLines);
        }

        public Image GetImage(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            return (Image)fai.GetValue(Template.Field.ValueTypes.Image);
        }

        //public List<Image> GetTextLineImages(string fieldName)
        //{
        //    FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
        //    if (!fai.Found)
        //        return null;
        //    if (fai.ActualField.DefaultValueType.ToString().StartsWith("Pdf"))
        //        return (List<string>)fai.GetValue(Template.Field.ValueTypes.PdfTextLines);
        //    return (List<Image>)fai.GetValue(Template.Field.ValueTypes.OcrTextLineImages);
        //}

        /// <summary>
        /// Tries field definitions of the given name in turn until some is found on the page.
        /// </summary>
        /// <param name="fieldName">field is referenced by name because there may be several field-definitions for the same name</param>
        /// <param name="valueType">if not set then DefaultValueType is used</param>
        /// <returns></returns>
        public object GetValue(string fieldName, Template.Field.ValueTypes? valueType = null)
        {
            return GetValue(fieldName, out _, valueType);
        }

        /// <summary>
        /// Tries field definitions of the given name in turn until some is found on the page.
        /// </summary>
        /// <param name="fieldName">field is referenced by name because there may be several field-definitions for the same name</param>
        /// <param name="actualField">actual field definition which was found on the page</param>
        /// <param name="valueType">if not set then DefaultValueType is used</param>
        /// <returns></returns>
        public object GetValue(string fieldName, out Template.Field actualField, Template.Field.ValueTypes? valueType = null)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
            {
                actualField = null;
                return null;
            }
            actualField = fai.ActualField;
            return fai.GetValue(valueType == null ? fai.ActualField.DefaultValueType : (Template.Field.ValueTypes)valueType);
        }

        // !!!passing Template.Field as parameter would be deceitful for these reasons:
        // - it may belong to another template than ActiveTemplate;
        // - it implies that a Template.Field object is equivalent to a field while it is just one of its defintions;
        // - it may be the same by logic while being another as an object!
        //public object GetValue(Template.Field exactField, Template.Field.ValueTypes? valueType = null)
        //{
        //    RectangleF? ar = getFieldActualRectangle(exactField);
        //    FieldActualInfo fai = new FieldActualInfo(this, exactField, ar, exactField.ColumnOfTable != null ? getFoundFieldActualInfo(exactField.ColumnOfTable) : null);
        //    if (!fai.Found)
        //        return null;
        //    return fai.GetValue(valueType == null ? fai.ActualField.DefaultValueType : (Template.Field.ValueTypes)valueType);
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
        Dictionary<string, List<FieldActualInfo>> fieldNames2fieldActualInfos = new Dictionary<string, List<FieldActualInfo>>();

        internal class FieldActualInfo : IDisposable
        {
            ~FieldActualInfo()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (valueTypes2cachedValue.Count < 1)
                    return;
                foreach (object o in valueTypes2cachedValue.Values)
                    if (o is IDisposable)
                        ((IDisposable)o).Dispose();
                valueTypes2cachedValue.Clear();
            }

            readonly internal Template.Field ActualField;
            readonly internal RectangleF? ActualRectangle;
            readonly internal FieldActualInfo TableFieldActualInfo = null;
            readonly internal bool Found;

            internal object GetValue(Template.Field.ValueTypes valueType)
            {
                //    return getValue(valueType, true);//test; if works, remove
                //}

                //object getValue(Template.Field.ValueTypes valueType, bool cached)
                //{
                //    if (!cached)
                //        return getValue_(valueType);
                if (!valueTypes2cachedValue.TryGetValue(valueType, out object o))
                {//!!!to cache Table field values to re-use them internally only!!!
                    o = getValue_(valueType);
                    valueTypes2cachedValue[valueType] = o;
                }
                return o;
            }
            Dictionary<Template.Field.ValueTypes, object> valueTypes2cachedValue = new Dictionary<Template.Field.ValueTypes, object>();//!!!cache Table field values for internal reuse only!!! 
            object getValue_(Template.Field.ValueTypes valueType)
            {
                if (ActualRectangle == null || TableFieldActualInfo?.Found == false)
                    return null;
                switch (valueType)
                {
                    case Template.Field.ValueTypes.PdfText:
                        {
                            List<string> ss = getPdfTextLines();
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.ValueTypes.PdfTextLines:
                        return getPdfTextLines();
                    case Template.Field.ValueTypes.PdfCharBoxs:
                        return getPdfCharBoxs();
                    case Template.Field.ValueTypes.OcrText:
                        {
                            List<string> ss = getOcrTextLines();
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.ValueTypes.OcrTextLines:
                        return getOcrTextLines();
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        return getOcrCharBoxs();
                    case Template.Field.ValueTypes.Image:
                        return getImage();
                    case Template.Field.ValueTypes.OcrTextLineImages:
                        return getOcrTextLineImages();
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }
            readonly Page page;

            List<string> getPdfTextLines()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;

                if (ActualField.ColumnOfTable == null)
                    return Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, ar, page.PageCollection.ActiveTemplate.TextAutoInsertSpace);

                List<string> ls = new List<string>();
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.PdfCharBoxs);
                foreach (Line<Pdf.CharBox> l in GetLines(cbs, page.PageCollection.ActiveTemplate.TextAutoInsertSpace))
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
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.PdfCharBoxs);
                return cbs.Where(a => a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
            }


            List<string> getOcrTextLines()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;

                if (ActualField.ColumnOfTable == null)
                {
                    if (page.PageCollection.ActiveTemplate.FieldOcrMode.HasFlag(Template.FieldOcrModes.SingleFieldFromFieldImage))
                    {
                        string s = Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, ar, page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        return Regex.Split(s, "$", RegexOptions.Multiline).ToList();
                    }
                    else
                        return Ocr.GetTextLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar, page.PageCollection.ActiveTemplate.TextAutoInsertSpace);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
                List<string> ls = new List<string>();
                if (page.PageCollection.ActiveTemplate.FieldOcrMode.HasFlag(Template.FieldOcrModes.ColumnFieldFromFieldImage))
                {
                    List<Line<Ocr.CharBox>> ols = GetLinesWithAdjacentBorders(cbs, TableFieldActualInfo.ActualRectangle.Value);
                    foreach (Line<Ocr.CharBox> l in ols)
                    {
                        float x = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                        RectangleF r = new RectangleF(
                            x,
                            l.Top,
                           (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - x,
                           l.Bottom - l.Top
                            );
                        ls.Add(Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, r, page.PageCollection.ActiveTemplate.TesseractPageSegMode));
                    }
                }
                else
                {
                    foreach (Line<Ocr.CharBox> l in GetLines(cbs, page.PageCollection.ActiveTemplate.TextAutoInsertSpace))
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

                if (ActualField.ColumnOfTable == null)
                {
                    if (page.PageCollection.ActiveTemplate.FieldOcrMode.HasFlag(Template.FieldOcrModes.SingleFieldFromFieldImage))
                        return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                    else
                        return Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                if (page.PageCollection.ActiveTemplate.FieldOcrMode.HasFlag(Template.FieldOcrModes.ColumnFieldFromFieldImage))
                {
                    float x = ar.X > TableFieldActualInfo.ActualRectangle.Value.X ? ar.X : TableFieldActualInfo.ActualRectangle.Value.X;
                    float y = ar.Top > TableFieldActualInfo.ActualRectangle.Value.Top ? ar.Top : TableFieldActualInfo.ActualRectangle.Value.Top;
                    RectangleF r = new RectangleF(
                        x,
                        y,
                       (ar.Right < TableFieldActualInfo.ActualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.ActualRectangle.Value.Right) - x,
                       (ar.Bottom < TableFieldActualInfo.ActualRectangle.Value.Bottom ? ar.Bottom : TableFieldActualInfo.ActualRectangle.Value.Bottom) - y
                        );
                    return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                }
                else
                {
                    List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
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

                Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
                if (b == null)
                    return null;
                using (b)
                {
                    return GetImageScaled2Pdf(b);
                }
            }

            List<Bitmap> getOcrTextLineImages()
            {
                if (ActualRectangle == null)
                    return null;
                RectangleF ar = ActualRectangle.Value;

                List<Ocr.CharBox> cbs;
                if (ActualField.ColumnOfTable != null)
                {
                    if (!TableFieldActualInfo.Found)
                        return null;
                    cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
                }
                else
                    cbs = Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar);
                List<Bitmap> ls = new List<Bitmap>();
                List<Line<Ocr.CharBox>> ols = GetLinesWithAdjacentBorders(cbs, TableFieldActualInfo == null ? ar : TableFieldActualInfo.ActualRectangle.Value);
                foreach (Line<Ocr.CharBox> l in ols)
                {
                    RectangleF r = new RectangleF(ar.X, l.Top, ar.Width, l.Bottom - l.Top);
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