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
    public partial class FloatingAnchorOcrTextControl : UserControl
    {
        public FloatingAnchorOcrTextControl(DataGridViewRow row)
        {
            InitializeComponent();

            Row = row;
        }
        public readonly DataGridViewRow Row;

        public Template.FloatingAnchor.OcrTextValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                StringBuilder sb = new StringBuilder();
                foreach (var l in Ocr.GetLines(value.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() })))
                {
                    foreach (var cb in l.CharBoxes)
                        sb.Append(cb.Char);
                    sb.Append("\r\n");
                }
                text.Text = sb.ToString();
            }
        }
        Template.FloatingAnchor.OcrTextValue _value;
    }
}