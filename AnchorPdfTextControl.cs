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
    public partial class AnchorPdfTextControl : AnchorControl
    {
        public AnchorPdfTextControl(float textAutoInsertSpaceThreshold, string textAutoInsertSpaceSubstitute)
        {
            InitializeComponent();

            this.textAutoInsertSpaceThreshold = textAutoInsertSpaceThreshold;
            this.textAutoInsertSpaceSubstitute = textAutoInsertSpaceSubstitute;

            cSearchRectangleMargin.CheckedChanged += delegate
            {
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
                if (SearchRectangleMargin.Value >= 0)
                    return;
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((_object == null || _object.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }
        float textAutoInsertSpaceThreshold;
        string textAutoInsertSpaceSubstitute;

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.PdfText();
            _object.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
            _object.PositionDeviation = (float)PositionDeviation.Value;
            _object.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            return _object;

        }

        public override void Initialize(DataGridViewRow row, Action<DataGridViewRow> onLeft)
        {
            base.Initialize(row, onLeft);

            _object = (Template.Anchor.PdfText)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.PdfText();
            StringBuilder sb = new StringBuilder();
            foreach (var l in Pdf.RemoveDuplicatesAndGetLines(_object.CharBoxs.Select(x => new Pdf.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), textAutoInsertSpaceThreshold, textAutoInsertSpaceSubstitute))
            {
                foreach (var cb in l.CharBoxs)
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
            SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
        }

        Template.Anchor.PdfText _object;
    }
}