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
    public partial class FieldGeneralControl : FieldControl
    {
        public FieldGeneralControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            this.textAutoInsertSpace = textAutoInsertSpace;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field();
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
            field = (Template.Field)row.Tag;
            if (field == null)
                field = new Template.Field();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            Rectangle.Text = Serialization.Json.Serialize(field.Rectangle);

            switch (field.DefaultValueType)
            {
                case Template.Field.ValueTypes.PdfText:
                    Value.Text = Page.NormalizeText((string)value);
                    break;
                case Template.Field.ValueTypes.PdfTextLines:
                    {
                        List<string> vs = (List<string>)value;
                        Page.NormalizeText(vs);
                        Value.Text = string.Join("\r\n", vs);
                    }
                    break;
                case Template.Field.ValueTypes.PdfCharBoxs:
                    {
                        List<Page.Line<Pdf.CharBox>> cbss = Page.GetLines((List<Pdf.CharBox>)value, textAutoInsertSpace);
                        List<string> ls = new List<string>();
                        foreach (var cbs in cbss)
                            ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                        Value.Text = string.Join("\r\n", ls);
                    }
                    break;
                case Template.Field.ValueTypes.OcrText:
                    Value.Text = Page.NormalizeText((string)value);
                    break;
                case Template.Field.ValueTypes.OcrTextLines:
                    {
                        List<string> vs = (List<string>)value;
                        Page.NormalizeText(vs);
                        Value.Text = string.Join("\r\n", vs);
                    }
                    break;
                case Template.Field.ValueTypes.OcrCharBoxs:
                    {
                        List<Page.Line<Ocr.CharBox>> cbss = Page.GetLines((List<Ocr.CharBox>)value, textAutoInsertSpace);
                        List<string> ls = new List<string>();
                        foreach (var cbs in cbss)
                            ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                        Value.Text = string.Join("\r\n", ls);
                    }
                    break;
                case Template.Field.ValueTypes.Image:
                    break;
                case Template.Field.ValueTypes.OcrTextLineImages:
                    break;
                default:
                    throw new Exception("Unknown option: " + field.DefaultValueType);
            }
        }

        Template.Field field;
    }
}
