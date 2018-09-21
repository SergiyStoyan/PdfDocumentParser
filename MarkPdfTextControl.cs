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
    public partial class MarkPdfTextControl : UserControl
    {
        public MarkPdfTextControl()
        {
            InitializeComponent();
        }

        public Template.Mark.PdfTextValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                    value = new Template.Mark.PdfTextValue();
                _value = value;
                text.Text = value.Text;
                rectangle.Text = SerializationRoutines.Json.Serialize(value.Rectangle);
            }
        }
        Template.Mark.PdfTextValue _value;
    }
}