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
    public partial class AnchorImageDataControl : AnchorControl
    {
        public AnchorImageDataControl()
        {
            InitializeComponent();

            cSearchRectangleMargin.CheckedChanged += delegate
            {
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
                if (SearchRectangleMargin.Value >= 0)
                    return;
                SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? ((_object == null || _object.ParentAnchorId != null) ? (decimal)Settings.Constants.CoordinateDeviationMargin : 100) : -1;
            };
        }

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Anchor.ImageData();
            _object.FindBestImageMatch = FindBestImageMatch.Checked;
            _object.BrightnessTolerance = (float)BrightnessTolerance.Value;
            _object.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
            _object.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
            _object.PositionDeviation = (float)PositionDeviation.Value;
            _object.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
            return _object;
        }

        public override void Initialize(DataGridViewRow row)
        {
            base.Initialize(row);

            _object = (Template.Anchor.ImageData)row.Tag;
            if (_object == null)
                _object = new Template.Anchor.ImageData();
            FindBestImageMatch.Checked = _object.FindBestImageMatch;
            BrightnessTolerance.Value = (decimal)_object.BrightnessTolerance;
            DifferentPixelNumberTolerance.Value = (decimal)_object.DifferentPixelNumberTolerance;
            pictures.Controls.Clear();
            if (_object.ImageBoxs != null)
                foreach (Template.Anchor.ImageData.ImageBox id in _object.ImageBoxs)
                {
                    PictureBox p = new PictureBox();
                    p.SizeMode = PictureBoxSizeMode.AutoSize;
                    p.Image = id.ImageData.GetImage();
                    pictures.Controls.Add(p);
                }
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

        Template.Anchor.ImageData _object;
    }
}