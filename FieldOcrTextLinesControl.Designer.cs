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
            this.label2 = new System.Windows.Forms.Label();
            this.Rectangle = new System.Windows.Forms.TextBox();
            this.SpecialOcrSettings = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SpecialTextAutoInsertSpace = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ColumnOfTable
            // 
            this.ColumnOfTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnOfTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnOfTable.FormattingEnabled = true;
            this.ColumnOfTable.Location = new System.Drawing.Point(123, 0);
            this.ColumnOfTable.Name = "ColumnOfTable";
            this.ColumnOfTable.Size = new System.Drawing.Size(167, 21);
            this.ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            this.Value.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Value.Location = new System.Drawing.Point(3, 66);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(287, 156);
            this.Value.TabIndex = 2;
            this.Value.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Column Of Table Field:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rectangle:";
            // 
            // Rectangle
            // 
            this.Rectangle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Rectangle.Location = new System.Drawing.Point(68, 24);
            this.Rectangle.Name = "Rectangle";
            this.Rectangle.Size = new System.Drawing.Size(222, 20);
            this.Rectangle.TabIndex = 5;
            // 
            // SpecialOcrSettings
            // 
            this.SpecialOcrSettings.AutoSize = true;
            this.SpecialOcrSettings.Location = new System.Drawing.Point(119, 50);
            this.SpecialOcrSettings.Name = "SpecialOcrSettings";
            this.SpecialOcrSettings.Size = new System.Drawing.Size(15, 14);
            this.SpecialOcrSettings.TabIndex = 6;
            this.SpecialOcrSettings.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Special OCR Settings:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(146, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Special Text Space:";
            // 
            // SpecialTextAutoInsertSpace
            // 
            this.SpecialTextAutoInsertSpace.AutoSize = true;
            this.SpecialTextAutoInsertSpace.Location = new System.Drawing.Point(251, 50);
            this.SpecialTextAutoInsertSpace.Name = "SpecialTextAutoInsertSpace";
            this.SpecialTextAutoInsertSpace.Size = new System.Drawing.Size(15, 14);
            this.SpecialTextAutoInsertSpace.TabIndex = 8;
            this.SpecialTextAutoInsertSpace.UseVisualStyleBackColor = true;
            // 
            // FieldOcrCharBoxsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SpecialTextAutoInsertSpace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SpecialOcrSettings);
            this.Controls.Add(this.Rectangle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Value);
            this.Controls.Add(this.ColumnOfTable);
            this.Name = "FieldOcrCharBoxsControl";
            this.Size = new System.Drawing.Size(293, 223);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Rectangle;
        private System.Windows.Forms.CheckBox SpecialOcrSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox SpecialTextAutoInsertSpace;
    }
}
