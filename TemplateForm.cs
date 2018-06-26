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
        public TemplateForm()
        {
            InitializeComponent();

            Icon = AssemblyRoutines.GetAppIcon();
            Text = "Template Manager";

            ValueType2.ValueType = typeof(Settings.Template.ValueTypes);
            ValueType2.DataSource = Enum.GetValues(typeof(Settings.Template.ValueTypes));

            invoiceFirstPageRecognitionMarks.CurrentCellDirtyStateChanged += delegate
            {
                if (invoiceFirstPageRecognitionMarks.IsCurrentCellDirty)
                    invoiceFirstPageRecognitionMarks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            Shown += delegate
            {
                setTemplate();
            };

            FormClosed += delegate
            {
                if (pdfReader != null)
                {
                    pdfReader.Close();
                    pdfReader = null;
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
            };

            //KeyDown += delegate (object sender, KeyEventArgs e)
            //{
            //    if (e.Control)
            //        selectedBoxTexts = new HashSet<BoxText>();
            //};         

            //KeyUp += delegate (object sender, KeyEventArgs e)
            //{
            //    if ((e.Modifiers & Keys.Control) != Keys.Control)
            //        saveSelectedBoxTexts();
            //};

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

                            if (selectedBoxTexts == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                selectedBoxTexts = new List<Pdf.BoxText>();
                            RectangleF selectedR = new RectangleF(p1, new SizeF(p2.X - p1.X, p2.Y - p1.Y));
                            selectedBoxTexts.AddRange(pageCharBoxListss[currentPage].Where(a => /*selectedR.IntersectsWith(a.R) || */selectedR.Contains(a.R)));

                            if ((ModifierKeys & Keys.Control) != Keys.Control)
                                saveSelectedBoxTexts();



                            break;
                        case Modes.SetInvoiceFirstPageRecognitionTextMarks:
                        {
                            if (invoiceFirstPageRecognitionMarks.SelectedRows.Count < 1)
                                break;
                            DataGridViewRow rr = invoiceFirstPageRecognitionMarks.SelectedRows[0];
                            r.X -= point0.X;
                            r.Y -= point0.Y;
                            rr.Cells["Rectangle2"].Value = SerializationRoutines.Json.Serialize(r);
                            invoiceFirstPageRecognitionMarks.EndEdit();
                        }
                        break;
                    case Modes.SetFieldRectangle:
                        {
                            if (fields.SelectedRows.Count < 1)
                                break;
                            fields.SelectedRows[0].Cells["FloatingAnchor"].Value = null;
                            r.X -= point0.X;
                            r.Y -= point0.Y;
                            fields.SelectedRows[0].Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
                            fields.EndEdit();
                        }
                        break;
                    default:
                        break;
                }
            };

            fields.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate)
                    return;

                switch (fields.Columns[e.ColumnIndex].Name)
                {
                    case "Rectangle":
                    case "Ocr":
                    case "FloatingAnchor":
                        string rs = (string)fields.Rows[e.RowIndex].Cells["Rectangle"].Value;
                        if (!string.IsNullOrWhiteSpace(rs))
                        {
                            Settings.Template.FloatingAnchor fa = null;
                            string fas = (string)fields.Rows[e.RowIndex].Cells["FloatingAnchor"].Value;
                            if (!string.IsNullOrWhiteSpace(fas))
                                fa = SerializationRoutines.Json.Deserialize<Settings.Template.FloatingAnchor>(fas);
                            Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
                            r.X += point0.X;
                            r.Y += point0.Y;
                            fields.Rows[e.RowIndex].Cells["Value"].Value = exctractValueAndDrawBox(fa, r, Convert.ToBoolean(fields.Rows[e.RowIndex].Cells["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText);
                        }
                        break;
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
                        Message.Error2("Rectangle", ex);
                        e.Cancel = true;
                    }
                }
                else if (e.ColumnIndex == fields.Columns["Name_"].Index)
                {
                    if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                    {
                        Message.Error("Name cannot be empty!");
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
                    Message.Error2("Name", ex);
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
                    Message.Error2("Rectangle", ex);
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
                        cSetPosition0Anchor.Checked = false;
                        invoiceFirstPageRecognitionMarks.ClearSelection();
                        int i = fields.SelectedRows[0].Index;
                        string rs = (string)fields.Rows[i].Cells["Rectangle"].Value;
                        if (!string.IsNullOrWhiteSpace(rs))
                        {
                            Settings.Template.FloatingAnchor fa = null;
                            string fas = (string)fields.Rows[i].Cells["FloatingAnchor"].Value;
                            if (!string.IsNullOrWhiteSpace(fas))
                                fa = SerializationRoutines.Json.Deserialize<Settings.Template.FloatingAnchor>(fas);
                            Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
                            r.X += point0.X;
                            r.Y += point0.Y;
                            fields.Rows[i].Cells["Value"].Value = exctractValueAndDrawBox(fa, r, Convert.ToBoolean(fields.Rows[i].Cells["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText);
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
                    if (pdfReader != null)
                    {
                        pdfReader.Close();
                        pdfReader = null;
                    }
                    if (scaledCurrentPageBitmap != null)
                    {
                        scaledCurrentPageBitmap.Dispose();
                        scaledCurrentPageBitmap = null;
                    }
                    if (pageBitmaps != null)
                    {
                        pageBitmaps.Dispose();
                        pageBitmaps = null;
                    }

                    if (string.IsNullOrWhiteSpace(testFile.Text))
                        return;

                    if (!File.Exists(testFile.Text))
                    {
                        Message.Error("File '" + testFile.Text + "' does not exist!");
                        return;
                    }

                    pageBitmaps = new BitmapCollection(delegate (int page_i)
                    {
                        return BitmapCollection.GetPageBitmap(testFile.Text, page_i, pagesRotation, autoDeskew.Checked);
                    });
                    pdfReader = Pdf.CreatePdfReader(testFile.Text);

                    pageCharBoxListss = new CharBoxCollection(delegate (int page_i)
                    {
                        var bts = pdfReader.GetCharacterTextChunks(page_i).Select(x => new BoxText
                        {
                            R = new RectangleF
                            {
                                X = x.StartLocation[Vector.I1],
                                Y = pdfReader.GetPageSize(page_i).Height - x.EndLocation[Vector.I2],
                                Width = x.EndLocation[Vector.I1] - x.StartLocation[Vector.I1],
                                Height = x.EndLocation[Vector.I2] - x.StartLocation[Vector.I2],
                            },
                            Text = x.Text
                        });
                        //foreach(BoxText bt in bts.OrderBy(a=>a.R.Y))
                        //     if()
                        return bts.ToList();
                    });

                    totalPageNumber = pdfReader.NumberOfPages;
                    lTotalPages.Text = " / " + totalPageNumber;
                    showPage(1);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            invoiceFirstPageRecognitionMarks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
            };

            invoiceFirstPageRecognitionMarks.CellContentClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                switch (invoiceFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                {
                    case "ValueType2":
                        invoiceFirstPageRecognitionMarks.EndEdit();
                        break;
                }
            };

            invoiceFirstPageRecognitionMarks.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate)
                    return;

                switch (invoiceFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                {
                    case "Rectangle2":
                    case "ValueType2":
                        string rs = (string)invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["Rectangle2"].Value;
                        if (rs != null)
                        {
                            object o = invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["ValueType2"].Value;
                            if (o != null)
                            {
                                Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
                                r.X += point0.X;
                                r.Y += point0.Y;
                                invoiceFirstPageRecognitionMarks.Rows[e.RowIndex].Cells["Value2"].Value = exctractValueAndDrawBox(null, r, (Settings.Template.ValueTypes)o);
                            }
                        }
                        break;
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
                        cSetPosition0Anchor.Checked = false;
                        fields.ClearSelection();
                        int i = invoiceFirstPageRecognitionMarks.SelectedRows[0].Index;

                        if (invoiceFirstPageRecognitionMarks.Rows[i].IsNewRow)//hacky forcing commit a newly added row and display the blank row
                        {
                            invoiceFirstPageRecognitionMarks.Rows.Add();
                            invoiceFirstPageRecognitionMarks.Rows[i].Selected = true;
                            return;
                        }

                        string rs = (string)invoiceFirstPageRecognitionMarks.Rows[i].Cells["Rectangle2"].Value;
                        if (string.IsNullOrWhiteSpace(rs))
                            return;
                        Settings.Template.RectangleF r = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
                        //invoiceFirstPageRecognitionMarks.Rows[i].Cells["Value2"].Value = exctractTextAndDrawBox(r, Convert.ToBoolean(invoiceFirstPageRecognitionMarks.Rows[i].Cells["Ocr2"].Value));
                        drawBox(Settings.General.SelectionBoxColor, r.X, r.Y, r.Width, r.Height, true);
                    }
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
                    Message.Error2("FileFilterRegex", ex);
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

        string exctractValueAndDrawBox(Settings.Template.FloatingAnchor fa, Settings.Template.RectangleF r, Settings.Template.ValueTypes valueType, bool renewImage = true)
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
                    x += bts[0].R.X;
                    y += bts[0].R.Y;
                    renewImage = false;
                }

                drawBox(Settings.General.SelectionBoxColor, x, y, r.Width, r.Height, renewImage);
                switch (valueType)
                {
                    case Settings.Template.ValueTypes.PdfText:
                        if (pdfReader != null)
                            //return FieldPreparation.Normalize(pdfReader.ExtractText(currentPage, x, pdfReader.GetPageSize(currentPage).Height - y - r.Height, r.Width, r.Height));
                            return FieldPreparation.Normalize(Pdf.GetTextByTopLeftCoordinates(pageCharBoxListss[currentPage], x, y, r.Width, r.Height));
                        return null;
                    case Settings.Template.ValueTypes.OcrText:
                        return FieldPreparation.Normalize(TesseractW.This.GetText(pageBitmaps.Get(currentPage), x / Settings.General.Image2PdfResolutionRatio, y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio));
                    case Settings.Template.ValueTypes.ImageData:
                        Settings.Template.RectangleF r_ = new Settings.Template.RectangleF(x / Settings.General.Image2PdfResolutionRatio, y / Settings.General.Image2PdfResolutionRatio, r.Width / Settings.General.Image2PdfResolutionRatio, r.Height / Settings.General.Image2PdfResolutionRatio);
                        ImageData id = new ImageData(pageBitmaps.Get(currentPage, r_));
                        return id.GetAsString();
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }
            catch (Exception ex)
            {
                Message.Error2("Rectangle", ex);
            }
            return null;
        }

        void drawBoxes(Color c, IEnumerable<System.Drawing.RectangleF> rs, bool renewImage = true)
        {
            if (pageBitmaps == null)
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
            if (pageBitmaps == null)
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

        void setFloatingAnchor()
        {
            if (selectedBoxTexts == null)
                return;
            selectedBoxTexts = Pdf.RemoveDuplicatesAndOrder(selectedBoxTexts);
            if (selectedBoxTexts.Count < 1)
                return;
            if (floatingAnchors.SelectedRows.Count < 1)
                return;
            DataGridViewRow r = floatingAnchors.SelectedRows[0];
            string rs = (string)r.Cells["Rectangle3"].Value;
            if (string.IsNullOrWhiteSpace(rs))
                return;
            Settings.Template.FloatingAnchor fa = new Settings.Template.FloatingAnchor();
            fa.Elements = new List<Settings.Template.FloatingAnchor.Element>();
            fa.Elements.Add(new Settings.Template.FloatingAnchor.Element {
                ElementType = (Settings.Template.ValueTypes)r.Cells["ValueType3"].Value,
                SerializedElement =





                selectedBoxTexts.Select(a => new Settings.Template.FloatingAnchor.Element
                {
                    Rectangle = new Settings.Template.RectangleF(a.R.X, a.R.Y, a.R.Width, a.R.Height),
                    Text = a.Text
                }).ToList();

            Settings.Template.RectangleF fieldR = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>(rs);
            fieldR.X -= fa.Elements[0].Rectangle.X;
            fieldR.Y -= fa.Elements[0].Rectangle.Y;
            fields.SelectedRows[0].Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(fieldR);
            fields.SelectedRows[0].Cells["FloatingAnchor"].Value = SerializationRoutines.Json.Serialize(fa);
            fields.EndEdit();



            DataGridViewRow rr = floatingAnchors.SelectedRows[0];
            r.X -= point0.X;
            r.Y -= point0.Y;
            rr.Cells["Rectangle3"].Value = SerializationRoutines.Json.Serialize(r);
            floatingAnchors.EndEdit();


            selectedBoxTexts = null;
        }
        List<Pdf.BoxText> selectedBoxTexts;

        void setTemplate()
        {
            try
            {
                loadingTemplate = true;

                Text = "Template Editor";
                name.Text = template.Name;

                //imageResolution.Value = template.ImageResolution;
                
                autoDeskew.Checked = template.AutoDeskew;

                floatingAnchors.Rows.Clear();
                if (template.FloatingAnchors != null)
                {
                    foreach (Settings.Template.FloatingAnchor f in template.FloatingAnchors.Values)
                    {
                        int i = floatingAnchors.Rows.Add();
                        var cs = floatingAnchors.Rows[i].Cells;
                        cs["Id3"].Value = f.Id;
                        cs["ValueType3"].Value = f.ValueType;
                        cs["Value3"].Value = f.Value;
                    }
                }

                invoiceFirstPageRecognitionMarks.Rows.Clear();
                if (template.InvoiceFirstPageRecognitionMarks != null)
                {
                    foreach (Settings.Template.Mark m in template.InvoiceFirstPageRecognitionMarks)
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
                if (template.Fields != null)
                {
                    foreach (Settings.Template.Field f in template.Fields)
                    {
                        int i = fields.Rows.Add();
                        var cs = fields.Rows[i].Cells;
                        cs["Name_"].Value = f.Name;
                        cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
                        cs["Ocr"].Value = f.ValueType == Settings.Template.ValueTypes.PdfText ? false : true;
                        cs["FloatingAnchorId"].Value = f.FloatingAnchorId;
                    }
                }

                if (template.FileFilterRegex != null)
                    fileFilterRegex.Text = SerializationRoutines.Json.Serialize(template.FileFilterRegex);
                else
                    fileFilterRegex.Text = "";

                pictureScale.Value = template.TestPictureScale > 0 ? template.TestPictureScale : 1;

                if (File.Exists(template.TestFile))
                    testFile.Text = template.TestFile;

            }
            finally
            {
                loadingTemplate = false;
            }
        }

        public Settings.Template Template
        {
            set
            {
                template = value;
                if (Visible)
                    setTemplate();
            }
            get
            {
                return template;//must be called after Save!
            }
        }
        Settings.Template template;
        PdfReader pdfReader;
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
            if (!string.IsNullOrWhiteSpace(template.TestFile))
                d.InitialDirectory = PathRoutines.GetDirFromPath(template.TestFile);
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

                Settings.Template t = getCurrentTemplate();
                string error;
                if(!pages[currentPage].IsInvoiceFirstPage(out error))
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
                Message.Error2(ex);
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
                    Message.Error(ex);
                }
            }

            private void bText_Click(object sender, EventArgs e)
            {
                TextForm tf = new TextForm(PdfTextExtractor.GetTextFromPage(pdfReader, currentPage));
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
                    Message.Error("Page is not a number.");
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

            private void save_Click(object sender, EventArgs e)
            {
                try
                {
                    template = getCurrentTemplate();
                    Settings.Templates.Save();

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

            Settings.Template getCurrentTemplate()
            {
                Settings.Template t = new Settings.Template();

                if (string.IsNullOrWhiteSpace(name.Text))
                    throw new Exception("Name is empty!");

                if (invoiceFirstPageRecognitionMarks.Rows.Count < 2)
                    throw new Exception("InvoiceFirstPageRecognitionMarks is empty!");

                t.Name = name.Text;

                t.FloatingAnchors = new Dictionary<int, Settings.Template.FloatingAnchor>();
                foreach (DataGridViewRow r in floatingAnchors.Rows)
                    if (!string.IsNullOrWhiteSpace((string)r.Cells["Id3"].Value))
                    {
                        string fas = (string)r.Cells["FloatingAnchor"].Value;
                        t.FloatingAnchors[(int)r.Cells["Id3"].Value] = new Settings.Template.FloatingAnchor
                        {
                            Id = (int)r.Cells["Id3"].Value,
                            ValueType = (Settings.Template.ValueTypes)r.Cells["ValueType3"].Value,
                            Value = (string)r.Cells["Value3"].Value,
                        };
                    }

                t.InvoiceFirstPageRecognitionMarks = new List<Settings.Template.Mark>();
                foreach (DataGridViewRow r in invoiceFirstPageRecognitionMarks.Rows)
                    if (!string.IsNullOrWhiteSpace((string)r.Cells["Rectangle2"].Value))
                    {
                        t.InvoiceFirstPageRecognitionMarks.Add(
                            new Settings.Template.Mark
                            {
                                Rectangle = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)r.Cells["Rectangle2"].Value),
                                ValueType = (Settings.Template.ValueTypes)r.Cells["ValueType2"].Value,
                                Value = (string)r.Cells["Value2"].Value,
                                FloatingAnchorId = r.Cells["FloatingAnchorId2"].Value == null ? -1 : (int)r.Cells["FloatingAnchorId2"].Value,
                            });
                    }

                t.Fields = new List<Settings.Template.Field>();
                foreach (DataGridViewRow r in fields.Rows)
                    if (!string.IsNullOrWhiteSpace((string)r.Cells["Name_"].Value))
                    {
                        string fas = (string)r.Cells["FloatingAnchor"].Value;
                        t.Fields.Add(new Settings.Template.Field
                        {
                            Name = ((string)r.Cells["Name_"].Value).Trim(),
                            Rectangle = SerializationRoutines.Json.Deserialize<Settings.Template.RectangleF>((string)r.Cells["Rectangle"].Value),
                            ValueType = Convert.ToBoolean(r.Cells["Ocr"].Value) ? Settings.Template.ValueTypes.OcrText : Settings.Template.ValueTypes.PdfText,
                            FloatingAnchorId = r.Cells["FloatingAnchorId"].Value == null ? -1 : (int)r.Cells["FloatingAnchorId"].Value,
                        });
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