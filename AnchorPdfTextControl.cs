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
    public partial class AnchorPdfTextControl : UserControl
    {
        public AnchorPdfTextControl()
        {
            InitializeComponent();

            cSearchRectangleMargin.CheckedChanged += delegate { SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? 100 : -1; SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked; };
        }
        EventHandler cSearchRectangleMarginChecked;

        public Template.Anchor.PdfText Anchor
        {
            get
            {
                if (anchor == null)
                    anchor = new Template.Anchor.PdfText();
                anchor.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                anchor.PositionDeviation = (float)PositionDeviation.Value;
                anchor.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return anchor;
            }
            set
            {
                if (value == null)
                    value = new Template.Anchor.PdfText();
                anchor = value;
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

                cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            }
        }
        Template.Anchor.PdfText anchor;

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