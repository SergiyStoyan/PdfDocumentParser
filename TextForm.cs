using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class TextForm : Form
    {
        public TextForm(string caption, string t, bool edit)
        {
            InitializeComponent();
            Icon = Win.AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": " + caption;// Application.ProductName;

            textBox.ReadOnly = !edit;
            panel.Visible = edit;
            textBox.Text = t;
        }

        private void bSave_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public string Content
        {
            get
            {
                return textBox.Text;
            }
        }

        private void bCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
