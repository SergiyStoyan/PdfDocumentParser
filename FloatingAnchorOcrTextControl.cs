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
    public partial class FloatingAnchorOcrTextControl : UserControl
    {
        public FloatingAnchorOcrTextControl()
        {
            InitializeComponent();
        }

        public Template.FloatingAnchor.OcrTextValue Value
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
                    value = new Template.FloatingAnchor.OcrTextValue();
                _value = value;
                StringBuilder sb = new StringBuilder();
                foreach (var l in Ocr.GetLines(value.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() })))
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
                SearchRectangleMargin.Value = _value.SearchRectangleMargin;
            }
        }
        Template.FloatingAnchor.OcrTextValue _value;
    }
}