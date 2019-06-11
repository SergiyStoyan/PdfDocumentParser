using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// Interaction logic for AnchorOcrTextControlW.xaml
    /// </summary>
    public partial class AnchorPdfTextControlW : TableRowControlW
    {
        public AnchorPdfTextControlW(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            this.textAutoInsertSpace = textAutoInsertSpace;

            RoutedEventHandler checkedHandler = delegate
            {
                SearchRectangleMargin.IsEnabled = cSearchRectangleMargin.IsChecked == true;
                if (SearchRectangleMargin.Value >= 0)
                    return;
                SearchRectangleMargin.Value = cSearchRectangleMargin.IsChecked == true ? ((_object == null || _object.ParentAnchorId != null) ? (int)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
            cSearchRectangleMargin.Checked += checkedHandler;
            cSearchRectangleMargin.Unchecked += checkedHandler;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.OcrText();
            _object.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.IsChecked == true;
            _object.PositionDeviation = (float)PositionDeviation.Value;
            _object.SearchRectangleMargin = SearchRectangleMargin.IsEnabled ? (int)SearchRectangleMargin.Value : -1;
            _object.OcrEntirePage = OcrEntirePage.IsChecked == true;
            return _object;
        }

        public override void Initialize(DataGridRow row, Action<DataGridRow> onLeft)
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
            PositionDeviationIsAbsolute.IsChecked = _object.PositionDeviationIsAbsolute;
            try
            {
                PositionDeviation.Value = (decimal)_object.PositionDeviation;
            }
            catch { }

            SearchRectangleMargin.Value = _object.SearchRectangleMargin;
            SearchRectangleMargin.IsEnabled = cSearchRectangleMargin.IsChecked == true;
            cSearchRectangleMargin.IsChecked = SearchRectangleMargin.Value >= 0;

            OcrEntirePage.IsChecked = _object.OcrEntirePage;
        }

        Template.Anchor.OcrText _object;
    }
}