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
    public partial class MarkImageDataControl : MarkControl
    {
        public MarkImageDataControl()
        {
            InitializeComponent();
        }

        override public Template.Mark GetMark()
        {
            return Mark;
        }

        public Template.Mark.ImageData Mark
        {
            get
            {
                if (mark == null)
                    mark = new Template.Mark.ImageData();
                mark.FindBestImageMatch = findBestImageMatch.Checked;
                mark.BrightnessTolerance = (float)brightnessTolerance.Value;
                mark.DifferentPixelNumberTolerance = (float)differentPixelNumberTolerance.Value;
                return mark;
            }
            set
            {
                if (value == null)
                    value = new Template.Mark.ImageData();
                mark = value;
                findBestImageMatch.Checked = value.FindBestImageMatch;
                brightnessTolerance.Value = (decimal)value.BrightnessTolerance;
                differentPixelNumberTolerance.Value = (decimal)value.DifferentPixelNumberTolerance;
                if (value.ImageData_ != null)
                    picture.Image = value.ImageData_.GetImage();
                rectangle.Text = SerializationRoutines.Json.Serialize(value.Rectangle);
            }
        }
        Template.Mark.ImageData mark;

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