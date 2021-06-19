//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;

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

            Type.ValueType = typeof(Template.Field.ValueTypes);
            Type.DataSource = Enum.GetValues(typeof(Template.Field.ValueTypes));

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

            fields.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            fields.PreviewKeyDown += delegate (object sender, PreviewKeyDownEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Add:
                    case Keys.Oemplus:
                        duplicateSelectedField();
                        break;
                    case Keys.Delete:
                    case Keys.OemMinus:
                        deleteSelectedField();
                        break;
                    case Keys.Up:
                        if (e.Modifiers == Keys.Control)
                            moveUpSelectedField();
                        break;
                    case Keys.Down:
                        if (e.Modifiers == Keys.Control)
                            moveDownSelectedField();
                        break;
                }
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
                        case "Type":
                            {
                                //foreach (DataGridViewRow r in fields.Rows)
                                //{
                                //    if (r.Tag != null && ((Template.Field)r.Tag).Name == f.Name)
                                //    {
                                //        f = (Template.Field)r.Tag;
                                //        f.DefaultValueType = (Template.Field.ValueTypes)row.Cells["Type"].Value;
                                //        setFieldRow(r, f);
                                //    }
                                //}
                                f.DefaultValueType = (Template.Field.ValueTypes)row.Cells["Type"].Value;
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
                            throw new Exception("Field name cannot be empty!");
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
                    //Win.LogMessage.Error("Name", ex);
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
                    if (f == null)//hacky forcing to commit a newly added row and display the blank row
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
                    Win.LogMessage.Error(ex);
                }
            };

            copy2ClipboardField.LinkClicked += delegate
            {
                if (pages == null)
                    return;
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r.Tag == null)
                    return;
                Template.Field f = (Template.Field)r.Tag;
                object o = pages[currentPageI].GetValue(f.Name);
                switch (f.DefaultValueType)
                {
                    case Template.Field.ValueTypes.Image:
                    case Template.Field.ValueTypes.OcrTextLineImages:
                        Clipboard.SetData(f.DefaultValueType.ToString(), (Image)o);
                        break;
                    case Template.Field.ValueTypes.PdfText:
                    case Template.Field.ValueTypes.OcrText:
                        Clipboard.SetText((string)o);
                        break;
                    case Template.Field.ValueTypes.PdfTextLines:
                    case Template.Field.ValueTypes.OcrTextLines:
                        Clipboard.SetText(string.Join("\r\n", (List<string>)o));
                        break;
                    case Template.Field.ValueTypes.PdfCharBoxs:
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        Clipboard.SetText(Serialization.Json.Serialize(o));
                        break;
                    default:
                        throw new Exception("Unknown option: " + f.DefaultValueType);
                }
            };

            duplicateField.LinkClicked += delegate
            {
                duplicateSelectedField();
            };
            //duplicateField.LinkClicked += delegate
            //{
            //    if (fields.SelectedRows.Count < 1)
            //        return;
            //    DataGridViewRow r0 = fields.SelectedRows[fields.SelectedRows.Count - 1];
            //    if (r0.Tag == null)
            //        return;
            //    Template.Field f0 = (Template.Field)r0.Tag;
            //    if (f0.ColumnOfTable != null)
            //    {
            //        Message.Exclaim("This field is a column of table " + f0.ColumnOfTable + " so you should create a new definition of it.");
            //        return;
            //    }
            //    List<DataGridViewRow> cloningFieldRows = new List<DataGridViewRow> { r0 };
            //    //foreach (DataGridViewRow r in fields.Rows)
            //    //    if (r != r0 && (r.Tag as Template.Field)?.ColumnOfTable == f0.Name)
            //    //    {
            //    //        string fn = (r.Tag as Template.Field)?.Name;
            //    //        if (cloningFieldRows.Find(x => (x.Tag as Template.Field)?.Name == fn) == null)
            //    //            cloningFieldRows.Add(r);
            //    //    }

            //    settingCurrentFieldRow = true;//required due to fields-column error calculation when selected row changes
            //    foreach (DataGridViewRow row in cloningFieldRows)
            //    {
            //        Template.Field f = (Template.Field)Serialization.Json.Clone(((Template.Field)row.Tag).GetType(), row.Tag);
            //        f.LeftAnchor = null;
            //        f.TopAnchor = null;
            //        f.RightAnchor = null;
            //        f.BottomAnchor = null;
            //        int i = fields.Rows.Add();
            //        DataGridViewRow r = fields.Rows[i];
            //        setFieldRow(r, f);
            //        fields.Rows.Remove(r);
            //        fields.Rows.Insert(row.Index + 1, r);
            //    }
            //    settingCurrentFieldRow = false;
            //};

            deleteField.LinkClicked += delegate
            {
                deleteSelectedField();
            };
            //deleteField.LinkClicked += delegate
            //{
            //    if (fields.SelectedRows.Count < 1)
            //        return;
            //    DataGridViewRow r0 = fields.SelectedRows[fields.SelectedRows.Count - 1];
            //    if (r0.Tag == null)
            //        return;
            //    Template.Field f0 = (Template.Field)r0.Tag;
            //    bool unique = true;
            //    foreach (DataGridViewRow rr in fields.Rows)
            //        if (rr != r0 && rr.Tag != null && ((Template.Field)rr.Tag).Name == f0.Name)
            //        {
            //            unique = false;
            //            break;
            //        }
            //    if (unique)
            //    {
            //        Message.Inform("This field definition cannot be deleted because it is the last of the field.");
            //        return;
            //    }
            //    if (f0.ColumnOfTable != null)
            //    {
            //        Message.Exclaim("This field is a column of table " + f0.ColumnOfTable + " so you should delete the respectivea definition of it.");
            //        return;
            //    }
            //    Dictionary<string, List<DataGridViewRow>> fieldName2orderedRows = new Dictionary<string, List<DataGridViewRow>>();
            //    foreach (DataGridViewRow r in fields.Rows)
            //        if ((r.Tag as Template.Field.PdfText)?.ColumnOfTable == f0.Name)
            //        {
            //            List<DataGridViewRow> rs;
            //            string fn = (r.Tag as Template.Field)?.Name;
            //            if (!fieldName2orderedRows.TryGetValue(fn, out rs))
            //            {
            //                rs = new List<DataGridViewRow>();
            //                fieldName2orderedRows[fn] = rs;
            //            }
            //            rs.Add(r);
            //        }

            //    int definitionIndex = fieldName2orderedRows[f0.Name].IndexOf(r0);
            //    fieldName2orderedRows.Remove(f0.Name);
            //    List<DataGridViewRow> deletingFieldRows = new List<DataGridViewRow> { r0 };
            //    foreach (List<DataGridViewRow> rs in fieldName2orderedRows.Values)
            //        deletingFieldRows.Add(rs[definitionIndex]);

            //    settingCurrentFieldRow = true;//required due to fields-column error calculation when selected row changes
            //    foreach (DataGridViewRow row in deletingFieldRows)
            //        fields.Rows.Remove(row);
            //    settingCurrentFieldRow = false;
            //};

            moveUpField.LinkClicked += delegate
            {
                moveUpSelectedField();
            };

            moveDownField.LinkClicked += delegate
            {
                moveDownSelectedField();
            };

            newField.LinkClicked += delegate
             {
                 createNewField();
             };
        }

        void duplicateSelectedField()
        {
            try
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r0 = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r0.Tag == null)
                    return;
                Template.Field f0 = (Template.Field)r0.Tag;
                List<DataGridViewRow> cloningFieldRows = new List<DataGridViewRow> { r0 };
                //if((f0 as Template.Field.PdfText)?.ColumnOfTable!=null)
                //{
                //    Message.Exclaim("This field is a column of "+ (f0 as Template.Field.PdfText)?.ColumnOfTable + " so you should create a new definition of it.");
                //    return;
                //}
                //foreach (DataGridViewRow r in fields.Rows)
                //    if ((r.Tag as Template.Field.PdfText)?.ColumnOfTable == f0.Name)
                //        cloningFieldRows.Add(r);
                if (f0.ColumnOfTable != null
                    && Message.YesNo("This field is a column of table " + f0.ColumnOfTable + ".\r\nWould you like new definions to be created for all the column fields of the table?")
                    )
                {
                    foreach (DataGridViewRow r in fields.Rows)
                        if (r != r0 && (r.Tag as Template.Field)?.ColumnOfTable == f0.ColumnOfTable)
                        {
                            string fn = (r.Tag as Template.Field)?.Name;
                            if (cloningFieldRows.Find(x => (x.Tag as Template.Field)?.Name == fn) == null)
                                cloningFieldRows.Add(r);
                        }
                }
                settingCurrentFieldRow = true;//required due to fields-column error calculation when selected row changes
                foreach (DataGridViewRow row in cloningFieldRows)
                {
                    Template.Field f = (Template.Field)Serialization.Json.Clone(((Template.Field)row.Tag).GetType(), row.Tag);
                    f.LeftAnchor = null;
                    f.TopAnchor = null;
                    f.RightAnchor = null;
                    f.BottomAnchor = null;
                    int i = fields.Rows.Add();
                    DataGridViewRow r = fields.Rows[i];
                    setFieldRow(r, f);
                    fields.Rows.Remove(r);
                    fields.Rows.Insert(row.Index + 1, r);
                }
            }
            catch (Exception e)
            {
                Win.LogMessage.Error(e);
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }

        void deleteSelectedField()
        {
            try
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r0 = fields.SelectedRows[fields.SelectedRows.Count - 1];
                if (r0.Tag == null)
                    return;
                Template.Field f0 = (Template.Field)r0.Tag;
                bool unique = true;
                foreach (DataGridViewRow rr in fields.Rows)
                    if (rr != r0 && rr.Tag != null && ((Template.Field)rr.Tag).Name == f0.Name)
                    {
                        unique = false;
                        break;
                    }
                if (unique && !Message.YesNo("This field '" + f0.Name + "' will be removed completely because it is the last definition for it.\r\nProceed?", this, Message.Icons.Exclamation))
                {
                    //Message.Inform("This field definition cannot be deleted because it is the last of the field.");
                    return;
                }
                fields.Rows.Remove(r0);
            }
            catch (Exception e)
            {
                Win.LogMessage.Error(e);
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }

        void moveUpSelectedField()
        {
            try
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                int i2 = r.Index - 1;
                if (i2 < 0)
                    return;
                //int minI = int.MaxValue;
                //foreach (DataGridViewRow rr in fields.Rows)
                //    if (rr != r && rr.Tag != null && ((Template.Field)rr.Tag).Name == ((Template.Field)r.Tag).Name && rr.Index < minI)
                //        minI = rr.Index;
                //if (i2 < minI)
                //    return;
                settingCurrentFieldRow = true;//required due to fields-column error calculation when selected row changes
                fields.Rows.Remove(r);
                fields.Rows.Insert(i2, r);
                settingCurrentFieldRow = false;
                r.Selected = true;
            }
            catch (Exception e)
            {
                Win.LogMessage.Error(e);
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }

        void moveDownSelectedField()
        {
            try
            {
                if (fields.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = fields.SelectedRows[fields.SelectedRows.Count - 1];
                int i2 = r.Index + 1;
                if (i2 > fields.Rows.Count - 1)
                    return;
                //int maxI = 0;
                //foreach (DataGridViewRow rr in fields.Rows)
                //    if (rr != r && rr.Tag != null && ((Template.Field)rr.Tag).Name == ((Template.Field)r.Tag).Name && rr.Index > maxI)
                //        maxI = rr.Index;
                //if (i2 > maxI + 1)
                //    return;
                settingCurrentFieldRow = true;//required due to fields-column error calculation when selected row changes
                fields.Rows.Remove(r);
                fields.Rows.Insert(i2, r);
                settingCurrentFieldRow = false;
                r.Selected = true;
            }
            catch (Exception e)
            {
                Win.LogMessage.Error(e);
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }

        void createNewField()
        {
            Template.Field f = new Template.Field { Name = "" };
            int i = fields.CurrentRow?.Index >= 0 ? fields.CurrentRow.Index + 1 : fields.Rows.Count;
            fields.Rows.Insert(i, 1);
            setFieldRow(fields.Rows[i], f);
            fields.Rows[i].Selected = true;
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
                    currentFieldControl = null;
                    return;
                }

                fields.CurrentCell = fields[0, row.Index];
                //Template.Field f = (Template.Field)row.Tag;
                //setCurrentAnchorRow(f.LeftAnchorId, true);
                //setCurrentAnchorRow(f.TopAnchorId, false);
                //setCurrentAnchorRow(f.RightAnchorId, false);
                //setCurrentAnchorRow(f.BottomAnchorId, false);
                setCurrentAnchorRow(null, true);
                setCurrentConditionRow(null);
                object v = setFieldRowValue(row, false);
                Template.Field f = (Template.Field)row.Tag;

                List<Template.Field> fs = new List<Template.Field>();
                foreach (DataGridViewRow r in fields.Rows)
                {
                    if (r.Tag == null)
                        continue;
                    fs.Add((Template.Field)r.Tag);
                }
                currentFieldControl = new FieldGeneralControl(new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpaceThreshold.Value, Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text) });
                currentFieldControl.Initialize(row, v, fs, (DataGridViewRow r) => { setFieldRow(r, f); });
            }
            finally
            {
                settingCurrentFieldRow = false;
            }
        }
        bool settingCurrentFieldRow = false;
        DataGridViewRow currentFieldRow = null;

        object setFieldRowValue(DataGridViewRow row, bool setEmpty)
        {
            Template.Field f = (Template.Field)row.Tag;
            if (f == null)
                return null;
            if (!f.IsSet())
            {
                setRowStatus(statuses.WARNING, row, "Not set");
                return null;
            }
            DataGridViewCell c = row.Cells["Value"];
            if (c.Value != null && c.Value is IDisposable)
                ((IDisposable)c.Value).Dispose();
            if (f.DefaultValueType == Template.Field.ValueTypes.Image || f.DefaultValueType == Template.Field.ValueTypes.OcrTextLineImages)
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
            if (setEmpty)
            {
                c.Value = null;
                setRowStatus(statuses.NEUTRAL, row, "");
                return null;
            }
            clearImageFromBoxes();
            object v = extractFieldAndDrawSelectionBox(f);

            if (v != null)
                switch (f.DefaultValueType)
                {
                    case Template.Field.ValueTypes.PdfText:
                        c.Value = Page.NormalizeText((string)v);
                        break;
                    case Template.Field.ValueTypes.PdfTextLines:
                        c.Value = Page.NormalizeText(string.Join("\r\n", (List<string>)v));
                        break;
                    case Template.Field.ValueTypes.PdfCharBoxs:
                        c.Value = Page.NormalizeText(Serialization.Json.Serialize(v));
                        break;
                    case Template.Field.ValueTypes.OcrText:
                        c.Value = Page.NormalizeText((string)v);
                        break;
                    case Template.Field.ValueTypes.OcrTextLines:
                        c.Value = Page.NormalizeText(string.Join("\r\n", (List<string>)v));
                        break;
                    case Template.Field.ValueTypes.OcrCharBoxs:
                        c.Value = Page.NormalizeText(Serialization.Json.Serialize(v));
                        break;
                    default:
                        c.Value = v;
                        break;
                }
            else
                c.Value = v;

            if (c.Value != null)
                setRowStatus(statuses.SUCCESS, row, "Found");
            else
                setRowStatus(statuses.ERROR, row, "Not found");

            return v;
        }

        void setFieldRow(DataGridViewRow row, Template.Field f)
        {
            row.Tag = f;
            row.Cells["Name_"].Value = f.Name;
            //row.Cells["Rectangle"].Value = Serialization.Json.Serialize(f.Rectangle);
            row.Cells["Type"].Value = f.DefaultValueType;
            row.Cells["LeftAnchorId"].Value = f.LeftAnchor?.Id;
            row.Cells["TopAnchorId"].Value = f.TopAnchor?.Id;
            row.Cells["RightAnchorId"].Value = f.RightAnchor?.Id;
            row.Cells["BottomAnchorId"].Value = f.BottomAnchor?.Id;

            if (loadingTemplate)
                return;

            if (currentFieldControl != null && row == currentFieldRow)
                setCurrentFieldRow(row);
        }

        FieldGeneralControl currentFieldControl
        {
            get
            {
                if (splitContainer4.Panel2.Controls.Count < 1)
                    return null;
                return (FieldGeneralControl)splitContainer4.Panel2.Controls[0];
            }
            set
            {
                splitContainer4.Panel2.Controls.Clear();
                if (value == null)
                    return;
                splitContainer4.Panel2.Controls.Add(value);
                value.Dock = DockStyle.Fill;
            }
        }
    }
}