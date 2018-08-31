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
            this.Group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Copy = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Selected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.bLog = new System.Windows.Forms.Button();
            this.bOutput = new System.Windows.Forms.Button();
            this.bSettings = new System.Windows.Forms.Button();
            this.lProgress = new System.Windows.Forms.Label();
            this.bHeaders = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.help = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.namePattern = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.useNamePattern = new System.Windows.Forms.CheckBox();
            this.useGroupPattern = new System.Windows.Forms.CheckBox();
            this.useActivePattern = new System.Windows.Forms.CheckBox();
            this.selectByFilter = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.activePattern = new System.Windows.Forms.CheckBox();
            this.selectInvertion = new System.Windows.Forms.Button();
            this.selectNothing = new System.Windows.Forms.Button();
            this.selectAll = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupPattern = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.applyActiveChange = new System.Windows.Forms.Button();
            this.applyGroupChange = new System.Windows.Forms.Button();
            this.activeChange = new System.Windows.Forms.CheckBox();
            this.groupChange = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.templates)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.bInputFolder.Location = new System.Drawing.Point(733, 26);
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
            this.bOutputFolder.Location = new System.Drawing.Point(733, 75);
            this.bOutputFolder.Name = "bOutputFolder";
            this.bOutputFolder.Size = new System.Drawing.Size(24, 23);
            this.bOutputFolder.TabIndex = 3;
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
            this.InputFolder.Size = new System.Drawing.Size(715, 20);
            this.InputFolder.TabIndex = 0;
            // 
            // OutputFolder
            // 
            this.OutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFolder.Location = new System.Drawing.Point(12, 75);
            this.OutputFolder.Name = "OutputFolder";
            this.OutputFolder.Size = new System.Drawing.Size(715, 20);
            this.OutputFolder.TabIndex = 2;
            // 
            // bExit
            // 
            this.bExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExit.Location = new System.Drawing.Point(116, 3);
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
            this.bRun.Location = new System.Drawing.Point(35, 3);
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
            this.progress.Location = new System.Drawing.Point(12, 464);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(745, 12);
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
            this.label2.Location = new System.Drawing.Point(12, 59);
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
            this.Group,
            this.Edit,
            this.Copy,
            this.Selected});
            this.templates.Location = new System.Drawing.Point(15, 123);
            this.templates.MultiSelect = false;
            this.templates.Name = "templates";
            this.templates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.templates.Size = new System.Drawing.Size(443, 311);
            this.templates.TabIndex = 4;
            // 
            // Active
            // 
            this.Active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Active.HeaderText = "Active";
            this.Active.Name = "Active";
            this.Active.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Active.Width = 62;
            // 
            // Name_
            // 
            this.Name_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.Name_.DefaultCellStyle = dataGridViewCellStyle2;
            this.Name_.HeaderText = "Name";
            this.Name_.Name = "Name_";
            this.Name_.Width = 60;
            // 
            // Group
            // 
            this.Group.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Group.HeaderText = "Group";
            this.Group.Name = "Group";
            this.Group.Width = 61;
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
            // Selected
            // 
            this.Selected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Selected.HeaderText = "";
            this.Selected.Name = "Selected";
            this.Selected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Selected.Width = 21;
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
            this.bLog.TabIndex = 4;
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
            this.bOutput.TabIndex = 6;
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
            this.bSettings.TabIndex = 3;
            this.bSettings.Text = "Settings";
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // lProgress
            // 
            this.lProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(12, 446);
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
            this.bHeaders.TabIndex = 5;
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 484);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(631, 30);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // help
            // 
            this.help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.help.Location = new System.Drawing.Point(84, 3);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(75, 23);
            this.help.TabIndex = 1;
            this.help.Text = "Help";
            this.help.UseVisualStyleBackColor = true;
            this.help.Click += new System.EventHandler(this.help_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.bExit);
            this.flowLayoutPanel2.Controls.Add(this.bRun);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(563, 484);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(194, 31);
            this.flowLayoutPanel2.TabIndex = 18;
            // 
            // namePattern
            // 
            this.namePattern.Location = new System.Drawing.Point(81, 59);
            this.namePattern.Name = "namePattern";
            this.namePattern.Size = new System.Drawing.Size(192, 20);
            this.namePattern.TabIndex = 19;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.useNamePattern);
            this.groupBox1.Controls.Add(this.useGroupPattern);
            this.groupBox1.Controls.Add(this.useActivePattern);
            this.groupBox1.Controls.Add(this.selectByFilter);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.activePattern);
            this.groupBox1.Controls.Add(this.selectInvertion);
            this.groupBox1.Controls.Add(this.selectNothing);
            this.groupBox1.Controls.Add(this.selectAll);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.groupPattern);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.namePattern);
            this.groupBox1.Location = new System.Drawing.Point(464, 107);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 186);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select";
            // 
            // useNamePattern
            // 
            this.useNamePattern.AutoSize = true;
            this.useNamePattern.Checked = true;
            this.useNamePattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useNamePattern.Location = new System.Drawing.Point(15, 62);
            this.useNamePattern.Name = "useNamePattern";
            this.useNamePattern.Size = new System.Drawing.Size(15, 14);
            this.useNamePattern.TabIndex = 38;
            this.useNamePattern.UseVisualStyleBackColor = true;
            // 
            // useGroupPattern
            // 
            this.useGroupPattern.AutoSize = true;
            this.useGroupPattern.Checked = true;
            this.useGroupPattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useGroupPattern.Location = new System.Drawing.Point(15, 93);
            this.useGroupPattern.Name = "useGroupPattern";
            this.useGroupPattern.Size = new System.Drawing.Size(15, 14);
            this.useGroupPattern.TabIndex = 37;
            this.useGroupPattern.UseVisualStyleBackColor = true;
            // 
            // useActivePattern
            // 
            this.useActivePattern.AutoSize = true;
            this.useActivePattern.Checked = true;
            this.useActivePattern.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useActivePattern.Location = new System.Drawing.Point(15, 31);
            this.useActivePattern.Name = "useActivePattern";
            this.useActivePattern.Size = new System.Drawing.Size(15, 14);
            this.useActivePattern.TabIndex = 35;
            this.useActivePattern.UseVisualStyleBackColor = true;
            // 
            // selectByFilter
            // 
            this.selectByFilter.Location = new System.Drawing.Point(15, 119);
            this.selectByFilter.Name = "selectByFilter";
            this.selectByFilter.Size = new System.Drawing.Size(60, 23);
            this.selectByFilter.TabIndex = 34;
            this.selectByFilter.Text = "Apply";
            this.selectByFilter.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Active:";
            // 
            // activePattern
            // 
            this.activePattern.AutoSize = true;
            this.activePattern.Location = new System.Drawing.Point(81, 31);
            this.activePattern.Name = "activePattern";
            this.activePattern.Size = new System.Drawing.Size(15, 14);
            this.activePattern.TabIndex = 32;
            this.activePattern.UseVisualStyleBackColor = true;
            // 
            // selectInvertion
            // 
            this.selectInvertion.Location = new System.Drawing.Point(213, 151);
            this.selectInvertion.Name = "selectInvertion";
            this.selectInvertion.Size = new System.Drawing.Size(60, 23);
            this.selectInvertion.TabIndex = 28;
            this.selectInvertion.Text = "Invert";
            this.selectInvertion.UseVisualStyleBackColor = true;
            // 
            // selectNothing
            // 
            this.selectNothing.Location = new System.Drawing.Point(146, 151);
            this.selectNothing.Name = "selectNothing";
            this.selectNothing.Size = new System.Drawing.Size(60, 23);
            this.selectNothing.TabIndex = 27;
            this.selectNothing.Text = "Nothing";
            this.selectNothing.UseVisualStyleBackColor = true;
            // 
            // selectAll
            // 
            this.selectAll.Location = new System.Drawing.Point(80, 151);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(60, 23);
            this.selectAll.TabIndex = 26;
            this.selectAll.Text = "All";
            this.selectAll.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Group:";
            // 
            // groupPattern
            // 
            this.groupPattern.Location = new System.Drawing.Point(81, 90);
            this.groupPattern.Name = "groupPattern";
            this.groupPattern.Size = new System.Drawing.Size(192, 20);
            this.groupPattern.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.applyActiveChange);
            this.groupBox2.Controls.Add(this.applyGroupChange);
            this.groupBox2.Controls.Add(this.activeChange);
            this.groupBox2.Controls.Add(this.groupChange);
            this.groupBox2.Location = new System.Drawing.Point(464, 299);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(293, 135);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Change Selected";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(261, 29);
            this.label9.TabIndex = 32;
            this.label9.Text = "Your approval will be asked before saving templates to disk!";
            // 
            // applyActiveChange
            // 
            this.applyActiveChange.Location = new System.Drawing.Point(15, 28);
            this.applyActiveChange.Name = "applyActiveChange";
            this.applyActiveChange.Size = new System.Drawing.Size(60, 23);
            this.applyActiveChange.TabIndex = 30;
            this.applyActiveChange.Text = "Active =";
            this.applyActiveChange.UseVisualStyleBackColor = true;
            // 
            // applyGroupChange
            // 
            this.applyGroupChange.Location = new System.Drawing.Point(15, 63);
            this.applyGroupChange.Name = "applyGroupChange";
            this.applyGroupChange.Size = new System.Drawing.Size(60, 23);
            this.applyGroupChange.TabIndex = 29;
            this.applyGroupChange.Text = "Group =";
            this.applyGroupChange.UseVisualStyleBackColor = true;
            // 
            // activeChange
            // 
            this.activeChange.AutoSize = true;
            this.activeChange.Checked = true;
            this.activeChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.activeChange.Location = new System.Drawing.Point(81, 33);
            this.activeChange.Name = "activeChange";
            this.activeChange.Size = new System.Drawing.Size(15, 14);
            this.activeChange.TabIndex = 25;
            this.activeChange.UseVisualStyleBackColor = true;
            // 
            // groupChange
            // 
            this.groupChange.Location = new System.Drawing.Point(81, 65);
            this.groupChange.Name = "groupChange";
            this.groupChange.Size = new System.Drawing.Size(192, 20);
            this.groupChange.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(34, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 39;
            this.label10.Text = "Select:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 529);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Button bHeaders;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button help;
        private System.Windows.Forms.TextBox namePattern;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button selectInvertion;
        private System.Windows.Forms.Button selectNothing;
        private System.Windows.Forms.Button selectAll;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox groupPattern;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox activeChange;
        private System.Windows.Forms.TextBox groupChange;
        private System.Windows.Forms.Button applyGroupChange;
        private System.Windows.Forms.Button applyActiveChange;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox activePattern;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button selectByFilter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewTextBoxColumn Group;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Copy;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selected;
        private System.Windows.Forms.CheckBox useGroupPattern;
        private System.Windows.Forms.CheckBox useActivePattern;
        private System.Windows.Forms.CheckBox useNamePattern;
        private System.Windows.Forms.Label label10;
    }
}