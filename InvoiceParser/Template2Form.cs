using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliver.InvoiceParser
{
    public partial class Template2Form : Form
    {
        public Template2Form(Template t)
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName + ": additional properties of '" + t.Name + "'";

            template = t;

            Active.Checked = t.Active;
            Group.Text = t.Group;
            Comment.Text = t.Comment;
            OrderWeight.Value = (decimal)t.OrderWeight;
            DetectingTemplateLastPageNumber.Value = t.DetectingTemplateLastPageNumber;
            FileFilterRegex.Text = t.FileFilterRegex.ToString();
            CanShareFileWithAnotherTemplates.Checked = t.CanShareFileWithAnotherTemplates;
        }
        Template template;

        private void bTestFileFilterRegex_Click(object sender, EventArgs e)
        {

        }

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                template.Active = Active.Checked;
                template.Group = Group.Text;
                template.Comment = Comment.Text;
                template.OrderWeight = (float)OrderWeight.Value;
                template.DetectingTemplateLastPageNumber = (uint)DetectingTemplateLastPageNumber.Value;
                template.FileFilterRegex = new System.Text.RegularExpressions.Regex(FileFilterRegex.Text);
                template.CanShareFileWithAnotherTemplates = CanShareFileWithAnotherTemplates.Checked;

                Close();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Message.Error2(ex);
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
