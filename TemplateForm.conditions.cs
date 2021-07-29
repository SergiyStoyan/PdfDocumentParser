//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// template editor GUI
    /// </summary>
    public partial class TemplateForm : Form
    {
        void initializeConditionsTable()
        {
            conditions.EnableHeadersVisualStyles = false;//needed to set row headers

            conditions.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = anchors.Rows[e.RowIndex];
                Message.Error("Condition[" + r.Index + "] has unacceptable value of " + conditions.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message, this);
            };

            conditions.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                if (conditions.Rows.Count < 3 && conditions.SelectedRows.Count > 0)
                    conditions.SelectedRows[0].Selected = false;//to avoid auto-creating row
            };

            conditions.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (loadingTemplate)
                    return;
                if (e.ColumnIndex < 0)//row's header
                    return;
                DataGridViewRow row = conditions.Rows[e.RowIndex];
                var cs = row.Cells;
                Template.Condition c = (Template.Condition)row.Tag;
                switch (conditions.Columns[e.ColumnIndex].Name)
                {
                    case "Name2":
                        {
                            c.Name = (string)row.Cells["Name2"].Value;
                            break;
                        }
                    case "Value2":
                        {
                            c.Value = (string)row.Cells["Value2"].Value;
                            break;
                        }
                }
                setConditionRow(row, c);
                setConditionStatus(row);
            };

            conditions.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                    if (loadingTemplate)
                        return;

                    if (settingCurrentConditionRow)
                        return;

                    if (conditions.SelectedRows.Count < 1)
                        return;
                    DataGridViewRow row = conditions.SelectedRows[0];
                    Template.Condition c = (Template.Condition)row.Tag;
                    if (c == null)//hacky forcing commit a newly added row and display the blank row
                    {
                        int i = conditions.Rows.Add();
                        row = conditions.Rows[i];
                        c = templateManager.CreateDefaultCondition();
                        setConditionRow(row, c);
                        row.Selected = true;
                        return;
                    }
                    setCurrentConditionRow(row);
                }
                catch (Exception ex)
                {
                    Message.Error2(ex, this);
                }
            };

            deleteCondition.LinkClicked += delegate
            {
                deleteSelectedCondition();
            };

            moveUpCondition.LinkClicked += delegate
            {
                moveUpSelectedCondition();
            };

            moveDownCondition.LinkClicked += delegate
            {
                moveDownSelectedCondition();
            };

            newCondition.LinkClicked += delegate
            {
                createNewCondition();
            };
        }

        void deleteSelectedCondition()
        {
            try
            {
                if (conditions.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r0 = conditions.SelectedRows[conditions.SelectedRows.Count - 1];
                if (r0.Tag == null)
                    return;
                Template.Condition c0 = (Template.Condition)r0.Tag;
                if (!Message.YesNo("Proceed with removing condition '" + c0.Name + "'?", this))
                    return;
                conditions.Rows.Remove(r0);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Message.Error(e, this);
            }
            finally
            {
                settingCurrentConditionRow = false;
            }
        }

        void moveUpSelectedCondition()
        {
            try
            {
                if (conditions.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = conditions.SelectedRows[conditions.SelectedRows.Count - 1];
                int i2 = r.Index - 1;
                if (i2 < 0)
                    return;
                settingCurrentConditionRow = true;//required due to fields-column error calculation when selected row changes
                conditions.Rows.Remove(r);
                conditions.Rows.Insert(i2, r);
                settingCurrentConditionRow = false;
                r.Selected = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Message.Error(e, this);
            }
            finally
            {
                settingCurrentConditionRow = false;
            }
        }

        void moveDownSelectedCondition()
        {
            try
            {
                if (conditions.SelectedRows.Count < 1)
                    return;
                DataGridViewRow r = conditions.SelectedRows[conditions.SelectedRows.Count - 1];
                int i2 = r.Index + 1;
                if (i2 > conditions.Rows.Count - 1)
                    return;
                settingCurrentConditionRow = true;//required due to fields-column error calculation when selected row changes
                conditions.Rows.Remove(r);
                conditions.Rows.Insert(i2, r);
                settingCurrentConditionRow = false;
                r.Selected = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Message.Error(e, this);
            }
            finally
            {
                settingCurrentConditionRow = false;
            }
        }

        void createNewCondition()
        {
            Template.Condition c = new Template.Condition { Name = "" };
            int i = conditions.CurrentRow?.Index >= 0 ? conditions.CurrentRow.Index + 1 : conditions.Rows.Count;
            conditions.Rows.Insert(i, 1);
            setConditionRow(conditions.Rows[i], c);
            conditions.Rows[i].Selected = true;
        }

        void setCurrentConditionRow(DataGridViewRow row)
        {
            if (settingCurrentConditionRow)
                return;
            try
            {
                settingCurrentConditionRow = true;

                currentConditionRow = row;

                if (row == null)
                {
                    conditions.ClearSelection();
                    conditions.CurrentCell = null;
                    return;
                }

                conditions.CurrentCell = conditions[0, row.Index];

                Template.Condition c = (Template.Condition)row.Tag;
                bool firstAnchor = true;
                foreach (int ai in BooleanEngine.GetAnchorIds(c.Value))
                {
                    Template.Anchor a = getAnchor(ai, out DataGridViewRow r);
                    if (r == null)
                        continue;
                    if (firstAnchor)
                    {
                        firstAnchor = false;
                        showAnchorRowAs(ai, rowStates.Linked, true);
                        setCurrentAnchorRow(ai, true);
                        clearImageFromBoxes();
                    }
                    else
                        showAnchorRowAs(ai, rowStates.Condition, false);
                    findAndDrawAnchor(ai);
                }
                if (firstAnchor)
                {
                    showAnchorRowAs(null, rowStates.NULL, true);
                    setCurrentAnchorRow(null, true);
                }

                setCurrentFieldRow(null);

                setConditionStatus(currentConditionRow);
            }
            finally
            {
                settingCurrentConditionRow = false;
            }
        }
        bool settingCurrentConditionRow = false;
        DataGridViewRow currentConditionRow = null;

        void setConditionRow(DataGridViewRow row, Template.Condition c)
        {
            row.Tag = c;
            c.Value = BooleanEngine.GetFormatted(c.Value);
            row.Cells["Name2"].Value = c.Name;
            row.Cells["Value2"].Value = c.Value;

            if (loadingTemplate)
                return;

            if (row == currentConditionRow)
                setCurrentConditionRow(row);
        }

        void setConditionsStatus()
        {
            if (pages == null)
                return;
            foreach (DataGridViewRow r in conditions.Rows)
                setConditionStatus(r);

            List<int> conditionAnchorIds = new List<int>();
            foreach (DataGridViewRow r in conditions.Rows)
            {
                Template.Condition c = (Template.Condition)r.Tag;
                if (c != null && c.IsSet())
                    conditionAnchorIds.AddRange(BooleanEngine.GetAnchorIds(c.Value));
            }
            foreach (int anchorId in conditionAnchorIds.Distinct())
                _setAnchorStatus(anchorId);
        }
        void _setAnchorStatus(int anchorId)
        {
            Template.Anchor a = getAnchor(anchorId, out DataGridViewRow row);
            if (a == null || row == null)
                return;

            if (pages == null)
                return;

            a = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[Id=" + a.Id + "] does not exist.");

            bool set = true;
            for (Template.Anchor a_ = a; a_ != null; a_ = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == a_.ParentAnchorId))
                if (!a_.IsSet())
                {
                    set = false;
                    getAnchor(a_.Id, out DataGridViewRow r_);
                    setRowStatus(statuses.WARNING, r_, "Not set");
                }
            if (!set)
                return;
            Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(a.Id);
            getAnchor(a.Id, out DataGridViewRow r);
            if (!aai.Found)
                setRowStatus(statuses.ERROR, r, "Not found");
            else
                setRowStatus(statuses.SUCCESS, r, "Found");
        }

        void setConditionStatus(DataGridViewRow r)
        {
            if (pages == null)
                return;
            Template.Condition c = (Template.Condition)r.Tag;
            if (c == null)
                return;
            if (!c.IsSet())
            {
                setRowStatus(statuses.NEUTRAL, r, "");
                return;
            }
            try
            {
                pages.ActiveTemplate = GetTemplateFromUI(false);
                if (pages[currentPageI].IsCondition(c.Name))
                    setRowStatus(statuses.SUCCESS, r, "True");
                else
                    setRowStatus(statuses.ERROR, r, "False");
            }
            catch (Exception e)
            {
                setRowStatus(statuses.WRONG, r, e.Message);
            }
        }
    }
}