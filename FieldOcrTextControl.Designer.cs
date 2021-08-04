namespace Cliver.PdfDocumentParser
{
    partial class FieldOcrTextControl
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
            this.label19 = new System.Windows.Forms.Label();
            this.SingleFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.ColumnCellFromCellImage = new System.Windows.Forms.CheckBox();
            this.gOcr = new System.Windows.Forms.GroupBox();
            this.TesseractPageSegMode = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.gOcr.SuspendLayout();
            this.SuspendLayout();
            // 
            // ColumnOfTable
            // 
            this.ColumnOfTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnOfTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnOfTable.FormattingEnabled = true;
            this.ColumnOfTable.Location = new System.Drawing.Point(86, 3);
            this.ColumnOfTable.MinimumSize = new System.Drawing.Size(30, 0);
            this.ColumnOfTable.Name = "ColumnOfTable";
            this.ColumnOfTable.Size = new System.Drawing.Size(132, 21);
            this.ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            this.Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Value.Location = new System.Drawing.Point(0, 88);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(267, 193);
            this.Value.TabIndex = 2;
            this.Value.Text = "";
            this.Value.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Column Of Field:";
            // 
            // SpecialOcrSettings
            // 
            this.SpecialOcrSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpecialOcrSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.SpecialOcrSettings.AutoSize = true;
            this.SpecialOcrSettings.Location = new System.Drawing.Point(224, 2);
            this.SpecialOcrSettings.Name = "SpecialOcrSettings";
            this.SpecialOcrSettings.Size = new System.Drawing.Size(40, 23);
            this.SpecialOcrSettings.TabIndex = 6;
            this.SpecialOcrSettings.Text = "OCR";
            this.SpecialOcrSettings.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SpecialOcrSettings);
            this.panel1.Controls.Add(this.ColumnOfTable);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(267, 29);
            this.panel1.TabIndex = 8;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(13, 17);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 13);
            this.label19.TabIndex = 108;
            this.label19.Text = "Tesseract Mode:";
            // 
            // SingleFieldFromFieldImage
            // 
            this.SingleFieldFromFieldImage.AutoSize = true;
            this.SingleFieldFromFieldImage.Location = new System.Drawing.Point(130, 16);
            this.SingleFieldFromFieldImage.Name = "SingleFieldFromFieldImage";
            this.SingleFieldFromFieldImage.Size = new System.Drawing.Size(141, 17);
            this.SingleFieldFromFieldImage.TabIndex = 105;
            this.SingleFieldFromFieldImage.Text = "Single Field By Its Image";
            this.SingleFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // ColumnCellFromCellImage
            // 
            this.ColumnCellFromCellImage.AutoSize = true;
            this.ColumnCellFromCellImage.Location = new System.Drawing.Point(130, 36);
            this.ColumnCellFromCellImage.Name = "ColumnCellFromCellImage";
            this.ColumnCellFromCellImage.Size = new System.Drawing.Size(147, 17);
            this.ColumnCellFromCellImage.TabIndex = 106;
            this.ColumnCellFromCellImage.Text = "Column Cell By Its Image";
            this.ColumnCellFromCellImage.UseVisualStyleBackColor = true;
            // 
            // gOcr
            // 
            this.gOcr.Controls.Add(this.TesseractPageSegMode);
            this.gOcr.Controls.Add(this.label19);
            this.gOcr.Controls.Add(this.ColumnCellFromCellImage);
            this.gOcr.Controls.Add(this.SingleFieldFromFieldImage);
            this.gOcr.Dock = System.Windows.Forms.DockStyle.Top;
            this.gOcr.Location = new System.Drawing.Point(0, 29);
            this.gOcr.Name = "gOcr";
            this.gOcr.Size = new System.Drawing.Size(267, 59);
            this.gOcr.TabIndex = 10;
            this.gOcr.TabStop = false;
            this.gOcr.Text = "OCR";
            // 
            // TesseractPageSegMode
            // 
            this.TesseractPageSegMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TesseractPageSegMode.DropDownWidth = 120;
            this.TesseractPageSegMode.FormattingEnabled = true;
            this.TesseractPageSegMode.Location = new System.Drawing.Point(16, 32);
            this.TesseractPageSegMode.Name = "TesseractPageSegMode";
            this.TesseractPageSegMode.Size = new System.Drawing.Size(103, 21);
            this.TesseractPageSegMode.TabIndex = 142;
            // 
            // FieldOcrTextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Value);
            this.Controls.Add(this.gOcr);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(200, 100);
            this.Name = "FieldOcrTextControl";
            this.Size = new System.Drawing.Size(267, 281);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gOcr.ResumeLayout(false);
            this.gOcr.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SpecialOcrSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox SingleFieldFromFieldImage;
        private System.Windows.Forms.CheckBox ColumnCellFromCellImage;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox gOcr;
        private System.Windows.Forms.ComboBox TesseractPageSegMode;
    }
}
