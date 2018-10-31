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
            AnchorId.ValueType = typeof(int);
            AnchorId.ValueMember = "Id";
            AnchorId.DisplayMember = "Name";

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
                        //        cs["Value"].Value = extractValueAndDrawSelectionBox(f.AnchorId, f.Rectangle, f.Type);
                        //        break;
                        //    }
                        case "Ocr":
                            {
                                bool ocr = Convert.ToBoolean(cs["Ocr"].Value);
                                if (ocr == (f.Type != Template.Types.PdfText))
                                    break;
                                Template.Field f2;
                                if (ocr)
                                    f2 = new Template.Field.OcrText();
                                else
                                    f2 = new Template.Field.PdfText();
                                f2.Name = f.Name;
                                f2.AnchorId = f.AnchorId;
                                f2.Rectangle = f.Rectangle;
                                f = f2;
                                setFieldRow(row, f);
                                break;
                            }
                        case "AnchorId":
                            {
                                f.AnchorId = (int?)cs["AnchorId"].Value;
                                //if (f.AnchorId != null)
                                //{
                                //    pages.ActiveTemplate = getTemplateFromUI(false);
                                //    PointF? p = pages[currentPage].GetAnchorPoint0((int)f.AnchorId);
                                //    if (p == null)
                                //        setRowStatus(statuses.ERROR, row, "Anchor not found");
                                //    else
                                //        setRowStatus(statuses.SUCCESS, row, "Anchor found");
                                //}
                                setFieldRow(row, f);
                                break;
                            }
                        case "Name_":
                            f.Name = (string)row.Cells["Name_"].Value;
                            break;
                    }
                    //cs["Value"].Value = extractValueAndDrawSelectionBox(f.AnchorId, f.Rectangle, f.Type);
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

                    if (settingCurrentFieldRow)
                        return;

                    if (fields.SelectedRows.Count < 1)
                        return;
                    DataGridViewRow row = fields.SelectedRows[0];
                    Template.Field f = (Template.Field)row.Tag;
                    if (f == null)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = fields.Rows.Add();
                        row = fields.Rows[i];
                        f = templateManager.CreateDefaultField();
                        setFieldRow(row, f);
                        row.Selected = true;
                        return;
                    }
                    setCurrentFieldRow(row);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };
        }

        void setCurrentFieldRow(DataGridViewRow row)
        {
            if (settingCurrentFieldRow)
                return;
            try
            {
                settingCurrentFieldRow = true;
                //if (row == currentFieldRow)
                //    return;
                currentFieldRow = row;

                if (row == null)
                {
                    fields.ClearSelection();
                    fields.CurrentCell = null;
                    return;
                }

                setCurrentMarkRow(null);

                fields.CurrentCell = fields[0, row.Index];
                Template.Field f = (Template.Field)row.Tag;
                setCurrentAnchorRow(f.AnchorId, true);

                if (f.Rectangle != null)
                {
                    row.Cells["Value"].Value = extractValueAndDrawSelectionBox(f.AnchorId, f.Rectangle, f.Type);
                    if (row.Cells["Value"].Value != null)
                        setRowStatus(statuses.SUCCESS, row, "Found");
                    else
                        setRowStatus(statuses.ERROR, row, "Error");
                }
                else
                    setRowStatus(statuses.WARNING, row, "Not set");
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }
        bool settingCurrentFieldRow = false;
        DataGridViewRow currentFieldRow = null;

        void setFieldRow(DataGridViewRow row, Template.Field f)
        {
            row.Tag = f;
            row.Cells["Name_"].Value = f.Name;
            row.Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
            row.Cells["Ocr"].Value = f.Type == Template.Types.PdfText ? false : true;
            row.Cells["AnchorId"].Value = f.AnchorId;

            if (loadingTemplate)
                return;

            if (row == currentFieldRow)
                setCurrentFieldRow(row);
        }
    }
}