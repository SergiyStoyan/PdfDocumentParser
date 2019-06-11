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
    /// Interaction logic for AnchorImageDataControlW.xaml
    /// </summary>
    public partial class AnchorImageDataControlW : TableRowControlW
    {
        public AnchorImageDataControlW(float imageScale)
        {
            InitializeComponent();

            this.imageScale = imageScale;

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
        float imageScale = 1;

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.ImageData();
            _object.FindBestImageMatch = FindBestImageMatch.IsChecked == true;
            _object.BrightnessTolerance = (float)BrightnessTolerance.Value;
            _object.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
            _object.SearchRectangleMargin = SearchRectangleMargin.IsEnabled ? (int)SearchRectangleMargin.Value : -1;
            return _object;
        }

        public override void Initialize(DataGridRow row, Action<DataGridRow> onLeft)
        {
            base.Initialize(row, onLeft);

            _object = (Template.Anchor.ImageData)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.ImageData();
            FindBestImageMatch.IsChecked = _object.FindBestImageMatch;
            BrightnessTolerance.Value = (decimal)_object.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)_object.DifferentPixelNumberTolerance;
            pictureBox.Source = null;
            if (_object.Image != null)
            {
                ImageSource i = _object.Image.GetImageSource();
                //pictureBox.Stretch = Stretch.Uniform;
                pictureBox.Stretch = Stretch.None;
                pictureBox.Width = (int)(i.Width * imageScale);
                pictureBox.Height = (int)(i.Height * imageScale);
                pictureBox.Source = i;
            }

            SearchRectangleMargin.Value = _object.SearchRectangleMargin;
            SearchRectangleMargin.IsEnabled = cSearchRectangleMargin.IsChecked == true;
            cSearchRectangleMargin.IsChecked = SearchRectangleMargin.Value >= 0;
        }

        Template.Anchor.ImageData _object;
    }
}