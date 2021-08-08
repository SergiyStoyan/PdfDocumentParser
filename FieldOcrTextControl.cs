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
    public partial class FieldOcrTextControl : FieldControl
    {
        public FieldOcrTextControl()
        {
            InitializeComponent();

            SpecialOcrSettings.CheckedChanged += delegate { synchronizeControls(); };
            ColumnOfTable.SelectedIndexChanged += delegate { synchronizeControls(); };
            synchronizeControls();

            TesseractPageSegMode.DataSource = Enum.GetValues(typeof(Tesseract.PageSegMode));
        }

        void synchronizeControls()
        {
            gOcr.Visible = SpecialOcrSettings.Checked;
            SingleFieldFromFieldImage.Enabled = ColumnOfTable.SelectedIndex < 0;
            ColumnCellFromCellImage.Enabled = !SingleFieldFromFieldImage.Enabled;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrText();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (SpecialOcrSettings.Checked)
            {
                field.AdjustLineBorders = AdjustLineBorders.Checked;
                field.SingleFieldFromFieldImage = SingleFieldFromFieldImage.Checked;
                field.ColumnCellFromCellImage = ColumnCellFromCellImage.Checked;
                field.TesseractPageSegMode = (Tesseract.PageSegMode)TesseractPageSegMode.SelectedItem;
            }
            else
            {
                field.AdjustLineBorders = null;
                field.SingleFieldFromFieldImage = null;
                field.ColumnCellFromCellImage = null;
                field.TesseractPageSegMode = null;
            }
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.OcrText)row.Tag;
            if (field == null)
                field = new Template.Field.OcrText();

            List<string> fieldNames = template.Fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialOcrSettings.Checked = field.AdjustLineBorders != null || field.SingleFieldFromFieldImage != null || field.ColumnCellFromCellImage != null || field.TesseractPageSegMode != null;
            AdjustLineBorders.Checked = field.AdjustLineBorders ?? template.AdjustLineBorders;
            SingleFieldFromFieldImage.Checked = field.SingleFieldFromFieldImage ?? template.SingleFieldFromFieldImage;
            ColumnCellFromCellImage.Checked = field.ColumnCellFromCellImage ?? template.ColumnCellFromCellImage;
            TesseractPageSegMode.SelectedItem = field.TesseractPageSegMode ?? template.TesseractPageSegMode;

            if (value != null)
                Value.Text = Page.NormalizeText((string)value);
        }

        Template.Field.OcrText field;
    }
}
