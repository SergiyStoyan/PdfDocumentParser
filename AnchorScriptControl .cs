//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
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
    public partial class AnchorScriptControl : AnchorControl
    {
        public AnchorScriptControl()
        {
            InitializeComponent();
        }

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.Script();

            _object.Expression = expression.Text;

            return _object;
        }

        public override void Initialize(DataGridViewRow row, Action<DataGridViewRow> onLeft)
        {
            base.Initialize(row, onLeft);

            _object = (Template.Anchor.Script)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.Script();

            expression.Text = _object.Expression;
        }

        Template.Anchor.Script _object;
    }
}