namespace Cliver.PdfDocumentParser
{
    partial class AnchorImageDataControl
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
            this.label20 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            this.BrightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.DifferentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cSearchRectangleMargin = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 2);
            this.label20.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(312, 32);
            this.label20.TabIndex = 66;
            this.label20.Text = "Find Best Image Match:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 100);
            this.label13.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(435, 32);
            this.label13.TabIndex = 63;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 51);
            this.label11.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(292, 32);
            this.label11.TabIndex = 61;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FindBestImageMatch.Location = new System.Drawing.Point(464, 2);
            this.FindBestImageMatch.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.FindBestImageMatch.Name = "FindBestImageMatch";
            this.FindBestImageMatch.Size = new System.Drawing.Size(34, 33);
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
            this.BrightnessTolerance.Location = new System.Drawing.Point(467, 47);
            this.BrightnessTolerance.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.BrightnessTolerance.Size = new System.Drawing.Size(125, 38);
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
            this.DifferentPixelNumberTolerance.Location = new System.Drawing.Point(467, 97);
            this.DifferentPixelNumberTolerance.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.DifferentPixelNumberTolerance.Size = new System.Drawing.Size(125, 38);
            this.DifferentPixelNumberTolerance.TabIndex = 64;
            this.DifferentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 198);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 32);
            this.label4.TabIndex = 76;
            this.label4.Text = "Pattern:";
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(403, 153);
            this.cSearchRectangleMargin.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(34, 33);
            this.cSearchRectangleMargin.TabIndex = 97;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 149);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 32);
            this.label3.TabIndex = 95;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(467, 147);
            this.SearchRectangleMargin.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.SearchRectangleMargin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SearchRectangleMargin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.SearchRectangleMargin.Name = "SearchRectangleMargin";
            this.SearchRectangleMargin.Size = new System.Drawing.Size(125, 38);
            this.SearchRectangleMargin.TabIndex = 96;
            this.SearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(3, 236);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(76, 65);
            this.pictureBox.TabIndex = 98;
            this.pictureBox.TabStop = false;
            // 
            // AnchorImageDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FindBestImageMatch);
            this.Controls.Add(this.BrightnessTolerance);
            this.Controls.Add(this.DifferentPixelNumberTolerance);
            this.Margin = new System.Windows.Forms.Padding(21, 17, 21, 17);
            this.MinimumSize = new System.Drawing.Size(213, 172);
            this.Name = "AnchorImageDataControl";
            this.Size = new System.Drawing.Size(603, 424);
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.CheckBox FindBestImageMatch;
        public System.Windows.Forms.NumericUpDown BrightnessTolerance;
        public System.Windows.Forms.NumericUpDown DifferentPixelNumberTolerance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cSearchRectangleMargin;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown SearchRectangleMargin;
        private System.Windows.Forms.PictureBox pictureBox;
    }
}
