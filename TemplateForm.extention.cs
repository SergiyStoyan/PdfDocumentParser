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
        enum statuses
        {
            SUCCESS,
            NEUTRAL,
            WARNING,
            ERROR,
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

        void setUIFromTemplate(Template t)
        {
            try
            {
                loadingTemplate = true;

                name.Text = t.Name;

                //imageResolution.Value = template.ImageResolution;

                pageRotation.SelectedIndex = (int)t.PagesRotation;
                autoDeskew.Checked = t.AutoDeskew;
                autoDeskewThreshold.Value = t.AutoDeskewThreshold;

                floatingAnchors.Rows.Clear();
                if (t.FloatingAnchors != null)
                {
                    foreach (Template.FloatingAnchor fa in t.FloatingAnchors)
                    {
                        int i = floatingAnchors.Rows.Add();
                        var row = floatingAnchors.Rows[i];
                        row.Tag = fa;
                        row.Cells["Id3"].Value = fa.Id;
                        row.Cells["Type3"].Value = fa.Type;
                    }
                    onFloatingAnchorsChanged();
                }

                documentFirstPageRecognitionMarks.Rows.Clear();
                if (t.DocumentFirstPageRecognitionMarks != null)
                {
                    foreach (Template.Mark m in t.DocumentFirstPageRecognitionMarks)
                    {
                        int i = documentFirstPageRecognitionMarks.Rows.Add();
                        var row = documentFirstPageRecognitionMarks.Rows[i];
                        var cs = row.Cells;
                        cs["Type2"].Value = m.Type;
                        cs["FloatingAnchorId2"].Value = m.FloatingAnchorId;
                        row.Tag = m;
                    }
                }

                fields.Rows.Clear();
                if (t.Fields != null)
                {
                    foreach (Template.Field f in t.Fields)
                    {
                        int i = fields.Rows.Add();
                        var row = fields.Rows[i];
                        var cs = row.Cells;
                        cs["Name_"].Value = f.Name;
                        cs["Rectangle"].Value = f.Rectangle == null ? null : SerializationRoutines.Json.Serialize(f.Rectangle);
                        cs["Ocr"].Value = f.Type == Template.Types.PdfText ? false : true;
                        cs["FloatingAnchorId"].Value = f.FloatingAnchorId;
                        row.Tag = f;
                    }
                }

                pictureScale.Value = t.Editor.TestPictureScale > 0 ? t.Editor.TestPictureScale : 1;

                ExtractFieldsAutomaticallyWhenPageChanged.Checked = t.Editor.ExtractFieldsAutomaticallyWhenPageChanged;

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

        private void SaveAsInitialTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Template t = getTemplateFromUI(true);
                templateManager.SaveAsInitialTemplate(t);
                Message.Inform("Saved");
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
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

        private void Help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            templateManager.HelpRequest();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                //NewTemplate = getTemplateFromUI(true);
                Template t = getTemplateFromUI(true);
                templateManager.ReplaceWith(t);
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

            if (string.IsNullOrWhiteSpace(name.Text))
                if (saving)
                    throw new Exception("Name is empty!");
            
            t.Name = name.Text.Trim();

            t.PagesRotation = (Template.PageRotations)pageRotation.SelectedIndex;
            t.AutoDeskew = autoDeskew.Checked;
            t.AutoDeskewThreshold = (int)autoDeskewThreshold.Value;

            bool? removeNotLinkedAnchors = null;
            t.FloatingAnchors = new List<Template.FloatingAnchor>();
            foreach (DataGridViewRow r in floatingAnchors.Rows)
            {
                Template.FloatingAnchor fa = (Template.FloatingAnchor)r.Tag;
                if (fa != null)
                {
                    if (saving)
                    {
                        bool linked = false;
                        foreach (DataGridViewRow rr in documentFirstPageRecognitionMarks.Rows)
                        {
                            Template.Mark m = (Template.Mark)rr.Tag;
                            if (m != null && m.FloatingAnchorId == fa.Id)
                            {
                                linked = true;
                                break;
                            }
                        }
                        if (!linked)
                            foreach (DataGridViewRow rr in fields.Rows)
                            {
                                Template.Field m = (Template.Field)rr.Tag;
                                if (m != null && m.FloatingAnchorId == fa.Id)
                                {
                                    linked = true;
                                    break;
                                }
                            }
                        if (!linked)
                        {
                            if (removeNotLinkedAnchors == null)
                                removeNotLinkedAnchors = Message.YesNo("The template contains not linked anchor[s]. Should they be removed?");
                            if (removeNotLinkedAnchors == true)
                                continue;
                        }

                        if (!fa.IsSet())
                            throw new Exception("FloatingAnchor[Id=" + fa.Id + "] is not set!");
                    }

                    t.FloatingAnchors.Add(fa);
                }
            }
            t.FloatingAnchors = t.FloatingAnchors.OrderBy(a => a.Id).ToList();

            t.DocumentFirstPageRecognitionMarks = new List<Template.Mark>();
            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
            {
                Template.Mark m = (Template.Mark)r.Tag;
                if (m != null)
                {
                    if (!m.IsSet())
                        throw new Exception("DocumentFirstPageRecognitionMark[" + r.Index + "] is not set!");
                    if (m.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == m.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + m.FloatingAnchorId);
                    t.DocumentFirstPageRecognitionMarks.Add(m);
                }
            }
            if (saving && t.DocumentFirstPageRecognitionMarks.Count < 1)
                throw new Exception("DocumentFirstPageRecognitionMarks is empty!");

            t.Fields = new List<Template.Field>();
            foreach (DataGridViewRow r in fields.Rows)
            {
                Template.Field f = (Template.Field)r.Tag;
                if (f != null)
                {
                    if (!f.IsSet())
                        throw new Exception("Field[" + r.Index + "] is not set!");
                    if (f.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == f.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + f.FloatingAnchorId);
                    t.Fields.Add(f);
                }
            }
            if (saving && t.Fields.Count < 1)
                throw new Exception("Fields is empty!");

            if (saving)
            {
                if (t.Editor == null)
                    t.Editor = new Template.EditorSettings();
                t.Editor.TestFile = testFile.Text;
                t.Editor.TestPictureScale = pictureScale.Value;
                t.Editor.ExtractFieldsAutomaticallyWhenPageChanged = ExtractFieldsAutomaticallyWhenPageChanged.Checked;
            }

            return t;
        }
    }
}