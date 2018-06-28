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

namespace Cliver.InvoiceParser
{
    public partial class TemplateForm : Form
    {
        public TemplateForm(Settings.Template template)
        {
            InitializeComponent();

            Icon = AssemblyRoutines.GetAppIcon();
            Text = "Template Manager";

            ValueType3.ValueType = typeof(Settings.Template.ValueTypes);
            ValueType3.DataSource = Enum.GetValues(typeof(Settings.Template.ValueTypes));

            ValueType2.ValueType = typeof(Settings.Template.ValueTypes);
            ValueType2.DataSource = Enum.GetValues(typeof(Settings.Template.ValueTypes));

            FloatingAnchorId2.ValueType = typeof(int);
            FloatingAnchorId2.DataSource = null;
            
            FloatingAnchorId.ValueType = typeof(int);
            FloatingAnchorId.DataSource = null;

            Shown += delegate
            {
                Application.DoEvents();//make form be drawn completely
                setUIFromTemplate(template);
                floatingAnchors.ClearSelection();
                invoiceFirstPageRecognitionMarks.ClearSelection();
                fields.ClearSelection();
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
            };

            picture.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;
                drawingSelectingBox = true;
                p0 = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));
                p1 = new Point(p0.X, p0.Y);
                p2 = new Point(p0.X, p0.Y);
                selectionCoordinates.Text = p1.ToString();
            };

            picture.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;

                Point p = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));

                if (!drawingSelectingBox)
                {
                    selectionCoordinates.Text = p.ToString();
                    return;
                }

                if (p0.X < p.X)
                {
                    p1.X = p0.X;
                    p2.X = p.X;
                }
                else
                {
                    p1.X = p.X;
                    p2.X = p0.X;
                }
                if (p0.Y < p.Y)
                {
                    p1.Y = p0.Y;
                    p2.Y = p.Y;
                }
                else
                {
                    p1.Y = p.Y;
                    p2.Y = p0.Y;
                }
                selectionCoordinates.Text = p1.ToString() + ":" + p2.ToString();

                drawBox(Settings.General.SelectionBoxColor, p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            };

            picture.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;

                if (!drawingSelectingBox)
                    return;
                drawingSelectingBox = false;

                Settings.Template.RectangleF r = new Settings.Template.RectangleF(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);

                switch (mode)
                {
                    case Modes.SetFloatingAnchor:
                        {
                            if (floatingAnchors.SelectedRows.Count < 1)
                                break;

                            RectangleF selectedR = new RectangleF(p1, new SizeF(p2.X - p1.X, p2.Y - p1.Y));
                            Settings.Template.ValueTypes vt = (Settings.Template.ValueTypes)floatingAnchors.SelectedRows[0].Cells["ValueType3"].Value;
                            switch (vt)
                            {
                                case Settings.Template.ValueTypes.PdfText:
                                    if (selectedPdfBoxTexts == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                        selectedPdfBoxTexts = new List<Pdf.BoxText>();
                                    selectedPdfBoxTexts.AddRange(Pdf.GetBoxTextsSurroundedByRectangle(pages[currentPage].CharBoxs, selectedR));
                                    break;
                                case Settings.Template.ValueTypes.OcrText:
                                    {
                                        if (selectedOcrTextElement == null)
                                            selectedOcrTextElement = new Settings.Template.FloatingAnchor.OcrTextElement
                                            {
                                                TextBoxs = new List<Settings.Template.FloatingAnchor.OcrTextElement.TextBox>()
                                            };
                                        string error;
                                        selectedOcrTextElement.TextBoxs.Add(new Settings.Template.FloatingAnchor.OcrTextElement.TextBox
                                        {
                                            Rectangle = Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR),
                                            Text = (string)pages[currentPage].GetValue(null, Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR), Settings.Template.ValueTypes.OcrText, out error)
                                        });
                                    }
                                    break;
                                case Settings.Template.ValueTypes.ImageData:
                                    {
                                        if (selectedImageDataElement == null)
                                            selectedImageDataElement = new Settings.Template.FloatingAnchor.ImageDataElement
                                            {
                                                ImageBoxs = new List<Settings.Template.FloatingAnchor.ImageDataElement.ImageBox>()
                                            };
                                        string error;
                                        selectedImageDataElement.ImageBoxs.Add(new Settings.Template.FloatingAnchor.ImageDataElement.ImageBox
                                        {
                                            Rectangle = Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR),
                                            ImageData = (ImageData)pages[currentPage].GetValue(null, Settings.Template.RectangleF.GetFromSystemRectangleF(selectedR), Settings.Template.ValueTypes.ImageData, out error)
                                        });
                                    }
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + vt);
                            }

                            if ((ModifierKeys & Keys.Control) != Keys.Control)
                                setFloatingAnchorFromSelectedElement();
                        }
                        break;
                    case Modes.SetInvoiceFirstPageRecognitionTextMarks:
                        {
                            if (invoiceFirstPageRecognitionMarks.SelectedRows.Count < 1)
                                break;                            
                            var cs = invoiceFirstPageRecognitionMarks.SelectedRows[0].Cells;
                            Settings.Template.FloatingAnchor fa = null;
                            int? fai = (int?)cs["FloatingAnchorId2"].Value;
                            if (fai != null)
                            {
                                fa = getFloatingAnchor((int)fai);
                                List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
                                if (rs == null || rs.Count < 1)
                                    throw new Exception("Could not find FloatingAnchor " + fa.Id + " in the page");
                                r.X -= rs[0].X;
                                r.Y -= rs[0].Y;
                            }
                            cs["Rectangle2"].Value = SerializationRoutines.Json.Serialize(r);
                            invoiceFirstPageRecognitionMarks.EndEdit();
                        }
                        break;
                    case Modes.SetFieldRectangle:
                        {
                            if (fields.SelectedRows.Count < 1)
                                break;
                            var cs = fields.SelectedRows[0].Cells;
                            Settings.Template.FloatingAnchor fa = null;
                            int? fai = (int?)cs["FloatingAnchorId"].Value;
                            if (fai != null)
                            {
                                fa = getFloatingAnchor((int)fai);
                                List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
                                if (rs == null || rs.Count < 1)
                                    throw new Exception("Could not find FloatingAnchor " + fa.Id + " in the page");
                                r.X -= rs[0].X;
                                r.Y -= rs[0].Y;
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
                    floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value = Settings.Template.ValueTypes.PdfText;
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
                switch (floatingAnchors.Columns[e.ColumnIndex].Name)
                {
                    //case "Id3":
                    //        onFloatingAnchorsChanged();
                    //    break;
                    case "Value3":
                        //if (floatingAnchors.Rows[e.RowIndex].Cells["Value3"].Value == null)                        
                        //    floatingAnchors.Rows[e.RowIndex].Cells["Id3"].Value = null;
                        onFloatingAnchorsChanged((int?)r.Cells["Id3"].Value);
                        drawFloatingAnchor(r);
                        break;
                    case "ValueType3":
                        r.Cells["Value3"].Value = null;
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
                    invoiceFirstPageRecognitionMarks.ClearSelection();
                    fields.ClearSelection();
                    var r = floatingAnchors.SelectedRows[0];

                    if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = floatingAnchors.Rows.Add();
                        floatingAnchors.Rows[i].Selected = true;
                        return;
                    }
                    drawFloatingAnchor(r);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            invoiceFirstPageRecognitionMarks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                if (invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["ValueType2"].Value == null)
                    invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["ValueType2"].Value = Settings.Template.ValueTypes.PdfText;
            };

            invoiceFirstPageRecognitionMarks.CurrentCellDirtyStateChanged += delegate
            {
                if (invoiceFirstPageRecognitionMarks.IsCurrentCellDirty)
                    invoiceFirstPageRecognitionMarks.CommitEdit(DataGridViewDataErrorContexts.Commit);                
            };

            invoiceFirstPageRecognitionMarks.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                DataGridViewRow r = invoiceFirstPageRecognitionMarks.Rows[e.RowIndex];
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

            invoiceFirstPageRecognitionMarks.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            invoiceFirstPageRecognitionMarks.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                { 
                if (loadingTemplate)
                    return;

                var cs = invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells;
                Settings.Template.FloatingAnchor fa = null;
                int? fai = (int?)cs["FloatingAnchorId2"].Value;
                if (fai != null)
                    fa = getFloatingAnchor((int)fai);
                string r_ = (string)cs["Rectangle2"].Value;
                    if (r_ == null)
                        return;
                Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(r_);
                switch (invoiceFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                {
                    case "Rectangle2":
                    case "ValueType2":
                        object o = cs["ValueType2"].Value;
                        if (o != null)
                            cs["Value2"].Value = extractValueAndDrawBox(fa, r, (Settings.Template.ValueTypes)o);
                        break;
                    case "FloatingAnchorId2":
                        if (fa != null)
                        {
                            List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
                            if (rs == null || rs.Count < 1)
                                throw new Exception("Could not find FloatingAnchor " + fa.Id + " in the page");
                            r.X -= rs[0].X;
                            r.Y -= rs[0].Y;
                        }
                        cs["Rectangle2"].Value = SerializationRoutines.Json.Serialize(r);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            invoiceFirstPageRecognitionMarks.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (invoiceFirstPageRecognitionMarks.SelectedRows.Count > 0)
                    {
                        floatingAnchors.ClearSelection();
                        fields.ClearSelection();
                        int i = invoiceFirstPageRecognitionMarks.SelectedRows[0].Index;

                        if (invoiceFirstPageRecognitionMarks.Rows[i].IsNewRow)//hacky forcing commit a newly added row and display the blank row
                        {
                            int j = invoiceFirstPageRecognitionMarks.Rows.Add();
                            invoiceFirstPageRecognitionMarks.Rows[j].Selected = true;
                            return;
                        }
                        var cs = invoiceFirstPageRecognitionMarks.Rows[i].Cells;
                        string rs = (string)cs["Rectangle2"].Value;
                        if (string.IsNullOrWhiteSpace(rs))
                            return;

                        Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);

                        Settings.Template.FloatingAnchor fa = null;
                        int? fai = (int?)cs["FloatingAnchorId2"].Value;
                        if (fai != null)
                            fa = getFloatingAnchor((int)fai);

                        //drawBox(Settings.General.SelectionBoxColor, r.X, r.Y, r.Width, r.Height, true);
                        string t1 = (string)cs["Value2"].Value;
                        var vt = (Settings.Template.ValueTypes)cs["ValueType2"].Value;
                        string t2 = extractValueAndDrawBox(fa, r, vt);
                        if (t1 != t2)
                        {
                            lStatus.BackColor = Color.LightPink;
                            if(vt!= Settings.Template.ValueTypes.ImageData)
                                lStatus.Text = "InvoiceFirstPageRecognitionMark[" + i + "]:\r\n" + t2 + "\r\n <> \r\n" + t1;
                            else
                                lStatus.Text = "InvoiceFirstPageRecognitionMark[" + i + "]:\r\nimage is not similar";
                        }
                        else
                        {
                            lStatus.BackColor = Color.LightGreen;
                            if (vt != Settings.Template.ValueTypes.ImageData)
                                lStatus.Text = "InvoiceFirstPageRecognitionMark[" + i + "]:\r\n" + t2;
                            else
                                lStatus.Text = "InvoiceFirstPageRecognitionMark[" + i + "]:\r\nimage is similar";
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
                    Settings.Template.FloatingAnchor fa = null;
                    int? fai = (int?)cs["FloatingAnchorId"].Value;
                    if (fai != null)
                        fa = getFloatingAnchor((int)fai);
                    string r_ = (string)cs["Rectangle"].Value;
                    if (r_ == null)
                        return;
                    Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(r_);
                    switch (fields.Columns[e.ColumnIndex].Name)
                    {
                        case "Rectangle":
                        case "Ocr":
                            cs["Value"].Value = extractValueAndDrawBox(fa, r, Convert.ToBoolean(cs["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText);
                            break;
                        case "FloatingAnchorId":
                            if (fa != null)
                            {
                                List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
                                if (rs == null || rs.Count < 1)
                                    throw new Exception("Could not find FloatingAnchor " + fa.Id + " in the page");
                                r.X -= rs[0].X;
                                r.Y -= rs[0].Y;
                            }
                            cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
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
                if (e.ColumnIndex == fields.Columns["Rectangle"].Index)
                {
                    try
                    {
                        if ((string)fields.Rows[e.RowIndex].Cells["Rectangle"].Value == (string)e.FormattedValue)
                            return;

                        SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)e.FormattedValue);
                    }
                    catch (Exception ex)
                    {
                        //LogMessage.Error("Rectangle", ex);
                        LogMessage.Error(ex);
                        e.Cancel = true;
                    }
                }
                else if (e.ColumnIndex == fields.Columns["Name_"].Index)
                {
                    if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                    {
                        LogMessage.Error("Name cannot be empty!");
                        e.Cancel = true;
                        return;
                    }
                }
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
                try
                {
                    if (string.IsNullOrWhiteSpace((string)r.Cells["Rectangle"].Value))
                        throw new Exception("Rectangle cannot be empty!");
                    SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)r.Cells["Rectangle"].Value);
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Rectangle", ex);
                    LogMessage.Error(ex);
                    e.Cancel = true;
                }
            };

            fields.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace((string)e.Row.Cells["Rectangle"].Value))
                    {
                        e.Row.Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(new Settings.Template.RectangleF(0, 0, 0, 0));
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
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
                        invoiceFirstPageRecognitionMarks.ClearSelection();
                        int i = fields.SelectedRows[0].Index;
                        string rs = (string)fields.Rows[i].Cells["Rectangle"].Value;
                        if (!string.IsNullOrWhiteSpace(rs))
                        {
                            var cs = fields.Rows[i].Cells;
                            Settings.Template.FloatingAnchor fa = null;
                            int? fI = (int?)cs["FloatingAnchorId"].Value;
                            if (fI != null)
                                fa = getFloatingAnchor((int)fI);
                            Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
                            fields.Rows[i].Cells["Value"].Value = extractValueAndDrawBox(fa, r, Convert.ToBoolean(cs["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText);
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
                    picture.Image = null;
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

                    if (!File.Exists(testFile.Text))
                    {
                        LogMessage.Error("File '" + testFile.Text + "' does not exist!");
                        return;
                    }

                    pages = new PageCollection(testFile.Text);
                    pages.ActiveTemplate = template;
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
                if (invoiceFirstPageRecognitionMarks.Rows.Count > 0 && !invoiceFirstPageRecognitionMarks.Rows[0].IsNewRow)
                    invoiceFirstPageRecognitionMarks.Rows[0].Selected = true;
            };
        }

        PageCollection pages = null;

        void onFloatingAnchorsChanged(int? updatedFloatingAnchorId)
        {
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Value3"].Value == null || rr.Cells["ValueType3"].Value == null)
                {
                    rr.Cells["Value3"].Value = null;
                }

            List<int> fais = new List<int>();
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value != null)
                    fais.Add((int)rr.Cells["Id3"].Value);

            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value == null && rr.Cells["Value3"].Value != null && rr.Cells["ValueType3"].Value != null)
                {
                    int fai = 1;
                    if (fais.Count > 0)
                        fai = fais.Max() + 1;
                    fais.Add(fai);
                    rr.Cells["Id3"].Value = fai;
                }

            foreach (DataGridViewRow r in invoiceFirstPageRecognitionMarks.Rows)
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

        void drawFloatingAnchor(DataGridViewRow floatingAnchorRow)
        {
            int? fai = (int?)floatingAnchorRow.Cells["Id3"].Value;
            if (fai == null)
                return;
            Settings.Template.FloatingAnchor fa = getFloatingAnchor((int)fai);
            if(fa.Value== null)
            {
                lStatus.BackColor = Color.LightYellow;
                lStatus.Text = "FindFloatingAnchor[" + fa.Id + "] is not defined.";
                return;
            }
            List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
            if (rs == null || rs.Count < 1)
            {
                lStatus.BackColor = Color.LightPink;
                lStatus.Text = "FindFloatingAnchor[" + fa.Id + "] is not found.";
                return;
            }
            lStatus.BackColor = Color.LightGreen;
            lStatus.Text = "FindFloatingAnchor[" + fa.Id + "] is found.";
            drawBoxes(Settings.General.BoundingBoxColor, rs, true);
        }

        Settings.Template.FloatingAnchor getFloatingAnchor(int id)
        {
            foreach (DataGridViewRow r in floatingAnchors.Rows)
            {
                if (r.Cells["Id3"].Value == null)
                    continue;
                int fai = (int)r.Cells["Id3"].Value;
                if (fai == id)
                {
                    return new Settings.Template.FloatingAnchor
                    {
                        Id = fai,
                        ValueType = (Settings.Template.ValueTypes)r.Cells["ValueType3"].Value,
                        Value = (string)r.Cells["Value3"].Value,
                    };
                }
            }
            throw new Exception("There is no FloatingAnchor with Id=" + id);
        }

        void reloadPageBitmaps()
        {
            if (pages == null)
                return;
            pages.Clear();
            pages.ActiveTemplate = getTemplateFromUI(false);
            showPage(currentPage);
        }

        void setScaledImage()
        {
            if (pages == null)
                return;
            scaledCurrentPageBitmap = ImageRoutines.GetScaled(pages[currentPage].BitmapPreparedForTemplate, (float)pictureScale.Value * Settings.General.Image2PdfResolutionRatio);
            picture.Image = scaledCurrentPageBitmap;
        }
        Bitmap scaledCurrentPageBitmap;

        private void SplitContainer1_Paint(object sender, PaintEventArgs e)
        {
            SplitContainer s = sender as SplitContainer;
            if (s != null)
                e.Graphics.FillRectangle(SystemBrushes.ButtonShadow, s.SplitterRectangle);
        }

        string extractValueAndDrawBox(Settings.Template.FloatingAnchor fa, Settings.Template.RectangleF r, Settings.Template.ValueTypes valueType, bool renewImage = true)
        {
            try
            {
                if (pages == null)
                    return null;

                float x = r.X, y = r.Y;
                if (fa != null)
                {
                    List<RectangleF> rs = pages[currentPage].FindFloatingAnchor(fa);
                    if (rs == null || rs.Count < 1)
                        return null;
                    drawBoxes(Settings.General.BoundingBoxColor, rs, renewImage);
                    x += rs[0].X;
                    y += rs[0].Y;
                    renewImage = false;
                }

                drawBox(Settings.General.SelectionBoxColor, x, y, r.Width, r.Height, renewImage);

                string error;
                object v = pages[currentPage].GetValue(null, new Settings.Template.RectangleF(x, y, r.Width, r.Height), valueType, out error);
                switch (valueType)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        return FieldPreparation.Normalize((string)v);
                    case Settings.Template.ValueTypes.OcrText:
                        return FieldPreparation.Normalize((string)v);
                    case Settings.Template.ValueTypes.ImageData:
                        return ((ImageData)v).Serialize();
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
                Pen p = new Pen(Settings.General.BoundingBoxColor);
                foreach (System.Drawing.RectangleF r in rs)
                    gr.DrawRectangle(p, r.X * factor, r.Y * factor, r.Width * factor, r.Height * factor);
            }
            picture.Image = bm;
        }

        void drawBox(Color c, float x, float y, float w, float h, bool renewImage = true)
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
                gr.DrawRectangle(new Pen(c), x * factor, y * factor, w * factor, h * factor);
            }
            picture.Image = bm;
        }
        Point p0, p1, p2;
        bool drawingSelectingBox = false;

        void setFloatingAnchorFromSelectedElement()
        {
            try
            {
                if (floatingAnchors.SelectedRows.Count < 1)
                    return;

                DataGridViewRow r = floatingAnchors.SelectedRows[0];
                var vt = (Settings.Template.ValueTypes)r.Cells["ValueType3"].Value;
                switch (vt)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        {
                            selectedPdfBoxTexts = Pdf.RemoveDuplicatesAndOrder(selectedPdfBoxTexts);
                            if (selectedPdfBoxTexts.Count < 1)
                                return;
                            Settings.Template.FloatingAnchor.PdfTextElement pte = new Settings.Template.FloatingAnchor.PdfTextElement();
                            pte.CharBoxs = selectedPdfBoxTexts.Select(a => new Settings.Template.FloatingAnchor.PdfTextElement.CharBox
                            {
                                Char = a.Text,
                                Rectangle = new Settings.Template.RectangleF(a.R.X, a.R.Y, a.R.Width, a.R.Height),
                            }).ToList();
                            r.Cells["Value3"].Value = SerializationRoutines.Json.Serialize(pte);
                        }
                        break;
                    case Settings.Template.ValueTypes.OcrText:
                        {
                            if (selectedOcrTextElement.TextBoxs.Count < 1)
                                return;
                            r.Cells["Value3"].Value = SerializationRoutines.Json.Serialize(selectedOcrTextElement);
                        }
                        break;
                    case Settings.Template.ValueTypes.ImageData:
                        {
                            if (selectedImageDataElement.ImageBoxs.Count < 1)
                                return;
                            r.Cells["Value3"].Value = SerializationRoutines.Json.Serialize(selectedImageDataElement);
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + vt);
                }
            }
            finally
            {
                floatingAnchors.EndEdit();
                selectedPdfBoxTexts = null;
                selectedOcrTextElement = null;
                selectedImageDataElement = null;
            }
        }
        List<Pdf.BoxText> selectedPdfBoxTexts;
        Settings.Template.FloatingAnchor.OcrTextElement selectedOcrTextElement;
        Settings.Template.FloatingAnchor.ImageDataElement selectedImageDataElement;

        void setUIFromTemplate(Settings.Template t)
        {
            try
            {
                loadingTemplate = true;

                Text = "Template Editor";
                name.Text = t.Name;

                //imageResolution.Value = template.ImageResolution;

                autoDeskew.Checked = t.AutoDeskew;

                floatingAnchors.Rows.Clear();
                if (t.FloatingAnchors != null)
                {
                    foreach (Settings.Template.FloatingAnchor f in t.FloatingAnchors)
                    {
                        int i = floatingAnchors.Rows.Add();
                        var cs = floatingAnchors.Rows[i].Cells;
                        cs["Id3"].Value = f.Id;
                        cs["ValueType3"].Value = f.ValueType;
                        cs["Value3"].Value = f.Value;
                    }
                    onFloatingAnchorsChanged(null);
                }

                invoiceFirstPageRecognitionMarks.Rows.Clear();
                if (t.InvoiceFirstPageRecognitionMarks != null)
                {
                    foreach (Settings.Template.Mark m in t.InvoiceFirstPageRecognitionMarks)
                    {
                        int i = invoiceFirstPageRecognitionMarks.Rows.Add();
                        var cs = invoiceFirstPageRecognitionMarks.Rows[i].Cells;
                        cs["Rectangle2"].Value = SerializationRoutines.Json.Serialize(m.Rectangle);
                        cs["ValueType2"].Value = m.ValueType;
                        cs["Value2"].Value = m.Value;
                        cs["FloatingAnchorId2"].Value = m.FloatingAnchorId;
                    }
                }

                fields.Rows.Clear();
                if (t.Fields != null)
                {
                    foreach (Settings.Template.Field f in t.Fields)
                    {
                        int i = fields.Rows.Add();
                        var cs = fields.Rows[i].Cells;
                        cs["Name_"].Value = f.Name;
                        cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
                        cs["Ocr"].Value = f.ValueType == Settings.Template.ValueTypes.PdfText ? false : true;
                        cs["FloatingAnchorId"].Value = f.FloatingAnchorId;
                    }
                }

                if (t.FileFilterRegex != null)
                    fileFilterRegex.Text = SerializationRoutines.Json.Serialize(t.FileFilterRegex);
                else
                    fileFilterRegex.Text = "";

                pictureScale.Value = t.TestPictureScale > 0 ? t.TestPictureScale : 1;

                if (File.Exists(t.TestFile))
                    testFile.Text = t.TestFile;

            }
            finally
            {
                loadingTemplate = false;
            }
        }
        bool loadingTemplate = false;

        void showPage(int page_i)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= page_i || totalPageNumber < page_i)
                    return;

                currentPage = page_i;
                tCurrentPage.Text = currentPage.ToString();
                setScaledImage();
                enableNabigationButtons();

                checkIfCurrentPageIsInvoiceFirstPage();

                //if (invoiceFirstPageRecognitionMarks.SelectedRows.Count < 1)
                //    catchFields();

                //using (var engine = new Tesseract.TesseractEngine(@"./tessdata", "eng", Tesseract.EngineMode.Default))
                //{
                //    using (var page = engine.Process(pageBitmaps[currentPage/*, Settings.Template.PageRotations.Clockwise90*/]))
                //    {
                //        //string t = page.GetHOCRText(1, true);
                //        //var dfg = page.GetThresholdedImage();                        
                //        Tesseract.Orientation o;
                //        float c;
                //       // page.DetectBestOrientation(out o, out c);
                //      //  var l = page.AnalyseLayout();
                //     //var ti =   l.GetBinaryImage(Tesseract.PageIteratorLevel.Para);
                //        Tesseract.Rect r;
                //       // l.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
                //        using (var i = page.GetIterator())
                //        {
                //            int j = 0;
                //            i.Begin();
                //            do
                //            {
                //                bool g = i.IsAtBeginningOf(Tesseract.PageIteratorLevel.Block);
                //                bool v = i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block, out r);
                //                var bt = i.BlockType;
                //                if (Regex.IsMatch(bt.ToString(), @"image", RegexOptions.IgnoreCase))
                //                {
                //                    //i.TryGetBoundingBox(Tesseract.PageIteratorLevel.Block,out r);
                //                    Tesseract.Pix p = i.GetBinaryImage(Tesseract.PageIteratorLevel.Block);
                //                    Bitmap b = Tesseract.PixConverter.ToBitmap(p);
                //                    b.Save(Log.AppDir + "\\test" + (j++) + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //                }
                //            } while (i.Next(Tesseract.PageIteratorLevel.Block));
                //        }
                //    }
                //}
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
                if (!string.IsNullOrWhiteSpace(Settings.General.InputFolder))
                d.InitialDirectory = Settings.General.InputFolder;

            d.Filter = "PDF|*.pdf|"
                + "All files (*.*)|*.*";
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            testFile.Text = d.FileName;
        }

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
            SetInvoiceFirstPageRecognitionTextMarks,
            SetFieldRectangle,
        }
        Modes mode
        {
            get
            {
                if (floatingAnchors.SelectedRows.Count > 0)
                    return Modes.SetFloatingAnchor;
                if (invoiceFirstPageRecognitionMarks.SelectedRows.Count > 0)
                    return Modes.SetInvoiceFirstPageRecognitionTextMarks;
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

        private void bIsInvoiceFirstPage_Click(object sender, EventArgs e)
        {
            checkIfCurrentPageIsInvoiceFirstPage();
        }

        bool? checkIfCurrentPageIsInvoiceFirstPage()
        {
            try
            {
                if (invoiceFirstPageRecognitionMarks.Rows.Count < 2)
                {
                    lStatus.Text = "No condition of first page of invoice is specified!";
                    lStatus.BackColor = Color.LightYellow;
                    return null;
                }

                Settings.Template t = getTemplateFromUI(false);
                pages.ActiveTemplate = t;
                string error;
                if (!pages[currentPage].IsInvoiceFirstPage(out error))
                {
                    lStatus.Text = error;
                    lStatus.BackColor = Color.LightPink;
                    return false;
                }
                lStatus.Text = "The page matches first page of invoice.";
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
                string d = string.IsNullOrWhiteSpace(testFile.Text) ? Settings.General.InputFolder : PathRoutines.GetDirFromPath(testFile.Text);
                FileFilterForm f = new FileFilterForm(d, SerializationRoutines.Json.Deserialize<Regex>(fileFilterRegex.Text));
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void bText_Click(object sender, EventArgs e)
        {
            if (pages == null)
                return;
            TextForm tf = new TextForm(PdfTextExtractor.GetTextFromPage(pages.PdfReader, currentPage));
            tf.ShowDialog();
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

        public Settings.Template EditedTemplate;

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                EditedTemplate = getTemplateFromUI(true);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }
        Settings.Template.PageRotations pagesRotation
        {
            get
            {
                if (pageRotation.SelectedIndex <= 0)
                    return 0;
                else
                    return (Settings.Template.PageRotations)pageRotation.SelectedIndex;
            }
        }

        Settings.Template getTemplateFromUI(bool savingValidation)
        {
            Settings.Template t = new Settings.Template();

            if (string.IsNullOrWhiteSpace(name.Text))
                if(savingValidation)
                throw new Exception("Name is empty!");

            if (invoiceFirstPageRecognitionMarks.Rows.Count < 2)
                if (savingValidation)
                    throw new Exception("InvoiceFirstPageRecognitionMarks is empty!");

            t.Name = name.Text;

            t.FloatingAnchors = new List<Settings.Template.FloatingAnchor>();
            foreach (DataGridViewRow r in floatingAnchors.Rows)
                if (r.Cells["Id3"].Value != null)
                {
                    t.FloatingAnchors.Add(new Settings.Template.FloatingAnchor
                    {
                        Id = (int)r.Cells["Id3"].Value,
                        ValueType = (Settings.Template.ValueTypes)r.Cells["ValueType3"].Value,
                        Value = (string)r.Cells["Value3"].Value,
                    });
                }
            t.FloatingAnchors = t.FloatingAnchors.OrderBy(a => a.Id).ToList();

            t.InvoiceFirstPageRecognitionMarks = new List<Settings.Template.Mark>();
            foreach (DataGridViewRow r in invoiceFirstPageRecognitionMarks.Rows)
                if (!string.IsNullOrWhiteSpace((string)r.Cells["Rectangle2"].Value))
                {
                    Settings.Template.Mark m = new Settings.Template.Mark
                    {
                        Rectangle = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)r.Cells["Rectangle2"].Value),
                        ValueType = (Settings.Template.ValueTypes)r.Cells["ValueType2"].Value,
                        Value = (string)r.Cells["Value2"].Value,
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId2"].Value,
                    };
                    if (m.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == m.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + m.FloatingAnchorId);
                    t.InvoiceFirstPageRecognitionMarks.Add(m);
                }

            t.Fields = new List<Settings.Template.Field>();
            foreach (DataGridViewRow r in fields.Rows)
                if (!string.IsNullOrWhiteSpace((string)r.Cells["Name_"].Value))
                {
                    Settings.Template.Field f = new Settings.Template.Field
                    {
                        Name = ((string)r.Cells["Name_"].Value).Trim(),
                        Rectangle = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)r.Cells["Rectangle"].Value),
                        ValueType = Convert.ToBoolean(r.Cells["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText,
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId"].Value
                    };
                    if (f.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == f.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + f.FloatingAnchorId);
                    t.Fields.Add(f);
                }

            //if (pageRotation.SelectedIndex <= 0)
            //    template.PageRotationRules = null;
            //else
            //    template.PageRotationRules = new List<Settings.Template.PageRotationRule> { new Settings.Template.PageRotationRule { Angle = pageRotation.SelectedIndex * 90 } };
            t.PagesRotation = pagesRotation;

            t.AutoDeskew = autoDeskew.Checked;

            t.TestFile = testFile.Text;

            //t.ImageResolution = (int)imageResolution.Value;

            t.TestPictureScale = pictureScale.Value;

            t.FileFilterRegex = SerializationRoutines.Json.Deserialize<Regex>(fileFilterRegex.Text);

            return t;
        }
    }
}