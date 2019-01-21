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
            pages.ActiveTemplate = getTemplateFromUI(false);
            showPage(currentPageI);
        }

        void setScaledImage()
        {
            if (pages == null)
                return;
            if (scaledCurrentPageBitmap != null)
                scaledCurrentPageBitmap.Dispose();
            if (pages[currentPageI].ActiveTemplateBitmap == null)
                pages.ActiveTemplate = getTemplateFromUI(false);
            scaledCurrentPageBitmap = ImageRoutines.GetScaled(pages[currentPageI].ActiveTemplateBitmap, (float)pictureScale.Value * Settings.Constants.Image2PdfResolutionRatio);
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);
        }
        Bitmap scaledCurrentPageBitmap;

        PointF? findAndDrawAnchor(int anchorId)
        {
            if (pages == null)
                return null;

            pages.ActiveTemplate = getTemplateFromUI(false);
            Template.Anchor a = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[Id=" + a.Id + "] does not exist.");

            bool set = true;
            for (Template.Anchor a_ = a; a_ != null; a_ = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == a_.ParentAnchorId))
            {
                if (a != a_)
                    showAnchorRowAs(a_.Id, rowStates.Parent, false);
                if (!a_.IsSet())
                {
                    set = false;
                    getAnchor(a_.Id, out DataGridViewRow r_);
                    setRowStatus(statuses.WARNING, r_, "Not set");
                }
            }
            if (!set)
                return null;
            List<List<RectangleF>> rss = pages[currentPageI].GetAnchorRectangless(a.Id);
            getAnchor(a.Id, out DataGridViewRow r);
            if (rss == null || rss.Count < 1)
            {
                setRowStatus(statuses.ERROR, r, "Not found");
                return null;
            }
            setRowStatus(statuses.SUCCESS, r, "Found");

            PointF? p0 = null;
            for (int i = rss.Count - 1; i >= 0; i--)
            {
                List<RectangleF> rs = rss[i];
                drawBoxes(Settings.Appearance.AnchorMasterBoxColor, Settings.Appearance.AnchorMasterBoxBorderWidth, new List<System.Drawing.RectangleF> { rs[0] });
                if (rs.Count > 1)
                    drawBoxes(Settings.Appearance.AnchorSecondaryBoxColor, Settings.Appearance.AnchorSecondaryBoxBorderWidth, rs.GetRange(1, rs.Count - 1));

                if (i == rss.Count - 1)
                    p0 = new PointF(rs[0].X, rs[0].Y);
            }
            return p0;
        }

        object extractFieldAndDrawSelectionBox(Template.Field field)
        {
            try
            {
                if (pages == null)
                    return null;

                if (field.Rectangle == null)
                    return null;

                pages.ActiveTemplate = getTemplateFromUI(false);

                RectangleF r = field.Rectangle.GetSystemRectangleF();
                if (field.LeftAnchor != null)
                {
                    if (findAndDrawAnchor(field.LeftAnchor.Id) == null)
                        return null;
                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(field.LeftAnchor.Id);
                    if (!aai.Found)
                        return null;
                    float right = r.Right;
                    r.X += aai.Shift.Width - field.LeftAnchor.Shift;
                    r.Width = right - r.X;
                }
                if (field.TopAnchor != null)
                {
                    if (findAndDrawAnchor(field.TopAnchor.Id) == null)
                        return null;
                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(field.TopAnchor.Id);
                    if (!aai.Found)
                        return null;
                    float bottom = r.Bottom;
                    r.Y += aai.Shift.Height - field.TopAnchor.Shift;
                    r.Height = bottom - r.Y;
                }
                if (field.RightAnchor != null)
                {
                    if (findAndDrawAnchor(field.RightAnchor.Id) == null)
                        return null;
                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(field.RightAnchor.Id);
                    if (!aai.Found)
                        return null;
                    r.Width += aai.Shift.Width - field.RightAnchor.Shift;
                }
                if (field.BottomAnchor != null)
                {
                    if (findAndDrawAnchor(field.BottomAnchor.Id) == null)
                        return null;
                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(field.BottomAnchor.Id);
                    if (!aai.Found)
                        return null;
                    r.Height += aai.Shift.Height - field.BottomAnchor.Shift;
                }
                if (r.Width <= 0 || r.Height <= 0)
                    return null;
                switch (field.Type)
                {
                    case Template.Field.Types.PdfText:
                        RectangleF? tr = pages[currentPageI].GetTableRectangle((Template.Field.PdfText)field);
                        if (tr != null)
                            drawBoxes(Settings.Appearance.TableBoxColor, Settings.Appearance.TableBoxBorderWidth, new List<RectangleF> { (RectangleF)tr });
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return Page.NormalizeText(Pdf.GetTextSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, r, pages.ActiveTemplate.TextAutoInsertSpaceThreshold, pages.ActiveTemplate.TextAutoInsertSpaceSubstitute));
                    case Template.Field.Types.OcrText:
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return Page.NormalizeText(Ocr.This.GetTextSurroundedByRectangle(pages[currentPageI].ActiveTemplateBitmap, r));
                    case Template.Field.Types.ImageData:
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        using (Bitmap rb = pages[currentPageI].GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                        {
                            return ImageData.GetScaled(rb, Settings.Constants.Image2PdfResolutionRatio);
                        }
                    default:
                        throw new Exception("Unknown option: " + field.Type);
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Error("Rectangle", ex);
                LogMessage.Error(ex);
            }
            return null;
        }

        void clearImageFromBoxes()
        {
            picture.Image?.Dispose();
            if (scaledCurrentPageBitmap != null)
                picture.Image = new Bitmap(scaledCurrentPageBitmap);
            drawnAnchorIds.Clear();
        }
        readonly HashSet<int> drawnAnchorIds = new HashSet<int>();

        void drawBoxes(Color color, float borderWidth, IEnumerable<System.Drawing.RectangleF> rs)
        {
            if (pages == null)
                return;

            Bitmap bm = new Bitmap(picture.Image);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                float factor = (float)pictureScale.Value;
                Pen p = new Pen(color, borderWidth);
                p.Alignment = System.Drawing.Drawing2D.PenAlignment.Outset;
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

        void showPage(int pageI)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= pageI || totalPageNumber < pageI)
                    return;

                foreach (DataGridViewRow r in fields.Rows)
                    setFieldRowValue(r, true);

                currentPageI = pageI;
                tCurrentPage.Text = currentPageI.ToString();

                setScaledImage();
                enableNavigationButtons();

                anchors.CurrentCell = null;//1-st row is autoselected
                conditions.CurrentCell = null;//1-st row is autoselected
                fields.CurrentCell = null;//1-st row is autoselected
                anchors.ClearSelection();//1-st row is autoselected
                conditions.ClearSelection();//1-st row is autoselected
                fields.ClearSelection();//1-st row is autoselected
                //setCurrentAnchorRow(null, true);
                //setCurrentConditionRow(null);
                //setCurrentFieldRow(null);
                loadingTemplate = false;

                if (ExtractFieldsAutomaticallyWhenPageChanged.Checked)
                {
                    HashSet<string> foundFieldNames = new HashSet<string>();
                    foreach (DataGridViewRow row in fields.Rows)
                    {
                        string fn = ((Template.Field)row.Tag).Name;
                        if (foundFieldNames.Contains(fn))
                            continue;
                        if (setFieldRowValue(row, false))
                            foundFieldNames.Add(fn);
                    }
                }

                if (CheckConditionsAutomaticallyWhenPageChanged.Checked)
                    setConditionsStatus();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
        int currentPageI;
        int totalPageNumber;

        private void bPrevPage_Click(object sender, EventArgs e)
        {
            showPage(currentPageI - 1);
        }

        private void bNextPage_Click(object sender, EventArgs e)
        {
            showPage(currentPageI + 1);
        }

        void enableNavigationButtons()
        {
            bPrevPage.Enabled = currentPageI > 1;
            bNextPage.Enabled = currentPageI < totalPageNumber;
        }

        private void changeCurrentPage()
        {
            if (int.TryParse(tCurrentPage.Text, out int i))
            {
                if (i != currentPageI)
                    showPage(i);
            }
            else
            {
                LogMessage.Error("Page is not a number.");
                tCurrentPage.Text = currentPageI.ToString();
            }
        }

        private void tCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                changeCurrentPage();
        }
    }
}