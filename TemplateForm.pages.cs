//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//using ZedGraph;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// template editor GUI
    /// </summary>
    public partial class TemplateForm : Form
    {
        PageCollection pages = null;

        internal async Task ReloadPageActiveTemplateBitmapAsync()
        {
            if (pages == null)
                return;
            //pages.Clear();
            pages.ActiveTemplate = GetTemplateFromUI(false);
            highlightScanSettings(pages.ActiveTemplate);
            await showPageAsync(currentPageI);
        }

        async Task setScaledImageAsync()
        {
            if (pages == null)
                return;

            picture.Cursor = Cursors.WaitCursor;

            if (pages.ActiveTemplate == null)
                pages.ActiveTemplate = GetTemplateFromUI(false);
            scaledCurrentPageBitmap?.Dispose();

            await Task.Run(() =>
            {
                var b = pages[currentPageI].ActiveTemplateBitmap;
                scaledCurrentPageBitmap = Win.ImageRoutines.GetScaled(b, (float)pictureScale.Value * Settings.Constants.Pdf2ImageResolutionRatio);
            });
            picture.Image?.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);

            picture.Cursor = Cursors.Default;
        }
        Bitmap scaledCurrentPageBitmap;

        bool findAndDrawAnchor(int anchorId)
        {
            if (pages == null)
                return false;

            pages.ActiveTemplate = GetTemplateFromUI(false);
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
                    setRowStatus(Statuses.WARNING, r_, "Not set");
                }
            }
            if (!set)
                return false;
            getAnchor(a.Id, out DataGridViewRow r);
            Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(a.Id);
            if (!aai.Found)
            {
                setRowStatus(Statuses.ERROR, r, "Not found");
                return false;
            }
            setRowStatus(Statuses.SUCCESS, r, "Found");

            for (Page.AnchorActualInfo aai_ = aai; aai_ != null; aai_ = aai_.ParentAnchorActualInfo)
            {
                RectangleF r_ = aai_.Anchor.Rectangle();
                r_.X += aai_.Shift.Width;
                r_.Y += aai_.Shift.Height;
                if (aai_ == aai)
                    if (currentAnchorControl != null)
                    {
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r_ });
                        owners2resizebleBox[aai_.Anchor] = new ResizebleBox(aai_.Anchor, r_, Settings.Appearance.SelectionBoxBorderWidth);
                    }
                    else
                        drawBoxes(Settings.Appearance.AnchorBoxColor, Settings.Appearance.AnchorBoxBorderWidth, new List<RectangleF> { r_ });
                else
                    drawBoxes(Settings.Appearance.AscendantAnchorBoxColor, Settings.Appearance.AscendantAnchorBoxBorderWidth, new List<RectangleF> { r_ });

                List<RectangleF> bs = null;
                switch (aai_.Anchor.Type)
                {
                    case Template.Anchor.Types.PdfText:
                        {
                            var pt = (Template.Anchor.PdfText)aai_.Anchor;
                            bs = pt.CharBoxs.Select(x => new RectangleF(x.Rectangle.X + aai_.Shift.Width, x.Rectangle.Y + aai_.Shift.Height, x.Rectangle.Width, x.Rectangle.Height)).ToList();
                        }
                        break;
                    case Template.Anchor.Types.OcrText:
                        {
                            var ot = (Template.Anchor.OcrText)aai_.Anchor;
                            bs = ot.CharBoxs.Select(x => new RectangleF(x.Rectangle.X + aai_.Shift.Width, x.Rectangle.Y + aai_.Shift.Height, x.Rectangle.Width, x.Rectangle.Height)).ToList();
                        }
                        break;
                    case Template.Anchor.Types.ImageData:
                        //bs = new List<System.Drawing.RectangleF> { rs[0] };
                        break;
                    case Template.Anchor.Types.CvImage:
                        //bs = new List<System.Drawing.RectangleF> { rs[0] };
                        break;
                    default:
                        throw new Exception("Unknown option: " + aai_.Anchor.Type);
                }
                if (bs != null)
                    if (aai_.Anchor == a)
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

                pages.ActiveTemplate = GetTemplateFromUI(false);

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
                object v = fai.GetValue(field.Type);
                switch (field.Type)
                {
                    case Template.Field.Types.PdfText:
                    case Template.Field.Types.PdfTextLines:
                    case Template.Field.Types.PdfCharBoxs:
                        if (field.ColumnOfTable != null)
                        {
                            if (!fai.TableFieldActualInfo.Found)
                                return null;
                            drawBoxes(Settings.Appearance.TableBoxColor, Settings.Appearance.TableBoxBorderWidth, new List<RectangleF> { (RectangleF)fai.TableFieldActualInfo.ActualRectangle });
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                RectangleF tableAR = (RectangleF)fai.TableFieldActualInfo.ActualRectangle;
                                List<Page.Line<Pdf.CharBox>> lines = Page.GetLines(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, tableAR), null, null).ToList();
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
                                List<Page.Line<Pdf.CharBox>> lines = Page.GetLines(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, r), null, null).ToList();
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                for (int i = 1; i < lines.Count; i++)
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = lines[i].Top, Width = r.Width, Height = 0 });
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    case Template.Field.Types.OcrText2:
                    case Template.Field.Types.OcrTextLines2:
                        Template.Field.Ocr2 of2 = field as Template.Field.Ocr2;
                        if (field.ColumnOfTable != null)
                        {
                            throw new Exception("Not implemented.");
                        }
                        else
                        {
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                var or = Ocr.This.GetOCRResultSurroundedByRectangle2(pages[currentPageI].ActiveTemplateBitmap, r, of2.CropPadding ?? pages.ActiveTemplate.CropPadding);
                                if (or != null)
                                {
                                    //List<Page.Line<Ocr.CharBox>> ols = Page.GetLines(cbs, null, field.CharFilter ?? pages.ActiveTemplate.CharFilter);
                                    //if (of.AdjustLineBorders ?? pages.ActiveTemplate.AdjustLineBorders)
                                    //    Page.AdjustBorders(ols, r);
                                    //else
                                    //    Page.PadLines(ols, field.LinePaddingY ?? pages.ActiveTemplate.LinePaddingY);
                                    //if (ols.Count > 0)
                                    //    ols.RemoveAt(0);
                                    //List<RectangleF> lineBoxes = new List<RectangleF>();
                                    //foreach (Page.Line<Ocr.CharBox> l in ols)
                                    //    lineBoxes.Add(new RectangleF { X = r.X, Y = l.Top, Width = r.Width, Height = 0 });
                                    var rs = or.TextBlocks.Select(a => new RectangleF(a.BoxPoints[0].X, a.BoxPoints[1].Y, a.BoxPoints[2].X - a.BoxPoints[0].X, a.BoxPoints[3].Y - a.BoxPoints[1].Y)).ToList();
                                    drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, rs);
                                }
                            }
                        }
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    case Template.Field.Types.OcrText:
                    case Template.Field.Types.OcrTextLines:
                    case Template.Field.Types.OcrCharBoxs:
                        Template.Field.Ocr of = field as Template.Field.Ocr;
                        if (field.ColumnOfTable != null)
                        {
                            if (!fai.TableFieldActualInfo.Found)
                                return null;
                            drawBoxes(Settings.Appearance.TableBoxColor, Settings.Appearance.TableBoxBorderWidth, new List<RectangleF> { (RectangleF)fai.TableFieldActualInfo.ActualRectangle });
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                List<Page.Line<Ocr.CharBox>> ols = Page.GetLines((List<Ocr.CharBox>)fai.TableFieldActualInfo.GetValue(Template.Field.Types.OcrCharBoxs), null, field.CharFilter ?? pages.ActiveTemplate.CharFilter);
                                if (of.AdjustLineBorders ?? pages.ActiveTemplate.AdjustLineBorders)
                                    Page.AdjustBorders(ols, fai.TableFieldActualInfo.ActualRectangle.Value);
                                else
                                    Page.PadLines(ols, field.LinePaddingY ?? pages.ActiveTemplate.LinePaddingY);
                                if (ols.Count > 0)
                                    ols.RemoveAt(0);
                                List<RectangleF> lineBoxes = new List<RectangleF>();
                                foreach (Page.Line<Ocr.CharBox> l in ols)
                                    lineBoxes.Add(new RectangleF { X = r.X, Y = l.Top, Width = r.Width, Height = 0 });
                                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                            }
                        }
                        else
                        {
                            if (ShowFieldTextLineSeparators.Checked)
                            {
                                List<Ocr.CharBox> cbs;
                                if (of.SingleFieldFromFieldImage ?? pages.ActiveTemplate.SingleFieldFromFieldImage)
                                    cbs = Ocr.This.GetCharBoxsSurroundedByRectangle(pages[currentPageI].ActiveTemplateImage, r, of.TesseractPageSegMode ?? pages.ActiveTemplate.TesseractPageSegMode);
                                else
                                    cbs = Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPageI].ActiveTemplateOcrCharBoxs, r);
                                if (cbs != null)
                                {
                                    List<Page.Line<Ocr.CharBox>> ols = Page.GetLines(cbs, null, field.CharFilter ?? pages.ActiveTemplate.CharFilter);
                                    if (of.AdjustLineBorders ?? pages.ActiveTemplate.AdjustLineBorders)
                                        Page.AdjustBorders(ols, r);
                                    else
                                        Page.PadLines(ols, field.LinePaddingY ?? pages.ActiveTemplate.LinePaddingY);
                                    if (ols.Count > 0)
                                        ols.RemoveAt(0);
                                    List<RectangleF> lineBoxes = new List<RectangleF>();
                                    foreach (Page.Line<Ocr.CharBox> l in ols)
                                        lineBoxes.Add(new RectangleF { X = r.X, Y = l.Top, Width = r.Width, Height = 0 });
                                    drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.TextLineSeparatorWidth, lineBoxes);
                                }
                            }
                        }
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    case Template.Field.Types.Image:
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    case Template.Field.Types.OcrTextLineImages:
                        drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<RectangleF> { r });
                        return v;
                    default:
                        throw new Exception("Unknown option: " + field.Type);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                this.Error(ex);
            }
            return null;
        }

        void clearImageFromBoxes()
        {
            picture.Image?.Dispose();
            picture.Image = scaledCurrentPageBitmap != null ? new Bitmap(scaledCurrentPageBitmap) : null;
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
            picture.Image?.Dispose();
            picture.Image = bm;
        }

        async Task showPageAsync(int pageI)
        {
            try
            {
                //UseWaitCursor = true;
                //Cursor.Current = Cursors.WaitCursor;

                currentAnchorControl = null;
                currentFieldControl = null;

                if (pages == null)
                    return;

                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= pageI || totalPageNumber < pageI)
                    return;

                foreach (DataGridViewRow r in fields.Rows)
                    setFieldRowValue(r, true);

                currentPageI = pageI;
                tCurrentPage.Text = currentPageI.ToString();

                await setScaledImageAsync();
                enableNavigationButtons();

                if (pages[pageI].DetectedImageScale < 0)
                {
                    detectedImageScale.BackColor = SystemColors.Window;
                    detectedImageScale.Text = null;
                }
                else if (pages[pageI].DetectedImageScale == 0)
                {
                    detectedImageScale.BackColor = Color.Pink;
                    detectedImageScale.Text = "Not found";
                }
                else
                {
                    detectedImageScale.BackColor = Color.LightGreen;
                    detectedImageScale.Text = pages[pageI].DetectedImageScale.ToString();
                }

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
                    setRowStatus(Statuses.NEUTRAL, row, "");
                loadingTemplate = false;

                if (ExtractFieldsAutomaticallyWhenPageChanged.Checked)
                {
                    HashSet<string> foundFieldNames = new HashSet<string>();
                    foreach (DataGridViewRow row in fields.Rows)
                    {
                        string fn = ((Template.Field)row.Tag).Name;
                        if (foundFieldNames.Contains(fn))
                            continue;
                        if (setFieldRowValue(row, false) != null)
                            foundFieldNames.Add(fn);
                    }
                }
                else
                    foreach (DataGridViewRow row in fields.Rows)
                        setRowStatus(Statuses.NEUTRAL, row, "");

                if (CheckConditionsAutomaticallyWhenPageChanged.Checked)
                    setConditionsStatus();
                else
                    foreach (DataGridViewRow row in conditions.Rows)
                        setRowStatus(Statuses.NEUTRAL, row, "");

                tCurrentPage.BackColor = pages[currentPageI].IsScan ? Color.Beige : SystemColors.Window;
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.Error(e);
            }
            finally
            {
                //UseWaitCursor = false;
                //Cursor.Current = Cursors.Default;
            }
        }
        int currentPageI;
        int totalPageNumber;

        private async void bPrevPage_Click(object sender, EventArgs e)
        {
            await showPageAsync(currentPageI - 1);
        }

        private async void bNextPage_Click(object sender, EventArgs e)
        {
            await showPageAsync(currentPageI + 1);
        }

        void enableNavigationButtons()
        {
            bPrevPage.Enabled = currentPageI > 1;
            bNextPage.Enabled = currentPageI < totalPageNumber;
        }

        private async Task changeCurrentPageAsync()
        {
            if (int.TryParse(tCurrentPage.Text, out int i))
            {
                if (i != currentPageI)
                    await showPageAsync(i);
            }
            else
            {
                this.Error("Page must be a number.");
                tCurrentPage.Text = currentPageI.ToString();
            }
        }

        private async Task tCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                await changeCurrentPageAsync();
        }
    }
}