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
    public partial class FieldControl : TableRowControl
    {
        public FieldControl()
        {
            InitializeComponent();
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            return field;
        }

        //virtual public void SetValue(object value)
        //{
        //    string
        //    Value.Text = value;
        //}

        public override void Initialize(DataGridViewRow row, Action<DataGridViewRow> onLeft)
        {
            base.Initialize(row, onLeft);

            field = (Template.Field)row.Tag;
            if (field == null)
                field = new Template.Field();
            ColumnOfTable.SelectedItem = field.ColumnOfTable;
            Rectangle.Text = Serialization.Json.Serialize(field.Rectangle);

            switch (field.DefaultValueType)
            {
                case Template.Field.ValueTypes.PdfText:
                case Template.Field.ValueTypes.PdfTextLines:
                case Template.Field.ValueTypes.PdfCharBoxs:
                    Value.Text = (string)row.Cells["Value"].Value;
                    break;
                case Template.Field.ValueTypes.OcrText:
                case Template.Field.ValueTypes.OcrTextLines:
                case Template.Field.ValueTypes.OcrCharBoxs:
                    Value.Text = (string)row.Cells["Value"].Value;
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
