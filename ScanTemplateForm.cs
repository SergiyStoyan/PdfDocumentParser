//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Cliver.PdfDocumentParser
{
    public partial class ScanTemplateForm : Form
    {
        public ScanTemplateForm(TemplateForm templateForm)
        {
            InitializeComponent();

            FormClosing += delegate (object sender, FormClosingEventArgs e)
              {
                  e.Cancel = true;
                  this.Visible = false;
              };

            Icon = Win.AssemblyRoutines.GetAppIcon();

            this.templateForm = templateForm;

            Template template = null;
            Activated += delegate
            {
                template = templateForm.GetTemplateFromUI(false);
                SetUI(template, true);
            };

            PageRotation.SelectedIndexChanged += delegate { changed = true; };
            ScalingAnchor.SelectedIndexChanged += delegate { changed = true; };
            bitmapPreprocessorClassDefinition.TextChanged += delegate { changed = true; };
            PreprocessBitmap.CheckedChanged += delegate
            {
                bitmapPreprocessorClassDefinition.Enabled = PreprocessBitmap.Checked;
                if (bitmapPreprocessorClassDefinition.Enabled && string.IsNullOrWhiteSpace(bitmapPreprocessorClassDefinition.Text))
                {
                    string className = Regex.Replace(template.Name, @"[\W]", "_");
                    bitmapPreprocessorClassDefinition.Text = Regex.Replace(defaultBitmapPreprocessor, @"Default_BitmapPreprocessor", className + "_BitmapPreprocessor", RegexOptions.Singleline);
                }
            };
            Deskew.CheckedChanged += delegate
            {
                changed = true;
                DeskewMarginColor.Enabled = DeskewSingleBlock.Enabled = DeskewColumnOfBlocks.Enabled = DeskewStructuringElementX.Enabled = DeskewStructuringElementY.Enabled = Deskew.Checked;
                DeskewContourMaxCount.Enabled = DeskewAngleMaxDeviation.Enabled = DeskewBlockMaxHeight.Enabled = DeskewBlockMinGap.Enabled = DeskewColumnOfBlocks.Checked;
            };
            DeskewSingleBlock.CheckedChanged += delegate { changed = true; };
            DeskewColumnOfBlocks.CheckedChanged += delegate
            {
                changed = true;
                DeskewBlockMaxHeight.Enabled = DeskewBlockMinGap.Enabled = DeskewColumnOfBlocks.Checked;
            };
            DeskewBlockMaxHeight.ValueChanged += delegate { changed = true; };
            DeskewBlockMinGap.ValueChanged += delegate { changed = true; };
            DeskewContourMaxCount.ValueChanged += delegate { changed = true; };
            DeskewAngleMaxDeviation.ValueChanged += delegate { changed = true; };
            DeskewMarginColor.BackColorChanged += delegate { changed = true; };
        }
        TemplateForm templateForm;
        bool changed = false;

        //public class DefaultBitmapPreprocessorClassDefinitionItem
        //{
        //    public string Value { set; get; }
        //    public string Value { set; get; }
        //}

        internal void SetUI(Template t, bool updateSharedValuesOnly)
        {
            Text = "Scanned image preparation for '" + t.Name + "'";
            if (!updateSharedValuesOnly)
            {
                PageRotation.SelectedIndex = (int)t.PageRotation;
                bitmapPreprocessorClassDefinition.Text = t.BitmapPreprocessorClassDefinition;
                bitmapPreprocessorClassDefinition.Enabled = PreprocessBitmap.Checked = t.PreprocessBitmap;

                Deskewer.Config dc = t.Deskew != null ? t.Deskew : new Deskewer.Config();
                DeskewBlockMaxHeight.Value = dc.BlockMaxLength;
                DeskewBlockMinGap.Value = dc.BlockMinGap;
                DeskewStructuringElementX.Value = dc.StructuringElementSize.Width;
                DeskewStructuringElementY.Value = dc.StructuringElementSize.Height;
                DeskewContourMaxCount.Value = dc.ContourMaxCount;
                DeskewBlockMinGap.Value = dc.BlockMinGap;
                DeskewMarginColor.BackColor = dc.MarginColor;
                DeskewMarginColor.Enabled = DeskewContourMaxCount.Enabled = DeskewAngleMaxDeviation.Enabled = DeskewSingleBlock.Enabled = DeskewColumnOfBlocks.Enabled = DeskewStructuringElementX.Enabled = DeskewStructuringElementY.Enabled = Deskew.Checked = t.Deskew != null;
                if (t.Deskew != null)
                    DeskewSingleBlock.Checked = t.Deskew.Mode.HasFlag(Deskewer.Modes.SingleBlock) || !t.Deskew.Mode.HasFlag(Deskewer.Modes.ColumnOfBlocks);
                DeskewColumnOfBlocks.Checked = !DeskewSingleBlock.Checked;

                changed = false;
            }
            if (!changed && t.ScalingAnchorId != ScalingAnchor.SelectedItem as int?)
                changed = true;
            ScalingAnchor.Items.Clear();
            ScalingAnchor.Items.Add("");
            ScalingAnchor.Items.AddRange(t.Anchors.Where(a => a is Template.Anchor.CvImage).Select(a => (object)a.Id).ToArray());
            ScalingAnchor.SelectedItem = t.GetScalingAnchor()?.Id;
        }
        string defaultBitmapPreprocessor = @"using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Cliver.PdfDocumentParser
{
    public class Default_BitmapPreprocessor : BitmapPreprocessor
    {
        public override Bitmap GetProcessed(Bitmap bitmap)
        {
            return bitmap;
            using (bitmap)
            {
                Image<Gray, byte> image = bitmap.ToImage<Gray, byte>();
                //Emgu.CV.CvInvoke.Blur(image, image, new Size(3, 3), new Point(0, 0));
                //image = image.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 7, new Gray(0.03));
                //  Emgu.CV.CvInvoke.AdaptiveThreshold(image, image,, 125, 255, ThresholdType.Otsu | ThresholdType.Binary)new Gray(255), Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.ADAPTIVE_THRESH_GAUSSIAN_C, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY , windowSize, new Gray(0.03));
                //Emgu.CV.CvInvoke.Threshold(image, image, 125, 255, ThresholdType.Otsu | ThresholdType.Binary);
                //Emgu.CV.CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
                //CvInvoke.Dilate(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
                //CvInvoke.Erode(image, image, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
                return image.ToBitmap();
            }
        }
    }
}";

        internal void SetTemplate(Template t)
        {
            if (changed)
                validate();
            t.PageRotation = (Template.PageRotations)PageRotation.SelectedIndex;
            t.ScalingAnchorId = ScalingAnchor.SelectedItem is int ? (int)ScalingAnchor.SelectedItem : -1;
            t.BitmapPreprocessorClassDefinition = bitmapPreprocessorClassDefinition.Text;
            t.PreprocessBitmap = PreprocessBitmap.Checked;

            if (Deskew.Checked)
            {
                t.Deskew = new Deskewer.Config
                {
                    Mode = DeskewColumnOfBlocks.Checked ? Deskewer.Modes.ColumnOfBlocks : Deskewer.Modes.SingleBlock,
                    BlockMaxLength = (int)DeskewBlockMaxHeight.Value,
                    BlockMinGap = (int)DeskewBlockMinGap.Value,
                    StructuringElementSize = new System.Drawing.Size((int)DeskewStructuringElementX.Value, (int)DeskewStructuringElementY.Value),
                    ContourMaxCount = (int)DeskewContourMaxCount.Value,
                    AngleMaxDeviation = (float)DeskewAngleMaxDeviation.Value,
                    MarginColor = DeskewMarginColor.BackColor
                };
            }
            else
                t.Deskew = null;
        }

        void validate()
        {
            if (PreprocessBitmap.Checked)
            {
                bitmapPreprocessorClassDefinition.Document.MarkerStrategy.RemoveAll(marker => true);
                try
                {
                    BitmapPreprocessor.GetCompiledBitmapPreprocessorType(bitmapPreprocessorClassDefinition.Text);//checking
                }
                catch (BitmapPreprocessor.CompilationException ex)
                {
                    foreach (BitmapPreprocessor.CompilationError ce in ex.Data.Values)
                    {
                        ICSharpCode.TextEditor.Document.TextMarker tm = new ICSharpCode.TextEditor.Document.TextMarker(ce.P1, ce.P2 - ce.P1, ICSharpCode.TextEditor.Document.TextMarkerType.WaveLine, System.Drawing.Color.Red);
                        tm.ToolTip = ce.Message;
                        bitmapPreprocessorClassDefinition.Document.MarkerStrategy.AddMarker(tm);
                    }
                    throw ex;
                }
            }
        }

        private bool applyTemplate()
        {
            try
            {
                if (changed)
                {
                    validate();
                    changed = false;
                    templateForm.ReloadPageActiveTemplateBitmap();
                }
                return true;
            }
            catch (Exception ex)
            {
                Message.Error2(ex, this);
                return false;
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bApply_Click(object sender, EventArgs e)
        {
            if (applyTemplate())
                Close();
        }

        private void bSaveDafault_Click(object sender, EventArgs e)
        {

        }

        private void bRemove_Click(object sender, EventArgs e)
        {

        }

        private void defaults_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DeskewMarginColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = DeskewMarginColor.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
                DeskewMarginColor.BackColor = cd.Color;
        }
    }
}
