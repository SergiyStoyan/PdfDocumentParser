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
                var row = anchors.Rows[e.RowIndex];
                switch (conditions.Columns[e.ColumnIndex].Name)
                {
                    case "Condition2":
                        {
                            break;
                        }
                    case "Expression2":
                        {
                            break;
                        }
                }
                setConditionsStatus();
            };

            conditions.SelectionChanged += delegate (object sender, EventArgs e)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };
        }

        void setConditionsStatus()
        {
            if (pages == null)
                return;
            pages.ActiveTemplate = getTemplateFromUI(false);
            foreach (DataGridViewRow r in conditions.Rows)
            {
                if (r.IsNewRow)
                    continue;
                if (string.IsNullOrWhiteSpace((string)r.Cells["Expression2"].Value))
                {
                    setRowStatus(statuses.NEUTRAL, r, "");
                    continue;
                }
                try
                {
                    if (pages[currentPage].IsCondition((string)r.Cells["Condition2"].Value))
                        setRowStatus(statuses.SUCCESS, r, "Match");
                    else
                        setRowStatus(statuses.ERROR, r, "Not match");
                    //findAndDrawAnchor(a.Id);//to set anchor status
                }
                catch (Exception e)
                {
                    setRowStatus(statuses.WRONG, r, e.Message);
                }
            }
        }
    }
}