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

        ///// <summary>
        ///// !!!passing Template.Field would be deceitful for 2 reasons:
        ///// - it may belong to another template than ActiveTemplate;
        ///// - it implies that a Template.Field object is equivalent to a field while it is just one of its defintions;
        ///// </summary>
        ///// <param name="field"></param>
        ///// <returns></returns>
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
            FieldActualInfo fai = fais.Find(a => a.ActualField == field);
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
                    if (ar != null)
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
            internal bool Found { get { return ActualRectangle != null; } }

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
                RectangleF r = (RectangleF)ActualRectangle;
                switch (valueType)
                {
                    case Template.Field.ValueTypes.PdfText:
                        List<string> ls;
                        if (ActualField.ColumnOfTable == null)
                            ls = Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, r, page.PageCollection.ActiveTemplate.TextAutoInsertSpace);
                        else
                            ls = getPdfTextLinesAsTableColumn();
                        return string.Join("\r\n", ls);
                    case Template.Field.ValueTypes.PdfTextLines:
                        if (ActualField.ColumnOfTable == null)
                            return Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, r, page.PageCollection.ActiveTemplate.TextAutoInsertSpace);
                        return getPdfTextLinesAsTableColumn();
                    case Template.Field.ValueTypes.PdfCharBoxs:
                        return Pdf.GetCharBoxsSurroundedByRectangle(page.PdfCharBoxs, r);
                    case Template.Field.ValueTypes.OcrText:
                        if (ActualField.ColumnOfTable == null)
                            return Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, r, page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        return string.Join("\r\n", getOcrTextLinesAsTableColumn());
                    case Template.Field.ValueTypes.OcrTextLines:
                        if (ActualField.ColumnOfTable == null)
                            return Regex.Split(Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, r, page.PageCollection.ActiveTemplate.TesseractPageSegMode), "$", RegexOptions.Multiline);
                        return getOcrTextLinesAsTableColumn();
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        //return Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, r);
                        if (ActualField.ColumnOfTable == null)
                            return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, r, page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        return getOcrCharBoxsAsTableColumn();
                    case Template.Field.ValueTypes.Image:
                        Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
                        if (b == null)
                            return null;
                        using (b)
                        {
                            return GetScaledImage2Pdf(b);
                        }
                    case Template.Field.ValueTypes.OcrTextLineImages:
                        return getOcrTextLineImages();
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }
            readonly Page page;

            List<string> getPdfTextLinesAsTableColumn()
            {
                RectangleF ar = (RectangleF)ActualRectangle;
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.PdfCharBoxs);
                List<string> ls = new List<string>();
                foreach (Pdf.Line l in Pdf.GetLines(cbs, page.PageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Pdf.CharBox cb in l.CharBoxs)
                        if (cb.R.Left >= ar.Left && cb.R.Right <= ar.Right && cb.R.Top >= ar.Top && cb.R.Bottom <= ar.Bottom)
                            sb.Append(cb.Char);
                    ls.Add(sb.ToString());
                }
                return ls;
            }

            List<string> getOcrTextLinesAsTableColumn()
            {
                if (ActualRectangle == null)
                    return null;
                if (!TableFieldActualInfo.Found)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
                List<string> ls = new List<string>();
                foreach (Ocr.Line l in Ocr.GetLines(cbs, page.PageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Ocr.CharBox cb in l.CharBoxs)
                        if (cb.R.Left >= ar.Left && cb.R.Right <= ar.Right && cb.R.Top >= ar.Top && cb.R.Bottom <= ar.Bottom)
                            sb.Append(cb.Char);
                    ls.Add(sb.ToString());
                }
                return ls;
            }

            List<Ocr.CharBox> getOcrCharBoxsAsTableColumn()
            {
                if (ActualRectangle == null)
                    return null;
                if (!TableFieldActualInfo.Found)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
                return cbs.Where(a => a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
            }

            List<Bitmap> getOcrTextLineImages()
            {
                if (ActualRectangle == null)
                    return null;
                if (ActualField.ColumnOfTable != null && !TableFieldActualInfo.Found)
                    return null;
                RectangleF ar = ActualRectangle.Value;
                List<Ocr.CharBox> cbs;
                if (ActualField.ColumnOfTable == null)
                    cbs = Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar);
                else
                    cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.OcrCharBoxs);
                List<Bitmap> ls = new List<Bitmap>();
                foreach (Ocr.Line l in Ocr.GetLines(cbs, page.PageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    RectangleF r = new RectangleF(ar.X, l.Top, ar.Width, l.Bottom - l.Top);
                    Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio);
                    if (b == null)
                        return null;
                    using (b)
                    {
                        ls.Add(GetScaledImage2Pdf(b));
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
            }
        }

        public class CharBox
        {
            public string Char;
            public System.Drawing.RectangleF R;
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
                Page.AnchorActualInfo aai = GetAnchorActualInfo(field.BottomAnchor.Id);
                if (!aai.Found)
                    return null;
                r.Height += aai.Shift.Height - field.BottomAnchor.Shift;
            }
            if (r.Width <= 0 || r.Height <= 0)
                return null;
            return r;
        }

        /// <summary>
        /// Auxiliary method which can be applied to a string during post-processing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NormalizeText(string value)
        {
            if (value == null)
                return null;
            value = FieldPreparation.ReplaceNonPrintableChars(value);
            value = Regex.Replace(value, @"\s+", " ");
            value = value.Trim();
            return value;
        }

        public static void NormalizeText(List<string> values)
        {
            for (int i = 0; i < values.Count; i++)
                values[i] = NormalizeText(values[i]);
        }

        internal static Bitmap GetScaledImage2Pdf(Image image)
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
            return b;
        }
    }
}