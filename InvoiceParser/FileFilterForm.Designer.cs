namespace Cliver.InvoiceParser
{
    partial class FileFilterForm
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
            this.files = new ReadOnlyListBox();
            this.SuspendLayout();
            // 
            // files
            // 
            this.files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.files.FormattingEnabled = true;
            this.files.HorizontalScrollbar = true;
            this.files.IntegralHeight = false;
            this.files.Location = new System.Drawing.Point(0, 0);
            this.files.Name = "files";
            this.files.ReadOnly = false;
            this.files.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.files.Size = new System.Drawing.Size(284, 262);
            this.files.TabIndex = 0;
            // 
            // FileFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.files);
            this.Name = "FileFilterForm";
            this.Text = "TestFileFilterForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ReadOnlyListBox files;
    }
}