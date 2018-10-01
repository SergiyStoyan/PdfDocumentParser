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

            AnchorId2.ValueType = typeof(int);
            AnchorId2.ValueMember = "Id";
            AnchorId2.DisplayMember = "Name";

            marks.EnableHeadersVisualStyles = false;//needed to set row headers

            marks.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = marks.Rows[e.RowIndex];
                Message.Error("marks[" + r.Index + "] has unacceptable value of " + marks.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            marks.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
            };

            marks.CurrentCellDirtyStateChanged += delegate
            {
                if (marks.IsCurrentCellDirty)
                    marks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            marks.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
            };

            marks.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            marks.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;
                    if (e.ColumnIndex < 0)//row's header
                        return;

                    marks.BackgroundColor = SystemColors.ControlDark;

                    DataGridViewRow row = marks.Rows[e.RowIndex];
                    var cs = row.Cells;
                    Template.Mark m = (Template.Mark)row.Tag;
                    switch (marks.Columns[e.ColumnIndex].Name)
                    {
                        case "Type2":
                            {
                                Template.Mark m2;
                                Template.Types t2 = (Template.Types)cs["Type2"].Value;
                                if (t2 == m.Type)
                                    return;
                                switch (t2)
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
                                        throw new Exception("Unknown option: " + t2);
                                }
                                m2.AnchorId = m.AnchorId;
                                m2.Rectangle = m.Rectangle;
                                m = m2;
                                //currentMarkControl = null;
                                find_setMarkValue(row);
                            }
                            break;
                        case "AnchorId2":
                            m.AnchorId = (int?)cs["AnchorId2"].Value;
                            //setRowStatus(statuses.WARNING, row, "Changed");
                            break;
                    }
                    setMarkRow(row, m);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            marks.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (settingCurrentMarkRow)
                        return;

                    if (marks.SelectedRows.Count < 1)
                    {
                        setCurrentMarkRow(null);
                        return;
                    }
                    DataGridViewRow row = marks.SelectedRows[0];
                    Template.Mark m = (Template.Mark)row.Tag;
                    if (m == null)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = marks.Rows.Add();
                        row = marks.Rows[i];
                        m = new Template.Mark.PdfText();
                        setMarkRow(row, m);
                        row.Selected = true;
                        return;
                    }
                    setCurrentMarkRow(row);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };
        }

        bool isMarkFound(DataGridViewRow row, bool pointAnchor_renewImage)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (pointAnchor_renewImage)
                setCurrentAnchorRow(m.AnchorId, true);
            if (!m.IsSet())
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                return false;
            }
            DataGridViewRow r;
            Template.Anchor fa = getAnchor(m.AnchorId, out r);
            if (fa != null && !fa.IsSet())
            {
                setRowStatus(statuses.ERROR, row, "Error");
                return false;
            }
            if (m.Rectangle != null)
            {
                switch (m.Type)
                {
                    case Template.Types.PdfText:
                        {
                            Template.Mark.PdfText ptv1 = (Template.Mark.PdfText)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(m.AnchorId, m.Rectangle, m.Type, pointAnchor_renewImage);
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
                            string t2 = (string)extractValueAndDrawSelectionBox(m.AnchorId, m.Rectangle, m.Type, pointAnchor_renewImage);
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
                            ImageData id2 = (ImageData)extractValueAndDrawSelectionBox(m.AnchorId, m.Rectangle, m.Type, pointAnchor_renewImage);
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
            if (m.AnchorId != null)
            {
                if (null != findAndDrawAnchor((int)m.AnchorId, pointAnchor_renewImage))
                {
                    setRowStatus(statuses.SUCCESS, row, "Found");
                    return true;
                }
                setRowStatus(statuses.ERROR, row, "Not found");
                return false;
            }
            setRowStatus(statuses.WARNING, row, "Not set");
            return false;
        }

        void setCurrentMarkRow(DataGridViewRow row)
        {
            if (settingCurrentMarkRow)
                return;
            try
            {
                settingCurrentMarkRow = true;

                currentMarkRow = row;

                if (row == null)
                {
                    marks.ClearSelection();
                    marks.CurrentCell = null;
                    currentMarkControl = null;
                    return;
                }

                setCurrentFieldRow(null);

                marks.CurrentCell = marks[0, row.Index];
                Template.Mark m = (Template.Mark)row.Tag;
                //setCurrentAnchorRow(m.AnchorId, true);

                MarkControl c = currentMarkControl;
                switch (m.Type)
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
                        throw new Exception("Unknown option: " + m.Type);
                }
                c.Initialize(row);
                currentMarkControl = c;
                isMarkFound(row, true);
            }
            finally
            {
                settingCurrentMarkRow = false;
            }
        }
        bool settingCurrentMarkRow = false;
        DataGridViewRow currentMarkRow = null;
        MarkControl currentMarkControl
        {
            get
            {
                if (markControl.Controls.Count < 1)
                    return null;
                return (MarkControl)markControl.Controls[0];
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

        void setMarkRectangle(DataGridViewRow row, Template.RectangleF r)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (m == null)
                return;
            m.Rectangle = r;

            find_setMarkValue(row);

            if (row == currentMarkRow)
                setCurrentMarkRow(row);
        }

        void find_setMarkValue(DataGridViewRow row)
        {
            Template.Mark m = (Template.Mark)row.Tag;
            if (m == null)
                return;
            row.Tag = m;
            object v = extractValueAndDrawSelectionBox(m.AnchorId, m.Rectangle, m.Type);
            if (v == null)
                setRowStatus(statuses.ERROR, row, "Not set");
            else
                setRowStatus(statuses.SUCCESS, row, "Set");
            switch (m.Type)
            {
                case Template.Types.PdfText:
                    {
                        Template.Mark.PdfText ptv = (Template.Mark.PdfText)m;
                        ptv.Text = (string)v;
                    }
                    break;
                case Template.Types.OcrText:
                    {
                        Template.Mark.OcrText otv = (Template.Mark.OcrText)m;
                        otv.Text = (string)v;
                    }
                    break;
                case Template.Types.ImageData:
                    {
                        Template.Mark.ImageData idv = (Template.Mark.ImageData)m;
                        idv.ImageData_ = (ImageData)v;
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + m.Type);
            }
        }

        void setMarkRow(DataGridViewRow row, Template.Mark m)
        {
            row.Tag = m;
            row.Cells["Type2"].Value = m.Type;
            row.Cells["AnchorId2"].Value = m.AnchorId;

            if (loadingTemplate)
                return;

            if (row == currentMarkRow)
                setCurrentMarkRow(row);
        }
    }
}