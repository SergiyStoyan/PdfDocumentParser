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
        void setTableHandlers()
        {
            Id3.ValueType = typeof(int);
            ValueType3.ValueType = typeof(Template.ValueTypes);
            ValueType3.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            ValueType2.ValueType = typeof(Template.ValueTypes);
            ValueType2.DataSource = Enum.GetValues(typeof(Template.ValueTypes));

            FloatingAnchorId2.ValueType = typeof(int);
            FloatingAnchorId2.ValueMember = "Id";
            FloatingAnchorId2.DisplayMember = "Name";

            FloatingAnchorId.ValueType = typeof(int);
            FloatingAnchorId.ValueMember = "Id";
            FloatingAnchorId.DisplayMember = "Name";

            floatingAnchors.EnableHeadersVisualStyles = false;//needed to set row headers
            documentFirstPageRecognitionMarks.EnableHeadersVisualStyles = false;//needed to set row headers
            fields.EnableHeadersVisualStyles = false;//needed to set row headers

            floatingAnchors.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
                if (floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value == null)
                    floatingAnchors.Rows[e.RowIndex].Cells["ValueType3"].Value = Template.ValueTypes.PdfText;
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
                        if (row.Selected)
                            findAndDrawFloatingAnchor(fai);
                        break;
                    case "ValueType3":
                        setFloatingAnchorValue(row, null);
                        break;
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

                    setCurrentFloatingAnchorValueFromControl();

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

                    documentFirstPageRecognitionMarks.BackgroundColor = SystemColors.ControlDarkDark;

                    DataGridViewRow row = documentFirstPageRecognitionMarks.Rows[e.RowIndex];
                    var cs = row.Cells;
                    int? fai = (int?)cs["FloatingAnchorId2"].Value;
                    Template.RectangleF r = getMarkRectangle(row);
                    switch (documentFirstPageRecognitionMarks.Columns[e.ColumnIndex].Name)
                    {
                        case "ValueType2":
                            {
                                Template.ValueTypes vt = (Template.ValueTypes)cs["ValueType2"].Value;
                                if (fai == null && r == null)
                                {
                                    setRowStatus(statuses.NEUTRAL, row, "");
                                    setMarkValue(row, null);
                                    return;
                                }
                                object v = extractValueAndDrawSelectionBox(fai, r, vt);
                                if (v == null)
                                {
                                    setRowStatus(statuses.ERROR, row, "Not found");
                                    setMarkValue(row, null);
                                    return;
                                }
                                else
                                    setRowStatus(statuses.SUCCESS, row, "Set");
                                switch (vt)
                                {
                                    case Template.ValueTypes.PdfText:
                                        {
                                            Template.Mark.PdfTextValue ptv = (Template.Mark.PdfTextValue)row.Tag;
                                            if (ptv == null)
                                                ptv = new Template.Mark.PdfTextValue();
                                            ptv.Text = (string)v;
                                            setMarkValue(row, ptv);
                                        }
                                        break;
                                    case Template.ValueTypes.OcrText:
                                        {
                                            Template.Mark.OcrTextValue otv = (Template.Mark.OcrTextValue)row.Tag;
                                            if (otv == null)
                                                otv = new Template.Mark.OcrTextValue();
                                            otv.Text = (string)v;
                                            setMarkValue(row, otv);
                                        }
                                        break;
                                    case Template.ValueTypes.ImageData:
                                        {
                                            Template.Mark.ImageDataValue idv = (Template.Mark.ImageDataValue)row.Tag;
                                            if (idv == null)
                                                idv = new Template.Mark.ImageDataValue();
                                            idv.ImageData = (ImageData)v;
                                            setMarkValue(row, idv);
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + vt);
                                }
                            }
                            return;
                        case "FloatingAnchorId2":
                            setRowStatus(statuses.WARNING, row, "Changed");
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

                    setCurrentMarkValueFromControl();

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

            fields.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;
                    if (e.ColumnIndex < 0)//row's header
                        return;
                    DataGridViewRow row = fields.Rows[e.RowIndex];
                    var cs = row.Cells;
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
                                    setRowStatus(statuses.ERROR, row, "Anchor not found");
                                else
                                {
                                    setRowStatus(statuses.SUCCESS, row, "Anchor found");
                                    r.X -= ((PointF)p).X;
                                    r.Y -= ((PointF)p).Y;
                                    cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
                                }
                            }
                            else//anchor deselected
                            {
                                setRowStatus(statuses.WARNING, row, "Changed");
                                cs["Value"].Value = extractValueAndDrawSelectionBox((int?)cs["FloatingAnchorId"].Value, r, Convert.ToBoolean(cs["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            fields.CurrentCellDirtyStateChanged += delegate
            {
                if (fields.IsCurrentCellDirty)
                    fields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            fields.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                DataGridViewRow r = fields.Rows[e.RowIndex];
                try
                {
                    string n = FieldPreparation.Normalize((string)r.Cells["Name_"].Value);
                    if (!r.IsNewRow)
                    {
                        if (string.IsNullOrWhiteSpace(n))
                            throw new Exception("Name cannot be empty!");
                        foreach (DataGridViewRow rr in fields.Rows)
                        {
                            if (r != rr && n == FieldPreparation.Normalize((string)rr.Cells["Name_"].Value))
                                throw new Exception("Name '" + n + "' is duplicated!");
                        }
                    }
                    r.Cells["Name_"].Value = n;
                }
                catch (Exception ex)
                {
                    //LogMessage.Error("Name", ex);
                    Message.Error2(ex);
                    e.Cancel = true;
                }
            };

            fields.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            fields.CellContentClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (e.ColumnIndex < 0)//row's header
                    return;
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

                    if (fields.SelectedRows.Count < 1)
                        return;

                    floatingAnchors.ClearSelection();
                    floatingAnchors.CurrentCell = null;
                    documentFirstPageRecognitionMarks.ClearSelection();
                    documentFirstPageRecognitionMarks.CurrentCell = null;
                    DataGridViewRow row = fields.SelectedRows[0];
                    int i = row.Index;

                    if (fields.Rows[i].IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int j = fields.Rows.Add();
                        fields.Rows[j].Selected = true;
                        return;
                    }
                    var cs = fields.Rows[i].Cells;

                    var vt = Convert.ToBoolean(cs["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText;
                    int? fai = (int?)cs["FloatingAnchorId"].Value;
                    setCurrentFloatingAnchorRow(fai, true);
                    string rs = (string)cs["Rectangle"].Value;
                    if (rs != null)
                    {
                        cs["Value"].Value = extractValueAndDrawSelectionBox(fai, SerializationRoutines.Json.Deserialize<Template.RectangleF>(rs), vt);
                        if (cs["Value"].Value != null)
                            setRowStatus(statuses.SUCCESS, row, "Found");
                        else
                            setRowStatus(statuses.ERROR, row, "Error");
                    }
                    else
                        setRowStatus(statuses.WARNING, row, "Not set");
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
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

        bool isMarkFound(DataGridViewRow row, bool selectAnchor)
        {
            var cs = row.Cells;
            var vt = (Template.ValueTypes)cs["ValueType2"].Value;
            int? fai = (int?)cs["FloatingAnchorId2"].Value;
            if (selectAnchor)
                setCurrentFloatingAnchorRow(fai, false);
            Template.RectangleF r = getMarkRectangle(row);
            if (r != null)
            {
                switch (vt)
                {
                    case Template.ValueTypes.PdfText:
                        {
                            Template.Mark.PdfTextValue ptv1 = (Template.Mark.PdfTextValue)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (ptv1.Text != t2)
                            {
                                setRowStatus(statuses.ERROR, row, "Not found:!'" + t2 + "'");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    case Template.ValueTypes.OcrText:
                        {
                            Template.Mark.OcrTextValue otv1 = (Template.Mark.OcrTextValue)row.Tag;
                            string t2 = (string)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (otv1.Text != t2)
                            {
                                setRowStatus(statuses.ERROR, row, "Not found:!'" + t2 + "'");
                                return false;
                            }
                            setRowStatus(statuses.SUCCESS, row, "Found");
                            return true;
                        }
                    case Template.ValueTypes.ImageData:
                        {
                            Template.Mark.ImageDataValue idv1 = (Template.Mark.ImageDataValue)row.Tag;
                            ImageData id2 = (ImageData)extractValueAndDrawSelectionBox(fai, r, vt);
                            if (!idv1.ImageData.ImageIsSimilar(id2, idv1.BrightnessTolerance, idv1.DifferentPixelNumberTolerance))
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
            Template.Mark.BaseValue v = (Template.Mark.BaseValue)row.Tag;
            if (v != null)
                return v.Rectangle;
            return null;
        }

        void setMarkRectangle(DataGridViewRow row, Template.RectangleF r)
        {
            int? fai = (int?)row.Cells["FloatingAnchorId2"].Value;
            if (fai == null && r == null)
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                setMarkValue(row, null);
                return;
            }
            Template.ValueTypes vt = (Template.ValueTypes)row.Cells["ValueType2"].Value;
            object v = extractValueAndDrawSelectionBox(fai, r, vt);
            if (v == null)
                setRowStatus(statuses.ERROR, row, "Not found");
            else
                setRowStatus(statuses.SUCCESS, row, "Set");
            switch (vt)
            {
                case Template.ValueTypes.PdfText:
                    {
                        Template.Mark.PdfTextValue ptv = (Template.Mark.PdfTextValue)row.Tag;
                        if (ptv == null)
                            ptv = new Template.Mark.PdfTextValue();
                        ptv.Text = (string)v;
                        ptv.Rectangle = r;
                        setMarkValue(row, ptv);
                    }
                    break;
                case Template.ValueTypes.OcrText:
                    {
                        Template.Mark.OcrTextValue otv = (Template.Mark.OcrTextValue)row.Tag;
                        if (otv == null)
                            otv = new Template.Mark.OcrTextValue();
                        otv.Text = (string)v;
                        otv.Rectangle = r;
                        setMarkValue(row, otv);
                    }
                    break;
                case Template.ValueTypes.ImageData:
                    {
                        Template.Mark.ImageDataValue idv = (Template.Mark.ImageDataValue)row.Tag;
                        if (idv == null)
                            idv = new Template.Mark.ImageDataValue();
                        idv.ImageData = (ImageData)v;
                        idv.Rectangle = r;
                        setMarkValue(row, idv);
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + vt);
            }
        }
    }
}