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
    public partial class AnchorOcrTextControl : AnchorControl
    {
        public AnchorOcrTextControl()
        {
            InitializeComponent();

            cSearchRectangleMargin.CheckedChanged += delegate
            {
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((_object == null || _object.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1; SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            };
        }

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.OcrText();
            _object.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
            _object.PositionDeviation = (float)PositionDeviation.Value;
            _object.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
            return _object;
        }

        public override void Initialize(DataGridViewRow row)
        {
            base.Initialize(row);

            _object = (Template.Anchor.OcrText)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.OcrText();
            StringBuilder sb = new StringBuilder();
            foreach (var l in Ocr.GetLines(_object.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() })))
            {
                foreach (var cb in l.CharBoxes)
                    sb.Append(cb.Char);
                sb.Append("\r\n");
            }
            text.Text = sb.ToString();
            PositionDeviationIsAbsolute.Checked = _object.PositionDeviationIsAbsolute;
            try
            {
                PositionDeviation.Value = (decimal)_object.PositionDeviation;
            }
            catch { }
            SearchRectangleMargin.Value = _object.SearchRectangleMargin;

            cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
            SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
        }

        Template.Anchor.OcrText _object;
    }
}