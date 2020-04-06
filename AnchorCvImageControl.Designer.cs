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
            ((System.ComponentModel.ISupportInitialize)(this.Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleDeviation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 59);
            this.label13.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(215, 32);
            this.label13.TabIndex = 63;
            this.label13.Text = "ScaleDeviation:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 10);
            this.label11.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(150, 32);
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
            this.Threshold.Location = new System.Drawing.Point(467, 6);
            this.Threshold.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.Threshold.Size = new System.Drawing.Size(125, 38);
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
            this.ScaleDeviation.Location = new System.Drawing.Point(467, 56);
            this.ScaleDeviation.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.ScaleDeviation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ScaleDeviation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ScaleDeviation.Name = "ScaleDeviation";
            this.ScaleDeviation.Size = new System.Drawing.Size(125, 38);
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
            this.label4.Location = new System.Drawing.Point(8, 157);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 32);
            this.label4.TabIndex = 76;
            this.label4.Text = "Pattern:";
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(403, 112);
            this.cSearchRectangleMargin.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(34, 33);
            this.cSearchRectangleMargin.TabIndex = 97;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 108);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 32);
            this.label3.TabIndex = 95;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(467, 106);
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
            this.pictureBox.Location = new System.Drawing.Point(3, 195);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(76, 65);
            this.pictureBox.TabIndex = 98;
            this.pictureBox.TabStop = false;
            // 
            // AnchorCvImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Threshold);
            this.Controls.Add(this.ScaleDeviation);
            this.Margin = new System.Windows.Forms.Padding(21, 17, 21, 17);
            this.MinimumSize = new System.Drawing.Size(213, 172);
            this.Name = "AnchorCvImageControl";
            this.Size = new System.Drawing.Size(603, 424);
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
    }
}
