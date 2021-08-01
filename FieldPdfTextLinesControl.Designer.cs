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
            this.Value = new System.Windows.Forms.RichTextBox();
            this.SpecialTextAutoInsertSpace = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ColumnOfTable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceIgnoreSourceSpaces = new System.Windows.Forms.CheckBox();
            this.textAutoInsertSpaceRepresentative = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textAutoInsertSpaceThreshold = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.gSpacing = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).BeginInit();
            this.gSpacing.SuspendLayout();
            this.SuspendLayout();
            // 
            // Value
            // 
            this.Value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Value.Location = new System.Drawing.Point(0, 91);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(247, 190);
            this.Value.TabIndex = 2;
            this.Value.Text = "";
            // 
            // SpecialTextAutoInsertSpace
            // 
            this.SpecialTextAutoInsertSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpecialTextAutoInsertSpace.Appearance = System.Windows.Forms.Appearance.Button;
            this.SpecialTextAutoInsertSpace.AutoSize = true;
            this.SpecialTextAutoInsertSpace.Location = new System.Drawing.Point(191, 1);
            this.SpecialTextAutoInsertSpace.Name = "SpecialTextAutoInsertSpace";
            this.SpecialTextAutoInsertSpace.Size = new System.Drawing.Size(56, 23);
            this.SpecialTextAutoInsertSpace.TabIndex = 6;
            this.SpecialTextAutoInsertSpace.Text = "Spacing";
            this.SpecialTextAutoInsertSpace.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ColumnOfTable);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.SpecialTextAutoInsertSpace);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(247, 29);
            this.panel1.TabIndex = 8;
            // 
            // ColumnOfTable
            // 
            this.ColumnOfTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnOfTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnOfTable.FormattingEnabled = true;
            this.ColumnOfTable.Location = new System.Drawing.Point(90, 2);
            this.ColumnOfTable.MinimumSize = new System.Drawing.Size(30, 0);
            this.ColumnOfTable.Name = "ColumnOfTable";
            this.ColumnOfTable.Size = new System.Drawing.Size(95, 21);
            this.ColumnOfTable.TabIndex = 72;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Column Of Field:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(80, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "Ignore Source Spaces:";
            // 
            // textAutoInsertSpaceIgnoreSourceSpaces
            // 
            this.textAutoInsertSpaceIgnoreSourceSpaces.AutoSize = true;
            this.textAutoInsertSpaceIgnoreSourceSpaces.Location = new System.Drawing.Point(202, 18);
            this.textAutoInsertSpaceIgnoreSourceSpaces.Name = "textAutoInsertSpaceIgnoreSourceSpaces";
            this.textAutoInsertSpaceIgnoreSourceSpaces.Size = new System.Drawing.Size(15, 14);
            this.textAutoInsertSpaceIgnoreSourceSpaces.TabIndex = 73;
            this.textAutoInsertSpaceIgnoreSourceSpaces.UseVisualStyleBackColor = true;
            // 
            // textAutoInsertSpaceRepresentative
            // 
            this.textAutoInsertSpaceRepresentative.Location = new System.Drawing.Point(165, 37);
            this.textAutoInsertSpaceRepresentative.Name = "textAutoInsertSpaceRepresentative";
            this.textAutoInsertSpaceRepresentative.Size = new System.Drawing.Size(52, 20);
            this.textAutoInsertSpaceRepresentative.TabIndex = 71;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(80, 41);
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
            this.textAutoInsertSpaceThreshold.Location = new System.Drawing.Point(14, 37);
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
            this.label12.Location = new System.Drawing.Point(11, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 69;
            this.label12.Text = "Threshold:";
            // 
            // gSpacing
            // 
            this.gSpacing.Controls.Add(this.textAutoInsertSpaceThreshold);
            this.gSpacing.Controls.Add(this.label4);
            this.gSpacing.Controls.Add(this.label12);
            this.gSpacing.Controls.Add(this.textAutoInsertSpaceIgnoreSourceSpaces);
            this.gSpacing.Controls.Add(this.label15);
            this.gSpacing.Controls.Add(this.textAutoInsertSpaceRepresentative);
            this.gSpacing.Dock = System.Windows.Forms.DockStyle.Top;
            this.gSpacing.Location = new System.Drawing.Point(0, 29);
            this.gSpacing.Name = "gSpacing";
            this.gSpacing.Size = new System.Drawing.Size(247, 62);
            this.gSpacing.TabIndex = 75;
            this.gSpacing.TabStop = false;
            this.gSpacing.Text = "Auto-Insert Space";
            // 
            // FieldPdfTextLinesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Value);
            this.Controls.Add(this.gSpacing);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(200, 100);
            this.Name = "FieldPdfTextLinesControl";
            this.Size = new System.Drawing.Size(247, 281);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textAutoInsertSpaceThreshold)).EndInit();
            this.gSpacing.ResumeLayout(false);
            this.gSpacing.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox Value;
        private System.Windows.Forms.CheckBox SpecialTextAutoInsertSpace;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox textAutoInsertSpaceIgnoreSourceSpaces;
        private System.Windows.Forms.TextBox textAutoInsertSpaceRepresentative;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown textAutoInsertSpaceThreshold;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox ColumnOfTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gSpacing;
    }
}
