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
            PdfPageImageResolution.Value = Settings.General.PdfPageImageResolution;
            IgnoreHidddenFiles.Checked = Settings.General.IgnoreHidddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;
            TestPictureScale.Value = Settings.General.TestPictureScale;

            BrightnessTolerance.Value = (decimal)Settings.ImageProcessing.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)Settings.ImageProcessing.DifferentPixelNumberTolerance;
            FindBestImageMatch.Checked = Settings.ImageProcessing.FindBestImageMatch;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.General.PdfPageImageResolution = (int)PdfPageImageResolution.Value;
                Settings.General.IgnoreHidddenFiles = IgnoreHidddenFiles.Checked;
                Settings.General.ReadInputFolderRecursively = ReadInputFolderRecursively.Checked;
                Settings.General.TestPictureScale = TestPictureScale.Value;    

                Settings.General.Save();
                Settings.General.Reload();            

                Settings.ImageProcessing.BrightnessTolerance = (float)BrightnessTolerance.Value;
                Settings.ImageProcessing.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                Settings.ImageProcessing.FindBestImageMatch = FindBestImageMatch.Checked;

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
