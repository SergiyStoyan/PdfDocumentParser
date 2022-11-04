//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
    public partial class FieldOcrTextLineImagesControl : FieldControl
    {
        public FieldOcrTextLineImagesControl(float pictureScale)
        {
            InitializeComponent();

            this.pictureScale = pictureScale;
        }
        float pictureScale;

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrTextLineImages();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            return field;
        }

        //virtual public void SetValue(object value)
        //{
        //    switch (field.DefaultValueType)
        //    {
        //        case Template.Field.ValueTypes.PdfText:
        //        case Template.Field.ValueTypes.PdfTextLines:
        //        case Template.Field.ValueTypes.PdfCharBoxs:
        //            Value.Text = (string)value;
        //            break;
        //        case Template.Field.ValueTypes.OcrText:
        //        case Template.Field.ValueTypes.OcrTextLines:
        //        case Template.Field.ValueTypes.OcrCharBoxs:
        //            Value.Text = (string)value;
        //            break;
        //        case Template.Field.ValueTypes.Image:
        //            break;
        //        case Template.Field.ValueTypes.OcrTextLineImages:
        //            break;
        //        default:
        //            throw new Exception("Unknown option: " + field.DefaultValueType);
        //    }
        //}

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.OcrTextLineImages)row.Tag;
            if (field == null)
                field = new Template.Field.OcrTextLineImages();

            List<string> fieldNames = template.Fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            if (value != null)
            {
                List<Bitmap> bs = (List<Bitmap>)value;
                for (int i = 0; i < bs.Count; i++)
                {
                    Bitmap b = bs[i];
                    if (pictureScale != 1)
                        b = Win.ImageRoutines.GetScaled(b, pictureScale);
                    images.Controls.Add(new PictureBox { Image = b, SizeMode = PictureBoxSizeMode.AutoSize });
                }
            }
        }

        Template.Field.OcrTextLineImages field;
    }
}
