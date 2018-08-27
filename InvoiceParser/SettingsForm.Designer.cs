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
            this.IgnoreHidddenFiles = new System.Windows.Forms.CheckBox();
            this.ReadInputFolderRecursively = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.UpdateTemplatesOnStart = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RemoteAccessToken = new System.Windows.Forms.TextBox();
            this.updateTemplates = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.LastDownloadedTemplatesTimestamp = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bReset
            // 
            this.bReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bReset.Location = new System.Drawing.Point(252, 3);
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
            this.bSave.Location = new System.Drawing.Point(333, 3);
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
            this.bCancel.Location = new System.Drawing.Point(414, 3);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 225);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(492, 31);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // IgnoreHidddenFiles
            // 
            this.IgnoreHidddenFiles.AutoSize = true;
            this.IgnoreHidddenFiles.Location = new System.Drawing.Point(16, 24);
            this.IgnoreHidddenFiles.Name = "IgnoreHidddenFiles";
            this.IgnoreHidddenFiles.Size = new System.Drawing.Size(123, 17);
            this.IgnoreHidddenFiles.TabIndex = 53;
            this.IgnoreHidddenFiles.Text = "Ignore Hiddden Files";
            this.IgnoreHidddenFiles.UseVisualStyleBackColor = true;
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
            this.groupBox2.Controls.Add(this.IgnoreHidddenFiles);
            this.groupBox2.Controls.Add(this.ReadInputFolderRecursively);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(467, 83);
            this.groupBox2.TabIndex = 58;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LastDownloadedTemplatesTimestamp);
            this.groupBox1.Controls.Add(this.UpdateTemplatesOnStart);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.RemoteAccessToken);
            this.groupBox1.Controls.Add(this.updateTemplates);
            this.groupBox1.Location = new System.Drawing.Point(13, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(467, 116);
            this.groupBox1.TabIndex = 59;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Templates";
            // 
            // UpdateTemplatesOnStart
            // 
            this.UpdateTemplatesOnStart.AutoSize = true;
            this.UpdateTemplatesOnStart.Location = new System.Drawing.Point(15, 81);
            this.UpdateTemplatesOnStart.Name = "UpdateTemplatesOnStart";
            this.UpdateTemplatesOnStart.Size = new System.Drawing.Size(155, 17);
            this.UpdateTemplatesOnStart.TabIndex = 58;
            this.UpdateTemplatesOnStart.Text = "Update Templates On Start";
            this.UpdateTemplatesOnStart.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "Access Token:";
            // 
            // RemoteAccessToken
            // 
            this.RemoteAccessToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoteAccessToken.Location = new System.Drawing.Point(116, 45);
            this.RemoteAccessToken.Name = "RemoteAccessToken";
            this.RemoteAccessToken.Size = new System.Drawing.Size(345, 20);
            this.RemoteAccessToken.TabIndex = 51;
            // 
            // updateTemplates
            // 
            this.updateTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateTemplates.Location = new System.Drawing.Point(386, 75);
            this.updateTemplates.Name = "updateTemplates";
            this.updateTemplates.Size = new System.Drawing.Size(75, 23);
            this.updateTemplates.TabIndex = 49;
            this.updateTemplates.Text = "Update";
            this.updateTemplates.UseVisualStyleBackColor = true;
            this.updateTemplates.Click += new System.EventHandler(this.updateTemplates_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Current Timestamp:";
            // 
            // LastDownloadedTemplatesTimestamp
            // 
            this.LastDownloadedTemplatesTimestamp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LastDownloadedTemplatesTimestamp.Location = new System.Drawing.Point(116, 19);
            this.LastDownloadedTemplatesTimestamp.Name = "LastDownloadedTemplatesTimestamp";
            this.LastDownloadedTemplatesTimestamp.ReadOnly = true;
            this.LastDownloadedTemplatesTimestamp.Size = new System.Drawing.Size(345, 20);
            this.LastDownloadedTemplatesTimestamp.TabIndex = 59;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 256);
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
        private System.Windows.Forms.CheckBox IgnoreHidddenFiles;
        private System.Windows.Forms.CheckBox ReadInputFolderRecursively;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox RemoteAccessToken;
        private System.Windows.Forms.Button updateTemplates;
        private System.Windows.Forms.CheckBox UpdateTemplatesOnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LastDownloadedTemplatesTimestamp;
    }
}