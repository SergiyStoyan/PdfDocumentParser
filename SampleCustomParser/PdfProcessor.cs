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

        Dictionary<string, string> fieldNames2text = new Dictionary<string, string>();

        static public bool? Process(string inputPdf, List<Template2> template2s, Action<string, int, int, Dictionary<string, string>> record)
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
        bool process(Action<string, int, int, Dictionary<string, string>> record, List<Template2> template2s)
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
                            break;
                        }
                    }
                if (currentTemplate == null)
                {
                    Log.Main.Warning2("No template found for page #" + pageI);
                    continue;
                }
                foreach (Template.Field f in currentTemplate.Fields)
                    extractFieldText(Pages[pageI], f);
                if (Pages[pageI].IsCondition(Template2.ConditionNames.DocumentLastPage))
                {
                    record(currentTemplate.Name, documentFirstPageI, pageI, fieldNames2text);
                    fieldNames2text.Clear();
                    currentTemplate = null;
                    continue;
                }
            }
            return documentCount > 0;
        }

        void extractFieldText(Page p, Template.Field field)
        {
            if (field.Rectangle == null)
                return;
            object v = p.GetValue(field);
            if (v is ImageData)
            {
                if (!fieldNames2text.ContainsKey(field.Name))
                    fieldNames2text[field.Name] = "--image--";
                return;
            }
            if (v != null)
                fieldNames2text[field.Name] = Page.NormalizeText((string)v);
        }
    }
}