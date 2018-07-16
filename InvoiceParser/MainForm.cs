using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cliver;
using System.IO;
using System.Threading;
using Cliver.PdfDocumentParser;

namespace Cliver.InvoiceParser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;

            Message.Owner = this;

            InputFolder.Text = Settings.General.InputFolder;

            OutputFolder.Text = Settings.General.OutputFolder;

            loadTemplates();

            templates.CellValidating += delegate (object sender, DataGridViewCellValidatingEventArgs e)
            {
                if (e.ColumnIndex == templates.Columns["Name_"].Index)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                            throw new Exception("Name cannot be empty!");
                        foreach (DataGridViewRow r in templates.Rows)
                        {
                            if (r.Index != e.RowIndex && (string)e.FormattedValue == (string)r.Cells["Name_"].Value)
                                throw new Exception("Name '" + e.FormattedValue + "' is duplicated!");
                        }
                        if ((string)templates.Rows[e.RowIndex].Cells["Name_"].Value != ((string)e.FormattedValue).Trim())
                            templates.Rows[e.RowIndex].Cells["Name_"].Value = ((string)e.FormattedValue).Trim();
                    }
                    catch (Exception ex)
                    {
                        e.Cancel = true;
                        Message.Error2("Name", ex);
                        return;
                    }
                }
                else if (e.ColumnIndex == templates.Columns["Active"].Index)
                {
                }
            };

            templates.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                try
                {
                    if (e.Row == null)
                        return;
                    if (!Message.YesNo("Template '" + e.Row.Cells["Name_"].Value + "' will be deleted forever! Are you sure to proceed?"))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            templates.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                    if (e.Row == null)
                        return;

                    Template t = (Template)e.Row.Tag;
                    PdfDocumentParser.Settings.Templates.Templates.RemoveAll(x => x == t);
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            templates.RowValidated += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;

                var r = templates.Rows[e.RowIndex];
                Template t = (Template)r.Tag;
                templates.EndEdit();//needed to set checkbox values

                t.Name = (string)r.Cells["Name_"].Value;
                t.Active = Convert.ToBoolean(r.Cells["Active"].Value);
            };

            templates.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
                //DataGridViewRow r = templates.Rows[e.RowIndex];
                //if (r.IsNewRow && string.IsNullOrWhiteSpace((string)r.Cells["Name_"].Value))
                //    templates.CancelEdit();//.Rows.Remove(r);

                //try
                //{
                //    string n = (string)r.Cells["Name_"].Value;
                //    if (string.IsNullOrWhiteSpace(n))
                //        throw new Exception("Name cannot be empty!");

                //    //templates.NotifyCurrentCellDirty(true);
                //    //templates.EndEdit();
                //}
                //catch (Exception ex)
                //{
                //    Message.Error2("Name", ex);
                //    e.Cancel = true;
                //}
            };

            templates.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                    if (e.Row.Cells["Active"].Value == null)
                        e.Row.Cells["Active"].Value = true;

                    Template t = (Template)e.Row.Tag;
                    if (t == null)
                    {
                        t = PdfDocumentParser.Settings.Templates.CreateInitialTemplate();
                        PdfDocumentParser.Settings.Templates.Templates.Add(t);
                        e.Row.Tag = t;
                        e.Row.Cells["Name_"].Value = t.Name;
                        e.Row.Cells["Active"].Value = t.Active;
                        //templates.NotifyCurrentCellDirty(true);
                        //templates.EndEdit();
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            templates.UserAddedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            templates.CellContentClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (e.RowIndex < 0 || templates.Rows[e.RowIndex].IsNewRow)
                        return;

                    if (templates.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                    {
                        if (templates.Columns[e.ColumnIndex].Name == "Edit")
                        {
                            Template t = (Template)templates.Rows[e.RowIndex].Tag;
                            t.Name = (string)templates.Rows[e.RowIndex].Cells["Name_"].Value;
                            TemplateForm tf = new TemplateForm(t, Settings.General.InputFolder, (Template nt) =>
                            {
                                t = PdfDocumentParser.Settings.Templates.Templates.Where(a => a.Name == nt.Name).FirstOrDefault();
                                if (t == null)
                                    PdfDocumentParser.Settings.Templates.Templates.Add(nt);
                                else
                                    PdfDocumentParser.Settings.Templates.Templates[PdfDocumentParser.Settings.Templates.Templates.IndexOf(t)] = nt;
                                PdfDocumentParser.Settings.Templates.Save();

                                foreach (DataGridViewRow r in templates.Rows)
                                {
                                    if ((string)r.Cells["Name_"].Value == nt.Name)
                                    {
                                        r.Tag = nt;
                                        r.Cells["Name_"].Value = nt.Name;
                                        break;
                                    }
                                }
                            });
                            tf.Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            templates.Validating += delegate (object sender, CancelEventArgs e)
            {
                PdfDocumentParser.Settings.Templates.Templates.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name));
                PdfDocumentParser.Settings.Templates.Save();
            };

            progress.Maximum = 10000;

            if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
                OutputFolder.Text = Log.AppDir;
        }

        void loadTemplates()
        {
            try
            {
                foreach (Template t in PdfDocumentParser.Settings.Templates.Templates)
                {
                    if (string.IsNullOrWhiteSpace(t.Name))
                        continue;
                    int i = templates.Rows.Add(new DataGridViewRow());
                    DataGridViewRow r = templates.Rows[i];
                    r.Cells["Name_"].Value = t.Name.Trim();
                    r.Cells["Active"].Value = t.Active;
                    r.Tag = t;
                }
                //templates.Columns["Name_"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //templates.Columns["Name_"].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        //void saveTemplates()
        //{
        //    try
        //    {
        //        templates.EndEdit();//needed to set checkbox values

        //        foreach (DataGridViewRow r in templates.Rows)
        //        {
        //            string n = (string)r.Cells["Name_"].Value;
        //            if (string.IsNullOrWhiteSpace(n))
        //                continue;
        //            Settings.Template t = (Settings.Template)r.Tag;
        //            t.Name = n;
        //            t.Active = Convert.ToBoolean(r.Cells["Active"].Value);
        //        }
        //        PdfDocumentParser.Settings.Templates.Save();
        //    }
        //    catch (Exception e)
        //    {
        //        LogMessage.Error(e);
        //    }
        //}

        private void bInputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (string.IsNullOrWhiteSpace(d.SelectedPath))
                if (string.IsNullOrWhiteSpace(Settings.General.InputFolder))
                    d.SelectedPath = Log.AppDir;
                else
                    d.SelectedPath = Settings.General.InputFolder;
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            InputFolder.Text = d.SelectedPath;
        }

        private void bOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (string.IsNullOrWhiteSpace(d.SelectedPath))
                if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
                    d.SelectedPath = Log.AppDir;
                else
                    d.SelectedPath = Settings.General.OutputFolder;
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            OutputFolder.Text = d.SelectedPath;
        }

        private void bRun_Click(object sender, EventArgs e)
        {
            if (t != null && t.IsAlive)
            {
                if (!LogMessage.AskYesNo("Processing is running. Would you like to abort it and restart?", true))
                    return;
                while (t.IsAlive)
                {
                    t.Abort();
                    Thread.Sleep(100);
                }
            }

            Settings.General.InputFolder = InputFolder.Text;
            Settings.General.Save();
            Settings.General.OutputFolder = OutputFolder.Text;
            Settings.General.Save();

            bRun.Enabled = false;
            t = Cliver.ThreadRoutines.Start(
                () =>
                {
                    try
                    {
                        Processor.Run((processedN, totalN) =>
                        {
                            if (totalN > 0)
                            {
                                if (processedN == totalN)
                                    progress.Invoke(() =>
                                    {
                                        progress.Value = (int)(progress.Maximum * ((float)processedN / totalN));
                                        lProgress.Text = "Completed";
                                    });
                                else
                                    progress.Invoke(() =>
                                    {
                                        progress.Value = (int)(progress.Maximum * ((float)processedN / totalN));
                                        lProgress.Text = "Processed " + processedN + " files of " + totalN;
                                    });
                            }
                            else
                                progress.Invoke(() =>
                                {
                                    progress.Value = 0;
                                    lProgress.Text = "";
                                });
                        });
                    }
                    catch (Exception ex)
                    {
                        LogMessage.Error(ex);
                    }
                    finally
                    {
                        bRun.BeginInvoke2(() => { bRun.Enabled = true; });
                    }
                }
                );
        }

        Thread t = null;

        private void bAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            if (t != null && t.IsAlive)
            {
                if (!LogMessage.AskYesNo("Processing is running. Would you like to abort it?", true))
                    return;
            }
            Close();
            Environment.Exit(0);
        }

        private void bLog_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Log.WorkDir);
        }

        private void bOutput_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(OutputFolder.Text))
                System.Diagnostics.Process.Start(OutputFolder.Text);
            else
                Message.Error("No such folder: '" + OutputFolder.Text + "'");
        }

        private void bHeaders_Click(object sender, EventArgs e)
        {
            OutputConfigForm ocf = new OutputConfigForm();
            ocf.ShowDialog();
        }

        private void bSettings_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
        }

        private void help_Click(object sender, EventArgs e)
        {
            string helpFile = "help.html";
            //try
            //{
            //    System.Net.WebClient wc = new System.Net.WebClient();
            //    wc.DownloadFile("http.cliversoft.com/InvoiceParser/version.1/help.html", helpFile);
            //}
            //catch { }
            System.Diagnostics.Process.Start(helpFile);
        }

        private void Engine_Click(object sender, EventArgs e)
        {
            PdfDocumentParser.SettingsForm sf = new PdfDocumentParser.SettingsForm();
            sf.ShowDialog();
        }
    }
}