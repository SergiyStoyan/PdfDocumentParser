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
    public partial class FloatingAnchorPdfTextControl : UserControl
    {
        public FloatingAnchorPdfTextControl(DataGridViewRow row)
        {
            InitializeComponent();

            Row = row;
        }
        public readonly DataGridViewRow Row;

        public Template.FloatingAnchor.PdfTextValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                   StringBuilder sb = new StringBuilder();
                foreach (var l in Pdf.RemoveDuplicatesAndGetLines(value.CharBoxs.Select(x => new Pdf.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() })))
                {
                    foreach (var cb in l.CharBoxes)
                        sb.Append(cb.Char);
                    sb.Append("\r\n");
                }
                text.Text = sb.ToString();
            }
        }
        Template.FloatingAnchor.PdfTextValue _value;

        //public bool Text
        //{
        //    get
        //    {
        //        return findBestImageMatch.Checked;
        //    }
        //    set
        //    {
        //        findBestImageMatch.Checked = value;
        //    }
        //}
    }
}
