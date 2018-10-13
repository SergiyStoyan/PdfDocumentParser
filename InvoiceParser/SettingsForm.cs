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
            IgnoreHiddenFiles.Checked = Settings.General.IgnoreHiddenFiles;
            ReadInputFolderRecursively.Checked = Settings.General.ReadInputFolderRecursively;

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
                if (Synchronize.Checked && string.IsNullOrWhiteSpace(SynchronizationFolder.Text))
                    throw new Exception("Synchronization Folder is empty.");
                //if (PathRoutines.ArePathsEqual(SynchronizationFolder.Text, Config.StorageDir))
                //    throw new Exception("Synchronization Folder cannot be the application's config folder itself.");

                Settings.General.IgnoreHiddenFiles = IgnoreHiddenFiles.Checked;
                Settings.General.ReadInputFolderRecursively = ReadInputFolderRecursively.Checked;

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
                Message.Error2(ex);
            }
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            Settings.General.Reset();
            Settings.Synchronization.Reset();
            PdfDocumentParser.Settings.Appearance.Reset();
            PdfDocumentParser.Settings.Constants.Reset();
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
                string path = GetDropboxDirectory();
                if (path != null)
                    SynchronizationFolder.Text = path + "\\" + ProgramRoutines.GetAppName();
                else
                    SynchronizationFolder.Text = Config.StorageDir + "\\" + ProgramRoutines.GetAppName();
            }
        }

        public static string GetDropboxDirectory()
        {
            try
            {
                string cf = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dropbox\info.json";
                if (!File.Exists(cf))
                    cf = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Dropbox\info.json";
                if (!File.Exists(cf))
                    return null;
                JObject o = JObject.Parse(File.ReadAllText(cf, Encoding.UTF8));
                string path = (string)o.SelectToken("personal.path");
                if (path == null)
                    path = (string)o.SelectToken("business.path");
                return path;
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
                return null;
            }
        }

        //public static string GetGoogleDriveDirectory()
        //{
        //    try
        //    {
        //        // Google Drive's sync database can be in a couple different locations. Go find it. 
        //        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //        string dbName = "sync_config.db";
        //        var pathsToTry = new[] { @"Google\Drive\" + dbName, @"Google\Drive\user_default\" + dbName };

        //        string syncDbPath = (from p in pathsToTry
        //                             where File.Exists(Path.Combine(appDataPath, p))
        //                             select Path.Combine(appDataPath, p))
        //                            .FirstOrDefault();
        //        if (syncDbPath == null)
        //            throw new FileNotFoundException("Cannot find Google Drive sync database", dbName);

        //        // Build the connection and sql command
        //        string conString = string.Format(@"Data Source='{0}';Version=3;New=False;Compress=True;", syncDbPath);
        //        using (var con = new SQLiteConnection(conString))
        //        using (var cmd = new SQLiteCommand("select * from data where entry_key='local_sync_root_path'", con))
        //        {
        //            // Open the connection and execute the command
        //            con.Open();
        //            var reader = cmd.ExecuteReader();
        //            reader.Read();

        //            // Extract the data from the reader
        //            string path = reader["data_value"]?.ToString();
        //            if (string.IsNullOrWhiteSpace(path))
        //                throw new InvalidDataException("Cannot read 'local_sync_root_path' from Google Drive configuration db");

        //            // By default, the path will be prefixed with "\\?\" (unless another app has explicitly changed it).
        //            // \\?\ indicates to Win32 that the filename may be longer than MAX_PATH (see MSDN). 
        //            // Parts of .NET (e.g. the File class) don't handle this very well, so remove this prefix.
        //            if (path.StartsWith(@"\\?\"))
        //                path = path.Substring(@"\\?\".Length);

        //            return path;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage.Error(ex);
        //        return null;
        //    }
        //}
    }
}
