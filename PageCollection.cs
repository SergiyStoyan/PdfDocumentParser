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
using System.IO;
//using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.parser;
using System.Windows.Forms;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// page collection manager of a single pdf file 
    /// </summary>
    public class PageCollection : HandyDictionary<int, Page>
    {
        public PageCollection(string pdfFile) : base(null)
        {
            PdfFile = pdfFile;
            PdfReader = new PdfReader(pdfFile);
            getValue = (int pageI) => { return new Page(this, pageI); };
        }

        public readonly string PdfFile;
        public readonly PdfReader PdfReader;

        public override void Dispose()
        {
            base.Dispose();
            PdfReader.Dispose();
        }

        public Template ActiveTemplate
        {
            set
            {
                foreach (Page p in Values)
                    //p.ActiveTemplate = value;
                    p.OnActiveTemplateUpdating(value);
                _ActiveTemplate = value;
            }
            get
            {
                return _ActiveTemplate;
            }
        }
        Template _ActiveTemplate;

        //public delegate void OnActiveTemplateUpdating(Settings.Template newTemplate);
        //public event OnActiveTemplateUpdating ActiveTemplateUpdating = null;
    }
}