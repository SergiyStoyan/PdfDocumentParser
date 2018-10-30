namespace Cliver.InvoiceParser
{
    partial class Template2Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.Comment = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Active = new System.Windows.Forms.CheckBox();
            this.Group = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OrderWeight = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.DetectingTemplateLastPageNumber = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.FileFilterRegex = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bTestFileFilterRegex = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SharedFileTemplateNamesRegex = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrderWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetectingTemplateLastPageNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Comment:";
            // 
            // Comment
            // 
            this.Comment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Comment.Location = new System.Drawing.Point(72, 69);
            this.Comment.Name = "Comment";
            this.Comment.Size = new System.Drawing.Size(388, 20);
            this.Comment.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 60;
            this.label2.Text = "Active:";
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.Location = new System.Drawing.Point(397, 3);
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
            this.bOK.Location = new System.Drawing.Point(316, 3);
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
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 227);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(475, 31);
            this.flowLayoutPanel1.TabIndex = 60;
            // 
            // Active
            // 
            this.Active.AutoSize = true;
            this.Active.Location = new System.Drawing.Point(72, 12);
            this.Active.Name = "Active";
            this.Active.Size = new System.Drawing.Size(15, 14);
            this.Active.TabIndex = 53;
            this.Active.UseVisualStyleBackColor = true;
            // 
            // Group
            // 
            this.Group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Group.Location = new System.Drawing.Point(72, 39);
            this.Group.Name = "Group";
            this.Group.Size = new System.Drawing.Size(388, 20);
            this.Group.TabIndex = 64;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "Group:";
            // 
            // OrderWeight
            // 
            this.OrderWeight.DecimalPlaces = 1;
            this.OrderWeight.Location = new System.Drawing.Point(72, 100);
            this.OrderWeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.OrderWeight.Name = "OrderWeight";
            this.OrderWeight.Size = new System.Drawing.Size(63, 20);
            this.OrderWeight.TabIndex = 66;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 65;
            this.label7.Text = "Order:";
            // 
            // DetectingTemplateLastPageNumber
            // 
            this.DetectingTemplateLastPageNumber.Location = new System.Drawing.Point(272, 130);
            this.DetectingTemplateLastPageNumber.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.DetectingTemplateLastPageNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DetectingTemplateLastPageNumber.Name = "DetectingTemplateLastPageNumber";
            this.DetectingTemplateLastPageNumber.Size = new System.Drawing.Size(63, 20);
            this.DetectingTemplateLastPageNumber.TabIndex = 68;
            this.DetectingTemplateLastPageNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(251, 13);
            this.label4.TabIndex = 67;
            this.label4.Text = "Last Page Number In PDF File To Detect Template:";
            // 
            // FileFilterRegex
            // 
            this.FileFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileFilterRegex.Location = new System.Drawing.Point(72, 159);
            this.FileFilterRegex.Name = "FileFilterRegex";
            this.FileFilterRegex.Size = new System.Drawing.Size(335, 20);
            this.FileFilterRegex.TabIndex = 69;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "File Filter:";
            // 
            // bTestFileFilterRegex
            // 
            this.bTestFileFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bTestFileFilterRegex.Location = new System.Drawing.Point(413, 156);
            this.bTestFileFilterRegex.Name = "bTestFileFilterRegex";
            this.bTestFileFilterRegex.Size = new System.Drawing.Size(47, 23);
            this.bTestFileFilterRegex.TabIndex = 70;
            this.bTestFileFilterRegex.Text = "Test";
            this.bTestFileFilterRegex.UseVisualStyleBackColor = true;
            this.bTestFileFilterRegex.Click += new System.EventHandler(this.bTestFileFilterRegex_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 13);
            this.label6.TabIndex = 73;
            this.label6.Text = "Shared File Templates Regex:";
            // 
            // SharedFileTemplateNamesRegex
            // 
            this.SharedFileTemplateNamesRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SharedFileTemplateNamesRegex.Location = new System.Drawing.Point(167, 189);
            this.SharedFileTemplateNamesRegex.Name = "SharedFileTemplateNamesRegex";
            this.SharedFileTemplateNamesRegex.Size = new System.Drawing.Size(293, 20);
            this.SharedFileTemplateNamesRegex.TabIndex = 74;
            // 
            // Template2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 258);
            this.Controls.Add(this.SharedFileTemplateNamesRegex);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.FileFilterRegex);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.bTestFileFilterRegex);
            this.Controls.Add(this.DetectingTemplateLastPageNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.OrderWeight);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Active);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Group);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.Comment);
            this.Controls.Add(this.label1);
            this.Name = "Template2Form";
            this.Text = "Template2Form";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OrderWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetectingTemplateLastPageNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Comment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox Active;
        private System.Windows.Forms.TextBox Group;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown OrderWeight;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown DetectingTemplateLastPageNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox FileFilterRegex;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bTestFileFilterRegex;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox SharedFileTemplateNamesRegex;
    }
}