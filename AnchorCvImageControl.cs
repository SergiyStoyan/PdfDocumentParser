//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class AnchorCvImageControl : AnchorControl
    {
        public AnchorCvImageControl(float imageScale)
        {
            InitializeComponent();

            this.imageScale = imageScale;

            cSearchRectangleMargin.CheckedChanged += delegate
            {
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
                if (SearchRectangleMargin.Value >= 0)
                    return;
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((_object == null || _object.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }
        float imageScale = 1;

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.CvImage();
            _object.Threshold = (float)Threshold.Value;
            _object.ScaleDeviation = (float)ScaleDeviation.Value;
            _object.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            return _object;
        }

        public override void Initialize(DataGridViewRow row, Action<DataGridViewRow> onLeft)
        {
            base.Initialize(row, onLeft);

            _object = (Template.Anchor.CvImage)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.CvImage();
            Threshold.Value = (decimal)_object.Threshold;
            ScaleDeviation.Value = (decimal)_object.ScaleDeviation;
            pictureBox.Image = null;
            if (_object.Image != null)
            {
                System.Drawing.Image i = _object.Image.GetImage();
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = (int)(i.Width * imageScale);
                pictureBox.Height = (int)(i.Height * imageScale);
                pictureBox.Image = i;
            }

            SearchRectangleMargin.Value = _object.SearchRectangleMargin;
            SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
        }

        Template.Anchor.CvImage _object;
    }
}