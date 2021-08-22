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
using System.Text.RegularExpressions;
using Cliver.PdfDocumentParser;

namespace Cliver.SampleParser
{
    class PdfProcessor : IDisposable
    {
        PdfProcessor(string inputPdf)
        {
            Pages = new PageCollection(inputPdf);
        }
        PageCollection Pages;

        ~PdfProcessor()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (Pages != null)
                {
                    Pages.Dispose();
                    Pages = null;
                }
            }
        }

        public class Document
        {
            public string Invoice;
            public string Total;
            public List<Product> Products = new List<Product>();
            public class Product
            {
                public string Name;
                public string Cost;
            }
        }

        Document document;

        static public bool? Process(string inputPdf, List<Template2> template2s, Action<string, int, int, Document> record)
        {
            Log.Main.Inform(">>> Processing file '" + inputPdf + "'");

            var t2s = template2s.Where(x => x.FileFilterRegex == null || x.FileFilterRegex.IsMatch(inputPdf)).ToList();
            if (t2s.Count < 1)
            {
                Log.Main.Warning("No template matched to the file path.");
                return false;
            }

            using (PdfProcessor cp = new PdfProcessor(inputPdf))
            {
                if (cp.Pages.TotalCount < 1)
                {
                    Log.Main.Warning("The file has no page.");
                    return false;
                }
                return cp.process(record, t2s);
            }
        }
        bool process(Action<string, int, int, Document> record, List<Template2> template2s)
        {
            int documentFirstPageI = 0;
            int documentCount = 0;
            Template currentTemplate = null;

            for (int pageI = 1; pageI <= Pages.TotalCount; pageI++)
            {
                if (currentTemplate == null)
                    foreach (Template2 t2 in template2s)
                    {
                        Pages.ActiveTemplate = t2.Template;
                        if (Pages[pageI].IsCondition(Template2.ConditionNames.DocumentFirstPage))
                        {
                            currentTemplate = Pages.ActiveTemplate;
                            documentFirstPageI = pageI;
                            Log.Main.Inform("Document #" + (++documentCount) + " detected at page " + documentFirstPageI + " with template '" + currentTemplate.Name + "'");
                            Settings.TemplateLocalInfo.SetUsedTime(currentTemplate.Name);

                            document = new Document();
                            document.Invoice = (string)Pages[pageI].GetValue(Template2.FieldNames.InvoiceId);
                            break;
                        }
                    }
                if (currentTemplate == null)
                {
                    Log.Main.Warning2("No template found for page #" + pageI);
                    continue;
                }

                List<string> names = Pages[pageI].GetTextLines(Template2.FieldNames.ProductNames);
                List<string> costs = Pages[pageI].GetTextLines(Template2.FieldNames.ProductCosts);
                if (names != null)
                    for (int i = 0; i < names.Count; i++)
                        if (!string.IsNullOrWhiteSpace(names[i]))
                            document.Products.Add(new Document.Product { Name = names[i], Cost = costs[i] });

                if (Pages[pageI].IsCondition(Template2.ConditionNames.DocumentLastPage))
                {
                    document.Total = (string)Pages[pageI].GetValue(Template2.FieldNames.TotalAmount);
                    record(currentTemplate.Name, documentFirstPageI, pageI, document);
                    currentTemplate = null;
                    continue;
                }
            }
            return documentCount > 0;
        }

        //void extractFieldText(Page p, Template.Field field)
        //{
        //    if (field.Rectangle == null)
        //        return;
        //    object v = p.GetValue(field.Name);
        //    if (v is ImageData)
        //    {
        //        if (!fieldNames2text.ContainsKey(field.Name))
        //            fieldNames2text[field.Name] = "--image--";
        //        return;
        //    }
        //    if (v != null)
        //        fieldNames2text[field.Name] = Page.NormalizeText((string)v);
        //}
    }
}