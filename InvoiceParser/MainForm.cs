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
            Selected.ValueType = typeof(bool);
            OrderWeight.ValueType = typeof(float);
            DetectingTemplateLastPageNumber.ValueType = typeof(uint);

            Settings.Templates.TouchedChanged += delegate {
                this.BeginInvoke(() =>
                {
                    saveTemplates.Enabled = Settings.Templates.IsTouched();
                });
            };
            saveTemplates.Enabled = false;
            saveTemplates.Click += delegate
            {
                saveTemplatesFromTableIfTouched(false);
            };

            FormClosing += delegate (object sender, FormClosingEventArgs e)
              {
                  if (!saveTemplatesFromTableIfTouched(true))
                      e.Cancel = true;
              };

            templates.CellValidating += delegate (object sender, DataGridViewCellValidatingEventArgs e)
            {
                try
                {
                    DataGridViewRow r = templates.Rows[e.RowIndex];
                    Template t = (Template)r.Tag;

                    switch (templates.Columns[e.ColumnIndex].Name)
                    {
                        case "Name_":
                            {
                                if (string.IsNullOrWhiteSpace((string)e.FormattedValue))
                                {
                                    if (t != null)
                                        throw new Exception("Name cannot be empty!");
                                    return;
                                }
                                string name2 = ((string)e.FormattedValue).Trim();
                                foreach (DataGridViewRow rr in templates.Rows)
                                {
                                    if (rr.Index != e.RowIndex && name2 == (string)rr.Cells["Name_"].Value)
                                        throw new Exception("Name '" + name2 + "' is duplicated!");
                                }
                                if ((string)r.Cells["Name_"].Value != name2)
                                    r.Cells["Name_"].Value = name2;
                            }
                            return;
                            //case "OrderWeight":
                            //    if(!(e.FormattedValue is float))
                            //        throw new Exception("Order must be a float number.");
                            //    return;
                            //case "DetectingTemplateLastPageNumber":
                            //    if (!(e.FormattedValue is uint))
                            //        throw new Exception("DetectingTemplateLastPageNumber must be a uint number.");
                            //    return;
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    Message.Error2(ex);
                }
            };

            templates.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                try
                {
                    DataGridViewRow r = templates.Rows[e.RowIndex];
                    Template t = (Template)r.Tag;

                    switch (templates.Columns[e.ColumnIndex].Name)
                    {
                        case "OrderWeight":
                            throw new Exception("Order must be a float number:\r\n" + e.Exception.Message);
                        case "DetectingTemplateLastPageNumber":
                            throw new Exception("DetectingTemplateLastPageNumber must be a uint number:\r\n" + e.Exception.Message);
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            templates.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    DataGridViewRow r = templates.Rows[e.RowIndex];
                    Template t = (Template)r.Tag;
                    if (t == null)
                        return;

                    switch (templates.Columns[e.ColumnIndex].Name)
                    {
                        case "Name_":
                            t.Name = (string)r.Cells["Name_"].Value;
                            Settings.Templates.Touch();
                            return;
                        case "Active":
                            t.Active = (bool)r.Cells["Active"].Value;
                            Settings.Templates.Touch();
                            return;
                        case "Comment":
                            t.Comment = (string)r.Cells["Comment"].Value;
                            Settings.Templates.Touch();
                            return;
                        case "Group":
                            t.Group = (string)r.Cells["Group"].Value;
                            Settings.Templates.Touch();
                            return;
                        case "OrderWeight":
                            t.OrderWeight = (float)r.Cells["OrderWeight"].Value;
                            Settings.Templates.Touch();
                            return;
                        case "DetectingTemplateLastPageNumber":
                            t.DetectingTemplateLastPageNumber = (uint)r.Cells["DetectingTemplateLastPageNumber"].Value;
                            Settings.Templates.Touch();
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            templates.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
            {
                try
                {
                    if (e.Row == null || e.Row.Tag == null)
                        return;
                    if (!Message.YesNo("Template '" + e.Row.Cells["Name_"].Value + "' will be deleted! Proceed?"))
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
                Settings.Templates.Touch();
            };

            TemplateManager.Templates = templates;

            templates.SelectionChanged += delegate (object sender, EventArgs e)
            {
                //if (templates.SelectedRows.Count < 1)
                //    return;
                //var r = templates.SelectedRows[templates.SelectedRows.Count - 1];
                //if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                //{
                //    try
                //    {
                //        int i = templates.Rows.Add();
                //        templates.Rows[i].Selected = true;
                //        templates.Rows[i].Cells["Active"].Value = true;
                //        templates.Rows[i].Cells["Group"].Value = "";
                //    }
                //    catch { }
                //}
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
                        Template t = Settings.Templates.CreateInitialTemplate();
                        templates.Rows[i].Cells["Active"].Value = t.Active;
                        templates.Rows[i].Cells["Group"].Value = t.Group;
                        templates.Rows[i].Cells["OrderWeight"].Value = t.OrderWeight;
                        templates.Rows[i].Cells["OrderWeight"].Value = t.DetectingTemplateLastPageNumber;
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
                        t2.Editor.TestFile = null;
                        int i = templates.Rows.Add(new DataGridViewRow());
                        DataGridViewRow r2 = templates.Rows[i];
                        r2.Cells["Name_"].Value = t2.Name.Trim();
                        r2.Cells["Active"].Value = t2.Active;
                        r2.Cells["Group"].Value = t2.Group;
                        r2.Cells["OrderWeight"].Value = t2.OrderWeight;
                        r2.Cells["DetectingTemplateLastPageNumber"].Value = t2.DetectingTemplateLastPageNumber;
                        r2.Tag = t2;
                        editTemplate(r2);
                        break;
                }
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
                if (tm.LastTestFile != null)//the customer asked for this
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
                Template t2 = (Template)newTemplate;

                if (Settings.Templates.Templates.Where(a => a != Template && a.Name == t2.Name).FirstOrDefault() != null)
                    throw new Exception("Template '" + t2.Name + "' already exists.");

                t2.ModifiedTime = DateTime.Now;
                t2.Group = (string)Row.Cells["Group"].Value;
                t2.Comment = (string)Row.Cells["Comment"].Value;
                t2.OrderWeight = (float)Row.Cells["OrderWeight"].Value;
                t2.DetectingTemplateLastPageNumber = (uint)Row.Cells["DetectingTemplateLastPageNumber"].Value;

                if (!Settings.Templates.Templates.Contains(Template))
                    Settings.Templates.Templates.Add(t2);
                else
                    Settings.Templates.Templates[Settings.Templates.Templates.IndexOf((Template)Template)] = t2;
                Settings.Templates.Touch();

                Row.Tag = t2;
                Row.Cells["Name_"].Value = t2.Name;
                Row.Cells["ModifiedTime"].Value = t2.GetModifiedTimeAsString();

                Template = t2;
            }

            override public void SaveAsInitialTemplate(PdfDocumentParser.Template template)
            {
                Settings.Templates.InitialTemplate = (Template)template;
                Settings.Templates.Touch();
            }
        }

        bool saveTemplatesFromTableIfTouched(bool trueIfDeclined)
        {
            try
            {
                if (!Settings.Templates.IsTouched())
                    return true;

                if (!Message.YesNo("Save the recent changes to templates?"))
                    return trueIfDeclined || false;

                templates.EndEdit();//needed to set checkbox values

                Settings.Templates.Templates.Clear();
                foreach (DataGridViewRow r in templates.Rows)
                {
                    Template t = (Template)r.Tag;
                    if (t == null)
                        continue;

                    if (Settings.Templates.Templates.Where(a => a.Name == t.Name).FirstOrDefault() != null)
                        throw new Exception("Template name '" + t.Name + "' is duplicated!");
                    Settings.Templates.Templates.Add(t);
                }
                Settings.Templates.Save();
                return true;
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
                return false;
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
                    r.Cells["ModifiedTime"].Value = t.GetModifiedTimeAsString();
                    r.Cells["Comment"].Value = t.Comment;
                    r.Cells["OrderWeight"].Value = t.OrderWeight;
                    r.Cells["DetectingTemplateLastPageNumber"].Value = t.DetectingTemplateLastPageNumber;
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

            if (!saveTemplatesFromTableIfTouched(false))
                return;

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
            helpRequest();
        }

        void helpRequest()
        {
            try
            {
                System.Diagnostics.Process.Start(PdfDocumentParser.Settings.Constants.HelpFile);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
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
            useActivePattern.CheckedChanged += delegate { activePattern.Enabled = useActivePattern.Checked; };
            useNamePattern.CheckedChanged += delegate { namePattern.Enabled = useNamePattern.Checked; };
            useGroupPattern.CheckedChanged += delegate { groupPattern.Enabled = useGroupPattern.Checked; };
            useCommentPattern.CheckedChanged += delegate { commentPattern.Enabled = useCommentPattern.Checked; };
            //useOrderWeightPattern.CheckedChanged += delegate { orderWeightPattern.Enabled = useGroupPattern.Checked; };

            useActivePattern.Checked = Settings.General.UseActiveSelectPattern;
            useNamePattern.Checked = Settings.General.UseNameSelectPattern;
            useGroupPattern.Checked = Settings.General.UseGroupSelectPattern;
            useCommentPattern.Checked = Settings.General.UseCommentSelectPattern;
            //useOrderWeightPattern.Checked = Settings.General.;
            sortSelectedUp.Checked = Settings.General.SortSelectedUp;

            EventHandler select_by_filter = delegate 
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Cells["Selected"].Value = (!activePattern.Enabled || (getBoolValue(r, "Active") == activePattern.Checked))
                         && (!namePattern.Enabled || (string.IsNullOrEmpty(namePattern.Text) ? string.IsNullOrEmpty(getStringValue(r, "Name_")) : Regex.IsMatch(getStringValue(r, "Name_"), namePattern.Text, RegexOptions.IgnoreCase)))
                         && (!groupPattern.Enabled || (string.IsNullOrEmpty(groupPattern.Text) ? string.IsNullOrEmpty(getStringValue(r, "Group")) : Regex.IsMatch(getStringValue(r, "Group"), groupPattern.Text, RegexOptions.IgnoreCase)))
                 && (!commentPattern.Enabled || (string.IsNullOrEmpty(commentPattern.Text) ? string.IsNullOrEmpty(getStringValue(r, "Comment")) : Regex.IsMatch(getStringValue(r, "Comment"), commentPattern.Text, RegexOptions.IgnoreCase)));
                    //&& (!orderWeightPattern.Enabled || (string.IsNullOrEmpty(orderWeightPattern.Text) ? string.IsNullOrEmpty(getStringValue(r, "OrderWeight")) : Regex.IsMatch(getStringValue(r, "OrderWeight"), groupPattern.Text, RegexOptions.IgnoreCase)));
                }
                if (sortSelectedUp.Checked)
                    templates.Sort(templates.Columns["Selected"], ListSortDirection.Descending);

                Settings.General.UseActiveSelectPattern = useActivePattern.Checked;
                Settings.General.UseNameSelectPattern = useNamePattern.Checked;
                Settings.General.UseGroupSelectPattern = useGroupPattern.Checked;
                Settings.General.UseCommentSelectPattern = useCommentPattern.Checked;
                //Settings.General. = useOrderWeightPattern.Checked;
                Settings.General.SortSelectedUp = sortSelectedUp.Checked;
                Settings.General.Save();
            };
            selectByFilter.Click += select_by_filter;

            KeyEventHandler select_on_Enter = delegate (object sender, KeyEventArgs e)
            {
                if (e.KeyCode != Keys.Enter)
                    return;
                e.Handled = true;
                select_by_filter(null, null);
            };
            //AcceptButton = new Button { Visible = false };//just to suppress a ding
            activePattern.KeyDown += select_on_Enter;
            namePattern.KeyDown += select_on_Enter;
            groupPattern.KeyDown += select_on_Enter;
            commentPattern.KeyDown += select_on_Enter;
            //orderWeightPattern.KeyDown += select_on_Enter;

            selectAll.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Cells["Selected"].Value = true;
                }
            };
            selectNothing.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Cells["Selected"].Value = false;
                }
            };
            selectInvertion.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                {
                    r.Cells["Selected"].Value = !getBoolValue(r, "Selected");
                }
            };

            applyActiveChange.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["Active"].Value = activeChange.Checked;
                Settings.Templates.Touch();
            };
            applyGroupChange.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["Group"].Value = groupChange.Text;
                Settings.Templates.Touch();
            };
            applyOrderWeightChange.Click += delegate
            {
                foreach (DataGridViewRow r in templates.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["OrderWeight"].Value = (float)orderWeightChange.Value;
                Settings.Templates.Touch();
            };
        }
    }
}