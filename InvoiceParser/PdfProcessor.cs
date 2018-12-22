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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.parser;
using Cliver.PdfDocumentParser;

namespace Cliver.InvoiceParser
{
    class PdfProcessor : IDisposable
    {
        PdfProcessor(string inputPdf)
        {
            Pages = new PageCollection(inputPdf);
        }
        PageCollection Pages;
        PdfStamper pdfStamper;

        ~PdfProcessor()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (pdfStamper != null)
                {
                    pdfStamper.Close();
                    pdfStamper = null;
                }
                if (Pages != null)
                {
                    Pages.Dispose();
                    Pages = null;
                }
            }
        }

        void stampInvoicePages(int page_i1, int page_i2)
        {
            for (int i = page_i1; i <= page_i2; i++)
                stampInvoicePage(i);
        }

        const float stampOpacity = 0.7f;
        void stampInvoicePage(int page_i)
        {
            BaseColor color = new BaseColor(Settings.General.StampColor);
            PdfContentByte pcb = pdfStamper.GetOverContent(page_i);
            PdfGState gs = new PdfGState();
            //gs.FillOpacity = Settings.General.StampOpacity;
            gs.StrokeOpacity = stampOpacity;
            pcb.SetGState(gs);
            //pcb.SetColorFill(new BaseColor(Settings.General.StampColor));
            int x = 100;
            int y = 500;
            int w = 450;
            int h = 150;
            Rectangle r = new Rectangle(x, y, x + w, y - h);
            r.Border = Rectangle.BOX;
            r.BorderWidth = 6;
            r.BorderColor = color;
            pcb.Rectangle(r);

            //BaseFont font = BaseFont.CreateFont(); // Helvetica, WinAnsiEncoding
            BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);

            PdfContentByte oc = pdfStamper.GetOverContent(page_i);
            oc.SaveState();
            oc.BeginText();
            gs.FillOpacity = stampOpacity;
            oc.SetGState(gs);
            oc.SetColorFill(color);
            oc.SetFontAndSize(font, 20.0f);
            x += 20;
            y -= 40;
            int d = 120;
            int s = 40;
            oc.SetTextMatrix(x, y);
            oc.ShowText("INVOICE");
            oc.SetTextMatrix(x + d, y);
            oc.ShowText("#: " + getStampValue(Template2.FieldNames.INVOICE));
            y -= s;
            oc.SetTextMatrix(x, y);
            oc.ShowText("JOB");
            oc.SetTextMatrix(x + d, y);
            oc.ShowText("#: " + getStampValue(Template2.FieldNames.JOB));
            y -= s;
            oc.SetTextMatrix(x, y);
            oc.ShowText("PO");
            oc.SetTextMatrix(x + d, y);
            oc.ShowText("#: " + getStampValue(Template2.FieldNames.PO));
            oc.EndText();
            oc.RestoreState();
        }
        string getStampValue(string fieldName)
        {
            if (fieldNames2text.TryGetValue(fieldName, out string v) && v != null)
                return Regex.Replace(v, @"\-", "");
            return "";
        }

        Dictionary<string, string> fieldNames2text = new Dictionary<string, string>();

        static public bool? Process(string inputPdf, List<Template2> template2s, string stampedPdf, Action<string, int, Dictionary<string, string>> record)
        {
            if (File.Exists(stampedPdf))
                File.Delete(stampedPdf);

            Log.Main.Inform(">>> Processing file '" + inputPdf + "'");

            var t2s = template2s.Where(x => x.FileFilterRegex == null || x.FileFilterRegex.IsMatch(inputPdf)).ToList();
            if (t2s.Count < 1)
            {
                Log.Main.Warning("No template matched to the file path.");
                return false;
            }

            using (PdfProcessor cp = new PdfProcessor(inputPdf))
            {
                if (cp.Pages.PdfReader.NumberOfPages < 1)
                {
                    Log.Main.Warning("The file has no page.");
                    return false;
                }
                return cp.process(stampedPdf, record, t2s);
            }
        }
        bool process(string stampedPdf, Action<string, int, Dictionary<string, string>> record, List<Template2> template2s)
        {
            Template2 currentTemplate2 = null;
            int documentFirstPageI = 0;
            int documentCount = 0;

            for (int page_i = 1; page_i <= Pages.PdfReader.NumberOfPages; page_i++)
            {
                List<Template2> possibleTemplate2s;
                if (currentTemplate2 == null)
                    possibleTemplate2s = template2s.Where(x => x.DetectingTemplateLastPageNumber >= page_i).ToList();
                else
                {
                    possibleTemplate2s = new List<Template2> { currentTemplate2 };
                    if (currentTemplate2.SharedFileTemplateNamesRegex != null)
                    {
                        foreach (Template2 t2 in template2s)
                        {
                            if (!currentTemplate2.SharedFileTemplateNamesRegex.IsMatch(t2.Template.Name))
                                continue;
                            if (possibleTemplate2s.Contains(t2))
                                continue;
                            possibleTemplate2s.Add(t2);
                        }
                    }
                }
                foreach (Template2 t2 in possibleTemplate2s)
                {
                    Pages.ActiveTemplate = t2.Template;
                    if (Pages[page_i].IsCondition(Template2.ConditionNames.DocumentFirstPage))
                    {
                        if (documentFirstPageI > 0)
                        {
                            record(currentTemplate2.Template.Name, documentFirstPageI, fieldNames2text);
                            stampInvoicePages(documentFirstPageI, page_i - 1);
                        }
                        fieldNames2text.Clear();
                        if (pdfStamper == null)
                        {
                            Log.Main.Inform("Stamped file: '" + stampedPdf + "'");
                            pdfStamper = new PdfStamper(Pages.PdfReader, new FileStream(stampedPdf, FileMode.Create, FileAccess.Write, FileShare.None));
                        }
                        currentTemplate2 = t2;
                        documentFirstPageI = page_i;
                        Log.Main.Inform("Document #" + (++documentCount) + " detected at page " + documentFirstPageI + " with template '" + currentTemplate2.Template.Name + "'");
                        Settings.TemplateLocalInfo.SetUsedTime(currentTemplate2.Template.Name);
                        break;
                    }
                }
                if (currentTemplate2 == null)
                    continue;
                Pages.ActiveTemplate = currentTemplate2.Template;
                foreach (Template.Field f in Pages.ActiveTemplate.Fields)
                    extractFieldText(Pages[page_i], f);
            }
            if (currentTemplate2 == null)
            {
                Log.Main.Warning("No template found");
                return false;
            }
            record(Pages.ActiveTemplate.Name, documentFirstPageI, fieldNames2text);
            stampInvoicePages(documentFirstPageI, Pages.PdfReader.NumberOfPages);
            return true;
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