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
    public partial class MarkImageDataControl : UserControl
    {
        public MarkImageDataControl()
        {
            InitializeComponent();
        }

        public Template.Mark.ImageDataValue Value
        {
            get
            {
                _value.FindBestImageMatch = findBestImageMatch.Checked;
                _value.BrightnessTolerance = (float)brightnessTolerance.Value;
                _value.DifferentPixelNumberTolerance = (float)differentPixelNumberTolerance.Value;
                return _value;
            }
            set
            {
                if (value == null)
                    value = new Template.Mark.ImageDataValue();
                _value = value;
                findBestImageMatch.Checked = value.FindBestImageMatch;
                brightnessTolerance.Value = (decimal)value.BrightnessTolerance;
                differentPixelNumberTolerance.Value = (decimal)value.DifferentPixelNumberTolerance;
                if (value.ImageData != null)
                    picture.Image = value.ImageData.GetImage();
                rectangle.Text = SerializationRoutines.Json.Serialize(value.Rectangle);
            }
        }
        Template.Mark.ImageDataValue _value;

        public bool FindBestImageMatch
        {
            get
            {
                return findBestImageMatch.Checked;
            }
            //set
            //{
            //    findBestImageMatch.Checked = value;
            //}
        }

        public float BrightnessTolerance
        {
            get
            {
                return (float)brightnessTolerance.Value;
            }
            //set
            //{
            //    brightnessTolerance.Value = (decimal)value;
            //}
        }

        public float DifferentPixelNumberTolerance
        {
            get
            {
                return (float)differentPixelNumberTolerance.Value;
            }
            //set
            //{
            //    differentPixelNumberTolerance.Value = (decimal)value;
            //}
        }

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