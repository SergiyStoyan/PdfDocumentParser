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
    public partial class BitmapPreparationForm : Form
    {
        public BitmapPreparationForm(TemplateForm templateForm)
        {
            InitializeComponent();

            FormClosing += delegate (object sender, FormClosingEventArgs e)
              {
                  e.Cancel = true;
                  this.Visible = false;
              };

            this.Icon = Win.AssemblyRoutines.GetAppIcon();

            //defaultBitmapPreprocessorClassDefinitions.Items = 

            this.templateForm = templateForm;

            Template template = null;
            Activated += delegate
            {
                template = templateForm.GetTemplateFromUI(false);
            };

            pageRotation.SelectedIndexChanged += delegate { changed = true; };
            autoDeskew.CheckedChanged += delegate { changed = true; };
            autoDeskewThreshold.ValueChanged += delegate { changed = true; };
            DetectingScaleAnchor.SelectedIndexChanged += delegate { changed = true; };
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
        }
        TemplateForm templateForm;
        bool changed = false;

        //public class DefaultBitmapPreprocessorClassDefinitionItem
        //{
        //    public string Value { set; get; }
        //    public string Value { set; get; }
        //}

        internal void SetUI(Template t, bool updateDependentOnly = false)
        {
            Text = "Bitmap preparation for '" + t.Name + "'";
            if (!updateDependentOnly)
            {
                pageRotation.SelectedIndex = (int)t.PageRotation;
                autoDeskew.Checked = t.AutoDeskew;
                autoDeskewThreshold.Value = t.AutoDeskewThreshold;
                bitmapPreprocessorClassDefinition.Text = t.BitmapPreprocessorClassDefinition;
                bitmapPreprocessorClassDefinition.Enabled = PreprocessBitmap.Checked = t.PreprocessBitmap;
                changed = false;
            }
            if (!changed
                && t.GetScaleDetectingAnchor() != (Template.Anchor.CvImage)DetectingScaleAnchor.SelectedItem
                )
                changed = true;
            DetectingScaleAnchor.Items.Clear();
            DetectingScaleAnchor.Items.Add("");
            DetectingScaleAnchor.Items.AddRange(t.Anchors.Where(a => a is Template.Anchor.CvImage).Select(a => (object)a.Id).ToArray());
            DetectingScaleAnchor.SelectedItem = t.GetScaleDetectingAnchor();
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
                return image.ToBitmap();
            }
        }
    }
}";

        internal void SetTemplate(Template t)
        {
            if (changed)
                validate();
            t.PageRotation = (Template.PageRotations)pageRotation.SelectedIndex;
            t.AutoDeskew = autoDeskew.Checked;
            t.AutoDeskewThreshold = (int)autoDeskewThreshold.Value;
            Template.Anchor.CvImage detectingScaleAnchor = DetectingScaleAnchor.SelectedItem as Template.Anchor.CvImage;
            t.ScaleDetectingAnchorId = detectingScaleAnchor == null ? -1 : detectingScaleAnchor.Id;
            t.BitmapPreprocessorClassDefinition = bitmapPreprocessorClassDefinition.Text;
            t.PreprocessBitmap = PreprocessBitmap.Checked;
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
                    templateForm.ReloadPageBitmaps();
                }
                return true;
            }
            catch (Exception ex)
            {
                Message.Error2(ex, this);
                return false;
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (!applyTemplate())
                return;
            Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bApply_Click(object sender, EventArgs e)
        {
            applyTemplate();
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
    }
}
