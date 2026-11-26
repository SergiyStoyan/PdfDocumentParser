namespace Cliver.PdfDocumentParser
{
    partial class FieldOcrTextControl2
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
            ColumnOfTable = new ComboBox();
            Value = new RichTextBox();
            label1 = new Label();
            SpecialOcrSettings = new CheckBox();
            panel1 = new Panel();
            gOcr = new GroupBox();
            label19 = new Label();
            CropPadding = new NumericUpDown();
            panel1.SuspendLayout();
            gOcr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CropPadding).BeginInit();
            SuspendLayout();
            // 
            // ColumnOfTable
            // 
            ColumnOfTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ColumnOfTable.DropDownStyle = ComboBoxStyle.DropDownList;
            ColumnOfTable.FormattingEnabled = true;
            ColumnOfTable.Location = new Point(100, 3);
            ColumnOfTable.Margin = new Padding(4, 3, 4, 3);
            ColumnOfTable.MinimumSize = new Size(34, 0);
            ColumnOfTable.Name = "ColumnOfTable";
            ColumnOfTable.Size = new Size(153, 23);
            ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            Value.Dock = DockStyle.Fill;
            Value.Location = new Point(0, 79);
            Value.Margin = new Padding(4, 3, 4, 3);
            Value.Name = "Value";
            Value.Size = new Size(312, 245);
            Value.TabIndex = 2;
            Value.Text = "";
            Value.WordWrap = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 7);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 3;
            label1.Text = "Column Of Field:";
            // 
            // SpecialOcrSettings
            // 
            SpecialOcrSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SpecialOcrSettings.Appearance = Appearance.Button;
            SpecialOcrSettings.AutoSize = true;
            SpecialOcrSettings.Location = new Point(267, 2);
            SpecialOcrSettings.Margin = new Padding(4, 3, 4, 3);
            SpecialOcrSettings.Name = "SpecialOcrSettings";
            SpecialOcrSettings.Size = new Size(41, 25);
            SpecialOcrSettings.TabIndex = 6;
            SpecialOcrSettings.Text = "OCR";
            SpecialOcrSettings.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(SpecialOcrSettings);
            panel1.Controls.Add(ColumnOfTable);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(312, 33);
            panel1.TabIndex = 8;
            // 
            // gOcr
            // 
            gOcr.Controls.Add(CropPadding);
            gOcr.Controls.Add(label19);
            gOcr.Dock = DockStyle.Top;
            gOcr.Location = new Point(0, 33);
            gOcr.Margin = new Padding(4, 3, 4, 3);
            gOcr.Name = "gOcr";
            gOcr.Padding = new Padding(4, 3, 4, 3);
            gOcr.Size = new Size(312, 46);
            gOcr.TabIndex = 11;
            gOcr.TabStop = false;
            gOcr.Text = "OCR";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(4, 20);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(83, 15);
            label19.TabIndex = 108;
            label19.Text = "Crop Padding:";
            // 
            // CropPadding
            // 
            CropPadding.Location = new Point(94, 18);
            CropPadding.Name = "CropPadding";
            CropPadding.Size = new Size(46, 23);
            CropPadding.TabIndex = 109;
            CropPadding.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // FieldOcrTextControl2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Value);
            Controls.Add(gOcr);
            Controls.Add(panel1);
            Margin = new Padding(5, 3, 5, 3);
            MinimumSize = new Size(233, 115);
            Name = "FieldOcrTextControl2";
            Size = new Size(312, 324);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            gOcr.ResumeLayout(false);
            gOcr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CropPadding).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SpecialOcrSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gOcr;
        private System.Windows.Forms.Label label19;
        private NumericUpDown CropPadding;
    }
}
