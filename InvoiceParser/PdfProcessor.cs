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
        PdfStamper ps;

        ~PdfProcessor()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (ps != null)
                {
                    ps.Close();
                    ps = null;
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
            PdfContentByte pcb = ps.GetOverContent(page_i);
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

            PdfContentByte oc = ps.GetOverContent(page_i);
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
            string v;
            if (fieldNames2texts.TryGetValue("INVOICE#", out v))
                oc.ShowText("#: " + v);
            y -= s;
            oc.SetTextMatrix(x, y);
            oc.ShowText("JOB");
            oc.SetTextMatrix(x + d, y);
            if (fieldNames2texts.TryGetValue("JOB#", out v))
                oc.ShowText("#: " + v);
            y -= s;
            oc.SetTextMatrix(x, y);
            oc.ShowText("PO");
            oc.SetTextMatrix(x + d, y);
            if (fieldNames2texts.TryGetValue("PO#", out v))
                oc.ShowText("#: " + v);
            oc.EndText();
            oc.RestoreState();
        }
        Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();

        static public bool? Process(string inputPdf, List<Template> templates, string stampedPdf, Action<string, int, Dictionary<string, string>> record)
        {
            if (File.Exists(stampedPdf))
                File.Delete(stampedPdf);

            var ts = templates.Where(x => x.FileFilterRegex.IsMatch(inputPdf)).ToList();
            if (ts.Count < 1)
            {
                Log.Main.Warning("No template matched to file path '" + inputPdf + "'");
                return false;
            }

            using (PdfProcessor cp = new PdfProcessor(inputPdf))
            {
                if (cp.Pages.PdfReader.NumberOfPages < 1)
                {
                    Log.Main.Warning("File path '" + inputPdf + "' has no page.");
                    return false;
                }

                foreach (Template t in ts)
                {
                    cp.Pages.ActiveTemplate = t;
                    for (int page_i = 1; page_i <= cp.Pages.PdfReader.NumberOfPages; page_i++)
                    {
                        if (cp.Pages[page_i].IsDocumentFirstPage())
                        {
                            Log.Main.Inform("Applying to file '" + inputPdf + "' template '" + t.Name + "'\r\nStamped file: '" + stampedPdf);
                            //cp.pageBitmaps.RememberConverted = true;
                            cp.process(page_i, stampedPdf, record);
                            return true;
                        }
                    }
                }
            }
            Log.Main.Warning("No template found for file '" + inputPdf + "'");
            return false;
        }
        void process(int documentFirstPageI, string stampedPdf, Action<string, int, Dictionary<string, string>> record)
        {
            ps = new PdfStamper(Pages.PdfReader, new FileStream(stampedPdf, FileMode.Create, FileAccess.Write, FileShare.None));

            foreach (Template.Field f in Pages.ActiveTemplate.Fields)
                fieldNames2texts[f.Name] = GetFieldText(Pages[documentFirstPageI], f);
            for (int page_i = documentFirstPageI + 1; page_i <= Pages.PdfReader.NumberOfPages; page_i++)
            {
                if (Pages[page_i].IsDocumentFirstPage())
                {
                    record(Pages.ActiveTemplate.Name, documentFirstPageI, fieldNames2texts);
                    stampInvoicePages(documentFirstPageI, page_i - 1);
                    fieldNames2texts.Clear();
                    documentFirstPageI = page_i;
                }
                foreach (Template.Field f in Pages.ActiveTemplate.Fields)
                {
                    string t = GetFieldText(Pages[page_i], f);
                    if (t != null)
                        fieldNames2texts[f.Name] = t;
                }
            }
            record(Pages.ActiveTemplate.Name, documentFirstPageI, fieldNames2texts);
            stampInvoicePages(documentFirstPageI, Pages.PdfReader.NumberOfPages);
        }

        public string GetFieldText(Page p, Template.Field field)
        {
            if (field.Rectangle == null)
                return null;
            string error;
            object v = p.GetValue(field.FloatingAnchorId, field.Rectangle, field.ValueType, out error);
            if (v is ImageData)
                return SerializationRoutines.Json.Serialize(v, false);
            if (v == null)
                return null;
            return Page.NormalizeText(Regex.Replace((string)v, @"\-", ""));
        }
    }
}