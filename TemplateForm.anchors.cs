﻿//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

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

            Type3.ValueType = typeof(Template.Anchor.Types);
            Type3.DataSource = Enum.GetValues(typeof(Template.Anchor.Types));

            ParentAnchorId3.ValueType = typeof(int);
            ParentAnchorId3.ValueMember = "Id";
            ParentAnchorId3.DisplayMember = "Name";

            Pattern.DefaultCellStyle.NullValue = null;//to avoid error when changing cell type to image

            anchors.EnableHeadersVisualStyles = false;//needed to set row headers

            anchors.CellPainting += delegate (object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;
                if (e.ColumnIndex != -1)
                    return;
                rowStates? rowState = anchors.Rows[e.RowIndex].HeaderCell.Tag as rowStates?;
                if (rowState == null || rowState == rowStates.NULL)
                    return;
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
                string s;
                switch (rowState)
                {
                    case rowStates.Selected:
                        return;
                    case rowStates.Parent:
                        s = "●";
                        break;
                    case rowStates.Condition:
                        s = "▶";
                        break;
                    case rowStates.Linked:
                        return;
                    default:
                        throw new Exception("Unknown option: " + rowState);
                }
                Rectangle r = e.CellBounds;
                r.X += 2;
                TextRenderer.DrawText(e.Graphics, s, e.CellStyle.Font, r, Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                e.Handled = true;
            };

            anchors.CellBeginEdit += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                string cn = anchors.Columns[e.ColumnIndex].Name;
                if (cn == "SearchRectangleMargin")
                {
                    DataGridViewCell c = anchors[e.ColumnIndex, e.RowIndex];
                    if (string.IsNullOrWhiteSpace((string)c.Value) || !int.TryParse((string)c.Value, out int searchRectangleMargin))
                        c.Value = Settings.Constants.InitialSearchRectangleMargin.ToString();
                    return;
                }
                if (cn == "ParentAnchorId3")
                {
                    setAnchorParentAnchorIdList(anchors.Rows[e.RowIndex]);
                    return;
                }
            };

            anchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
            };

            anchors.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = anchors.Rows[e.RowIndex];
                Message.Error("Anchor[Id=" + r.Cells["Id3"].Value + "] has unacceptable value of " + anchors.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message, this);
            };

            anchors.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
                onAnchorsChanged();
            };

            anchors.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                if (anchors.Rows.Count < 3 && anchors.SelectedRows.Count > 0)
                    anchors.SelectedRows[0].Selected = false;//to avoid auto-creating row
            };

            //anchors.CellFormatting += delegate (object sender, DataGridViewCellFormattingEventArgs e)
            //  {
            //      if (anchors.Columns[e.ColumnIndex].Name == "Pattern")
            //          e.FormattingApplied = false;
            //  };

            anchors.CurrentCellDirtyStateChanged += delegate (object sender, EventArgs e)
            {
                if (anchors.IsCurrentCellDirty)
                {
                    //string cn = anchors.Columns[anchors.CurrentCell.ColumnIndex].Name;
                    //if (cn == "SearchRectangleMargin")
                    //    return;
                    anchors.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            anchors.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                //if (loadingTemplate)
                //    return;
                //DataGridViewRow r = anchors.Rows[e.RowIndex];
                //try
                //{
                //    if (r.Tag != null)
                //    {
                //        //if (!string.IsNullOrWhiteSpace((string)r.Cells["SearchRectangleMargin"].Value) && !int.TryParse((string)r.Cells["SearchRectangleMargin"].Value, out int searchRectangleMargin))
                //        //    throw new Exception("SearchRectangleMargin must be a non-negative integer.");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Message.Error2(ex, this);
                //    e.Cancel = true;
                //}
            };

            bool isWithinCellValueChanged = false;
            anchors.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate || isWithinCellValueChanged)
                    return;
                try
                {
                    isWithinCellValueChanged = true;

                    if (e.ColumnIndex < 0)//row's header
                        return;
                    var row = anchors.Rows[e.RowIndex];
                    Template.Anchor a = (Template.Anchor)row.Tag;
                    DataGridViewCell c = anchors[e.ColumnIndex, e.RowIndex];
                    switch (anchors.Columns[e.ColumnIndex].Name)
                    {
                        case "ParentAnchorId3":
                            {
                                a.ParentAnchorId = (int?)row.Cells["ParentAnchorId3"].Value;
                                break;
                            }
                        case "Pattern":
                            return;
                        case "Type3":
                            {
                                Template.Anchor.Types t2 = (Template.Anchor.Types)c.Value;
                                if (t2 == a.Type)
                                    break;
                                Template.Anchor a2;
                                switch (t2)
                                {
                                    case Template.Anchor.Types.PdfText:
                                        a2 = new Template.Anchor.PdfText();
                                        break;
                                    case Template.Anchor.Types.OcrText:
                                        a2 = new Template.Anchor.OcrText();
                                        break;
                                    case Template.Anchor.Types.ImageData:
                                        a2 = new Template.Anchor.ImageData();
                                        break;
                                    case Template.Anchor.Types.CvImage:
                                        a2 = new Template.Anchor.CvImage();
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + t2);
                                }
                                a2.Id = a.Id;
                                a = a2;
                                break;
                            }
                        case "SearchRectangleMargin":
                            {
                                if (string.IsNullOrWhiteSpace((string)c.Value) || !int.TryParse((string)c.Value, out int searchRectangleMargin))
                                {
                                    a.SearchRectangleMargin = -1;
                                    break;
                                }
                                a.SearchRectangleMargin = searchRectangleMargin < 0 ? -1 : searchRectangleMargin;
                                break;
                            }
                    }
                    setAnchorRow(row, a);
                    clearImageFromBoxes();
                    findAndDrawAnchor(a.Id);
                }
                finally
                {
                    isWithinCellValueChanged = false;
                }
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
                    if (a == null)//hacky forcing to commit a newly added row and display the blank row
                    {
                        int i = anchors.Rows.Add();
                        row = anchors.Rows[i];
                        a = templateManager.CreateDefaultAnchor();
                        setAnchorRow(row, a);
                        onAnchorsChanged();
                        row.Selected = true;
                        return;
                    }
                    setCurrentAnchorRow(a.Id, false);
                    showAnchorRowAs(a.Id, rowStates.Selected, true);
                    clearImageFromBoxes();
                    findAndDrawAnchor(a.Id);
                }
                catch (Exception ex)
                {
                    Message.Error2(ex, this);
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

                Template.Anchor a = getAnchor(anchorId, out DataGridViewRow row);
                if (row == null || a == null)
                    throw new Exception("Anchor[Id=" + anchorId + "] does not exist.");
                if (anchors.CurrentCell?.RowIndex != row.Index)
                    anchors.CurrentCell = anchors[0, row.Index];

                if (clearSelection)
                    anchors.ClearSelection();
                else
                {
                    setCurrentConditionRow(null);
                    setCurrentFieldRow(null);
                }

                Template.Anchor.Types t = ((Template.Anchor)row.Tag).Type;
                switch (t)
                {
                    case Template.Anchor.Types.PdfText:
                        {
                            currentAnchorControl = new AnchorPdfTextControl(new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpace_Threshold.Value, IgnoreSourceSpaces = textAutoInsertSpace_IgnoreSourceSpaces.Checked/*, Representative//default*/ });
                        }
                        break;
                    case Template.Anchor.Types.OcrText:
                        {
                            currentAnchorControl = new AnchorOcrTextControl(new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpace_Threshold.Value, IgnoreSourceSpaces = textAutoInsertSpace_IgnoreSourceSpaces.Checked/*, Representative//default*/ });
                        }
                        break;
                    case Template.Anchor.Types.ImageData:
                        {
                            currentAnchorControl = new AnchorImageDataControl((float)pictureScale.Value);
                        }
                        break;
                    case Template.Anchor.Types.CvImage:
                        {
                            currentAnchorControl = new AnchorCvImageControl((float)pictureScale.Value);
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + t);
                }
                currentAnchorControl.Initialize(row, (DataGridViewRow r) => { setAnchorRow(r, a); });
                settingsControlHeader.Text = "Anchor [" + a?.Id + "]:";
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
                foreach (Control c in splitContainer4.Panel2.Controls)
                    if (c is AnchorControl)
                        return c as AnchorControl;
                return null;
            }
            set
            {
                foreach (Control c in splitContainer4.Panel2.Controls)
                {
                    if (c == settingsControlHeader)
                        continue;
                    splitContainer4.Panel2.Controls.Remove(c);
                    c.Dispose();
                }
                if (value == null)
                    return;
                splitContainer4.Panel2.Controls.Add(value);
                value.BringToFront();
                value.Dock = DockStyle.Fill;
            }
        }

        enum rowStates
        {
            NULL,
            Selected,
            Parent,
            Condition,
            Linked,
        }

        void setAnchorRow(DataGridViewRow row, Template.Anchor a)
        {
            row.Tag = a;
            row.Cells["Id3"].Value = a.Id;
            row.Cells["Type3"].Value = a.Type;
            row.Cells["ParentAnchorId3"].Value = a.ParentAnchorId;
            row.Cells["SearchRectangleMargin"].Value = a.SearchRectangleMargin < 0 ? "-" : a.SearchRectangleMargin.ToString();

            DataGridViewCell patternCell = row.Cells["Pattern"];
            if (patternCell.Value != null && patternCell.Value is IDisposable)
                ((IDisposable)patternCell.Value).Dispose();
            if (a is Template.Anchor.CvImage || a is Template.Anchor.ImageData)
            {
                if (!(patternCell is DataGridViewImageCell))
                {
                    patternCell.Dispose();
                    patternCell = new DataGridViewImageCell { ImageLayout = DataGridViewImageCellLayout.NotSet };
                    row.Cells["Pattern"] = patternCell;
                }
            }
            else
            {
                if (patternCell is DataGridViewImageCell)
                {
                    patternCell.Dispose();
                    patternCell = new DataGridViewTextBoxCell { };
                    row.Cells["Pattern"] = patternCell;
                }
            }
            row.Cells["Pattern"].ReadOnly = true;
            switch (a.Type)
            {
                case Template.Anchor.Types.PdfText:
                    {
                        Template.Anchor.PdfText pt = (Template.Anchor.PdfText)a;
                        if (pt.CharBoxs == null)
                        {
                            patternCell.Value = null;
                            break;
                        }
                        StringBuilder sb = new StringBuilder();
                        foreach (var l in Page.GetLines(pt.CharBoxs.Select(x => new Pdf.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), new TextAutoInsertSpace { IgnoreSourceSpaces = textAutoInsertSpace_IgnoreSourceSpaces.Checked, Threshold = (float)textAutoInsertSpace_Threshold.Value/*, Representative*/}, null))
                        {
                            foreach (var cb in l.CharBoxs)
                                sb.Append(cb.Char);
                            sb.Append("\r\n");
                        }
                        patternCell.Value = sb.ToString();
                    }
                    break;
                case Template.Anchor.Types.OcrText:
                    {
                        Template.Anchor.OcrText ot = (Template.Anchor.OcrText)a;
                        if (ot.CharBoxs == null)
                        {
                            patternCell.Value = null;
                            break;
                        }
                        StringBuilder sb = new StringBuilder();
                        foreach (var l in Page.GetLines(ot.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), new TextAutoInsertSpace { IgnoreSourceSpaces = textAutoInsertSpace_IgnoreSourceSpaces.Checked, Threshold = (float)textAutoInsertSpace_Threshold.Value/*, Representative*/}, null))
                        {
                            foreach (var cb in l.CharBoxs)
                                sb.Append(cb.Char);
                            sb.Append("\r\n");
                        }
                        patternCell.Value = sb.ToString();
                    }
                    break;
                case Template.Anchor.Types.ImageData:
                    {
                        Template.Anchor.ImageData id = (Template.Anchor.ImageData)a;
                        Bitmap b = null;
                        if (id.Image != null)
                        {
                            b = id.Image.GetBitmap();
                            Size s = patternCell.Size;
                            if (s.Height < b.Height * pictureScale.Value)
                            {
                                s.Width = int.MaxValue;
                                Win.ImageRoutines.Scale(ref b, s);
                            }
                            else if (pictureScale.Value != 1)
                                Win.ImageRoutines.Scale(ref b, (float)pictureScale.Value);
                        }
                        patternCell.Value = b;
                    }
                    break;
                case Template.Anchor.Types.CvImage:
                    {
                        Template.Anchor.CvImage ci = (Template.Anchor.CvImage)a;
                        Bitmap b = null;
                        if (ci.Image != null)
                        {
                            b = ci.Image.GetBitmap();
                            Size s = patternCell.Size;
                            if (s.Height < b.Height * pictureScale.Value)
                            {
                                s.Width = int.MaxValue;
                                Win.ImageRoutines.Scale(ref b, s);
                            }
                            else if (pictureScale.Value != 1)
                                Win.ImageRoutines.Scale(ref b, (float)pictureScale.Value);
                        }
                        patternCell.Value = b;
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + a.Type);
            }

            Template.PointF p = a.Position;
            if (p != null)
                row.Cells["Position3"].Value = Serialization.Json.Serialize(p);

            if (loadingTemplate)
                return;

            if (currentAnchorControl != null && row == currentAnchorControl.Row)
                setCurrentAnchorRow(a.Id, false);

            setConditionsStatus();
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

            foreach (DataGridViewRow r in fields.Rows)
            {
                if (r.Tag == null)
                    continue;
                Template.Field f = (Template.Field)r.Tag;
                if (f.LeftAnchor != null && !ais.Contains(f.LeftAnchor.Id))
                    r.Cells["LeftAnchorId"].Value = null;
                if (f.TopAnchor != null && !ais.Contains(f.TopAnchor.Id))
                    r.Cells["TopAnchorId"].Value = null;
                if (f.RightAnchor != null && !ais.Contains(f.RightAnchor.Id))
                    r.Cells["RightAnchorId"].Value = null;
                if (f.BottomAnchor != null && !ais.Contains(f.BottomAnchor.Id))
                    r.Cells["BottomAnchorId"].Value = null;
            }

            setConditionsStatus();

            {
                List<dynamic> ais_ = ais.Select(x => new { Id = x, Name = x.ToString() }).ToList<dynamic>();
                ais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
                LeftAnchorId.DataSource = ais_;
                TopAnchorId.DataSource = ais_;
                RightAnchorId.DataSource = ais_;
                BottomAnchorId.DataSource = ais_;
            }
        }

        void showAnchorRowAs(int? anchorId, rowStates rowState, bool resetRows)
        {
            if (resetRows)
                foreach (DataGridViewRow r in anchors.Rows)
                {
                    if (r.HeaderCell.Tag as rowStates? == rowStates.NULL)
                        continue;
                    r.HeaderCell.Tag = rowStates.NULL;
                    anchors.InvalidateCell(r.HeaderCell);
                }
            if (anchorId == null)
                return;
            Template.Anchor a = getAnchor(anchorId, out DataGridViewRow row);
            if (row.HeaderCell.Tag as rowStates? == rowState)
                return;
            row.HeaderCell.Tag = rowState;
            anchors.InvalidateCell(row.HeaderCell);
        }

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