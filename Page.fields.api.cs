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
        //#region new API with strong types(!!!TBD)
        //public void Get(Template.Field.Text field, FieldTextMatch fieldTextMatch)
        //{
        //    throw new Exception("TBD");
        //}
        //public delegate bool FieldTextMatch(string v);

        //public List<string> GetAll(Template.Field.Text field)
        //{
        //    List<string> vs = new List<string>();
        //    Get(field, (string v) => { vs.Add(v); return true; });
        //    return vs;
        //}

        //public string Get(Template.Field.Text field)
        //{
        //    string v0 = null;
        //    Get(field, (string v) => { v0 = v; return false; });
        //    return v0;
        //}

        //public void Get(Template.Field.TextLines field, FieldTextLinesMatch fieldTextLinesMatch)
        //{
        //    throw new Exception("TBD");
        //}
        //public delegate bool FieldTextLinesMatch(List<string> v);

        //public List<List<string>> GetAll(Template.Field.TextLines field)
        //{
        //    List<List<string>> vs = new List<List<string>>();
        //    Get(field, (List<string> v) => { vs.Add(v); return true; });
        //    return vs;
        //}

        //public List<string> Get(Template.Field.TextLines field)
        //{
        //    List<string> v0 = null;
        //    Get(field, (List<string> v) => { v0 = v; return false; });
        //    return v0;
        //}

        //public void Get(Template.Field.CharBoxs field, FieldCharBoxsMatch fieldCharBoxsMatch)
        //{
        //    throw new Exception("TBD");
        //}
        //public delegate bool FieldCharBoxsMatch(List<CharBox> v);

        //public List<List<CharBox>> GetAll(Template.Field.CharBoxs field)
        //{
        //    List<List<CharBox>> vs = new List<List<CharBox>>();
        //    Get(field, (List<CharBox> v) => { vs.Add(v); return true; });
        //    return vs;
        //}

        //public List<CharBox> Get(Template.Field.CharBoxs field)
        //{
        //    List<CharBox> v0 = null;
        //    Get(field, (List<CharBox> v) => { v0 = v; return false; });
        //    return v0;
        //}

        //public void Get(Template.Field.Image field, FieldImageMatch fieldImageMatch)
        //{
        //    throw new Exception("TBD");
        //}
        //public delegate bool FieldImageMatch(Bitmap v);

        //public List<Bitmap> GetAll(Template.Field.Image field)
        //{
        //    List<Bitmap> vs = new List<Bitmap>();
        //    Get(field, (Bitmap v) => { vs.Add(v); return true; });
        //    return vs;
        //}

        //public Bitmap Get(Template.Field.Image field)
        //{
        //    Bitmap v0 = null;
        //    Get(field, (Bitmap v) => { v0 = v; return false; });
        //    return v0;
        //}

        //public void Get(Template.Field.OcrTextLineImages field, FieldOcrTextLineImagesMatch fieldOcrTextLineImagesMatch)
        //{
        //    throw new Exception("TBD");
        //}
        //public delegate bool FieldOcrTextLineImagesMatch(List<Bitmap> v);

        //public List<List<Bitmap>> GetAll(Template.Field.OcrTextLineImages field)
        //{
        //    List<List<Bitmap>> vs = new List<List<Bitmap>>();
        //    Get(field, (List<Bitmap> v) => { vs.Add(v); return true; });
        //    return vs;
        //}

        //public List<Bitmap> Get(Template.Field.OcrTextLineImages field)
        //{
        //    List<Bitmap> v0 = null;
        //    Get(field, (List<Bitmap> v) => { v0 = v; return false; });
        //    return v0;
        //}

        //#endregion

        #region old API

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

        public List<Bitmap> GetTextLineImages(string fieldName)
        {
            FieldActualInfo fai = getFoundFieldActualInfo(fieldName);
            if (!fai.Found)
                return null;
            return (List<Bitmap>)fai.GetValue(Template.Field.Types.OcrTextLineImages);
        }

        #endregion

        #region new API for multiple matches (!!!TBD)
        public void GetText(string fieldName, FieldTextMatch fieldTextMatch)
        {
            throw new Exception("TBD");
        }
        public delegate bool FieldTextMatch(string v);

        public List<string> GetTextAll(string fieldName)
        {
            List<string> vs = new List<string>();
            GetText(fieldName, (string v) => { vs.Add(v); return true; });
            return vs;
        }

        public string GetText1(string fieldName)
        {
            string v0 = null;
            GetText(fieldName, (string v) => { v0 = v; return false; });
            return v0;
        }

        public void GetTextLines(string fieldName, FieldTextLinesMatch fieldTextLinesMatch)
        {
            throw new Exception("TBD");
        }
        public delegate bool FieldTextLinesMatch(List<string> v);

        public List<List<string>> GetTextLinesAll(string fieldName)
        {
            List<List<string>> vs = new List<List<string>>();
            GetTextLines(fieldName, (List<string> v) => { vs.Add(v); return true; });
            return vs;
        }

        public List<string> GetTextLines1(string fieldName)
        {
            List<string> v0 = null;
            GetTextLines(fieldName, (List<string> v) => { v0 = v; return false; });
            return v0;
        }

        public void GetCharBoxes(string fieldName, FieldCharBoxsMatch fieldCharBoxsMatch)
        {
            throw new Exception("TBD");
        }
        public delegate bool FieldCharBoxsMatch(List<CharBox> v);

        public List<List<CharBox>> GetCharBoxsAll(string fieldName)
        {
            List<List<CharBox>> vs = new List<List<CharBox>>();
            GetCharBoxes(fieldName, (List<CharBox> v) => { vs.Add(v); return true; });
            return vs;
        }

        public List<CharBox> GetCharBoxes1(string fieldName)
        {
            List<CharBox> v0 = null;
            GetCharBoxes(fieldName, (List<CharBox> v) => { v0 = v; return false; });
            return v0;
        }

        public void GetImage(string fieldName, FieldImageMatch fieldImageMatch)
        {
            throw new Exception("TBD");
        }
        public delegate bool FieldImageMatch(Bitmap v);

        public List<Bitmap> GetImageAll(string fieldName)
        {
            List<Bitmap> vs = new List<Bitmap>();
            GetImage(fieldName, (Bitmap v) => { vs.Add(v); return true; });
            return vs;
        }

        public Bitmap GetImage1(string fieldName)
        {
            Bitmap v0 = null;
            GetImage(fieldName, (Bitmap v) => { v0 = v; return false; });
            return v0;
        }

        public void GetTextLineImages(string fieldName, FieldTextLineImagesMatch fieldTextLineImagesMatch)
        {
            throw new Exception("TBD");
        }
        public delegate bool FieldTextLineImagesMatch(List<Bitmap> v);

        public List<List<Bitmap>> GetTextLineImagesAll(string fieldName)
        {
            List<List<Bitmap>> vs = new List<List<Bitmap>>();
            GetTextLineImages(fieldName, (List<Bitmap> v) => { vs.Add(v); return true; });
            return vs;
        }

        public List<Bitmap> GetTextLineImages1(string fieldName)
        {
            List<Bitmap> v0 = null;
            GetTextLineImages(fieldName, (List<Bitmap> v) => { v0 = v; return false; });
            return v0;
        }

        #endregion

    }
}