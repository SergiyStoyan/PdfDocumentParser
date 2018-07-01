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
            Text = Application.ProductName;

            load_settings();
        }

        void load_settings()
        {
            imageResolution.Value = Settings.General.PdfPageImageResolution;
            ignoreHidddenFiles.Checked = Settings.General.IgnoreHidddenFiles;
            testPictureScale.Value = Settings.General.TestPictureScale;

            brightnessTolerance.Value = (decimal)Settings.ImageProcessing.BrightnessTolerance;
            differentPixelNumberTolerance.Value = (decimal)Settings.ImageProcessing.DifferentPixelNumberTolerance;
            findBestImageMatch.Checked = Settings.ImageProcessing.FindBestImageMatch;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.General.PdfPageImageResolution = (int)imageResolution.Value;
                Settings.General.IgnoreHidddenFiles = ignoreHidddenFiles.Checked;
                Settings.General.TestPictureScale = testPictureScale.Value;    

                Settings.General.Save();
                Settings.General.Reload();            

                Settings.ImageProcessing.BrightnessTolerance = (float)brightnessTolerance.Value;
                Settings.ImageProcessing.DifferentPixelNumberTolerance = (float)differentPixelNumberTolerance.Value;
                Settings.ImageProcessing.FindBestImageMatch = findBestImageMatch.Checked;

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
            Settings.General.Reset();
            load_settings();
        }

        private void bResetTemplates_Click(object sender, EventArgs e)
        {
            if (!Message.YesNo("The templates will be reset to the initial state. Proceed?"))
                return;
            Settings.Templates.Reset();
            ProcessRoutines.Restart();
        }
    }
}
