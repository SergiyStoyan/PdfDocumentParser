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
        public MarkPdfTextControl(Template.Mark.PdfTextValue value, Template.RectangleF rectangle)
        {
            InitializeComponent();

            Value = value;
            this.rectangle.Text = SerializationRoutines.Json.Serialize(rectangle);
        }

        public Template.Mark.PdfTextValue Value
        {
            get
            {
                return _value;
            }
            private set
            {
                if (value == null)
                    value = new Template.Mark.PdfTextValue();
                text.Text = value.Text;
            }
        }
        Template.Mark.PdfTextValue _value;
    }
}