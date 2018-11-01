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
                SetTagFromControl();
            };
        }
        public DataGridViewRow Row;
        //protected TemplateForm templateForm;
        protected Action onLeft = null;

        public virtual void Initialize(DataGridViewRow row/*, TemplateForm templateForm*/, Action onLeft)
        {
            Row = row;
            //this.templateForm = templateForm;
            this.onLeft = onLeft;
        }

        virtual protected object getObject()
        {
            throw new Exception("Not overrrided!");
        }

        virtual public void SetTagFromControl()
        {
            Row.Tag = getObject();
            onLeft?.Invoke();
        }
    }
}