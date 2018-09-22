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
    public partial class FloatingAnchorImageDataControl : UserControl
    {
        public FloatingAnchorImageDataControl()
        {
            InitializeComponent();
        }

        public Template.FloatingAnchor.ImageData FloatingAnchor
        {
            get
            {
                if (floatingAnchor == null)
                    floatingAnchor = new Template.FloatingAnchor.ImageData();
                floatingAnchor.FindBestImageMatch = FindBestImageMatch.Checked;
                floatingAnchor.BrightnessTolerance = (float)BrightnessTolerance.Value;
                floatingAnchor.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                //_value.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                floatingAnchor.PositionDeviation = (float)PositionDeviation.Value;
                floatingAnchor.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return floatingAnchor;
            }
            set
            {
                if (value == null)
                    value = new Template.FloatingAnchor.ImageData();
                floatingAnchor = value;
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
                SearchRectangleMargin.Value = floatingAnchor.SearchRectangleMargin;
            }
        }
        Template.FloatingAnchor.ImageData floatingAnchor;

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