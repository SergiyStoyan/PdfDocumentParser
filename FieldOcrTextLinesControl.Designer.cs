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
            this.label2 = new System.Windows.Forms.Label();
            this.OcrSingleFieldFromFieldImage = new System.Windows.Forms.CheckBox();
            this.OcrColumnFieldFromFieldImage = new System.Windows.Forms.CheckBox();
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
            this.ColumnOfTable.Size = new System.Drawing.Size(136, 21);
            this.ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            this.Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Value.Location = new System.Drawing.Point(0, 61);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(334, 220);
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
            this.SpecialOcrSettings.Location = new System.Drawing.Point(277, 6);
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
            this.panel1.Size = new System.Drawing.Size(334, 29);
            this.panel1.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 71;
            this.label3.Text = "+";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.OcrSingleFieldFromFieldImage);
            this.panel2.Controls.Add(this.OcrColumnFieldFromFieldImage);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(334, 32);
            this.panel2.TabIndex = 9;
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
            // OcrSingleFieldFromFieldImage
            // 
            this.OcrSingleFieldFromFieldImage.AutoSize = true;
            this.OcrSingleFieldFromFieldImage.Location = new System.Drawing.Point(43, 4);
            this.OcrSingleFieldFromFieldImage.Name = "OcrSingleFieldFromFieldImage";
            this.OcrSingleFieldFromFieldImage.Size = new System.Drawing.Size(141, 17);
            this.OcrSingleFieldFromFieldImage.TabIndex = 105;
            this.OcrSingleFieldFromFieldImage.Text = "Single Field By Its Image";
            this.OcrSingleFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // OcrColumnFieldFromFieldImage
            // 
            this.OcrColumnFieldFromFieldImage.AutoSize = true;
            this.OcrColumnFieldFromFieldImage.Location = new System.Drawing.Point(190, 6);
            this.OcrColumnFieldFromFieldImage.Name = "OcrColumnFieldFromFieldImage";
            this.OcrColumnFieldFromFieldImage.Size = new System.Drawing.Size(147, 17);
            this.OcrColumnFieldFromFieldImage.TabIndex = 106;
            this.OcrColumnFieldFromFieldImage.Text = "Column Field By Its Image";
            this.OcrColumnFieldFromFieldImage.UseVisualStyleBackColor = true;
            // 
            // FieldOcrCharBoxsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Value);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(300, 100);
            this.Name = "FieldOcrCharBoxsControl";
            this.Size = new System.Drawing.Size(334, 281);
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
        private System.Windows.Forms.CheckBox OcrSingleFieldFromFieldImage;
        private System.Windows.Forms.CheckBox OcrColumnFieldFromFieldImage;
    }
}
