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

        public Template.FloatingAnchor.ImageDataValue Value
        {
            get
            {
                _value.FindBestImageMatch = FindBestImageMatch.Checked;
                _value.BrightnessTolerance = (float)BrightnessTolerance.Value;
                _value.DifferentPixelNumberTolerance = (float)DifferentPixelNumberTolerance.Value;
                //_value.PositionDeviationIsAbsolute = PositionDeviationIsAbsolute.Checked;
                _value.PositionDeviation = (float)PositionDeviation.Value;
                _value.SearchRectangleMargin = (int)SearchRectangleMargin.Value;
                return _value;
            }
            set
            {
                if (value == null)
                    value = new Template.FloatingAnchor.ImageDataValue();
                _value = value;
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
                SearchRectangleMargin.Value = _value.SearchRectangleMargin;
            }
        }
        Template.FloatingAnchor.ImageDataValue _value;

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