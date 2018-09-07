namespace Cliver.InvoiceParser
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
            this.IgnoreHiddenFiles = new System.Windows.Forms.CheckBox();
            this.ReadInputFolderRecursively = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bSynchronizedFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SynchronizationFolder = new System.Windows.Forms.TextBox();
            this.Synchronize = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(228, 3);
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
            this.bSave.Location = new System.Drawing.Point(309, 3);
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
            this.bCancel.Location = new System.Drawing.Point(390, 3);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 228);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(468, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // IgnoreHiddenFiles
            // 
            this.IgnoreHiddenFiles.AutoSize = true;
            this.IgnoreHiddenFiles.Location = new System.Drawing.Point(16, 24);
            this.IgnoreHiddenFiles.Name = "IgnoreHiddenFiles";
            this.IgnoreHiddenFiles.Size = new System.Drawing.Size(117, 17);
            this.IgnoreHiddenFiles.TabIndex = 53;
            this.IgnoreHiddenFiles.Text = "Ignore Hidden Files";
            this.IgnoreHiddenFiles.UseVisualStyleBackColor = true;
            // 
            // ReadInputFolderRecursively
            // 
            this.ReadInputFolderRecursively.AutoSize = true;
            this.ReadInputFolderRecursively.Location = new System.Drawing.Point(16, 50);
            this.ReadInputFolderRecursively.Name = "ReadInputFolderRecursively";
            this.ReadInputFolderRecursively.Size = new System.Drawing.Size(169, 17);
            this.ReadInputFolderRecursively.TabIndex = 57;
            this.ReadInputFolderRecursively.Text = "Read Input Folder Recursively";
            this.ReadInputFolderRecursively.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.IgnoreHiddenFiles);
            this.groupBox2.Controls.Add(this.ReadInputFolderRecursively);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(443, 80);
            this.groupBox2.TabIndex = 58;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bSynchronizedFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.SynchronizationFolder);
            this.groupBox1.Controls.Add(this.Synchronize);
            this.groupBox1.Location = new System.Drawing.Point(12, 105);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 114);
            this.groupBox1.TabIndex = 59;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remote Storage";
            // 
            // bSynchronizedFolder
            // 
            this.bSynchronizedFolder.Location = new System.Drawing.Point(135, 48);
            this.bSynchronizedFolder.Name = "bSynchronizedFolder";
            this.bSynchronizedFolder.Size = new System.Drawing.Size(26, 23);
            this.bSynchronizedFolder.TabIndex = 61;
            this.bSynchronizedFolder.Text = "...";
            this.bSynchronizedFolder.UseVisualStyleBackColor = true;
            this.bSynchronizedFolder.Click += new System.EventHandler(this.bSynchronizedFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Synchronization Folder:";
            // 
            // SynchronizationFolder
            // 
            this.SynchronizationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SynchronizationFolder.Location = new System.Drawing.Point(15, 74);
            this.SynchronizationFolder.Name = "SynchronizationFolder";
            this.SynchronizationFolder.Size = new System.Drawing.Size(422, 20);
            this.SynchronizationFolder.TabIndex = 59;
            // 
            // Synchronize
            // 
            this.Synchronize.AutoSize = true;
            this.Synchronize.Location = new System.Drawing.Point(15, 27);
            this.Synchronize.Name = "Synchronize";
            this.Synchronize.Size = new System.Drawing.Size(84, 17);
            this.Synchronize.TabIndex = 58;
            this.Synchronize.Text = "Synchronize";
            this.Synchronize.UseVisualStyleBackColor = true;
            this.Synchronize.CheckedChanged += new System.EventHandler(this.Synchronize_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 259);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox IgnoreHiddenFiles;
        private System.Windows.Forms.CheckBox ReadInputFolderRecursively;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Synchronize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SynchronizationFolder;
        private System.Windows.Forms.Button bSynchronizedFolder;
    }
}