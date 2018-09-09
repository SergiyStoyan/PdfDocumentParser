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
    public partial class MarkOcrTextControl : UserControl
    {
        public MarkOcrTextControl()
        {
            InitializeComponent();
        }

        public Template.Mark.OcrTextValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                text.Text = value.Text;
            }
        }
        Template.Mark.OcrTextValue _value;
    }
}
