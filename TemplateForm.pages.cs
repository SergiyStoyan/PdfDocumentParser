//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

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
            scaledCurrentPageBitmap = Win.ImageRoutines.GetScaled(pages[currentPageI].ActiveTemplateBitmap, (float)pictureScale.Value * Settings.Constants.Image2PdfResolutionRatio);
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);
        }
        Bitmap scaledCurrentPageBitmap;

        bool findAndDrawAnchor(int anchorId)
        {
            if (pages == null)
                return false;

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
                return false;
            getAnchor(a.Id, out DataGridViewRow r);
            if (!pages[currentPageI].GetAnchorActualInfo(a.Id).Found)
            {
                setRowStatus(statuses.ERROR, r, "Not found");
                return false;
            }
            setRowStatus(statuses.SUCCESS, r, "Found");

            for (Template.Anchor a_ = a; a_ != null; a_ = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == a_.ParentAnchorId))
            {
                SizeF shift = pages[currentPageI].GetAnchorActualInfo(a_.Id).Shift;
                RectangleF r_ = a_.Rectangle();
                r_.X += shift.Width;
                r_.Y += shift.Height;
                if (a_ == a)
                    if (currentAnchorControl != null)
                    {
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r_ });
                        owners2resizebleBox[a_] = new ResizebleBox(a_, r_, Settings.Appearance.SelectionBoxBorderWidth);
                    }
                    else
                        drawBoxes(Settings.Appearance.AnchorBoxColor, Settings.Appearance.AnchorBoxBorderWidth, new List<RectangleF> { r_ });
                else
                    drawBoxes(Settings.Appearance.AscendantAnchorBoxColor, Settings.Appearance.AscendantAnchorBoxBorderWidth, new List<RectangleF> { r_ });

                List<RectangleF> bs = null;
                switch (a_.Type)
                {
                    case Template.Anchor.Types.PdfText:
                        {
                            var pt = (Template.Anchor.PdfText)a_;
                            bs = pt.CharBoxs.Select(x => new RectangleF(x.Rectangle.X + shift.Width, x.Rectangle.Y + shift.Height, x.Rectangle.Width, x.Rectangle.Height)).ToList();
                        }
                        break;
                    case Template.Anchor.Types.OcrText:
                        {
                            var ot = (Template.Anchor.OcrText)a_;
                            bs = ot.CharBoxs.Select(x => new RectangleF(x.Rectangle.X + shift.Width, x.Rectangle.Y + shift.Height, x.Rectangle.Width, x.Rectangle.Height)).ToList();
                        }
                        break;
                    case Template.Anchor.Types.ImageData:
                        //bs = new List<System.Drawing.RectangleF> { rs[0] };
                        break;
                    case Template.Anchor.Types.CvImage:
                        //bs = new List<System.Drawing.RectangleF> { rs[0] };
                        break;
                    default:
                        throw new Exception("Unknown option: " + a_.Type);
                }
                if (bs != null)
                    if (a_ == a)
                        drawBoxes(Settings.Appearance.AnchorBoxColor, Settings.Appearance.AnchorBoxBorderWidth, bs);
                    else
                        drawBoxes(Settings.Appearance.AscendantAnchorBoxColor, Settings.Appearance.AscendantAnchorBoxBorderWidth, bs);
            }
            return true;
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

                if (field.LeftAnchor != null && !findAndDrawAnchor(field.LeftAnchor.Id))
                    return null;
                if (field.TopAnchor != null && !findAndDrawAnchor(field.TopAnchor.Id))
                    return null;
                if (field.RightAnchor != null && !findAndDrawAnchor(field.RightAnchor.Id))
                    return null;
                if (field.BottomAnchor != null && !findAndDrawAnchor(field.BottomAnchor.Id))
                    return null;

                Page.FieldActualInfo fai = pages[currentPageI].GetFieldActualInfo(field);
                if (!fai.Found)
                    return null;
                RectangleF r = (RectangleF)fai.ActualRectangle;
                owners2resizebleBox[field] = new ResizebleBox(field, r, Settings.Appearance.SelectionBoxBorderWidth);
                object v = fai.GetValue(field.DefaultValueType);
                switch (field.DefaultValueType)
                {
                    case Template.Field.ValueTypes.PdfText:
                    case Template.Field.ValueTypes.PdfTextLines:
                    case Template.Field.ValueTypes.PdfCharBoxs:
                        if (field.ColumnOfTable != null)
                        {
                            if (!fai.TableFieldActualInfo.Found)
                                return null;
                            drawBoxes(Settings.Appearance.TableBoxColor, Settings.Appearance.TableBoxBorderWidth, new List<RectangleF> { (RectangleF)fai.TableFieldActualInfo.ActualRectangle });
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                RectangleF tableAR = (RectangleF)fai.TableFieldActualInfo.ActualRectangle;
                                List<Pdf.Line> lines = Pdf.GetLines(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, tableAR), pages.ActiveTemplate.TextAutoInsertSpace).ToList();
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                for (int i = 1; i < lines.Count; i++)
                                {
                                    if (lines[i].Bottom < tableAR.Top || lines[i].Top > tableAR.Bottom
                                        || lines[i].Bottom < r.Top || lines[i].Top > r.Bottom
                                        )
                                        continue;
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = lines[i].Top, Width = r.Width, Height = 0 });
                                }
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        else
                        {
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                List<Pdf.Line> lines = Pdf.GetLines(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, r), pages.ActiveTemplate.TextAutoInsertSpace).ToList();
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                for (int i = 1; i < lines.Count; i++)
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = lines[i].Top, Width = r.Width, Height = 0 });
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        if (field.DefaultValueType == Template.Field.ValueTypes.PdfText)
                            return Page.NormalizeText((string)v);
                        if (field.DefaultValueType == Template.Field.ValueTypes.PdfTextLines)
                            return Page.NormalizeText(string.Join("\r\n", (List<string>)v));
                        //if (field.DefaultValueType == Template.Field.ValueTypes.PdfTextCharBoxs)
                        return Page.NormalizeText(Serialization.Json.Serialize(v));
                    case Template.Field.ValueTypes.OcrText:
                    case Template.Field.ValueTypes.OcrTextLines:
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        if (field.ColumnOfTable != null)
                        {
                            if (!fai.TableFieldActualInfo.Found)
                                return null;
                            drawBoxes(Settings.Appearance.TableBoxColor, Settings.Appearance.TableBoxBorderWidth, new List<RectangleF> { (RectangleF)fai.TableFieldActualInfo.ActualRectangle });
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                RectangleF tableAR = (RectangleF)fai.TableFieldActualInfo.ActualRectangle;
                                List<Ocr.Line> lines = Ocr.GetLines(Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPageI].ActiveTemplateOcrCharBoxs, tableAR), pages.ActiveTemplate.TextAutoInsertSpace).ToList();
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                for (int i = 1; i < lines.Count; i++)
                                {
                                    if (lines[i].Bottom < tableAR.Top || lines[i].Top > tableAR.Bottom
                                        || lines[i].Bottom < r.Top || lines[i].Top > r.Bottom
                                        )
                                        continue;
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = lines[i].Top, Width = r.Width, Height = 0 });
                                }
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        else
                        {
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                List<Ocr.Line> lines = Ocr.GetLines(Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPageI].ActiveTemplateOcrCharBoxs, r), pages.ActiveTemplate.TextAutoInsertSpace).ToList();
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                for (int i = 1; i < lines.Count; i++)
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = lines[i].Top, Width = r.Width, Height = 0 });
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        if (field.DefaultValueType == Template.Field.ValueTypes.PdfText)
                            return Page.NormalizeText((string)v);
                        if (field.DefaultValueType == Template.Field.ValueTypes.PdfTextLines)
                            return Page.NormalizeText(string.Join("\r\n", (List<string>)v));
                        //if (field.DefaultValueType == Template.Field.ValueTypes.PdfTextCharBoxs)
                        return Page.NormalizeText(Serialization.Json.Serialize(v));
                        //drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        //if (field.DefaultValueType == Template.Field.ValueTypes.OcrText)
                        //    return Page.NormalizeText((string)v);
                        //if (field.DefaultValueType == Template.Field.ValueTypes.OcrTextLines)
                        //    return Page.NormalizeText(string.Join("\r\n", (List<string>)v));
                        ////if (field.DefaultValueType == Template.Field.ValueTypes.OcrTextCharBoxs)
                        //return Page.NormalizeText(Serialization.Json.Serialize(v));
                    case Template.Field.ValueTypes.Image:
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    default:
                        throw new Exception("Unknown option: " + field.DefaultValueType);
                }
            }
            catch (Exception ex)
            {
                //Win.LogMessage.Error("Rectangle", ex);
                Win.LogMessage.Error(ex);
            }
            return null;
        }

        void clearImageFromBoxes()
        {
            picture.Image?.Dispose();
            if (scaledCurrentPageBitmap != null)
                picture.Image = new Bitmap(scaledCurrentPageBitmap);
            drawnAnchorIds.Clear();
            owners2resizebleBox.Clear();
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
                    if (r_.Height == 0)
                        gr.DrawLine(p, r_.X, r_.Y, r_.X + r_.Width, r_.Y);
                    else if (r_.Width == 0)
                        gr.DrawLine(p, r_.X, r_.Y, r_.X, r_.Y + r_.Height);
                    gr.DrawRectangle(p, r_);
                }
            }
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = bm;
        }

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
                foreach (DataGridViewRow row in anchors.Rows)
                    setRowStatus(statuses.NEUTRAL, row, "");
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
                else
                    foreach (DataGridViewRow row in fields.Rows)
                        setRowStatus(statuses.NEUTRAL, row, "");

                if (CheckConditionsAutomaticallyWhenPageChanged.Checked)
                    setConditionsStatus();
                else
                    foreach (DataGridViewRow row in conditions.Rows)
                        setRowStatus(statuses.NEUTRAL, row, "");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Message.Error(e);
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
                Win.LogMessage.Error("Page is not a number.");
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