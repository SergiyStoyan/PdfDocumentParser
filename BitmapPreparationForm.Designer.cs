namespace Cliver.PdfDocumentParser
{
    partial class BitmapPreparationForm
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
            this.bCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bApply = new System.Windows.Forms.Button();
            this.defaultBitmapPreprocessorClassDefinitions = new System.Windows.Forms.ComboBox();
            this.bRemove = new System.Windows.Forms.Button();
            this.bSaveDafault = new System.Windows.Forms.Button();
            this.bitmapPreprocessorClassDefinition = new ICSharpCode.TextEditor.TextEditorControl();
            this.label9 = new System.Windows.Forms.Label();
            this.DeskewBlockMaxHeight = new System.Windows.Forms.NumericUpDown();
            this.Deskew = new System.Windows.Forms.CheckBox();
            this.PageRotation = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PreprocessBitmap = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ScalingAnchor = new System.Windows.Forms.ComboBox();
            this.DeskewBlockMinSpan = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.TesseractPageSegMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ParseOcrFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CvImageScalePyramidStep = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DeskewBlockMaxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeskewBlockMinSpan)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CvImageScalePyramidStep)).BeginInit();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(764, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 50;
            this.bCancel.Text = "Close";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.bCancel);
            this.flowLayoutPanel1.Controls.Add(this.bApply);
            this.flowLayoutPanel1.Controls.Add(this.defaultBitmapPreprocessorClassDefinitions);
            this.flowLayoutPanel1.Controls.Add(this.bRemove);
            this.flowLayoutPanel1.Controls.Add(this.bSaveDafault);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 316);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(842, 31);
            this.flowLayoutPanel1.TabIndex = 60;
            // 
            // bApply
            // 
            this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bApply.Location = new System.Drawing.Point(683, 3);
            this.bApply.Name = "bApply";
            this.bApply.Size = new System.Drawing.Size(75, 23);
            this.bApply.TabIndex = 51;
            this.bApply.Text = "Apply";
            this.bApply.UseVisualStyleBackColor = true;
            this.bApply.Click += new System.EventHandler(this.bApply_Click);
            // 
            // defaultBitmapPreprocessorClassDefinitions
            // 
            this.defaultBitmapPreprocessorClassDefinitions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defaultBitmapPreprocessorClassDefinitions.FormattingEnabled = true;
            this.defaultBitmapPreprocessorClassDefinitions.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.defaultBitmapPreprocessorClassDefinitions.Location = new System.Drawing.Point(456, 3);
            this.defaultBitmapPreprocessorClassDefinitions.Name = "defaultBitmapPreprocessorClassDefinitions";
            this.defaultBitmapPreprocessorClassDefinitions.Size = new System.Drawing.Size(221, 21);
            this.defaultBitmapPreprocessorClassDefinitions.TabIndex = 85;
            this.defaultBitmapPreprocessorClassDefinitions.SelectedIndexChanged += new System.EventHandler(this.defaults_SelectedIndexChanged);
            // 
            // bRemove
            // 
            this.bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bRemove.Location = new System.Drawing.Point(416, 3);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(34, 23);
            this.bRemove.TabIndex = 52;
            this.bRemove.Text = "-";
            this.bRemove.UseVisualStyleBackColor = true;
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bSaveDafault
            // 
            this.bSaveDafault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSaveDafault.Location = new System.Drawing.Point(376, 3);
            this.bSaveDafault.Name = "bSaveDafault";
            this.bSaveDafault.Size = new System.Drawing.Size(34, 23);
            this.bSaveDafault.TabIndex = 53;
            this.bSaveDafault.Text = "+";
            this.bSaveDafault.UseVisualStyleBackColor = true;
            this.bSaveDafault.Click += new System.EventHandler(this.bSaveDafault_Click);
            // 
            // bitmapPreprocessorClassDefinition
            // 
            this.bitmapPreprocessorClassDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapPreprocessorClassDefinition.AutoHideScrollbars = false;
            this.bitmapPreprocessorClassDefinition.Highlighting = null;
            this.bitmapPreprocessorClassDefinition.Location = new System.Drawing.Point(1, 97);
            this.bitmapPreprocessorClassDefinition.Margin = new System.Windows.Forms.Padding(1);
            this.bitmapPreprocessorClassDefinition.Name = "bitmapPreprocessorClassDefinition";
            this.bitmapPreprocessorClassDefinition.ShowVRuler = false;
            this.bitmapPreprocessorClassDefinition.Size = new System.Drawing.Size(842, 218);
            this.bitmapPreprocessorClassDefinition.TabIndex = 78;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 77;
            this.label9.Text = "Preprocess:";
            // 
            // DeskewBlockMaxHeight
            // 
            this.DeskewBlockMaxHeight.Location = new System.Drawing.Point(115, 37);
            this.DeskewBlockMaxHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.DeskewBlockMaxHeight.Name = "DeskewBlockMaxHeight";
            this.DeskewBlockMaxHeight.Size = new System.Drawing.Size(57, 20);
            this.DeskewBlockMaxHeight.TabIndex = 83;
            this.DeskewBlockMaxHeight.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // Deskew
            // 
            this.Deskew.AutoSize = true;
            this.Deskew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Deskew.Location = new System.Drawing.Point(115, 23);
            this.Deskew.Name = "Deskew";
            this.Deskew.Size = new System.Drawing.Size(15, 14);
            this.Deskew.TabIndex = 81;
            this.Deskew.UseVisualStyleBackColor = true;
            // 
            // PageRotation
            // 
            this.PageRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PageRotation.FormattingEnabled = true;
            this.PageRotation.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.PageRotation.Location = new System.Drawing.Point(12, 26);
            this.PageRotation.Name = "PageRotation";
            this.PageRotation.Size = new System.Drawing.Size(74, 21);
            this.PageRotation.TabIndex = 80;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 43);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 13);
            this.label14.TabIndex = 82;
            this.label14.Text = "Block Max Height:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 79;
            this.label1.Text = "Rotate Pages:";
            // 
            // PreprocessBitmap
            // 
            this.PreprocessBitmap.AutoSize = true;
            this.PreprocessBitmap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.PreprocessBitmap.Location = new System.Drawing.Point(77, 78);
            this.PreprocessBitmap.Name = "PreprocessBitmap";
            this.PreprocessBitmap.Size = new System.Drawing.Size(15, 14);
            this.PreprocessBitmap.TabIndex = 85;
            this.PreprocessBitmap.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "Rescale By Anchor:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "Preprocessing Code:";
            // 
            // ScalingAnchor
            // 
            this.ScalingAnchor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScalingAnchor.FormattingEnabled = true;
            this.ScalingAnchor.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.ScalingAnchor.Location = new System.Drawing.Point(99, 26);
            this.ScalingAnchor.Name = "ScalingAnchor";
            this.ScalingAnchor.Size = new System.Drawing.Size(98, 21);
            this.ScalingAnchor.TabIndex = 89;
            // 
            // DeskewBlockMinSpan
            // 
            this.DeskewBlockMinSpan.Location = new System.Drawing.Point(115, 57);
            this.DeskewBlockMinSpan.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.DeskewBlockMinSpan.Name = "DeskewBlockMinSpan";
            this.DeskewBlockMinSpan.Size = new System.Drawing.Size(57, 20);
            this.DeskewBlockMinSpan.TabIndex = 96;
            this.DeskewBlockMinSpan.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 95;
            this.label4.Text = "Block Min Span:";
            // 
            // TesseractPageSegMode
            // 
            this.TesseractPageSegMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TesseractPageSegMode.FormattingEnabled = true;
            this.TesseractPageSegMode.Location = new System.Drawing.Point(215, 26);
            this.TesseractPageSegMode.Name = "TesseractPageSegMode";
            this.TesseractPageSegMode.Size = new System.Drawing.Size(127, 21);
            this.TesseractPageSegMode.TabIndex = 98;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(212, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 97;
            this.label5.Text = "Tesseract Mode:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.Deskew);
            this.groupBox1.Controls.Add(this.DeskewBlockMaxHeight);
            this.groupBox1.Controls.Add(this.DeskewBlockMinSpan);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(635, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 92);
            this.groupBox1.TabIndex = 99;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Deskew";
            // 
            // ParseOcrFieldFromFieldImage
            // 
            this.ParseOcrFieldFromFieldImage.AutoSize = true;
            this.ParseOcrFieldFromFieldImage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ParseOcrFieldFromFieldImage.Location = new System.Drawing.Point(577, 12);
            this.ParseOcrFieldFromFieldImage.Name = "ParseOcrFieldFromFieldImage";
            this.ParseOcrFieldFromFieldImage.Size = new System.Drawing.Size(15, 14);
            this.ParseOcrFieldFromFieldImage.TabIndex = 100;
            this.ParseOcrFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(13, 23);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(40, 13);
            this.label21.TabIndex = 84;
            this.label21.Text = "Active:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(373, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 13);
            this.label6.TabIndex = 101;
            this.label6.Text = "Parse Ocr Field From Field\'s Image:";
            // 
            // CvImageScalePyramidStep
            // 
            this.CvImageScalePyramidStep.Location = new System.Drawing.Point(549, 29);
            this.CvImageScalePyramidStep.Name = "CvImageScalePyramidStep";
            this.CvImageScalePyramidStep.Size = new System.Drawing.Size(43, 20);
            this.CvImageScalePyramidStep.TabIndex = 103;
            this.CvImageScalePyramidStep.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(373, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 13);
            this.label7.TabIndex = 102;
            this.label7.Text = "CvImage Scale Pyramid Step:";
            // 
            // BitmapPreparationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 347);
            this.Controls.Add(this.CvImageScalePyramidStep);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ParseOcrFieldFromFieldImage);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TesseractPageSegMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ScalingAnchor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PreprocessBitmap);
            this.Controls.Add(this.PageRotation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bitmapPreprocessorClassDefinition);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "BitmapPreparationForm";
            this.Text = "Scanned Document Settings";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DeskewBlockMaxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DeskewBlockMinSpan)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CvImageScalePyramidStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bApply;
        private ICSharpCode.TextEditor.TextEditorControl bitmapPreprocessorClassDefinition;
        private System.Windows.Forms.NumericUpDown DeskewBlockMaxHeight;
        private System.Windows.Forms.CheckBox Deskew;
        private System.Windows.Forms.ComboBox PageRotation;
        private System.Windows.Forms.Button bRemove;
        private System.Windows.Forms.Button bSaveDafault;
        private System.Windows.Forms.ComboBox defaultBitmapPreprocessorClassDefinitions;
        private System.Windows.Forms.CheckBox PreprocessBitmap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ScalingAnchor;
        private System.Windows.Forms.NumericUpDown DeskewBlockMinSpan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox TesseractPageSegMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox ParseOcrFieldFromFieldImage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown CvImageScalePyramidStep;
        private System.Windows.Forms.Label label7;
    }
}