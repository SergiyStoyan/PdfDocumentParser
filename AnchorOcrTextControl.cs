//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class AnchorOcrTextControl : AnchorControl
    {
        public AnchorOcrTextControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            this.textAutoInsertSpace = textAutoInsertSpace;

            cSearchRectangleMargin.CheckedChanged += delegate
            {
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
                if (SearchRectangleMargin.Value >= 0)
                    return;
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((_object == null || _object.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.OcrText();
            _object.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
            _object.PositionDeviation = (float)PositionDeviation.Value;
            _object.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            _object.OcrEntirePage = OcrEntirePage.Checked;
            return _object;
        }

        public override void Initialize(DataGridViewRow row, Action<DataGridViewRow> onLeft)
        {
            base.Initialize(row, onLeft);

            _object = (Template.Anchor.OcrText)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.OcrText();
            StringBuilder sb = new StringBuilder();
            foreach (var l in Ocr.GetLines(_object.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), textAutoInsertSpace))
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

            OcrEntirePage.Checked = _object.OcrEntirePage;
        }

        Template.Anchor.OcrText _object;
    }
}