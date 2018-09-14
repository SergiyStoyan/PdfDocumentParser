//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// template editor GUI
    /// </summary>
    public partial class TemplateForm : Form
    {
        PageCollection pages = null;

        void reloadPageBitmaps()
        {
            if (pages == null)
                return;
            pages.Clear();
            showPage(currentPage);
        }

        void setScaledImage()
        {
            if (pages == null)
                return;
            if (scaledCurrentPageBitmap != null)
                scaledCurrentPageBitmap.Dispose();
            scaledCurrentPageBitmap = ImageRoutines.GetScaled(pages[currentPage].ActiveTemplateBitmap, (float)pictureScale.Value * Settings.ImageProcessing.Image2PdfResolutionRatio);
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);
        }
        Bitmap scaledCurrentPageBitmap;

        PointF? findAndDrawFloatingAnchor(int? floatingAnchorId, bool renewImage = true)
        {
            if (floatingAnchorId == null)
                return null;

            pages.ActiveTemplate = getTemplateFromUI(false);

            Template.FloatingAnchor fa = pages.ActiveTemplate.FloatingAnchors.Where(a => a.Id == (int)floatingAnchorId).FirstOrDefault();
            if (fa == null || fa.Value == null)
            {
                setStatus(statuses.WARNING, "FloatingAnchor[" + fa.Id + "] is not defined.");
                clearPicture(renewImage);
                return null;
            }

            List<RectangleF> rs = pages[currentPage].GetFloatingAnchorRectangles(fa);
            if (rs == null || rs.Count < 1)
            {
                setStatus(statuses.ERROR, "FloatingAnchor[" + fa.Id + "] is not found.");
                clearPicture(renewImage);
                return null;
            }
            setStatus(statuses.SUCCESS, "FloatingAnchor[" + fa.Id + "] is found.");

            drawBoxes(Settings.Appearance.FloatingAnchorMasterBoxColor, new List<System.Drawing.RectangleF> { rs[0] }, renewImage);
            if (rs.Count > 1)
                drawBoxes(Settings.Appearance.FloatingAnchorSecondaryBoxColor, rs.GetRange(1, rs.Count - 1), false);
            return new PointF(rs[0].X, rs[0].Y);
        }

        object extractValueAndDrawSelectionBox(int? floatingAnchorId, Template.RectangleF r, Template.ValueTypes valueType, bool renewImage = true)
        {
            try
            {
                if (pages == null)
                    return null;

                pages.ActiveTemplate = getTemplateFromUI(false);

                float x = 0;
                float y = 0;
                if (floatingAnchorId != null)
                {
                    PointF? p0_ = findAndDrawFloatingAnchor(floatingAnchorId);
                    if (p0_ == null)
                        return null;
                    PointF p0 = (PointF)p0_;
                    x = p0.X;
                    y = p0.Y;
                    renewImage = false;
                }

                if (r == null)
                    return null;

                x += r.X;
                y += r.Y;
                RectangleF r_ = new RectangleF(x, y, r.Width, r.Height);
                drawBoxes(Settings.Appearance.SelectionBoxColor, new List<System.Drawing.RectangleF> { r_ }, renewImage);

                string error;
                object v = pages[currentPage].GetValue(null, new Template.RectangleF(x, y, r.Width, r.Height), valueType, out error);
                switch (valueType)
                {
                    case Template.ValueTypes.PdfText:
                        return Page.NormalizeText((string)v);
                    case Template.ValueTypes.OcrText:
                        return Page.NormalizeText((string)v);
                    case Template.ValueTypes.ImageData:
                        return v;
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Error("Rectangle", ex);
                LogMessage.Error(ex);
            }
            return null;
        }

        void drawBoxes(Color c, IEnumerable<System.Drawing.RectangleF> rs, bool renewImage)
        {
            if (pages == null)
                return;

            Bitmap bm;
            if (renewImage)
                bm = new Bitmap(scaledCurrentPageBitmap);
            else
                bm = new Bitmap(picture.Image);

            using (Graphics gr = Graphics.FromImage(bm))
            {
                float factor = (float)pictureScale.Value;
                Pen p = new Pen(c);
                foreach (System.Drawing.RectangleF r in rs)
                {
                    System.Drawing.Rectangle r_ = new System.Drawing.Rectangle((int)(r.X * factor), (int)(r.Y * factor), (int)(r.Width * factor), (int)(r.Height * factor));
                    //if (invertColor)
                    //{
                    //    for (int i = r_.X; i <= r_.X + r_.Width; i++)
                    //        for (int j = r_.Y; j <= r_.Y + r_.Height; j++)
                    //        {
                    //            Color rgb = bm.GetPixel(i, j);
                    //            rgb = Color.FromArgb(255 - rgb.R, 255 - rgb.G, 255 - rgb.B);
                    //            bm.SetPixel(i, j, rgb);
                    //        }
                    //}
                    gr.DrawRectangle(p, r_);
                }
            }
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = bm;
        }
        Point selectionBoxPoint0, selectionBoxPoint1, selectionBoxPoint2;
        bool drawingSelectionBox = false;

        void clearPicture(bool renewImage)
        {
            if (pages == null)
                return;

            Bitmap bm;
            if (renewImage)
            {
                bm = new Bitmap(scaledCurrentPageBitmap);
                if (picture.Image != null)
                    picture.Image.Dispose();
                picture.Image = bm;
                return;
            }
        }

        void showPage(int page_i)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= page_i || totalPageNumber < page_i)
                    return;

                floatingAnchors.ClearSelection();
                documentFirstPageRecognitionMarks.ClearSelection();
                fields.ClearSelection();

                foreach (DataGridViewRow r in fields.Rows)
                    r.Cells["Value"].Value = null;

                currentPage = page_i;
                tCurrentPage.Text = currentPage.ToString();

                pages.ActiveTemplate = getTemplateFromUI(false);
                setScaledImage();
                enableNavigationButtons();

                if (ExtractFieldsAutomaticallyWhenPageChanged.Checked)
                {
                    foreach (DataGridViewRow r in fields.Rows)
                    {
                        if (r.IsNewRow)
                            continue;
                        var vt = Convert.ToBoolean(r.Cells["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText;
                        int? fai = (int?)r.Cells["FloatingAnchorId"].Value;
                        string rs = (string)r.Cells["Rectangle"].Value;
                        if (rs != null)
                            r.Cells["Value"].Value = extractValueAndDrawSelectionBox(fai, SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs), vt);
                    }
                }

                checkIfCurrentPageIsDocumentFirstPage();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
        int currentPage;
        int totalPageNumber;

        private void bPrevPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage - 1);
        }

        private void bNextPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage + 1);
        }

        void enableNavigationButtons()
        {
            bPrevPage.Enabled = currentPage > 1;
            bNextPage.Enabled = currentPage < totalPageNumber;
        }

        bool? checkIfCurrentPageIsDocumentFirstPage()
        {
            try
            {
                if (documentFirstPageRecognitionMarks.Rows.Count < 2)
                {
                    status.Text = "No condition of first page of document is specified!";
                    status.BackColor = Color.LightYellow;
                    return null;
                }

                Template t = getTemplateFromUI(false);
                pages.ActiveTemplate = t;
                string error;
                if (!pages[currentPage].IsDocumentFirstPage(out error))
                {
                    status.Text = error;
                    status.BackColor = Color.LightPink;
                    return false;
                }
                status.Text = "The page matches first page of document.";
                status.BackColor = Color.LightGreen;
                return true;
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
            return false;
        }

        private void tCurrentPage_Leave(object sender, EventArgs e)
        {
            changeCurrentPage();
        }

        private void changeCurrentPage()
        {
            int i = 0;
            if (int.TryParse(tCurrentPage.Text, out i))
            {
                if (i != currentPage)
                    showPage(i);
            }
            else
            {
                LogMessage.Error("Page is not a number.");
                tCurrentPage.Text = currentPage.ToString();
            }
        }

        private void tCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                changeCurrentPage();
        }

        private void IsDocumentFirstPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            checkIfCurrentPageIsDocumentFirstPage();
        }
    }
}