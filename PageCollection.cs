//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
//using iTextSharp.text;
using iTextSharp.text.pdf;

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
            //PdfReader.unethicalreading = true;
            PdfReader = new PdfReader(pdfFile);
            TotalCount = PdfReader.NumberOfPages;
            getValue = (int pageI) => { return new Page(this, pageI); };
        }

        public readonly string PdfFile;
        public readonly PdfReader PdfReader;
        public readonly int TotalCount;

        public override void Dispose()
        {
            lock (this)
            {
                if (PdfReader != null)
                    PdfReader.Dispose();
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