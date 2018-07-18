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
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            //Text = Application.ProductName;
            Text = AboutBox.AssemblyTitle;

            load_settings();
        }

        void load_settings()
        {
            TestPictureScale.Value = Settings.Appearance.TestPictureScale;

            PdfPageImageResolution.Value = Settings.ImageProcessing.PdfPageImageResolution;
            CoordinateDeviationMargin.Value = (decimal)Settings.ImageProcessing.CoordinateDeviationMargin;

            BrightnessTolerance.Value = (decimal)Settings.ImageProcessing.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)Settings.ImageProcessing.DifferentPixelNumberTolerance;
            FindBestImageMatch.Checked = Settings.ImageProcessing.FindBestImageMatch;

            AutoDeskewThreshold.Value = Settings.ImageProcessing.AutoDeskewThreshold;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Appearance.TestPictureScale = TestPictureScale.Value;    

                Settings.Appearance.Save();
                Settings.Appearance.Reload();   
                
                Settings.ImageProcessing.PdfPageImageResolution = (int)PdfPageImageResolution.Value;
                Settings.ImageProcessing.CoordinateDeviationMargin = (float)CoordinateDeviationMargin.Value;

                Settings.ImageProcessing.BrightnessTolerance = (float)BrightnessTolerance.Value;
                Settings.ImageProcessing.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                Settings.ImageProcessing.FindBestImageMatch = FindBestImageMatch.Checked;

                Settings.ImageProcessing.AutoDeskewThreshold = (int)AutoDeskewThreshold.Value;

                Settings.ImageProcessing.Save();
                Settings.ImageProcessing.Reload();

                Close();
            }
            catch(Exception ex)
            {
                Message.Error2(ex);
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            Settings.Appearance.Reset();
            Settings.ImageProcessing.Reset();
            load_settings();
        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
    }
}
