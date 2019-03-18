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
using Cliver.PdfDocumentParser;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cliver.SampleParser
{
    public partial class SettingsForm : Form
    {
        //static public void OpenDialog()
        //{
        //    if (SettingsForm.This.Visible)
        //        SettingsForm.This.Activate();
        //    else
        //        SettingsForm.This.ShowDialog();
        //}
        //public static SettingsForm This
        //{
        //    get
        //    {
        //        if (_This == null || _This.IsDisposed)
        //            _This = new SettingsForm();
        //        return _This;
        //    }
        //}
        //static SettingsForm _This = null;

        public SettingsForm()
        {
            InitializeComponent();

            this.Icon = Win.AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;

            //important.Text = "Important! Letting folder '" + Config.StorageDir + "' be synchronized by a remote drive application may bring to malfunction.";
            load_settings();
        }

        void load_settings()
        {
            IgnoreHiddenFiles.Checked = Settings.General.IgnoreHiddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.General.IgnoreHiddenFiles = IgnoreHiddenFiles.Checked;
                Settings.General.ReadInputFolderRecursively = ReadInputFolderRecursively.Checked;
                
                Settings.General.Save();
                Settings.General.Reload();

                Close();
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            Settings.General.Reset();
            PdfDocumentParser.Settings.Appearance.Reset();
            PdfDocumentParser.Settings.Constants.Reset();
            load_settings();
        }
    }
}
