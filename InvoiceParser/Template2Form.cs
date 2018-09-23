//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Cliver.InvoiceParser
{
    public partial class Template2Form : Form
    {
        public Template2Form(Template2 t)
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName + ": additional properties of '" + t.Template.Name + "'";

            template2 = t;

            Active.Checked = t.Active;
            Group.Text = t.Group;
            Comment.Text = t.Comment;
            OrderWeight.Value = (decimal)t.OrderWeight;
            DetectingTemplateLastPageNumber.Value = t.DetectingTemplateLastPageNumber;
            if (t.FileFilterRegex != null)
                FileFilterRegex.Text = SerializationRoutines.Json.Serialize(t.FileFilterRegex);
            else
                FileFilterRegex.Text = "";
            CanShareFileWithAnotherTemplates.Checked = t.CanShareFileWithAnotherTemplates;
        }
        Template2 template2;

        private void bTestFileFilterRegex_Click(object sender, EventArgs e)
        {
            try
            {
                string d = string.IsNullOrWhiteSpace(template2.Template.Editor.TestFile) ? Settings.General.InputFolder : PathRoutines.GetDirFromPath(template2.Template.Editor.TestFile);
                FileFilterForm f = new FileFilterForm(d, SerializationRoutines.Json.Deserialize<Regex>(FileFilterRegex.Text));
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                template2.Active = Active.Checked;
                template2.Group = Group.Text;
                template2.Comment = Comment.Text;
                template2.OrderWeight = (float)OrderWeight.Value;
                template2.DetectingTemplateLastPageNumber = (uint)DetectingTemplateLastPageNumber.Value;
                template2.FileFilterRegex = SerializationRoutines.Json.Deserialize<Regex>(FileFilterRegex.Text);
                template2.CanShareFileWithAnotherTemplates = CanShareFileWithAnotherTemplates.Checked;

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
