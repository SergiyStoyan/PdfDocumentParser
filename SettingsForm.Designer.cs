namespace Cliver.PdfDocumentParser
{
    partial class SettingsForm
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
            this.PdfPageImageResolution = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.bReset = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.About = new System.Windows.Forms.Button();
            this.TextLineSeparatorWidth = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.TableBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.AscendantAnchorBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.AnchorBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.SelectionBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TableBoxColor = new System.Windows.Forms.Button();
            this.AscendantAnchorBoxColor = new System.Windows.Forms.Button();
            this.SelectionBoxColor = new System.Windows.Forms.Button();
            this.AnchorBoxColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CoordinateDeviationMargin = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.OcrConfig = new System.Windows.Forms.RichTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.InitialSearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextLineSeparatorWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TableBoxBorderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AscendantAnchorBoxBorderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorBoxBorderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBorderWidth)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InitialSearchRectangleMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // PdfPageImageResolution
            // 
            this.PdfPageImageResolution.Location = new System.Drawing.Point(242, 25);
            this.PdfPageImageResolution.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PdfPageImageResolution.Minimum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.PdfPageImageResolution.Name = "PdfPageImageResolution";
            this.PdfPageImageResolution.Size = new System.Drawing.Size(69, 20);
            this.PdfPageImageResolution.TabIndex = 47;
            this.PdfPageImageResolution.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(197, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Image Resolution (OCR requires >=300):";
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(133, 3);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(75, 23);
            this.bReset.TabIndex = 48;
            this.bReset.Text = "Reset";
            this.bReset.UseVisualStyleBackColor = true;
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSave.Location = new System.Drawing.Point(214, 3);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 49;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(295, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 50;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.bCancel);
            this.flowLayoutPanel1.Controls.Add(this.bSave);
            this.flowLayoutPanel1.Controls.Add(this.bReset);
            this.flowLayoutPanel1.Controls.Add(this.About);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 304);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(373, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // About
            // 
            this.About.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.About.Location = new System.Drawing.Point(52, 3);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(75, 23);
            this.About.TabIndex = 51;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // TextLineSeparatorWidth
            // 
            this.TextLineSeparatorWidth.Location = new System.Drawing.Point(242, 204);
            this.TextLineSeparatorWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TextLineSeparatorWidth.Name = "TextLineSeparatorWidth";
            this.TextLineSeparatorWidth.Size = new System.Drawing.Size(42, 20);
            this.TextLineSeparatorWidth.TabIndex = 65;
            this.TextLineSeparatorWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(23, 206);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(134, 13);
            this.label9.TabIndex = 64;
            this.label9.Text = "Text Line Separator Width:";
            // 
            // TableBoxBorderWidth
            // 
            this.TableBoxBorderWidth.Location = new System.Drawing.Point(242, 139);
            this.TableBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TableBoxBorderWidth.Name = "TableBoxBorderWidth";
            this.TableBoxBorderWidth.Size = new System.Drawing.Size(42, 20);
            this.TableBoxBorderWidth.TabIndex = 63;
            this.TableBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(23, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 62;
            this.label6.Text = "Table Box:";
            // 
            // AscendantAnchorBoxBorderWidth
            // 
            this.AscendantAnchorBoxBorderWidth.Location = new System.Drawing.Point(242, 111);
            this.AscendantAnchorBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AscendantAnchorBoxBorderWidth.Name = "AscendantAnchorBoxBorderWidth";
            this.AscendantAnchorBoxBorderWidth.Size = new System.Drawing.Size(42, 20);
            this.AscendantAnchorBoxBorderWidth.TabIndex = 61;
            this.AscendantAnchorBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AnchorBoxBorderWidth
            // 
            this.AnchorBoxBorderWidth.Location = new System.Drawing.Point(242, 83);
            this.AnchorBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AnchorBoxBorderWidth.Name = "AnchorBoxBorderWidth";
            this.AnchorBoxBorderWidth.Size = new System.Drawing.Size(42, 20);
            this.AnchorBoxBorderWidth.TabIndex = 60;
            this.AnchorBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SelectionBoxBorderWidth
            // 
            this.SelectionBoxBorderWidth.Location = new System.Drawing.Point(242, 55);
            this.SelectionBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SelectionBoxBorderWidth.Name = "SelectionBoxBorderWidth";
            this.SelectionBoxBorderWidth.Size = new System.Drawing.Size(42, 20);
            this.SelectionBoxBorderWidth.TabIndex = 59;
            this.SelectionBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(240, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 58;
            this.label5.Text = "Border Width:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(146, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 57;
            this.label7.Text = "Color:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(23, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "Ascendant Anchor Box:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.TableBoxColor);
            this.panel2.Controls.Add(this.AscendantAnchorBoxColor);
            this.panel2.Controls.Add(this.SelectionBoxColor);
            this.panel2.Controls.Add(this.AnchorBoxColor);
            this.panel2.Location = new System.Drawing.Point(149, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(63, 123);
            this.panel2.TabIndex = 54;
            // 
            // TableBoxColor
            // 
            this.TableBoxColor.BackColor = System.Drawing.Color.White;
            this.TableBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TableBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.TableBoxColor.Location = new System.Drawing.Point(6, 92);
            this.TableBoxColor.Name = "TableBoxColor";
            this.TableBoxColor.Size = new System.Drawing.Size(49, 22);
            this.TableBoxColor.TabIndex = 58;
            this.TableBoxColor.Text = "...";
            this.TableBoxColor.UseVisualStyleBackColor = false;
            this.TableBoxColor.Click += new System.EventHandler(this.TableBoxColor_Click);
            // 
            // AscendantAnchorBoxColor
            // 
            this.AscendantAnchorBoxColor.BackColor = System.Drawing.Color.White;
            this.AscendantAnchorBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AscendantAnchorBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.AscendantAnchorBoxColor.Location = new System.Drawing.Point(6, 63);
            this.AscendantAnchorBoxColor.Name = "AscendantAnchorBoxColor";
            this.AscendantAnchorBoxColor.Size = new System.Drawing.Size(49, 22);
            this.AscendantAnchorBoxColor.TabIndex = 57;
            this.AscendantAnchorBoxColor.Text = "...";
            this.AscendantAnchorBoxColor.UseVisualStyleBackColor = false;
            this.AscendantAnchorBoxColor.Click += new System.EventHandler(this.AnchorSecondaryBoxColor_Click);
            // 
            // SelectionBoxColor
            // 
            this.SelectionBoxColor.BackColor = System.Drawing.Color.White;
            this.SelectionBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectionBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.SelectionBoxColor.Location = new System.Drawing.Point(6, 8);
            this.SelectionBoxColor.Name = "SelectionBoxColor";
            this.SelectionBoxColor.Size = new System.Drawing.Size(49, 22);
            this.SelectionBoxColor.TabIndex = 56;
            this.SelectionBoxColor.Text = "...";
            this.SelectionBoxColor.UseVisualStyleBackColor = false;
            this.SelectionBoxColor.Click += new System.EventHandler(this.SelectionBoxColor_Click);
            // 
            // AnchorBoxColor
            // 
            this.AnchorBoxColor.BackColor = System.Drawing.Color.White;
            this.AnchorBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AnchorBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.AnchorBoxColor.Location = new System.Drawing.Point(6, 35);
            this.AnchorBoxColor.Name = "AnchorBoxColor";
            this.AnchorBoxColor.Size = new System.Drawing.Size(49, 22);
            this.AnchorBoxColor.TabIndex = 55;
            this.AnchorBoxColor.Text = "...";
            this.AnchorBoxColor.UseVisualStyleBackColor = false;
            this.AnchorBoxColor.Click += new System.EventHandler(this.AnchorMasterBoxColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(24, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "Master Anchor Box:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(24, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Selection Box:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Coordinate Deviation Margin (px):";
            // 
            // CoordinateDeviationMargin
            // 
            this.CoordinateDeviationMargin.DecimalPlaces = 3;
            this.CoordinateDeviationMargin.Location = new System.Drawing.Point(242, 53);
            this.CoordinateDeviationMargin.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.CoordinateDeviationMargin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.CoordinateDeviationMargin.Name = "CoordinateDeviationMargin";
            this.CoordinateDeviationMargin.Size = new System.Drawing.Size(69, 20);
            this.CoordinateDeviationMargin.TabIndex = 56;
            this.CoordinateDeviationMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(349, 286);
            this.tabControl1.TabIndex = 61;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TextLineSeparatorWidth);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.TableBoxBorderWidth);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.AscendantAnchorBoxBorderWidth);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.AnchorBoxBorderWidth);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.SelectionBoxBorderWidth);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(341, 260);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Appearance";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.OcrConfig);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.CoordinateDeviationMargin);
            this.tabPage2.Controls.Add(this.PdfPageImageResolution);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(341, 260);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Image Processing";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // OcrConfig
            // 
            this.OcrConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OcrConfig.Location = new System.Drawing.Point(27, 100);
            this.OcrConfig.Name = "OcrConfig";
            this.OcrConfig.ReadOnly = true;
            this.OcrConfig.Size = new System.Drawing.Size(287, 139);
            this.OcrConfig.TabIndex = 58;
            this.OcrConfig.Text = "";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 82);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(149, 13);
            this.label11.TabIndex = 57;
            this.label11.Text = "Tesseract Config Set By Host:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.InitialSearchRectangleMargin);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(341, 260);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Anchors";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // InitialSearchRectangleMargin
            // 
            this.InitialSearchRectangleMargin.Location = new System.Drawing.Point(207, 38);
            this.InitialSearchRectangleMargin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.InitialSearchRectangleMargin.Name = "InitialSearchRectangleMargin";
            this.InitialSearchRectangleMargin.Size = new System.Drawing.Size(69, 20);
            this.InitialSearchRectangleMargin.TabIndex = 60;
            this.InitialSearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(158, 13);
            this.label10.TabIndex = 59;
            this.label10.Text = "Initial Search Rectangle Margin:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 335);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TextLineSeparatorWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TableBoxBorderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AscendantAnchorBoxBorderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorBoxBorderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBorderWidth)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InitialSearchRectangleMargin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown PdfPageImageResolution;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown CoordinateDeviationMargin;
        private System.Windows.Forms.Button About;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SelectionBoxColor;
        private System.Windows.Forms.Button AnchorBoxColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AscendantAnchorBoxColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown SelectionBoxBorderWidth;
        private System.Windows.Forms.NumericUpDown AscendantAnchorBoxBorderWidth;
        private System.Windows.Forms.NumericUpDown AnchorBoxBorderWidth;
        private System.Windows.Forms.NumericUpDown TableBoxBorderWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button TableBoxColor;
        private System.Windows.Forms.NumericUpDown TextLineSeparatorWidth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox OcrConfig;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown InitialSearchRectangleMargin;
        private System.Windows.Forms.Label label10;
    }
}