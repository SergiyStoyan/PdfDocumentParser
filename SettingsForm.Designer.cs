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
            this.About = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.AnchorSecondaryBoxColor = new System.Windows.Forms.Button();
            this.SelectionBoxColor = new System.Windows.Forms.Button();
            this.AnchorMasterBoxColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CoordinateDeviationMargin = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SelectionBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.AnchorMasterBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            this.AnchorSecondaryBoxBorderWidth = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBorderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorMasterBoxBorderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorSecondaryBoxBorderWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // PdfPageImageResolution
            // 
            this.PdfPageImageResolution.Location = new System.Drawing.Point(624, 79);
            this.PdfPageImageResolution.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.PdfPageImageResolution.Size = new System.Drawing.Size(184, 38);
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
            this.label8.Location = new System.Drawing.Point(43, 83);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(521, 32);
            this.label8.TabIndex = 46;
            this.label8.Text = "Image Resolution (OCR requires>=300):";
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(305, 7);
            this.bReset.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(200, 55);
            this.bReset.TabIndex = 48;
            this.bReset.Text = "Reset";
            this.bReset.UseVisualStyleBackColor = true;
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSave.Location = new System.Drawing.Point(521, 7);
            this.bSave.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(200, 55);
            this.bSave.TabIndex = 49;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(737, 7);
            this.bCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(200, 55);
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
            this.flowLayoutPanel1.Controls.Add(this.About);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 676);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(945, 74);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // About
            // 
            this.About.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.About.Location = new System.Drawing.Point(89, 7);
            this.About.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(200, 55);
            this.About.TabIndex = 51;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AnchorSecondaryBoxBorderWidth);
            this.groupBox3.Controls.Add(this.AnchorMasterBoxBorderWidth);
            this.groupBox3.Controls.Add(this.SelectionBoxBorderWidth);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(35, 29);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox3.Size = new System.Drawing.Size(875, 372);
            this.groupBox3.TabIndex = 59;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appearance";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(40, 275);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 32);
            this.label4.TabIndex = 56;
            this.label4.Text = "Anchor Secondary Box:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.AnchorSecondaryBoxColor);
            this.panel2.Controls.Add(this.SelectionBoxColor);
            this.panel2.Controls.Add(this.AnchorMasterBoxColor);
            this.panel2.Location = new System.Drawing.Point(375, 104);
            this.panel2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(168, 222);
            this.panel2.TabIndex = 54;
            // 
            // AnchorSecondaryBoxColor
            // 
            this.AnchorSecondaryBoxColor.BackColor = System.Drawing.Color.White;
            this.AnchorSecondaryBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AnchorSecondaryBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.AnchorSecondaryBoxColor.Location = new System.Drawing.Point(16, 150);
            this.AnchorSecondaryBoxColor.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.AnchorSecondaryBoxColor.Name = "AnchorSecondaryBoxColor";
            this.AnchorSecondaryBoxColor.Size = new System.Drawing.Size(131, 52);
            this.AnchorSecondaryBoxColor.TabIndex = 57;
            this.AnchorSecondaryBoxColor.Text = "...";
            this.AnchorSecondaryBoxColor.UseVisualStyleBackColor = false;
            this.AnchorSecondaryBoxColor.Click += new System.EventHandler(this.AnchorSecondaryBoxColor_Click);
            // 
            // SelectionBoxColor
            // 
            this.SelectionBoxColor.BackColor = System.Drawing.Color.White;
            this.SelectionBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectionBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.SelectionBoxColor.Location = new System.Drawing.Point(16, 19);
            this.SelectionBoxColor.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.SelectionBoxColor.Name = "SelectionBoxColor";
            this.SelectionBoxColor.Size = new System.Drawing.Size(131, 52);
            this.SelectionBoxColor.TabIndex = 56;
            this.SelectionBoxColor.Text = "...";
            this.SelectionBoxColor.UseVisualStyleBackColor = false;
            this.SelectionBoxColor.Click += new System.EventHandler(this.SelectionBoxColor_Click);
            // 
            // AnchorMasterBoxColor
            // 
            this.AnchorMasterBoxColor.BackColor = System.Drawing.Color.White;
            this.AnchorMasterBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AnchorMasterBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.AnchorMasterBoxColor.Location = new System.Drawing.Point(16, 83);
            this.AnchorMasterBoxColor.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.AnchorMasterBoxColor.Name = "AnchorMasterBoxColor";
            this.AnchorMasterBoxColor.Size = new System.Drawing.Size(131, 52);
            this.AnchorMasterBoxColor.TabIndex = 55;
            this.AnchorMasterBoxColor.Text = "...";
            this.AnchorMasterBoxColor.UseVisualStyleBackColor = false;
            this.AnchorMasterBoxColor.Click += new System.EventHandler(this.AnchorMasterBoxColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(43, 209);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 32);
            this.label1.TabIndex = 52;
            this.label1.Text = "Anchor Master Box:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(43, 140);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 32);
            this.label3.TabIndex = 51;
            this.label3.Text = "Selection Box:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.CoordinateDeviationMargin);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.PdfPageImageResolution);
            this.groupBox4.Location = new System.Drawing.Point(35, 415);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox4.Size = new System.Drawing.Size(875, 231);
            this.groupBox4.TabIndex = 60;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Image Processing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 150);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(439, 32);
            this.label2.TabIndex = 55;
            this.label2.Text = "Coordinate Deviation Margin (px):";
            // 
            // CoordinateDeviationMargin
            // 
            this.CoordinateDeviationMargin.DecimalPlaces = 3;
            this.CoordinateDeviationMargin.Location = new System.Drawing.Point(624, 145);
            this.CoordinateDeviationMargin.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
            this.CoordinateDeviationMargin.Size = new System.Drawing.Size(184, 38);
            this.CoordinateDeviationMargin.TabIndex = 56;
            this.CoordinateDeviationMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(369, 58);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 32);
            this.label7.TabIndex = 57;
            this.label7.Text = "Color:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(618, 58);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 32);
            this.label5.TabIndex = 58;
            this.label5.Text = "Border Width:";
            // 
            // SelectionBoxBorderWidth
            // 
            this.SelectionBoxBorderWidth.Location = new System.Drawing.Point(624, 138);
            this.SelectionBoxBorderWidth.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.SelectionBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SelectionBoxBorderWidth.Name = "SelectionBoxBorderWidth";
            this.SelectionBoxBorderWidth.Size = new System.Drawing.Size(113, 38);
            this.SelectionBoxBorderWidth.TabIndex = 59;
            this.SelectionBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AnchorMasterBoxBorderWidth
            // 
            this.AnchorMasterBoxBorderWidth.Location = new System.Drawing.Point(624, 207);
            this.AnchorMasterBoxBorderWidth.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.AnchorMasterBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AnchorMasterBoxBorderWidth.Name = "AnchorMasterBoxBorderWidth";
            this.AnchorMasterBoxBorderWidth.Size = new System.Drawing.Size(113, 38);
            this.AnchorMasterBoxBorderWidth.TabIndex = 60;
            this.AnchorMasterBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AnchorSecondaryBoxBorderWidth
            // 
            this.AnchorSecondaryBoxBorderWidth.Location = new System.Drawing.Point(624, 273);
            this.AnchorSecondaryBoxBorderWidth.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.AnchorSecondaryBoxBorderWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AnchorSecondaryBoxBorderWidth.Name = "AnchorSecondaryBoxBorderWidth";
            this.AnchorSecondaryBoxBorderWidth.Size = new System.Drawing.Size(113, 38);
            this.AnchorSecondaryBoxBorderWidth.TabIndex = 61;
            this.AnchorSecondaryBoxBorderWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 750);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBorderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorMasterBoxBorderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorSecondaryBoxBorderWidth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown PdfPageImageResolution;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown CoordinateDeviationMargin;
        private System.Windows.Forms.Button About;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SelectionBoxColor;
        private System.Windows.Forms.Button AnchorMasterBoxColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AnchorSecondaryBoxColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown SelectionBoxBorderWidth;
        private System.Windows.Forms.NumericUpDown AnchorSecondaryBoxBorderWidth;
        private System.Windows.Forms.NumericUpDown AnchorMasterBoxBorderWidth;
    }
}