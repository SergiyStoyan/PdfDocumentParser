﻿//********************************************************************************************
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
        public abstract class TemplateManager
        {
            /// <summary>
            /// this object is a buffer and can be changed unpredictably, so it should not be referenced from outside
            /// </summary>
            public Template Template;
            abstract public void Save();
            public string LastTestFile;
            public void HelpRequest()
            {
                try
                {
                    System.Diagnostics.Process.Start(Settings.Constants.HelpFile);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            }
            public Template.Anchor CreateDefaultAnchor()
            {
                return new Template.Anchor.PdfText { };
            }
            public Template.Field CreateDefaultField()
            {
                return new Template.Field.PdfText { };
            }
        }

        public TemplateForm(TemplateManager templateManager, string testFileDefaultFolder)
        {
            InitializeComponent();

            Icon = AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": Template Editor";

            this.templateManager = templateManager;

            initializeAnchorsTable();
            initializeConditionsTable();
            initializeFieldsTable();

            picture.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;
                drawingSelectionBox = true;
                selectionBoxPoint0 = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));
                selectionBoxPoint1 = new Point(selectionBoxPoint0.X, selectionBoxPoint0.Y);
                selectionBoxPoint2 = new Point(selectionBoxPoint0.X, selectionBoxPoint0.Y);
                selectionCoordinates.Text = selectionBoxPoint1.ToString();
            };

            picture.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;

                Point p = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));

                if (!drawingSelectionBox)
                {
                    selectionCoordinates.Text = p.ToString();
                    return;
                }

                if (selectionBoxPoint0.X < p.X)
                {
                    selectionBoxPoint1.X = selectionBoxPoint0.X;
                    selectionBoxPoint2.X = p.X;
                }
                else
                {
                    selectionBoxPoint1.X = p.X;
                    selectionBoxPoint2.X = selectionBoxPoint0.X;
                }
                if (selectionBoxPoint0.Y < p.Y)
                {
                    selectionBoxPoint1.Y = selectionBoxPoint0.Y;
                    selectionBoxPoint2.Y = p.Y;
                }
                else
                {
                    selectionBoxPoint1.Y = p.Y;
                    selectionBoxPoint2.Y = selectionBoxPoint0.Y;
                }
                selectionCoordinates.Text = selectionBoxPoint1.ToString() + ":" + selectionBoxPoint2.ToString();

                RectangleF r = new RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);
                drawBoxes(Settings.Appearance.SelectionBoxColor, new List<System.Drawing.RectangleF> { r }, true);
            };

            picture.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                try
                {
                    if (pages == null)
                        return;

                    if (!drawingSelectionBox)
                        return;
                    drawingSelectionBox = false;

                    Template.RectangleF r = new Template.RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);

                    switch (mode)
                    {
                        case Modes.SetAnchor:
                            {
                                if (currentAnchorControl == null)
                                    break;
                                
                                RectangleF selectedR = new RectangleF(selectionBoxPoint1, new SizeF(selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y));
                                currentAnchorControl.SetTagFromControl();
                                Template.Anchor a = (Template.Anchor)currentAnchorControl.Row.Tag;
                                switch (a.Type)
                                {
                                    case Template.Types.PdfText:
                                        if (selectedPdfCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedPdfCharBoxs = new List<Pdf.CharBox>();
                                        selectedPdfCharBoxs.AddRange(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPage].PdfCharBoxs, selectedR, true));
                                        break;
                                    case Template.Types.OcrText:
                                        if (selectedOcrCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedOcrCharBoxs = new List<Ocr.CharBox>();
                                        Template.Anchor.OcrText ot = a as Template.Anchor.OcrText;
                                        if (ot.OcrEntirePage)
                                            selectedOcrCharBoxs.AddRange(PdfDocumentParser.Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPage].ActiveTemplateOcrCharBoxs, selectedR));
                                        else
                                        {
                                            foreach (Ocr.CharBox cb in PdfDocumentParser.Ocr.This.GetCharBoxs(pages[currentPage].GetRectangleFromActiveTemplateBitmap(selectedR.X / Settings.Constants.Image2PdfResolutionRatio, selectedR.Y / Settings.Constants.Image2PdfResolutionRatio, selectedR.Width / Settings.Constants.Image2PdfResolutionRatio, selectedR.Height / Settings.Constants.Image2PdfResolutionRatio)))
                                            {
                                                cb.R.X += selectedR.X;
                                                cb.R.Y += selectedR.Y;
                                                selectedOcrCharBoxs.Add(cb);
                                            }
                                        }
                                        break;
                                    case Template.Types.ImageData:
                                        {
                                            if (selectedImageBoxs == null)
                                                selectedImageBoxs = new List<Template.Anchor.ImageData.ImageBox>();
                                            string error;
                                            ImageData id;
                                            using (Bitmap b = (Bitmap)pages[currentPage].GetValue(null, Template.RectangleF.GetFromSystemRectangleF(selectedR), Template.Types.ImageData, out error))
                                                id = new ImageData(b, false);
                                            //id = (ImageData)pages[currentPage].GetValue(null, Template.RectangleF.GetFromSystemRectangleF(selectedR), Template.Types.ImageData, out error);
                                            selectedImageBoxs.Add(new Template.Anchor.ImageData.ImageBox
                                            {
                                                Rectangle = Template.RectangleF.GetFromSystemRectangleF(selectedR),
                                                ImageData = id
                                            });
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown option: " + a.Type);
                                }

                                if ((ModifierKeys & Keys.Control) != Keys.Control)
                                    setAnchorFromSelectedElements();
                            }
                            break;
                        case Modes.SetField:
                            {
                                if (fields.SelectedRows.Count < 1)
                                    break;
                                var row = fields.SelectedRows[0];
                                Template.Field f = (Template.Field)row.Tag;
                                f.Rectangle = r;
                                setFieldRow(row, f);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            Shown += delegate
            {
                Application.DoEvents();//make form be drawn completely
                setUIFromTemplate(templateManager.Template);
            };

            FormClosed += delegate
            {
                if (scaledCurrentPageBitmap != null)
                {
                    scaledCurrentPageBitmap.Dispose();
                    scaledCurrentPageBitmap = null;
                }
                if (pages != null)
                {
                    pages.Dispose();
                    pages = null;
                }

                templateManager.LastTestFile = testFile.Text;
            };

            this.EnumControls((Control c) =>
            {
                SplitContainer s = c as SplitContainer;
                if (s != null)
                {
                    s.Panel1.BackColor = Color.Blue;
                    s.Panel2.BackColor = Color.Blue;
                    s.BackColor = Color.DarkGray;
                    s.SplitterWidth = 2;
                    s.Panel1.BackColor = SystemColors.Control;
                    s.Panel2.BackColor = SystemColors.Control;
                }
            }, true);

            testFile.TextChanged += delegate
            {
                try
                {
                    if (picture.Image != null)
                    {
                        picture.Image.Dispose();
                        picture.Image = null;
                    }
                    if (scaledCurrentPageBitmap != null)
                    {
                        scaledCurrentPageBitmap.Dispose();
                        scaledCurrentPageBitmap = null;
                    }
                    if (pages != null)
                    {
                        pages.Dispose();
                        pages = null;
                    }

                    if (string.IsNullOrWhiteSpace(testFile.Text))
                        return;

                    testFile.SelectionStart = testFile.Text.Length;
                    testFile.ScrollToCaret();

                    if (!File.Exists(testFile.Text))
                    {
                        LogMessage.Error("File '" + testFile.Text + "' does not exist!");
                        return;
                    }

                    pages = new PageCollection(testFile.Text);
                    totalPageNumber = pages.PdfReader.NumberOfPages;
                    lTotalPages.Text = " / " + totalPageNumber;
                    showPage(1);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            pictureScale.ValueChanged += delegate
            {
                if (!loadingTemplate)
                    setScaledImage();
            };

            pageRotation.SelectedIndexChanged += delegate
            {
                reloadPageBitmaps();
                //showPage(currentPage);
            };

            autoDeskew.CheckedChanged += delegate
            {
                reloadPageBitmaps();
                //showPage(currentPage);
            };

            Load += delegate
            {
                //if (marks.Rows.Count > 0 && !marks.Rows[0].IsNewRow)
                //    marks.Rows[0].Selected = true;
            };

            save.Click += Save_Click;
            Help.LinkClicked += Help_LinkClicked;
            Configure.LinkClicked += Configure_LinkClicked;
            cancel.Click += delegate { Close(); };

            bTestFile.Click += delegate (object sender, EventArgs e)
         {
             OpenFileDialog d = new OpenFileDialog();
             if (!string.IsNullOrWhiteSpace(testFile.Text))
                 d.InitialDirectory = PathRoutines.GetDirFromPath(testFile.Text);
             else
                if (!string.IsNullOrWhiteSpace(testFileDefaultFolder))
                 d.InitialDirectory = testFileDefaultFolder;

             d.Filter = "PDF|*.pdf|"
                + "All files (*.*)|*.*";
             if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                 return;
             testFile.Text = d.FileName;
         };
        }
        TemplateManager templateManager;

        enum Modes
        {
            NULL,
            SetAnchor,
            SetDocumentFirstPageRecognitionTextMark,
            SetField,
        }
        Modes mode
        {
            get
            {
                if (anchors.SelectedRows.Count > 0)
                    return Modes.SetAnchor;
                if (fields.SelectedRows.Count > 0)
                    return Modes.SetField;
                return Modes.NULL;
            }
        }
    }
}