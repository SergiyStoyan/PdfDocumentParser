//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
//using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// Page collection manager of a pdf file.
    /// </summary>
    public class PageCollection : HandyDictionary<int, Page>
    {
        public PageCollection(string pdfFile, bool cacheDisposableFieldValues = false) : base()
        {
            PdfFile = pdfFile;
            CacheDisposableFieldValues = cacheDisposableFieldValues;
            PdfReader.unethicalreading = true;
            PdfReader = new PdfReader(pdfFile);
            TotalCount = PdfReader.NumberOfPages;
            getValue = (int pageI) => { return new Page(this, pageI); };
        }

        /// <summary>
        /// (!)If TRUE then the disposables like Bitmaps are cached and (!) automatically disposed when ActiveTemplate is changed. 
        /// To make sure that the disposables are not disposed, set FALSE and dispose them by your own code.
        /// </summary>
        public readonly bool CacheDisposableFieldValues = false;
        public readonly string PdfFile;
        public readonly PdfReader PdfReader;
        public readonly int TotalCount;

        public override void Dispose()
        {
            lock (this)
            {
                PdfReader?.Dispose();
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