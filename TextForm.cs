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
    public partial class TextForm : Form
    {
        public TextForm(string t)
        {
            InitializeComponent();
            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;

            text.Text = t;
        }
    }
}
