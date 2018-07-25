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
            this.panel2 = new System.Windows.Forms.Panel();
            this.SelectionBoxColor = new System.Windows.Forms.Button();
            this.FloatingAnchorMasterBoxColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bFloatingAnchorMasterBoxColor = new System.Windows.Forms.Button();
            this.bSelectionBoxColor = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CoordinateDeviationMargin = new System.Windows.Forms.NumericUpDown();
            this.FloatingAnchorSecondaryBoxColor = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.bFloatingAnchorSecondaryBoxColor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PdfPageImageResolution)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoordinateDeviationMargin)).BeginInit();
            this.SuspendLayout();
            // 
            // PdfPageImageResolution
            // 
            this.PdfPageImageResolution.Location = new System.Drawing.Point(234, 33);
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
            this.PdfPageImageResolution.Size = new System.Drawing.Size(69, 20);
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
            this.label8.Location = new System.Drawing.Point(16, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(194, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Image Resolution (OCR requires>=300):";
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(113, 3);
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
            this.bSave.Location = new System.Drawing.Point(194, 3);
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
            this.bCancel.Location = new System.Drawing.Point(275, 3);
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
            this.flowLayoutPanel1.Controls.Add(this.About);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 255);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(353, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // About
            // 
            this.About.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.About.Location = new System.Drawing.Point(32, 3);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(75, 23);
            this.About.TabIndex = 51;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.bFloatingAnchorSecondaryBoxColor);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.bFloatingAnchorMasterBoxColor);
            this.groupBox3.Controls.Add(this.bSelectionBoxColor);
            this.groupBox3.Location = new System.Drawing.Point(13, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(328, 122);
            this.groupBox3.TabIndex = 59;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appearance";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.FloatingAnchorSecondaryBoxColor);
            this.panel2.Controls.Add(this.SelectionBoxColor);
            this.panel2.Controls.Add(this.FloatingAnchorMasterBoxColor);
            this.panel2.Location = new System.Drawing.Point(193, 14);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(63, 93);
            this.panel2.TabIndex = 54;
            // 
            // SelectionBoxColor
            // 
            this.SelectionBoxColor.BackColor = System.Drawing.Color.White;
            this.SelectionBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectionBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.SelectionBoxColor.Location = new System.Drawing.Point(6, 8);
            this.SelectionBoxColor.Name = "SelectionBoxColor";
            this.SelectionBoxColor.Size = new System.Drawing.Size(49, 19);
            this.SelectionBoxColor.TabIndex = 56;
            this.SelectionBoxColor.UseVisualStyleBackColor = false;
            // 
            // FloatingAnchorMasterBoxColor
            // 
            this.FloatingAnchorMasterBoxColor.BackColor = System.Drawing.Color.White;
            this.FloatingAnchorMasterBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FloatingAnchorMasterBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.FloatingAnchorMasterBoxColor.Location = new System.Drawing.Point(6, 35);
            this.FloatingAnchorMasterBoxColor.Name = "FloatingAnchorMasterBoxColor";
            this.FloatingAnchorMasterBoxColor.Size = new System.Drawing.Size(49, 19);
            this.FloatingAnchorMasterBoxColor.TabIndex = 55;
            this.FloatingAnchorMasterBoxColor.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(16, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "Anchor Master Box Color:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(16, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Selection Box Color:";
            // 
            // bFloatingAnchorMasterBoxColor
            // 
            this.bFloatingAnchorMasterBoxColor.Location = new System.Drawing.Point(280, 47);
            this.bFloatingAnchorMasterBoxColor.Name = "bFloatingAnchorMasterBoxColor";
            this.bFloatingAnchorMasterBoxColor.Size = new System.Drawing.Size(24, 23);
            this.bFloatingAnchorMasterBoxColor.TabIndex = 50;
            this.bFloatingAnchorMasterBoxColor.Text = "...";
            this.bFloatingAnchorMasterBoxColor.UseVisualStyleBackColor = true;
            this.bFloatingAnchorMasterBoxColor.Click += new System.EventHandler(this.bFloatingAnchorMasterBoxColor_Click);
            // 
            // bSelectionBoxColor
            // 
            this.bSelectionBoxColor.Location = new System.Drawing.Point(280, 19);
            this.bSelectionBoxColor.Name = "bSelectionBoxColor";
            this.bSelectionBoxColor.Size = new System.Drawing.Size(24, 23);
            this.bSelectionBoxColor.TabIndex = 48;
            this.bSelectionBoxColor.Text = "...";
            this.bSelectionBoxColor.UseVisualStyleBackColor = true;
            this.bSelectionBoxColor.Click += new System.EventHandler(this.bSelectionBoxColor_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.CoordinateDeviationMargin);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.PdfPageImageResolution);
            this.groupBox4.Location = new System.Drawing.Point(13, 147);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(328, 97);
            this.groupBox4.TabIndex = 60;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Image Processing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Coordinate Deviation Margin:";
            // 
            // CoordinateDeviationMargin
            // 
            this.CoordinateDeviationMargin.DecimalPlaces = 3;
            this.CoordinateDeviationMargin.Location = new System.Drawing.Point(234, 61);
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
            this.CoordinateDeviationMargin.Size = new System.Drawing.Size(69, 20);
            this.CoordinateDeviationMargin.TabIndex = 56;
            this.CoordinateDeviationMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // FloatingAnchorSecondaryBoxColor
            // 
            this.FloatingAnchorSecondaryBoxColor.BackColor = System.Drawing.Color.White;
            this.FloatingAnchorSecondaryBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FloatingAnchorSecondaryBoxColor.ForeColor = System.Drawing.Color.Maroon;
            this.FloatingAnchorSecondaryBoxColor.Location = new System.Drawing.Point(6, 63);
            this.FloatingAnchorSecondaryBoxColor.Name = "FloatingAnchorSecondaryBoxColor";
            this.FloatingAnchorSecondaryBoxColor.Size = new System.Drawing.Size(49, 19);
            this.FloatingAnchorSecondaryBoxColor.TabIndex = 57;
            this.FloatingAnchorSecondaryBoxColor.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(15, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "Anchor Secondary Box Color:";
            // 
            // bFloatingAnchorSecondaryBoxColor
            // 
            this.bFloatingAnchorSecondaryBoxColor.Location = new System.Drawing.Point(279, 75);
            this.bFloatingAnchorSecondaryBoxColor.Name = "bFloatingAnchorSecondaryBoxColor";
            this.bFloatingAnchorSecondaryBoxColor.Size = new System.Drawing.Size(24, 23);
            this.bFloatingAnchorSecondaryBoxColor.TabIndex = 55;
            this.bFloatingAnchorSecondaryBoxColor.Text = "...";
            this.bFloatingAnchorSecondaryBoxColor.UseVisualStyleBackColor = true;
            this.bFloatingAnchorSecondaryBoxColor.Click += new System.EventHandler(this.bFloatingAnchorSecondaryBoxColor_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 286);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.flowLayoutPanel1);
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
        private System.Windows.Forms.Button bSelectionBoxColor;
        private System.Windows.Forms.Button bFloatingAnchorMasterBoxColor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SelectionBoxColor;
        private System.Windows.Forms.Button FloatingAnchorMasterBoxColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bFloatingAnchorSecondaryBoxColor;
        private System.Windows.Forms.Button FloatingAnchorSecondaryBoxColor;
    }
}