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
            this.findBestImageMatch = new System.Windows.Forms.CheckBox();
            this.brightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.differentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.differentPixelNumberTolerance)).BeginInit();
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
            // findBestImageMatch
            // 
            this.findBestImageMatch.AutoSize = true;
            this.findBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.findBestImageMatch.Location = new System.Drawing.Point(171, 4);
            this.findBestImageMatch.Name = "findBestImageMatch";
            this.findBestImageMatch.Size = new System.Drawing.Size(15, 14);
            this.findBestImageMatch.TabIndex = 65;
            this.findBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // brightnessTolerance
            // 
            this.brightnessTolerance.DecimalPlaces = 2;
            this.brightnessTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.brightnessTolerance.Location = new System.Drawing.Point(171, 26);
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
            this.brightnessTolerance.Size = new System.Drawing.Size(42, 20);
            this.brightnessTolerance.TabIndex = 62;
            this.brightnessTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // differentPixelNumberTolerance
            // 
            this.differentPixelNumberTolerance.DecimalPlaces = 2;
            this.differentPixelNumberTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.differentPixelNumberTolerance.Location = new System.Drawing.Point(171, 52);
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
            this.differentPixelNumberTolerance.Size = new System.Drawing.Size(42, 20);
            this.differentPixelNumberTolerance.TabIndex = 64;
            this.differentPixelNumberTolerance.Value = new decimal(new int[] {
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
            this.Controls.Add(this.findBestImageMatch);
            this.Controls.Add(this.brightnessTolerance);
            this.Controls.Add(this.differentPixelNumberTolerance);
            this.Name = "FloatingAnchorImageDataControl";
            this.Size = new System.Drawing.Size(218, 186);
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.differentPixelNumberTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox findBestImageMatch;
        private System.Windows.Forms.NumericUpDown brightnessTolerance;
        private System.Windows.Forms.NumericUpDown differentPixelNumberTolerance;
    }
}
