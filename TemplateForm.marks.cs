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
        void initializeMarksTable()
        {
            Type2.ValueType = typeof(Template.Types);
            Type2.DataSource = Enum.GetValues(typeof(Template.Types));

            FloatingAnchorId2.ValueType = typeof(int);
            FloatingAnchorId2.ValueMember = "Id";
            FloatingAnchorId2.DisplayMember = "Name";

            documentFirstPageRecognitionMarks.EnableHeadersVisualStyles = false;//needed to set row headers

            documentFirstPageRecognitionMarks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                DataGridViewRow row = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                Template.Mark fa = (Template.Mark)row.Tag;
                if (fa == null)
                    fa = new Template.Mark.PdfText();
                row.Cells["Type2"].Value = fa.Type;
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
                    if (e.ColumnIndex < 0)//row's header
                        return;

                    documentFirstPageRecognitionMarks.BackgroundColor = SystemColors.ControlDark;

                    DataGridViewRow row = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                    var cs = row.Cells;
                    Template.Mark m = (Template.Mark)row.Tag;
                    switch (documentFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                    {
                        case "Type2":
                            {
                                Template.Mark m2;
                                Template.Types t = (Template.Types)cs["Type2"].Value;
                                switch (t)
                                {
                                    case Template.Types.PdfText:
                                        m2 = new Template.Mark.PdfText();
                                        break;
                                    case Template.Types.OcrText:
                                        m2 = new Template.Mark.OcrText();
                                        break;
                                    case Template.Types.ImageData:
                                        m2 = new Template.Mark.ImageData();
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + t);
                                }
                                m2.FloatingAnchorId = m.FloatingAnchorId;
                                m2.Rectangle = m.Rectangle;
                                m = m2;

                                if (m.FloatingAnchorId == null && m.Rectangle == null)
                                {
                                    setRowStatus(statuses.NEUTRAL, row, "");
                                    setMarkRow(row, m);
                                    return;
                                }
                                object v = extractValueAndDrawSelectionBox(m.FloatingAnchorId, m.Rectangle, t);
                                if (v == null)
                                {
                                    setRowStatus(statuses.ERROR, row, "Not found");
                                    setMarkRow(row, m);
                                    return;
                                }
                                else
                                    setRowStatus(statuses.SUCCESS, row, "Set");
                                switch (t)
                                {
                                    case Template.Types.PdfText:
                                        {
                                            Template.Mark.PdfText ptv = (Template.Mark.PdfText)m;
                                            ptv.Text = (string)v;
                                            setMarkRow(row, ptv);
                                        }
                                        break;
                                    case Template.Types.OcrText:
                                        {
                                            Template.Mark.OcrText otv = (Template.Mark.OcrText)m;
                                            otv.Text = (string)v;
                                            setMarkRow(row, otv);
                                        }
                                        break;
                                    case Template.Types.ImageData:
                                        {
                                            Template.Mark.ImageData idv = (Template.Mark.ImageData)m;
                                            idv.ImageData_ = (ImageData)v;
                                            setMarkRow(row, idv);
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + t);
                                }
                            }
                            return;
                        case "FloatingAnchorId2":
                            m.FloatingAnchorId = (int?)cs["FloatingAnchorId2"].Value;
                            setMarkRow(row, m);
                            //setRowStatus(statuses.WARNING, row, "Changed");
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

                    setCurrentMarkFromControl();

                    if (documentFirstPageRecognitionMarks.SelectedRows.Count < 1)
                    {
                        setMarkControl(null);
                        return;
                    }
                    floatingAnchors.ClearSelection();
                    floatingAnchors.CurrentCell = null;
                    fields.ClearSelection();
                    fields.CurrentCell = null;
                    DataGridViewRow row = documentFirstPageRecognitionMarks.SelectedRows[0];

                    if (row.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int j = documentFirstPageRecognitionMarks.Rows.Add();
                        documentFirstPageRecognitionMarks.Rows[j].Selected = true;
                        return;
                    }

                    setMarkControl(row);
                    isMarkFound(row, true);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };
        }

        bool isMarkFound(DataGridViewRow row, bool selectAnchor)
        {
            var cs = row.Cells;
            var vt = (Template.Types)cs["Type2"].Value;
            int? fai = (int?)cs["FloatingAnchorId2"].Value;
            if (selectAnchor)
                setCurrentFloatingAnchorRow(fai, false);
            Template.RectangleF r = getMarkRectangle(row);
            if (r != null)
            {
                switch (vt)
                {
                    case Template.Types.PdfText:
                        {
                            Template.Mark.PdfText ptv1 = (Template.Mark.PdfText)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (ptv1.Text != t2)
                            {
                                setRowStatus(statuses.ERROR, row, "Not found:!'" + t2 + "'");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    case Template.Types.OcrText:
                        {
                            Template.Mark.OcrText otv1 = (Template.Mark.OcrText)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (otv1.Text != t2)
                            {
                                setRowStatus(statuses.ERROR, row, "Not found:!'" + t2 + "'");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    case Template.Types.ImageData:
                        {
                            Template.Mark.ImageData idv1 = (Template.Mark.ImageData)row.Tag;
                            ImageData id2 = (ImageData)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (!idv1.ImageData_.ImageIsSimilar(id2, idv1.BrightnessTolerance, idv1.DifferentPixelNumberTolerance))
                            {
                                setRowStatus(statuses.ERROR, row, "Not found");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    default:
                        throw new Exception("Unknown option: " + vt);
                }
            }
            else
            {
                if (fai != null)
                {
                    if (null != findAndDrawFloatingAnchor(fai))
                    {
                        setRowStatus(statuses.SUCCESS, row, "Found");
                        return true;
                    }
                    setRowStatus(statuses.ERROR, row, "Not found");
                    return false;
                }
                setRowStatus(statuses.NEUTRAL, row, "");
                return true;
            }
        }

        Template.RectangleF getMarkRectangle(DataGridViewRow row)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (m != null)
                return m.Rectangle;
            return null;
        }

        void setMarkRectangle(DataGridViewRow row, Template.RectangleF r)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (m == null)
                return;
            if (m.FloatingAnchorId == null && m.Rectangle == null)
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                setMarkRow(row, null);
                return;
            }
            Template.Types vt = (Template.Types)row.Cells["Type2"].Value;
            object v = extractValueAndDrawSelectionBox(m.FloatingAnchorId, r, vt);
            if (v == null)
                setRowStatus(statuses.ERROR, row, "Not found");
            else
                setRowStatus(statuses.SUCCESS, row, "Set");
            switch (vt)
            {
                case Template.Types.PdfText:
                    {
                        Template.Mark.PdfText ptv = (Template.Mark.PdfText)row.Tag;
                        if (ptv == null)
                            ptv = new Template.Mark.PdfText();
                        ptv.Text = (string)v;
                        ptv.Rectangle = r;
                        setMarkRow(row, ptv);
                    }
                    break;
                case Template.Types.OcrText:
                    {
                        Template.Mark.OcrText otv = (Template.Mark.OcrText)row.Tag;
                        if (otv == null)
                            otv = new Template.Mark.OcrText();
                        otv.Text = (string)v;
                        otv.Rectangle = r;
                        setMarkRow(row, otv);
                    }
                    break;
                case Template.Types.ImageData:
                    {
                        Template.Mark.ImageData idv = (Template.Mark.ImageData)row.Tag;
                        if (idv == null)
                            idv = new Template.Mark.ImageData();
                        idv.ImageData_ = (ImageData)v;
                        idv.Rectangle = r;
                        setMarkRow(row, idv);
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + vt);
            }
        }

        void setMarkRow(DataGridViewRow row, Template.Mark m)
        {
            //string v0;
            //rows2valueString.TryGetValue(row, out v0);
            //rows2valueString[row] = SerializationRoutines.Json.Serialize(value);
            row.Tag = m;
            if (loadingTemplate)
                return;
            //if (v0 == SerializationRoutines.Json.Serialize(value))
            //    return;

            setMarkControl(row);
            //if (row.Selected)
            //    if (isMarkFound(row, true))
            //        setRowStatus(statuses.SUCCESS, row, "Found");
            //    else
            //        setRowStatus(statuses.ERROR, row, "Not found");
        }

        void setMarkControl(DataGridViewRow row)
        {
            if (row == null || !row.Selected || row.IsNewRow || !documentFirstPageRecognitionMarks.Rows.Contains(row))
            {
                currentMarkRow = null;
                currentMarkControl = null;
                return;
            }
            currentMarkRow = row;
            Template.Types valueType = (Template.Types)row.Cells["Type2"].Value;
            Control c = currentMarkControl;
            switch (valueType)
            {
                case Template.Types.PdfText:
                    {
                        if (c == null || !(c is MarkPdfTextControl))
                            c = new MarkPdfTextControl();
                        ((MarkPdfTextControl)c).Mark = (Template.Mark.PdfText)row.Tag;
                    }
                    break;
                case Template.Types.OcrText:
                    {
                        if (c == null || !(c is MarkOcrTextControl))
                            c = new MarkOcrTextControl();
                        ((MarkOcrTextControl)c).Mark = (Template.Mark.OcrText)row.Tag;
                    }
                    break;
                case Template.Types.ImageData:
                    {
                        if (c == null || !(c is MarkImageDataControl))
                            c = new MarkImageDataControl();
                        ((MarkImageDataControl)c).Mark = (Template.Mark.ImageData)row.Tag;
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            currentMarkControl = c;
        }

        Control currentMarkControl
        {
            get
            {
                if (markControl.Controls.Count < 1)
                    return null;
                return markControl.Controls[0];
            }
            set
            {
                markControl.Controls.Clear();
                if (value == null)
                    return;
                markControl.Controls.Add(value);
                value.Dock = DockStyle.Fill;
            }
        }
        DataGridViewRow currentMarkRow = null;

        void setCurrentMarkFromControl()
        {
            if (currentMarkRow == null || currentMarkRow.Index < 0)//removed row
                return;
            //    var cs = currentfloatingAnchorRow.Cells;
            Template.Mark mark = null;
            Template.Types valueType = (Template.Types)currentMarkRow.Cells["Type2"].Value;
            switch (valueType)
            {
                case Template.Types.PdfText:
                    mark = ((MarkPdfTextControl)currentMarkControl).Mark;
                    break;
                case Template.Types.OcrText:
                    mark = ((MarkOcrTextControl)currentMarkControl).Mark;
                    break;
                case Template.Types.ImageData:
                    mark = ((MarkImageDataControl)currentMarkControl).Mark;
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            if (currentMarkRow.Cells["FloatingAnchorId2"].Value == null && mark.Rectangle == null)
                setRowStatus(statuses.WARNING, currentMarkRow, "Not set");
            setMarkRow(currentMarkRow, mark);
        }
    }
}