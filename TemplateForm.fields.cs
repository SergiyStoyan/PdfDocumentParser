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
                        case "Rectangle":
                            {
                                string r_ = (string)cs["Rectangle"].Value;
                                if (r_ == null)
                                    f.Rectangle = null;
                                else
                                    f.Rectangle = SerializationRoutines.Json.Deserialize<Template.RectangleF>(r_);
                                cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                                break;
                            }
                        case "Ocr":
                            {
                                Template.Field f2;
                                if (Convert.ToBoolean(cs["Ocr"].Value))
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
                                f = f2;
                                row.Tag = f;
                                cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                                //if (cs["Value"].Value == null)
                                //    setRowStatus(statuses.WARNING, row, "Not found");
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
                                        cs["Rectangle"].Value = SerializationRoutines.Json.Serialize(f.Rectangle);
                                    }
                                }
                                else//anchor deselected
                                {
                                    setRowStatus(statuses.WARNING, row, "Changed");
                                    cs["Value"].Value = extractValueAndDrawSelectionBox(f.FloatingAnchorId, f.Rectangle, f.Type);
                                }
                                break;
                            }
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

                    var vt = Convert.ToBoolean(cs["Ocr"].Value) ? Template.Types.OcrText : Template.Types.PdfText;
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
    }
}