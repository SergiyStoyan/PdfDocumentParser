//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class AnchorControl : UserControl
    {
        public AnchorControl()
        {
            InitializeComponent();

            Leave += delegate (object sender, EventArgs e)
            {
                SetTagFromControl();
            };
        }

        public void Initialize(DataGridViewRow row/*, TemplateForm templateForm*/, Action<DataGridViewRow> onLeft)
        {
            Row = row;
            //this.templateForm = templateForm;
            this.onLeft = onLeft;
            initialize(row);
        }
        public DataGridViewRow Row;
        //protected TemplateForm templateForm;
        protected Action<DataGridViewRow> onLeft = null;

        protected virtual void initialize(DataGridViewRow row)
        {
            throw new Exception("Not overridden!");
        }

        virtual protected object getObject()
        {
            throw new Exception("Not overridden!");
        }

        virtual public void SetTagFromControl()
        {
            Row.Tag = getObject();
            onLeft?.Invoke(Row);
        }
    }
}
