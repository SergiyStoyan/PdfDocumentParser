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
            
            documentFirstPageRecognitionMarks.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                Message.Error("documentFirstPageRecognitionMarks[" + r.Index + "] has unacceptable value of " + documentFirstPageRecognitionMarks.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            documentFirstPageRecognitionMarks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
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

                                setMarkRow(row, m);
                                if (!m.IsSet())
                                    return;
                                object v = extractValueAndDrawSelectionBox(m.FloatingAnchorId, m.Rectangle, t);
                                if (v == null)
                                {
                                    setRowStatus(statuses.ERROR, row, "Not found");
                                    return;
                                }
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
                        int i = documentFirstPageRecognitionMarks.Rows.Add();
                        row = documentFirstPageRecognitionMarks.Rows[i];
                        Template.Mark fa = new Template.Mark.PdfText();
                        row.Tag = fa;
                        row.Cells["Type2"].Value = fa.Type;
                        row.Selected = true;
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
            Template.Mark m = (Template.Mark)row.Tag;
            if (selectAnchor)
                setCurrentFloatingAnchorRow(m.FloatingAnchorId, false);
            if (m.Rectangle != null)
            {
                switch (m.Type)
                {
                    case Template.Types.PdfText:
                        {
                            Template.Mark.PdfText ptv1 = (Template.Mark.PdfText)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(m.FloatingAnchorId, m.Rectangle, m.Type);
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
                            string t2 = (string)extractValueAndDrawSelectionBox(m.FloatingAnchorId, m.Rectangle, m.Type);
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
                            ImageData id2 = (ImageData)extractValueAndDrawSelectionBox(m.FloatingAnchorId, m.Rectangle, m.Type);
                            if (!idv1.ImageData_.ImageIsSimilar(id2, idv1.BrightnessTolerance, idv1.DifferentPixelNumberTolerance))
                            {
                                setRowStatus(statuses.ERROR, row, "Not found");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    default:
                        throw new Exception("Unknown option: " + m.Type);
                }
            }
            else
            {
                if (m.FloatingAnchorId != null)
                {
                    if (null != findAndDrawFloatingAnchor(m.FloatingAnchorId))
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

        void setMarkRectangle(DataGridViewRow row, Template.RectangleF r)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (m == null)
                return;
            object v = extractValueAndDrawSelectionBox(m.FloatingAnchorId, r, m.Type);
            if (v == null)
            {
                setRowStatus(statuses.ERROR, row, "Not found");
                return;
            }
            switch (m.Type)
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
                    throw new Exception("Unknown option: " + m.Type);
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
            if (m.IsSet())
                setRowStatus(statuses.SUCCESS, currentMarkRow, "Set");
            else
                setRowStatus(statuses.WARNING, currentMarkRow, "Not set");
        }

        void setMarkControl(DataGridViewRow row)
        {
            if (row == null || !row.Selected || row.IsNewRow || row.Tag==null || !documentFirstPageRecognitionMarks.Rows.Contains(row))
            {
                currentMarkRow = null;
                currentMarkControl = null;
                return;
            }
            currentMarkRow = row;            
            Control c = currentMarkControl;
            Template.Types t = ((Template.Mark)row.Tag).Type;
            switch (t)
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
                    throw new Exception("Unknown option: " + t);
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
            Template.Mark m = (Template.Mark)currentMarkRow.Tag;
            Template.Mark m2 = null;
            switch (m.Type)
            {
                case Template.Types.PdfText:
                    m2 = ((MarkPdfTextControl)currentMarkControl).Mark;
                    break;
                case Template.Types.OcrText:
                    m2 = ((MarkOcrTextControl)currentMarkControl).Mark;
                    break;
                case Template.Types.ImageData:
                    m2 = ((MarkImageDataControl)currentMarkControl).Mark;
                    break;
                default:
                    throw new Exception("Unknown option: " + m.Type);
            }
            setMarkRow(currentMarkRow, m2);
        }
    }
}