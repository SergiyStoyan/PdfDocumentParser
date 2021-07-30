namespace Cliver.PdfDocumentParser
{
    partial class FieldPdfTextLinesControl
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
            this.SpecialTextAutoInsertSpace = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceIgnoreSourceSpaces = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceRepresentative = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceThreshold = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).BeginInit();
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
            this.ColumnOfTable.Size = new System.Drawing.Size(135, 21);
            this.ColumnOfTable.TabIndex = 0;
            // 
            // Value
            // 
            this.Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Value.Location = new System.Drawing.Point(0, 61);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(333, 220);
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
            // SpecialTextAutoInsertSpace
            // 
            this.SpecialTextAutoInsertSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpecialTextAutoInsertSpace.AutoSize = true;
            this.SpecialTextAutoInsertSpace.Location = new System.Drawing.Point(276, 6);
            this.SpecialTextAutoInsertSpace.Name = "SpecialTextAutoInsertSpace";
            this.SpecialTextAutoInsertSpace.Size = new System.Drawing.Size(15, 14);
            this.SpecialTextAutoInsertSpace.TabIndex = 6;
            this.SpecialTextAutoInsertSpace.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SpecialTextAutoInsertSpace);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ColumnOfTable);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(333, 29);
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
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.textAutoInsertSpaceIgnoreSourceSpaces);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textAutoInsertSpaceRepresentative);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.textAutoInsertSpaceThreshold);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(333, 32);
            this.panel2.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(322, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "Ignore Source:";
            // 
            // textAutoInsertSpaceIgnoreSourceSpaces
            // 
            this.textAutoInsertSpaceIgnoreSourceSpaces.AutoSize = true;
            this.textAutoInsertSpaceIgnoreSourceSpaces.Location = new System.Drawing.Point(402, 6);
            this.textAutoInsertSpaceIgnoreSourceSpaces.Name = "textAutoInsertSpaceIgnoreSourceSpaces";
            this.textAutoInsertSpaceIgnoreSourceSpaces.Size = new System.Drawing.Size(15, 14);
            this.textAutoInsertSpaceIgnoreSourceSpaces.TabIndex = 73;
            this.textAutoInsertSpaceIgnoreSourceSpaces.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 72;
            this.label2.Text = "Insert Space:";
            // 
            // textAutoInsertSpaceRepresentative
            // 
            this.textAutoInsertSpaceRepresentative.Location = new System.Drawing.Point(264, 3);
            this.textAutoInsertSpaceRepresentative.Name = "textAutoInsertSpaceRepresentative";
            this.textAutoInsertSpaceRepresentative.Size = new System.Drawing.Size(52, 20);
            this.textAutoInsertSpaceRepresentative.TabIndex = 71;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(201, 5);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 70;
            this.label15.Text = "Substitute:";
            // 
            // textAutoInsertSpaceThreshold
            // 
            this.textAutoInsertSpaceThreshold.DecimalPlaces = 2;
            this.textAutoInsertSpaceThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.textAutoInsertSpaceThreshold.Location = new System.Drawing.Point(143, 3);
            this.textAutoInsertSpaceThreshold.Name = "textAutoInsertSpaceThreshold";
            this.textAutoInsertSpaceThreshold.Size = new System.Drawing.Size(52, 20);
            this.textAutoInsertSpaceThreshold.TabIndex = 68;
            this.textAutoInsertSpaceThreshold.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(80, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 69;
            this.label12.Text = "Threshold:";
            // 
            // FieldPdfTextLinesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Value);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(300, 100);
            this.Name = "FieldPdfTextLinesControl";
            this.Size = new System.Drawing.Size(333, 281);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SpecialTextAutoInsertSpace;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox textAutoInsertSpaceIgnoreSourceSpaces;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textAutoInsertSpaceRepresentative;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown textAutoInsertSpaceThreshold;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
    }
}
