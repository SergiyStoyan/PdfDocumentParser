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
            this.picture = new System.Windows.Forms.PictureBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            this.BrightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.DifferentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            this.PositionDeviationIsAbsolute = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PositionDeviation = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.cSearchRectangleMargin = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // picture
            // 
            this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picture.Location = new System.Drawing.Point(0, 122);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(218, 92);
            this.picture.TabIndex = 67;
            this.picture.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(119, 13);
            this.label20.TabIndex = 66;
            this.label20.Text = "Find Best Image Match:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 40);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 61;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FindBestImageMatch.Location = new System.Drawing.Point(168, 0);
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
            this.BrightnessTolerance.Location = new System.Drawing.Point(168, 15);
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
            this.BrightnessTolerance.Size = new System.Drawing.Size(47, 20);
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
            this.DifferentPixelNumberTolerance.Location = new System.Drawing.Point(168, 37);
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
            this.DifferentPixelNumberTolerance.Size = new System.Drawing.Size(47, 20);
            this.DifferentPixelNumberTolerance.TabIndex = 64;
            this.DifferentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // PositionDeviationIsAbsolute
            // 
            this.PositionDeviationIsAbsolute.AutoSize = true;
            this.PositionDeviationIsAbsolute.Enabled = false;
            this.PositionDeviationIsAbsolute.Location = new System.Drawing.Point(168, 81);
            this.PositionDeviationIsAbsolute.Name = "PositionDeviationIsAbsolute";
            this.PositionDeviationIsAbsolute.Size = new System.Drawing.Size(15, 14);
            this.PositionDeviationIsAbsolute.TabIndex = 68;
            this.PositionDeviationIsAbsolute.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Position Deviation:";
            // 
            // PositionDeviation
            // 
            this.PositionDeviation.DecimalPlaces = 1;
            this.PositionDeviation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionDeviation.Location = new System.Drawing.Point(168, 59);
            this.PositionDeviation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PositionDeviation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionDeviation.Name = "PositionDeviation";
            this.PositionDeviation.Size = new System.Drawing.Size(47, 20);
            this.PositionDeviation.TabIndex = 70;
            this.PositionDeviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(3, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 71;
            this.label2.Text = "Position Deviation Is Absolute:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 72;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(168, 96);
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
            this.SearchRectangleMargin.Size = new System.Drawing.Size(47, 20);
            this.SearchRectangleMargin.TabIndex = 73;
            this.SearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(151, 99);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(15, 14);
            this.cSearchRectangleMargin.TabIndex = 74;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // AnchorImageDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PositionDeviation);
            this.Controls.Add(this.PositionDeviationIsAbsolute);
            this.Controls.Add(this.picture);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FindBestImageMatch);
            this.Controls.Add(this.BrightnessTolerance);
            this.Controls.Add(this.DifferentPixelNumberTolerance);
            this.Name = "AnchorImageDataControl";
            this.Size = new System.Drawing.Size(218, 214);
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).EndInit();
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
        public System.Windows.Forms.CheckBox PositionDeviationIsAbsolute;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown PositionDeviation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown SearchRectangleMargin;
        private System.Windows.Forms.CheckBox cSearchRectangleMargin;
    }
}
