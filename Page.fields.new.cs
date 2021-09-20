//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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
    public partial class Page
    {
        FieldMatchEnumerator getFieldMatchEnumerator(string fieldName, Template.Field.Types? type)
        {
            if (!fieldNames2fieldMatchEnumerator.TryGetValue(fieldName, out FieldMatchEnumerator fme))
            {
                IEnumerable<Template.Field> fs = PageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName);
                if (!fs.Any())
                    throw new Exception("Field[name=" + fieldName + "] does not exist.");

                foreach (Template.Field f in fs)
                {
                    fme = new FieldMatchEnumerator(this, f, type);
                    if (fme.GetMatches().Any())
                    {
                        fieldNames2fieldMatchEnumerator[fieldName] = fme;
                        break;
                    }
                }
            }
            return fme;
        }
        HandyDictionary<string, FieldMatchEnumerator> fieldNames2fieldMatchEnumerator = new HandyDictionary<string, FieldMatchEnumerator>();

        internal class FieldMatchEnumerator
        {
            internal FieldMatchEnumerator(Page page, Template.Field field, Template.Field.Types? type)
            {
                this.page = page;
                this.Field = field;
                if (type == null)
                    type = field.Type;
                this.type = type.Value;
                TableFieldMatchEnumerator = field.ColumnOfTable != null ? page.getFieldMatchEnumerator(field.ColumnOfTable, ) : null;
            }
            readonly Page page;
            internal readonly Template.Field Field;
            Template.Field.Types type;
            internal readonly FieldMatchEnumerator TableFieldMatchEnumerator;

            internal IEnumerable<object> GetMatches()
            {
                foreach (RectangleF ar in page.getFieldMatchRectangles(Field))
                {
                    yield return getValue(ar);
                }
            }

            object getValue(RectangleF ar)
            {
                if (ar == null || TableFieldActualInfo?.Found == false)
                    return null;
                switch (type)
                {
                    case Template.Field.Types.PdfText:
                        {
                            List<string> ss = getPdfTextLines(ar);
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.Types.PdfTextLines:
                        return getPdfTextLines(ar);
                    case Template.Field.Types.PdfCharBoxs:
                        return getPdfCharBoxs(ar);
                    case Template.Field.Types.OcrText:
                        {
                            List<string> ss = getOcrTextLines(ar);
                            return ss == null ? null : string.Join("\r\n", ss);
                        }
                    case Template.Field.Types.OcrTextLines:
                        return getOcrTextLines(ar);
                    case Template.Field.Types.OcrCharBoxs:
                        return getOcrCharBoxs(ar);
                    case Template.Field.Types.Image:
                        return getImage(ar);
                    case Template.Field.Types.OcrTextLineImages:
                        return getOcrTextLineImages(ar);
                    default:
                        throw new Exception("Unknown option: " + type);
                }
            }

            List<string> getPdfTextLines(RectangleF ar)
            {
                TextAutoInsertSpace textAutoInsertSpace = Field.TextAutoInsertSpace != null ? Field.TextAutoInsertSpace : page.PageCollection.ActiveTemplate.TextAutoInsertSpace;

                if (Field.ColumnOfTable == null)
                    return Pdf.GetTextLinesSurroundedByRectangle(page.PdfCharBoxs, ar, textAutoInsertSpace);

                List<string> ls = new List<string>();
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldMatchEnumerator.GetMatches(Template.Field.Types.PdfCharBoxs);
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

            List<Pdf.CharBox> getPdfCharBoxs(RectangleF ar)
            {
                if (Field.ColumnOfTable == null)
                    return Pdf.GetCharBoxsSurroundedByRectangle(page.PdfCharBoxs, ar);

                if (!TableFieldActualInfo.Found)
                    return null;
                List<Pdf.CharBox> cbs = (List<Pdf.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.PdfCharBoxs);
                return cbs.Where(a => a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
            }

            List<string> getOcrTextLines(RectangleF ar)
            {
                Template.Field.Ocr aof = Field as Template.Field.Ocr;
                TextAutoInsertSpace textAutoInsertSpace = aof?.TextAutoInsertSpace != null ? aof?.TextAutoInsertSpace : page.PageCollection.ActiveTemplate.TextAutoInsertSpace;

                if (Field.ColumnOfTable == null)
                {
                    if (aof?.SingleFieldFromFieldImage ?? page.PageCollection.ActiveTemplate.SingleFieldFromFieldImage)
                    {
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, aof?.TesseractPageSegMode ?? page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        if (cs == null)
                            return null;
                        return GetTextLines(cs, textAutoInsertSpace, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    }
                    else
                        return Ocr.GetTextLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar, textAutoInsertSpace, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                List<string> ls = new List<string>();
                if (aof?.ColumnCellFromCellImage ?? page.PageCollection.ActiveTemplate.ColumnCellFromCellImage)
                {
                    List<Line<Ocr.CharBox>> ols = GetLines(cbs, null, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    if (aof?.AdjustLineBorders ?? page.PageCollection.ActiveTemplate.AdjustLineBorders)
                        AdjustBorders(ols, TableFieldActualInfo.actualRectangle.Value);
                    else
                        PadLines(ols, Field.LinePaddingY ?? page.PageCollection.ActiveTemplate.LinePaddingY);
                    foreach (Line<Ocr.CharBox> l in ols)
                    {
                        float x = ar.X > TableFieldActualInfo.actualRectangle.Value.X ? ar.X : TableFieldActualInfo.actualRectangle.Value.X;
                        RectangleF r = new RectangleF(
                            x,
                            l.Top,
                           (ar.Right < TableFieldActualInfo.actualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.actualRectangle.Value.Right) - x,
                           l.Bottom - l.Top
                            );
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, r, aof?.TesseractPageSegMode ?? page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        ls.Add(cs != null ? string.Join("", GetTextLines(cs, textAutoInsertSpace, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter)) : "");
                    }
                }
                else
                {
                    foreach (Line<Ocr.CharBox> l in GetLines(cbs, textAutoInsertSpace, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter))
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

            List<Ocr.CharBox> getOcrCharBoxs(RectangleF ar)
            {
                Template.Field.Ocr aof = Field as Template.Field.Ocr;

                if (Field.ColumnOfTable == null)
                {
                    if (aof?.SingleFieldFromFieldImage ?? page.PageCollection.ActiveTemplate.SingleFieldFromFieldImage)
                        return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, aof?.TesseractPageSegMode ?? page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                    else
                        return Ocr.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar);
                }

                if (!TableFieldActualInfo.Found)
                    return null;
                if (aof?.ColumnCellFromCellImage ?? page.PageCollection.ActiveTemplate.ColumnCellFromCellImage)
                {
                    float x = ar.X > TableFieldActualInfo.actualRectangle.Value.X ? ar.X : TableFieldActualInfo.actualRectangle.Value.X;
                    float y = ar.Top > TableFieldActualInfo.actualRectangle.Value.Top ? ar.Top : TableFieldActualInfo.actualRectangle.Value.Top;
                    RectangleF r = new RectangleF(
                        x,
                        y,
                       (ar.Right < TableFieldActualInfo.actualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.actualRectangle.Value.Right) - x,
                       (ar.Bottom < TableFieldActualInfo.actualRectangle.Value.Bottom ? ar.Bottom : TableFieldActualInfo.actualRectangle.Value.Bottom) - y
                        );
                    return Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, aof?.TesseractPageSegMode ?? page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                }
                else
                {
                    List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                    return cbs.Where(a => /*!check: ar.Contains(a.R)*/ a.R.Left >= ar.Left && a.R.Right <= ar.Right && a.R.Top >= ar.Top && a.R.Bottom <= ar.Bottom).ToList();
                }
            }

            Bitmap getImage(RectangleF ar)
            {
                RectangleF r;

                if (Field.ColumnOfTable != null)
                {
                    if (!TableFieldActualInfo.Found)
                        return null;

                    float x = ar.X > TableFieldActualInfo.actualRectangle.Value.X ? ar.X : TableFieldActualInfo.actualRectangle.Value.X;
                    float y = ar.Top > TableFieldActualInfo.actualRectangle.Value.Top ? ar.Top : TableFieldActualInfo.actualRectangle.Value.Top;
                    r = new RectangleF(
                        x,
                        y,
                       (ar.Right < TableFieldActualInfo.actualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.actualRectangle.Value.Right) - x,
                       (ar.Bottom < TableFieldActualInfo.actualRectangle.Value.Bottom ? ar.Bottom : TableFieldActualInfo.actualRectangle.Value.Bottom) - y
                        );
                }
                else
                    r = ar;

                using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Pdf2ImageResolutionRatio, r.Y / Settings.Constants.Pdf2ImageResolutionRatio, r.Width / Settings.Constants.Pdf2ImageResolutionRatio, r.Height / Settings.Constants.Pdf2ImageResolutionRatio))
                {
                    if (b == null)
                        return null;
                    return GetImageScaled2Pdf(b);
                }
            }

            List<Bitmap> getOcrTextLineImages(RectangleF ar)
            {
                Template.Field.Ocr aof = Field as Template.Field.Ocr;

                List<Line<Ocr.CharBox>> ols;
                float left, width;
                if (Field.ColumnOfTable == null)
                {
                    if (aof?.SingleFieldFromFieldImage ?? page.PageCollection.ActiveTemplate.SingleFieldFromFieldImage)
                    {
                        List<Ocr.CharBox> cs = Ocr.This.GetCharBoxsSurroundedByRectangle(page.ActiveTemplateBitmap, ar, aof?.TesseractPageSegMode ?? page.PageCollection.ActiveTemplate.TesseractPageSegMode);
                        if (cs == null)
                            return null;
                        ols = GetLines(cs, null, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    }
                    else
                        ols = Ocr.GetLinesSurroundedByRectangle(page.ActiveTemplateOcrCharBoxs, ar, null, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    if (aof?.AdjustLineBorders ?? page.PageCollection.ActiveTemplate.AdjustLineBorders)
                        AdjustBorders(ols, ar);
                    else
                        PadLines(ols, Field.LinePaddingY ?? page.PageCollection.ActiveTemplate.LinePaddingY);
                    left = ar.Left;
                    width = ar.Width;
                }
                else
                {
                    if (!TableFieldActualInfo.Found)
                        return null;
                    List<Ocr.CharBox> cbs = (List<Ocr.CharBox>)TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs);
                    if (aof?.ColumnCellFromCellImage ?? page.PageCollection.ActiveTemplate.ColumnCellFromCellImage)
                        ols = GetLines(cbs, null, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    else
                        ols = GetLines(cbs, null, Field.CharFilter ?? page.PageCollection.ActiveTemplate.CharFilter);
                    if (aof?.AdjustLineBorders ?? page.PageCollection.ActiveTemplate.AdjustLineBorders)
                        AdjustBorders(ols, TableFieldActualInfo.actualRectangle.Value);
                    else
                        PadLines(ols, Field.LinePaddingY ?? page.PageCollection.ActiveTemplate.LinePaddingY);
                    left = ar.X > TableFieldActualInfo.actualRectangle.Value.X ? ar.X : TableFieldActualInfo.actualRectangle.Value.X;
                    width = (ar.Right < TableFieldActualInfo.actualRectangle.Value.Right ? ar.Right : TableFieldActualInfo.actualRectangle.Value.Right) - left;
                }

                List<Bitmap> ls = new List<Bitmap>();
                foreach (Line<Ocr.CharBox> l in ols)
                {
                    RectangleF r = new RectangleF(left, l.Top, width, l.Bottom - l.Top);
                    using (Bitmap b = page.GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Pdf2ImageResolutionRatio, r.Y / Settings.Constants.Pdf2ImageResolutionRatio, r.Width / Settings.Constants.Pdf2ImageResolutionRatio, r.Height / Settings.Constants.Pdf2ImageResolutionRatio))
                    {
                        ls.Add(b == null ? b : GetImageScaled2Pdf(b));
                    }
                }
                return ls;
            }
        }

        IEnumerable<RectangleF> getFieldMatchRectangles(Template.Field field)
        {
            if (!field.IsSet())
                throw new Exception("Field is not set.");
            if (field.Rectangle.Width <= Settings.Constants.CoordinateDeviationMargin || field.Rectangle.Height <= Settings.Constants.CoordinateDeviationMargin)
                throw new Exception("Rectangle is malformed.");
            RectangleF r = field.Rectangle.GetSystemRectangleF();
            IEnumerator<SizeF> leftAnchorShifts = field.LeftAnchor != null ? getAnchorMatchEnumerator(field.LeftAnchor.Id).GetShifts() : null;
            IEnumerator<SizeF> topAnchorShifts = field.TopAnchor != null ? getAnchorMatchEnumerator(field.TopAnchor.Id).GetShifts() : null;
            IEnumerator<SizeF> rightAnchorShifts = field.RightAnchor != null ? getAnchorMatchEnumerator(field.RightAnchor.Id).GetShifts() : null;
            IEnumerator<SizeF> bottomAnchorShifts = field.BottomAnchor != null ? getAnchorMatchEnumerator(field.BottomAnchor.Id).GetShifts() : null;
            while (leftAnchorShifts?.Current != null || topAnchorShifts?.Current != null || rightAnchorShifts?.Current != null || bottomAnchorShifts?.Current != null)
            {
                leftAnchorShifts?.MoveNext();
                float right = r.Right;
                r.X += leftAnchorShifts.Current.Width - field.LeftAnchor.Shift;
                r.Width = right - r.X;
                topAnchorShifts?.MoveNext();
                float bottom = r.Bottom;
                r.Y += topAnchorShifts.Current.Height - field.TopAnchor.Shift;
                r.Height = bottom - r.Y;
                rightAnchorShifts?.MoveNext();
                r.Width += rightAnchorShifts.Current.Width - field.RightAnchor.Shift;
                bottomAnchorShifts?.MoveNext();
                r.Height += bottomAnchorShifts.Current.Height - field.BottomAnchor.Shift;
                //!!!???when all the anchors found then not null even if it is collapsed
                //if (r.Width <= 0 || r.Height <= 0)
                //    return null;
                yield return r;
            }
        }
    }
}