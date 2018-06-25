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
using System.Windows.Forms;

namespace Cliver.InvoiceParser
{
    class PdfProcessor : IDisposable
    {
        PdfProcessor(string inputPdf)
        {
            pr = Pdf.CreatePdfReader(inputPdf);
            this.inputPdf = inputPdf;
        }
        string inputPdf;
        PdfReader pr;
        PdfStamper ps;

        System.Drawing.Bitmap getPageBitmap(int page_i, Settings.Template.PageRotations pagesRotation, bool autoDeskew, Settings.Template.RectangleF r = null)
        {
            System.Drawing.Bitmap b;
            if (lastPageI == page_i && lastPagesRotation == pagesRotation && lastAutoDeskew == autoDeskew)
                b = lastBitmap;
            else
            {
                if (lastBitmap != null)
                {
                    lastBitmap.Dispose();
                    lastBitmap = null;
                }

                b = BitmapCollection.GetPageBitmap(inputPdf, page_i, pagesRotation, autoDeskew);
                lastPageI = page_i;
                lastPagesRotation = pagesRotation;
                lastAutoDeskew = autoDeskew;
                lastBitmap = b;
            }
            if (r != null)
                b = b.Clone(r.GetSystemRectangle(), System.Drawing.Imaging.PixelFormat.Undefined);
            return b;
        }
        int lastPageI = -1;
        System.Drawing.Bitmap lastBitmap;
        Settings.Template.PageRotations lastPagesRotation;
        bool lastAutoDeskew;



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
                if (pr != null)
                {
                    pr.Close();
                    pr = null;
                }
                if (lastBitmap != null)
                {
                    lastBitmap.Dispose();
                    lastBitmap = null;
                }
            }
        }

        bool isInvoiceFirstPage(int page_i, Settings.Template template)
        {
            foreach (Settings.Template.Mark m in template.InvoiceFirstPageRecognitionMarks)
            {
                switch (m.ValueType)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        {
                            string t = pr.ExtractText(page_i, m.Rectangle.X, pageHeight - m.Rectangle.Y - m.Rectangle.Height, m.Rectangle.Width, m.Rectangle.Height);
                            t = Pdf.GetTextByTopLeftCoordinates(pageCharBoxListss[page_i], m.Rectangle.X, m.Rectangle.Y, m.Rectangle.Width, m.Rectangle.Height);
                            return FieldPreparation.Normalize(m.Value) == FieldPreparation.Normalize(t);
                        }
                    case Settings.Template.ValueTypes.OcrText:
                        {
                            string t = TesseractW.This.GetText(getPageBitmap(page_i, template.PagesRotation, template.AutoDeskew), m.Rectangle.X / Settings.General.Image2PdfResolutionRatio, m.Rectangle.Y / Settings.General.Image2PdfResolutionRatio, m.Rectangle.Width / Settings.General.Image2PdfResolutionRatio, m.Rectangle.Height / Settings.General.Image2PdfResolutionRatio);
                            return FieldPreparation.Normalize(m.Value) == FieldPreparation.Normalize(t);
                        }
                    case Settings.Template.ValueTypes.ImageData:
                        {
                            using (System.Drawing.Bitmap b = getPageBitmap(page_i, template.PagesRotation, template.AutoDeskew, m.Rectangle))
                            {
                                ImageData id = ImageData.GetFromString(m.Value);
                                return id.ImageIsSimilar(new ImageData(b));
                            }
                        }
                    default:
                        throw new Exception("Unknown option: " + m.ValueType);
                }
            }
            return false;
        }

        string getFieldText(int page_i, Settings.Template.Field field, Settings.Template template)
        {
            //foreach (PageRecognitionTextMarkFilter f in fieldNames2pageRecognitionTextMarksFilters[name])
            //    if (FieldPreparation.Normalize(pr.ExtractText(page_i, f.Filter)) != f.Text)
            //        return null;    
            string t = null;
            float x = field.Rectangle.X, y = field.Rectangle.Y;
            if (field.FloatingAnchor != null)
            {
                List<BoxText> bts = FindFloatingAnchor(pageCharBoxListss[page_i], field.FloatingAnchor);
                if (bts == null || bts.Count < 1)
                    return null;
                x += bts[0].R.X;
                y += bts[0].R.Y;
            }
            if (!field.Ocr)
                //t = pr.ExtractText(page_i, pr.CreateFilters(x, pageHeight - y - f.Rectangle.Height, f.Rectangle.Width, f.Rectangle.Height));
                t = Pdf.GetTextByTopLeftCoordinates(pageCharBoxListss[page_i], x, y, field.Rectangle.Width, field.Rectangle.Height);
            else
                t = TesseractW.This.GetText(getPageBitmap(page_i, template.PagesRotation, template.AutoDeskew), x / Settings.General.Image2PdfResolutionRatio, y / Settings.General.Image2PdfResolutionRatio, field.Rectangle.Width / Settings.General.Image2PdfResolutionRatio, field.Rectangle.Height / Settings.General.Image2PdfResolutionRatio);
            return prepareField(FieldPreparation.Normalize(t));
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

        static public bool? Process(string inputPdf, List<Settings.Template> templates, string stampedPdf, Action<string, int, Dictionary<string, string>> record)
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
                if (cp.pr.NumberOfPages < 1)
                {
                    Log.Main.Warning("File path '" + inputPdf + "' has no page.");
                    return false;
                }

                cp.pageHeight = cp.pr.GetPageSize(1).Height;//it is assumed that all pages have the same Height

                cp.pageCharBoxListss = new CharBoxCollection(delegate (int page_i)
                {
                    var bts = cp.pr.GetCharacterTextChunks(page_i).Select(x => new BoxText
                    {
                        R = new System.Drawing.RectangleF
                        {
                            X = x.StartLocation[Vector.I1],
                            Y = cp.pr.GetPageSize(page_i).Height - x.EndLocation[Vector.I2],
                            Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
                            Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
                        },
                        Text = x.Text
                    });
                    return bts.ToList();
                });

                foreach (Settings.Template t in ts)
                {
                    for (int page_i = 1; page_i <= cp.pr.NumberOfPages; page_i++)
                    {
                        if (cp.isInvoiceFirstPage(page_i, t))
                        {
                            Log.Main.Inform("Applying to file '" + inputPdf + "' template '" + t.Name + "'\r\nStamped file: '" + stampedPdf);
                            //cp.pageBitmaps.RememberConverted = true;
                            cp.processWithTemplate(t, page_i, stampedPdf, record);
                            return true;
                        }
                    }
                }
            }
            Log.Main.Warning("No template found for file '" + inputPdf + "'");
            return false;
        }
        float pageHeight;//it is assumed that all pages have the same Height

        void processWithTemplate(Settings.Template template, int invoice_first_page_i, string stampedPdf, Action<string, int, Dictionary<string, string>> record)
        {
            ps = new PdfStamper(pr, new FileStream(stampedPdf, FileMode.Create, FileAccess.Write, FileShare.None));

            foreach (Settings.Template.Field f in template.Fields)
                fieldNames2texts[f.Name] = getFieldText(invoice_first_page_i, f, template);
            for (int page_i = invoice_first_page_i + 1; page_i <= pr.NumberOfPages; page_i++)
            {
                if (isInvoiceFirstPage(page_i, template))
                {
                    record(template.Name, invoice_first_page_i, fieldNames2texts);
                    stampInvoicePages(invoice_first_page_i, page_i - 1);
                    fieldNames2texts.Clear();
                    invoice_first_page_i = page_i;
                    foreach (Settings.Template.Field f in template.Fields)//fields are read only from the first page!
                        fieldNames2texts[f.Name] = getFieldText(page_i, f, template);
                }
                Settings.Template.Field costF = template.Fields.Where(a => a.Name == "COST").FirstOrDefault();
                if (costF != null)
                {
                    string c = getFieldText(page_i, costF, template);
                    if (c != null)
                        fieldNames2texts["COST"] = c;
                }
            }
            record(template.Name, invoice_first_page_i, fieldNames2texts);
            stampInvoicePages(invoice_first_page_i, pr.NumberOfPages);
        }

        static string prepareField(string f)
        {
            return Regex.Replace(f, @"\-", "");
        }

        public static List<BoxText> FindFloatingAnchor(List<BoxText> boxTexts, Settings.Template.FloatingAnchor fa)
        {
            List<BoxText> bts = new List<BoxText>();
            foreach (BoxText bt0 in boxTexts.Where(a => a.Text == fa.Elements[0].Text))
            {
                bts.Clear();
                bts.Add(bt0);
                for (int i = 1; i < fa.Elements.Count; i++)
                {
                    float x = bt0.R.X + fa.Elements[i].Rectangle.X - fa.Elements[0].Rectangle.X;
                    float y = bt0.R.Y + fa.Elements[i].Rectangle.Y - fa.Elements[0].Rectangle.Y;
                    foreach (BoxText bt in boxTexts.Where(a => a.Text == fa.Elements[i].Text))
                    {
                        if (Math.Abs(bt.R.X - x) > Settings.General.CoordinateDeviationMargin)
                            continue;
                        if (Math.Abs(bt.R.Y - y) > Settings.General.CoordinateDeviationMargin)
                            continue;
                        if (bts.Contains(bt))
                            continue;
                        bts.Add(bt);
                    }
                }
                if (bts.Count == fa.Elements.Count)
                    return bts;
            }
            return null;
        }
        CharBoxCollection pageCharBoxListss;
    }
}