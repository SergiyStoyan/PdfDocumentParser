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
    public partial class FloatingAnchorPdfTextControl : UserControl
    {
        public FloatingAnchorPdfTextControl(Template.FloatingAnchor.PdfTextValue value)
        {
            InitializeComponent();

            Value = value;
        }

        public Template.FloatingAnchor.PdfTextValue Value
        {
            get
            {
                _value.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                _value.PositionDeviation = (float)PositionDeviation.Value;
                _value.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return _value;
            }
            set
            {
                if (value == null)
                    value = new Template.FloatingAnchor.PdfTextValue();
                _value = value;
                StringBuilder sb = new StringBuilder();
                foreach (var l in Pdf.RemoveDuplicatesAndGetLines(value.CharBoxs.Select(x => new Pdf.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), true))
                {
                    foreach (var cb in l.CharBoxes)
                        sb.Append(cb.Char);
                    sb.Append("\r\n");
                }
                text.Text = sb.ToString();

                PositionDeviationIsAbsolute.Checked = value.PositionDeviationIsAbsolute;
                try
                {
                    PositionDeviation.Value = (decimal)value.PositionDeviation;
                }
                catch { }
                SearchRectangleMargin.Value = value.SearchRectangleMargin;
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