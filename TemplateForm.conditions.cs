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
        void initializeConditionsTable()
        {
            conditions.EnableHeadersVisualStyles = false;//needed to set row headers

            conditions.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                DataGridViewRow r = anchors.Rows[e.RowIndex];
                Message.Error("Condition[" + r.Index + "] has unacceptable value of " + conditions.Columns[e.ColumnIndex].HeaderText + ":\r\n" + e.Exception.Message);
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
                    Message.Error2(ex);
                }
            };
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
                    if (firstAnchor)
                    {
                        showAnchorRowAs(ai, rowStates.NULL, true);
                        setCurrentAnchorRow(ai, true);
                    }
                    else
                        showAnchorRowAs(ai, rowStates.Condition, false);
                    findAndDrawAnchor(ai, firstAnchor);
                    firstAnchor = false;
                }
                if (firstAnchor)
                {
                    showAnchorRowAs(null, rowStates.NULL, true);
                    setCurrentAnchorRow(null, true);
                }

                    setCurrentFieldRow(null);
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
            pages.ActiveTemplate = getTemplateFromUI(false);
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
            DataGridViewRow row;
            Template.Anchor a = getAnchor(anchorId, out row);
            if (a == null || row == null)
                return;

            if (pages == null)
                return;

            pages.ActiveTemplate = getTemplateFromUI(false);
            a = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[Id=" + a.Id + "] is not defined.");

            for (; a != null; a = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == a.ParentAnchorId))
            {
                DataGridViewRow r;
                getAnchor(a.Id, out r);
                if (!a.IsSet())
                    setRowStatus(statuses.WARNING, r, "Not set");
                List<RectangleF> rs = pages[currentPage].GetAnchorRectangles(a);
                if (rs == null || rs.Count < 1)
                    setRowStatus(statuses.ERROR, r, "Not found");
                else
                    setRowStatus(statuses.SUCCESS, r, "Found");
            }
        }

        void setConditionStatus(DataGridViewRow r)
        {
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
                if (pages[currentPage].IsCondition(c.Name))
                    setRowStatus(statuses.SUCCESS, r, "Match");
                else
                    setRowStatus(statuses.ERROR, r, "Not match");
            }
            catch (Exception e)
            {
                setRowStatus(statuses.WRONG, r, e.Message);
            }
        }
    }
}