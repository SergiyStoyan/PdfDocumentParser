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
        public object GetValue(string fieldName, Template.Field.ValueTypes? valueType = null)
        {
            FieldActualInfo fai = getFieldActualInfo(fieldName, true);
            if (!fai.Found)
                return null;
            return fai.GetValue(valueType == null ? fai.ActualField.DefaultValueType : (Template.Field.ValueTypes)valueType);
        }

        internal FieldActualInfo GetNonCachedFieldActualInfo(Template.Field field)
        {
            return new FieldActualInfo(this, field, getFieldActualRectangle(field), field.ColumnOfTable != null ? getFieldActualInfo(field.ColumnOfTable, false) : null);
        }

        FieldActualInfo getFieldActualInfo(string fieldName, bool cached)
        {
            if (!cached || !fieldNames2fieldActualInfo.TryGetValue(fieldName, out FieldActualInfo fai))
            {
                RectangleF? ar = null;
                Template.Field af = null;
                foreach (Template.Field f in pageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName))
                {
                    ar = getFieldActualRectangle(f);
                    af = f;
                    if (ar != null)
                        break;
                }
                if (af == null)
                    throw new Exception("Field[name=" + fieldName + "] does not exist.");
                fai = new FieldActualInfo(this, af, ar, af.ColumnOfTable != null ? getFieldActualInfo(af.ColumnOfTable, cached) : null);
                fieldNames2fieldActualInfo[fieldName] = fai;
            }
            return fai;
        }
        Dictionary<string, FieldActualInfo> fieldNames2fieldActualInfo = new Dictionary<string, FieldActualInfo>();
        internal class FieldActualInfo
        {
            readonly public Template.Field ActualField;
            readonly public RectangleF? ActualRectangle;
            readonly public FieldActualInfo TableFieldActualInfo = null;
            public bool Found { get { return ActualRectangle != null; } }
            public Dictionary<Template.Field.ValueTypes, object> ValueTypes2Values = new Dictionary<Template.Field.ValueTypes, object>();
            public object GetValue(Template.Field.ValueTypes valueType)
            {
                if (!ValueTypes2Values.TryGetValue(valueType, out object o))
                {
                    if (!Found)
                        o = null;
                    else
                        o = getValue(valueType);
                    ValueTypes2Values[valueType] = o;
                }
                return o;
            }
            readonly Page page;

            object getValue(Template.Field.ValueTypes valueType)
            {
                RectangleF r = (RectangleF)ActualRectangle;
                switch (valueType)
                {
                    case Template.Field.ValueTypes.PdfText:
                    case Template.Field.ValueTypes.PdfTextLines:
                        List<string> ls;
                        if (ActualField.ColumnOfTable == null)
                            ls = Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, r, page.pageCollection.ActiveTemplate.TextAutoInsertSpace);
                        else
                            ls = getTextLinesAsTableColumn();
                        if (valueType == Template.Field.ValueTypes.PdfText)
                            return string.Join("\r\n", ls);
                        return ls;
                    case Template.Field.ValueTypes.PdfCharBoxs:
                        return Pdf.GetCharBoxsSurroundedByRectangle(page.PdfCharBoxs, r);
                    case Template.Field.ValueTypes.OcrText:
                        return Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, r);
                    case Template.Field.ValueTypes.OcrTextLines:
                        throw new Exception("To be implemented.");
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        return Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, r);
                    case Template.Field.ValueTypes.Image:
                        using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                        {
                            return Page.GetScaledImage2Pdf(b);
                        }
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }

            List<string> getTextLinesAsTableColumn()
            {
                if (ActualRectangle == null)
                    return null;
                if (!TableFieldActualInfo.Found)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.ValueTypes.PdfCharBoxs);
                List<string> ls = new List<string>();
                foreach (Pdf.Line l in Pdf.GetLines(cbs, page.pageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Pdf.CharBox cb in l.CharBoxs)
                        if (cb.R.Left >= ar.Left && cb.R.Right <= ar.Right && cb.R.Top >= ar.Top && cb.R.Bottom <= ar.Bottom)
                            sb.Append(cb.Char);
                    ls.Add(sb.ToString());
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

        internal static Bitmap GetScaledImage2Pdf(Image image)
        {
            var b = new Bitmap((int)Math.Round(image.Width * Settings.Constants.Image2PdfResolutionRatio, 0), (int)Math.Round(image.Height * Settings.Constants.Image2PdfResolutionRatio, 0), PixelFormat.Format24bppRgb);
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