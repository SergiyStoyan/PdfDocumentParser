//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

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

                if (t.TextAutoInsertSpace == null)
                    t.TextAutoInsertSpace = new TextAutoInsertSpace();
                textAutoInsertSpaceThreshold.Value = (decimal)t.TextAutoInsertSpace.Threshold;
                textAutoInsertSpaceRepresentative.Text = Regex.Escape(t.TextAutoInsertSpace.Representative);

                CvImageScalePyramidStep.Value = t.CvImageScalePyramidStep;
                TesseractPageSegMode.SelectedItem = t.TesseractPageSegMode;

                SingleFieldFromFieldImage.Checked = t.FieldOcrMode.HasFlag(Template.FieldOcrModes.SingleFieldFromFieldImage);
                ColumnFieldFromFieldImage.Checked = t.FieldOcrMode.HasFlag(Template.FieldOcrModes.ColumnFieldFromFieldImage);

                bitmapPreparationForm.SetUI(t, false);

                anchors.Rows.Clear();
                if (t.Anchors != null)
                {
                    foreach (Template.Anchor a in t.Anchors)
                    {
                        int i = anchors.Rows.Add();
                        var row = anchors.Rows[i];
                        if (a.ParentAnchorId != null)//to avoid validation error
                            ((DataGridViewComboBoxCell)row.Cells[anchors.Columns["ParentAnchorId3"].Index]).DataSource = new List<dynamic> { new { Id = a.ParentAnchorId, Name = a.ParentAnchorId.ToString() } };
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

                //for (int i = 0; i < t.Fields.Count; i++)
                //{
                //    for (int j = i + 1; j < t.Fields.Count; j++)
                //        if (t.Fields[i].Name == t.Fields[j].Name)
                //            t.Fields.RemoveAt(j);
                //}
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
                ShowFieldTextLineSeparators.Checked = t.Editor.ShowFieldTextLineSeparators;
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
            //TextForm tf = new TextForm("Pdf Entity Text", PdfTextExtractor.GetTextFromPage(pages.PdfReader, currentPageI), false);
            TextForm tf = new TextForm("Pdf Entity Text", Page.GetText(pages[currentPageI].PdfCharBoxs, new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpaceThreshold.Value, Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text) }), false);
            tf.ShowDialog();
        }

        private void ShowOcrText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (pages == null)
                return;
            List<string> ls = Page.GetTextLines(pages[currentPageI].ActiveTemplateOcrCharBoxs, new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpaceThreshold.Value, Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text) });
            TextForm tf = new TextForm("OCR Text", string.Join("\r\n", ls), false);
            tf.ShowDialog();
        }

        private void showAsJson_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Template t = GetTemplateFromUI(true);
            TextForm tf = new TextForm("Template JSON object", Serialization.Json.Serialize(t), true);
            while (tf.ShowDialog() == DialogResult.OK)
                try
                {
                    Template t2 = Serialization.Json.Deserialize<Template>(tf.Content);
                    t2.Editor = t.Editor;
                    setUIFromTemplate(t2);
                    break;
                }
                catch (Exception ex)
                {
                    Message.Error2("Updating template:", ex);
                }
        }

        private void Help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            templateManager.HelpRequest();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                templateManager.Template = GetTemplateFromUI(true);
                templateManager.Save();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }

        internal Template GetTemplateFromUI(bool saving)
        {
            Template t = new Template();

            if (saving && string.IsNullOrWhiteSpace(name.Text))
                throw new Exception("Name is empty!");

            t.Name = name.Text.Trim();

            t.TextAutoInsertSpace = new TextAutoInsertSpace
            {
                Threshold = (float)textAutoInsertSpaceThreshold.Value,
                Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text),
            };

            t.CvImageScalePyramidStep = (int)CvImageScalePyramidStep.Value;
            t.TesseractPageSegMode = (Tesseract.PageSegMode)TesseractPageSegMode.SelectedItem;

            if (SingleFieldFromFieldImage.Checked)
                t.FieldOcrMode |= Template.FieldOcrModes.SingleFieldFromFieldImage;
            else
                t.FieldOcrMode &= ~Template.FieldOcrModes.SingleFieldFromFieldImage;
            if (ColumnFieldFromFieldImage.Checked)
                t.FieldOcrMode |= Template.FieldOcrModes.ColumnFieldFromFieldImage;
            else
                t.FieldOcrMode &= ~Template.FieldOcrModes.ColumnFieldFromFieldImage;

            bitmapPreparationForm.SetTemplate(t);

            bool? removeNotLinkedAnchors = null;
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
            t.Anchors = new List<Template.Anchor>();
            foreach (DataGridViewRow r in anchors.Rows)
            {
                Template.Anchor a = (Template.Anchor)r.Tag;
                if (a == null)
                    continue;

                if (saving)
                {
                    if (!a.IsSet())
                        throw new Exception("Anchor[Id=" + a.Id + "] is not set!");

                    if (!conditionAnchorIds.Contains(a.Id))
                    {
                        bool engaged = false;
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
                        {
                            foreach (DataGridViewRow rr in fields.Rows)
                            {
                                Template.Field m = (Template.Field)rr.Tag;
                                if (m != null && (m.LeftAnchor?.Id == a.Id || m.TopAnchor?.Id == a.Id || m.RightAnchor?.Id == a.Id || m.BottomAnchor?.Id == a.Id))
                                {
                                    engaged = true;
                                    break;
                                }
                            }
                            if (!engaged)
                            {
                                if (a.Id != t.ScalingAnchorId)
                                {
                                    if (removeNotLinkedAnchors == null)
                                        removeNotLinkedAnchors = Message.YesNo("The template contains not linked anchor[s]. Remove them?");
                                    if (removeNotLinkedAnchors == true)
                                        continue;
                                }
                            }
                        }
                    }
                }
                t.Anchors.Add((Template.Anchor)Serialization.Json.Clone2(a));
            }
            t.Anchors = t.Anchors.OrderBy(a => a.Id).ToList();

            if (saving)
                t.GetScalingAnchor();//check is it is correct;

            t.Conditions = new List<Template.Condition>();
            foreach (DataGridViewRow r in conditions.Rows)
            {
                Template.Condition c = (Template.Condition)r.Tag;
                if (c == null)
                    continue;
                if (saving)
                {
                    if (!c.IsSet())
                        throw new Exception("Condition[name=" + c.Name + "] is not set!");
                    BooleanEngine.Check(c.Value, t.Anchors.Select(x => x.Id));
                }
                t.Conditions.Add(Serialization.Json.Clone(c));
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
                if (saving)
                    foreach (int? ai in new List<int?> { f.LeftAnchor?.Id, f.TopAnchor?.Id, f.RightAnchor?.Id, f.BottomAnchor?.Id })
                        if (ai != null && t.Anchors.FirstOrDefault(x => x.Id == ai) == null)
                            throw new Exception("Anchor[Id=" + ai + " does not exist.");
                t.Fields.Add(Serialization.Json.Clone(f));
            }
            if (saving)
            {
                if (t.Fields.Count < 1)
                    throw new Exception("Fields is empty!");
                //var dfs = t.Fields.GroupBy(x => x.Name).Where(x => x.Count() > 1).FirstOrDefault();
                //if (dfs != null)
                //    throw new Exception("Field '" + dfs.First().Name + "' is duplicated!");

                //foreach (string columnOfTable in t.Fields.Where(x => x is Template.Field.PdfText).Select(x => (Template.Field.PdfText)x).Where(x => x.ColumnOfTable != null).Select(x => x.ColumnOfTable))
                //{
                //    Dictionary<string, List<Template.Field>> fieldName2orderedFields = new Dictionary<string, List<Template.Field>>();
                //    foreach (Template.Field.PdfText pt in t.Fields.Where(x => x is Template.Field.PdfText).Select(x => (Template.Field.PdfText)x).Where(x => x.ColumnOfTable == columnOfTable))
                //    {
                //        List<Template.Field> fs;
                //        if (!fieldName2orderedFields.TryGetValue(pt.Name, out fs))
                //        {
                //            fs = new List<Template.Field>();
                //            fieldName2orderedFields[pt.Name] = fs;
                //        }
                //        fs.Add(pt);
                //    }
                //    int definitionCount = fieldName2orderedFields.Max(x => x.Value.Count());
                //    foreach (string fn in fieldName2orderedFields.Keys)
                //        if (definitionCount > fieldName2orderedFields[fn].Count)
                //            throw new Exception("Field '" + fn + "' is column of table " + columnOfTable + " and so it must have the same number of definitions as the rest column fields!");
                //}

                //foreach (Template.Field f0 in t.Fields)
                //{
                //    int tableDefinitionsCount = t.Fields.Where(x => x.Name == f0.Name).Count();
                //    string fn = t.Fields.Where(x => x.ColumnOfTable == f0.Name).GroupBy(x => x.Name).Where(x => x.Count() > tableDefinitionsCount).FirstOrDefault()?.First().Name;
                //    if (fn != null)
                //        throw new Exception("Field '" + fn + "' is a column of table field " + f0.Name + " so " + f0.Name + " must have number of definitions not less then that of " + fn + ".");
                //}

                //it is done only to avoid unneeded OCR
                //string fn = t.Fields.Where(x => x.ColumnOfTable != null && (x.DefaultValueType != Template.Field.ValueTypes.PdfText && x.DefaultValueType != Template.Field.ValueTypes.PdfTextLines)).FirstOrDefault()?.Name;
                //if (fn != null)
                //    throw new Exception("Field '" + fn + "' is a column of a table field. Its default value type can be either " + Template.Field.ValueTypes.PdfText + " or " + Template.Field.ValueTypes.PdfTextLines);
            }

            if (saving)
            {
                t.Editor = new Template.EditorSettings
                {
                    TestFile = testFile.Text,
                    TestPictureScale = pictureScale.Value,
                    ExtractFieldsAutomaticallyWhenPageChanged = ExtractFieldsAutomaticallyWhenPageChanged.Checked,
                    ShowFieldTextLineSeparators = ShowFieldTextLineSeparators.Checked,
                    CheckConditionsAutomaticallyWhenPageChanged = CheckConditionsAutomaticallyWhenPageChanged.Checked,
                };
            }

            return t;
        }
    }
}