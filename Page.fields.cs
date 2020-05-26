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
        public FieldActualInfo this[string fieldName]
        {
            get
            {
                return getFieldActualInfo(fieldName, true);
            }
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

        public class FieldActualInfo
        {
            readonly public Template.Field ActualField;
            readonly public RectangleF? ActualRectangle;
            readonly public FieldActualInfo TableFieldActualInfo = null;
            public bool Found { get { return ActualRectangle != null; } }

            public string PdfText
            {
                get
                {
                    if (_PdfText == null && Found)
                    {
                        List<string> ls;
                        if (ActualField.ColumnOfTable == null)
                            ls = Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, (RectangleF)ActualRectangle, page.pageCollection.ActiveTemplate.TextAutoInsertSpace);
                        else
                            ls = getPdfTextLinesAsTableColumn();
                        _PdfText = string.Join("\r\n", ls);
                    }
                    return _PdfText;
                }
            }
            string _PdfText;

            public List<string> PdfTextLines
            {
                get
                {
                    if (_PdfTextLines == null && Found)
                    {
                        if (ActualField.ColumnOfTable == null)
                            _PdfTextLines = Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, (RectangleF)ActualRectangle, page.pageCollection.ActiveTemplate.TextAutoInsertSpace);
                        else
                            _PdfTextLines = getPdfTextLinesAsTableColumn();
                    }
                    return _PdfTextLines;
                }
            }
            List<string> _PdfTextLines;

            public List<PdfCharBox> PdfCharBoxs
            {
                get
                {
                    if (_PdfCharBoxs == null && Found)
                        _PdfCharBoxs = Pdf.GetCharBoxsSurroundedByRectangle(page.PdfCharBoxs, (RectangleF)ActualRectangle);
                    return _PdfCharBoxs;
                }
            }
            List<PdfCharBox> _PdfCharBoxs;

           public string OcrText
            {
                get
                {
                    if (_OcrText == null && Found)
                        _OcrText = Ocr.This.GetTextSurroundedByRectangle(page.ActiveTemplateBitmap, (RectangleF)ActualRectangle);
                    return _OcrText;
                }
            }
            string _OcrText;

            public List<string> OcrTextLines
            {
                get
                {
                    if (_OcrTextLines == null && Found)
                    {
                        if (ActualField.ColumnOfTable == null)
                            _OcrTextLines = Ocr.GetTextLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, (RectangleF)ActualRectangle, page.pageCollection.ActiveTemplate.TextAutoInsertSpace);
                        else
                            _OcrTextLines = getOcrTextLinesAsTableColumn();
                    }
                    return _OcrTextLines;
                }
            }
            List<string> _OcrTextLines;

            public List<OcrCharBox> OcrCharBoxs
            {
                get
                {
                    if (_OcrCharBoxs == null && Found)
                        _OcrCharBoxs = Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, (RectangleF)ActualRectangle);
                    return _OcrCharBoxs;
                }
            }

            List<OcrCharBox> _OcrCharBoxs;

            public Bitmap Image
            {
                get
                {
                    if (_Image == null && Found)
                    {
                        RectangleF r = (RectangleF)ActualRectangle;
                        using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                        {
                            _Image = GetScaledImage2Pdf(b);
                        }
                    }
                    return _Image;
                }
            }
            Bitmap _Image;

            readonly Page page;

            List<string> getPdfTextLinesAsTableColumn()
            {
                if (ActualRectangle == null)
                    return null;
                if (!TableFieldActualInfo.Found)
                    return null;
                RectangleF ar = (RectangleF)ActualRectangle;
                List<string> ls = new List<string>();
                foreach (Pdf.Line l in Pdf.GetLines(TableFieldActualInfo.PdfCharBoxs, page.pageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (PdfCharBox cb in l.CharBoxs)
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
                List<string> ls = new List<string>();
                foreach (Ocr.Line l in Ocr.GetLines(TableFieldActualInfo.OcrCharBoxs, page.pageCollection.ActiveTemplate.TextAutoInsertSpace))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (OcrCharBox cb in l.CharBoxs)
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