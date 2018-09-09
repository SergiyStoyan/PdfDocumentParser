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
        public abstract class TemplateManager
        {
            public Template Template;
            abstract public Template New();
            abstract public void ReplaceWith(Template newTemplate);
            abstract public void SaveAsInitialTemplate(Template template);
            public string LastTestFile;
            public void HelpRequest()
            {
                try
                {
                    System.Diagnostics.Process.Start(Settings.Constants.HelpFile);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            }
        }

        public TemplateForm(TemplateManager templateManager, string testFileDefaultFolder)
        {
            InitializeComponent();

            Icon = AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": Template Editor";

            this.templateManager = templateManager;
            this.testFileDefaultFolder = testFileDefaultFolder;

            Id3.ValueType = typeof(int);
            PositionDeviation3.ValueType = typeof(float);
            ValueType3.ValueType = typeof(Template.ValueTypes);
            ValueType3.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            ValueType2.ValueType = typeof(Template.ValueTypes);
            ValueType2.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            FloatingAnchorId2.ValueType = typeof(int);
            FloatingAnchorId2.ValueMember = "Id";
            FloatingAnchorId2.DisplayMember = "Name";

            FloatingAnchorId.ValueType = typeof(int);
            FloatingAnchorId.ValueMember = "Id";
            FloatingAnchorId.DisplayMember = "Name";

            int statusDefaultHeight = status.Height;
            status.MouseEnter += delegate
            {
                Size s = TextRenderer.MeasureText(status.Text, status.Font);
                if (s.Width > status.Width || s.Height > status.Height)
                {
                    status.Height = splitContainer2.Panel1.Bottom - status.Top;
                    if (s.Width > status.Width || s.Height > status.Height)
                        status.ScrollBars = ScrollBars.Both;
                }
            };
            status.MouseLeave += delegate
            {
                status.Height = statusDefaultHeight;
                status.ScrollBars = ScrollBars.None;
            };

            Shown += delegate
            {
                Application.DoEvents();//make form be drawn completely
                setUIFromTemplate(templateManager.Template);
            };

            FormClosed += delegate
            {
                if (scaledCurrentPageBitmap != null)
                {
                    scaledCurrentPageBitmap.Dispose();
                    scaledCurrentPageBitmap = null;
                }
                if (pages != null)
                {
                    pages.Dispose();
                    pages = null;
                }

                templateManager.LastTestFile = testFile.Text;
            };

            picture.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;
                drawingSelectionBox = true;
                selectionBoxPoint0 = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));
                selectionBoxPoint1 = new Point(selectionBoxPoint0.X, selectionBoxPoint0.Y);
                selectionBoxPoint2 = new Point(selectionBoxPoint0.X, selectionBoxPoint0.Y);
                selectionCoordinates.Text = selectionBoxPoint1.ToString();
            };

            picture.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;

                Point p = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));

                if (!drawingSelectionBox)
                {
                    selectionCoordinates.Text = p.ToString();
                    return;
                }

                if (selectionBoxPoint0.X < p.X)
                {
                    selectionBoxPoint1.X = selectionBoxPoint0.X;
                    selectionBoxPoint2.X = p.X;
                }
                else
                {
                    selectionBoxPoint1.X = p.X;
                    selectionBoxPoint2.X = selectionBoxPoint0.X;
                }
                if (selectionBoxPoint0.Y < p.Y)
                {
                    selectionBoxPoint1.Y = selectionBoxPoint0.Y;
                    selectionBoxPoint2.Y = p.Y;
                }
                else
                {
                    selectionBoxPoint1.Y = p.Y;
                    selectionBoxPoint2.Y = selectionBoxPoint0.Y;
                }
                selectionCoordinates.Text = selectionBoxPoint1.ToString() + ":" + selectionBoxPoint2.ToString();

                RectangleF r = new RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);
                drawBoxes(Settings.Appearance.SelectionBoxColor, new List<System.Drawing.RectangleF> { r }, true);
            };

            picture.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                try
                {
                    if (pages == null)
                        return;

                    if (!drawingSelectionBox)
                        return;
                    drawingSelectionBox = false;

                    Template.RectangleF r = new Template.RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);

                    switch (mode)
                    {
                        case Modes.SetFloatingAnchor:
                            {
                                if (floatingAnchors.SelectedRows.Count < 1)
                                    break;

                                RectangleF selectedR = new RectangleF(selectionBoxPoint1, new SizeF(selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y));
                                Template.ValueTypes vt = (Template.ValueTypes)floatingAnchors.SelectedRows[0].Cells["ValueType3"].Value;
                                switch (vt)
                                {
                                    case Template.ValueTypes.PdfText:
                                        if (selectedPdfCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedPdfCharBoxs = new List<Pdf.CharBox>();
                                        selectedPdfCharBoxs.AddRange(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPage].PdfCharBoxs, selectedR, true));
                                        break;
                                    case Template.ValueTypes.OcrText:
                                        //{
                                        //    if (selectedOcrTextValue == null)
                                        //        selectedOcrTextValue = new Settings.Template.FloatingAnchor.OcrTextValue
                                        //        {
                                        //            TextBoxs = new List<Settings.Template.FloatingAnchor.OcrTextValue.TextBox>()
                                        //        };
                                        //    string error;
                                        //    pages.ActiveTemplate = getTemplateFromUI(false);
                                        //    selectedOcrTextValue.TextBoxs.Add(new Settings.Template.FloatingAnchor.OcrTextValue.TextBox
                                        //    {
                                        //        Rectangle = Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR),
                                        //        Text = (string)pages[currentPage].GetValue(null, Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR), Settings.Template.ValueTypes.OcrText, out error)
                                        //    });
                                        //}
                                        if (selectedOcrCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedOcrCharBoxs = new List<Ocr.CharBox>();
                                        selectedOcrCharBoxs.AddRange(PdfDocumentParser.Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPage].ActiveTemplateOcrCharBoxs, selectedR));
                                        break;
                                    case Template.ValueTypes.ImageData:
                                        {
                                            if (selectedImageDataValue == null)
                                            {
                                                selectedImageDataValue = new Template.FloatingAnchor.ImageDataValue();
                                                //if (currentFloatingAnchorControl!=null)
                                                {
                                                    FloatingAnchorImageDataControl c = (FloatingAnchorImageDataControl)currentFloatingAnchorControl;
                                                    selectedImageDataValue.FindBestImageMatch = c.FindBestImageMatch;
                                                    selectedImageDataValue.BrightnessTolerance = c.BrightnessTolerance;
                                                    selectedImageDataValue.DifferentPixelNumberTolerance = c.DifferentPixelNumberTolerance;
                                                }
                                            }
                                            string error;
                                            selectedImageDataValue.ImageBoxs.Add(new Template.FloatingAnchor.ImageDataValue.ImageBox
                                            {
                                                Rectangle = Template.RectangleF.GetFromSystemRectangleF(selectedR),
                                                ImageData = (ImageData)pages[currentPage].GetValue(null, Template.RectangleF.GetFromSystemRectangleF(selectedR), Template.ValueTypes.ImageData, out error)
                                            });
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + vt);
                                }

                                if ((ModifierKeys & Keys.Control) != Keys.Control)
                                    setFloatingAnchorFromSelectedElements();
                            }
                            break;
                        case Modes.SetDocumentFirstPageRecognitionTextMarks:
                            {
                                if (documentFirstPageRecognitionMarks.SelectedRows.Count < 1)
                                    break;
                                var cs = documentFirstPageRecognitionMarks.SelectedRows[0].Cells;
                                int? fai = (int?)cs["FloatingAnchorId2"].Value;
                                if (fai != null)
                                {
                                    pages.ActiveTemplate = getTemplateFromUI(false);
                                    PointF? p = pages[currentPage].GetFloatingAnchorPoint0((int)fai);
                                    if (p == null)
                                        throw new Exception("Could not find FloatingAnchor[" + fai + "] in the page");
                                    r.X -= ((PointF)p).X;
                                    r.Y -= ((PointF)p).Y;
                                }
                                cs["Rectangle2"].Value = SerializationRoutines.Json.Serialize(r);
                                documentFirstPageRecognitionMarks.EndEdit();
                            }
                            break;
                        case Modes.SetFieldRectangle:
                            {
                                if (fields.SelectedRows.Count < 1)
                                    break;
                                var cs = fields.SelectedRows[0].Cells;
                                int? fai = (int?)cs["FloatingAnchorId"].Value;
                                if (fai != null)
                                {
                                    pages.ActiveTemplate = getTemplateFromUI(false);
                                    PointF? p = pages[currentPage].GetFloatingAnchorPoint0((int)fai);
                                    if (p == null)
                                        throw new Exception("Could not find FloatingAnchor[" + fai + "] in the page");
                                    r.X -= ((PointF)p).X;
                                    r.Y -= ((PointF)p).Y;
                                }
                                cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
                                fields.EndEdit();
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            floatingAnchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                if (floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value == null)
                    floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value = Template.ValueTypes.PdfText;
            };

            floatingAnchors.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = floatingAnchors.Rows[e.RowIndex];
                Message.Error("FloatingAnchor[" + r.Cells["Id3"].Value + "] has unacceptable value of " + floatingAnchors.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            floatingAnchors.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
                onFloatingAnchorsChanged(null);
            };

            floatingAnchors.CurrentCellDirtyStateChanged += delegate
            {
                if (floatingAnchors.IsCurrentCellDirty)
                    floatingAnchors.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            floatingAnchors.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate)
                    return;
                var row = floatingAnchors.Rows[e.RowIndex];
                int? fai = (int?)row.Cells["Id3"].Value;
                switch (floatingAnchors.Columns[e.ColumnIndex].Name)
                {
                    case "Id3":
                        if (row.Selected)
                            findAndDrawFloatingAnchor(fai);
                        break;
                    case "ValueType3":
                        setFloatingAnchorValue(row, null);
                        setFloatingAnchorControl(row);
                        break;
                }
            };

            floatingAnchors.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    setCurrentFloatingAnchorValueFromControl();

                    setStatus(statuses.NEUTRAL, "");
                    if (floatingAnchors.SelectedRows.Count < 1)
                    {
                        setFloatingAnchorControl(null);
                        return;
                    }
                    documentFirstPageRecognitionMarks.ClearSelection();
                    fields.ClearSelection();
                    var row = floatingAnchors.SelectedRows[0];

                    if (row.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = floatingAnchors.Rows.Add();
                        floatingAnchors.Rows[i].Selected = true;
                        return;
                    }
                    setFloatingAnchorControl(row);
                    findAndDrawFloatingAnchor((int?)row.Cells["Id3"].Value);
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            documentFirstPageRecognitionMarks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                if (documentFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["ValueType2"].Value == null)
                    documentFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["ValueType2"].Value = Template.ValueTypes.PdfText;
            };

            documentFirstPageRecognitionMarks.CurrentCellDirtyStateChanged += delegate
            {
                if (documentFirstPageRecognitionMarks.IsCurrentCellDirty)
                    documentFirstPageRecognitionMarks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            documentFirstPageRecognitionMarks.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
            };

            documentFirstPageRecognitionMarks.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            documentFirstPageRecognitionMarks.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;
                    DataGridViewRow row = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                    var cs = row.Cells;
                    int? fai = (int?)cs["FloatingAnchorId2"].Value;
                    string r_ = (string)cs["Rectangle2"].Value;
                    Template.RectangleF r = null;
                    if (r_ != null)
                        r = SerializationRoutines.Json.Deserialize<Template.RectangleF>(r_);
                    switch (documentFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                    {
                        case "Rectangle2":
                            {
                                if (r == null)
                                {
                                    setStatus(statuses.WARNING, "The selection rectangle is empty.");
                                    setMarkValue(row, null);
                                    return;
                                }
                                Template.ValueTypes vt = (Template.ValueTypes)cs["ValueType2"].Value;
                                object v = extractValueAndDrawSelectionBox(fai, r, vt);
                                switch (vt)
                                {
                                    case Template.ValueTypes.PdfText:
                                        {
                                            Template.Mark.PdfTextValue ptv = (Template.Mark.PdfTextValue)row.Tag;
                                            if (ptv == null)
                                                ptv = new Template.Mark.PdfTextValue();
                                            ptv.Text = (string)v;
                                            setMarkValue(row, ptv);
                                        }
                                        break;
                                    case Template.ValueTypes.OcrText:
                                        {
                                            Template.Mark.OcrTextValue otv = (Template.Mark.OcrTextValue)row.Tag;
                                            if (otv == null)
                                                otv = new Template.Mark.OcrTextValue();
                                            otv.Text = (string)v;
                                            setMarkValue(row, otv);
                                        }
                                        break;
                                    case Template.ValueTypes.ImageData:
                                        {
                                            Template.Mark.ImageDataValue idv = (Template.Mark.ImageDataValue)row.Tag;
                                            if (idv == null)
                                                idv = new Template.Mark.ImageDataValue();
                                            idv.ImageData = (ImageData)v;
                                            setMarkValue(row, idv);
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + vt);
                                }
                            }
                            return;
                        case "ValueType2":
                            {
                                setMarkValue(row, null);
                                setMarkControl(row);
                                Template.ValueTypes vt = (Template.ValueTypes)cs["ValueType2"].Value;
                                object v = extractValueAndDrawSelectionBox(fai, r, vt);
                                switch (vt)
                                {
                                    case Template.ValueTypes.PdfText:
                                        {
                                            Template.Mark.PdfTextValue ptv = (Template.Mark.PdfTextValue)row.Tag;
                                            if (ptv == null)
                                                ptv = new Template.Mark.PdfTextValue();
                                            ptv.Text = (string)v;
                                            setMarkValue(row, ptv); 
                                        }
                                        break;
                                    case Template.ValueTypes.OcrText:
                                        {
                                            Template.Mark.OcrTextValue otv = (Template.Mark.OcrTextValue)row.Tag;
                                            if (otv == null)
                                                otv = new Template.Mark.OcrTextValue();
                                            otv.Text = (string)v;
                                            setMarkValue(row, otv);
                                        }
                                        break;
                                    case Template.ValueTypes.ImageData:
                                        {
                                            Template.Mark.ImageDataValue idv = (Template.Mark.ImageDataValue)row.Tag;
                                            if (idv == null)
                                                idv = new Template.Mark.ImageDataValue();
                                            idv.ImageData = (ImageData)v;
                                            setMarkValue(row, idv);
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + vt);
                                }
                            }
                            return;
                        case "FloatingAnchorId2":
                            setStatus(statuses.WARNING, "documentFirstPageRecognitionMark[" + e.RowIndex + "] may need correction due to anchor change.");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            documentFirstPageRecognitionMarks.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    setCurrentMarkValueFromControl();

                    setStatus(statuses.NEUTRAL, "");
                    if (documentFirstPageRecognitionMarks.SelectedRows.Count < 1)
                    {
                        setMarkControl(null);
                        return;
                    }
                    floatingAnchors.ClearSelection();
                    fields.ClearSelection();
                    int i = documentFirstPageRecognitionMarks.SelectedRows[0].Index;
                    var row = documentFirstPageRecognitionMarks.Rows[i];

                    if (row.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int j = documentFirstPageRecognitionMarks.Rows.Add();
                        documentFirstPageRecognitionMarks.Rows[j].Selected = true;
                        return;
                    }

                    setMarkControl(row);
                    var cs = row.Cells;
                    var vt = (Template.ValueTypes)cs["ValueType2"].Value;
                    int? fai = (int?)cs["FloatingAnchorId2"].Value;
                    string rs = (string)cs["Rectangle2"].Value;
                    if (rs != null)
                    {
                        Template.RectangleF r = rs == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs);
                        switch (vt)
                        {
                            case Template.ValueTypes.PdfText:
                                {
                                    Template.Mark.PdfTextValue ptv1 = (Template.Mark.PdfTextValue)row.Tag;
                                    string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                                    if (ptv1.Text != t2)
                                        setStatus(statuses.ERROR, "documentFirstPageRecognitionMark[" + i + "] is not found:\r\n" + t2 + "\r\n <> \r\n" + ptv1.Text);
                                    else
                                        setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "] is found:\r\n" + t2);
                                }
                                break;
                            case Template.ValueTypes.OcrText:
                                {
                                    Template.Mark.OcrTextValue otv1 = (Template.Mark.OcrTextValue)row.Tag;
                                    string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                                    if (otv1.Text != t2)
                                        setStatus(statuses.ERROR, "documentFirstPageRecognitionMark[" + i + "] is not found:\r\n" + t2 + "\r\n <> \r\n" + otv1.Text);
                                    else
                                        setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "] is found:\r\n" + t2);
                                }
                                break;
                            case Template.ValueTypes.ImageData:
                                {
                                    Template.Mark.ImageDataValue idv1 = (Template.Mark.ImageDataValue)row.Tag;
                                    ImageData id2 = (ImageData)extractValueAndDrawSelectionBox(fai, r, vt);
                                    if (idv1.ImageData.ImageIsSimilar(id2, idv1.BrightnessTolerance, idv1.DifferentPixelNumberTolerance))
                                        setStatus(statuses.ERROR, "documentFirstPageRecognitionMark[" + i + "] is not found:\r\nimage is not similar");
                                    else
                                        setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "] is found:\r\nimage is similar");
                                }
                                break;
                            default:
                                throw new Exception("Unknown option: " + vt);
                        }
                    }
                    else if (fai != null)
                    {
                        if (null != findAndDrawFloatingAnchor(fai))
                            setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "] is found");
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            fields.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    var cs = fields.Rows[e.RowIndex].Cells;
                    string r_ = (string)cs["Rectangle"].Value;
                    if (r_ == null)
                        return;
                    Template.RectangleF r = SerializationRoutines.Json.Deserialize<Template.RectangleF>(r_);
                    switch (fields.Columns[e.ColumnIndex].Name)
                    {
                        case "Rectangle":
                        case "Ocr":
                            cs["Value"].Value = extractValueAndDrawSelectionBox((int?)cs["FloatingAnchorId"].Value, r, Convert.ToBoolean(cs["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText);
                            break;
                        case "FloatingAnchorId":
                            int? fai = (int?)cs["FloatingAnchorId"].Value;
                            if (fai != null)
                            {
                                pages.ActiveTemplate = getTemplateFromUI(false);
                                PointF? p = pages[currentPage].GetFloatingAnchorPoint0((int)fai);
                                if (p == null)
                                    setStatus(statuses.ERROR, "FloatingAnchor[" + fai + "] is not found.");
                                else
                                {
                                    setStatus(statuses.SUCCESS, "FloatingAnchor[" + fai + "] is found.");
                                    r.X -= ((PointF)p).X;
                                    r.Y -= ((PointF)p).Y;
                                    cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
                                }
                            }
                            else//anchor deselected
                            {
                                setStatus(statuses.WARNING, "FloatingAnchor unlinked. The selection rectangle may need fix.");
                                cs["Value"].Value = extractValueAndDrawSelectionBox((int?)cs["FloatingAnchorId"].Value, r, Convert.ToBoolean(cs["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            fields.CurrentCellDirtyStateChanged += delegate
            {
                if (fields.IsCurrentCellDirty)
                    fields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            fields.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                DataGridViewRow r = fields.Rows[e.RowIndex];
                try
                {
                    string n = FieldPreparation.Normalize((string)r.Cells["Name_"].Value);
                    if (!r.IsNewRow)
                    {
                        if (string.IsNullOrWhiteSpace(n))
                            throw new Exception("Name cannot be empty!");
                        foreach (DataGridViewRow rr in fields.Rows)
                        {
                            if (r != rr && n == FieldPreparation.Normalize((string)rr.Cells["Name_"].Value))
                                throw new Exception("Name '" + n + "' is duplicated!");
                        }
                    }
                    r.Cells["Name_"].Value = n;
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Name", ex);
                    Message.Error2(ex);
                    e.Cancel = true;
                }
            };

            fields.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            fields.CellContentClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                switch (fields.Columns[e.ColumnIndex].Name)
                {
                    case "Ocr":
                        fields.EndEdit();
                        break;
                }
            };

            fields.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    setStatus(statuses.NEUTRAL, "");
                    if (fields.SelectedRows.Count > 0)
                    {
                        floatingAnchors.ClearSelection();
                        documentFirstPageRecognitionMarks.ClearSelection();
                        int i = fields.SelectedRows[0].Index;

                        if (fields.Rows[i].IsNewRow)//hacky forcing commit a newly added row and display the blank row
                        {
                            int j = fields.Rows.Add();
                            fields.Rows[j].Selected = true;
                            return;
                        }
                        var cs = fields.Rows[i].Cells;

                        var vt = Convert.ToBoolean(cs["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText;
                        int? fai = (int?)cs["FloatingAnchorId"].Value;
                        string rs = (string)cs["Rectangle"].Value;
                        if (rs != null)
                            cs["Value"].Value = extractValueAndDrawSelectionBox(fai, SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs), vt);
                        else
                        {//to show status
                            if (fai == null || findAndDrawFloatingAnchor(fai) != null)
                                setStatus(statuses.WARNING, "field[" + i + "] has no selecting box defined");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            splitContainer1.Paint += SplitContainer1_Paint;

            testFile.TextChanged += delegate
            {
                try
                {
                    if (picture.Image != null)
                    {
                        picture.Image.Dispose();
                        picture.Image = null;
                    }
                    if (scaledCurrentPageBitmap != null)
                    {
                        scaledCurrentPageBitmap.Dispose();
                        scaledCurrentPageBitmap = null;
                    }
                    if (pages != null)
                    {
                        pages.Dispose();
                        pages = null;
                    }

                    if (string.IsNullOrWhiteSpace(testFile.Text))
                        return;

                    testFile.SelectionStart = testFile.Text.Length;
                    testFile.ScrollToCaret();

                    if (!File.Exists(testFile.Text))
                    {
                        LogMessage.Error("File '" + testFile.Text + "' does not exist!");
                        return;
                    }

                    pages = new PageCollection(testFile.Text);
                    totalPageNumber = pages.PdfReader.NumberOfPages;
                    lTotalPages.Text = " / " + totalPageNumber;
                    showPage(1);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            pictureScale.ValueChanged += delegate
            {
                if (!loadingTemplate)
                    setScaledImage();
            };

            pageRotation.SelectedIndexChanged += delegate
            {
                reloadPageBitmaps();
                //showPage(currentPage);
            };

            autoDeskew.CheckedChanged += delegate
            {
                reloadPageBitmaps();
                //showPage(currentPage);
            };

            Load += delegate
            {
                if (documentFirstPageRecognitionMarks.Rows.Count > 0 && !documentFirstPageRecognitionMarks.Rows[0].IsNewRow)
                    documentFirstPageRecognitionMarks.Rows[0].Selected = true;
            };
        }

        private void bTestFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(testFile.Text))
                d.InitialDirectory = PathRoutines.GetDirFromPath(testFile.Text);
            else
                if (!string.IsNullOrWhiteSpace(testFileDefaultFolder))
                d.InitialDirectory = testFileDefaultFolder;

            d.Filter = "PDF|*.pdf|"
                + "All files (*.*)|*.*";
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            testFile.Text = d.FileName;
        }
        string testFileDefaultFolder;
        TemplateManager templateManager;

        void setStatus(statuses s, string m)
        {
            status.Text = m;
            switch (s)
            {
                case statuses.SUCCESS:
                    status.BackColor = Color.LightGreen;
                    break;
                case statuses.ERROR:
                    status.BackColor = Color.Pink;
                    break;
                case statuses.WARNING:
                    status.BackColor = Color.Yellow;
                    break;
                case statuses.NEUTRAL:
                    status.BackColor = Color.WhiteSmoke;
                    break;
                default:
                    throw new Exception("Unknown option: " + s);
            }
        }
        enum statuses
        {
            SUCCESS,
            NEUTRAL,
            WARNING,
            ERROR,
        }

        enum Modes
        {
            NULL,
            SetFloatingAnchor,
            SetDocumentFirstPageRecognitionTextMarks,
            SetFieldRectangle,
        }
        Modes mode
        {
            get
            {
                if (floatingAnchors.SelectedRows.Count > 0)
                    return Modes.SetFloatingAnchor;
                if (documentFirstPageRecognitionMarks.SelectedRows.Count > 0)
                    return Modes.SetDocumentFirstPageRecognitionTextMarks;
                if (fields.SelectedRows.Count > 0)
                    return Modes.SetFieldRectangle;
                //foreach (DataGridViewRow r in fields.Rows)
                //{
                //    if ((bool?)r.Cells["cPageRecognitionTextMarks"].Value == true)
                //        return Modes.SetPageRecognitionTextMarks;
                //}
                return Modes.NULL;
            }
        }

        private void SplitContainer1_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer s = sender as SplitContainer;
            if (s != null)
                e.Graphics.FillRectangle(SystemBrushes.ButtonShadow, s.SplitterRectangle);
        }
    }
}