using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class AnchorControl : TableRowControl
    {
        public AnchorControl()
        {
            InitializeComponent();

            Leave += delegate (object sender, EventArgs e)
            {
                templateForm.setAnchorRow(Row, GetAnchor());
            };
        }

        virtual public Template.Anchor GetAnchor()
        {
            throw new Exception("Not overrrided!");
        }
    }
}
