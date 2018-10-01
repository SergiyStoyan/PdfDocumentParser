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
    public partial class MarkPdfTextControl : MarkControl
    {
        public MarkPdfTextControl()
        {
            InitializeComponent();
        }

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Mark.PdfText();
            return _object;
        }

        public override void Initialize(DataGridViewRow row)
        {
            base.Initialize(row);

            _object = (Template.Mark.PdfText)row.Tag;
            if (_object == null)
                _object = new Template.Mark.PdfText();
            text.Text = _object.Text;
            rectangle.Text = SerializationRoutines.Json.Serialize(_object.Rectangle);
        }

        Template.Mark.PdfText _object;
    }
}