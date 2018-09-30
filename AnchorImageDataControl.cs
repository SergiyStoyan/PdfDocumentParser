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
    public partial class AnchorImageDataControl : UserControl
    {
        public AnchorImageDataControl()
        {
            InitializeComponent();

            LostFocus += delegate
            {

            };
            Leave+= delegate
            {

            };

            cSearchRectangleMargin.CheckedChanged += delegate { SearchRectangleMargin.Value = cSearchRectangleMargin.Checked ? 100 : -1; SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked; };
        }

        public Template.Anchor.ImageData Anchor
        {
            get
            {
                if (anchor == null)
                    anchor = new Template.Anchor.ImageData();
                anchor.FindBestImageMatch = FindBestImageMatch.Checked;
                anchor.BrightnessTolerance = (float)BrightnessTolerance.Value;
                anchor.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                //_value.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                anchor.PositionDeviation = (float)PositionDeviation.Value;
                anchor.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return anchor;
            }
            set
            {
                if (value == null)
                    value = new Template.Anchor.ImageData();
                anchor = value;
                FindBestImageMatch.Checked = value.FindBestImageMatch;
                BrightnessTolerance.Value = (decimal)value.BrightnessTolerance;
                DifferentPixelNumberTolerance.Value = (decimal)value.DifferentPixelNumberTolerance;
                if (value.ImageBoxs != null && value.ImageBoxs.Count > 0)
                    picture.Image = value.ImageBoxs[0].ImageData.GetImage();
                try
                {
                    PositionDeviation.Value = (decimal)value.PositionDeviation;
                }
                catch { }
                SearchRectangleMargin.Value = anchor.SearchRectangleMargin;

                cSearchRectangleMargin.Checked = SearchRectangleMargin.Value >= 0;
                SearchRectangleMargin.Enabled = cSearchRectangleMargin.Checked;
            }
        }
        Template.Anchor.ImageData anchor;

        //public bool FindBestImageMatch
        //{
        //    get
        //    {
        //        return findBestImageMatch.Checked;
        //    }
        //    //set
        //    //{
        //    //    findBestImageMatch.Checked = value;
        //    //}
        //}

        //public float BrightnessTolerance
        //{
        //    get
        //    {
        //        return (float)brightnessTolerance.Value;
        //    }
        //    //set
        //    //{
        //    //    brightnessTolerance.Value = (decimal)value;
        //    //}
        //}

        //public float DifferentPixelNumberTolerance
        //{
        //    get
        //    {
        //        return (float)differentPixelNumberTolerance.Value;
        //    }
        //    //set
        //    //{
        //    //    differentPixelNumberTolerance.Value = (decimal)value;
        //    //}
        //}

        //public Image Image
        //{
        //    get
        //    {
        //        return picture.Image;
        //    }
        //    //set
        //    //{
        //    //    picture.Image = value;
        //    //}
        //}
    }
}