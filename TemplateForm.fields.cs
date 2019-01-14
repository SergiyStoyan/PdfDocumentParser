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
            LeftAnchorId.ValueType = typeof(int);
            LeftAnchorId.ValueMember = "Id";
            LeftAnchorId.DisplayMember = "Name";

            TopAnchorId.ValueType = typeof(int);
            TopAnchorId.ValueMember = "Id";
            TopAnchorId.DisplayMember = "Name";

            RightAnchorId.ValueType = typeof(int);
            RightAnchorId.ValueMember = "Id";
            RightAnchorId.DisplayMember = "Name";

            BottomAnchorId.ValueType = typeof(int);
            BottomAnchorId.ValueMember = "Id";
            BottomAnchorId.DisplayMember = "Name";

            Type.ValueType = typeof(Template.Types);
            Type.DataSource = Enum.GetValues(typeof(Template.Types));

            fields.EnableHeadersVisualStyles = false;//needed to set row headers

            fields.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = fields.Rows[e.RowIndex];
                Message.Error("fields[" + r.Index + "] has unacceptable value of " + fields.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
            };

            fields.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                if (fields.Rows.Count < 3 && fields.SelectedRows.Count > 0)
                    fields.SelectedRows[0].Selected = false;//to avoid auto-creating row
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
                        case "Type":
                            {
                                Template.Types t2 = (Template.Types)row.Cells["Type"].Value;
                                if (t2 == f.Type)
                                    break;
                                Template.Field f2;
                                switch (t2)
                                {
                                    case Template.Types.PdfText:
                                        f2 = new Template.Field.PdfText();
                                        break;
                                    case Template.Types.OcrText:
                                        f2 = new Template.Field.OcrText();
                                        break;
                                    case Template.Types.ImageData:
                                        f2 = new Template.Field.ImageData();
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + t2);
                                }
                                f2.Name = f.Name;
                                f2.LeftAnchorId = f.LeftAnchorId;
                                f2.TopAnchorId = f.TopAnchorId;
                                f2.RightAnchorId = f.RightAnchorId;
                                f2.BottomAnchorId = f.BottomAnchorId;
                                f2.Rectangle = f.Rectangle;
                                f = f2;
                                setFieldRow(row, f);
                                break;
                            }
                        case "LeftAnchorId":
                        case "TopAnchorId":
                        case "RightAnchorId":
                        case "BottomAnchorId":
                            {
                                f.LeftAnchorId = (int?)cs["LeftAnchorId"].Value;
                                f.TopAnchorId = (int?)cs["TopAnchorId"].Value;
                                f.RightAnchorId = (int?)cs["RightAnchorId"].Value;
                                f.BottomAnchorId = (int?)cs["BottomAnchorId"].Value;
                                setFieldRow(row, f);
                                break;
                            }
                        case "Name_":
                            f.Name = (string)row.Cells["Name_"].Value;
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

                fields.CurrentCell = fields[0, row.Index];
                Template.Field f = (Template.Field)row.Tag;
                //setCurrentAnchorRow(f.LeftAnchorId, true);
                //setCurrentAnchorRow(f.TopAnchorId, false);
                //setCurrentAnchorRow(f.RightAnchorId, false);
                //setCurrentAnchorRow(f.BottomAnchorId, false);
                setCurrentAnchorRow(null, true);
                setCurrentConditionRow(null);
                setFieldRowValue(row, false);
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }
        bool settingCurrentFieldRow = false;
        DataGridViewRow currentFieldRow = null;

        void setFieldRowValue(DataGridViewRow row, bool setEmpty)
        {
            Template.Field f = (Template.Field)row.Tag;
            if (f == null)
                return;
            if (!f.IsSet())
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                return;
            }
            DataGridViewCell c = row.Cells["Value"];
            if (c is DataGridViewImageCell && c.Value != null)
                ((Bitmap)c.Value).Dispose();
            if (setEmpty)
            {
                c.Value = null;
                setRowStatus(statuses.NEUTRAL, row, "");
                return;
            }
            clearImageFromBoxes();
            object v = extractFieldAndDrawSelectionBox(f);
            if (f.Type == Template.Types.ImageData)
            {
                if (!(c is DataGridViewImageCell))
                {
                    c.Dispose();
                    c = new DataGridViewImageCell();
                    row.Cells["Value"] = c;
                }
            }
            else
            {
                if (c is DataGridViewImageCell)
                {
                    c.Dispose();
                    c = new DataGridViewTextBoxCell();
                    row.Cells["Value"] = c;
                }
            }
            c.Value = v;
            if (c.Value != null)
                setRowStatus(statuses.SUCCESS, row, "Found");
            else
                setRowStatus(statuses.ERROR, row, "Error");
        }

        void setFieldRow(DataGridViewRow row, Template.Field f)
        {
            row.Tag = f;
            row.Cells["Name_"].Value = f.Name;
            row.Cells["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
            switch (f.Type)
            {
                case Template.Types.PdfText:
                    row.Cells["Type"].Value = Template.Types.PdfText;
                    break;
                case Template.Types.OcrText:
                    row.Cells["Type"].Value = Template.Types.OcrText;
                    break;
                case Template.Types.ImageData:
                    row.Cells["Type"].Value = Template.Types.ImageData;
                    break;
                default:
                    throw new Exception("Unknown option: " + f.Type);
            }
            row.Cells["LeftAnchorId"].Value = f.LeftAnchorId;
            row.Cells["TopAnchorId"].Value = f.TopAnchorId;
            row.Cells["RightAnchorId"].Value = f.RightAnchorId;
            row.Cells["BottomAnchorId"].Value = f.BottomAnchorId;

            if (loadingTemplate)
                return;

            if (row == currentFieldRow)
                setCurrentFieldRow(row);
        }
    }
}