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
    /// <summary>
    /// Pdf page parsing API (!!! to be implemented)
    /// </summary>
    public partial class Page
    {
        #region old API

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

        public List<Bitmap> GetOcrTextLineImages(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            return (List<Bitmap>)fai.GetValue(Template.Field.Types.OcrTextLineImages);
        }

        #endregion

        #region new API for multiple matches (!!!TBD)

        public IEnumerable<object> GetValueAll(string fieldName, out Template.Field actualField, Template.Field.Types? type = null)
        {
            FieldMatchEnumerator fm = getFieldMatchEnumerator(fieldName, type);
            actualField = fm?.Field;
            return fm?.GetMatchs();
        }

        public IEnumerable<string> GetTextAll(string fieldName)
        {
            return GetValueAll(fieldName, out _).Cast<string>();
        }

        public string GetText1(string fieldName)
        {
            return GetTextAll(fieldName).FirstOrDefault();
        }

        public IEnumerable<List<string>> GetTextLinesAll(string fieldName)
        {
            return GetValueAll(fieldName, out _).Cast<List<string>>();
        }

        public List<string> GetTextLines1(string fieldName)
        {
            return GetTextLinesAll(fieldName).FirstOrDefault();
        }

        public IEnumerable<List<CharBox>> GetCharBoxsAll(string fieldName)
        {
            return GetValueAll(fieldName, out _).Cast<List<CharBox>>();
        }

        public List<CharBox> GetCharBoxes1(string fieldName)
        {
            return GetCharBoxsAll(fieldName).FirstOrDefault();
        }

        public IEnumerable<Bitmap> GetImageAll(string fieldName)
        {
            return GetValueAll(fieldName, out _).Cast<Bitmap>();
        }

        public Bitmap GetImage1(string fieldName)
        {
            return GetImageAll(fieldName).FirstOrDefault();
        }

        public IEnumerable<List<Bitmap>> GetTextLineImagesAll(string fieldName)
        {
            return GetValueAll(fieldName, out _).Cast<List<Bitmap>>();
        }

        public List<Bitmap> GetTextLineImages1(string fieldName)
        {
            return GetTextLineImagesAll(fieldName).FirstOrDefault();
        }

        #endregion
    }
}