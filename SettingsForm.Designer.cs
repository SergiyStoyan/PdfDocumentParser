namespace Cliver.InvoiceParser
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
            this.imageResolution = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.bReset = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bResetTemplates = new System.Windows.Forms.Button();
            this.ignoreHidddenFiles = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.findBestImageMatch = new System.Windows.Forms.CheckBox();
            this.differentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.brightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.testPictureScale = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageResolution)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.differentPixelNumberTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testPictureScale)).BeginInit();
            this.SuspendLayout();
            // 
            // imageResolution
            // 
            this.imageResolution.Location = new System.Drawing.Point(245, 51);
            this.imageResolution.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.imageResolution.Minimum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.imageResolution.Name = "imageResolution";
            this.imageResolution.Size = new System.Drawing.Size(69, 20);
            this.imageResolution.TabIndex = 47;
            this.imageResolution.Value = new decimal(new int[] {
            72,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(162, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Resolution (OCR requires>=300):";
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(107, 3);
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
            this.bSave.Location = new System.Drawing.Point(188, 3);
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
            this.bCancel.Location = new System.Drawing.Point(269, 3);
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
            this.flowLayoutPanel1.Controls.Add(this.bResetTemplates);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 258);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(347, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // bResetTemplates
            // 
            this.bResetTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bResetTemplates.AutoSize = true;
            this.bResetTemplates.Location = new System.Drawing.Point(4, 3);
            this.bResetTemplates.Name = "bResetTemplates";
            this.bResetTemplates.Size = new System.Drawing.Size(97, 23);
            this.bResetTemplates.TabIndex = 51;
            this.bResetTemplates.Text = "Reset Templates";
            this.bResetTemplates.UseVisualStyleBackColor = true;
            this.bResetTemplates.Click += new System.EventHandler(this.bResetTemplates_Click);
            // 
            // ignoreHidddenFiles
            // 
            this.ignoreHidddenFiles.AutoSize = true;
            this.ignoreHidddenFiles.Location = new System.Drawing.Point(30, 21);
            this.ignoreHidddenFiles.Name = "ignoreHidddenFiles";
            this.ignoreHidddenFiles.Size = new System.Drawing.Size(123, 17);
            this.ignoreHidddenFiles.TabIndex = 53;
            this.ignoreHidddenFiles.Text = "Ignore Hiddden Files";
            this.ignoreHidddenFiles.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.findBestImageMatch);
            this.groupBox1.Controls.Add(this.differentPixelNumberTolerance);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.brightnessTolerance);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(12, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 127);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scanned Document Settings";
            // 
            // findBestImageMatch
            // 
            this.findBestImageMatch.AutoSize = true;
            this.findBestImageMatch.Location = new System.Drawing.Point(18, 27);
            this.findBestImageMatch.Name = "findBestImageMatch";
            this.findBestImageMatch.Size = new System.Drawing.Size(135, 17);
            this.findBestImageMatch.TabIndex = 58;
            this.findBestImageMatch.Text = "Find Best Image Match";
            this.findBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // differentPixelNumberTolerance
            // 
            this.differentPixelNumberTolerance.DecimalPlaces = 2;
            this.differentPixelNumberTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.differentPixelNumberTolerance.Location = new System.Drawing.Point(233, 89);
            this.differentPixelNumberTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.differentPixelNumberTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.differentPixelNumberTolerance.Name = "differentPixelNumberTolerance";
            this.differentPixelNumberTolerance.Size = new System.Drawing.Size(69, 20);
            this.differentPixelNumberTolerance.TabIndex = 57;
            this.differentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 91);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 56;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // brightnessTolerance
            // 
            this.brightnessTolerance.DecimalPlaces = 2;
            this.brightnessTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.brightnessTolerance.Location = new System.Drawing.Point(233, 63);
            this.brightnessTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.brightnessTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.brightnessTolerance.Name = "brightnessTolerance";
            this.brightnessTolerance.Size = new System.Drawing.Size(69, 20);
            this.brightnessTolerance.TabIndex = 55;
            this.brightnessTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 54;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // testPictureScale
            // 
            this.testPictureScale.DecimalPlaces = 1;
            this.testPictureScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.testPictureScale.Location = new System.Drawing.Point(245, 77);
            this.testPictureScale.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.testPictureScale.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.testPictureScale.Name = "testPictureScale";
            this.testPictureScale.Size = new System.Drawing.Size(69, 20);
            this.testPictureScale.TabIndex = 56;
            this.testPictureScale.Value = new decimal(new int[] {
            13,
            0,
            0,
            65536});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Test Picture Scale";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 289);
            this.Controls.Add(this.testPictureScale);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ignoreHidddenFiles);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.imageResolution);
            this.Controls.Add(this.label8);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.imageResolution)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.differentPixelNumberTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testPictureScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown imageResolution;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button bResetTemplates;
        private System.Windows.Forms.CheckBox ignoreHidddenFiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox findBestImageMatch;
        private System.Windows.Forms.NumericUpDown differentPixelNumberTolerance;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown brightnessTolerance;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown testPictureScale;
        private System.Windows.Forms.Label label1;
    }
}