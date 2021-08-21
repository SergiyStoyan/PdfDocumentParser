//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((anchor == null || anchor.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (anchor == null)
                anchor = new Template.Anchor.OcrText();
            anchor.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
            anchor.PositionDeviation = (float)PositionDeviation.Value;
            anchor.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            anchor.OcrEntirePage = OcrEntirePage.Checked;
            return anchor;
        }

        protected override void initialize(DataGridViewRow row)
        {
            anchor = (Template.Anchor.OcrText)row.Tag;
            if (anchor == null)
                anchor = new Template.Anchor.OcrText();
            StringBuilder sb = new StringBuilder();
            foreach (var l in Page.GetLines(anchor.CharBoxs.Select(x => new Ocr.CharBox { Char = x.Char, R = x.Rectangle.GetSystemRectangleF() }), textAutoInsertSpace, null))
            {
                foreach (var cb in l.CharBoxs)
                    sb.Append(cb.Char);
                sb.Append("\r\n");
            }
            text.Text = sb.ToString();
            PositionDeviationIsAbsolute.Checked = anchor.PositionDeviationIsAbsolute;
            try
            {
                PositionDeviation.Value = (decimal)anchor.PositionDeviation;
            }
            catch { }

            SearchRectangleMargin.Value = anchor.SearchRectangleMargin;
            SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;

            OcrEntirePage.Checked = anchor.OcrEntirePage;
        }

        Template.Anchor.OcrText anchor;
    }
}