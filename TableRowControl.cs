using System;
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
        protected Action<DataGridViewRow> onLeft = null;

        public virtual void Initialize(DataGridViewRow row/*, TemplateForm templateForm*/, Action<DataGridViewRow> onLeft)
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
            onLeft?.Invoke(Row);
        }
    }
}