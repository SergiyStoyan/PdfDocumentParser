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
        void setFloatingAnchorValue(DataGridViewRow row, object value)
        {
            row.Tag = value;
            if (loadingTemplate)
                return;
            setFloatingAnchorControl(row);
            int? fai = (int?)row.Cells["Id3"].Value;
            onFloatingAnchorsChanged(fai);
            if (row.Selected)
                findAndDrawFloatingAnchor(fai);
        }

        void setFloatingAnchorControl(DataGridViewRow row)
        {
            if (row == null || !row.Selected || row.IsNewRow || !floatingAnchors.Rows.Contains(row))
            {
                currentFloatingAnchorRow = null;
                currentFloatingAnchorControl = null;
                return;
            }
            currentFloatingAnchorRow = row;
            Template.ValueTypes valueType = (Template.ValueTypes)row.Cells["ValueType3"].Value;
            Control c = null;
            switch (valueType)
            {
                case Template.ValueTypes.PdfText:
                    {
                        c = new FloatingAnchorPdfTextControl((Template.FloatingAnchor.PdfTextValue)row.Tag);
                    }
                    break;
                case Template.ValueTypes.OcrText:
                    {
                        c = new FloatingAnchorOcrTextControl((Template.FloatingAnchor.OcrTextValue)row.Tag);
                    }
                    break;
                case Template.ValueTypes.ImageData:
                    {
                        c = new FloatingAnchorImageDataControl((Template.FloatingAnchor.ImageDataValue)row.Tag);
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            currentFloatingAnchorControl = c;
        }
        Control currentFloatingAnchorControl
        {
            get
            {
                if (floatingAnchorsContainer.Panel2.Controls.Count < 1)
                    return null;
                return floatingAnchorsContainer.Panel2.Controls[0];
            }
            set
            {
                floatingAnchorsContainer.Panel2.Controls.Clear();
                if (value == null)
                    return;
                floatingAnchorsContainer.Panel2.Controls.Add(value);
                value.Dock = DockStyle.Fill;
            }
        }
        DataGridViewRow currentFloatingAnchorRow = null;
        void setCurrentFloatingAnchorValueFromControl()
        {
            if (currentFloatingAnchorRow == null)
                return;
            object value = null;
            Template.ValueTypes valueType = (Template.ValueTypes)currentFloatingAnchorRow.Cells["ValueType3"].Value;
            switch (valueType)
            {
                case Template.ValueTypes.PdfText:
                    value = ((FloatingAnchorPdfTextControl)currentFloatingAnchorControl).Value;
                    break;
                case Template.ValueTypes.OcrText:
                    value = ((FloatingAnchorOcrTextControl)currentFloatingAnchorControl).Value;
                    break;
                case Template.ValueTypes.ImageData:
                    value = ((FloatingAnchorImageDataControl)currentFloatingAnchorControl).Value;
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            if (value != null)
                setFloatingAnchorValue(currentFloatingAnchorRow, value);
        }

        void setMarkValue(DataGridViewRow row, object value)
        {
            row.Tag = value;
            if (loadingTemplate)
                return;
            setMarkControl(row); 
        }
        void setMarkControl(DataGridViewRow row)
        {
            if (row == null || !row.Selected || row.IsNewRow || row.Tag == null || !documentFirstPageRecognitionMarks.Rows.Contains(row))
            {
                currentMarkRow = null;
                currentMarkControl = null;
                return;
            }
            currentMarkRow = row;
            Template.RectangleF r = (Template.RectangleF)row.Cells["Rectangle2"].Value;
            Template.ValueTypes valueType = (Template.ValueTypes)row.Cells["ValueType2"].Value;
            Control c = null;
            switch (valueType)
            {
                case Template.ValueTypes.PdfText:
                    {
                        c = new MarkPdfTextControl((Template.Mark.PdfTextValue)row.Tag, r);
                    }
                    break;
                case Template.ValueTypes.OcrText:
                    {
                        c = new MarkOcrTextControl((Template.Mark.OcrTextValue)row.Tag, r);
                    }
                    break;
                case Template.ValueTypes.ImageData:
                    {
                        c = new MarkImageDataControl((Template.Mark.ImageDataValue)row.Tag, r);
                    }
                    break;
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            currentMarkControl = c;
        }
        Control currentMarkControl
        {
            get
            {
                if (marksContainer.Panel2.Controls.Count < 1)
                    return null;
                return marksContainer.Panel2.Controls[0];
            }
            set
            {
                marksContainer.Panel2.Controls.Clear();
                if (value == null)
                    return;
                marksContainer.Panel2.Controls.Add(value);
                value.Dock = DockStyle.Fill;
            }
        }
        DataGridViewRow currentMarkRow = null;
        void setCurrentMarkValueFromControl()
        {
            if (currentMarkRow != null)
            {
                //    var cs = currentfloatingAnchorRow.Cells;
                object value = null;
                Template.ValueTypes valueType = (Template.ValueTypes)currentMarkRow.Cells["ValueType2"].Value;
                switch (valueType)
                {
                    case Template.ValueTypes.PdfText:
                        value = ((MarkPdfTextControl)currentMarkControl).Value;
                        break;
                    case Template.ValueTypes.OcrText:
                        value = ((MarkOcrTextControl)currentMarkControl).Value;
                        break;
                    case Template.ValueTypes.ImageData:
                        value = ((MarkImageDataControl)currentMarkControl).Value;
                        break;
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
                if (value != null)
                    setMarkValue(currentMarkRow, value);
            }
        }

        void onFloatingAnchorsChanged(int? updatedFloatingAnchorId)
        {
            SortedSet<int> fais = new SortedSet<int>();
            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value != null)
                    fais.Add((int)rr.Cells["Id3"].Value);

            foreach (DataGridViewRow rr in floatingAnchors.Rows)
                if (rr.Cells["Id3"].Value == null && rr.Tag != null && rr.Cells["ValueType3"].Value != null)
                {
                    int fai = 1;
                    //if (fais.Count > 0)
                    //    fai = fais.Max() + 1;                    
                    foreach (int i in fais)
                    {
                        if (fai < i)
                            break;
                        if (fai == i)
                            fai++;
                    }
                    fais.Add(fai);
                    rr.Cells["Id3"].Value = fai;
                }

            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
            {
                int? i = (int?)r.Cells["FloatingAnchorId2"].Value;
                if (i != null && !fais.Contains((int)i))
                {
                    r.Cells["FloatingAnchorId2"].Value = null;
                    r.Cells["Rectangle2"].Value = null;
                }
                if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                {
                    r.Cells["Rectangle2"].Value = null;
                }
            }
            foreach (DataGridViewRow r in fields.Rows)
            {
                int? i = (int?)r.Cells["FloatingAnchorId"].Value;
                if (i != null && !fais.Contains((int)i))
                {
                    r.Cells["FloatingAnchorId"].Value = null;
                    r.Cells["Rectangle"].Value = null;
                    r.Cells["Value"].Value = null;
                }
                //if (updatedFloatingAnchorId != null && i == updatedFloatingAnchorId)
                //{
                //    r.Cells["Rectangle"].Value = null;
                //    r.Cells["Value"].Value = null;
                //}
            }

            List<dynamic> fais_ = fais.Select(f => new { Id = f, Name = f.ToString() }).ToList<dynamic>();
            fais_.Insert(0, new { Id = -1, Name = string.Empty });//commbobox returns value null for -1 (and throws an unclear expection if Id=null)
            FloatingAnchorId2.DataSource = fais_;
            FloatingAnchorId.DataSource = fais_;
        }

        void setFloatingAnchorFromSelectedElements()
        {
            try
            {
                if (floatingAnchors.SelectedRows.Count < 1)
                    return;

                DataGridViewRow r = floatingAnchors.SelectedRows[0];
                var vt = (Template.ValueTypes)r.Cells["ValueType3"].Value;
                switch (vt)
                {
                    case Template.ValueTypes.PdfText:
                        {
                            List<Pdf.Line> lines = Pdf.RemoveDuplicatesAndGetLines(selectedPdfCharBoxs, false);
                            if (lines.Count < 1)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            Template.FloatingAnchor.PdfTextValue pte = new Template.FloatingAnchor.PdfTextValue { CharBoxs = new List<Template.FloatingAnchor.PdfTextValue.CharBox>() };
                            foreach (Pdf.Line l in lines)
                                foreach (Pdf.CharBox cb in l.CharBoxes)
                                    pte.CharBoxs.Add(new Template.FloatingAnchor.PdfTextValue.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            if (pte.CharBoxs.Count < 1)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            setFloatingAnchorValue(r, pte);
                        }
                        break;
                    case Template.ValueTypes.OcrText:
                        {
                            List<Ocr.Line> lines = PdfDocumentParser.Ocr.GetLines(selectedOcrCharBoxs);
                            if (lines.Count < 1)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            Template.FloatingAnchor.OcrTextValue ote = new Template.FloatingAnchor.OcrTextValue { CharBoxs = new List<Template.FloatingAnchor.OcrTextValue.CharBox>() };
                            foreach (Ocr.Line l in lines)
                                foreach (Ocr.CharBox cb in l.CharBoxes)
                                    ote.CharBoxs.Add(new Template.FloatingAnchor.OcrTextValue.CharBox
                                    {
                                        Char = cb.Char,
                                        Rectangle = new Template.RectangleF(cb.R.X, cb.R.Y, cb.R.Width, cb.R.Height),
                                    });
                            if (ote.CharBoxs.Count < 1)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            setFloatingAnchorValue(r, ote);
                        }
                        break;
                    case Template.ValueTypes.ImageData:
                        {
                            if (selectedImageDataValue.ImageBoxs.Count < 1)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            if (selectedImageDataValue.ImageBoxs.Where(x => x.ImageData == null).FirstOrDefault() != null)
                            {
                                setFloatingAnchorValue(r, null);
                                setStatus(statuses.WARNING, "FloatingAnchor[" + r.Cells["Id3"].Value + "] has not been set.");
                                return;
                            }
                            setFloatingAnchorValue(r, selectedImageDataValue);
                        }
                        break;
                    default:
                        throw new Exception("Unknown option: " + vt);
                }
            }
            finally
            {
                floatingAnchors.EndEdit();
                selectedPdfCharBoxs = null;
                selectedOcrCharBoxs = null;
                selectedImageDataValue = null;
            }
        }
        List<Pdf.CharBox> selectedPdfCharBoxs;
        List<Ocr.CharBox> selectedOcrCharBoxs;
        Template.FloatingAnchor.ImageDataValue selectedImageDataValue;

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
                        var cs = floatingAnchors.Rows[i].Cells;
                        cs["Id3"].Value = fa.Id;
                        cs["ValueType3"].Value = fa.ValueType;
                        cs["PositionDeviation3"].Value = fa.PositionDeviation;
                        setFloatingAnchorValue(floatingAnchors.Rows[i], fa.GetValue());
                    }
                    onFloatingAnchorsChanged(null);
                }

                documentFirstPageRecognitionMarks.Rows.Clear();
                if (t.DocumentFirstPageRecognitionMarks != null)
                {
                    foreach (Template.Mark m in t.DocumentFirstPageRecognitionMarks)
                    {
                        int i = documentFirstPageRecognitionMarks.Rows.Add();
                        var row = documentFirstPageRecognitionMarks.Rows[i];
                        var cs = row.Cells;
                        cs["Rectangle2"].Value = m.Rectangle;
                        cs["ValueType2"].Value = m.ValueType;
                        cs["FloatingAnchorId2"].Value = m.FloatingAnchorId;
                        setMarkValue(row, m.GetValue());
                    }
                }

                fields.Rows.Clear();
                if (t.Fields != null)
                {
                    foreach (Template.Field f in t.Fields)
                    {
                        int i = fields.Rows.Add();
                        var cs = fields.Rows[i].Cells;
                        cs["Name_"].Value = f.Name;
                        cs["Rectangle"].Value = f.Rectangle == null ? null : SerializationRoutines.Json.Serialize(f.Rectangle);
                        cs["Ocr"].Value = f.ValueType == Template.ValueTypes.PdfText ? false : true;
                        cs["FloatingAnchorId"].Value = f.FloatingAnchorId;
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

        private void save_Click(object sender, EventArgs e)
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
            Template t;
            if (saving)
                t = templateManager.New();
            else
                t = new Template();

            if (string.IsNullOrWhiteSpace(name.Text))
                if (saving)
                    throw new Exception("Name is empty!");

            if (documentFirstPageRecognitionMarks.Rows.Count < 2)
                if (saving)
                    throw new Exception("DocumentFirstPageRecognitionMarks is empty!");

            t.Name = name.Text.Trim();

            t.PagesRotation = (Template.PageRotations)pageRotation.SelectedIndex;
            t.AutoDeskew = autoDeskew.Checked;
            t.AutoDeskewThreshold = (int)autoDeskewThreshold.Value;

            bool? removeNotLinkedAnchors = null;
            t.FloatingAnchors = new List<Template.FloatingAnchor>();
            foreach (DataGridViewRow r in floatingAnchors.Rows)
                if (r.Cells["Id3"].Value != null)
                {
                    int floatingAnchorId = (int)r.Cells["Id3"].Value;

                    if (saving)
                    {
                        bool linked = false;
                        foreach (DataGridViewRow rr in documentFirstPageRecognitionMarks.Rows)
                            if (rr.Cells["FloatingAnchorId2"].Value != null)
                            {
                                int fai = (int)rr.Cells["FloatingAnchorId2"].Value;
                                if (fai == floatingAnchorId)
                                {
                                    linked = true;
                                    break;
                                }
                            }
                        if (!linked)
                            foreach (DataGridViewRow rr in fields.Rows)
                                if (rr.Cells["FloatingAnchorId"].Value != null)
                                {
                                    int fai = (int)rr.Cells["FloatingAnchorId"].Value;
                                    if (fai == floatingAnchorId)
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
                    }

                    if (r.Cells["PositionDeviation3"].Value == null)
                        r.Cells["PositionDeviation3"].Value = Settings.ImageProcessing.CoordinateDeviationMargin;//it must be > 0
                    float positionDeviation = (float)r.Cells["PositionDeviation3"].Value;
                    if (positionDeviation <= 0)
                        throw new Exception("FloatingAnchor[" + (int)r.Cells["Id3"].Value + "] has wrong Deviation. Deviation always must be a positive floating number due to internal image re-scaling.");

                    Template.FloatingAnchor fa = new Template.FloatingAnchor
                    {
                        Id = floatingAnchorId,
                        ValueType = (Template.ValueTypes)r.Cells["ValueType3"].Value,
                        PositionDeviation = positionDeviation,
                        ValueAsString = Template.FloatingAnchor.GetValueAsString((Template.ValueTypes)r.Cells["ValueType3"].Value, r.Tag),
                    };
                    //if (fa.GetValue() == null)
                    //    throw new Exception("FloatingAnchor[" + fa.Id + "] is not set.");
                    //if (fa.ValueType == Template.ValueTypes.ImageData)
                    //{
                    //    var v = (Template.FloatingAnchor.ImageDataValue)fa.GetValue();
                    //    if (v.ImageBoxs.Count < 1)
                    //        throw new Exception("FloatingAnchor[" + fa.Id + "] is malformed.");
                    //    if (v.ImageBoxs.Where(x => x.ImageData == null).FirstOrDefault() != null)
                    //        throw new Exception("FloatingAnchor[" + fa.Id + "] is malformed.");
                    //}
                    t.FloatingAnchors.Add(fa);
                }
            t.FloatingAnchors = t.FloatingAnchors.OrderBy(a => a.Id).ToList();

            t.DocumentFirstPageRecognitionMarks = new List<Template.Mark>();
            foreach (DataGridViewRow r in documentFirstPageRecognitionMarks.Rows)
                if (r.Tag != null || r.Cells["FloatingAnchorId2"].Value != null)
                {
                    Template.Mark m = new Template.Mark
                    {
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId2"].Value,
                        Rectangle = (Template.RectangleF)r.Cells["Rectangle2"].Value,
                        ValueType = (Template.ValueTypes)r.Cells["ValueType2"].Value,
                        ValueAsString = Template.Mark.GetValueAsString((Template.ValueTypes)r.Cells["ValueType2"].Value, r.Tag)
                    };
                    if (m.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == m.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + m.FloatingAnchorId);
                    t.DocumentFirstPageRecognitionMarks.Add(m);
                }

            t.Fields = new List<Template.Field>();
            foreach (DataGridViewRow r in fields.Rows)
            {
                string name = (string)r.Cells["Name_"].Value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    //if (r.Cells["Rectangle"].Value == null)
                    //{
                    //    if (saving)
                    //        throw new Exception("Field '" + name + "' is not set!");
                    //    continue;
                    //}
                    Template.Field f = new Template.Field
                    {
                        Name = name.Trim(),
                        Rectangle = r.Cells["Rectangle"].Value == null ? null : SerializationRoutines.Json.Deserialize<Template.RectangleF>((string)r.Cells["Rectangle"].Value),
                        ValueType = Convert.ToBoolean(r.Cells["Ocr"].Value) ? Template.ValueTypes.OcrText : Template.ValueTypes.PdfText,
                        FloatingAnchorId = (int?)r.Cells["FloatingAnchorId"].Value
                    };
                    if (f.FloatingAnchorId != null && t.FloatingAnchors.FirstOrDefault(x => x.Id == f.FloatingAnchorId) == null)
                        throw new Exception("There is no FloatingAnchor with Id=" + f.FloatingAnchorId);
                    t.Fields.Add(f);
                }
            }

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