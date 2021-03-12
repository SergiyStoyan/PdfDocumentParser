namespace Cliver.PdfDocumentParser
{
    partial class AnchorCvImageControl
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
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Threshold = new System.Windows.Forms.NumericUpDown();
            this.ScaleDeviation = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cSearchRectangleMargin = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label20 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleDeviation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "ScaleDeviation:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 61;
            this.label11.Text = "Threshold:";
            // 
            // Threshold
            // 
            this.Threshold.DecimalPlaces = 2;
            this.Threshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Threshold.Location = new System.Drawing.Point(175, 19);
            this.Threshold.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Threshold.Name = "Threshold";
            this.Threshold.Size = new System.Drawing.Size(47, 20);
            this.Threshold.TabIndex = 62;
            this.Threshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ScaleDeviation
            // 
            this.ScaleDeviation.DecimalPlaces = 2;
            this.ScaleDeviation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ScaleDeviation.Location = new System.Drawing.Point(175, 40);
            this.ScaleDeviation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ScaleDeviation.Name = "ScaleDeviation";
            this.ScaleDeviation.Size = new System.Drawing.Size(47, 20);
            this.ScaleDeviation.TabIndex = 64;
            this.ScaleDeviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 76;
            this.label4.Text = "Pattern:";
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(151, 64);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(15, 14);
            this.cSearchRectangleMargin.TabIndex = 97;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 95;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(175, 61);
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
            this.SearchRectangleMargin.TabIndex = 96;
            this.SearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(1, 99);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(28, 27);
            this.pictureBox.TabIndex = 98;
            this.pictureBox.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(119, 13);
            this.label20.TabIndex = 100;
            this.label20.Text = "Find Best Image Match:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FindBestImageMatch.Location = new System.Drawing.Point(174, 0);
            this.FindBestImageMatch.Name = "FindBestImageMatch";
            this.FindBestImageMatch.Size = new System.Drawing.Size(15, 14);
            this.FindBestImageMatch.TabIndex = 99;
            this.FindBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // AnchorCvImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label20);
            this.Controls.Add(this.FindBestImageMatch);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Threshold);
            this.Controls.Add(this.ScaleDeviation);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(80, 72);
            this.Name = "AnchorCvImageControl";
            this.Size = new System.Drawing.Size(600, 263);
            ((System.ComponentModel.ISupportInitialize)(this.Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleDeviation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.NumericUpDown Threshold;
        public System.Windows.Forms.NumericUpDown ScaleDeviation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cSearchRectangleMargin;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown SearchRectangleMargin;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label20;
        public System.Windows.Forms.CheckBox FindBestImageMatch;
    }
}
