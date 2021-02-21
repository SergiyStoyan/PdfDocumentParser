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
            InputFolder.Text = Settings.General.InputFolder;
            OutputFolder.Text = Settings.General.OutputFolder;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.GeneralSettings g2 = Settings.General.CreateClone();

                g2.InputFolder = InputFolder.Text;
                if (string.IsNullOrWhiteSpace(g2.InputFolder))
                    throw new Exception("Input Folder is not set.");

                if (string.IsNullOrWhiteSpace(g2.OutputFolder))
                    throw new Exception("Output Folder is not set.");
                g2.OutputFolder = OutputFolder.Text;

                Settings.General = g2;
                Settings.General.Save();

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
            load_settings();
        }

        private void bInputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.RootFolder = Environment.SpecialFolder.Desktop;
            if (string.IsNullOrWhiteSpace(d.SelectedPath))
                if (string.IsNullOrWhiteSpace(Settings.General.InputFolder))
                    d.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                else
                    d.SelectedPath = Settings.General.InputFolder;
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            InputFolder.Text = d.SelectedPath;
        }

        private void bOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.RootFolder = Environment.SpecialFolder.Desktop;
            if (string.IsNullOrWhiteSpace(d.SelectedPath))
                if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
                    d.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                else
                    d.SelectedPath = Settings.General.OutputFolder;
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            OutputFolder.Text = d.SelectedPath;
        }

        private void lShowInput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Cliver.FileSystemRoutines.CreateDirectory(InputFolder.Text));
        }

        private void lShowOutput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Cliver.FileSystemRoutines.CreateDirectory(OutputFolder.Text));
        }
    }
}
