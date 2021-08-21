//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Cliver.PdfDocumentParser
{
    public partial class FieldControl : UserControl
    {
        public FieldControl()
        {
            InitializeComponent();

            Leave += delegate (object sender, EventArgs e)
            {
                SetTagFromControl();
            };
        }

        public void Initialize(DataGridViewRow row, object value, Template template/*, IEnumerable<Template.Field> fields*//*, TemplateForm templateForm*/, Action<DataGridViewRow> onLeft)
        {
            Row = row;
            this.template = template;
            //this.fields = fields;
            //this.templateForm = templateForm;
            this.onLeft = onLeft;
            initialize(row, value);
        }
        public DataGridViewRow Row;
        //protected TemplateForm templateForm;
        protected Action<DataGridViewRow> onLeft = null;
        //protected IEnumerable<Template.Field> fields = null;
        protected Template template;

        virtual protected void initialize(DataGridViewRow row, object value/*, TemplateForm templateForm*/)
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