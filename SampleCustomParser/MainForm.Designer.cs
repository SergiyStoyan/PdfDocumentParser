namespace Cliver.SampleParser
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bAbout = new System.Windows.Forms.Button();
            this.bInputFolder = new System.Windows.Forms.Button();
            this.InputFolder = new System.Windows.Forms.TextBox();
            this.bExit = new System.Windows.Forms.Button();
            this.bRun = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.template2s = new System.Windows.Forms.DataGridView();
            this.bLog = new System.Windows.Forms.Button();
            this.bSettings = new System.Windows.Forms.Button();
            this.lProgress = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.saveTemplates = new System.Windows.Forms.Button();
            this.Selected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Copy = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Edit2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Name_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifiedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileFilterRegex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.template2s)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bAbout
            // 
            this.bAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bAbout.Location = new System.Drawing.Point(3, 3);
            this.bAbout.Name = "bAbout";
            this.bAbout.Size = new System.Drawing.Size(75, 23);
            this.bAbout.TabIndex = 0;
            this.bAbout.Text = "About";
            this.bAbout.UseVisualStyleBackColor = true;
            this.bAbout.Click += new System.EventHandler(this.bAbout_Click);
            // 
            // bInputFolder
            // 
            this.bInputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bInputFolder.Location = new System.Drawing.Point(768, 8);
            this.bInputFolder.Name = "bInputFolder";
            this.bInputFolder.Size = new System.Drawing.Size(24, 23);
            this.bInputFolder.TabIndex = 1;
            this.bInputFolder.Text = "...";
            this.bInputFolder.UseVisualStyleBackColor = true;
            this.bInputFolder.Click += new System.EventHandler(this.bInputFolder_Click);
            // 
            // InputFolder
            // 
            this.InputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputFolder.Location = new System.Drawing.Point(92, 10);
            this.InputFolder.Name = "InputFolder";
            this.InputFolder.Size = new System.Drawing.Size(670, 20);
            this.InputFolder.TabIndex = 0;
            // 
            // bExit
            // 
            this.bExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExit.Location = new System.Drawing.Point(178, 3);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(75, 23);
            this.bExit.TabIndex = 1;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // bRun
            // 
            this.bRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRun.Location = new System.Drawing.Point(97, 3);
            this.bRun.Name = "bRun";
            this.bRun.Size = new System.Drawing.Size(75, 23);
            this.bRun.TabIndex = 0;
            this.bRun.Text = "Run";
            this.bRun.UseVisualStyleBackColor = true;
            this.bRun.Click += new System.EventHandler(this.bRun_Click);
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(12, 507);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(783, 12);
            this.progress.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Input Folder:";
            // 
            // template2s
            // 
            this.template2s.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.template2s.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.template2s.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selected,
            this.Edit,
            this.Copy,
            this.Edit2,
            this.Active,
            this.Name_,
            this.OrderWeight,
            this.UsedTime,
            this.Comment,
            this.ModifiedTime,
            this.FileFilterRegex});
            this.template2s.Location = new System.Drawing.Point(15, 37);
            this.template2s.MultiSelect = false;
            this.template2s.Name = "template2s";
            this.template2s.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.template2s.Size = new System.Drawing.Size(777, 440);
            this.template2s.TabIndex = 4;
            // 
            // bLog
            // 
            this.bLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLog.Location = new System.Drawing.Point(165, 3);
            this.bLog.Name = "bLog";
            this.bLog.Size = new System.Drawing.Size(75, 23);
            this.bLog.TabIndex = 4;
            this.bLog.Text = "Log";
            this.bLog.UseVisualStyleBackColor = true;
            this.bLog.Click += new System.EventHandler(this.bLog_Click);
            // 
            // bSettings
            // 
            this.bSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSettings.Location = new System.Drawing.Point(84, 3);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(75, 23);
            this.bSettings.TabIndex = 3;
            this.bSettings.Text = "Settings";
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // lProgress
            // 
            this.lProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(12, 489);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(22, 13);
            this.lProgress.TabIndex = 15;
            this.lProgress.Text = "     ";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.Controls.Add(this.bAbout);
            this.flowLayoutPanel1.Controls.Add(this.bSettings);
            this.flowLayoutPanel1.Controls.Add(this.bLog);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 527);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(505, 30);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.bExit);
            this.flowLayoutPanel2.Controls.Add(this.bRun);
            this.flowLayoutPanel2.Controls.Add(this.saveTemplates);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(539, 527);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(256, 31);
            this.flowLayoutPanel2.TabIndex = 18;
            // 
            // saveTemplates
            // 
            this.saveTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveTemplates.Location = new System.Drawing.Point(16, 3);
            this.saveTemplates.Name = "saveTemplates";
            this.saveTemplates.Size = new System.Drawing.Size(75, 23);
            this.saveTemplates.TabIndex = 33;
            this.saveTemplates.Text = "Save";
            this.saveTemplates.UseVisualStyleBackColor = true;
            // 
            // Selected
            // 
            this.Selected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Selected.HeaderText = "";
            this.Selected.Name = "Selected";
            this.Selected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Selected.Width = 21;
            // 
            // Edit
            // 
            this.Edit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Edit.HeaderText = "";
            this.Edit.Name = "Edit";
            this.Edit.Text = "Edit";
            this.Edit.UseColumnTextForButtonValue = true;
            this.Edit.Width = 21;
            // 
            // Copy
            // 
            this.Copy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Copy.HeaderText = "";
            this.Copy.Name = "Copy";
            this.Copy.Text = "Copy";
            this.Copy.UseColumnTextForButtonValue = true;
            this.Copy.Width = 21;
            // 
            // Edit2
            // 
            this.Edit2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Edit2.HeaderText = "";
            this.Edit2.Name = "Edit2";
            this.Edit2.Text = "Edit2";
            this.Edit2.UseColumnTextForButtonValue = true;
            this.Edit2.Width = 21;
            // 
            // Active
            // 
            this.Active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Active.HeaderText = "Active";
            this.Active.Name = "Active";
            this.Active.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Active.Width = 62;
            // 
            // Name_
            // 
            this.Name_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.Name_.DefaultCellStyle = dataGridViewCellStyle1;
            this.Name_.HeaderText = "Name";
            this.Name_.Name = "Name_";
            this.Name_.Width = 60;
            // 
            // OrderWeight
            // 
            this.OrderWeight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.OrderWeight.HeaderText = "Order";
            this.OrderWeight.MaxInputLength = 10;
            this.OrderWeight.Name = "OrderWeight";
            this.OrderWeight.Width = 58;
            // 
            // UsedTime
            // 
            this.UsedTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.UsedTime.HeaderText = "Used";
            this.UsedTime.Name = "UsedTime";
            this.UsedTime.ReadOnly = true;
            this.UsedTime.Width = 57;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            // 
            // ModifiedTime
            // 
            this.ModifiedTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ModifiedTime.HeaderText = "Modified";
            this.ModifiedTime.Name = "ModifiedTime";
            this.ModifiedTime.ReadOnly = true;
            this.ModifiedTime.Width = 72;
            // 
            // FileFilterRegex
            // 
            this.FileFilterRegex.HeaderText = "Filter";
            this.FileFilterRegex.Name = "FileFilterRegex";
            this.FileFilterRegex.ReadOnly = true;
            this.FileFilterRegex.Width = 50;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 572);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lProgress);
            this.Controls.Add(this.template2s);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.InputFolder);
            this.Controls.Add(this.bInputFolder);
            this.MinimumSize = new System.Drawing.Size(785, 567);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.template2s)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bAbout;
        private System.Windows.Forms.Button bInputFolder;
        private System.Windows.Forms.TextBox InputFolder;
        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.Button bRun;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView template2s;
        private System.Windows.Forms.Button bLog;
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button saveTemplates;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selected;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Copy;
        private System.Windows.Forms.DataGridViewButtonColumn Edit2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsedTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifiedTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileFilterRegex;
    }
}