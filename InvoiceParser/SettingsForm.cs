using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cliver.PdfDocumentParser;

namespace Cliver.InvoiceParser
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
            PdfPageImageResolution.Value = PdfDocumentParser.Settings.General.PdfPageImageResolution;
            IgnoreHidddenFiles.Checked = Settings.General.IgnoreHidddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;
            TestPictureScale.Value = PdfDocumentParser.Settings.General.TestPictureScale;

            BrightnessTolerance.Value = (decimal)PdfDocumentParser.Settings.ImageProcessing.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)PdfDocumentParser.Settings.ImageProcessing.DifferentPixelNumberTolerance;
            FindBestImageMatch.Checked = PdfDocumentParser.Settings.ImageProcessing.FindBestImageMatch;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.General.IgnoreHidddenFiles = IgnoreHidddenFiles.Checked;
                Settings.General.ReadInputFolderRecursively = ReadInputFolderRecursively.Checked;

                Settings.General.Save();
                Settings.General.Reload();

                PdfDocumentParser.Settings.General.PdfPageImageResolution = (int)PdfPageImageResolution.Value;
                PdfDocumentParser.Settings.General.TestPictureScale = TestPictureScale.Value;

                PdfDocumentParser.Settings.General.Save();
                PdfDocumentParser.Settings.General.Reload();                

                PdfDocumentParser.Settings.ImageProcessing.BrightnessTolerance = (float)BrightnessTolerance.Value;
                PdfDocumentParser.Settings.ImageProcessing.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                PdfDocumentParser.Settings.ImageProcessing.FindBestImageMatch = FindBestImageMatch.Checked;

                PdfDocumentParser.Settings.ImageProcessing.Save();
                PdfDocumentParser.Settings.ImageProcessing.Reload();

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
            PdfDocumentParser.Settings.Templates.Reset();
            ProcessRoutines.Restart();
        }
    }
}
