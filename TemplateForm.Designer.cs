namespace Cliver.InvoiceParser
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
            this.fields = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.floatingAnchors = new System.Windows.Forms.DataGridView();
            this.Id3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rectangle3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueType3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pageRotation = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.autoDeskew = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.bText = new System.Windows.Forms.Button();
            this.bIsInvoiceFirstPage = new System.Windows.Forms.Button();
            this.cSelectAnchor = new System.Windows.Forms.CheckBox();
            this.tCurrentPage = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureScale = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bTestFileFilterRegex = new System.Windows.Forms.Button();
            this.fileFilterRegex = new System.Windows.Forms.TextBox();
            this.lStatus = new System.Windows.Forms.TextBox();
            this.lTotalPages = new System.Windows.Forms.Label();
            this.selectionCoordinates = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.invoiceFirstPageRecognitionMarks = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.bNextPage = new System.Windows.Forms.Button();
            this.bPrevPage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.testFile = new System.Windows.Forms.TextBox();
            this.bTestFile = new System.Windows.Forms.Button();
            this.Name_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FloatingAnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Rectangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ocr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rectangle2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FloatingAnchorId2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ValueType2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fields)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.floatingAnchors)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.invoiceFirstPageRecognitionMarks)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.fields);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.picture);
            this.splitContainer1.Size = new System.Drawing.Size(1080, 639);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 1;
            // 
            // fields
            // 
            this.fields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name_,
            this.FloatingAnchorId,
            this.Rectangle,
            this.Ocr,
            this.Value});
            this.fields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fields.Location = new System.Drawing.Point(10, 490);
            this.fields.MultiSelect = false;
            this.fields.Name = "fields";
            this.fields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fields.Size = new System.Drawing.Size(338, 108);
            this.fields.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(10, 477);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Fields:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.save);
            this.flowLayoutPanel1.Controls.Add(this.cancel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 598);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(338, 31);
            this.flowLayoutPanel1.TabIndex = 27;
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.save.Location = new System.Drawing.Point(260, 3);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 20;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancel.Location = new System.Drawing.Point(179, 3);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 21;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.floatingAnchors);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Controls.Add(this.tCurrentPage);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.pictureScale);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.name);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.bTestFileFilterRegex);
            this.panel1.Controls.Add(this.fileFilterRegex);
            this.panel1.Controls.Add(this.lStatus);
            this.panel1.Controls.Add(this.lTotalPages);
            this.panel1.Controls.Add(this.selectionCoordinates);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.invoiceFirstPageRecognitionMarks);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.bNextPage);
            this.panel1.Controls.Add(this.bPrevPage);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.testFile);
            this.panel1.Controls.Add(this.bTestFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(338, 467);
            this.panel1.TabIndex = 31;
            // 
            // floatingAnchors
            // 
            this.floatingAnchors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.floatingAnchors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.floatingAnchors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id3,
            this.Rectangle3,
            this.ValueType3,
            this.Value3});
            this.floatingAnchors.Location = new System.Drawing.Point(0, 309);
            this.floatingAnchors.MultiSelect = false;
            this.floatingAnchors.Name = "floatingAnchors";
            this.floatingAnchors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.floatingAnchors.Size = new System.Drawing.Size(335, 68);
            this.floatingAnchors.TabIndex = 50;
            // 
            // Id3
            // 
            this.Id3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Id3.HeaderText = "Id";
            this.Id3.Name = "Id3";
            this.Id3.ReadOnly = true;
            this.Id3.Width = 41;
            // 
            // Rectangle3
            // 
            this.Rectangle3.HeaderText = "Rectangle";
            this.Rectangle3.Name = "Rectangle3";
            // 
            // ValueType3
            // 
            this.ValueType3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ValueType3.HeaderText = "Value Type";
            this.ValueType3.Name = "ValueType3";
            this.ValueType3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ValueType3.Width = 67;
            // 
            // Value3
            // 
            this.Value3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value3.HeaderText = "Value";
            this.Value3.Name = "Value3";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1, 293);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Floating Anchors:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pageRotation);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.autoDeskew);
            this.groupBox1.Location = new System.Drawing.Point(0, 236);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 54);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scanned Pdf Settings";
            // 
            // pageRotation
            // 
            this.pageRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageRotation.FormattingEnabled = true;
            this.pageRotation.Items.AddRange(new object[] {
            "---",
            "90 degree clockwise",
            "180 degree",
            "90 degree counterclockwise"});
            this.pageRotation.Location = new System.Drawing.Point(77, 23);
            this.pageRotation.Name = "pageRotation";
            this.pageRotation.Size = new System.Drawing.Size(121, 21);
            this.pageRotation.TabIndex = 51;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "Rotate Pages:";
            // 
            // autoDeskew
            // 
            this.autoDeskew.AutoSize = true;
            this.autoDeskew.Location = new System.Drawing.Point(218, 26);
            this.autoDeskew.Name = "autoDeskew";
            this.autoDeskew.Size = new System.Drawing.Size(88, 17);
            this.autoDeskew.TabIndex = 52;
            this.autoDeskew.Text = "Auto-deskew";
            this.autoDeskew.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.bText);
            this.flowLayoutPanel2.Controls.Add(this.bIsInvoiceFirstPage);
            this.flowLayoutPanel2.Controls.Add(this.cSelectAnchor);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(1, 197);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(334, 31);
            this.flowLayoutPanel2.TabIndex = 48;
            // 
            // bText
            // 
            this.bText.AutoSize = true;
            this.bText.Location = new System.Drawing.Point(3, 3);
            this.bText.Name = "bText";
            this.bText.Size = new System.Drawing.Size(68, 23);
            this.bText.TabIndex = 45;
            this.bText.Text = "Show Text";
            this.bText.UseVisualStyleBackColor = true;
            this.bText.Click += new System.EventHandler(this.bText_Click);
            // 
            // bIsInvoiceFirstPage
            // 
            this.bIsInvoiceFirstPage.AutoSize = true;
            this.bIsInvoiceFirstPage.Location = new System.Drawing.Point(77, 3);
            this.bIsInvoiceFirstPage.Name = "bIsInvoiceFirstPage";
            this.bIsInvoiceFirstPage.Size = new System.Drawing.Size(119, 23);
            this.bIsInvoiceFirstPage.TabIndex = 27;
            this.bIsInvoiceFirstPage.Text = "Refresh Status";
            this.bIsInvoiceFirstPage.UseVisualStyleBackColor = true;
            this.bIsInvoiceFirstPage.Click += new System.EventHandler(this.bIsInvoiceFirstPage_Click);
            // 
            // cSelectAnchor
            // 
            this.cSelectAnchor.Appearance = System.Windows.Forms.Appearance.Button;
            this.cSelectAnchor.AutoSize = true;
            this.cSelectAnchor.Location = new System.Drawing.Point(202, 3);
            this.cSelectAnchor.Name = "cSelectAnchor";
            this.cSelectAnchor.Size = new System.Drawing.Size(124, 23);
            this.cSelectAnchor.TabIndex = 44;
            this.cSelectAnchor.Text = "Select Floating Anchor";
            this.cSelectAnchor.UseVisualStyleBackColor = true;
            // 
            // tCurrentPage
            // 
            this.tCurrentPage.Location = new System.Drawing.Point(157, 1);
            this.tCurrentPage.Name = "tCurrentPage";
            this.tCurrentPage.Size = new System.Drawing.Size(26, 20);
            this.tCurrentPage.TabIndex = 47;
            this.tCurrentPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tCurrentPage_KeyDown);
            this.tCurrentPage.Leave += new System.EventHandler(this.tCurrentPage_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(116, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Page:";
            // 
            // pictureScale
            // 
            this.pictureScale.DecimalPlaces = 1;
            this.pictureScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pictureScale.Location = new System.Drawing.Point(52, 2);
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
            this.label7.Location = new System.Drawing.Point(-3, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Scale:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-3, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Name/Company:";
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Location = new System.Drawing.Point(86, 62);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(199, 20);
            this.name.TabIndex = 39;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "File Filter Regex:";
            // 
            // bTestFileFilterRegex
            // 
            this.bTestFileFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTestFileFilterRegex.Location = new System.Drawing.Point(291, 91);
            this.bTestFileFilterRegex.Name = "bTestFileFilterRegex";
            this.bTestFileFilterRegex.Size = new System.Drawing.Size(47, 23);
            this.bTestFileFilterRegex.TabIndex = 37;
            this.bTestFileFilterRegex.Text = "Test";
            this.bTestFileFilterRegex.UseVisualStyleBackColor = true;
            this.bTestFileFilterRegex.Click += new System.EventHandler(this.bTestFileFilterRegex_Click);
            // 
            // fileFilterRegex
            // 
            this.fileFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileFilterRegex.Location = new System.Drawing.Point(86, 93);
            this.fileFilterRegex.Name = "fileFilterRegex";
            this.fileFilterRegex.Size = new System.Drawing.Size(199, 20);
            this.fileFilterRegex.TabIndex = 36;
            // 
            // lStatus
            // 
            this.lStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lStatus.Location = new System.Drawing.Point(45, 152);
            this.lStatus.Multiline = true;
            this.lStatus.Name = "lStatus";
            this.lStatus.ReadOnly = true;
            this.lStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lStatus.Size = new System.Drawing.Size(292, 39);
            this.lStatus.TabIndex = 35;
            // 
            // lTotalPages
            // 
            this.lTotalPages.AutoSize = true;
            this.lTotalPages.Location = new System.Drawing.Point(187, 5);
            this.lTotalPages.Name = "lTotalPages";
            this.lTotalPages.Size = new System.Drawing.Size(32, 13);
            this.lTotalPages.TabIndex = 33;
            this.lTotalPages.Text = "Page";
            // 
            // selectionCoordinates
            // 
            this.selectionCoordinates.AutoSize = true;
            this.selectionCoordinates.Location = new System.Drawing.Point(123, 129);
            this.selectionCoordinates.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.selectionCoordinates.Name = "selectionCoordinates";
            this.selectionCoordinates.Size = new System.Drawing.Size(25, 13);
            this.selectionCoordinates.TabIndex = 32;
            this.selectionCoordinates.Text = "      ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-2, 130);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Selection Coordinates:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(-1, 159);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 30;
            this.label12.Text = "Status:";
            // 
            // invoiceFirstPageRecognitionMarks
            // 
            this.invoiceFirstPageRecognitionMarks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.invoiceFirstPageRecognitionMarks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.invoiceFirstPageRecognitionMarks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Rectangle2,
            this.FloatingAnchorId2,
            this.ValueType2,
            this.Value2});
            this.invoiceFirstPageRecognitionMarks.Location = new System.Drawing.Point(2, 396);
            this.invoiceFirstPageRecognitionMarks.MultiSelect = false;
            this.invoiceFirstPageRecognitionMarks.Name = "invoiceFirstPageRecognitionMarks";
            this.invoiceFirstPageRecognitionMarks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.invoiceFirstPageRecognitionMarks.Size = new System.Drawing.Size(335, 68);
            this.invoiceFirstPageRecognitionMarks.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 380);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(211, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Invoice First Page Recognition Text Marks:";
            // 
            // bNextPage
            // 
            this.bNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bNextPage.Location = new System.Drawing.Point(291, 0);
            this.bNextPage.Name = "bNextPage";
            this.bNextPage.Size = new System.Drawing.Size(47, 23);
            this.bNextPage.TabIndex = 23;
            this.bNextPage.Text = ">>";
            this.bNextPage.UseVisualStyleBackColor = true;
            this.bNextPage.Click += new System.EventHandler(this.bNextPage_Click);
            // 
            // bPrevPage
            // 
            this.bPrevPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPrevPage.Location = new System.Drawing.Point(238, 0);
            this.bPrevPage.Name = "bPrevPage";
            this.bPrevPage.Size = new System.Drawing.Size(47, 23);
            this.bPrevPage.TabIndex = 22;
            this.bPrevPage.Text = "<<";
            this.bPrevPage.UseVisualStyleBackColor = true;
            this.bPrevPage.Click += new System.EventHandler(this.bPrevPage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Test File:";
            // 
            // testFile
            // 
            this.testFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testFile.Location = new System.Drawing.Point(86, 32);
            this.testFile.Name = "testFile";
            this.testFile.Size = new System.Drawing.Size(222, 20);
            this.testFile.TabIndex = 10;
            // 
            // bTestFile
            // 
            this.bTestFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTestFile.Location = new System.Drawing.Point(314, 29);
            this.bTestFile.Name = "bTestFile";
            this.bTestFile.Size = new System.Drawing.Size(24, 23);
            this.bTestFile.TabIndex = 9;
            this.bTestFile.Text = "...";
            this.bTestFile.UseVisualStyleBackColor = true;
            this.bTestFile.Click += new System.EventHandler(this.bTestFile_Click);
            // 
            // Name_
            // 
            this.Name_.HeaderText = "Name";
            this.Name_.Name = "Name_";
            this.Name_.Width = 60;
            // 
            // FloatingAnchorId
            // 
            this.FloatingAnchorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FloatingAnchorId.HeaderText = "Floating Anchor";
            this.FloatingAnchorId.Name = "FloatingAnchorId";
            this.FloatingAnchorId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FloatingAnchorId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.FloatingAnchorId.Width = 97;
            // 
            // Rectangle
            // 
            this.Rectangle.HeaderText = "Rectangle";
            this.Rectangle.Name = "Rectangle";
            this.Rectangle.Width = 81;
            // 
            // Ocr
            // 
            this.Ocr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Ocr.HeaderText = "OCR";
            this.Ocr.Name = "Ocr";
            this.Ocr.Width = 36;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // Rectangle2
            // 
            this.Rectangle2.HeaderText = "Rectangle";
            this.Rectangle2.Name = "Rectangle2";
            // 
            // FloatingAnchorId2
            // 
            this.FloatingAnchorId2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FloatingAnchorId2.HeaderText = "Floating Anchor";
            this.FloatingAnchorId2.Name = "FloatingAnchorId2";
            this.FloatingAnchorId2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FloatingAnchorId2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.FloatingAnchorId2.Width = 97;
            // 
            // ValueType2
            // 
            this.ValueType2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ValueType2.HeaderText = "Value Type";
            this.ValueType2.Name = "ValueType2";
            this.ValueType2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ValueType2.Width = 60;
            // 
            // Value2
            // 
            this.Value2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value2.HeaderText = "Value";
            this.Value2.Name = "Value2";
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 639);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TemplateForm";
            this.Text = "TemplateForm";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fields)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.floatingAnchors)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.invoiceFirstPageRecognitionMarks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridView fields;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox testFile;
        private System.Windows.Forms.Button bTestFile;
        private System.Windows.Forms.Button bNextPage;
        private System.Windows.Forms.Button bPrevPage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bIsInvoiceFirstPage;
        private System.Windows.Forms.DataGridView invoiceFirstPageRecognitionMarks;
        private System.Windows.Forms.Label selectionCoordinates;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lTotalPages;
        private System.Windows.Forms.TextBox lStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bTestFileFilterRegex;
        private System.Windows.Forms.TextBox fileFilterRegex;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.NumericUpDown pictureScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cSelectAnchor;
        private System.Windows.Forms.Button bText;
        private System.Windows.Forms.TextBox tCurrentPage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox pageRotation;
        private System.Windows.Forms.CheckBox autoDeskew;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView floatingAnchors;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rectangle3;
        private System.Windows.Forms.DataGridViewComboBoxColumn ValueType3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewComboBoxColumn FloatingAnchorId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rectangle;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Ocr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rectangle2;
        private System.Windows.Forms.DataGridViewComboBoxColumn FloatingAnchorId2;
        private System.Windows.Forms.DataGridViewComboBoxColumn ValueType2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value2;
    }
}