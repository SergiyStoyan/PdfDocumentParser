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
        public enum ValueTypes
        {
            Default,
            TextLines,
            CharBoxs
        }
        public object GetValue(string fieldName, ValueTypes valueType = ValueTypes.Default)
        {
            object o = false;
            //int fieldDefinitionIndex = 0;
            foreach (Template.Field f in pageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName))
            {
                o = getValue(f/*, fieldDefinitionIndex*/, valueType);
                if (o != null)
                    return o;
                //fieldDefinitionIndex++;
            }
            if (o == null)
                return null;
            throw new Exception("These is no field[name=" + fieldName + "]");
        }
        //Dictionary<string, FieldActualInfo> fieldHashes2fieldActualInfo = new Dictionary<string, FieldActualInfo>();
        //class FieldActualInfo
        //{

        //}

        RectangleF? getFieldActualRectange(Template.Field field)
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

        object getValue(Template.Field field/*, int fieldDefinitionIndex*/, ValueTypes valueType = ValueTypes.Default)
        {
            RectangleF? r_ = getFieldActualRectange(field);
            if (r_ == null)
                return null;
            RectangleF r = (RectangleF)r_;
            switch (field.Type)
            {
                case Template.Field.Types.PdfText:
                    Template.Field.PdfText pt = (Template.Field.PdfText)field;
                    switch (valueType)
                    {
                        case ValueTypes.Default:
                            List<string> ls;
                            if (pt.ColumnOfTable == null)
                                ls = Pdf.GetTextLinesSurroundedByRectangle(PdfCharBoxs, r, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold, pageCollection.ActiveTemplate.TextAutoInsertSpaceSubstitute);
                            else
                                ls = getTextLinesAsTableColumn(pt, r);
                                return string.Join("\r\n", ls);  
                        case ValueTypes.TextLines:
                            if (pt.ColumnOfTable == null)
                                ls = Pdf.GetTextLinesSurroundedByRectangle(PdfCharBoxs, r, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold, pageCollection.ActiveTemplate.TextAutoInsertSpaceSubstitute);
                            else
                                ls = getTextLinesAsTableColumn(pt, r);
                            return ls;
                        case ValueTypes.CharBoxs:
                            return Pdf.GetCharBoxsSurroundedByRectangle(PdfCharBoxs, r);
                        default:
                            throw new Exception("Unknown option: " + valueType);
                    }
                case Template.Field.Types.OcrText:
                    Template.Field.OcrText ot = (Template.Field.OcrText)field;
                    switch (valueType)
                    {
                        case ValueTypes.Default:
                            return Ocr.This.GetText(ActiveTemplateBitmap, r);
                        case ValueTypes.TextLines:
                            throw new Exception("To be implemented.");
                        case ValueTypes.CharBoxs:
                            return Ocr.GetCharBoxsSurroundedByRectangle(ActiveTemplateOcrCharBoxs, r);
                        default:
                            throw new Exception("Unknown option: " + valueType);
                    }
                case Template.Field.Types.ImageData:
                    switch (valueType)
                    {
                        case ValueTypes.Default:
                            using (Bitmap rb = GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                            {
                                return ImageData.GetScaled(rb, Settings.Constants.Image2PdfResolutionRatio);
                            }
                        case ValueTypes.TextLines:
                        case ValueTypes.CharBoxs:
                            throw new Exception("Option " + valueType + " does not go with this Field type");
                        default:
                            throw new Exception("Unknown option: " + valueType);
                    }
                default:
                    throw new Exception("Unknown option: " + field.Type);
            }
        }
        //List<string> getTextLinesAsTableColumn(Template.Field.PdfText field, RectangleF fieldR)
        //{//can optimized by caching!
        //    int fieldDefinitionIndex = pageCollection.ActiveTemplate.Fields.Where(x => x.Name == field.Name).TakeWhile(x => x != field).Count();
        //    List<Pdf.CharBox> cbs = null;
        //    Template.Field tableField = pageCollection.ActiveTemplate.Fields.Where(x => x.Name == field.ColumnOfTable).ElementAt(fieldDefinitionIndex);

        //    cbs = (List<Pdf.CharBox>)getValue(tableField, ValueTypes.CharBoxs);
        //    if (cbs == null)
        //        return null;

        //    List<string> ls = new List<string>();
        //    foreach (Pdf.Line l in Pdf.GetLines(cbs, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold, pageCollection.ActiveTemplate.TextAutoInsertSpaceSubstitute))
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        foreach (Pdf.CharBox cb in l.CharBoxs)
        //            if (cb.R.X >= fieldR.X && cb.R.Right <= fieldR.Right)
        //                sb.Append(cb.Char);
        //        ls.Add(sb.ToString());
        //    }
        //    return ls;
        //}
        List<string> getTextLinesAsTableColumn(Template.Field.PdfText field, RectangleF fieldR)
        {//can optimized by caching!
            RectangleF tableR = fieldR;
            Dictionary<string, List<Template.Field>> fieldName2orderedFields = new Dictionary<string, List<Template.Field>>();
            foreach (Template.Field.PdfText pt in pageCollection.ActiveTemplate.Fields.Where(x => x is Template.Field.PdfText).Select(x => (Template.Field.PdfText)x).Where(x => x.ColumnOfTable == field.ColumnOfTable))
            {
                List<Template.Field> fs;
                if (!fieldName2orderedFields.TryGetValue(pt.Name, out fs))
                {
                    fs = new List<Template.Field>();
                    fieldName2orderedFields[pt.Name] = fs;
                }
                fs.Add(pt);
            }
            int i = fieldName2orderedFields[field.Name].IndexOf(field);
            fieldName2orderedFields.Remove(field.Name);
            foreach (string fn in fieldName2orderedFields.Keys)
            {
                RectangleF? r_ = getFieldActualRectange(fieldName2orderedFields[fn][i]);
                if (r_ == null)
                    return null;
                RectangleF r = (RectangleF)r_;
                if (tableR.X > r.X)
                {
                    float right = tableR.Right;
                    tableR.X = r.X;
                    tableR.Width = right - tableR.X;
                }
                if (tableR.Right < r.Right)
                    tableR.Width += r.Right - tableR.Right;
            }
            List<Pdf.CharBox> cbs = Pdf.GetCharBoxsSurroundedByRectangle(PdfCharBoxs, tableR);
            List<string> ls = new List<string>();
            foreach (Pdf.Line l in Pdf.GetLines(cbs, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold, pageCollection.ActiveTemplate.TextAutoInsertSpaceSubstitute))
            {
                StringBuilder sb = new StringBuilder();
                foreach (Pdf.CharBox cb in l.CharBoxs)
                    if (cb.R.X >= fieldR.X && cb.R.Right <= fieldR.Right)
                        sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return ls;
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
    }
}