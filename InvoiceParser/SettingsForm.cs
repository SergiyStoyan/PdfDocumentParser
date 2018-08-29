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

namespace Cliver.InvoiceParser
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

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;

            load_settings();
        }

        void load_settings()
        {
            IgnoreHidddenFiles.Checked = Settings.General.IgnoreHidddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;

            Synchronize.Checked = Settings.General.Synchronize;
            SynchronizedFolder.Text = Settings.General.SynchronizedFolder;
            SynchronizedFolder.Enabled = Synchronize.Checked;
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

                Settings.General.Synchronize = Synchronize.Checked;
                if (Synchronize.Checked && string.IsNullOrWhiteSpace(SynchronizedFolder.Text))
                    throw new Exception("Synchronized Folder is empty.");
                Settings.General.SynchronizedFolder = SynchronizedFolder.Text;

                Settings.General.Save();
                Settings.General.Reload();

                Close();
            }
            catch (Exception ex)
            {
                Settings.General.Reload();
                Message.Error2(ex);
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            Settings.General.Reset();
            PdfDocumentParser.Settings.Appearance.Reset();
            PdfDocumentParser.Settings.ImageProcessing.Reset();
            load_settings();
        }

        private void bSynchronizedFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.SelectedPath = string.IsNullOrWhiteSpace(SynchronizedFolder.Text) ? Settings.General.InputFolder : SynchronizedFolder.Text;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SynchronizedFolder.Text = d.SelectedPath;
        }

        private void Synchronize_CheckedChanged(object sender, EventArgs e)
        {
            SynchronizedFolder.Enabled = Synchronize.Checked;
        }

        //private void bResetTemplates_Click(object sender, EventArgs e)
        //{
        //    if (!Message.YesNo("The templates will be reset to the initial state. Proceed?"))
        //        return;
        //    Settings.Templates.Reset();
        //    Settings.Templates.Save();
        //    ProcessRoutines.Restart();
        //}
    }
}
