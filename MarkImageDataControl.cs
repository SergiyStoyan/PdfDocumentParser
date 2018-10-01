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

        override protected object getObject()
        {
            if (_object == null)
                _object = new Template.Mark.ImageData();
            _object.FindBestImageMatch = findBestImageMatch.Checked;
            _object.BrightnessTolerance = (float)brightnessTolerance.Value;
            _object.DifferentPixelNumberTolerance = (float)differentPixelNumberTolerance.Value;
            return _object;
        }

        public override void Initialize(DataGridViewRow row)
        {
            base.Initialize(row);

            _object = (Template.Mark.ImageData)row.Tag;
            if (_object == null)
                _object = new Template.Mark.ImageData();
            findBestImageMatch.Checked = _object.FindBestImageMatch;
            brightnessTolerance.Value = (decimal)_object.BrightnessTolerance;
            differentPixelNumberTolerance.Value = (decimal)_object.DifferentPixelNumberTolerance;
            if (_object.ImageData_ != null)
                picture.Image = _object.ImageData_.GetImage();
            rectangle.Text = SerializationRoutines.Json.Serialize(_object.Rectangle);
        }

        Template.Mark.ImageData _object;
    }
}