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

            ParentAnchorId3.ValueType = typeof(int);
            ParentAnchorId3.ValueMember = "Id";
            ParentAnchorId3.DisplayMember = "Name";

            Group3.ValueType = typeof(string);
            if (!templateManager.AnchorGroups.Contains(string.Empty))
                templateManager.AnchorGroups.Insert(0, string.Empty);
            Group3.DataSource = templateManager.AnchorGroups;
            Group3.FlatStyle = FlatStyle.Flat;//to make backcolor visible

            anchors.EnableHeadersVisualStyles = false;//needed to set row headers

            anchors.CellBeginEdit += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                if (anchors.Columns[e.ColumnIndex].Name != "ParentAnchorId3")
                    return;
                setAnchorParentAnchorIdList(anchors.Rows[e.RowIndex]);
            };

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
                Template.Anchor a = (Template.Anchor)row.Tag;
                switch (anchors.Columns[e.ColumnIndex].Name)
                {
                    //case "Id3":
                    //    {
                    //        int? anchorId = (int?)row.Cells["Id3"].Value;
                    //        if (anchorId == null)
                    //            break;
                    //        Template.Anchor a = (Template.Anchor)row.Tag;
                    //        a.Id = (int)anchorId;
                    //        setAnchorRow(row, a);
                    //        break;
                    //    }
                    case "Type3":
                        {
                            Template.Types t2 = (Template.Types)row.Cells["Type3"].Value;
                            if (t2 == a.Type)
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
                            fa2.Id = a.Id;
                            a = fa2;
                            break;
                        }
                    case "ParentAnchorId3":
                        {
                            a.ParentAnchorId = (int?)row.Cells["ParentAnchorId3"].Value;
                            break;
                        }
                    case "Group3":
                        {
                            a.Group = (string)row.Cells["Group3"].Value;
                            break;
                        }
                }
                setAnchorRow(row, a);
                findAndDrawAnchor(a.Id);
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
                    Template.Anchor a = (Template.Anchor)row.Tag;
                    if (a == null)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = anchors.Rows.Add();
                        row = anchors.Rows[i];
                        a = new Template.Anchor.PdfText();
                        setAnchorRow(row, a);
                        onAnchorsChanged();
                        row.Selected = true;
                        return;
                    }
                    setCurrentAnchorRow(a.Id, false);
                    findAndDrawAnchor(a.Id);
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };
        }

        void setAnchorParentAnchorIdList(DataGridViewRow row)
        {
            Template.Anchor currentRowAnchor = (Template.Anchor)row.Tag;
            if (currentRowAnchor == null)
                return;
            SortedSet<int> ais = new SortedSet<int>();
            List<Template.Anchor> as_ = new List<Template.Anchor>();
            foreach (DataGridViewRow r in anchors.Rows)
                if (r.Tag != null)
                {
                    Template.Anchor a = (Template.Anchor)r.Tag;
                    if (a.Id < 1)
                        continue;
                    as_.Add(a);
                    ais.Add(a.Id);
                }
            foreach (Template.Anchor a_ in as_)
                for (Template.Anchor a = a_; a != null; a = as_.FirstOrDefault(x => x.Id == a.ParentAnchorId))
                    if (a.Id == currentRowAnchor.Id)
                    {
                        ais.Remove(a_.Id);
                        break;
                    }
            List<dynamic> ais_ = ais.Select(x => new { Id = x, Name = x.ToString() }).ToList<dynamic>();
            ais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
            DataGridViewComboBoxCell c = row.Cells[anchors.Columns["ParentAnchorId3"].Index] as DataGridViewComboBoxCell;
            c.DataSource = ais_;
        }

        void setCurrentAnchorRow(int? anchorId, bool clearSelection)
        {
            if (settingCurrentAnchorRow)
                return;
            try
            {
                settingCurrentAnchorRow = true;

                if (anchorId == null)
                {
                    anchors.ClearSelection();
                    anchors.CurrentCell = null;
                    currentAnchorControl = null;
                    return;
                }

                DataGridViewRow row;
                Template.Anchor a = getAnchor(anchorId, out row);
                if (row == null || a == null)
                    throw new Exception("Anchor[Id=" + anchorId + "] does not exist.");
                anchors.CurrentCell = anchors[0, row.Index];

                if (clearSelection)
                    anchors.ClearSelection();
                else
                {
                    setCurrentFieldRow(null);
                    setCurrentMarkRow(null);
                }

                Template.Types t = ((Template.Anchor)row.Tag).Type;
                switch (t)
                {
                    case Template.Types.PdfText:
                        {
                            if (currentAnchorControl == null || !(currentAnchorControl is AnchorPdfTextControl))
                                currentAnchorControl = new AnchorPdfTextControl();
                        }
                        break;
                    case Template.Types.OcrText:
                        {
                            if (currentAnchorControl == null || !(currentAnchorControl is AnchorOcrTextControl))
                                currentAnchorControl = new AnchorOcrTextControl();
                        }
                        break;
                    case Template.Types.ImageData:
                        {
                            if (currentAnchorControl == null || !(currentAnchorControl is AnchorImageDataControl))
                                currentAnchorControl = new AnchorImageDataControl();
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + t);
                }
                currentAnchorControl.Initialize(row);
            }
            finally
            {
                settingCurrentAnchorRow = false;
            }
        }
        bool settingCurrentAnchorRow = false;
        AnchorControl currentAnchorControl
        {
            get
            {
                if (anchorControl.Controls.Count < 1)
                    return null;
                return (AnchorControl)anchorControl.Controls[0];
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

        void setAnchorRow(DataGridViewRow row, Template.Anchor a)
        {
            row.Tag = a;
            row.Cells["Id3"].Value = a.Id;
            row.Cells["Type3"].Value = a.Type;
            row.Cells["ParentAnchorId3"].Value = a.ParentAnchorId;
            row.Cells["Group3"].Value = a.Group;

            if (loadingTemplate)
                return;

            if (currentAnchorControl != null && row == currentAnchorControl.Row)
                setCurrentAnchorRow(a.Id, false);

            setAnchorGroupStatuses();
        }

        void setAnchorGroupStatuses()
        {
            pages.ActiveTemplate = getTemplateFromUI(false);
            foreach (DataGridViewRow r in anchors.Rows)
                if (r.Tag != null)
                {
                    DataGridViewComboBoxCell c = r.Cells["Group3"] as DataGridViewComboBoxCell;
                    Template.Anchor a = (Template.Anchor)r.Tag;
                    if (string.IsNullOrWhiteSpace(a.Group))
                        c.Style.BackColor = SystemColors.Control;
                    else
                    {                        
                        if (pages[currentPage].IsAnchorGroupFound(a.Group))
                            c.Style.BackColor = Color.LightGreen;
                        else
                            c.Style.BackColor = Color.Pink;
                        findAndDrawAnchor(a.Id);//to set anchor status
                    }
                }
        }

        void onAnchorsChanged()
        {
            SortedSet<int> ais = new SortedSet<int>();
            List<Template.Anchor> as_ = new List<Template.Anchor>();
            foreach (DataGridViewRow r in anchors.Rows)
                if (r.Tag != null)
                {
                    Template.Anchor a = (Template.Anchor)r.Tag;
                    as_.Add(a);
                    if (a.Id > 0)
                        ais.Add(a.Id);
                }

            foreach (DataGridViewRow r in anchors.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Anchor a = (Template.Anchor)r.Tag;
                if (/*a.IsSet() &&*/ a.Id <= 0)
                {
                    a.Id = 1;
                    //if (ais.Count > 0)
                    //    anchorId = ais.Max() + 1;                    
                    foreach (int i in ais)
                    {
                        if (a.Id < i)
                            break;
                        if (a.Id == i)
                            a.Id++;
                    }
                    ais.Add(a.Id);
                    r.Cells["Id3"].Value = a.Id;
                }
            }

            foreach (DataGridViewRow r in anchors.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Anchor a = (Template.Anchor)r.Tag;
                if (a.ParentAnchorId == null)
                    continue;
                if (!ais.Contains((int)a.ParentAnchorId))
                    r.Cells["ParentAnchorId3"].Value = null;
            }

            foreach (DataGridViewRow r in marks.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Mark m = (Template.Mark)r.Tag;
                if (m.AnchorId != null && !ais.Contains((int)m.AnchorId))
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
                if (f.AnchorId != null && !ais.Contains((int)f.AnchorId))
                {
                    r.Cells["AnchorId"].Value = null;
                    //r.Cells["Value"].Value = null;
                    //setFieldRectangle(r, null);
                }
            }

            {
                List<dynamic> ais_ = ais.Select(x => new { Id = x, Name = x.ToString() }).ToList<dynamic>();
                ais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
                AnchorId2.DataSource = ais_;
                AnchorId.DataSource = ais_;
            }
        }

        void setAnchorFromSelectedElements()
        {
            try
            {
                if (currentAnchorControl == null)
                    return;
                Template.Anchor a = (Template.Anchor)currentAnchorControl.Row.Tag;
                switch (a.Type)
                {
                    case Template.Types.PdfText:
                        {
                            Template.Anchor.PdfText pt = (Template.Anchor.PdfText)a;
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
                            Template.Anchor.OcrText ot = (Template.Anchor.OcrText)a;
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
                            Template.Anchor.ImageData id = (Template.Anchor.ImageData)a;
                            id.ImageBoxs = new List<Template.Anchor.ImageData.ImageBox>();
                            if (selectedImageBoxs.Count < 1)
                                break;
                            if (selectedImageBoxs.Where(x => x.ImageData == null).FirstOrDefault() != null)
                                break;
                            id.ImageBoxs = selectedImageBoxs;
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + a.Type);
                }
                setAnchorRow(currentAnchorControl.Row, a);
                findAndDrawAnchor(a.Id);
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
                    Template.Anchor a = (Template.Anchor)r.Tag;
                    if (a != null && a.Id == anchorId)
                    {
                        row = r;
                        return a;
                    }
                }
            row = null;
            return null;
        }
    }
}