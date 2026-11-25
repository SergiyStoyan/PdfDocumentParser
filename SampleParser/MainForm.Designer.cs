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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            bAbout = new Button();
            bExit = new Button();
            bRun = new Button();
            progress = new ProgressBar();
            template2s = new DataGridView();
            bLog = new Button();
            bSettings = new Button();
            lProgress = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            bInput = new Button();
            bOutput = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            saveTemplates = new Button();
            Selected = new DataGridViewCheckBoxColumn();
            Edit = new DataGridViewButtonColumn();
            Copy = new DataGridViewButtonColumn();
            Edit2 = new DataGridViewButtonColumn();
            Active = new DataGridViewCheckBoxColumn();
            Name_ = new DataGridViewTextBoxColumn();
            OrderWeight = new DataGridViewTextBoxColumn();
            UsedTime = new DataGridViewTextBoxColumn();
            Comment = new DataGridViewTextBoxColumn();
            ModifiedTime = new DataGridViewTextBoxColumn();
            FileFilterRegex = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)template2s).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // bAbout
            // 
            bAbout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bAbout.Location = new Point(4, 3);
            bAbout.Margin = new Padding(4, 3, 4, 3);
            bAbout.Name = "bAbout";
            bAbout.Size = new Size(88, 27);
            bAbout.TabIndex = 0;
            bAbout.Text = "About";
            bAbout.UseVisualStyleBackColor = true;
            bAbout.Click += bAbout_Click;
            // 
            // bExit
            // 
            bExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bExit.Location = new Point(207, 3);
            bExit.Margin = new Padding(4, 3, 4, 3);
            bExit.Name = "bExit";
            bExit.Size = new Size(88, 27);
            bExit.TabIndex = 1;
            bExit.Text = "Exit";
            bExit.UseVisualStyleBackColor = true;
            bExit.Click += bExit_Click;
            // 
            // bRun
            // 
            bRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bRun.Location = new Point(111, 3);
            bRun.Margin = new Padding(4, 3, 4, 3);
            bRun.Name = "bRun";
            bRun.Size = new Size(88, 27);
            bRun.TabIndex = 0;
            bRun.Text = "Run";
            bRun.UseVisualStyleBackColor = true;
            bRun.Click += bRun_Click;
            // 
            // progress
            // 
            progress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progress.Location = new Point(14, 585);
            progress.Margin = new Padding(4, 3, 4, 3);
            progress.Name = "progress";
            progress.Size = new Size(913, 14);
            progress.TabIndex = 7;
            // 
            // template2s
            // 
            template2s.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            template2s.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            template2s.Columns.AddRange(new DataGridViewColumn[] { Selected, Edit, Copy, Edit2, Active, Name_, OrderWeight, UsedTime, Comment, ModifiedTime, FileFilterRegex });
            template2s.Location = new Point(18, 14);
            template2s.Margin = new Padding(4, 3, 4, 3);
            template2s.MultiSelect = false;
            template2s.Name = "template2s";
            template2s.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            template2s.Size = new Size(906, 537);
            template2s.TabIndex = 4;
            // 
            // bLog
            // 
            bLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bLog.Location = new Point(196, 3);
            bLog.Margin = new Padding(4, 3, 4, 3);
            bLog.Name = "bLog";
            bLog.Size = new Size(88, 27);
            bLog.TabIndex = 4;
            bLog.Text = "Log";
            bLog.UseVisualStyleBackColor = true;
            bLog.Click += bLog_Click;
            // 
            // bSettings
            // 
            bSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bSettings.Location = new Point(100, 3);
            bSettings.Margin = new Padding(4, 3, 4, 3);
            bSettings.Name = "bSettings";
            bSettings.Size = new Size(88, 27);
            bSettings.TabIndex = 3;
            bSettings.Text = "Settings";
            bSettings.UseVisualStyleBackColor = true;
            bSettings.Click += bSettings_Click;
            // 
            // lProgress
            // 
            lProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lProgress.AutoSize = true;
            lProgress.Location = new Point(14, 564);
            lProgress.Margin = new Padding(4, 0, 4, 0);
            lProgress.Name = "lProgress";
            lProgress.Size = new Size(22, 15);
            lProgress.TabIndex = 15;
            lProgress.Text = "     ";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            flowLayoutPanel1.Controls.Add(bAbout);
            flowLayoutPanel1.Controls.Add(bSettings);
            flowLayoutPanel1.Controls.Add(bLog);
            flowLayoutPanel1.Controls.Add(bInput);
            flowLayoutPanel1.Controls.Add(bOutput);
            flowLayoutPanel1.Location = new Point(14, 608);
            flowLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(589, 35);
            flowLayoutPanel1.TabIndex = 17;
            // 
            // bInput
            // 
            bInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bInput.Location = new Point(292, 3);
            bInput.Margin = new Padding(4, 3, 4, 3);
            bInput.Name = "bInput";
            bInput.Size = new Size(88, 27);
            bInput.TabIndex = 5;
            bInput.Text = "Input";
            bInput.UseVisualStyleBackColor = true;
            bInput.Click += bInput_Click;
            // 
            // bOutput
            // 
            bOutput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bOutput.Location = new Point(388, 3);
            bOutput.Margin = new Padding(4, 3, 4, 3);
            bOutput.Name = "bOutput";
            bOutput.Size = new Size(88, 27);
            bOutput.TabIndex = 6;
            bOutput.Text = "Output";
            bOutput.UseVisualStyleBackColor = true;
            bOutput.Click += bOutput_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            flowLayoutPanel2.Controls.Add(bExit);
            flowLayoutPanel2.Controls.Add(bRun);
            flowLayoutPanel2.Controls.Add(saveTemplates);
            flowLayoutPanel2.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel2.Location = new Point(629, 608);
            flowLayoutPanel2.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(299, 36);
            flowLayoutPanel2.TabIndex = 18;
            // 
            // saveTemplates
            // 
            saveTemplates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveTemplates.Location = new Point(15, 3);
            saveTemplates.Margin = new Padding(4, 3, 4, 3);
            saveTemplates.Name = "saveTemplates";
            saveTemplates.Size = new Size(88, 27);
            saveTemplates.TabIndex = 33;
            saveTemplates.Text = "Save";
            saveTemplates.UseVisualStyleBackColor = true;
            // 
            // Selected
            // 
            Selected.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Selected.HeaderText = "";
            Selected.Name = "Selected";
            Selected.SortMode = DataGridViewColumnSortMode.Automatic;
            Selected.Width = 21;
            // 
            // Edit
            // 
            Edit.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Edit.HeaderText = "";
            Edit.Name = "Edit";
            Edit.Text = "Edit";
            Edit.UseColumnTextForButtonValue = true;
            Edit.Width = 21;
            // 
            // Copy
            // 
            Copy.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Copy.HeaderText = "";
            Copy.Name = "Copy";
            Copy.Text = "Copy";
            Copy.UseColumnTextForButtonValue = true;
            Copy.Width = 21;
            // 
            // Edit2
            // 
            Edit2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Edit2.HeaderText = "";
            Edit2.Name = "Edit2";
            Edit2.Text = "Edit2";
            Edit2.UseColumnTextForButtonValue = true;
            Edit2.Width = 21;
            // 
            // Active
            // 
            Active.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Active.HeaderText = "Active";
            Active.Name = "Active";
            Active.SortMode = DataGridViewColumnSortMode.Automatic;
            Active.Width = 65;
            // 
            // Name_
            // 
            Name_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopLeft;
            Name_.DefaultCellStyle = dataGridViewCellStyle1;
            Name_.HeaderText = "Name";
            Name_.Name = "Name_";
            Name_.Width = 64;
            // 
            // OrderWeight
            // 
            OrderWeight.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            OrderWeight.HeaderText = "Order";
            OrderWeight.MaxInputLength = 10;
            OrderWeight.Name = "OrderWeight";
            OrderWeight.Width = 62;
            // 
            // UsedTime
            // 
            UsedTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            UsedTime.HeaderText = "Used";
            UsedTime.Name = "UsedTime";
            UsedTime.ReadOnly = true;
            UsedTime.Width = 58;
            // 
            // Comment
            // 
            Comment.HeaderText = "Comment";
            Comment.Name = "Comment";
            // 
            // ModifiedTime
            // 
            ModifiedTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            ModifiedTime.HeaderText = "Modified";
            ModifiedTime.Name = "ModifiedTime";
            ModifiedTime.ReadOnly = true;
            ModifiedTime.Width = 80;
            // 
            // FileFilterRegex
            // 
            FileFilterRegex.HeaderText = "Filter";
            FileFilterRegex.Name = "FileFilterRegex";
            FileFilterRegex.ReadOnly = true;
            FileFilterRegex.Width = 50;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(941, 660);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(lProgress);
            Controls.Add(template2s);
            Controls.Add(progress);
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(913, 648);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)template2s).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bAbout;
        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.Button bRun;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.DataGridView template2s;
        private System.Windows.Forms.Button bLog;
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button saveTemplates;
        private System.Windows.Forms.Button bInput;
        private System.Windows.Forms.Button bOutput;
        private DataGridViewCheckBoxColumn Selected;
        private DataGridViewButtonColumn Edit;
        private DataGridViewButtonColumn Copy;
        private DataGridViewButtonColumn Edit2;
        private DataGridViewCheckBoxColumn Active;
        private DataGridViewTextBoxColumn Name_;
        private DataGridViewTextBoxColumn OrderWeight;
        private DataGridViewTextBoxColumn UsedTime;
        private DataGridViewTextBoxColumn Comment;
        private DataGridViewTextBoxColumn ModifiedTime;
        private DataGridViewTextBoxColumn FileFilterRegex;
    }
}