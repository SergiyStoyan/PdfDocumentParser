namespace Cliver.PdfDocumentParser
{
    partial class AnchorPdfTextControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PositionDeviation = new System.Windows.Forms.NumericUpDown();
            this.PositionDeviationIsAbsolute = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.cSearchRectangleMargin = new System.Windows.Forms.CheckBox();
            this.text = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IgnoreOtherCharsInSearchMargin = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.IgnoreInvisibleChars = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 75;
            this.label2.Text = "(absolute:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Position Deviation:";
            // 
            // PositionDeviation
            // 
            this.PositionDeviation.DecimalPlaces = 1;
            this.PositionDeviation.Location = new System.Drawing.Point(175, 3);
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
            this.PositionDeviation.TabIndex = 74;
            this.PositionDeviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // PositionDeviationIsAbsolute
            // 
            this.PositionDeviationIsAbsolute.AutoSize = true;
            this.PositionDeviationIsAbsolute.Location = new System.Drawing.Point(151, 5);
            this.PositionDeviationIsAbsolute.Name = "PositionDeviationIsAbsolute";
            this.PositionDeviationIsAbsolute.Size = new System.Drawing.Size(15, 14);
            this.PositionDeviationIsAbsolute.TabIndex = 72;
            this.PositionDeviationIsAbsolute.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 83;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(175, 25);
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
            this.SearchRectangleMargin.TabIndex = 84;
            this.SearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(151, 27);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(15, 14);
            this.cSearchRectangleMargin.TabIndex = 85;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // text
            // 
            this.text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.text.Location = new System.Drawing.Point(6, 110);
            this.text.Multiline = true;
            this.text.Name = "text";
            this.text.ReadOnly = true;
            this.text.Size = new System.Drawing.Size(216, 113);
            this.text.TabIndex = 86;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 89;
            this.label5.Text = "Pattern:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(163, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 90;
            this.label4.Text = ")";
            // 
            // IgnoreOtherCharsInSearchMargin
            // 
            this.IgnoreOtherCharsInSearchMargin.AutoSize = true;
            this.IgnoreOtherCharsInSearchMargin.Location = new System.Drawing.Point(207, 50);
            this.IgnoreOtherCharsInSearchMargin.Name = "IgnoreOtherCharsInSearchMargin";
            this.IgnoreOtherCharsInSearchMargin.Size = new System.Drawing.Size(15, 14);
            this.IgnoreOtherCharsInSearchMargin.TabIndex = 94;
            this.IgnoreOtherCharsInSearchMargin.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(183, 13);
            this.label7.TabIndex = 93;
            this.label7.Text = "Ignore Other Chars In Search Margin:";
            // 
            // IgnoreInvisibleChars
            // 
            this.IgnoreInvisibleChars.AutoSize = true;
            this.IgnoreInvisibleChars.Location = new System.Drawing.Point(207, 71);
            this.IgnoreInvisibleChars.Name = "IgnoreInvisibleChars";
            this.IgnoreInvisibleChars.Size = new System.Drawing.Size(15, 14);
            this.IgnoreInvisibleChars.TabIndex = 96;
            this.IgnoreInvisibleChars.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 13);
            this.label8.TabIndex = 95;
            this.label8.Text = "Ignore Invisible Chars:";
            // 
            // AnchorPdfTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.IgnoreInvisibleChars);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.IgnoreOtherCharsInSearchMargin);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.PositionDeviationIsAbsolute);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.text);
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PositionDeviation);
            this.Name = "AnchorPdfTextControl";
            this.Size = new System.Drawing.Size(225, 226);
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown PositionDeviation;
        public System.Windows.Forms.CheckBox PositionDeviationIsAbsolute;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown SearchRectangleMargin;
        private System.Windows.Forms.CheckBox cSearchRectangleMargin;
        private System.Windows.Forms.TextBox text;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox IgnoreOtherCharsInSearchMargin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox IgnoreInvisibleChars;
        private System.Windows.Forms.Label label8;
    }
}
