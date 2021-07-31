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
    public partial class FieldOcrTextLinesControl : FieldControl
    {
        public FieldOcrTextLinesControl()
        {
            InitializeComponent();

            SpecialOcrSettings.CheckedChanged += delegate { synchronizeControls(); };
            synchronizeControls();

            TesseractPageSegMode.DataSource = Enum.GetValues(typeof(Tesseract.PageSegMode));
        }

        void synchronizeControls()
        {
            gOcr.Visible = SpecialOcrSettings.Checked;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrTextLines();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (SpecialOcrSettings.Checked)
            {
                if (field.OcrSettings == null)
                    field.OcrSettings = new Template.Field.OcrSettings();
                field.OcrSettings.SingleFieldFromFieldImage = SingleFieldFromFieldImage.Checked;
                field.OcrSettings.ColumnFieldFromFieldImage = ColumnFieldFromFieldImage.Checked;
                field.OcrSettings.TesseractPageSegMode = (Tesseract.PageSegMode)TesseractPageSegMode.SelectedItem;
            }
            else
                field.OcrSettings = null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.OcrTextLines)row.Tag;
            if (field == null)
                field = new Template.Field.OcrTextLines();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialOcrSettings.Checked = field.OcrSettings != null;
            if (field.OcrSettings != null)
            {
                SingleFieldFromFieldImage.Checked = field.OcrSettings.SingleFieldFromFieldImage;
                ColumnFieldFromFieldImage.Checked = field.OcrSettings.ColumnFieldFromFieldImage;
                TesseractPageSegMode.SelectedItem = field.OcrSettings.TesseractPageSegMode;
            }

            if (value != null)
            {
                List<string> vs = (List<string>)value;
                Page.NormalizeText(vs);
                Value.Text = string.Join("\r\n", vs);
            }
        }

        Template.Field.OcrTextLines field;
    }
}
