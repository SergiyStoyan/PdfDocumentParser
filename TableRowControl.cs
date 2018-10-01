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
    public partial class TableRowControl : UserControl
    {
        public TableRowControl()
        {
            InitializeComponent();

            Leave += delegate (object sender, EventArgs e)
            {
                Row.Tag = getObject();
            };
        }
        public DataGridViewRow Row;
        //protected TemplateForm templateForm;

        public virtual void Initialize(DataGridViewRow row/*, TemplateForm templateForm*/)
        {
            Row = row;
            //this.templateForm = templateForm;
        }

        virtual protected object getObject()
        {
            throw new Exception("Not overrrided!");
        }
    }
}