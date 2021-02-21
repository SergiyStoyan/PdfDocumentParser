namespace Cliver.SampleParser
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
            this.bReset = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lShowOutput = new System.Windows.Forms.LinkLabel();
            this.lShowInput = new System.Windows.Forms.LinkLabel();
            this.OutputFolder = new System.Windows.Forms.TextBox();
            this.bInputFolder = new System.Windows.Forms.Button();
            this.bOutputFolder = new System.Windows.Forms.Button();
            this.InputFolder = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(380, 3);
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
            this.bSave.Location = new System.Drawing.Point(461, 3);
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
            this.bCancel.Location = new System.Drawing.Point(542, 3);
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
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 88);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(620, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // lShowOutput
            // 
            this.lShowOutput.AutoSize = true;
            this.lShowOutput.LinkArea = new System.Windows.Forms.LinkArea(0, 13);
            this.lShowOutput.Location = new System.Drawing.Point(17, 51);
            this.lShowOutput.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lShowOutput.Name = "lShowOutput";
            this.lShowOutput.Size = new System.Drawing.Size(77, 17);
            this.lShowOutput.TabIndex = 110;
            this.lShowOutput.TabStop = true;
            this.lShowOutput.Text = "Output Folder:";
            this.lShowOutput.UseCompatibleTextRendering = true;
            // 
            // lShowInput
            // 
            this.lShowInput.AutoSize = true;
            this.lShowInput.LinkArea = new System.Windows.Forms.LinkArea(0, 12);
            this.lShowInput.Location = new System.Drawing.Point(17, 25);
            this.lShowInput.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lShowInput.Name = "lShowInput";
            this.lShowInput.Size = new System.Drawing.Size(68, 17);
            this.lShowInput.TabIndex = 109;
            this.lShowInput.TabStop = true;
            this.lShowInput.Text = "Input Folder:";
            this.lShowInput.UseCompatibleTextRendering = true;
            // 
            // OutputFolder
            // 
            this.OutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFolder.Location = new System.Drawing.Point(94, 47);
            this.OutputFolder.Name = "OutputFolder";
            this.OutputFolder.Size = new System.Drawing.Size(479, 20);
            this.OutputFolder.TabIndex = 107;
            // 
            // bInputFolder
            // 
            this.bInputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bInputFolder.Location = new System.Drawing.Point(578, 20);
            this.bInputFolder.Name = "bInputFolder";
            this.bInputFolder.Size = new System.Drawing.Size(24, 23);
            this.bInputFolder.TabIndex = 106;
            this.bInputFolder.Text = "...";
            this.bInputFolder.UseVisualStyleBackColor = true;
            this.bInputFolder.Click += new System.EventHandler(this.bInputFolder_Click);
            // 
            // bOutputFolder
            // 
            this.bOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOutputFolder.Location = new System.Drawing.Point(578, 45);
            this.bOutputFolder.Name = "bOutputFolder";
            this.bOutputFolder.Size = new System.Drawing.Size(24, 23);
            this.bOutputFolder.TabIndex = 108;
            this.bOutputFolder.Text = "...";
            this.bOutputFolder.UseVisualStyleBackColor = true;
            this.bOutputFolder.Click += new System.EventHandler(this.bOutputFolder_Click);
            // 
            // InputFolder
            // 
            this.InputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputFolder.Location = new System.Drawing.Point(94, 21);
            this.InputFolder.Name = "InputFolder";
            this.InputFolder.Size = new System.Drawing.Size(479, 20);
            this.InputFolder.TabIndex = 105;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 119);
            this.Controls.Add(this.lShowOutput);
            this.Controls.Add(this.lShowInput);
            this.Controls.Add(this.OutputFolder);
            this.Controls.Add(this.bInputFolder);
            this.Controls.Add(this.bOutputFolder);
            this.Controls.Add(this.InputFolder);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.LinkLabel lShowOutput;
        private System.Windows.Forms.LinkLabel lShowInput;
        private System.Windows.Forms.TextBox OutputFolder;
        private System.Windows.Forms.Button bInputFolder;
        private System.Windows.Forms.Button bOutputFolder;
        private System.Windows.Forms.TextBox InputFolder;
    }
}