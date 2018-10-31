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
            if(pages[currentPage].ActiveTemplateBitmap == null)
                pages.ActiveTemplate = getTemplateFromUI(false);
            scaledCurrentPageBitmap = ImageRoutines.GetScaled(pages[currentPage].ActiveTemplateBitmap, (float)pictureScale.Value * Settings.Constants.Image2PdfResolutionRatio);
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);
        }
        Bitmap scaledCurrentPageBitmap;

        PointF? findAndDrawAnchor(int anchorId, bool renewImage = true)
        {
            DataGridViewRow row;
            Template.Anchor fa = getAnchor(anchorId, out row);
            if (fa == null || row == null)
                throw new Exception("Anchor[Id=" + anchorId + "] does not exist.");

            if (!fa.IsSet())
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                clearPicture(renewImage);
                return null;
            }
            if (pages == null)
                return null;

            pages.ActiveTemplate = getTemplateFromUI(false);

            fa = pages.ActiveTemplate.Anchors.Where(a => a.Id == anchorId).FirstOrDefault();
            if (fa == null)
                throw new Exception("Anchor[Id=" + fa.Id + "] is not defined.");

            List<RectangleF> rs = pages[currentPage].GetAnchorRectangles(fa);
            if (rs == null || rs.Count < 1)
            {
                setRowStatus(statuses.ERROR, row, "Not found");
                clearPicture(renewImage);
                return null;
            }
            setRowStatus(statuses.SUCCESS, row, "Found");

            drawBoxes(Settings.Appearance.AnchorMasterBoxColor, new List<System.Drawing.RectangleF> { rs[0] }, renewImage);
            if (rs.Count > 1)
                drawBoxes(Settings.Appearance.AnchorSecondaryBoxColor, rs.GetRange(1, rs.Count - 1), false);
            return new PointF(rs[0].X, rs[0].Y);
        }

        object extractValueAndDrawSelectionBox(int? anchorId, Template.RectangleF r, Template.Types valueType, bool renewImage = true)
        {
            try
            {
                if (pages == null)
                    return null;

                pages.ActiveTemplate = getTemplateFromUI(false);

                float x = 0;
                float y = 0;
                if (anchorId != null)
                {
                    PointF? p0_ = findAndDrawAnchor((int)anchorId);
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
                    case Template.Types.PdfText:
                        return Page.NormalizeText((string)v);
                    case Template.Types.OcrText:
                        return Page.NormalizeText((string)v);
                    case Template.Types.ImageData:
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

                foreach (DataGridViewRow r in fields.Rows)
                    r.Cells["Value"].Value = null;

                currentPage = page_i;
                tCurrentPage.Text = currentPage.ToString();

                setScaledImage();
                enableNavigationButtons();

                anchors.CurrentCell = null;//1-st row is autoselected
                marks.CurrentCell = null;//1-st row is autoselected
                fields.CurrentCell = null;//1-st row is autoselected
                anchors.ClearSelection();//1-st row is autoselected
                marks.ClearSelection();//1-st row is autoselected
                fields.ClearSelection();//1-st row is autoselected
                //setCurrentAnchorRow(null, true);
                //setCurrentMarkRow(null);
                //setCurrentFieldRow(null);
                loadingTemplate = false;

                if (ExtractFieldsAutomaticallyWhenPageChanged.Checked)
                {
                    foreach (DataGridViewRow row in fields.Rows)
                    {
                        Template.Field f = (Template.Field)row.Tag;
                        if (f == null)
                            continue;
                        if (f.IsSet())
                        {
                            row.Cells["Value"].Value = extractValueAndDrawSelectionBox(f.AnchorId, f.Rectangle, f.Type);
                            if (row.Cells["Value"].Value != null)
                                setRowStatus(statuses.SUCCESS, row, "Found");
                            else
                                setRowStatus(statuses.ERROR, row, "Not found");
                        }
                        else
                            setRowStatus(statuses.WARNING, row, "Not set");
                    }
                }

                checkIfCurrentPageIsDocumentFirstPage();
                setAnchorGroupStatuses();
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
                if (marks.Rows.Count < 2)
                {
                    Message.Warning("No condition of first page of document is specified!");
                    return null;
                }

                bool failed = false;
                drawBoxes(Color.Black, new List<RectangleF> { }, true);
                foreach (DataGridViewRow r in marks.Rows)
                    if (r.Tag != null && !isMarkFound(r, false))
                        failed = true;
                //marks.ClearSelection();
                //Template t = getTemplateFromUI(false);
                //pages.ActiveTemplate = t;
                //string error;
                //if (!pages[currentPage].IsDocumentFirstPage(out error))
                //{
                //    marks.BackgroundColor = Color.LightPink;
                //    return false;
                //}
                if (failed)
                {
                    marks.BackgroundColor = Color.LightPink;
                    return false;
                }
                marks.BackgroundColor = Color.LightGreen;
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