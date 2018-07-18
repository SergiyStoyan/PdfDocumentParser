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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AutoDeskewThreshold = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            this.DifferentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.BrightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.TestPictureScale = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CoordinateDeviationMargin = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoDeskewThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TestPictureScale)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // PdfPageImageResolution
            // 
            this.PdfPageImageResolution.Location = new System.Drawing.Point(234, 33);
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
            this.label8.Location = new System.Drawing.Point(16, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(194, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Image Resolution (OCR requires>=300):";
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(113, 3);
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
            this.bSave.Location = new System.Drawing.Point(194, 3);
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
            this.bCancel.Location = new System.Drawing.Point(275, 3);
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
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 353);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(353, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AutoDeskewThreshold);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.FindBestImageMatch);
            this.groupBox1.Controls.Add(this.DifferentPixelNumberTolerance);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.BrightnessTolerance);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(13, 171);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 176);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image Comparison";
            // 
            // AutoDeskewThreshold
            // 
            this.AutoDeskewThreshold.Location = new System.Drawing.Point(234, 66);
            this.AutoDeskewThreshold.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.AutoDeskewThreshold.Name = "AutoDeskewThreshold";
            this.AutoDeskewThreshold.Size = new System.Drawing.Size(69, 20);
            this.AutoDeskewThreshold.TabIndex = 60;
            this.AutoDeskewThreshold.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 59;
            this.label3.Text = "Auto-Deskew Threshold:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.Location = new System.Drawing.Point(16, 35);
            this.FindBestImageMatch.Name = "FindBestImageMatch";
            this.FindBestImageMatch.Size = new System.Drawing.Size(135, 17);
            this.FindBestImageMatch.TabIndex = 58;
            this.FindBestImageMatch.Text = "Find Best Image Match";
            this.FindBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // DifferentPixelNumberTolerance
            // 
            this.DifferentPixelNumberTolerance.DecimalPlaces = 2;
            this.DifferentPixelNumberTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DifferentPixelNumberTolerance.Location = new System.Drawing.Point(234, 120);
            this.DifferentPixelNumberTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DifferentPixelNumberTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DifferentPixelNumberTolerance.Name = "DifferentPixelNumberTolerance";
            this.DifferentPixelNumberTolerance.Size = new System.Drawing.Size(69, 20);
            this.DifferentPixelNumberTolerance.TabIndex = 57;
            this.DifferentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 56;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // BrightnessTolerance
            // 
            this.BrightnessTolerance.DecimalPlaces = 2;
            this.BrightnessTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessTolerance.Location = new System.Drawing.Point(234, 92);
            this.BrightnessTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BrightnessTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessTolerance.Name = "BrightnessTolerance";
            this.BrightnessTolerance.Size = new System.Drawing.Size(69, 20);
            this.BrightnessTolerance.TabIndex = 55;
            this.BrightnessTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 54;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // TestPictureScale
            // 
            this.TestPictureScale.DecimalPlaces = 1;
            this.TestPictureScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TestPictureScale.Location = new System.Drawing.Point(234, 19);
            this.TestPictureScale.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.TestPictureScale.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.TestPictureScale.Name = "TestPictureScale";
            this.TestPictureScale.Size = new System.Drawing.Size(69, 20);
            this.TestPictureScale.TabIndex = 56;
            this.TestPictureScale.Value = new decimal(new int[] {
            13,
            0,
            0,
            65536});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Test Picture Scale";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.TestPictureScale);
            this.groupBox3.Location = new System.Drawing.Point(13, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(328, 50);
            this.groupBox3.TabIndex = 59;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appearance";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.CoordinateDeviationMargin);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.PdfPageImageResolution);
            this.groupBox4.Location = new System.Drawing.Point(13, 68);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(328, 97);
            this.groupBox4.TabIndex = 60;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Common";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "CoordinateDeviationMargin:";
            // 
            // CoordinateDeviationMargin
            // 
            this.CoordinateDeviationMargin.DecimalPlaces = 3;
            this.CoordinateDeviationMargin.Location = new System.Drawing.Point(234, 61);
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
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 384);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AutoDeskewThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TestPictureScale)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown PdfPageImageResolution;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox FindBestImageMatch;
        private System.Windows.Forms.NumericUpDown DifferentPixelNumberTolerance;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown BrightnessTolerance;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown TestPictureScale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown CoordinateDeviationMargin;
        private System.Windows.Forms.NumericUpDown AutoDeskewThreshold;
        private System.Windows.Forms.Label label3;
    }
}