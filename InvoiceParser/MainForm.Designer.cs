namespace Cliver.InvoiceParser
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bAbout = new System.Windows.Forms.Button();
            this.bInputFolder = new System.Windows.Forms.Button();
            this.bOutputFolder = new System.Windows.Forms.Button();
            this.InputFolder = new System.Windows.Forms.TextBox();
            this.OutputFolder = new System.Windows.Forms.TextBox();
            this.bExit = new System.Windows.Forms.Button();
            this.bRun = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.templates = new System.Windows.Forms.DataGridView();
            this.Active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Name_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.bLog = new System.Windows.Forms.Button();
            this.bOutput = new System.Windows.Forms.Button();
            this.bSettings = new System.Windows.Forms.Button();
            this.lProgress = new System.Windows.Forms.Label();
            this.bHeaders = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.help = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.templates)).BeginInit();
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
            this.bInputFolder.Location = new System.Drawing.Point(684, 26);
            this.bInputFolder.Name = "bInputFolder";
            this.bInputFolder.Size = new System.Drawing.Size(24, 23);
            this.bInputFolder.TabIndex = 1;
            this.bInputFolder.Text = "...";
            this.bInputFolder.UseVisualStyleBackColor = true;
            this.bInputFolder.Click += new System.EventHandler(this.bInputFolder_Click);
            // 
            // bOutputFolder
            // 
            this.bOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOutputFolder.Location = new System.Drawing.Point(684, 77);
            this.bOutputFolder.Name = "bOutputFolder";
            this.bOutputFolder.Size = new System.Drawing.Size(24, 23);
            this.bOutputFolder.TabIndex = 2;
            this.bOutputFolder.Text = "...";
            this.bOutputFolder.UseVisualStyleBackColor = true;
            this.bOutputFolder.Click += new System.EventHandler(this.bOutputFolder_Click);
            // 
            // InputFolder
            // 
            this.InputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputFolder.Location = new System.Drawing.Point(12, 29);
            this.InputFolder.Name = "InputFolder";
            this.InputFolder.Size = new System.Drawing.Size(666, 20);
            this.InputFolder.TabIndex = 3;
            // 
            // OutputFolder
            // 
            this.OutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFolder.Location = new System.Drawing.Point(12, 77);
            this.OutputFolder.Name = "OutputFolder";
            this.OutputFolder.Size = new System.Drawing.Size(666, 20);
            this.OutputFolder.TabIndex = 4;
            // 
            // bExit
            // 
            this.bExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExit.Location = new System.Drawing.Point(116, 3);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(75, 23);
            this.bExit.TabIndex = 5;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // bRun
            // 
            this.bRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRun.Location = new System.Drawing.Point(35, 3);
            this.bRun.Name = "bRun";
            this.bRun.Size = new System.Drawing.Size(75, 23);
            this.bRun.TabIndex = 6;
            this.bRun.Text = "Run";
            this.bRun.UseVisualStyleBackColor = true;
            this.bRun.Click += new System.EventHandler(this.bRun_Click);
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(12, 343);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(696, 12);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Output Folder:";
            // 
            // templates
            // 
            this.templates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.templates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.templates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Active,
            this.Name_,
            this.Edit});
            this.templates.Location = new System.Drawing.Point(15, 123);
            this.templates.Name = "templates";
            this.templates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.templates.Size = new System.Drawing.Size(693, 190);
            this.templates.TabIndex = 10;
            // 
            // Active
            // 
            this.Active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Active.HeaderText = "Active";
            this.Active.Name = "Active";
            this.Active.Width = 43;
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
            // Edit
            // 
            this.Edit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Edit.DefaultCellStyle = dataGridViewCellStyle2;
            this.Edit.HeaderText = "";
            this.Edit.Name = "Edit";
            this.Edit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Edit.Text = "Edit";
            this.Edit.UseColumnTextForButtonValue = true;
            this.Edit.Width = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Templates:";
            // 
            // bLog
            // 
            this.bLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLog.Location = new System.Drawing.Point(246, 3);
            this.bLog.Name = "bLog";
            this.bLog.Size = new System.Drawing.Size(75, 23);
            this.bLog.TabIndex = 12;
            this.bLog.Text = "Log";
            this.bLog.UseVisualStyleBackColor = true;
            this.bLog.Click += new System.EventHandler(this.bLog_Click);
            // 
            // bOutput
            // 
            this.bOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bOutput.Location = new System.Drawing.Point(408, 3);
            this.bOutput.Name = "bOutput";
            this.bOutput.Size = new System.Drawing.Size(75, 23);
            this.bOutput.TabIndex = 13;
            this.bOutput.Text = "Output";
            this.bOutput.UseVisualStyleBackColor = true;
            this.bOutput.Click += new System.EventHandler(this.bOutput_Click);
            // 
            // bSettings
            // 
            this.bSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSettings.Location = new System.Drawing.Point(165, 3);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(75, 23);
            this.bSettings.TabIndex = 14;
            this.bSettings.Text = "Settings";
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // lProgress
            // 
            this.lProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(12, 325);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(22, 13);
            this.lProgress.TabIndex = 15;
            this.lProgress.Text = "     ";
            // 
            // bHeaders
            // 
            this.bHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bHeaders.Location = new System.Drawing.Point(327, 3);
            this.bHeaders.Name = "bHeaders";
            this.bHeaders.Size = new System.Drawing.Size(75, 23);
            this.bHeaders.TabIndex = 16;
            this.bHeaders.Text = "Headers";
            this.bHeaders.UseVisualStyleBackColor = true;
            this.bHeaders.Click += new System.EventHandler(this.bHeaders_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.Controls.Add(this.bAbout);
            this.flowLayoutPanel1.Controls.Add(this.help);
            this.flowLayoutPanel1.Controls.Add(this.bSettings);
            this.flowLayoutPanel1.Controls.Add(this.bLog);
            this.flowLayoutPanel1.Controls.Add(this.bHeaders);
            this.flowLayoutPanel1.Controls.Add(this.bOutput);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 363);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(496, 30);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.bExit);
            this.flowLayoutPanel2.Controls.Add(this.bRun);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(514, 363);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(194, 31);
            this.flowLayoutPanel2.TabIndex = 18;
            // 
            // help
            // 
            this.help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.help.Location = new System.Drawing.Point(84, 3);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(75, 23);
            this.help.TabIndex = 17;
            this.help.Text = "Help";
            this.help.UseVisualStyleBackColor = true;
            this.help.Click += new System.EventHandler(this.help_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 408);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.templates);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.OutputFolder);
            this.Controls.Add(this.InputFolder);
            this.Controls.Add(this.bOutputFolder);
            this.Controls.Add(this.bInputFolder);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.templates)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bAbout;
        private System.Windows.Forms.Button bInputFolder;
        private System.Windows.Forms.Button bOutputFolder;
        private System.Windows.Forms.TextBox InputFolder;
        private System.Windows.Forms.TextBox OutputFolder;
        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.Button bRun;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView templates;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bLog;
        private System.Windows.Forms.Button bOutput;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Button bHeaders;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button help;
    }
}