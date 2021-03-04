namespace Cliver.PdfDocumentParser
{
    partial class BitmapPreparationForm
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
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bApply = new System.Windows.Forms.Button();
            this.defaultBitmapPreprocessorClassDefinitions = new System.Windows.Forms.ComboBox();
            this.bRemove = new System.Windows.Forms.Button();
            this.bSaveDafault = new System.Windows.Forms.Button();
            this.bitmapPreprocessorClassDefinition = new ICSharpCode.TextEditor.TextEditorControl();
            this.label9 = new System.Windows.Forms.Label();
            this.autoDeskewThreshold = new System.Windows.Forms.NumericUpDown();
            this.autoDeskew = new System.Windows.Forms.CheckBox();
            this.pageRotation = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PreprocessBitmap = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DetectingScaleAnchor = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoDeskewThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(681, 3);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 50;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bOK.Location = new System.Drawing.Point(600, 3);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 49;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.bCancel);
            this.flowLayoutPanel1.Controls.Add(this.bOK);
            this.flowLayoutPanel1.Controls.Add(this.bApply);
            this.flowLayoutPanel1.Controls.Add(this.defaultBitmapPreprocessorClassDefinitions);
            this.flowLayoutPanel1.Controls.Add(this.bRemove);
            this.flowLayoutPanel1.Controls.Add(this.bSaveDafault);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 279);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(759, 31);
            this.flowLayoutPanel1.TabIndex = 60;
            // 
            // bApply
            // 
            this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bApply.Location = new System.Drawing.Point(519, 3);
            this.bApply.Name = "bApply";
            this.bApply.Size = new System.Drawing.Size(75, 23);
            this.bApply.TabIndex = 51;
            this.bApply.Text = "Apply";
            this.bApply.UseVisualStyleBackColor = true;
            this.bApply.Click += new System.EventHandler(this.bApply_Click);
            // 
            // defaultBitmapPreprocessorClassDefinitions
            // 
            this.defaultBitmapPreprocessorClassDefinitions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defaultBitmapPreprocessorClassDefinitions.FormattingEnabled = true;
            this.defaultBitmapPreprocessorClassDefinitions.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.defaultBitmapPreprocessorClassDefinitions.Location = new System.Drawing.Point(292, 3);
            this.defaultBitmapPreprocessorClassDefinitions.Name = "defaultBitmapPreprocessorClassDefinitions";
            this.defaultBitmapPreprocessorClassDefinitions.Size = new System.Drawing.Size(221, 21);
            this.defaultBitmapPreprocessorClassDefinitions.TabIndex = 85;
            this.defaultBitmapPreprocessorClassDefinitions.SelectedIndexChanged += new System.EventHandler(this.defaults_SelectedIndexChanged);
            // 
            // bRemove
            // 
            this.bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bRemove.Location = new System.Drawing.Point(211, 3);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(75, 23);
            this.bRemove.TabIndex = 52;
            this.bRemove.Text = "Remove";
            this.bRemove.UseVisualStyleBackColor = true;
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bSaveDafault
            // 
            this.bSaveDafault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSaveDafault.Location = new System.Drawing.Point(130, 3);
            this.bSaveDafault.Name = "bSaveDafault";
            this.bSaveDafault.Size = new System.Drawing.Size(75, 23);
            this.bSaveDafault.TabIndex = 53;
            this.bSaveDafault.Text = "Add";
            this.bSaveDafault.UseVisualStyleBackColor = true;
            this.bSaveDafault.Click += new System.EventHandler(this.bSaveDafault_Click);
            // 
            // bitmapPreprocessorClassDefinition
            // 
            this.bitmapPreprocessorClassDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapPreprocessorClassDefinition.AutoHideScrollbars = false;
            this.bitmapPreprocessorClassDefinition.Highlighting = null;
            this.bitmapPreprocessorClassDefinition.Location = new System.Drawing.Point(1, 78);
            this.bitmapPreprocessorClassDefinition.Margin = new System.Windows.Forms.Padding(1);
            this.bitmapPreprocessorClassDefinition.Name = "bitmapPreprocessorClassDefinition";
            this.bitmapPreprocessorClassDefinition.ShowVRuler = false;
            this.bitmapPreprocessorClassDefinition.Size = new System.Drawing.Size(759, 200);
            this.bitmapPreprocessorClassDefinition.TabIndex = 78;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 77;
            this.label9.Text = "Preprocess:";
            // 
            // autoDeskewThreshold
            // 
            this.autoDeskewThreshold.Location = new System.Drawing.Point(412, 12);
            this.autoDeskewThreshold.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.autoDeskewThreshold.Name = "autoDeskewThreshold";
            this.autoDeskewThreshold.Size = new System.Drawing.Size(52, 20);
            this.autoDeskewThreshold.TabIndex = 83;
            this.autoDeskewThreshold.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // autoDeskew
            // 
            this.autoDeskew.AutoSize = true;
            this.autoDeskew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.autoDeskew.Location = new System.Drawing.Point(249, 15);
            this.autoDeskew.Name = "autoDeskew";
            this.autoDeskew.Size = new System.Drawing.Size(15, 14);
            this.autoDeskew.TabIndex = 81;
            this.autoDeskew.UseVisualStyleBackColor = true;
            // 
            // pageRotation
            // 
            this.pageRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pageRotation.FormattingEnabled = true;
            this.pageRotation.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.pageRotation.Location = new System.Drawing.Point(93, 11);
            this.pageRotation.Name = "pageRotation";
            this.pageRotation.Size = new System.Drawing.Size(52, 21);
            this.pageRotation.TabIndex = 80;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(169, 15);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(74, 13);
            this.label21.TabIndex = 84;
            this.label21.Text = "Auto-Deskew:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(289, 15);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(124, 13);
            this.label14.TabIndex = 82;
            this.label14.Text = "Auto-Deskew Threshold:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 79;
            this.label1.Text = "Rotate Pages:";
            // 
            // PreprocessBitmap
            // 
            this.PreprocessBitmap.AutoSize = true;
            this.PreprocessBitmap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.PreprocessBitmap.Location = new System.Drawing.Point(93, 60);
            this.PreprocessBitmap.Name = "PreprocessBitmap";
            this.PreprocessBitmap.Size = new System.Drawing.Size(15, 14);
            this.PreprocessBitmap.TabIndex = 85;
            this.PreprocessBitmap.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(492, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "Detecting Scale Anchor:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "Preprocessing Code:";
            // 
            // DetectingScaleAnchor
            // 
            this.DetectingScaleAnchor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DetectingScaleAnchor.FormattingEnabled = true;
            this.DetectingScaleAnchor.Items.AddRange(new object[] {
            "",
            "↻ 90°",
            "↻ 180°",
            "↺ 90°",
            "Auto"});
            this.DetectingScaleAnchor.Location = new System.Drawing.Point(623, 11);
            this.DetectingScaleAnchor.Name = "DetectingScaleAnchor";
            this.DetectingScaleAnchor.Size = new System.Drawing.Size(52, 21);
            this.DetectingScaleAnchor.TabIndex = 89;
            // 
            // BitmapPreparationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 310);
            this.Controls.Add(this.DetectingScaleAnchor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PreprocessBitmap);
            this.Controls.Add(this.autoDeskewThreshold);
            this.Controls.Add(this.autoDeskew);
            this.Controls.Add(this.pageRotation);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bitmapPreprocessorClassDefinition);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "BitmapPreparationForm";
            this.Text = "Scanned Document Settings";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.autoDeskewThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bApply;
        private ICSharpCode.TextEditor.TextEditorControl bitmapPreprocessorClassDefinition;
        private System.Windows.Forms.NumericUpDown autoDeskewThreshold;
        private System.Windows.Forms.CheckBox autoDeskew;
        private System.Windows.Forms.ComboBox pageRotation;
        private System.Windows.Forms.Button bRemove;
        private System.Windows.Forms.Button bSaveDafault;
        private System.Windows.Forms.ComboBox defaultBitmapPreprocessorClassDefinitions;
        private System.Windows.Forms.CheckBox PreprocessBitmap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DetectingScaleAnchor;
    }
}