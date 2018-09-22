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
        void initializeFloatingAnchorsTable()
        {
            Id3.ValueType = typeof(int);
            Type3.ValueType = typeof(Template.Types);
            Type3.DataSource = Enum.GetValues(typeof(Template.Types));

            floatingAnchors.EnableHeadersVisualStyles = false;//needed to set row headers

            floatingAnchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                DataGridViewRow row = floatingAnchors.Rows[e.RowIndex];
                Template.FloatingAnchor fa = (Template.FloatingAnchor)row.Tag;
                if (fa == null)
                    fa = new Template.FloatingAnchor.PdfText();
                row.Cells["Type3"].Value = fa.Type;
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
                if (e.ColumnIndex < 0)//row's header
                    return;
                var row = floatingAnchors.Rows[e.RowIndex];
                int? fai = (int?)row.Cells["Id3"].Value;
                switch (floatingAnchors.Columns[e.ColumnIndex].Name)
                {
                    case "Id3":
                        {
                            if (fai == null)
                                break;
                            Template.FloatingAnchor fa = (Template.FloatingAnchor)row.Tag;
                            fa.Id = (int)fai;
                            setFloatingAnchorRow(row, fa);
                            break;
                        }
                    case "Type3":
                        {
                            Template.FloatingAnchor fa;
                            switch ((Template.Types)row.Cells["Type3"].Value)
                            {
                                case Template.Types.PdfText:
                                    fa = new Template.FloatingAnchor.PdfText();
                                    break;
                                case Template.Types.OcrText:
                                    fa = new Template.FloatingAnchor.OcrText();
                                    break;
                                case Template.Types.ImageData:
                                    fa = new Template.FloatingAnchor.ImageData();
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + (Template.Types)row.Cells["Type3"].Value);
                            }
                            setFloatingAnchorRow(row, fa);
                            break;
                        }
                }
            };

            floatingAnchors.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (settingCurrentFloatingAnchorRow)
                        return;

                    setCurrentFloatingAnchorFromControl();

                    if (floatingAnchors.SelectedRows.Count < 1)
                    {
                        setFloatingAnchorControl(null);
                        return;
                    }

                    documentFirstPageRecognitionMarks.ClearSelection();
                    documentFirstPageRecognitionMarks.CurrentCell = null;
                    fields.ClearSelection();
                    fields.CurrentCell = null;

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
        }

        void setCurrentFloatingAnchorRow(int? fai, bool unselectMark_SelectField)
        {
            try
            {
                floatingAnchors.ClearSelection();
                if (fai == null)
                {
                    floatingAnchors.CurrentCell = null;
                    return;
                }
                DataGridViewRow row = null;
                foreach (DataGridViewRow ar in floatingAnchors.Rows)
                    if ((int?)ar.Cells["Id3"].Value == fai)
                    {
                        row = ar;
                        break;
                    }
                if (row == null)
                    throw new Exception("FloatingAnchor[Id=" + fai + "] does not exist.");
                settingCurrentFloatingAnchorRow = true;
                floatingAnchors.CurrentCell = floatingAnchors[0, row.Index];
                //floatingAnchors.CurrentCell = row.HeaderCell;
                //row.HeaderCell.Selected = true;
                floatingAnchors.ClearSelection();
                settingCurrentFloatingAnchorRow = false;

                setFloatingAnchorControl(row);
                findAndDrawFloatingAnchor((int?)row.Cells["Id3"].Value);
            }
            finally
            {
            }
        }
        bool settingCurrentFloatingAnchorRow = false;

        void setFloatingAnchorRow(DataGridViewRow row, Template.FloatingAnchor fa)
        {
            //string v0;
            //rows2valueString.TryGetValue(row, out v0);
            //rows2valueString[row] = SerializationRoutines.Json.Serialize(value);
            row.Tag = fa;
            if (loadingTemplate)
                return;
            //if (v0 == SerializationRoutines.Json.Serialize(value))
            //    return;
            int? fai = (int?)row.Cells["Id3"].Value;
            onFloatingAnchorsChanged(fai);
            setFloatingAnchorControl(row);
            if (row.Selected)
                findAndDrawFloatingAnchor(fai);
        }
        //Dictionary<DataGridViewRow, string> rows2valueString = new Dictionary<DataGridViewRow, string>();

        void setFloatingAnchorControl(DataGridViewRow row)
        {
            if (row == null || floatingAnchors.CurrentCell == null || row.Index != floatingAnchors.CurrentCell.RowIndex || row.IsNewRow || !floatingAnchors.Rows.Contains(row))
            {
                currentFloatingAnchorRow = null;
                currentFloatingAnchorControl = null;
                return;
            }
            currentFloatingAnchorRow = row;
            Template.Types valueType = (Template.Types)row.Cells["Type3"].Value;
            Control c = currentFloatingAnchorControl;
            switch (valueType)
            {
                case Template.Types.PdfText:
                    {
                        if (c == null || !(c is FloatingAnchorPdfTextControl))
                            c = new FloatingAnchorPdfTextControl();
                        ((FloatingAnchorPdfTextControl)c).FloatingAnchor = (Template.FloatingAnchor.PdfText)row.Tag;
                    }
                    break;
                case Template.Types.OcrText:
                    {
                        if (c == null || !(c is FloatingAnchorOcrTextControl))
                            c = new FloatingAnchorOcrTextControl();
                        ((FloatingAnchorOcrTextControl)c).FloatingAnchor = (Template.FloatingAnchor.OcrText)row.Tag;
                    }
                    break;
                case Template.Types.ImageData:
                    {
                        if (c == null || !(c is FloatingAnchorImageDataControl))
                            c = new FloatingAnchorImageDataControl();
                        ((FloatingAnchorImageDataControl)c).FloatingAnchor = (Template.FloatingAnchor.ImageData)row.Tag;
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            currentFloatingAnchorControl = c;
        }
        Control currentFloatingAnchorControl
        {
            get
            {
                if (anchorControl.Controls.Count < 1)
                    return null;
                return anchorControl.Controls[0];
            }
            set
            {
                anchorControl.Controls.Clear();
                if (value == null)
                    return;
                anchorControl.Controls.Add(value);
                value.Dock = DockStyle.Fill;
            }
        }
        DataGridViewRow currentFloatingAnchorRow = null;

        void setCurrentFloatingAnchorFromControl()
        {
            if (currentFloatingAnchorRow == null || currentFloatingAnchorRow.Index < 0)//removed row
                return;
            Template.FloatingAnchor fa = null;
            Template.Types valueType = (Template.Types)currentFloatingAnchorRow.Cells["Type3"].Value;
            switch (valueType)
            {
                case Template.Types.PdfText:
                    fa = ((FloatingAnchorPdfTextControl)currentFloatingAnchorControl).FloatingAnchor;
                    break;
                case Template.Types.OcrText:
                    fa = ((FloatingAnchorOcrTextControl)currentFloatingAnchorControl).FloatingAnchor;
                    break;
                case Template.Types.ImageData:
                    fa = ((FloatingAnchorImageDataControl)currentFloatingAnchorControl).FloatingAnchor;
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            setFloatingAnchorRow(currentFloatingAnchorRow, fa);
        }

        void onFloatingAnchorsChanged(int? updatedFloatingAnchorId)
        {
            SortedSet<int> fais = new SortedSet<int>();
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value != null)
                    fais.Add((int)rr.Cells["Id3"].Value);

            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value == null && rr.Tag != null && rr.Cells["Type3"].Value != null)
                {
                    int fai = 1;
                    //if (fais.Count > 0)
                    //    fai = fais.Max() + 1;                    
                    foreach (int i in fais)
                    {
                        if (fai < i)
                            break;
                        if (fai == i)
                            fai++;
                    }
                    fais.Add(fai);
                    rr.Cells["Id3"].Value = fai;
                }

            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
            {
                int? i = (int?)r.Cells["FloatingAnchorId2"].Value;
                if (i != null && !fais.Contains((int)i))
                {
                    r.Cells["FloatingAnchorId2"].Value = null;
                    setMarkRectangle(r, null);
                }
                //if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                //    setMarkRectangle(r, null);
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
                //if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                //{
                //    r.Cells["Rectangle"].Value = null;
                //    r.Cells["Value"].Value = null;
                //}
            }

            List<dynamic> fais_ = fais.Select(f => new { Id = f, Name = f.ToString() }).ToList<dynamic>();
            fais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
            FloatingAnchorId2.DataSource = fais_;
            FloatingAnchorId.DataSource = fais_;
        }

        void setFloatingAnchorFromSelectedElements()
        {
            try
            {
                if (floatingAnchors.SelectedRows.Count < 1)
                    return;

                setCurrentFloatingAnchorFromControl();
                DataGridViewRow row = floatingAnchors.SelectedRows[0];
                var t = (Template.Types)row.Cells["Type3"].Value;
                switch (t)
                {
                    case Template.Types.PdfText:
                        {
                            Template.FloatingAnchor.PdfText pt = (Template.FloatingAnchor.PdfText)row.Tag;
                            pt.CharBoxs = new List<Template.FloatingAnchor.PdfText.CharBox>();
                            List<Pdf.Line> lines = Pdf.RemoveDuplicatesAndGetLines(selectedPdfCharBoxs, false);
                            if (lines.Count < 1)
                            {
                                setFloatingAnchorRow(row, pt);
                                return;
                            }
                            foreach (Pdf.Line l in lines)
                                foreach (Pdf.CharBox cb in l.CharBoxes)
                                    pt.CharBoxs.Add(new Template.FloatingAnchor.PdfText.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            if (pt.CharBoxs.Count < 1)
                            {
                                setFloatingAnchorRow(row, null);
                                return;
                            }
                            setFloatingAnchorRow(row, pt);
                        }
                        break;
                    case Template.Types.OcrText:
                        {
                            Template.FloatingAnchor.OcrText ot = (Template.FloatingAnchor.OcrText)row.Tag;
                            ot.CharBoxs = new List<Template.FloatingAnchor.OcrText.CharBox>();

                            List<Ocr.Line> lines = PdfDocumentParser.Ocr.GetLines(selectedOcrCharBoxs);
                            if (lines.Count < 1)
                            {
                                setFloatingAnchorRow(row, ot);
                                return;
                            }
                            foreach (Ocr.Line l in lines)
                                foreach (Ocr.CharBox cb in l.CharBoxes)
                                    ot.CharBoxs.Add(new Template.FloatingAnchor.OcrText.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            if (ot.CharBoxs.Count < 1)
                            {
                                setFloatingAnchorRow(row, null);
                                return;
                            }
                            setFloatingAnchorRow(row, ot);
                        }
                        break;
                    case Template.Types.ImageData:
                        {
                            Template.FloatingAnchor.ImageData id = (Template.FloatingAnchor.ImageData)row.Tag;
                            id.ImageBoxs = new List<Template.FloatingAnchor.ImageData.ImageBox>();

                            if (selectedImageBoxs.Count < 1)
                            {
                                setFloatingAnchorRow(row, id);
                                return;
                            }
                            if (selectedImageBoxs.Where(x => x.ImageData == null).FirstOrDefault() != null)
                            {
                                setFloatingAnchorRow(row, id);
                                return;
                            }
                            id.ImageBoxs = selectedImageBoxs;
                            setFloatingAnchorRow(row, id);
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + t);
                }
            }
            finally
            {
                floatingAnchors.EndEdit();
                selectedPdfCharBoxs = null;
                selectedOcrCharBoxs = null;
                selectedImageBoxs = null;
            }
        }
        List<Pdf.CharBox> selectedPdfCharBoxs;
        List<Ocr.CharBox> selectedOcrCharBoxs;
        List<Template.FloatingAnchor.ImageData.ImageBox> selectedImageBoxs;
    }
}