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
        void initializeAnchorsTable()
        {
            Id3.ValueType = typeof(int);
            Type3.ValueType = typeof(Template.Types);
            Type3.DataSource = Enum.GetValues(typeof(Template.Types));

            anchors.EnableHeadersVisualStyles = false;//needed to set row headers

            anchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
            };

            anchors.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = anchors.Rows[e.RowIndex];
                Message.Error("Anchor[Id=" + r.Cells["Id3"].Value + "] has unacceptable value of " + anchors.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            anchors.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
                onAnchorsChanged();
            };

            anchors.CurrentCellDirtyStateChanged += delegate
            {
                if (anchors.IsCurrentCellDirty)
                    anchors.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            anchors.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate)
                    return;
                if (e.ColumnIndex < 0)//row's header
                    return;
                var row = anchors.Rows[e.RowIndex];
                Template.Anchor fa = (Template.Anchor)row.Tag;
                switch (anchors.Columns[e.ColumnIndex].Name)
                {
                    //case "Id3":
                    //    {
                    //        int? anchorId = (int?)row.Cells["Id3"].Value;
                    //        if (anchorId == null)
                    //            break;
                    //        Template.Anchor fa = (Template.Anchor)row.Tag;
                    //        fa.Id = (int)anchorId;
                    //        setAnchorRow(row, fa);
                    //        break;
                    //    }
                    case "Type3":
                        {
                            Template.Types t2 = (Template.Types)row.Cells["Type3"].Value;
                            if (t2 == fa.Type)
                                break;
                            Template.Anchor fa2;
                            switch (t2)
                            {
                                case Template.Types.PdfText:
                                    fa2 = new Template.Anchor.PdfText();
                                    break;
                                case Template.Types.OcrText:
                                    fa2 = new Template.Anchor.OcrText();
                                    break;
                                case Template.Types.ImageData:
                                    fa2 = new Template.Anchor.ImageData();
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + t2);
                            }
                            fa2.Id = fa.Id;
                            fa = fa2;
                            currentAnchorControl = null;
                            break;
                        }
                }
                setAnchorRow(row, fa);
            };

            anchors.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (settingCurrentAnchorRow)
                        return;

                    if (anchors.SelectedRows.Count < 1)
                    {
                        setCurrentAnchorRow(null, true);
                        return;
                    }
                    var row = anchors.SelectedRows[0];
                    Template.Anchor fa = (Template.Anchor)row.Tag;
                    if (fa == null)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = anchors.Rows.Add();
                        row = anchors.Rows[i];
                        fa = new Template.Anchor.PdfText();
                        setAnchorRow(row, fa);
                        onAnchorsChanged();
                        row.Selected = true;
                        return;
                    }
                    setCurrentAnchorRow(fa.Id, false);
                    findAndDrawAnchor(fa.Id);
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };
        }

        void setCurrentAnchorRow(int? anchorId, bool clearSelection)
        {
            if (settingCurrentAnchorRow)
                return;
            try
            {
                settingCurrentAnchorRow = true;

                if (anchorId == currentAnchorId)
                    return;
                currentAnchorId = anchorId;

                setCurrentAnchorFromControl();

                if (clearSelection)
                    anchors.ClearSelection();
                else
                {
                    setCurrentFieldRow(null);
                    setCurrentMarkRow(null);
                }

                if (anchorId == null)
                {
                    anchors.ClearSelection();
                    anchors.CurrentCell = null;
                    currentAnchorRow = null;
                    currentAnchorControl = null;
                    return;
                }

                DataGridViewRow row;
                Template.Anchor fa = getAnchor(anchorId, out row);
                if (row == null || fa == null)
                    throw new Exception("Anchor[Id=" + anchorId + "] does not exist.");
                anchors.CurrentCell = anchors[0, row.Index];

                Control c = currentAnchorControl;
                Template.Types t = ((Template.Anchor)row.Tag).Type;
                switch (t)
                {
                    case Template.Types.PdfText:
                        {
                            if (c == null || !(c is AnchorPdfTextControl))
                                c = new AnchorPdfTextControl();
                            ((AnchorPdfTextControl)c).Anchor = (Template.Anchor.PdfText)row.Tag;
                        }
                        break;
                    case Template.Types.OcrText:
                        {
                            if (c == null || !(c is AnchorOcrTextControl))
                                c = new AnchorOcrTextControl();
                            ((AnchorOcrTextControl)c).Anchor = (Template.Anchor.OcrText)row.Tag;
                        }
                        break;
                    case Template.Types.ImageData:
                        {
                            if (c == null || !(c is AnchorImageDataControl))
                                c = new AnchorImageDataControl();
                            ((AnchorImageDataControl)c).Anchor = (Template.Anchor.ImageData)row.Tag;
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + t);
                }
                currentAnchorControl = c;
                currentAnchorRow = row;
            }
            finally
            {
                settingCurrentAnchorRow = false;
            }
        }
        bool settingCurrentAnchorRow = false;
        DataGridViewRow currentAnchorRow = null;
        int? currentAnchorId = null;
        Control currentAnchorControl
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
        void setCurrentAnchorFromControl()
        {
            if (currentAnchorRow == null || currentAnchorControl == null)
                return;
            Template.Anchor fa = (Template.Anchor)currentAnchorRow.Tag;
            switch (fa.Type)
            {
                case Template.Types.PdfText:
                    fa = ((AnchorPdfTextControl)currentAnchorControl).Anchor;
                    break;
                case Template.Types.OcrText:
                    fa = ((AnchorOcrTextControl)currentAnchorControl).Anchor;
                    break;
                case Template.Types.ImageData:
                    fa = ((AnchorImageDataControl)currentAnchorControl).Anchor;
                    break;
                default:
                    throw new Exception("Unknown option: " + fa.Type);
            }
            setAnchorRow(currentAnchorRow, fa);
        }

        void setAnchorRow(DataGridViewRow row, Template.Anchor fa)
        {
            row.Tag = fa;
            row.Cells["Id3"].Value = fa.Id;
            row.Cells["Type3"].Value = fa.Type;

            if (loadingTemplate)
                return;

            if (row == currentAnchorRow)
                setCurrentAnchorRow(fa.Id, false);
        }

        void onAnchorsChanged()
        {
            SortedSet<int> fais = new SortedSet<int>();
            foreach (DataGridViewRow r in anchors.Rows)
                if (r.Tag != null && ((Template.Anchor)r.Tag).Id > 0)
                    fais.Add(((Template.Anchor)r.Tag).Id);

            foreach (DataGridViewRow r in anchors.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Anchor fa = (Template.Anchor)r.Tag;
                if (/*fa.IsSet() &&*/ fa.Id <= 0)
                {
                    fa.Id = 1;
                    //if (fais.Count > 0)
                    //    anchorId = fais.Max() + 1;                    
                    foreach (int i in fais)
                    {
                        if (fa.Id < i)
                            break;
                        if (fa.Id == i)
                            fa.Id++;
                    }
                    fais.Add(fa.Id);
                    r.Cells["Id3"].Value = fa.Id;
                }
            }

            foreach (DataGridViewRow r in marks.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Mark m = (Template.Mark)r.Tag;
                if (m.AnchorId != null && !fais.Contains((int)m.AnchorId))
                {
                    r.Cells["AnchorId2"].Value = null;
                    setMarkRectangle(r, null);
                }
            }
            foreach (DataGridViewRow r in fields.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Field f = (Template.Field)r.Tag;
                if (f.AnchorId != null && !fais.Contains((int)f.AnchorId))
                {
                    r.Cells["AnchorId"].Value = null;
                    r.Cells["Value"].Value = null;
                    setFieldRectangle(r, null);
                }
            }

            List<dynamic> fais_ = fais.Select(f => new { Id = f, Name = f.ToString() }).ToList<dynamic>();
            fais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
            AnchorId2.DataSource = fais_;
            AnchorId.DataSource = fais_;
        }

        void setAnchorFromSelectedElements()
        {
            try
            {
                if (currentAnchorRow == null)
                    return;
                Template.Anchor fa = (Template.Anchor)currentAnchorRow.Tag;
                switch (fa.Type)
                {
                    case Template.Types.PdfText:
                        {
                            Template.Anchor.PdfText pt = (Template.Anchor.PdfText)fa;
                            pt.CharBoxs = new List<Template.Anchor.PdfText.CharBox>();
                            List<Pdf.Line> lines = Pdf.RemoveDuplicatesAndGetLines(selectedPdfCharBoxs, false);
                            if (lines.Count < 1)
                                break;
                            foreach (Pdf.Line l in lines)
                                foreach (Pdf.CharBox cb in l.CharBoxes)
                                    pt.CharBoxs.Add(new Template.Anchor.PdfText.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                        }
                        break;
                    case Template.Types.OcrText:
                        {
                            Template.Anchor.OcrText ot = (Template.Anchor.OcrText)fa;
                            ot.CharBoxs = new List<Template.Anchor.OcrText.CharBox>();
                            List<Ocr.Line> lines = PdfDocumentParser.Ocr.GetLines(selectedOcrCharBoxs);
                            if (lines.Count < 1)
                                break;
                            foreach (Ocr.Line l in lines)
                                foreach (Ocr.CharBox cb in l.CharBoxes)
                                    ot.CharBoxs.Add(new Template.Anchor.OcrText.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                        }
                        break;
                    case Template.Types.ImageData:
                        {
                            Template.Anchor.ImageData id = (Template.Anchor.ImageData)fa;
                            id.ImageBoxs = new List<Template.Anchor.ImageData.ImageBox>();
                            if (selectedImageBoxs.Count < 1)
                                break;
                            if (selectedImageBoxs.Where(x => x.ImageData == null).FirstOrDefault() != null)
                                break;
                            id.ImageBoxs = selectedImageBoxs;
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + fa.Type);
                }
                setAnchorRow(currentAnchorRow, fa);
            }
            finally
            {
                anchors.EndEdit();
                selectedPdfCharBoxs = null;
                selectedOcrCharBoxs = null;
                selectedImageBoxs = null;
            }
        }
        List<Pdf.CharBox> selectedPdfCharBoxs;
        List<Ocr.CharBox> selectedOcrCharBoxs;
        List<Template.Anchor.ImageData.ImageBox> selectedImageBoxs;

        Template.Anchor getAnchor(int? anchorId, out DataGridViewRow row)
        {
            if (anchorId != null)
                foreach (DataGridViewRow r in anchors.Rows)
                {
                    Template.Anchor fa = (Template.Anchor)r.Tag;
                    if (fa != null && fa.Id == anchorId)
                    {
                        row = r;
                        return fa;
                    }
                }
            row = null;
            return null;
        }
    }
}