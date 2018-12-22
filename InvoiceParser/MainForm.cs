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

            initializeSelectionEngine();

            Active.ValueType = typeof(bool);
            Selected.ValueType = typeof(bool);
            OrderWeight.ValueType = typeof(float);
            DetectingTemplateLastPageNumber.ValueType = typeof(uint);
            FileFilterRegex.ValueType = typeof(Regex);
            SharedFileTemplateNamesRegex.ValueType = typeof(Regex);

            Settings.Template2s.TouchedChanged += delegate
            {
                this.BeginInvoke(() =>
                {
                    saveTemplates.Enabled = Settings.Template2s.IsTouched();
                });
            };
            saveTemplates.Enabled = false;
            saveTemplates.Click += delegate
            {
                saveTemplatesFromTableIfTouched(false);
            };

            FormClosing += delegate (object sender, FormClosingEventArgs e)
            {
                Settings.TemplateLocalInfo.Clear_Save();
                if (!saveTemplatesFromTableIfTouched(true))
                    e.Cancel = true;
            };

            template2s.CellValidating += delegate (object sender, DataGridViewCellValidatingEventArgs e)
            {
                try
                {
                    DataGridViewRow r = template2s.Rows[e.RowIndex];
                    Template2 t = (Template2)r.Tag;

                    switch (template2s.Columns[e.ColumnIndex].Name)
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
                                foreach (DataGridViewRow rr in template2s.Rows)
                                {
                                    if (rr.Index != e.RowIndex && name2 == (string)rr.Cells["Name_"].Value)
                                        throw new Exception("Name '" + name2 + "' is duplicated!");
                                }
                                if ((string)r.Cells["Name_"].Value != name2)
                                    r.Cells["Name_"].Value = name2;
                            }
                            return;
                    }
                }
                catch (Exception ex)
                {
                    e.Cancel = true;
                    Message.Error2(ex);
                }
            };

            template2s.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e)
            {
                try
                {
                    DataGridViewRow r = template2s.Rows[e.RowIndex];
                    Template2 t = (Template2)r.Tag;

                    switch (template2s.Columns[e.ColumnIndex].Name)
                    {
                        case "OrderWeight":
                            throw new Exception("Order must be a float number:\r\n" + e.Exception.Message);
                        case "DetectingTemplateLastPageNumber":
                            throw new Exception("DetectingTemplateLastPageNumber must be a uint number:\r\n" + e.Exception.Message);
                        case "FileFilterRegex":
                            throw new Exception("FileFilterRegex must be a regex.");
                        case "SharedFileTemplateNamesRegex":
                            throw new Exception("SharedFileTemplateNamesRegex must be a regex.");
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            template2s.CellValueChanged += delegate (object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    DataGridViewRow r = template2s.Rows[e.RowIndex];
                    Template2 t = (Template2)r.Tag;
                    if (t == null)
                        return;
                    if (e.ColumnIndex < 0)//row's header
                        return;

                    switch (template2s.Columns[e.ColumnIndex].Name)
                    {
                        case "Name_":
                            t.Template.Name = (string)r.Cells["Name_"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "Active":
                            t.Active = (bool)r.Cells["Active"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "Comment":
                            t.Comment = (string)r.Cells["Comment"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "Group":
                            t.Group = (string)r.Cells["Group"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "OrderWeight":
                            t.OrderWeight = (float)r.Cells["OrderWeight"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "DetectingTemplateLastPageNumber":
                            t.DetectingTemplateLastPageNumber = (uint)r.Cells["DetectingTemplateLastPageNumber"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "FileFilterRegex":
                            t.FileFilterRegex = (Regex)r.Cells["FileFilterRegex"].Value;
                            Settings.Template2s.Touch();
                            return;
                        case "SharedFileTemplateNamesRegex":
                            t.SharedFileTemplateNamesRegex = (Regex)r.Cells["SharedFileTemplateNamesRegex"].Value;
                            Settings.Template2s.Touch();
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Message.Error2(ex);
                }
            };

            template2s.UserDeletingRow += delegate (object sender, DataGridViewRowCancelEventArgs e)
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

            template2s.UserDeletedRow += delegate (object sender, DataGridViewRowEventArgs e)
            {
                Settings.Template2s.Touch();
            };

            TemplateManager.Templates = template2s;

            template2s.SelectionChanged += delegate (object sender, EventArgs e)
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

            template2s.CellClick += delegate (object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;
                DataGridViewRow r = template2s.Rows[e.RowIndex];
                if (e.ColumnIndex < 0)//row's header
                    return;

                if (r.IsNewRow)//hacky forcing commit a newly added row and display the blank row
                {
                    try
                    {
                        int i = template2s.Rows.Add();
                        r = template2s.Rows[i];
                        Template2 t = Settings.Template2s.CreateInitialTemplate();
                        r.Tag = t;
                        r.Cells["Active"].Value = t.Active;
                        r.Cells["Group"].Value = t.Group;
                        r.Cells["OrderWeight"].Value = t.OrderWeight;
                        r.Cells["DetectingTemplateLastPageNumber"].Value = t.DetectingTemplateLastPageNumber;
                        r.Cells["FileFilterRegex"].Value = t.FileFilterRegex;
                        r.Cells["SharedFileTemplateNamesRegex"].Value = t.SharedFileTemplateNamesRegex;
                        r.Selected = true;
                    }
                    catch { }
                }

                switch (template2s.Columns[e.ColumnIndex].Name)
                {
                    case "Edit":
                        editTemplate(r);
                        break;
                    case "Copy":
                        Template2 t = (Template2)r.Tag;
                        if (t == null)
                            return;
                        Template2 t2 = SerializationRoutines.Json.Deserialize<Template2>(SerializationRoutines.Json.Serialize(t));
                        t2.Template.Name = "";
                        t2.Template.Editor.TestFile = null;
                        int i = template2s.Rows.Add(new DataGridViewRow());
                        DataGridViewRow r2 = template2s.Rows[i];
                        r2.Tag = t2;
                        r2.Cells["Name_"].Value = t2.Template.Name.Trim();
                        r2.Cells["Active"].Value = t2.Active;
                        r2.Cells["Group"].Value = t2.Group;
                        r2.Cells["OrderWeight"].Value = t2.OrderWeight;
                        r2.Cells["DetectingTemplateLastPageNumber"].Value = t2.DetectingTemplateLastPageNumber;
                        r2.Cells["FileFilterRegex"].Value = t2.FileFilterRegex;
                        r2.Cells["SharedFileTemplateNamesRegex"].Value = t2.SharedFileTemplateNamesRegex;
                        editTemplate(r2);
                        break;
                    case "Edit2":
                        edit2Template(r);
                        break;
                }
            };

            progress.Maximum = 10000;

            if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
                OutputFolder.Text = Log.AppDir;
        }

        void edit2Template(DataGridViewRow r)
        {
            Template2 t = (Template2)r.Tag;
            if (t == null)
                return;

         Template2Form   tf = new Template2Form(t);
            if (tf.ShowDialog() != DialogResult.OK)
                return;
            r.Cells["Active"].Value = t.Active;
            r.Cells["Group"].Value = t.Group;
            r.Cells["Comment"].Value = t.Comment;
            r.Cells["OrderWeight"].Value = t.OrderWeight;
            r.Cells["DetectingTemplateLastPageNumber"].Value = t.DetectingTemplateLastPageNumber;
            r.Cells["FileFilterRegex"].Value = t.FileFilterRegex;
            r.Cells["SharedFileTemplateNamesRegex"].Value = t.SharedFileTemplateNamesRegex;
        }

        void editTemplate(DataGridViewRow r)
        {
            if (rows2TemplateForm.TryGetValue(r, out TemplateForm tf) && !tf.IsDisposed)
            {
                tf.Show();
                tf.Activate();
                return;
            }

            Template2 t = (Template2)r.Tag;
            if (t == null)
            {
                t = Settings.Template2s.CreateInitialTemplate();
                if (!string.IsNullOrWhiteSpace((string)r.Cells["Name_"].Value))
                    t.Template.Name = (string)r.Cells["Name_"].Value;
                r.Tag = t;
            }

            TemplateManager tm = new TemplateManager(
                r,
                SerializationRoutines.Json.Clone(t.Template),
                Settings.TemplateLocalInfo.GetInfo(t.Template.Name).LastTestFile,
                Settings.General.InputFolder
                );
            Template2 it = Settings.Template2s.CreateInitialTemplate();
            foreach (Template.Field f in tm.Template.Fields)
            {
                int i = it.Template.Fields.FindIndex(x => x.Name == f.Name);
                if (i >= 0)
                    it.Template.Fields[i] = f;
                else
                    it.Template.Fields.Add(f);
            }
            tm.Template.Fields = it.Template.Fields;

            tf = new TemplateForm(tm);
            tf.FormClosed += delegate
            {
                if (tm.LastTestFile != null)
                    Settings.TemplateLocalInfo.SetLastTestFile(tm.Template.Name, tm.LastTestFile);
            };
            tf.Show();
            rows2TemplateForm[r] = tf;
        }
        Dictionary<DataGridViewRow, TemplateForm> rows2TemplateForm = new Dictionary<DataGridViewRow, TemplateForm>();

        public class TemplateManager : TemplateForm.TemplateManager
        {
            public TemplateManager(DataGridViewRow row, Template template, string lastTestFile, string testFileDefaultFolder) : base(template, lastTestFile, testFileDefaultFolder)
            {
                Row = row;
            }

            static internal DataGridView Templates;
            internal DataGridViewRow Row;

            override public void Save()
            {
                Template2 t = (Template2)Row.Tag;
                if (Settings.Template2s.Template2s.Where(a => a != t && a.Template.Name == Template.Name).FirstOrDefault() != null)
                    throw new Exception("Template '" + Template.Name + "' already exists.");

                Template2 it = Settings.Template2s.CreateInitialTemplate();
                foreach (Template.Condition c in it.Template.Conditions)
                    if (Template.Conditions.FirstOrDefault(x => x.Name == c.Name) == null)
                        throw new Exception("The template does not have obligatory condition '" + c.Name + "'.");

                foreach (Template.Field f in it.Template.Fields)
                    if (Template.Fields.FirstOrDefault(x => x.Name == f.Name) == null)
                        throw new Exception("The template does not have obligatory field '" + f.Name + "'.");

                t.Template = Template;
                t.ModifiedTime = DateTime.Now;

                if (!Settings.Template2s.Template2s.Contains(t))
                    Settings.Template2s.Template2s.Add(t);

                Settings.Template2s.Touch();

                Row.Cells["Name_"].Value = t.Template.Name;
                Row.Cells["ModifiedTime"].Value = t.GetModifiedTimeAsString();
            }
        }

        bool saveTemplatesFromTableIfTouched(bool trueIfDeclined)
        {
            try
            {
                if (!Settings.Template2s.IsTouched())
                    return true;

                if (!Message.YesNo("Save the recent changes to templates?"))
                    return trueIfDeclined || false;

                template2s.EndEdit();//needed to set checkbox values

                Settings.Template2s.Template2s.Clear();
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    Template2 t = (Template2)r.Tag;
                    if (t == null)
                        continue;

                    if (Settings.Template2s.Template2s.Where(a => a.Template.Name == t.Template.Name).FirstOrDefault() != null)
                        throw new Exception("Template name '" + t.Template.Name + "' is duplicated!");
                    Settings.Template2s.Template2s.Add(t);
                }
                Settings.Template2s.Save();
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
                template2s.Rows.Clear();
                foreach (Template2 t in Settings.Template2s.Template2s)
                {
                    if (string.IsNullOrWhiteSpace(t.Template.Name))
                        continue;
                    int i = template2s.Rows.Add(new DataGridViewRow());
                    DataGridViewRow r = template2s.Rows[i];
                    r.Cells["Name_"].Value = t.Template.Name.Trim();
                    r.Cells["Active"].Value = t.Active;
                    r.Cells["Group"].Value = t.Group;
                    r.Cells["ModifiedTime"].Value = t.GetModifiedTimeAsString();
                    r.Cells["Comment"].Value = t.Comment;
                    r.Cells["OrderWeight"].Value = t.OrderWeight;
                    r.Cells["DetectingTemplateLastPageNumber"].Value = t.DetectingTemplateLastPageNumber;
                    r.Cells["FileFilterRegex"].Value = t.FileFilterRegex;
                    r.Cells["UsedTime"].Value = Settings.TemplateLocalInfo.GetInfo(t.Template.Name).GetUsedTimeAsString();
                    r.Cells["SharedFileTemplateNamesRegex"].Value = t.SharedFileTemplateNamesRegex;
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
            try
            {
                System.Diagnostics.Process.Start(Settings.General.HelpFile);
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

        //string getStringValue(DataGridViewRow r, string name)
        //{
        //    string s = (string)r.Cells[name].Value;
        //    return s == null ? "" : s;
        //}

        //float getFloatValue(DataGridViewRow r, string name)
        //{
        //    float? f = (float?)r.Cells[name].Value;
        //    return f == null ? 0f : (float)f;
        //}

        void initializeSelectionEngine()
        {
            useActivePattern.CheckedChanged += delegate { activePattern.Enabled = useActivePattern.Checked; };
            useNamePattern.CheckedChanged += delegate { namePattern.Enabled = useNamePattern.Checked; };
            useGroupPattern.CheckedChanged += delegate { groupPattern.Enabled = useGroupPattern.Checked; };
            useCommentPattern.CheckedChanged += delegate { commentPattern.Enabled = useCommentPattern.Checked; };
            useOrderWeightPattern.CheckedChanged += delegate { orderWeightPattern1.Enabled = orderWeightPattern2.Enabled = useOrderWeightPattern.Checked; };

            useActivePattern.Checked = Settings.General.UseActiveSelectPattern;
            useNamePattern.Checked = Settings.General.UseNameSelectPattern;
            useGroupPattern.Checked = Settings.General.UseGroupSelectPattern;
            useCommentPattern.Checked = Settings.General.UseCommentSelectPattern;
            useOrderWeightPattern.Checked = Settings.General.UseOrderWeightPattern;
            sortSelectedUp.Checked = Settings.General.SortSelectedUp;

            void showSelectedCount()
            {
                int count = 0;
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    if (getBoolValue(r, "Selected"))
                        count++;
                }
                selectedTemplatesCount.Text = "Selected: " + count + " templates";
            };

            void select_by_filter(object sender, EventArgs e)
            {
                float orderWeight1 = (float)orderWeightPattern1.Value;
                float orderWeight2 = (float)orderWeightPattern2.Value;
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    Template2 t = (Template2)r.Tag;
                    if (t == null)
                        continue;
                    bool s = (!activePattern.Enabled || (t.Active == activePattern.Checked))
                         && (!namePattern.Enabled || (string.IsNullOrEmpty(namePattern.Text) ? string.IsNullOrEmpty(t.Template.Name) : Regex.IsMatch(t.Template.Name, namePattern.Text, RegexOptions.IgnoreCase)))
                         && (!groupPattern.Enabled || (string.IsNullOrEmpty(groupPattern.Text) ? string.IsNullOrEmpty(t.Group) : Regex.IsMatch(t.Group, groupPattern.Text, RegexOptions.IgnoreCase)))
                         && (!commentPattern.Enabled || (string.IsNullOrEmpty(commentPattern.Text) ? string.IsNullOrEmpty(t.Comment) : Regex.IsMatch(t.Comment, commentPattern.Text, RegexOptions.IgnoreCase)))
                         && (!orderWeightPattern2.Enabled || (orderWeight1 <= t.OrderWeight && t.OrderWeight <= orderWeight2));
                    r.Cells["Selected"].Value = s;
                }
                if (sortSelectedUp.Checked)
                    template2s.Sort(template2s.Columns["Selected"], ListSortDirection.Descending);

                showSelectedCount();

                Settings.General.UseActiveSelectPattern = useActivePattern.Checked;
                Settings.General.UseNameSelectPattern = useNamePattern.Checked;
                Settings.General.UseGroupSelectPattern = useGroupPattern.Checked;
                Settings.General.UseCommentSelectPattern = useCommentPattern.Checked;
                Settings.General.UseOrderWeightPattern = useOrderWeightPattern.Checked;
                Settings.General.SortSelectedUp = sortSelectedUp.Checked;
                Settings.General.Save();
            };
            selectByFilter.Click += select_by_filter;

            void select_on_Enter(object sender, KeyEventArgs e)
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
            orderWeightPattern1.KeyDown += select_on_Enter;
            orderWeightPattern2.KeyDown += select_on_Enter;

            selectAll.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    r.Cells["Selected"].Value = true;
                }
                showSelectedCount();
            };
            selectNothing.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    r.Cells["Selected"].Value = false;
                }
                showSelectedCount();
            };
            selectInvertion.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                {
                    r.Cells["Selected"].Value = !getBoolValue(r, "Selected");
                }
                showSelectedCount();
            };

            applyActiveChange.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["Active"].Value = activeChange.Checked;
                Settings.Template2s.Touch();
            };
            applyGroupChange.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["Group"].Value = groupChange.Text;
                Settings.Template2s.Touch();
            };
            applyOrderWeightChange.Click += delegate
            {
                foreach (DataGridViewRow r in template2s.Rows)
                    if (getBoolValue(r, "Selected"))
                        r.Cells["OrderWeight"].Value = (float)orderWeightChange.Value;
                Settings.Template2s.Touch();
            };
        }
    }
}