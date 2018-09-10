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
            set
            {
                _value = value;
                if (value == null)
                    return;
                text.Text = value.Text;
            }
        }
        Template.Mark.PdfTextValue _value;
    }
}
