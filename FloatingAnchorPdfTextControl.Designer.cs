﻿namespace Cliver.PdfDocumentParser
{
    partial class FloatingAnchorPdfTextControl
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
            this.text = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PositionDeviation = new System.Windows.Forms.NumericUpDown();
            this.PositionDeviationIsAbsolute = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).BeginInit();
            this.SuspendLayout();
            // 
            // text
            // 
            this.text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.text.Location = new System.Drawing.Point(0, 45);
            this.text.Name = "text";
            this.text.ReadOnly = true;
            this.text.Size = new System.Drawing.Size(204, 107);
            this.text.TabIndex = 62;
            this.text.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 75;
            this.label2.Text = "Position Deviation Is Absolute:";
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
            this.PositionDeviation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionDeviation.Location = new System.Drawing.Point(154, 3);
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
            this.PositionDeviation.Size = new System.Drawing.Size(42, 20);
            this.PositionDeviation.TabIndex = 74;
            this.PositionDeviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // PositionDeviationIsAbsolute
            // 
            this.PositionDeviationIsAbsolute.AutoSize = true;
            this.PositionDeviationIsAbsolute.Enabled = false;
            this.PositionDeviationIsAbsolute.Location = new System.Drawing.Point(154, 25);
            this.PositionDeviationIsAbsolute.Name = "PositionDeviationIsAbsolute";
            this.PositionDeviationIsAbsolute.Size = new System.Drawing.Size(15, 14);
            this.PositionDeviationIsAbsolute.TabIndex = 72;
            this.PositionDeviationIsAbsolute.UseVisualStyleBackColor = true;
            // 
            // FloatingAnchorPdfTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PositionDeviation);
            this.Controls.Add(this.PositionDeviationIsAbsolute);
            this.Controls.Add(this.text);
            this.Name = "FloatingAnchorPdfTextControl";
            this.Size = new System.Drawing.Size(204, 152);
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox text;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown PositionDeviation;
        public System.Windows.Forms.CheckBox PositionDeviationIsAbsolute;
    }
}