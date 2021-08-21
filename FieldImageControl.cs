﻿//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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
    public partial class FieldImageControl : FieldControl
    {
        public FieldImageControl(float pictureScale)
        {
            InitializeComponent();

            this.pictureScale = pictureScale;
        }
        float pictureScale;

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.Image();
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
            field = (Template.Field.Image)row.Tag;
            if (field == null)
                field = new Template.Field.Image();

            List<string> fieldNames =template.Fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            Rectangle.Text = Serialization.Json.Serialize(field.Rectangle);

            if (value != null)
            {
                Bitmap b = (Bitmap)value;
                if (pictureScale != 1)
                    b = Win.ImageRoutines.GetScaled(b, pictureScale);
                Value.Image = b;
            }
        }

        Template.Field.Image field;
    }
}
