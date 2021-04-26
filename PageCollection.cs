//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using iText.Kernel.Pdf;
using System.IO;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// page collection manager of a single pdf file 
    /// </summary>
    public class PageCollection : HandyDictionary<int, Page>
    {
        public PageCollection(string pdfFile) : base()
        {
            PdfFile = pdfFile;
            using (Stream s = new FileStream(pdfFile, FileMode.Open))//it is because of the bug in iText7: new PdfReader(file) keeps the fie locked if error
            {
                PdfReader = new PdfReader(s);
            }
            PdfDocument = new PdfDocument(PdfReader);
            TotalCount = PdfDocument.GetNumberOfPages();
            getValue = (int pageI) => { return new Page(this, pageI); };
        }

        public readonly string PdfFile;
        public readonly PdfReader PdfReader;
        public readonly PdfDocument PdfDocument;
        public readonly int TotalCount;

        public override void Dispose()
        {
            lock (this)
            {
                if (PdfDocument != null)
                    PdfDocument.Close();
                if (PdfReader != null)
                    PdfReader.Close();
                base.Dispose();
            }
        }

        public Template ActiveTemplate
        {
            set
            {
                if (value == _ActiveTemplate)
                    return;
                foreach (Page p in Values)
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