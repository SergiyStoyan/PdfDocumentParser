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

        object getValue(Template.Field field)
        {
            RectangleF? r_ = getFieldActualRectange(field);
            if (r_ == null)
                return null;
            RectangleF r = (RectangleF)r_;
            switch (field.Type)
            {
                case Template.Field.Types.PdfText:
                    Template.Field.PdfText pt = (Template.Field.PdfText)field;
                    if (pt.ValueAsCharBoxes)
                        return Pdf.GetCharBoxsSurroundedByRectangle(PdfCharBoxs, r);
                    if (field.Table == null)
                        return Pdf.GetTextSurroundedByRectangle(PdfCharBoxs, r, pageCollection.ActiveTemplate.TextAutoInsertSpaceThreshold, pageCollection.ActiveTemplate.TextAutoInsertSpaceSubstitute);
                    return getValueAsTableLines(field, r);
                case Template.Field.Types.OcrText:
                    Template.Field.OcrText ot = (Template.Field.OcrText)field;
                    if (ot.ValueAsCharBoxes)
                        return Ocr.GetCharBoxsSurroundedByRectangle(ActiveTemplateOcrCharBoxs, r);
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
        List<string> getValueAsTableLines(Template.Field field, RectangleF rectangle)
        {//!!!should be done with caching to evoid repeating calculations!
            RectangleF? r_ = getFieldActualRectange(field);
            if (r_ == null)
                return null;
            RectangleF tableR = (RectangleF)r_;
            Dictionary<string, List<Template.Field>> fieldName2orderedFields = new Dictionary<string, List<Template.Field>>();
            foreach (Template.Field f in pageCollection.ActiveTemplate.Fields)
            {
                if (f.Table != field.Table)
                    continue;
                List<Template.Field> fs;
                if (!fieldName2orderedFields.TryGetValue(f.Name, out fs))
                {
                    fs = new List<Template.Field>();
                    fieldName2orderedFields[f.Name] = fs;
                }
                fs.Add(f);
            }
            int i = fieldName2orderedFields[field.Name].IndexOf(field);
            fieldName2orderedFields.Remove(field.Name);
            foreach (string fn in fieldName2orderedFields.Keys)
            {
                r_ = getFieldActualRectange(fieldName2orderedFields[fn][i]);
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
                foreach (Pdf.CharBox cb in l.CharBoxes)
                    if (cb.R.X >= rectangle.X && cb.R.Right <= rectangle.Right)
                        sb.Append(cb.Char);
                ls.Add(sb.ToString());
            }
            return ls;
        }

        //class FieldActualInfo
        //{

        //}

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