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
using System.Linq.Expressions;
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
        enum statuses
        {
            SUCCESS,
            NEUTRAL,
            WARNING,
            ERROR,
            WRONG
        }

        void setRowStatus(statuses s, DataGridViewRow r, string m)
        {
            r.HeaderCell.Value = m;
            switch (s)
            {
                case statuses.SUCCESS:
                    r.HeaderCell.Style.BackColor = Color.LightGreen;
                    break;
                case statuses.ERROR:
                    r.HeaderCell.Style.BackColor = Color.Pink;
                    break;
                case statuses.WRONG:
                    r.HeaderCell.Style.BackColor = Color.Red;
                    break;
                case statuses.WARNING:
                    r.HeaderCell.Style.BackColor = Color.Yellow;
                    break;
                case statuses.NEUTRAL:
                    r.HeaderCell.Style.BackColor = SystemColors.Control;
                    break;
                default:
                    throw new Exception("Unknown option: " + s);
            }
        }

        class conditionComparer : IEqualityComparer<Template.Condition>
        {
            bool IEqualityComparer<Template.Condition>.Equals(Template.Condition x, Template.Condition y)
            {
                return x.Name == y.Name;
            }

            int IEqualityComparer<Template.Condition>.GetHashCode(Template.Condition obj)
            {
                throw new NotImplementedException();
            }
        }

        void setUIFromTemplate(Template t)
        {
            try
            {
                loadingTemplate = true;

                name.Text = t.Name;

                //imageResolution.Value = template.ImageResolution;

                textAutoInsertSpaceThreshold.Value = (decimal)t.TextAutoInsertSpaceThreshold;

                pageRotation.SelectedIndex = (int)t.PageRotation;
                autoDeskew.Checked = t.AutoDeskew;
                autoDeskewThreshold.Value = t.AutoDeskewThreshold;

                anchors.Rows.Clear();
                if (t.Anchors != null)
                {
                    foreach (Template.Anchor a in t.Anchors)
                    {
                        int i = anchors.Rows.Add();
                        var row = anchors.Rows[i];
                        if (a.ParentAnchorId != null)
                        {
                            DataGridViewComboBoxCell c = row.Cells[anchors.Columns["ParentAnchorId3"].Index] as DataGridViewComboBoxCell;
                            c.DataSource = new List<dynamic> { new { Id = a.ParentAnchorId, Name = a.ParentAnchorId.ToString() } };//to avoid error message
                        }
                        setAnchorRow(row, a);
                    }
                    onAnchorsChanged();

                    foreach (DataGridViewRow r in anchors.Rows)
                        setAnchorParentAnchorIdList(r);
                }

                for (int i = 0; i < t.Conditions.Count; i++)
                {
                    for (int j = i + 1; j < t.Conditions.Count; j++)
                        if (t.Conditions[i].Name == t.Conditions[j].Name)
                            t.Conditions.RemoveAt(j);
                }
                conditions.Rows.Clear();
                foreach (Template.Condition c in t.Conditions)
                {
                    int i = conditions.Rows.Add();
                    var row = conditions.Rows[i];
                    setConditionRow(row, c);
                }

                for (int i = 0; i < t.Fields.Count; i++)
                {
                    for (int j = i + 1; j < t.Fields.Count; j++)
                        if (t.Fields[i].Name == t.Fields[j].Name)
                            t.Fields.RemoveAt(j);
                }
                fields.Rows.Clear();
                if (t.Fields != null)
                {
                    foreach (Template.Field f in t.Fields)
                    {
                        int i = fields.Rows.Add();
                        var row = fields.Rows[i];
                        setFieldRow(row, f);
                    }
                }

                pictureScale.Value = t.Editor.TestPictureScale > 0 ? t.Editor.TestPictureScale : 1;

                ExtractFieldsAutomaticallyWhenPageChanged.Checked = t.Editor.ExtractFieldsAutomaticallyWhenPageChanged;
                CheckConditionsAutomaticallyWhenPageChanged.Checked = t.Editor.CheckConditionsAutomaticallyWhenPageChanged;

                if (t.Editor.TestFile != null && File.Exists(t.Editor.TestFile))
                    testFile.Text = t.Editor.TestFile;
                else
                {
                    if (templateManager.LastTestFile != null && File.Exists(templateManager.LastTestFile))
                        testFile.Text = templateManager.LastTestFile;
                }
            }
            finally
            {
                loadingTemplate = false;
            }
        }
        bool loadingTemplate = false;

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void About_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
        
        private void Configure_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
        }

        private void ShowPdfText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pages == null)
                return;
            TextForm tf = new TextForm("Pdf Entity Text", PdfTextExtractor.GetTextFromPage(pages.PdfReader, currentPage), false);
            tf.ShowDialog();
        }

        private void ShowOcrText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pages == null)
                return;
            //TextForm tf = new TextForm("OCR Text", PdfDocumentParser.Ocr.This.GetHtml(pages[currentPage].Bitmap), true);
            TextForm tf = new TextForm("OCR Text", PdfDocumentParser.Ocr.GetText(pages[currentPage].ActiveTemplateOcrCharBoxs), false);
            tf.ShowDialog();
        }

        private void showJson_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Template t = getTemplateFromUI(false);
            TextForm tf = new TextForm("Template JSON object", SerializationRoutines.Json.Serialize(t), false);
            tf.ShowDialog();
        }

        private void Help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            templateManager.HelpRequest();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                templateManager.Template = getTemplateFromUI(true);
                templateManager.Save();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }

        Template getTemplateFromUI(bool saving)
        {
            Template t = new Template();

            if (saving && string.IsNullOrWhiteSpace(name.Text))
                throw new Exception("Name is empty!");

            t.Name = name.Text.Trim();

            t.TextAutoInsertSpaceThreshold = (float)textAutoInsertSpaceThreshold.Value;

            t.PageRotation = (Template.PageRotations)pageRotation.SelectedIndex;
            t.AutoDeskew = autoDeskew.Checked;
            t.AutoDeskewThreshold = (int)autoDeskewThreshold.Value;

            bool? removeNotLinkedAnchors = null;
            t.Anchors = new List<Template.Anchor>();
            List<int> conditionAnchorIds = null;
            if (saving)
            {
                conditionAnchorIds = new List<int>();
                foreach (DataGridViewRow r in conditions.Rows)
                {
                    Template.Condition c = (Template.Condition)r.Tag;
                    if (c != null && c.IsSet())
                        conditionAnchorIds.AddRange(BooleanEngine.GetAnchorIds(c.Value));
                }
                conditionAnchorIds = conditionAnchorIds.Distinct().ToList();
            }
            foreach (DataGridViewRow r in anchors.Rows)
            {
                Template.Anchor a = (Template.Anchor)r.Tag;
                if (a == null)
                    continue;

                if (saving)
                {
                    if (!a.IsSet())
                        throw new Exception("Anchor[Id=" + a.Id + "] is not set!");

                    bool engaged = false;
                    if (conditionAnchorIds.Contains(a.Id))
                        engaged = true;
                    if (!engaged)
                        foreach (DataGridViewRow rr in anchors.Rows)
                        {
                            Template.Anchor a_ = (Template.Anchor)rr.Tag;
                            if (a_ == null)
                                continue;
                            if (a_.ParentAnchorId == a.Id)
                            {
                                engaged = true;
                                break;
                            }
                        }
                    if (!engaged)
                        foreach (DataGridViewRow rr in fields.Rows)
                        {
                            Template.Field m = (Template.Field)rr.Tag;
                            if (m != null && m.AnchorId == a.Id)
                            {
                                engaged = true;
                                break;
                            }
                        }
                    if (!engaged)
                    {
                        if (removeNotLinkedAnchors == null)
                            removeNotLinkedAnchors = Message.YesNo("The template contains not linked anchor[s]. Should they be removed?");
                        if (removeNotLinkedAnchors == true)
                            continue;
                    }
                }

                t.Anchors.Add(a);
            }
            t.Anchors = t.Anchors.OrderBy(a => a.Id).ToList();

            t.Conditions = new List<Template.Condition>();
            foreach (DataGridViewRow r in conditions.Rows)
            {
                Template.Condition c = (Template.Condition)r.Tag;
                if (c == null)
                    continue;
                if (!c.IsSet())
                {
                    if (saving && !string.IsNullOrWhiteSpace(c.Name))
                        throw new Exception("Condition[row=" + (r.Index + 1) + "] is not set!");
                    continue;
                }
                if (saving)
                    BooleanEngine.Check(c.Value, t.Anchors.Select(x => x.Id));
                t.Conditions.Add(c);
            }
            if (saving)
            {
                var dcs = t.Conditions.GroupBy(x => x.Name).Where(x => x.Count() > 1).FirstOrDefault();
                if (dcs != null)
                    throw new Exception("Condition '" + dcs.First().Name + "' is duplicated!");
            }

            t.Fields = new List<Template.Field>();
            foreach (DataGridViewRow r in fields.Rows)
            {
                Template.Field f = (Template.Field)r.Tag;
                if (f == null)
                    continue;
                if (saving && !f.IsSet())
                    throw new Exception("Field[" + r.Index + "] is not set!");
                if (saving && f.AnchorId != null && t.Anchors.FirstOrDefault(x => x.Id == f.AnchorId) == null)
                    throw new Exception("Anchor[Id=" + f.AnchorId + " does not exist.");
                t.Fields.Add(f);
            }
            if (saving)
            {
                if (t.Fields.Count < 1)
                    throw new Exception("Fields is empty!");
                var dfs = t.Fields.GroupBy(x => x.Name).Where(x => x.Count() > 1).FirstOrDefault();
                if (dfs != null)
                    throw new Exception("Field '" + dfs.First().Name + "' is duplicated!");
            }

            if (saving)
            {
                t.Editor = new Template.EditorSettings
                {
                    TestFile = testFile.Text,
                    TestPictureScale = pictureScale.Value,
                    ExtractFieldsAutomaticallyWhenPageChanged = ExtractFieldsAutomaticallyWhenPageChanged.Checked,
                    CheckConditionsAutomaticallyWhenPageChanged = CheckConditionsAutomaticallyWhenPageChanged.Checked,
                };
            }

            return t;
        }
    }
}