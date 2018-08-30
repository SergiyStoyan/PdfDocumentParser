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
        public static MainForm This
        {
            get
            {
                if (_This == null)
                    _This = new MainForm();
                return _This;
            }
        }
        static MainForm _This = null;

        public MainForm()
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;

            Message.Owner = this;

            InputFolder.Text = Settings.General.InputFolder;

            OutputFolder.Text = Settings.General.OutputFolder;

            LoadTemplates();

            initiateSelectionEngine();

            Active.ValueType = typeof(bool);

            templates.CellValidating += delegate (object sender, DataGridViewCellValidatingEventArgs e)
            {
                try
                {
                    DataGridViewRow r = templates.Rows[e.RowIndex];

                    if (e.ColumnIndex == templates.Columns["Name_"].Index)
                    {
                        if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                        {
                            if (r.Tag != null)
                                throw new Exception("Name cannot be empty!");
                            return;
                        }
                        foreach (DataGridViewRow rr in templates.Rows)
                        {
                            if (rr.Index != e.RowIndex && (string)e.FormattedValue == (string)rr.Cells["Name_"].Value)
                                throw new Exception("Name '" + e.FormattedValue + "' is duplicated!");
                        }
                        if ((string)r.Cells["Name_"].Value != ((string)e.FormattedValue).Trim())
                            r.Cells["Name_"].Value = ((string)e.FormattedValue).Trim();
                    }
                    else if (e.ColumnIndex == templates.Columns["Active"].Index)
                    {
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    Message.Error2(ex);
                }
            };

            templates.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                try
                {
                    if (e.Row == null || e.Row.Tag == null)
                        return;
                    if (!Message.YesNo("Template '" + e.Row.Cells["Name_"].Value + "' will be deleted forever! Are you sure to proceed?"))
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (e.Row.Index > 0)
                        templates.Rows[e.Row.Index - 1].Selected = true;
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
                    if (e.Row == null || e.Row.Tag == null)
                        return;

                    Template t = (Template)e.Row.Tag;
                    Settings.Templates.Templates.RemoveAll(x => x == t);
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
                if (r.Tag == null)
                {
                    //editTemplate(r);
                    return;
                }
                Template t = (Template)r.Tag;
                templates.EndEdit();//needed to set checkbox values

                t.Name = (string)r.Cells["Name_"].Value;
                t.Active = getBoolValue(r, "Active");
                t.Group = (string)r.Cells["Group"].Value;
            };

            templates.RowValidating += delegate (object sender, DataGridViewCellCancelEventArgs e)
            {
            };

            TemplateManager.Templates = templates;

            templates.DefaultValuesNeeded += delegate (object sender, DataGridViewRowEventArgs e)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex);
                }
            };

            templates.UserAddedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
            };

            templates.SelectionChanged += delegate (object sender, EventArgs e)
            {
                return;
                if (templates.SelectedRows.Count < 1)
                    return;
                var r = templates.SelectedRows[0];
                if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                {
                    try
                    {
                        int i = templates.Rows.Add();
                        templates.Rows[i].Selected = true;
                        templates.Rows[i].Cells["Active"].Value = true;
                        templates.Rows[i].Cells["Group"].Value = "";
                    }
                    catch { }
                    return;
                }
            };

            templates.CellClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;
                DataGridViewRow r = templates.Rows[e.RowIndex];
                if (e.ColumnIndex < 0)
                    return;

                if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                {
                    try
                    {
                        int i = templates.Rows.Add();
                        templates.Rows[i].Selected = true;
                        templates.Rows[i].Cells["Active"].Value = true;
                        templates.Rows[i].Cells["Group"].Value = "";
                        r.Selected = false;
                    }
                    catch { }
                }

                switch (templates.Columns[e.ColumnIndex].Name)
                {
                    case "Edit":
                        editTemplate(r);
                        break;
                    case "Copy":
                        Template t = (Template)r.Tag;
                        if (t == null)
                            return;
                        Template t2 = SerializationRoutines.Json.Deserialize<Template>(SerializationRoutines.Json.Serialize(t));
                        t2.Name = "";
                        int i = templates.Rows.Add(new DataGridViewRow());
                        DataGridViewRow r2 = templates.Rows[i];
                        r2.Cells["Name_"].Value = t2.Name.Trim();
                        r2.Cells["Active"].Value = t2.Active;
                        r2.Cells["Group"].Value = t2.Group;
                        r2.Tag = t2;
                        editTemplate(r2);
                        break;
                }
            };

            templates.Validating += delegate (object sender, CancelEventArgs e)
            {
                Settings.Templates.Templates.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name));
                Settings.Templates.Save();
            };

            progress.Maximum = 10000;

            if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
                OutputFolder.Text = Log.AppDir;
        }

        void editTemplate(DataGridViewRow r)
        {
            TemplateForm tf;
            if (rows2TemplateForm.TryGetValue(r, out tf) && !tf.IsDisposed)
            {
                tf.Show();
                tf.Activate();
                return;
            }

            Template t = (Template)r.Tag;
            if (t == null)
            {
                t = Settings.Templates.CreateInitialTemplate();
                if (!string.IsNullOrWhiteSpace((string)r.Cells["Name_"].Value))
                    t.Name = (string)r.Cells["Name_"].Value;
                //t.Active = Convert.ToBoolean(r.Cells["Active"].Value);
            }
            else
                t.Name = (string)r.Cells["Name_"].Value;
            string lastTestFile = null;
            if (t.Name != null)
                Settings.TestFiles.TemplateNames2TestFile.TryGetValue(t.Name, out lastTestFile);
            TemplateManager tm = new TemplateManager { Template = t, LastTestFile = lastTestFile, Row = r };
            tf = new TemplateForm(tm, Settings.General.InputFolder);
            tf.FormClosed += delegate
            {
                if (tm.Template.TestFile != tm.LastTestFile && tm.LastTestFile != null)//the customer asked for this
                {
                    Settings.TestFiles.TemplateNames2TestFile[tm.Template.Name] = tm.LastTestFile;
                    var deletedNs = Settings.TestFiles.TemplateNames2TestFile.Keys.Where(n => Settings.Templates.Templates.Where(a => a.Name == n).FirstOrDefault() == null).ToList();
                    foreach (string n in deletedNs)
                        Settings.TestFiles.TemplateNames2TestFile.Remove(n);
                    Settings.TestFiles.Save();
                }
            };
            tf.Show();
            rows2TemplateForm[r] = tf;
        }
        Dictionary<DataGridViewRow, TemplateForm> rows2TemplateForm = new Dictionary<DataGridViewRow, TemplateForm>();

        public class TemplateManager : TemplateForm.TemplateManager
        {
            static internal DataGridView Templates;
            internal DataGridViewRow Row;

            public override PdfDocumentParser.Template New()
            {
                return new Template();
            }

            override public void ReplaceWith(PdfDocumentParser.Template newTemplate)
            {
                if (Settings.Templates.Templates.Where(a => a != Template && a.Name == newTemplate.Name).FirstOrDefault() != null)
                    throw new Exception("Template '" + newTemplate.Name + "' already exists.");

                if (!Settings.Templates.Templates.Contains(Template))
                    Settings.Templates.Templates.Add((Template)newTemplate);
                else
                    Settings.Templates.Templates[Settings.Templates.Templates.IndexOf((Template)Template)] = (Template)newTemplate;
                Settings.Templates.Save();

                Row.Tag = newTemplate;
                Row.Cells["Name_"].Value = newTemplate.Name;

                Template = newTemplate;
            }

            override public void SaveAsInitialTemplate(PdfDocumentParser.Template template)
            {
                Settings.Templates.InitialTemplate = (Template)template;
                Settings.Templates.Save();
            }
        }

        public void LoadTemplates()
        {
            try
            {
                templates.Rows.Clear();
                foreach (Template t in Settings.Templates.Templates)
                {
                    if (string.IsNullOrWhiteSpace(t.Name))
                        continue;
                    int i = templates.Rows.Add(new DataGridViewRow());
                    DataGridViewRow r = templates.Rows[i];
                    r.Cells["Name_"].Value = t.Name.Trim();
                    r.Cells["Active"].Value = t.Active;
                    r.Cells["Group"].Value = t.Group;
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
            if (processorThread != null && processorThread.IsAlive)
            {
                if (!LogMessage.AskYesNo("Processing is running. Would you like to abort it and restart?", true))
                    return;
                while (processorThread.IsAlive)
                {
                    processorThread.Abort();
                    Thread.Sleep(100);
                }
            }

            Settings.General.InputFolder = InputFolder.Text;
            Settings.General.OutputFolder = OutputFolder.Text;
            Settings.General.Save();

            bRun.Enabled = false;
            processorThread = Cliver.ThreadRoutines.Start(
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

        Thread processorThread = null;

        private void bAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            if (processorThread != null && processorThread.IsAlive)
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
            SettingsForm f = new SettingsForm();
            f.ShowDialog();
        }

        private void help_Click(object sender, EventArgs e)
        {
            string helpFile = @"help\help.html";
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

        bool getBoolValue(DataGridViewRow r, string name)
        {
            bool? s = (bool?)r.Cells[name].Value;
            return s == null ? false : (bool)s;
        }

        string getStringValue(DataGridViewRow r, string name)
        {
            string s = (string)r.Cells[name].Value;
            return s == null ? "" : s;
        }

        void initiateSelectionEngine()
        {
            templates.SelectionChanged += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    r.Cells["Selected"].Value = r.Selected;
            };

            useActivePattern.CheckedChanged += delegate { activePattern.Enabled = useActivePattern.Checked; };
            useNamePattern.CheckedChanged += delegate { namePattern.Enabled = useNamePattern.Checked; };
            useGroupPattern.CheckedChanged += delegate { groupPattern.Enabled = useGroupPattern.Checked; };

            select.Click += delegate { selectTemplates(); };
            selectAll.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Selected = true;
                }
            };
            selectNothing.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Selected = false;
                }
            };
            selectInvertion.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Selected = !r.Selected;
                }
            };

            applyActiveChange.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    if (r.Selected)
                        r.Cells["Active"].Value = activeChange.Checked;
            };
            applyGroupChange.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    if (r.Selected)
                        r.Cells["Group"].Value = groupChange.Text;
            };
        }

        void selectTemplates()
        {
            foreach (DataGridViewRow r in templates.Rows)
            {
                r.Selected = (!activePattern.Enabled || (getBoolValue(r, "Active") == activePattern.Checked))
                     && (!namePattern.Enabled || (string.IsNullOrEmpty(namePattern.Text) ? string.IsNullOrEmpty(getStringValue(r, "Name_")) : Regex.IsMatch(getStringValue(r, "Name_"), namePattern.Text, RegexOptions.IgnoreCase)))
                     && (!groupPattern.Enabled || (string.IsNullOrEmpty(groupPattern.Text)? string.IsNullOrEmpty( getStringValue(r, "Group")) : Regex.IsMatch(getStringValue(r, "Group"), groupPattern.Text, RegexOptions.IgnoreCase)));
            }
        }
    }
}