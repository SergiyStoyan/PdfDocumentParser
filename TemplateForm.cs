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
        public TemplateForm(TemplateManager templateManager)
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

                Point p = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));

                ResizebleBox rb = findResizebleBox(p, out ResizebleBoxSides resizebleBoxSide);
                if (rb != null)
                {
                    drawingMode = resizebleBoxSide == ResizebleBoxSides.Left || resizebleBoxSide == ResizebleBoxSides.Right ? DrawingModes.resizingSelectionBoxV : DrawingModes.resizingSelectionBoxH;
                    Cursor.Current = drawingMode == DrawingModes.resizingSelectionBoxV ? Cursors.VSplit : Cursors.HSplit;
                    selectionBoxPoint0 = rb.R.Location;
                    selectionBoxPoint1 = rb.R.Location;
                    selectionBoxPoint2 = new Point(rb.R.Right, rb.R.Bottom);
                }
                else
                {
                    drawingMode = DrawingModes.drawingSelectionBox;
                    selectionBoxPoint0 = p;
                    selectionBoxPoint1 = p;
                    selectionBoxPoint2 = p;
                }
                selectionCoordinates.Text = selectionBoxPoint1.ToString();
            };

            picture.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (pages == null)
                    return;

                Point p = new Point((int)(e.X / (float)pictureScale.Value), (int)(e.Y / (float)pictureScale.Value));

                switch (drawingMode)
                {
                    case DrawingModes.NULL:
                        selectionCoordinates.Text = p.ToString();

                        if (findResizebleBox(p, out ResizebleBoxSides resizebleBoxSide) != null)
                            Cursor.Current = resizebleBoxSide == ResizebleBoxSides.Left || resizebleBoxSide == ResizebleBoxSides.Right ? Cursors.VSplit : Cursors.HSplit;
                        else
                            Cursor.Current = Cursors.Default;
                        return;
                    case DrawingModes.drawingSelectionBox:
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
                        break;
                    case DrawingModes.resizingSelectionBoxV:
                        if (Math.Abs(selectionBoxPoint2.X - p.X) < Math.Abs(p.X - selectionBoxPoint1.X))
                            selectionBoxPoint2.X = p.X;
                        else
                            selectionBoxPoint1.X = p.X;
                        break;
                    case DrawingModes.resizingSelectionBoxH:
                        if (Math.Abs(selectionBoxPoint2.Y - p.Y) < Math.Abs(p.Y - selectionBoxPoint1.Y))
                            selectionBoxPoint2.Y = p.Y;
                        else
                            selectionBoxPoint1.Y = p.Y;
                        break;
                        //case DrawingModes.resizingSelectionBoxR:
                        //    if (selectionBoxPoint1.X < p.X)
                        //        selectionBoxPoint2.X = p.X;
                        //    else
                        //    {
                        //        selectionBoxPoint2.X = selectionBoxPoint1.X;
                        //        selectionBoxPoint1.X = p.X;
                        //    }
                        //    break;
                        //case DrawingModes.resizingSelectionBoxB:
                        //    if (selectionBoxPoint1.Y < p.Y)
                        //        selectionBoxPoint2.Y = p.Y;
                        //    else
                        //    {
                        //        selectionBoxPoint2.Y = selectionBoxPoint1.Y;
                        //        selectionBoxPoint1.Y = p.Y;
                        //    }
                        //    break;
                }
                selectionCoordinates.Text = selectionBoxPoint1.ToString() + ":" + selectionBoxPoint2.ToString();
                RectangleF r = new RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);
                clearImageFromBoxes();
                drawBoxes(Settings.Appearance.SelectionBoxColor, Settings.Appearance.SelectionBoxBorderWidth, new List<System.Drawing.RectangleF> { r });
            };

            picture.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                try
                {
                    if (pages == null)
                        return;

                    if (drawingMode == DrawingModes.NULL)
                        return;
                    drawingMode = DrawingModes.NULL;

                    Template.RectangleF r = new Template.RectangleF(selectionBoxPoint1.X, selectionBoxPoint1.Y, selectionBoxPoint2.X - selectionBoxPoint1.X, selectionBoxPoint2.Y - selectionBoxPoint1.Y);

                    switch (settingMode)
                    {
                        case SettingModes.SetAnchor:
                            {
                                if (currentAnchorControl == null)
                                    break;

                                currentAnchorControl.SetTagFromControl();
                                Template.Anchor a = (Template.Anchor)currentAnchorControl.Row.Tag;
                                switch (a.Type)
                                {
                                    case Template.Anchor.Types.PdfText:
                                        if (selectedPdfCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedPdfCharBoxs = new List<Pdf.CharBox>();
                                        selectedPdfCharBoxs.AddRange(Pdf.GetCharBoxsSurroundedByRectangle(pages[currentPageI].PdfCharBoxs, r.GetSystemRectangleF(), true));
                                        break;
                                    case Template.Anchor.Types.OcrText:
                                        if (selectedOcrCharBoxs == null/* || (ModifierKeys & Keys.Control) != Keys.Control*/)
                                            selectedOcrCharBoxs = new List<Ocr.CharBox>();
                                        Template.Anchor.OcrText ot = a as Template.Anchor.OcrText;
                                        if (ot.OcrEntirePage)
                                            selectedOcrCharBoxs.AddRange(PdfDocumentParser.Ocr.GetCharBoxsSurroundedByRectangle(pages[currentPageI].ActiveTemplateOcrCharBoxs, r.GetSystemRectangleF()));
                                        else
                                        {
                                            foreach (Ocr.CharBox cb in PdfDocumentParser.Ocr.This.GetCharBoxs(pages[currentPageI].GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio)))
                                            {
                                                cb.R.X += r.X;
                                                cb.R.Y += r.Y;
                                                selectedOcrCharBoxs.Add(cb);
                                            }
                                        }
                                        break;
                                    case Template.Anchor.Types.ImageData:
                                        {
                                            if (selectedImageBoxs == null)
                                                selectedImageBoxs = new List<Template.Anchor.ImageData.ImageBox>();
                                            ImageData id;
                                            using (Bitmap rb = pages[currentPageI].GetRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                                            {
                                                using (Bitmap b = ImageData.GetScaled(rb, Settings.Constants.Image2PdfResolutionRatio))
                                                    id = new ImageData(b, false);
                                            }
                                            selectedImageBoxs.Add(new Template.Anchor.ImageData.ImageBox
                                            {
                                                Rectangle = r,
                                                ImageData = id
                                            });
                                        }
                                        break;
                                    //case Template.Anchor.Types.Script://not set by this way
                                    //    break;
                                    default:
                                        throw new Exception("Unknown option: " + a.Type);
                                }

                                if ((ModifierKeys & Keys.Control) != Keys.Control)
                                    setAnchorFromSelectedElements();
                            }
                            break;
                        case SettingModes.SetField:
                            {
                                if (fields.SelectedRows.Count < 1)
                                    break;
                                var row = fields.SelectedRows[0];
                                Template.Field f = (Template.Field)row.Tag;
                                f.Rectangle = r;

                                if (f.LeftAnchor != null)
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(f.LeftAnchor.Id);
                                    f.LeftAnchor.Shift = aai.Shift.Width;
                                }
                                if (f.TopAnchor != null)
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(f.TopAnchor.Id);
                                    f.TopAnchor.Shift = aai.Shift.Height;
                                }
                                if (f.RightAnchor != null)
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(f.RightAnchor.Id);
                                    f.RightAnchor.Shift = aai.Shift.Width;
                                }
                                if (f.BottomAnchor != null)
                                {
                                    Page.AnchorActualInfo aai = pages[currentPageI].GetAnchorActualInfo(f.BottomAnchor.Id);
                                    f.BottomAnchor.Shift = aai.Shift.Height;
                                }

                                setFieldRow(row, f);
                                owners2resizebleBox[f] = new ResizebleBox(f, f.Rectangle.GetSystemRectangleF(), Settings.Appearance.SelectionBoxBorderWidth);
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
                if (c is SplitContainer s)
                {
                    s.BackColor = Color.FromArgb(80, 70, 0);
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
                //showPage(currentPageI);
            };

            autoDeskew.CheckedChanged += delegate
            {
                reloadPageBitmaps();
                //showPage(currentPageI);
            };

            Load += delegate
            {
            };

            save.Click += Save_Click;
            cancel.Click += delegate { Close(); };
            Help.LinkClicked += Help_LinkClicked;
            Configure.LinkClicked += Configure_LinkClicked;
            About.LinkClicked += About_LinkClicked;

            bTestFile.Click += delegate (object sender, EventArgs e)
         {
             OpenFileDialog d = new OpenFileDialog();
             if (!string.IsNullOrWhiteSpace(testFile.Text))
                 d.InitialDirectory = PathRoutines.GetDirFromPath(testFile.Text);
             else
                if (!string.IsNullOrWhiteSpace(templateManager.TestFileDefaultFolder))
                 d.InitialDirectory = templateManager.TestFileDefaultFolder;

             d.Filter = "PDF|*.pdf|"
                + "All files (*.*)|*.*";
             if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                 return;
             testFile.Text = d.FileName;
         };

            ShowPdfText.LinkClicked += ShowPdfText_LinkClicked;
            ShowOcrText.LinkClicked += ShowOcrText_LinkClicked;
            ShowAsJson.LinkClicked += showAsJson_LinkClicked;

            tCurrentPage.Leave += delegate
             {
                 changeCurrentPage();
             };
            tCurrentPage.KeyDown += delegate (object sender, KeyEventArgs e)
              {
                  if (e.KeyCode == Keys.Enter)
                      changeCurrentPage();
              };
        }
        TemplateManager templateManager;
        Point selectionBoxPoint0, selectionBoxPoint1, selectionBoxPoint2;

        enum DrawingModes
        {
            NULL,
            drawingSelectionBox,
            resizingSelectionBoxV,
            resizingSelectionBoxH,
        }
        DrawingModes drawingMode = DrawingModes.NULL;

        enum SettingModes
        {
            NULL,
            SetAnchor,
            SetField,
        }
        SettingModes settingMode
        {
            get
            {
                if (anchors.SelectedRows.Count > 0)
                    return SettingModes.SetAnchor;
                if (fields.SelectedRows.Count > 0)
                    return SettingModes.SetField;
                return SettingModes.NULL;
            }
        }

        ResizebleBox findResizebleBox(Point p, out ResizebleBoxSides resizebleBoxSides)
        {
            foreach (ResizebleBox rb in owners2resizebleBox.Values)
            {
                if (!rb.outerR.Contains(p))
                    continue;
                if (rb.R.Left >= p.X)
                {
                    resizebleBoxSides = ResizebleBoxSides.Left;
                    return rb;
                }
                if (rb.R.Right <= p.X)
                {
                    resizebleBoxSides = ResizebleBoxSides.Right;
                    return rb;
                }
                if (rb.R.Top >= p.Y)
                {
                    resizebleBoxSides = ResizebleBoxSides.Top;
                    return rb;
                }
                if (rb.R.Bottom <= p.Y)
                {
                    resizebleBoxSides = ResizebleBoxSides.Bottom;
                    return rb;
                }
            }
            resizebleBoxSides = ResizebleBoxSides.Left;
            return null;
        }
        enum ResizebleBoxSides
        {
            Left,
            Top,
            Right,
            Bottom
        }
        readonly Dictionary<object, ResizebleBox> owners2resizebleBox = new Dictionary<object, ResizebleBox>();
        internal class ResizebleBox
        {
            readonly public Rectangle outerR;
            readonly public Rectangle R;
            readonly public object owner;

            public ResizebleBox(object owner, RectangleF rectangle, float borderWidth)
            {
                R = System.Drawing.Rectangle.Round(rectangle);
                int borderW = (int)borderWidth;
                outerR = System.Drawing.Rectangle.Round(rectangle);
                outerR.X -= borderW;
                outerR.Y -= borderW;
                outerR.Width += 2 * borderW;
                outerR.Height += 2 * borderW;
                this.owner = owner;
            }
        }
    }
}