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

namespace Cliver.InvoiceParser
{
    public class PageCollection : HandyDictionary<int, Page>
    {
        public PageCollection(string pdfFile) : base(null)
        {
            PdfFile = pdfFile;
            PdfReader = new PdfReader(pdfFile);
            getObject = (int pageI) => { return new Page(this, pageI); };
        }

        public readonly string PdfFile;
        public readonly PdfReader PdfReader;

        public Settings.Template ActiveTemplate
        {
            set
            {
                _ActiveTemplate = value;
                foreach (Page p in Values)
                    p.ActiveTemplate = value;
            }
            get
            {
                return _ActiveTemplate;
            }
        }
        Settings.Template _ActiveTemplate;
    }
}