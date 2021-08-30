﻿namespace Cliver.PdfDocumentParser
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
            this.bPrevPage = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.fields = new System.Windows.Forms.DataGridView();
            this.Name_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeftAnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TopAnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.RightAnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.BottomAnchorId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectionCoordinates = new System.Windows.Forms.Label();
            this.bNextPage = new System.Windows.Forms.Button();
            this.lTotalPages = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.ShowPdfText = new System.Windows.Forms.LinkLabel();
            this.label17 = new System.Windows.Forms.Label();
            this.ShowOcrText = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this.ShowAsJson = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceThreshold = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.anchorsContainer = new System.Windows.Forms.SplitContainer();
            this.anchors = new System.Windows.Forms.DataGridView();
            this.Id3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParentAnchorId3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Type3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Position3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.conditions = new System.Windows.Forms.DataGridView();
            this.Name2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.CheckConditionsAutomaticallyWhenPageChanged = new System.Windows.Forms.CheckBox();
            this.newCondition = new System.Windows.Forms.LinkLabel();
            this.deleteCondition = new System.Windows.Forms.LinkLabel();
            this.moveUpCondition = new System.Windows.Forms.LinkLabel();
            this.moveDownCondition = new System.Windows.Forms.LinkLabel();
            this.Configure = new System.Windows.Forms.LinkLabel();
            this.label16 = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.Help = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.About = new System.Windows.Forms.LinkLabel();
            this.tCurrentPage = new System.Windows.Forms.TextBox();
            this.picture = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SingleFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.ColumnFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.TesseractPageSegMode = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.CvImageScalePyramidStep = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.detectedImageScale = new System.Windows.Forms.TextBox();
            this.bScannedDocumentSettings = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textAutoInsertSpaceRepresentative = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.testFile = new System.Windows.Forms.TextBox();
            this.bTestFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureScale = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ExtractFieldsAutomaticallyWhenPageChanged = new System.Windows.Forms.CheckBox();
            this.ShowFieldTextLineSeparators = new System.Windows.Forms.CheckBox();
            this.copy2ClipboardField = new System.Windows.Forms.LinkLabel();
            this.newField = new System.Windows.Forms.LinkLabel();
            this.duplicateField = new System.Windows.Forms.LinkLabel();
            this.deleteField = new System.Windows.Forms.LinkLabel();
            this.moveUpField = new System.Windows.Forms.LinkLabel();
            this.moveDownField = new System.Windows.Forms.LinkLabel();
            this.bSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fields)).BeginInit();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.anchorsContainer)).BeginInit();
            this.anchorsContainer.Panel1.SuspendLayout();
            this.anchorsContainer.Panel2.SuspendLayout();
            this.anchorsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anchors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditions)).BeginInit();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CvImageScalePyramidStep)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bPrevPage
            // 
            this.bPrevPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPrevPage.Location = new System.Drawing.Point(429, 54);
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
            this.name.Size = new System.Drawing.Size(348, 20);
            this.name.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(298, 56);
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
            // fields
            // 
            this.fields.AllowUserToAddRows = false;
            this.fields.AllowUserToDeleteRows = false;
            this.fields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name_,
            this.LeftAnchorId,
            this.TopAnchorId,
            this.RightAnchorId,
            this.BottomAnchorId,
            this.Type,
            this.Value});
            this.fields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fields.Location = new System.Drawing.Point(0, 0);
            this.fields.MultiSelect = false;
            this.fields.Name = "fields";
            this.fields.RowHeadersWidth = 30;
            this.fields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fields.Size = new System.Drawing.Size(314, 232);
            this.fields.TabIndex = 30;
            // 
            // Name_
            // 
            this.Name_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Name_.HeaderText = "Name";
            this.Name_.MinimumWidth = 12;
            this.Name_.Name = "Name_";
            this.Name_.Width = 60;
            // 
            // LeftAnchorId
            // 
            this.LeftAnchorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.LeftAnchorId.HeaderText = "LA";
            this.LeftAnchorId.MinimumWidth = 12;
            this.LeftAnchorId.Name = "LeftAnchorId";
            this.LeftAnchorId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LeftAnchorId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.LeftAnchorId.ToolTipText = "Left Anchor";
            this.LeftAnchorId.Width = 45;
            // 
            // TopAnchorId
            // 
            this.TopAnchorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.TopAnchorId.HeaderText = "TA";
            this.TopAnchorId.MinimumWidth = 12;
            this.TopAnchorId.Name = "TopAnchorId";
            this.TopAnchorId.ToolTipText = "Top Anchor";
            this.TopAnchorId.Width = 27;
            // 
            // RightAnchorId
            // 
            this.RightAnchorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.RightAnchorId.HeaderText = "RA";
            this.RightAnchorId.MinimumWidth = 12;
            this.RightAnchorId.Name = "RightAnchorId";
            this.RightAnchorId.ToolTipText = "Right Anchor";
            this.RightAnchorId.Width = 28;
            // 
            // BottomAnchorId
            // 
            this.BottomAnchorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.BottomAnchorId.HeaderText = "BA";
            this.BottomAnchorId.MinimumWidth = 12;
            this.BottomAnchorId.Name = "BottomAnchorId";
            this.BottomAnchorId.ToolTipText = "Bottom Anchor";
            this.BottomAnchorId.Width = 27;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type.HeaderText = "Type";
            this.Type.MinimumWidth = 12;
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.Width = 37;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.FillWeight = 1000F;
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 12;
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // selectionCoordinates
            // 
            this.selectionCoordinates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionCoordinates.AutoSize = true;
            this.selectionCoordinates.Location = new System.Drawing.Point(410, 5);
            this.selectionCoordinates.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.selectionCoordinates.Name = "selectionCoordinates";
            this.selectionCoordinates.Size = new System.Drawing.Size(119, 13);
            this.selectionCoordinates.TabIndex = 32;
            this.selectionCoordinates.Text = "<selection coordinates>";
            // 
            // bNextPage
            // 
            this.bNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bNextPage.Location = new System.Drawing.Point(485, 54);
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
            this.lTotalPages.Location = new System.Drawing.Point(367, 56);
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
            this.flowLayoutPanel4.Controls.Add(this.label13);
            this.flowLayoutPanel4.Controls.Add(this.ShowAsJson);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(1, 77);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanel4.Size = new System.Drawing.Size(588, 25);
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
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(185, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(9, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "|";
            // 
            // ShowAsJson
            // 
            this.ShowAsJson.AutoSize = true;
            this.ShowAsJson.Location = new System.Drawing.Point(200, 5);
            this.ShowAsJson.Name = "ShowAsJson";
            this.ShowAsJson.Size = new System.Drawing.Size(121, 13);
            this.ShowAsJson.TabIndex = 27;
            this.ShowAsJson.TabStop = true;
            this.ShowAsJson.Text = "Show Template As Json";
            this.ShowAsJson.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // textAutoInsertSpaceThreshold
            // 
            this.textAutoInsertSpaceThreshold.DecimalPlaces = 2;
            this.textAutoInsertSpaceThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.textAutoInsertSpaceThreshold.Location = new System.Drawing.Point(71, 19);
            this.textAutoInsertSpaceThreshold.Name = "textAutoInsertSpaceThreshold";
            this.textAutoInsertSpaceThreshold.Size = new System.Drawing.Size(52, 20);
            this.textAutoInsertSpaceThreshold.TabIndex = 64;
            this.textAutoInsertSpaceThreshold.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 65;
            this.label12.Text = "Threshold:";
            // 
            // anchorsContainer
            // 
            this.anchorsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.anchorsContainer.Location = new System.Drawing.Point(0, 221);
            this.anchorsContainer.Name = "anchorsContainer";
            // 
            // anchorsContainer.Panel1
            // 
            this.anchorsContainer.Panel1.Controls.Add(this.anchors);
            // 
            // anchorsContainer.Panel2
            // 
            this.anchorsContainer.Panel2.Controls.Add(this.splitContainer3);
            this.anchorsContainer.Size = new System.Drawing.Size(532, 175);
            this.anchorsContainer.SplitterDistance = 278;
            this.anchorsContainer.TabIndex = 52;
            // 
            // anchors
            // 
            this.anchors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.anchors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id3,
            this.ParentAnchorId3,
            this.Type3,
            this.Position3});
            this.anchors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.anchors.Location = new System.Drawing.Point(0, 0);
            this.anchors.MultiSelect = false;
            this.anchors.Name = "anchors";
            this.anchors.RowHeadersWidth = 30;
            this.anchors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.anchors.Size = new System.Drawing.Size(278, 175);
            this.anchors.TabIndex = 50;
            // 
            // Id3
            // 
            this.Id3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Id3.HeaderText = "Id";
            this.Id3.MinimumWidth = 12;
            this.Id3.Name = "Id3";
            this.Id3.ReadOnly = true;
            this.Id3.Width = 41;
            // 
            // ParentAnchorId3
            // 
            this.ParentAnchorId3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ParentAnchorId3.HeaderText = "Parent";
            this.ParentAnchorId3.MinimumWidth = 12;
            this.ParentAnchorId3.Name = "ParentAnchorId3";
            this.ParentAnchorId3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ParentAnchorId3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ParentAnchorId3.Width = 63;
            // 
            // Type3
            // 
            this.Type3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type3.HeaderText = "Type";
            this.Type3.MinimumWidth = 12;
            this.Type3.Name = "Type3";
            this.Type3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type3.Width = 39;
            // 
            // Position3
            // 
            this.Position3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Position3.HeaderText = "Position";
            this.Position3.MinimumWidth = 12;
            this.Position3.Name = "Position3";
            this.Position3.ReadOnly = true;
            this.Position3.Width = 69;
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
            this.splitContainer3.Panel2.Controls.Add(this.flowLayoutPanel5);
            this.splitContainer3.Size = new System.Drawing.Size(250, 175);
            this.splitContainer3.SplitterDistance = 109;
            this.splitContainer3.TabIndex = 0;
            // 
            // conditions
            // 
            this.conditions.AllowUserToAddRows = false;
            this.conditions.AllowUserToDeleteRows = false;
            this.conditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conditions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name2,
            this.Value2});
            this.conditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditions.Location = new System.Drawing.Point(0, 17);
            this.conditions.MultiSelect = false;
            this.conditions.Name = "conditions";
            this.conditions.RowHeadersWidth = 30;
            this.conditions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.conditions.Size = new System.Drawing.Size(250, 45);
            this.conditions.TabIndex = 0;
            // 
            // Name2
            // 
            this.Name2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Name2.HeaderText = "Name";
            this.Name2.MinimumWidth = 12;
            this.Name2.Name = "Name2";
            this.Name2.Width = 60;
            // 
            // Value2
            // 
            this.Value2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Value2.HeaderText = "Expression";
            this.Value2.MinimumWidth = 12;
            this.Value2.Name = "Value2";
            this.Value2.Width = 83;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.label11);
            this.flowLayoutPanel5.Controls.Add(this.CheckConditionsAutomaticallyWhenPageChanged);
            this.flowLayoutPanel5.Controls.Add(this.newCondition);
            this.flowLayoutPanel5.Controls.Add(this.deleteCondition);
            this.flowLayoutPanel5.Controls.Add(this.moveUpCondition);
            this.flowLayoutPanel5.Controls.Add(this.moveDownCondition);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(250, 17);
            this.flowLayoutPanel5.TabIndex = 59;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 58;
            this.label11.Text = "Conditions:";
            // 
            // CheckConditionsAutomaticallyWhenPageChanged
            // 
            this.CheckConditionsAutomaticallyWhenPageChanged.AutoSize = true;
            this.CheckConditionsAutomaticallyWhenPageChanged.Location = new System.Drawing.Point(68, 0);
            this.CheckConditionsAutomaticallyWhenPageChanged.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.CheckConditionsAutomaticallyWhenPageChanged.Name = "CheckConditionsAutomaticallyWhenPageChanged";
            this.CheckConditionsAutomaticallyWhenPageChanged.Size = new System.Drawing.Size(163, 17);
            this.CheckConditionsAutomaticallyWhenPageChanged.TabIndex = 57;
            this.CheckConditionsAutomaticallyWhenPageChanged.Text = "Check When Page Changed";
            this.CheckConditionsAutomaticallyWhenPageChanged.UseVisualStyleBackColor = true;
            // 
            // newCondition
            // 
            this.newCondition.AutoSize = true;
            this.newCondition.LinkArea = new System.Windows.Forms.LinkArea(0, 3);
            this.newCondition.Location = new System.Drawing.Point(3, 20);
            this.newCondition.Name = "newCondition";
            this.newCondition.Size = new System.Drawing.Size(29, 13);
            this.newCondition.TabIndex = 73;
            this.newCondition.TabStop = true;
            this.newCondition.Text = "New";
            this.newCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteCondition
            // 
            this.deleteCondition.AutoSize = true;
            this.deleteCondition.Location = new System.Drawing.Point(35, 20);
            this.deleteCondition.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.deleteCondition.Name = "deleteCondition";
            this.deleteCondition.Size = new System.Drawing.Size(38, 13);
            this.deleteCondition.TabIndex = 70;
            this.deleteCondition.TabStop = true;
            this.deleteCondition.Text = "Delete";
            this.deleteCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // moveUpCondition
            // 
            this.moveUpCondition.AutoSize = true;
            this.moveUpCondition.Location = new System.Drawing.Point(76, 20);
            this.moveUpCondition.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.moveUpCondition.Name = "moveUpCondition";
            this.moveUpCondition.Size = new System.Drawing.Size(21, 13);
            this.moveUpCondition.TabIndex = 71;
            this.moveUpCondition.TabStop = true;
            this.moveUpCondition.Text = "Up";
            this.moveUpCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // moveDownCondition
            // 
            this.moveDownCondition.AutoSize = true;
            this.moveDownCondition.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.moveDownCondition.Location = new System.Drawing.Point(100, 20);
            this.moveDownCondition.Margin = new System.Windows.Forms.Padding(0);
            this.moveDownCondition.Name = "moveDownCondition";
            this.moveDownCondition.Size = new System.Drawing.Size(35, 13);
            this.moveDownCondition.TabIndex = 72;
            this.moveDownCondition.TabStop = true;
            this.moveDownCondition.Text = "Down";
            this.moveDownCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(186, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 21;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bOK.Location = new System.Drawing.Point(105, 3);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 20;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.bCancel);
            this.flowLayoutPanel1.Controls.Add(this.bOK);
            this.flowLayoutPanel1.Controls.Add(this.bSave);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(267, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(264, 31);
            this.flowLayoutPanel1.TabIndex = 27;
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.flowLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 252);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 31);
            this.panel1.TabIndex = 29;
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
            this.flowLayoutPanel3.Size = new System.Drawing.Size(377, 42);
            this.flowLayoutPanel3.TabIndex = 28;
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
            // tCurrentPage
            // 
            this.tCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tCurrentPage.Location = new System.Drawing.Point(339, 54);
            this.tCurrentPage.Name = "tCurrentPage";
            this.tCurrentPage.Size = new System.Drawing.Size(26, 20);
            this.tCurrentPage.TabIndex = 47;
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
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.picture);
            this.splitContainer1.Size = new System.Drawing.Size(1075, 703);
            this.splitContainer1.SplitterDistance = 552;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(10, 10);
            this.splitContainer2.MinimumSize = new System.Drawing.Size(429, 629);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add(this.TesseractPageSegMode);
            this.splitContainer2.Panel1.Controls.Add(this.label19);
            this.splitContainer2.Panel1.Controls.Add(this.CvImageScalePyramidStep);
            this.splitContainer2.Panel1.Controls.Add(this.label14);
            this.splitContainer2.Panel1.Controls.Add(this.label9);
            this.splitContainer2.Panel1.Controls.Add(this.detectedImageScale);
            this.splitContainer2.Panel1.Controls.Add(this.bScannedDocumentSettings);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
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
            this.splitContainer2.Panel1.Controls.Add(this.testFile);
            this.splitContainer2.Panel1.Controls.Add(this.bTestFile);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.pictureScale);
            this.splitContainer2.Panel1.Controls.Add(this.label7);
            this.splitContainer2.Panel1.Controls.Add(this.label10);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Size = new System.Drawing.Size(532, 683);
            this.splitContainer2.SplitterDistance = 396;
            this.splitContainer2.TabIndex = 32;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SingleFieldFromFieldImage);
            this.groupBox1.Controls.Add(this.ColumnFieldFromFieldImage);
            this.groupBox1.Location = new System.Drawing.Point(139, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(167, 69);
            this.groupBox1.TabIndex = 108;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Field OCR Parse Mode";
            // 
            // SingleFieldFromFieldImage
            // 
            this.SingleFieldFromFieldImage.AutoSize = true;
            this.SingleFieldFromFieldImage.Location = new System.Drawing.Point(14, 21);
            this.SingleFieldFromFieldImage.Name = "SingleFieldFromFieldImage";
            this.SingleFieldFromFieldImage.Size = new System.Drawing.Size(141, 17);
            this.SingleFieldFromFieldImage.TabIndex = 100;
            this.SingleFieldFromFieldImage.Text = "Single Field By Its Image";
            this.SingleFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // ColumnFieldFromFieldImage
            // 
            this.ColumnFieldFromFieldImage.AutoSize = true;
            this.ColumnFieldFromFieldImage.Location = new System.Drawing.Point(14, 43);
            this.ColumnFieldFromFieldImage.Name = "ColumnFieldFromFieldImage";
            this.ColumnFieldFromFieldImage.Size = new System.Drawing.Size(147, 17);
            this.ColumnFieldFromFieldImage.TabIndex = 104;
            this.ColumnFieldFromFieldImage.Text = "Column Field By Its Image";
            this.ColumnFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // TesseractPageSegMode
            // 
            this.TesseractPageSegMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TesseractPageSegMode.FormattingEnabled = true;
            this.TesseractPageSegMode.Location = new System.Drawing.Point(405, 154);
            this.TesseractPageSegMode.Name = "TesseractPageSegMode";
            this.TesseractPageSegMode.Size = new System.Drawing.Size(79, 21);
            this.TesseractPageSegMode.TabIndex = 107;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(315, 160);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 13);
            this.label19.TabIndex = 106;
            this.label19.Text = "Tesseract Mode:";
            // 
            // CvImageScalePyramidStep
            // 
            this.CvImageScalePyramidStep.Location = new System.Drawing.Point(441, 179);
            this.CvImageScalePyramidStep.Name = "CvImageScalePyramidStep";
            this.CvImageScalePyramidStep.Size = new System.Drawing.Size(43, 20);
            this.CvImageScalePyramidStep.TabIndex = 105;
            this.CvImageScalePyramidStep.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(315, 182);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(117, 13);
            this.label14.TabIndex = 104;
            this.label14.Text = "CvImage Pyramid Step:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(354, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 13);
            this.label9.TabIndex = 70;
            this.label9.Text = "Detected Image Scale:";
            // 
            // detectedImageScale
            // 
            this.detectedImageScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.detectedImageScale.Location = new System.Drawing.Point(473, 108);
            this.detectedImageScale.Name = "detectedImageScale";
            this.detectedImageScale.ReadOnly = true;
            this.detectedImageScale.Size = new System.Drawing.Size(59, 20);
            this.detectedImageScale.TabIndex = 71;
            // 
            // bScannedDocumentSettings
            // 
            this.bScannedDocumentSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bScannedDocumentSettings.Location = new System.Drawing.Point(0, 106);
            this.bScannedDocumentSettings.Name = "bScannedDocumentSettings";
            this.bScannedDocumentSettings.Size = new System.Drawing.Size(345, 23);
            this.bScannedDocumentSettings.TabIndex = 69;
            this.bScannedDocumentSettings.Text = "Scanned Document Settings";
            this.bScannedDocumentSettings.UseVisualStyleBackColor = true;
            this.bScannedDocumentSettings.Click += new System.EventHandler(this.bScannedDocumentSettings_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textAutoInsertSpaceRepresentative);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textAutoInsertSpaceThreshold);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Location = new System.Drawing.Point(0, 138);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 69);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Auto-Inserting Space";
            // 
            // textAutoInsertSpaceRepresentative
            // 
            this.textAutoInsertSpaceRepresentative.Location = new System.Drawing.Point(71, 41);
            this.textAutoInsertSpaceRepresentative.Name = "textAutoInsertSpaceRepresentative";
            this.textAutoInsertSpaceRepresentative.Size = new System.Drawing.Size(52, 20);
            this.textAutoInsertSpaceRepresentative.TabIndex = 67;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(10, 44);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 66;
            this.label15.Text = "Substitute:";
            // 
            // testFile
            // 
            this.testFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testFile.Location = new System.Drawing.Point(51, 28);
            this.testFile.Name = "testFile";
            this.testFile.Size = new System.Drawing.Size(448, 20);
            this.testFile.TabIndex = 10;
            this.testFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // bTestFile
            // 
            this.bTestFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTestFile.Location = new System.Drawing.Point(508, 25);
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
            this.label10.Location = new System.Drawing.Point(-2, 206);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Anchors:";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 20);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.fields);
            this.splitContainer4.Size = new System.Drawing.Size(532, 232);
            this.splitContainer4.SplitterDistance = 314;
            this.splitContainer4.TabIndex = 60;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.ExtractFieldsAutomaticallyWhenPageChanged);
            this.flowLayoutPanel2.Controls.Add(this.ShowFieldTextLineSeparators);
            this.flowLayoutPanel2.Controls.Add(this.copy2ClipboardField);
            this.flowLayoutPanel2.Controls.Add(this.newField);
            this.flowLayoutPanel2.Controls.Add(this.duplicateField);
            this.flowLayoutPanel2.Controls.Add(this.deleteField);
            this.flowLayoutPanel2.Controls.Add(this.moveUpField);
            this.flowLayoutPanel2.Controls.Add(this.moveDownField);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(1);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(532, 20);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Fields:";
            // 
            // ExtractFieldsAutomaticallyWhenPageChanged
            // 
            this.ExtractFieldsAutomaticallyWhenPageChanged.AutoSize = true;
            this.ExtractFieldsAutomaticallyWhenPageChanged.Location = new System.Drawing.Point(46, 3);
            this.ExtractFieldsAutomaticallyWhenPageChanged.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ExtractFieldsAutomaticallyWhenPageChanged.Name = "ExtractFieldsAutomaticallyWhenPageChanged";
            this.ExtractFieldsAutomaticallyWhenPageChanged.Size = new System.Drawing.Size(165, 17);
            this.ExtractFieldsAutomaticallyWhenPageChanged.TabIndex = 56;
            this.ExtractFieldsAutomaticallyWhenPageChanged.Text = "Extract When Page Changed";
            this.ExtractFieldsAutomaticallyWhenPageChanged.UseVisualStyleBackColor = true;
            // 
            // ShowFieldTextLineSeparators
            // 
            this.ShowFieldTextLineSeparators.AutoSize = true;
            this.ShowFieldTextLineSeparators.Location = new System.Drawing.Point(217, 3);
            this.ShowFieldTextLineSeparators.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ShowFieldTextLineSeparators.Name = "ShowFieldTextLineSeparators";
            this.ShowFieldTextLineSeparators.Size = new System.Drawing.Size(111, 17);
            this.ShowFieldTextLineSeparators.TabIndex = 60;
            this.ShowFieldTextLineSeparators.Text = "Show Table Lines";
            this.ShowFieldTextLineSeparators.UseVisualStyleBackColor = true;
            // 
            // copy2ClipboardField
            // 
            this.copy2ClipboardField.AutoSize = true;
            this.copy2ClipboardField.Location = new System.Drawing.Point(331, 3);
            this.copy2ClipboardField.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.copy2ClipboardField.Name = "copy2ClipboardField";
            this.copy2ClipboardField.Size = new System.Drawing.Size(124, 13);
            this.copy2ClipboardField.TabIndex = 32;
            this.copy2ClipboardField.TabStop = true;
            this.copy2ClipboardField.Text = "Copy Value To Clipboard";
            this.copy2ClipboardField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // newField
            // 
            this.newField.AutoSize = true;
            this.newField.LinkArea = new System.Windows.Forms.LinkArea(0, 3);
            this.newField.Location = new System.Drawing.Point(461, 3);
            this.newField.Name = "newField";
            this.newField.Size = new System.Drawing.Size(29, 13);
            this.newField.TabIndex = 68;
            this.newField.TabStop = true;
            this.newField.Text = "New";
            this.newField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // duplicateField
            // 
            this.duplicateField.AutoSize = true;
            this.duplicateField.Location = new System.Drawing.Point(0, 23);
            this.duplicateField.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.duplicateField.Name = "duplicateField";
            this.duplicateField.Size = new System.Drawing.Size(52, 13);
            this.duplicateField.TabIndex = 28;
            this.duplicateField.TabStop = true;
            this.duplicateField.Text = "Duplicate";
            this.duplicateField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteField
            // 
            this.deleteField.AutoSize = true;
            this.deleteField.Location = new System.Drawing.Point(55, 23);
            this.deleteField.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.deleteField.Name = "deleteField";
            this.deleteField.Size = new System.Drawing.Size(38, 13);
            this.deleteField.TabIndex = 29;
            this.deleteField.TabStop = true;
            this.deleteField.Text = "Delete";
            this.deleteField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // moveUpField
            // 
            this.moveUpField.AutoSize = true;
            this.moveUpField.Location = new System.Drawing.Point(96, 23);
            this.moveUpField.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.moveUpField.Name = "moveUpField";
            this.moveUpField.Size = new System.Drawing.Size(21, 13);
            this.moveUpField.TabIndex = 30;
            this.moveUpField.TabStop = true;
            this.moveUpField.Text = "Up";
            this.moveUpField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // moveDownField
            // 
            this.moveDownField.AutoSize = true;
            this.moveDownField.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.moveDownField.Location = new System.Drawing.Point(120, 23);
            this.moveDownField.Margin = new System.Windows.Forms.Padding(0);
            this.moveDownField.Name = "moveDownField";
            this.moveDownField.Size = new System.Drawing.Size(35, 13);
            this.moveDownField.TabIndex = 31;
            this.moveDownField.TabStop = true;
            this.moveDownField.Text = "Down";
            this.moveDownField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSave.Location = new System.Drawing.Point(24, 3);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 22;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 703);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TemplateForm";
            this.Text = "TemplateForm";
            ((System.ComponentModel.ISupportInitialize)(this.fields)).EndInit();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).EndInit();
            this.anchorsContainer.Panel1.ResumeLayout(false);
            this.anchorsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anchorsContainer)).EndInit();
            this.anchorsContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anchors)).EndInit();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.conditions)).EndInit();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CvImageScalePyramidStep)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScale)).EndInit();
            this.splitContainer4.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bPrevPage;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView fields;
        private System.Windows.Forms.Label selectionCoordinates;
        private System.Windows.Forms.Button bNextPage;
        private System.Windows.Forms.Label lTotalPages;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.LinkLabel ShowPdfText;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.LinkLabel ShowOcrText;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel ShowAsJson;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown textAutoInsertSpaceThreshold;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.SplitContainer anchorsContainer;
        private System.Windows.Forms.DataGridView anchors;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.DataGridView conditions;
        private System.Windows.Forms.CheckBox CheckConditionsAutomaticallyWhenPageChanged;
        private System.Windows.Forms.LinkLabel Configure;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.LinkLabel Help;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel About;
        private System.Windows.Forms.TextBox tCurrentPage;
        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox testFile;
        private System.Windows.Forms.Button bTestFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown pictureScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox ExtractFieldsAutomaticallyWhenPageChanged;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.LinkLabel duplicateField;
        private System.Windows.Forms.LinkLabel deleteField;
        private System.Windows.Forms.LinkLabel moveUpField;
        private System.Windows.Forms.LinkLabel moveDownField;
        private System.Windows.Forms.LinkLabel copy2ClipboardField;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textAutoInsertSpaceRepresentative;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id3;
        private System.Windows.Forms.DataGridViewComboBoxColumn ParentAnchorId3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position3;
        private System.Windows.Forms.CheckBox ShowFieldTextLineSeparators;
        private System.Windows.Forms.Button bScannedDocumentSettings;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox detectedImageScale;
        private System.Windows.Forms.NumericUpDown CvImageScalePyramidStep;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox TesseractPageSegMode;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox SingleFieldFromFieldImage;
        private System.Windows.Forms.CheckBox ColumnFieldFromFieldImage;
        private System.Windows.Forms.LinkLabel newField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel newCondition;
        private System.Windows.Forms.LinkLabel deleteCondition;
        private System.Windows.Forms.LinkLabel moveUpCondition;
        private System.Windows.Forms.LinkLabel moveDownCondition;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value2;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name_;
        private System.Windows.Forms.DataGridViewComboBoxColumn LeftAnchorId;
        private System.Windows.Forms.DataGridViewComboBoxColumn TopAnchorId;
        private System.Windows.Forms.DataGridViewComboBoxColumn RightAnchorId;
        private System.Windows.Forms.DataGridViewComboBoxColumn BottomAnchorId;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button bSave;
    }
}