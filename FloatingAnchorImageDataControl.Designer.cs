namespace Cliver.PdfDocumentParser
{
    partial class FloatingAnchorImageDataControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picture = new System.Windows.Forms.PictureBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            this.BrightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.DifferentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // picture
            // 
            this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picture.Location = new System.Drawing.Point(0, 78);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(218, 108);
            this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picture.TabIndex = 67;
            this.picture.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 4);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(119, 13);
            this.label20.TabIndex = 66;
            this.label20.Text = "Find Best Image Match:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 54);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 30);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 61;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FindBestImageMatch.Location = new System.Drawing.Point(171, 4);
            this.FindBestImageMatch.Name = "FindBestImageMatch";
            this.FindBestImageMatch.Size = new System.Drawing.Size(15, 14);
            this.FindBestImageMatch.TabIndex = 65;
            this.FindBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // BrightnessTolerance
            // 
            this.BrightnessTolerance.DecimalPlaces = 2;
            this.BrightnessTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessTolerance.Location = new System.Drawing.Point(171, 26);
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
            this.BrightnessTolerance.Size = new System.Drawing.Size(42, 20);
            this.BrightnessTolerance.TabIndex = 62;
            this.BrightnessTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // DifferentPixelNumberTolerance
            // 
            this.DifferentPixelNumberTolerance.DecimalPlaces = 2;
            this.DifferentPixelNumberTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DifferentPixelNumberTolerance.Location = new System.Drawing.Point(171, 52);
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
            this.DifferentPixelNumberTolerance.Size = new System.Drawing.Size(42, 20);
            this.DifferentPixelNumberTolerance.TabIndex = 64;
            this.DifferentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FloatingAnchorImageDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picture);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FindBestImageMatch);
            this.Controls.Add(this.BrightnessTolerance);
            this.Controls.Add(this.DifferentPixelNumberTolerance);
            this.Name = "FloatingAnchorImageDataControl";
            this.Size = new System.Drawing.Size(218, 186);
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.CheckBox FindBestImageMatch;
        public System.Windows.Forms.NumericUpDown BrightnessTolerance;
        public System.Windows.Forms.NumericUpDown DifferentPixelNumberTolerance;
    }
}
