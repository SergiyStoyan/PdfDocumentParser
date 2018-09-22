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
        public FloatingAnchorPdfTextControl()
        {
            InitializeComponent();
        }

        public Template.FloatingAnchor.PdfText FloatingAnchor
        {
            get
            {
                if (floatingAnchor == null)
                    floatingAnchor = new Template.FloatingAnchor.PdfText();
                floatingAnchor.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                floatingAnchor.PositionDeviation = (float)PositionDeviation.Value;
                floatingAnchor.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return floatingAnchor;
            }
            set
            {
                if (value == null)
                    value = new Template.FloatingAnchor.PdfText();
                floatingAnchor = value;
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
        Template.FloatingAnchor.PdfText floatingAnchor;

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