namespace Cliver.PdfDocumentParser
{
    partial class TemplateForm
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
            this.picture = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.anchorsContainer = new System.Windows.Forms.SplitContainer();
            this.anchors = new System.Windows.Forms.DataGridView();
            this.Id3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ParentAnchorId3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.conditions = new System.Windows.Forms.DataGridView();
            this.CheckConditionsAutomaticallyWhenPageChanged = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.selectionCoordinates = new System.Windows.Forms.Label();
            this.bNextPage = new System.Windows.Forms.Button();
            this.lTotalPages = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.ShowPdfText = new System.Windows.Forms.LinkLabel();
            this.label17 = new System.Windows.Forms.Label();
            this.ShowOcrText = new System.Windows.Forms.LinkLabel();
            this.bPrevPage = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tCurrentPage = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.autoDeskewThreshold = new System.Windows.Forms.NumericUpDown();
            this.autoDeskew = new System.Windows.Forms.CheckBox();
            this.pageRotation = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.testFile = new System.Windows.Forms.TextBox();
            this.bTestFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureScale = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.fields = new System.Windows.Forms.DataGridView();
            this.Name_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Rectangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancel = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.Configure = new System.Windows.Forms.LinkLabel();
            this.label16 = new System.Windows.Forms.Label();
            this.Help = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.About = new System.Windows.Forms.LinkLabel();
            this.ExtractFieldsAutomaticallyWhenPageChanged = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Name2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anchorsContainer)).BeginInit();
            this.anchorsContainer.Panel1.SuspendLayout();
            this.anchorsContainer.Panel2.SuspendLayout();
            this.anchorsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anchors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditions)).BeginInit();
            this.flowLayoutPanel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoDeskewThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fields)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // picture
            // 
            this.picture.Location = new System.Drawing.Point(3, 3);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(100, 50);
            this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picture.TabIndex = 0;
            this.picture.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.picture);
            this.splitContainer1.Size = new System.Drawing.Size(1080, 649);
            this.splitContainer1.SplitterDistance = 449;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(10, 10);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.anchorsContainer);
            this.splitContainer2.Panel1.Controls.Add(this.selectionCoordinates);
            this.splitContainer2.Panel1.Controls.Add(this.bNextPage);
            this.splitContainer2.Panel1.Controls.Add(this.lTotalPages);
            this.splitContainer2.Panel1.Controls.Add(this.flowLayoutPanel4);
            this.splitContainer2.Panel1.Controls.Add(this.bPrevPage);
            this.splitContainer2.Panel1.Controls.Add(this.name);
            this.splitContainer2.Panel1.Controls.Add(this.label8);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.tCurrentPage);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add(this.testFile);
            this.splitContainer2.Panel1.Controls.Add(this.bTestFile);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.pictureScale);
            this.splitContainer2.Panel1.Controls.Add(this.label7);
            this.splitContainer2.Panel1.Controls.Add(this.label10);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.fields);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Panel2.Controls.Add(this.ExtractFieldsAutomaticallyWhenPageChanged);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Size = new System.Drawing.Size(429, 629);
            this.splitContainer2.SplitterDistance = 425;
            this.splitContainer2.TabIndex = 32;
            // 
            // anchorsContainer
            // 
            this.anchorsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.anchorsContainer.Location = new System.Drawing.Point(0, 181);
            this.anchorsContainer.Name = "anchorsContainer";
            // 
            // anchorsContainer.Panel1
            // 
            this.anchorsContainer.Panel1.Controls.Add(this.anchors);
            // 
            // anchorsContainer.Panel2
            // 
            this.anchorsContainer.Panel2.Controls.Add(this.splitContainer3);
            this.anchorsContainer.Size = new System.Drawing.Size(429, 245);
            this.anchorsContainer.SplitterDistance = 228;
            this.anchorsContainer.TabIndex = 52;
            // 
            // anchors
            // 
            this.anchors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.anchors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id3,
            this.Type3,
            this.ParentAnchorId3});
            this.anchors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.anchors.Location = new System.Drawing.Point(0, 0);
            this.anchors.MultiSelect = false;
            this.anchors.Name = "anchors";
            this.anchors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.anchors.Size = new System.Drawing.Size(228, 245);
            this.anchors.TabIndex = 50;
            // 
            // Id3
            // 
            this.Id3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Id3.HeaderText = "Id";
            this.Id3.Name = "Id3";
            this.Id3.ReadOnly = true;
            this.Id3.Width = 41;
            // 
            // Type3
            // 
            this.Type3.HeaderText = "Type";
            this.Type3.Name = "Type3";
            this.Type3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type3.Width = 70;
            // 
            // ParentAnchorId3
            // 
            this.ParentAnchorId3.HeaderText = "Parent Id";
            this.ParentAnchorId3.Name = "ParentAnchorId3";
            this.ParentAnchorId3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ParentAnchorId3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ParentAnchorId3.Width = 75;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.conditions);
            this.splitContainer3.Panel2.Controls.Add(this.CheckConditionsAutomaticallyWhenPageChanged);
            this.splitContainer3.Panel2.Controls.Add(this.label4);
            this.splitContainer3.Panel2.Controls.Add(this.label11);
            this.splitContainer3.Size = new System.Drawing.Size(197, 245);
            this.splitContainer3.SplitterDistance = 158;
            this.splitContainer3.TabIndex = 0;
            // 
            // conditions
            // 
            this.conditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conditions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name2,
            this.Value2});
            this.conditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditions.Location = new System.Drawing.Point(0, 18);
            this.conditions.MultiSelect = false;
            this.conditions.Name = "conditions";
            this.conditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.conditions.Size = new System.Drawing.Size(197, 65);
            this.conditions.TabIndex = 0;
            // 
            // CheckConditionsAutomaticallyWhenPageChanged
            // 
            this.CheckConditionsAutomaticallyWhenPageChanged.AutoSize = true;
            this.CheckConditionsAutomaticallyWhenPageChanged.Location = new System.Drawing.Point(69, 4);
            this.CheckConditionsAutomaticallyWhenPageChanged.Name = "CheckConditionsAutomaticallyWhenPageChanged";
            this.CheckConditionsAutomaticallyWhenPageChanged.Size = new System.Drawing.Size(166, 17);
            this.CheckConditionsAutomaticallyWhenPageChanged.TabIndex = 57;
            this.CheckConditionsAutomaticallyWhenPageChanged.Text = "Check When Page Changed]";
            this.CheckConditionsAutomaticallyWhenPageChanged.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 58;
            this.label4.Text = "[";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label11.Size = new System.Drawing.Size(59, 18);
            this.label11.TabIndex = 50;
            this.label11.Text = "Conditions:";
            // 
            // selectionCoordinates
            // 
            this.selectionCoordinates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionCoordinates.AutoSize = true;
            this.selectionCoordinates.Location = new System.Drawing.Point(269, 5);
            this.selectionCoordinates.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.selectionCoordinates.Name = "selectionCoordinates";
            this.selectionCoordinates.Size = new System.Drawing.Size(119, 13);
            this.selectionCoordinates.TabIndex = 32;
            this.selectionCoordinates.Text = "<selection coordinates>";
            // 
            // bNextPage
            // 
            this.bNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bNextPage.Location = new System.Drawing.Point(382, 54);
            this.bNextPage.Name = "bNextPage";
            this.bNextPage.Size = new System.Drawing.Size(47, 23);
            this.bNextPage.TabIndex = 23;
            this.bNextPage.Text = ">>";
            this.bNextPage.UseVisualStyleBackColor = true;
            this.bNextPage.Click += new System.EventHandler(this.bNextPage_Click);
            // 
            // lTotalPages
            // 
            this.lTotalPages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTotalPages.AutoSize = true;
            this.lTotalPages.Location = new System.Drawing.Point(264, 56);
            this.lTotalPages.Name = "lTotalPages";
            this.lTotalPages.Size = new System.Drawing.Size(32, 13);
            this.lTotalPages.TabIndex = 33;
            this.lTotalPages.Text = "Page";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.Controls.Add(this.ShowPdfText);
            this.flowLayoutPanel4.Controls.Add(this.label17);
            this.flowLayoutPanel4.Controls.Add(this.ShowOcrText);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(1, 81);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanel4.Size = new System.Drawing.Size(354, 23);
            this.flowLayoutPanel4.TabIndex = 51;
            // 
            // ShowPdfText
            // 
            this.ShowPdfText.AutoSize = true;
            this.ShowPdfText.Location = new System.Drawing.Point(3, 5);
            this.ShowPdfText.Name = "ShowPdfText";
            this.ShowPdfText.Size = new System.Drawing.Size(77, 13);
            this.ShowPdfText.TabIndex = 22;
            this.ShowPdfText.TabStop = true;
            this.ShowPdfText.Text = "Show Pdf Text";
            this.ShowPdfText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ShowPdfText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowPdfText_LinkClicked);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(86, 5);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(9, 13);
            this.label17.TabIndex = 24;
            this.label17.Text = "|";
            // 
            // ShowOcrText
            // 
            this.ShowOcrText.AutoSize = true;
            this.ShowOcrText.Location = new System.Drawing.Point(101, 5);
            this.ShowOcrText.Name = "ShowOcrText";
            this.ShowOcrText.Size = new System.Drawing.Size(78, 13);
            this.ShowOcrText.TabIndex = 25;
            this.ShowOcrText.TabStop = true;
            this.ShowOcrText.Text = "Show Ocr Text";
            this.ShowOcrText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ShowOcrText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowOcrText_LinkClicked);
            // 
            // bPrevPage
            // 
            this.bPrevPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPrevPage.Location = new System.Drawing.Point(326, 54);
            this.bPrevPage.Name = "bPrevPage";
            this.bPrevPage.Size = new System.Drawing.Size(47, 23);
            this.bPrevPage.TabIndex = 22;
            this.bPrevPage.Text = "<<";
            this.bPrevPage.UseVisualStyleBackColor = true;
            this.bPrevPage.Click += new System.EventHandler(this.bPrevPage_Click);
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Location = new System.Drawing.Point(51, 2);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(211, 20);
            this.name.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(195, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Page:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Template:";
            // 
            // tCurrentPage
            // 
            this.tCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tCurrentPage.Location = new System.Drawing.Point(236, 54);
            this.tCurrentPage.Name = "tCurrentPage";
            this.tCurrentPage.Size = new System.Drawing.Size(26, 20);
            this.tCurrentPage.TabIndex = 47;
            this.tCurrentPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tCurrentPage_KeyDown);
            this.tCurrentPage.Leave += new System.EventHandler(this.tCurrentPage_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.autoDeskewThreshold);
            this.groupBox1.Controls.Add(this.autoDeskew);
            this.groupBox1.Controls.Add(this.pageRotation);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(0, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 51);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scanned Document Settings";
            // 
            // autoDeskewThreshold
            // 
            this.autoDeskewThreshold.Location = new System.Drawing.Point(367, 23);
            this.autoDeskewThreshold.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.autoDeskewThreshold.Name = "autoDeskewThreshold";
            this.autoDeskewThreshold.Size = new System.Drawing.Size(52, 20);
            this.autoDeskewThreshold.TabIndex = 62;
            this.autoDeskewThreshold.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // autoDeskew
            // 
            this.autoDeskew.AutoSize = true;
            this.autoDeskew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autoDeskew.Location = new System.Drawing.Point(216, 26);
            this.autoDeskew.Name = "autoDeskew";
            this.autoDeskew.Size = new System.Drawing.Size(15, 14);
            this.autoDeskew.TabIndex = 52;
            this.autoDeskew.UseVisualStyleBackColor = true;
            // 
            // pageRotation
            // 
            this.pageRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageRotation.FormattingEnabled = true;
            this.pageRotation.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.pageRotation.Location = new System.Drawing.Point(81, 22);
            this.pageRotation.Name = "pageRotation";
            this.pageRotation.Size = new System.Drawing.Size(52, 21);
            this.pageRotation.TabIndex = 51;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(143, 26);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(72, 13);
            this.label21.TabIndex = 63;
            this.label21.Text = "Auto-deskew:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(244, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(122, 13);
            this.label14.TabIndex = 61;
            this.label14.Text = "Auto-deskew Threshold:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "Rotate Pages:";
            // 
            // testFile
            // 
            this.testFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testFile.Location = new System.Drawing.Point(51, 28);
            this.testFile.Name = "testFile";
            this.testFile.Size = new System.Drawing.Size(345, 20);
            this.testFile.TabIndex = 10;
            this.testFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // bTestFile
            // 
            this.bTestFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTestFile.Location = new System.Drawing.Point(405, 25);
            this.bTestFile.Name = "bTestFile";
            this.bTestFile.Size = new System.Drawing.Size(24, 23);
            this.bTestFile.TabIndex = 9;
            this.bTestFile.Text = "...";
            this.bTestFile.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Test File:";
            // 
            // pictureScale
            // 
            this.pictureScale.DecimalPlaces = 1;
            this.pictureScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pictureScale.Location = new System.Drawing.Point(51, 54);
            this.pictureScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.pictureScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pictureScale.Name = "pictureScale";
            this.pictureScale.Size = new System.Drawing.Size(55, 20);
            this.pictureScale.TabIndex = 42;
            this.pictureScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-3, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Scale:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(-3, 168);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Anchors:";
            // 
            // fields
            // 
            this.fields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name_,
            this.AnchorId,
            this.Rectangle,
            this.Type,
            this.Value});
            this.fields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fields.Location = new System.Drawing.Point(0, 18);
            this.fields.MultiSelect = false;
            this.fields.Name = "fields";
            this.fields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fields.Size = new System.Drawing.Size(429, 151);
            this.fields.TabIndex = 30;
            // 
            // Name_
            // 
            this.Name_.HeaderText = "Name";
            this.Name_.Name = "Name_";
            this.Name_.Width = 60;
            // 
            // AnchorId
            // 
            this.AnchorId.HeaderText = "Anchor";
            this.AnchorId.Name = "AnchorId";
            this.AnchorId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AnchorId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AnchorId.Width = 50;
            // 
            // Rectangle
            // 
            this.Rectangle.HeaderText = "Rectangle";
            this.Rectangle.Name = "Rectangle";
            this.Rectangle.ReadOnly = true;
            this.Rectangle.Width = 60;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.Width = 70;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label1.Size = new System.Drawing.Size(37, 18);
            this.label1.TabIndex = 55;
            this.label1.Text = "Fields:";
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.flowLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 169);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 31);
            this.panel1.TabIndex = 29;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.cancel);
            this.flowLayoutPanel1.Controls.Add(this.save);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(259, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(170, 31);
            this.flowLayoutPanel1.TabIndex = 27;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancel.Location = new System.Drawing.Point(92, 3);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 21;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.save.Location = new System.Drawing.Point(11, 3);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 20;
            this.save.Text = "OK";
            this.save.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.Configure);
            this.flowLayoutPanel3.Controls.Add(this.label16);
            this.flowLayoutPanel3.Controls.Add(this.Help);
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.About);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(1, 4);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(306, 23);
            this.flowLayoutPanel3.TabIndex = 28;
            // 
            // Configure
            // 
            this.Configure.AutoSize = true;
            this.Configure.Location = new System.Drawing.Point(3, 5);
            this.Configure.Name = "Configure";
            this.Configure.Size = new System.Drawing.Size(52, 13);
            this.Configure.TabIndex = 25;
            this.Configure.TabStop = true;
            this.Configure.Text = "Configure";
            this.Configure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(61, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(9, 13);
            this.label16.TabIndex = 26;
            this.label16.Text = "|";
            // 
            // Help
            // 
            this.Help.AutoSize = true;
            this.Help.Location = new System.Drawing.Point(76, 5);
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(29, 13);
            this.Help.TabIndex = 27;
            this.Help.TabStop = true;
            this.Help.Text = "Help";
            this.Help.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(111, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(9, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "|";
            // 
            // About
            // 
            this.About.AutoSize = true;
            this.About.Location = new System.Drawing.Point(126, 5);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(35, 13);
            this.About.TabIndex = 23;
            this.About.TabStop = true;
            this.About.Text = "About";
            this.About.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExtractFieldsAutomaticallyWhenPageChanged
            // 
            this.ExtractFieldsAutomaticallyWhenPageChanged.AutoSize = true;
            this.ExtractFieldsAutomaticallyWhenPageChanged.Location = new System.Drawing.Point(51, 4);
            this.ExtractFieldsAutomaticallyWhenPageChanged.Name = "ExtractFieldsAutomaticallyWhenPageChanged";
            this.ExtractFieldsAutomaticallyWhenPageChanged.Size = new System.Drawing.Size(168, 17);
            this.ExtractFieldsAutomaticallyWhenPageChanged.TabIndex = 56;
            this.ExtractFieldsAutomaticallyWhenPageChanged.Text = "Extract When Page Changed]";
            this.ExtractFieldsAutomaticallyWhenPageChanged.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 59;
            this.label5.Text = "[";
            // 
            // Name2
            // 
            this.Name2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Name2.HeaderText = "Name";
            this.Name2.Name = "Name2";
            this.Name2.Width = 60;
            // 
            // Value2
            // 
            this.Value2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Value2.HeaderText = "Expression";
            this.Value2.Name = "Value2";
            this.Value2.Width = 83;
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 649);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TemplateForm";
            this.Text = "TemplateForm";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.anchorsContainer.Panel1.ResumeLayout(false);
            this.anchorsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anchorsContainer)).EndInit();
            this.anchorsContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anchors)).EndInit();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.conditions)).EndInit();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoDeskewThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fields)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView fields;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox testFile;
        private System.Windows.Forms.Button bTestFile;
        private System.Windows.Forms.Button bNextPage;
        private System.Windows.Forms.Button bPrevPage;
        private System.Windows.Forms.Label selectionCoordinates;
        private System.Windows.Forms.Label lTotalPages;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.NumericUpDown pictureScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tCurrentPage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox pageRotation;
        private System.Windows.Forms.CheckBox autoDeskew;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView anchors;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.NumericUpDown autoDeskewThreshold;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.LinkLabel ShowPdfText;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.LinkLabel ShowOcrText;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.SplitContainer anchorsContainer;
        private System.Windows.Forms.CheckBox ExtractFieldsAutomaticallyWhenPageChanged;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.LinkLabel Configure;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.LinkLabel Help;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel About;
        private System.Windows.Forms.CheckBox CheckConditionsAutomaticallyWhenPageChanged;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewComboBoxColumn AnchorId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rectangle;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.DataGridView conditions;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type3;
        private System.Windows.Forms.DataGridViewComboBoxColumn ParentAnchorId3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value2;
    }
}