//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((anchor == null || anchor.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }
        float imageScale = 1;

        override protected object getObject()
        {
            if (anchor == null)
                anchor = new Template.Anchor.CvImage();
            anchor.FindBestImageMatch = FindBestImageMatch.Checked;
            anchor.Threshold = (float)Threshold.Value;
            anchor.ScaleDeviation = (float)ScaleDeviation.Value;
            anchor.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            return anchor;
        }

        protected override void initialize(DataGridViewRow row)
        {
            anchor = (Template.Anchor.CvImage)row.Tag;
            if (anchor == null)
                anchor = new Template.Anchor.CvImage();
            FindBestImageMatch.Checked = anchor.FindBestImageMatch;
            Threshold.Value = (decimal)anchor.Threshold;
            ScaleDeviation.Value = (decimal)anchor.ScaleDeviation;
            pictureBox.Image = null;
            if (anchor.Image != null)
            {
                System.Drawing.Image i = anchor.Image.GetBitmap();
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = (int)(i.Width * imageScale);
                pictureBox.Height = (int)(i.Height * imageScale);
                pictureBox.Image = i;
            }

            SearchRectangleMargin.Value = anchor.SearchRectangleMargin;
            SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
        }

        Template.Anchor.CvImage anchor;
    }
}