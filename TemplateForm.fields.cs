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

            Type.ValueType = typeof(Template.Field.Types);
            Type.DataSource = Enum.GetValues(typeof(Template.Field.Types));

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
                                Template.Field.Types t2 = (Template.Field.Types)row.Cells["Type"].Value;
                                if (t2 == f.Type)
                                    break;
                                Template.Field f2;
                                switch (t2)
                                {
                                    case Template.Field.Types.PdfText:
                                        f2 = new Template.Field.PdfText();
                                        break;
                                    case Template.Field.Types.OcrText:
                                        f2 = new Template.Field.OcrText();
                                        break;
                                    case Template.Field.Types.ImageData:
                                        f2 = new Template.Field.ImageData();
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + t2);
                                }
                                f2.Name = f.Name;
                                f2.LeftAnchor = f.LeftAnchor;
                                f2.TopAnchor = f.TopAnchor;
                                f2.RightAnchor = f.RightAnchor;
                                f2.BottomAnchor = f.BottomAnchor;
                                f2.Rectangle = f.Rectangle;
                                f = f2;
                                setFieldRow(row, f);
                                break;
                            }
                        case "LeftAnchorId":
                            {
                                int? ai = (int?)cs["LeftAnchorId"].Value;
                                if (ai == null)
                                    f.LeftAnchor = null;
                                else
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo((int)ai);
                                    f.LeftAnchor = new Template.Field.SideAnchor
                                    {
                                        Id = (int)ai,
                                        Shift = aai.Shift.Width,
                                    };
                                }
                                setFieldRow(row, f);
                            }
                            break;
                        case "TopAnchorId":
                            {
                                int? ai = (int?)cs["TopAnchorId"].Value;
                                if (ai == null)
                                    f.TopAnchor = null;
                                else
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo((int)ai);
                                    f.TopAnchor = new Template.Field.SideAnchor
                                    {
                                        Id = (int)ai,
                                        Shift = aai.Shift.Height,
                                    };
                                }
                                setFieldRow(row, f);
                            }
                            break;
                        case "RightAnchorId":
                            {
                                int? ai = (int?)cs["RightAnchorId"].Value;
                                if (ai == null)
                                    f.RightAnchor = null;
                                else
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo((int)ai);
                                    f.RightAnchor = new Template.Field.SideAnchor
                                    {
                                        Id = (int)ai,
                                        Shift = aai.Shift.Width,
                                    };
                                }
                                setFieldRow(row, f);
                            }
                            break;
                        case "BottomAnchorId":
                            {
                                int? ai = (int?)cs["BottomAnchorId"].Value;
                                if (ai == null)
                                    f.BottomAnchor = null;
                                else
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo((int)ai);
                                    f.BottomAnchor = new Template.Field.SideAnchor
                                    {
                                        Id = (int)ai,
                                        Shift = aai.Shift.Height,
                                    };
                                }
                                setFieldRow(row, f);
                            }
                            break;
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
                        //foreach (DataGridViewRow rr in fields.Rows)
                        //{
                        //    if (r == rr)
                        //        continue;
                        //    Template.Field f = (Template.Field)rr.Tag;
                        //    if (f != null && n == f.Name)
                        //        throw new Exception("Name '" + n + "' is duplicated!");
                        //}
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

            copy2ClipboardField.LinkClicked += delegate
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r.Tag == null)
                    return;
                Template.Field f = (Template.Field)r.Tag;
                object o = pages[currentPageI].GetValue(f.Name);
                switch (f.Type)
                {
                    case Template.Field.Types.ImageData:
                        Clipboard.SetData(f.Type.ToString(), (Image)o);
                        break;
                    case Template.Field.Types.PdfText:
                    case Template.Field.Types.OcrText:
                        Clipboard.SetText((string)o);
                        break;
                    default:
                        throw new Exception("Unknown option: " + f.Type);
                }
            };

            duplicateField.LinkClicked += delegate
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r0 = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r0.Tag == null)
                    return;
                int i = fields.Rows.Add();
                DataGridViewRow row = fields.Rows[i];
                //fields.Rows.Remove(row);
                //fields.Rows.Insert(r0.Index, row);
                Template.Field f = (Template.Field)Serialization.Json.Clone(((Template.Field)r0.Tag).GetType(), r0.Tag);
                setFieldRow(row, f);
                row.Selected = true;
            };

            deleteField.LinkClicked += delegate
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r.Tag == null)
                    return;
                bool unique = true;
                foreach (DataGridViewRow rr in fields.Rows)
                    if (rr != r && rr.Tag != null && ((Template.Field)rr.Tag).Name == ((Template.Field)r.Tag).Name)
                    {
                        unique = false;
                        break;
                    }
                if (!unique)
                    fields.Rows.Remove(r);
            };

            moveUpField.LinkClicked += delegate
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                int i = r.Index - 1;
                if (i < 0)
                    return;
                fields.Rows.Remove(r);
                fields.Rows.Insert(i, r);
            };

            moveDownField.LinkClicked += delegate
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                int i = r.Index + 1;
                if (i > fields.Rows.Count - 1)
                    return;
                fields.Rows.Remove(r);
                fields.Rows.Insert(i, r);
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

        bool setFieldRowValue(DataGridViewRow row, bool setEmpty)
        {
            Template.Field f = (Template.Field)row.Tag;
            if (f == null)
                return false;
            if (!f.IsSet())
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                return false;
            }
            DataGridViewCell c = row.Cells["Value"];
            if (c is DataGridViewImageCell && c.Value != null)
                ((Bitmap)c.Value).Dispose();
            if (setEmpty)
            {
                c.Value = null;
                setRowStatus(statuses.NEUTRAL, row, "");
                return false;
            }
            clearImageFromBoxes();
            object v = extractFieldAndDrawSelectionBox(f);
            if (f.Type == Template.Field.Types.ImageData)
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
            return v != null;
        }

        void setFieldRow(DataGridViewRow row, Template.Field f)
        {
            row.Tag = f;
            row.Cells["Name_"].Value = f.Name;
            row.Cells["Rectangle"].Value = Serialization.Json.Serialize(f.Rectangle);
            switch (f.Type)
            {
                case Template.Field.Types.PdfText:
                    row.Cells["Type"].Value = Template.Field.Types.PdfText;
                    break;
                case Template.Field.Types.OcrText:
                    row.Cells["Type"].Value = Template.Field.Types.OcrText;
                    break;
                case Template.Field.Types.ImageData:
                    row.Cells["Type"].Value = Template.Field.Types.ImageData;
                    break;
                default:
                    throw new Exception("Unknown option: " + f.Type);
            }
            row.Cells["LeftAnchorId"].Value = f.LeftAnchor?.Id;
            row.Cells["TopAnchorId"].Value = f.TopAnchor?.Id;
            row.Cells["RightAnchorId"].Value = f.RightAnchor?.Id;
            row.Cells["BottomAnchorId"].Value = f.BottomAnchor?.Id;

            if (loadingTemplate)
                return;

            if (row == currentFieldRow)
                setCurrentFieldRow(row);
        }
    }
}