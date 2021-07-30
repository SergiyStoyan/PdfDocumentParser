namespace Cliver.PdfDocumentParser
{
    partial class FieldOcrTextLinesControl
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
            this.ColumnOfTable = new System.Windows.Forms.ComboBox();
            this.Value = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SpecialOcrSettings = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SingleFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.ColumnFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TesseractPageSegMode = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ColumnOfTable
            // 
            this.ColumnOfTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnOfTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnOfTable.FormattingEnabled = true;
            this.ColumnOfTable.Location = new System.Drawing.Point(123, 3);
            this.ColumnOfTable.MinimumSize = new System.Drawing.Size(80, 0);
            this.ColumnOfTable.Name = "ColumnOfTable";
            this.ColumnOfTable.Size = new System.Drawing.Size(326, 21);
            this.ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            this.Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Value.Location = new System.Drawing.Point(0, 58);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(524, 223);
            this.Value.TabIndex = 2;
            this.Value.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Column Of Table Field:";
            // 
            // SpecialOcrSettings
            // 
            this.SpecialOcrSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpecialOcrSettings.AutoSize = true;
            this.SpecialOcrSettings.Location = new System.Drawing.Point(467, 6);
            this.SpecialOcrSettings.Name = "SpecialOcrSettings";
            this.SpecialOcrSettings.Size = new System.Drawing.Size(15, 14);
            this.SpecialOcrSettings.TabIndex = 6;
            this.SpecialOcrSettings.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SpecialOcrSettings);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ColumnOfTable);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(524, 29);
            this.panel1.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(454, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 71;
            this.label3.Text = "+";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.TesseractPageSegMode);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.SingleFieldFromFieldImage);
            this.panel2.Controls.Add(this.ColumnFieldFromFieldImage);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(524, 29);
            this.panel2.TabIndex = 9;
            // 
            // SingleFieldFromFieldImage
            // 
            this.SingleFieldFromFieldImage.AutoSize = true;
            this.SingleFieldFromFieldImage.Location = new System.Drawing.Point(43, 4);
            this.SingleFieldFromFieldImage.Name = "SingleFieldFromFieldImage";
            this.SingleFieldFromFieldImage.Size = new System.Drawing.Size(141, 17);
            this.SingleFieldFromFieldImage.TabIndex = 105;
            this.SingleFieldFromFieldImage.Text = "Single Field By Its Image";
            this.SingleFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // ColumnFieldFromFieldImage
            // 
            this.ColumnFieldFromFieldImage.AutoSize = true;
            this.ColumnFieldFromFieldImage.Location = new System.Drawing.Point(190, 4);
            this.ColumnFieldFromFieldImage.Name = "ColumnFieldFromFieldImage";
            this.ColumnFieldFromFieldImage.Size = new System.Drawing.Size(147, 17);
            this.ColumnFieldFromFieldImage.TabIndex = 106;
            this.ColumnFieldFromFieldImage.Text = "Column Field By Its Image";
            this.ColumnFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 72;
            this.label2.Text = "OCR:";
            // 
            // TesseractPageSegMode
            // 
            this.TesseractPageSegMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TesseractPageSegMode.FormattingEnabled = true;
            this.TesseractPageSegMode.Location = new System.Drawing.Point(436, 3);
            this.TesseractPageSegMode.Name = "TesseractPageSegMode";
            this.TesseractPageSegMode.Size = new System.Drawing.Size(79, 21);
            this.TesseractPageSegMode.TabIndex = 109;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(343, 5);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 13);
            this.label19.TabIndex = 108;
            this.label19.Text = "Tesseract Mode:";
            // 
            // FieldOcrTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Value);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(300, 100);
            this.Name = "FieldOcrTextControl";
            this.Size = new System.Drawing.Size(524, 281);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SpecialOcrSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox SingleFieldFromFieldImage;
        private System.Windows.Forms.CheckBox ColumnFieldFromFieldImage;
        private System.Windows.Forms.ComboBox TesseractPageSegMode;
        private System.Windows.Forms.Label label19;
    }
}
