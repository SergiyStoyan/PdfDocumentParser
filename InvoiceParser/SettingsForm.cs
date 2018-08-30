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

            //important.Text = "Important! Letting folder '" + Config.StorageDir + "' be synchronized by a remote drive application may bring to malfunction.";
            load_settings();
        }

        void load_settings()
        {
            IgnoreHidddenFiles.Checked = Settings.General.IgnoreHidddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;
            MaxPageNumberToDetectTemplate.Text = Settings.General.MaxPageNumberToDetectTemplate.ToString();

            Synchronize.Checked = Settings.Synchronization.Synchronize;
            SynchronizationFolder.Text = Settings.Synchronization.SynchronizationFolder;

            SynchronizationFolder.Enabled = Synchronize.Checked;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                int maxPageNumberToDetectTemplate;
                if (!int.TryParse(MaxPageNumberToDetectTemplate.Text, out maxPageNumberToDetectTemplate) || maxPageNumberToDetectTemplate < 1)
                    throw new Exception("MaxPageNumberToDetectTemplate must be a positive number.");
                if (Synchronize.Checked && string.IsNullOrWhiteSpace(SynchronizationFolder.Text))
                    throw new Exception("Synchronization Folder is empty.");
                if (PathRoutines.ArePathsEqual(SynchronizationFolder.Text, Config.StorageDir))
                    throw new Exception("Synchronization Folder cannot be the application's config folder itself.");

                Settings.General.IgnoreHidddenFiles = IgnoreHidddenFiles.Checked;
                Settings.General.ReadInputFolderRecursively = ReadInputFolderRecursively.Checked;
                Settings.General.MaxPageNumberToDetectTemplate = maxPageNumberToDetectTemplate;

                Settings.Synchronization.Synchronize = Synchronize.Checked;
                Settings.Synchronization.SynchronizationFolder = SynchronizationFolder.Text;

                Settings.General.Save();
                Settings.General.Reload();
                Settings.Synchronization.Save();
                Settings.Synchronization.Reload();

                Close();
            }
            catch (Exception ex)
            {
                //Settings.General.Reload();
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
            d.SelectedPath = string.IsNullOrWhiteSpace(SynchronizationFolder.Text) ? Settings.General.InputFolder : SynchronizationFolder.Text;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SynchronizationFolder.Text = d.SelectedPath;
        }

        private void Synchronize_CheckedChanged(object sender, EventArgs e)
        {
            SynchronizationFolder.Enabled = Synchronize.Checked;

            if (string.IsNullOrWhiteSpace(SynchronizationFolder.Text))
            {
                string cf = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dropbox\info.json";
                if (!File.Exists(cf))
                    cf = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Dropbox\info.json";
                if (File.Exists(cf))
                {
                    JObject o = JObject.Parse(File.ReadAllText(cf, Encoding.UTF8));
                    string path = (string)o.SelectToken("personal.path");
                    if (path == null)
                        path = (string)o.SelectToken("business.path");
                    if (path != null)
                        SynchronizationFolder.Text = path + "\\" + ProgramRoutines.GetAppName();
                }
                if (string.IsNullOrWhiteSpace(SynchronizationFolder.Text))
                    SynchronizationFolder.Text = Config.StorageDir + "\\" + ProgramRoutines.GetAppName();
            }
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
