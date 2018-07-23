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
        }       

        public TemplateForm(TemplateManager templateManager, string testFileDefaultFolder)
        {
            InitializeComponent();

            Icon = AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": Template Editor";

            this.templateManager = templateManager;
            this.testFileDefaultFolder = testFileDefaultFolder;

            ValueType3.ValueType = typeof(Template.ValueTypes);
            ValueType3.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            ValueType2.ValueType = typeof(Template.ValueTypes);
            ValueType2.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            FloatingAnchorId2.ValueType = typeof(int);
            FloatingAnchorId2.DataSource = null;

            FloatingAnchorId.ValueType = typeof(int);
            FloatingAnchorId.DataSource = null;

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

                drawBox(Settings.Appearance.SelectionBoxColor, selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);
            };

            picture.MouseUp += delegate (object sender, MouseEventArgs e)
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
                                    selectedPdfCharBoxs.AddRange(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPage].PdfCharBoxs, selectedR));
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
                                            selectedImageDataValue = new Template.FloatingAnchor.ImageDataValue
                                            {
                                                ImageBoxs = new List<Template.FloatingAnchor.ImageDataValue.ImageBox>()
                                            };
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
                                    throw new Exception("Could not find FloatingAnchor " + fai + " in the page");
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
                                    throw new Exception("Could not find FloatingAnchor " + fai + " in the page");
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
            };

            floatingAnchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                if (floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value == null)
                    floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value = Template.ValueTypes.PdfText;
            };

            floatingAnchors.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                DataGridViewRow r = floatingAnchors.Rows[e.RowIndex];
                try
                {
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Name", ex);
                    LogMessage.Error(ex);
                    e.Cancel = true;
                }
            };

            floatingAnchors.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                    //if (e.Row.Cells["ValueType3"].Value == null)
                    //    e.Row.Cells["ValueType3"].Value = Settings.Template.ValueTypes.PdfText;
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
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
                var r = floatingAnchors.Rows[e.RowIndex];
                int? fai = (int?)r.Cells["Id3"].Value;
                switch (floatingAnchors.Columns[e.ColumnIndex].Name)
                {
                    case "Id3":
                        findAndDrawFloatingAnchor(fai);
                        break;
                    case "Body3":
                        //if (floatingAnchors.Rows[e.RowIndex].Cells["Body3"].Value == null)                        
                        //    floatingAnchors.Rows[e.RowIndex].Cells["Id3"].Value = null;
                        onFloatingAnchorsChanged(fai);
                        findAndDrawFloatingAnchor(fai);
                        break;
                    case "ValueType3":
                        r.Cells["Body3"].Value = null;
                        break;
                }
            };

            floatingAnchors.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (floatingAnchors.SelectedRows.Count < 1)
                        return;
                    documentFirstPageRecognitionMarks.ClearSelection();
                    fields.ClearSelection();
                    var r = floatingAnchors.SelectedRows[0];

                    if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = floatingAnchors.Rows.Add();
                        floatingAnchors.Rows[i].Selected = true;
                        return;
                    }                    
                    findAndDrawFloatingAnchor((int?)r.Cells["Id3"].Value);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
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
                DataGridViewRow r = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                try
                {
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Name", ex);
                    LogMessage.Error(ex);
                    e.Cancel = true;
                }
            };

            documentFirstPageRecognitionMarks.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            documentFirstPageRecognitionMarks.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;
                    var cs = documentFirstPageRecognitionMarks.Rows[e.RowIndex].Cells;
                    int? fai = (int?)cs["FloatingAnchorId2"].Value;
                    string r_ = (string)cs["Rectangle2"].Value;
                    if (r_ == null)
                        return;
                    Template.RectangleF r = SerializationRoutines.Json.Deserialize<Template.RectangleF>(r_);
                    switch (documentFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                    {
                        case "Rectangle2":
                            object o = cs["ValueType2"].Value;
                            if (o != null)
                                cs["Value2"].Value = extractValueAndDrawSelectionBox(fai, r, (Template.ValueTypes)o);
                            break;
                        case "ValueType2":
                            cs["Value2"].Value = null;
                            break;
                        case "FloatingAnchorId2":
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
                                    cs["Rectangle2"].Value = SerializationRoutines.Json.Serialize(r);
                                }
                            }
                            break;
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

                    if (documentFirstPageRecognitionMarks.SelectedRows.Count > 0)
                    {
                        floatingAnchors.ClearSelection();
                        fields.ClearSelection();
                        int i = documentFirstPageRecognitionMarks.SelectedRows[0].Index;

                        if (documentFirstPageRecognitionMarks.Rows[i].IsNewRow)//hacky forcing commit a newly added row and display the blank row
                        {
                            int j = documentFirstPageRecognitionMarks.Rows.Add();
                            documentFirstPageRecognitionMarks.Rows[j].Selected = true;
                            return;
                        }
                        var cs = documentFirstPageRecognitionMarks.Rows[i].Cells;

                        var vt = (Template.ValueTypes)cs["ValueType2"].Value;
                        int? fai = (int?)cs["FloatingAnchorId2"].Value;
                        string rs = (string)cs["Rectangle2"].Value;
                        if (rs != null)
                        {
                            Template.RectangleF r = rs == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs);
                            string t1 = (string)cs["Value2"].Value;
                            string t2 = extractValueAndDrawSelectionBox(fai, r, vt);
                            if (t1 != t2)
                            {
                                if (vt != Template.ValueTypes.ImageData)
                                    setStatus(statuses.ERROR, "documentFirstPageRecognitionMark[" + i + "]:\r\n" + t2 + "\r\n <> \r\n" + t1);
                                else
                                    setStatus(statuses.ERROR, "documentFirstPageRecognitionMark[" + i + "]:\r\nimage is not similar");
                            }
                            else
                            {
                                lStatus.BackColor = Color.LightGreen;
                                if (vt != Template.ValueTypes.ImageData)
                                    setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "]:\r\n" + t2);
                                else
                                    setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "]:\r\nimage is similar");
                            }
                        }
                        else if (fai != null)
                        {
                            if (null != findAndDrawFloatingAnchor(fai))
                                setStatus(statuses.SUCCESS, "documentFirstPageRecognitionMark[" + i + "] is found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            fields.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                //foreach (DataGridViewRow rr in fields.Rows)
                //{
                //    if (rr.Cells["ValueType"].Value == null)
                //        rr.Cells["ValueType"].Value = Settings.Template.ValueTypes.PdfText;
                //}
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
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            fields.CellValidating += delegate (object sender, DataGridViewCellValidatingEventArgs e)
            {
                //if (e.ColumnIndex == fields.Columns["Rectangle"].Index)
                //{
                //    try
                //    {
                //        if ((string)fields.Rows[e.RowIndex].Cells["Rectangle"].Value == (string)e.FormattedValue)
                //            return;

                //        SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)e.FormattedValue);
                //    }
                //    catch (Exception ex)
                //    {
                //        //LogMessage.Error("Rectangle", ex);
                //        LogMessage.Error(ex);
                //        e.Cancel = true;
                //    }
                //}
                //else if (e.ColumnIndex == fields.Columns["Name_"].Index)
                //{
                //    if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                //    {
                //        LogMessage.Error("Name cannot be empty!");
                //        e.Cancel = true;
                //        return;
                //    }
                //}
            };

            fields.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                DataGridViewRow r = fields.Rows[e.RowIndex];
                try
                {
                    string n = FieldPreparation.Normalize((string)r.Cells["Name_"].Value);
                    if (string.IsNullOrWhiteSpace(n))
                        throw new Exception("Name cannot be empty!");
                    foreach (DataGridViewRow rr in fields.Rows)
                    {
                        if (r != rr && n == FieldPreparation.Normalize((string)rr.Cells["Name_"].Value))
                            throw new Exception("Name '" + n + "' is duplicated!");
                    }
                    r.Cells["Name_"].Value = n;
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Name", ex);
                    LogMessage.Error(ex);
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
                        {
                            Template.RectangleF r = rs == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs);
                            cs["Value"].Value = extractValueAndDrawSelectionBox(fai, r, vt);
                        }
                        else
                        {
                            findAndDrawFloatingAnchor(fai);//to shows tatus
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

            fileFilterRegex.TextChanged += delegate
            {
                try
                {
                    SerializationRoutines.Json.Deserialize<Regex>(fileFilterRegex.Text);
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("FileFilterRegex", ex);
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

        PageCollection pages = null;

        void onFloatingAnchorsChanged(int? updatedFloatingAnchorId)
        {
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Body3"].Value == null || rr.Cells["ValueType3"].Value == null)
                {
                    rr.Cells["Body3"].Value = null;
                }

            List<int> fais = new List<int>();
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value != null)
                    fais.Add((int)rr.Cells["Id3"].Value);

            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value == null && rr.Cells["Body3"].Value != null && rr.Cells["ValueType3"].Value != null)
                {
                    int fai = 1;
                    if (fais.Count > 0)
                        fai = fais.Max() + 1;
                    fais.Add(fai);
                    rr.Cells["Id3"].Value = fai;
                }

            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
            {
                int? i = (int?)r.Cells["FloatingAnchorId2"].Value;
                if (i != null && !fais.Contains((int)i))
                {
                    r.Cells["FloatingAnchorId2"].Value = null;
                    r.Cells["Rectangle2"].Value = null;
                    r.Cells["Value2"].Value = null;
                }
                if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                {
                    r.Cells["Rectangle2"].Value = null;
                    r.Cells["Value2"].Value = null;
                }
            }
            foreach (DataGridViewRow r in fields.Rows)
            {
                int? i = (int?)r.Cells["FloatingAnchorId"].Value;
                if (i != null && !fais.Contains((int)i))
                {
                    r.Cells["FloatingAnchorId"].Value = null;
                    r.Cells["Rectangle"].Value = null;
                    r.Cells["Value"].Value = null;
                }
                if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                {
                    r.Cells["Rectangle"].Value = null;
                    r.Cells["Value"].Value = null;
                }
            }

            FloatingAnchorId2.DataSource = fais;
            FloatingAnchorId.DataSource = fais;
        }

        Template.FloatingAnchor getFloatingAnchor(int id)
        {
            foreach (DataGridViewRow r in floatingAnchors.Rows)
            {
                if (r.Cells["Id3"].Value == null)
                    continue;
                int fai = (int)r.Cells["Id3"].Value;
                if (fai == id)
                    return new Template.FloatingAnchor
                    {
                        Id = fai,
                        ValueType = (Template.ValueTypes)r.Cells["ValueType3"].Value,
                        ValueAsString = (string)r.Cells["Body3"].Value
                    };
            }
            throw new Exception("There is no FloatingAnchor with Id=" + id);
        }

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

        private void SplitContainer1_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer s = sender as SplitContainer;
            if (s != null)
                e.Graphics.FillRectangle(SystemBrushes.ButtonShadow, s.SplitterRectangle);
        }

        PointF? findAndDrawFloatingAnchor(int? floatingAnchorId, bool renewImage = true)
        {
            if (floatingAnchorId == null)
                return null;
            Template.FloatingAnchor fa = getFloatingAnchor((int)floatingAnchorId);
            if (fa.GetValue() == null)
            {
                setStatus(statuses.WARNING, "FloatingAnchor[" + fa.Id + "] is not defined.");
                return null;
            }
            pages.ActiveTemplate = getTemplateFromUI(false);
            List<RectangleF> rs = pages[currentPage].GetFloatingAnchorRectangles(fa);
            if (rs == null || rs.Count < 1)
            {
                setStatus(statuses.ERROR, "FloatingAnchor[" + fa.Id + "] is not found.");
                return null;
            }
            setStatus(statuses.SUCCESS, "FloatingAnchor[" + fa.Id + "] is found.");
            drawBoxes(Settings.Appearance.BoundingBoxColor, rs, renewImage);
            return new PointF(rs[0].X, rs[0].Y);
        }

        string extractValueAndDrawSelectionBox(int? floatingAnchorId, Template.RectangleF r, Template.ValueTypes valueType, bool renewImage = true)
        {
            try
            {
                if (pages == null)
                    return null;

                pages.ActiveTemplate = getTemplateFromUI(false);

                float x = r.X;
                float y = r.Y;
                if (floatingAnchorId != null)
                {
                   PointF? p0_ = findAndDrawFloatingAnchor(floatingAnchorId);
                    if (p0_ == null)
                        return null;
                    PointF p0 = (PointF)p0_;
                    x += p0.X;
                    y += p0.Y;
                    renewImage = false;
                }

                drawBox(Settings.Appearance.SelectionBoxColor, x, y, r.Width, r.Height, renewImage);

                string error;
                object v = pages[currentPage].GetValue(null, new Template.RectangleF(x, y, r.Width, r.Height), valueType, out error);
                switch (valueType)
                {
                    case Template.ValueTypes.PdfText:
                        return Page.NormalizeText((string)v);
                    case Template.ValueTypes.OcrText:
                        return Page.NormalizeText((string)v);
                    case Template.ValueTypes.ImageData:
                        return SerializationRoutines.Json.Serialize(v, false);
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

        void drawBoxes(Color c, IEnumerable<System.Drawing.RectangleF> rs, bool renewImage = true)
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
                    gr.DrawRectangle(p, r.X * factor, r.Y * factor, r.Width * factor, r.Height * factor);
            }
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = bm;
        }

        void drawBox(Color c, float x, float y, float w, float h, bool renewImage = true)
        {
            drawBoxes(c, new List<System.Drawing.RectangleF> { new RectangleF(x, y, w, h) }, renewImage);
        }
        Point selectionBoxPoint0, selectionBoxPoint1, selectionBoxPoint2;
        bool drawingSelectionBox = false;

        void setFloatingAnchorFromSelectedElements()
        {
            try
            {
                if (floatingAnchors.SelectedRows.Count < 1)
                    return;

                DataGridViewRow r = floatingAnchors.SelectedRows[0];
                var vt = (Template.ValueTypes)r.Cells["ValueType3"].Value;
                switch (vt)
                {
                    case Template.ValueTypes.PdfText:
                        {
                            List<Pdf.Line> lines = Pdf.RemoveDuplicatesAndGetLines(selectedPdfCharBoxs);
                            if (lines.Count < 1)
                                return;
                            Template.FloatingAnchor.PdfTextValue pte = new Template.FloatingAnchor.PdfTextValue { CharBoxs = new List<Template.FloatingAnchor.PdfTextValue.CharBox>() };
                            foreach (Pdf.Line l in lines)
                                foreach (Pdf.CharBox cb in l.CharBoxes)
                                    pte.CharBoxs.Add(new Template.FloatingAnchor.PdfTextValue.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            r.Cells["Body3"].Value = Template.FloatingAnchor.GetValueAsString(pte);
                        }
                        break;
                    case Template.ValueTypes.OcrText:
                        {
                            List<Ocr.Line> lines = PdfDocumentParser.Ocr.GetLines(selectedOcrCharBoxs);
                            if (lines.Count < 1)
                                return;
                            Template.FloatingAnchor.OcrTextValue ote = new Template.FloatingAnchor.OcrTextValue { CharBoxs = new List<Template.FloatingAnchor.OcrTextValue.CharBox>() };
                            foreach (Ocr.Line l in lines)
                                foreach (Ocr.CharBox cb in l.CharBoxes)
                                    ote.CharBoxs.Add(new Template.FloatingAnchor.OcrTextValue.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            r.Cells["Body3"].Value = Template.FloatingAnchor.GetValueAsString(ote);
                        }
                        break;
                    case Template.ValueTypes.ImageData:
                        {
                            if (selectedImageDataValue.ImageBoxs.Count < 1)
                                return;                            
                            r.Cells["Body3"].Value = Template.FloatingAnchor.GetValueAsString(selectedImageDataValue);
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + vt);
                }
            }
            finally
            {
                floatingAnchors.EndEdit();
                selectedPdfCharBoxs = null;
                selectedOcrCharBoxs = null;
                selectedImageDataValue = null;
            }
        }
        List<Pdf.CharBox> selectedPdfCharBoxs;
        List<Ocr.CharBox> selectedOcrCharBoxs;
        Template.FloatingAnchor.ImageDataValue selectedImageDataValue;
        
        void setUIFromTemplate(Template t)
        {
            try
            {
                loadingTemplate = true;

                name.Text = t.Name;

                //imageResolution.Value = template.ImageResolution;

                pageRotation.SelectedIndex = (int)t.PagesRotation;
                autoDeskew.Checked = t.AutoDeskew;
                AutoDeskewThreshold.Value = t.AutoDeskewThreshold;
                findBestImageMatch.Checked = t.FindBestImageMatch;
                brightnessTolerance.Value = (decimal)t.BrightnessTolerance;
                differentPixelNumberTolerance.Value = (decimal)t.DifferentPixelNumberTolerance;

                floatingAnchors.Rows.Clear();
                if (t.FloatingAnchors != null)
                {
                    foreach (Template.FloatingAnchor fa in t.FloatingAnchors)
                    {
                        int i = floatingAnchors.Rows.Add();
                        var cs = floatingAnchors.Rows[i].Cells;
                        cs["Id3"].Value = fa.Id;
                        cs["ValueType3"].Value = fa.ValueType;
                        cs["Body3"].Value = fa.ValueAsString;
                    }
                    onFloatingAnchorsChanged(null);
                }

                documentFirstPageRecognitionMarks.Rows.Clear();
                if (t.DocumentFirstPageRecognitionMarks != null)
                {
                    foreach (Template.Mark m in t.DocumentFirstPageRecognitionMarks)
                    {
                        int i = documentFirstPageRecognitionMarks.Rows.Add();
                        var cs = documentFirstPageRecognitionMarks.Rows[i].Cells;
                        cs["Rectangle2"].Value = m.Rectangle == null ? null : SerializationRoutines.Json.Serialize(m.Rectangle);
                        cs["ValueType2"].Value = m.ValueType;
                        cs["Value2"].Value = m.ValueAsString;
                        cs["FloatingAnchorId2"].Value = m.FloatingAnchorId;
                    }
                }

                fields.Rows.Clear();
                if (t.Fields != null)
                {
                    foreach (Template.Field f in t.Fields)
                    {
                        int i = fields.Rows.Add();
                        var cs = fields.Rows[i].Cells;
                        cs["Name_"].Value = f.Name;
                        cs["Rectangle"].Value = f.Rectangle == null ? null : SerializationRoutines.Json.Serialize(f.Rectangle);
                        cs["Ocr"].Value = f.ValueType == Template.ValueTypes.PdfText ? false : true;
                        cs["FloatingAnchorId"].Value = f.FloatingAnchorId;
                    }
                }

                if (t.FileFilterRegex != null)
                    fileFilterRegex.Text = SerializationRoutines.Json.Serialize(t.FileFilterRegex);
                else
                    fileFilterRegex.Text = "";

                pictureScale.Value = t.TestPictureScale > 0 ? t.TestPictureScale : 1;


                if (t.TestFile != null && File.Exists(t.TestFile))
                    testFile.Text = t.TestFile;
                else
                {
                    if (templateManager.LastTestFile != null && File.Exists(templateManager.LastTestFile))
                        testFile.Text = templateManager.LastTestFile;
                }
            }
            finally
            {
                loadingTemplate = false;
            }
        }
        bool loadingTemplate = false;

        void setStatus(statuses s, string m)
        {
            lStatus.Text = m;
            switch (s)
            {
                case statuses.SUCCESS:
                    lStatus.BackColor = Color.LightGreen;
                    break;
                case statuses.ERROR:
                    lStatus.BackColor = Color.LightPink;
                    break;
                case statuses.WARNING:
                    lStatus.BackColor = Color.LightYellow;
                    break;
                case statuses.NEUTRAL:
                    lStatus.BackColor = Color.WhiteSmoke;
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

        void showPage(int page_i)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= page_i || totalPageNumber < page_i)
                    return;

                floatingAnchors.ClearSelection();
                documentFirstPageRecognitionMarks.ClearSelection();
                fields.ClearSelection();

                currentPage = page_i;
                tCurrentPage.Text = currentPage.ToString();

                pages.ActiveTemplate = getTemplateFromUI(false);
                setScaledImage();
                enableNabigationButtons();

                checkIfCurrentPageIsDocumentFirstPage(); 
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
        int currentPage;
        int totalPageNumber;

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

        private void bPrevPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage - 1);
        }

        private void bNextPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage + 1);
        }

        void enableNabigationButtons()
        {
            bPrevPage.Enabled = currentPage > 1;
            bNextPage.Enabled = currentPage < totalPageNumber;
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

        bool? checkIfCurrentPageIsDocumentFirstPage()
        {
            try
            {
                if (documentFirstPageRecognitionMarks.Rows.Count < 2)
                {
                    lStatus.Text = "No condition of first page of document is specified!";
                    lStatus.BackColor = Color.LightYellow;
                    return null;
                }

                Template t = getTemplateFromUI(false);
                pages.ActiveTemplate = t;
                string error;
                if (!pages[currentPage].IsDocumentFirstPage(out error))
                {
                    lStatus.Text = error;
                    lStatus.BackColor = Color.LightPink;
                    return false;
                }
                lStatus.Text = "The page matches first page of document.";
                lStatus.BackColor = Color.LightGreen;
                return true;
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
            return false;
        }

        private void bTestFileFilterRegex_Click(object sender, EventArgs e)
        {
            try
            {
                string d = string.IsNullOrWhiteSpace(testFile.Text) ? testFileDefaultFolder : PathRoutines.GetDirFromPath(testFile.Text);
                FileFilterForm f = new FileFilterForm(d, SerializationRoutines.Json.Deserialize<Regex>(fileFilterRegex.Text));
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
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

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SaveAsInitialTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Template t = getTemplateFromUI(true);
                templateManager.SaveAsInitialTemplate(t);
                Message.Inform("Saved");
            }
            catch(Exception ex)
            {
                Message.Error2(ex);
            }
        }

        private void About_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void Configure_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
        }

        private void ShowPdfText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pages == null)
                return;
            TextForm tf = new TextForm("Pdf Entity Text", PdfTextExtractor.GetTextFromPage(pages.PdfReader, currentPage), false);
            tf.ShowDialog();
        }

        private void ShowOcrText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pages == null)
                return;
            //TextForm tf = new TextForm("OCR Text", PdfDocumentParser.Ocr.This.GetHtml(pages[currentPage].Bitmap), true);
            TextForm tf = new TextForm("OCR Text", PdfDocumentParser.Ocr.GetText(pages[currentPage].ActiveTemplateOcrCharBoxs), false);
            tf.ShowDialog();
        }

        private void IsDocumentFirstPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            checkIfCurrentPageIsDocumentFirstPage();
        }

        private void Help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string helpFile = @"help\help.html";
            System.Diagnostics.Process.Start(helpFile);
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                //NewTemplate = getTemplateFromUI(true);
                Template t = getTemplateFromUI(true);
                templateManager.ReplaceWith(t);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }

        Template getTemplateFromUI(bool saving)
        {
            Template t;
            if (saving)
                t = templateManager.New();
            else
                t = new Template();

            if (string.IsNullOrWhiteSpace(name.Text))
                if (saving)
                    throw new Exception("Name is empty!");

            if (documentFirstPageRecognitionMarks.Rows.Count < 2)
                if (saving)
                    throw new Exception("DocumentFirstPageRecognitionMarks is empty!");

            t.Name = name.Text.Trim();

            t.PagesRotation = (Template.PageRotations)pageRotation.SelectedIndex;
            t.AutoDeskew = autoDeskew.Checked;
            t.AutoDeskewThreshold = (int)AutoDeskewThreshold.Value;
            t.FindBestImageMatch = findBestImageMatch.Checked;
            t.BrightnessTolerance = (float)brightnessTolerance.Value;
            t.DifferentPixelNumberTolerance = (float)differentPixelNumberTolerance.Value;

            t.FloatingAnchors = new List<Template.FloatingAnchor>();
            foreach (DataGridViewRow r in floatingAnchors.Rows)
                if (r.Cells["Id3"].Value != null)
                    t.FloatingAnchors.Add(new Template.FloatingAnchor
                    {
                        Id = (int)r.Cells["Id3"].Value,
                        ValueType = (Template.ValueTypes)r.Cells["ValueType3"].Value,
                        ValueAsString = (string)r.Cells["Body3"].Value
                    });
            t.FloatingAnchors = t.FloatingAnchors.OrderBy(a => a.Id).ToList();

            t.DocumentFirstPageRecognitionMarks = new List<Template.Mark>();
            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
                if (r.Cells["Value2"].Value != null || r.Cells["FloatingAnchorId2"].Value != null)
                {
                    Template.Mark m = new Template.Mark
                    {
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId2"].Value,
                        Rectangle = r.Cells["Rectangle2"].Value == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>((string)r.Cells["Rectangle2"].Value),
                        ValueType = (Template.ValueTypes)r.Cells["ValueType2"].Value,
                        ValueAsString = (string)r.Cells["Value2"].Value
                    };
                    if (m.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == m.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + m.FloatingAnchorId);
                    t.DocumentFirstPageRecognitionMarks.Add(m);
                }

            t.Fields = new List<Template.Field>();
            foreach (DataGridViewRow r in fields.Rows)
            {
                string name = (string)r.Cells["Name_"].Value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    //if (r.Cells["Rectangle"].Value == null)
                    //{
                    //    if (saving)
                    //        throw new Exception("Field '" + name + "' is not set!");
                    //    continue;
                    //}
                    Template.Field f = new Template.Field
                    {
                        Name = name.Trim(),
                        Rectangle = r.Cells["Rectangle"].Value == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>((string)r.Cells["Rectangle"].Value),
                        ValueType = Convert.ToBoolean(r.Cells["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText,
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId"].Value
                    };
                    if (f.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == f.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + f.FloatingAnchorId);
                    t.Fields.Add(f);
                }
            }

            t.TestFile = testFile.Text;

            //t.ImageResolution = (int)imageResolution.Value;

            t.TestPictureScale = pictureScale.Value;

            t.FileFilterRegex = SerializationRoutines.Json.Deserialize<Regex>(fileFilterRegex.Text);

            return t;
        }
    }
}