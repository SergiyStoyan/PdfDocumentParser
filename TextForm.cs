using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class TextForm : Form
    {
        public TextForm(string caption, string t, bool html)
        {
            InitializeComponent();
            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": " + caption;// Application.ProductName;

            if (html)
            {
                browser.DocumentText = t;
                browser.BringToFront();
            }
            else
            {
                textBox.Text = t;
                textBox.BringToFront();
            }
        }
    }
}
