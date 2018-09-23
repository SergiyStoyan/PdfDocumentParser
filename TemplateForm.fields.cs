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
        void initializeFieldsTable()
        {
            FloatingAnchorId.ValueType = typeof(int);
            FloatingAnchorId.ValueMember = "Id";
            FloatingAnchorId.DisplayMember = "Name";

            fields.EnableHeadersVisualStyles = false;//needed to set row headers

            fields.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = fields.Rows[e.RowIndex];
                Message.Error("fields[" + r.Index + "] has unacceptable value of " + fields.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            fields.RowsAdded += delegate (object sender, DataGridViewRowsAddedEventArgs e)
            {
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
                    Template.Field f = (Template.Field)row.Tag;
                    switch (fields.Columns[e.ColumnIndex].Name)
                    {
                        //case "Rectangle":
                        //    {
                        //        cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                        //        break;
                        //    }
                        case "Ocr":
                            {
                                bool ocr = Convert.ToBoolean(cs["Ocr"].Value);
                                if (ocr == (f.Type != Template.Types.PdfText))
                                    break;
                                Template.Field f2;
                                if (ocr)
                                {
                                    f2 = new Template.Field.OcrText();
                                    ((Template.Field.OcrText)f2).FloatingAnchorId = ((Template.Field.PdfText)f).FloatingAnchorId;
                                    ((Template.Field.OcrText)f2).Rectangle = ((Template.Field.PdfText)f).Rectangle;
                                    ((Template.Field.OcrText)f2).Name = ((Template.Field.PdfText)f).Name;
                                }
                                else
                                {
                                    f2 = new Template.Field.PdfText();
                                    ((Template.Field.PdfText)f2).FloatingAnchorId = ((Template.Field.OcrText)f).FloatingAnchorId;
                                    ((Template.Field.PdfText)f2).Rectangle = ((Template.Field.OcrText)f).Rectangle;
                                    ((Template.Field.PdfText)f2).Name = ((Template.Field.OcrText)f).Name;
                                }
                                break;
                            }
                        case "FloatingAnchorId":
                            {
                                f.FloatingAnchorId = (int?)cs["FloatingAnchorId"].Value;
                                if (f.FloatingAnchorId != null)
                                {
                                    pages.ActiveTemplate = getTemplateFromUI(false);
                                    PointF? p = pages[currentPage].GetFloatingAnchorPoint0((int)f.FloatingAnchorId);
                                    if (p == null)
                                        setRowStatus(statuses.ERROR, row, "Anchor not found");
                                    else
                                    {
                                        setRowStatus(statuses.SUCCESS, row, "Anchor found");
                                        f.Rectangle.X -= ((PointF)p).X;
                                        f.Rectangle.Y -= ((PointF)p).Y;
                                        setFieldRectangle(row, f.Rectangle);
                                    }
                                }
                                else//anchor deselected
                                {
                                    setRowStatus(statuses.WARNING, row, "Changed");
                                    cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                                }
                                break;
                            }
                        case "Name_":
                            f.Name = (string)row.Cells["Name_"].Value;
                            break;
                    }
                    setFieldRow(row, f);
                    //cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                    //if (cs["Value"].Value == null)
                    //    setRowStatus(statuses.WARNING, row, "Not found");
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
                    if (r.Tag != null)
                    {
                        string n = FieldPreparation.Normalize((string)r.Cells["Name_"].Value);
                        if (string.IsNullOrWhiteSpace(n))
                            throw new Exception("Name cannot be empty!");
                        foreach (DataGridViewRow rr in fields.Rows)
                        {
                            if (r == rr)
                                continue;
                            Template.Field f = (Template.Field)rr.Tag;
                            if (f != null && n == f.Name)
                                throw new Exception("Name '" + n + "' is duplicated!");
                        }
                        r.Cells["Name_"].Value = n;
                    }
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
                    DataGridViewRow row = fields.SelectedRows[0];
                    setCurrentFieldRow(row);
                    Template.Field f = (Template.Field)row.Tag;

                    if (row.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = fields.Rows.Add();
                        row = fields.Rows[i];
                        f = new Template.Field.PdfText();
                        row.Tag = f;
                        row.Cells["Ocr"].Value = f.Type == Template.Types.PdfText ? false : true;
                        row.Selected = true;
                        return;
                    }
                    var cs = row.Cells;

                    if (f.Rectangle != null)
                    {
                        cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
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

        void setCurrentFieldRow(DataGridViewRow row)
        {
            try
            {
                fields.ClearSelection();
                setCurrentMarkRow(null);

                if (row == null)
                {
                    fields.CurrentCell = null;
                    setCurrentFloatingAnchorRow(null);
                    return;
                }
                fields.CurrentCell = fields[0, row.Index];
            }
            finally
            {
            }
        }

        void setFieldRectangle(DataGridViewRow row, Template.RectangleF r)
        {
            Template.Field f = (Template.Field)row.Tag;
            if (f == null)
                return;
            f.Rectangle = r;
            row.Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(r);
            setFieldRow(row, f);
        }

        void setFieldRow(DataGridViewRow row, Template.Field f)
        {
            row.Tag = f;
            row.Cells["Name_"].Value = f.Name;
            row.Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
            row.Cells["Ocr"].Value = f.Type == Template.Types.PdfText ? false : true;
            row.Cells["FloatingAnchorId"].Value = f.FloatingAnchorId;

            if (loadingTemplate)
                return;

            if (f.IsSet())
            {
                DataGridViewRow rr;
                Template.FloatingAnchor fa = getFloatingAnchor(f.FloatingAnchorId, out rr);
                if (fa != null && !fa.IsSet())
                    setRowStatus(statuses.ERROR, row, "Error");
                else
                    setRowStatus(statuses.SUCCESS, row, "Set");
            }
            else
                setRowStatus(statuses.WARNING, row, "Not set");
        }
    }
}