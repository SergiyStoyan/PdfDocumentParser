//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class AnchorImageDataControl : AnchorControl
    {
        public AnchorImageDataControl(float imageScale)
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
                anchor = new Template.Anchor.ImageData();
            anchor.FindBestImageMatch = FindBestImageMatch.Checked;
            anchor.BrightnessTolerance = (float)BrightnessTolerance.Value;
            anchor.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
            anchor.SearchRectangleMargin = SearchRectangleMargin.Enabled ? (int)SearchRectangleMargin.Value : -1;
            return anchor;
        }

        protected override void initialize(DataGridViewRow row)
        {
            anchor = (Template.Anchor.ImageData)row.Tag;
            if (anchor == null)
                anchor = new Template.Anchor.ImageData();
            FindBestImageMatch.Checked = anchor.FindBestImageMatch;
            BrightnessTolerance.Value = (decimal)anchor.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)anchor.DifferentPixelNumberTolerance;
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

        Template.Anchor.ImageData anchor;
    }
}