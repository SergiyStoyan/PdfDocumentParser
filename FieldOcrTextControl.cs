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
        public FieldOcrTextControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            SpecialOcrSettings.CheckedChanged += delegate { synchronizeControls(); };
            synchronizeControls();

            this.textAutoInsertSpace = textAutoInsertSpace;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        void synchronizeControls()
        {
            panel2.Visible = SpecialOcrSettings.Checked;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrText();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (field.OcrSettings == null)
                field.OcrSettings = new Template.Field.OcrSettings();
            field.OcrSettings.SingleFieldFromFieldImage = OcrSingleFieldFromFieldImage.Checked;
            field.OcrSettings.ColumnFieldFromFieldImage = OcrColumnFieldFromFieldImage.Checked;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
               field = (Template.Field.OcrText)row.Tag;
            if (field == null)
                field = new Template.Field.OcrText();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialOcrSettings.Checked = field.OcrSettings != null;
            if (field.OcrSettings != null)
            {
                OcrSingleFieldFromFieldImage.Checked = field.OcrSettings.SingleFieldFromFieldImage;
                OcrColumnFieldFromFieldImage.Checked = field.OcrSettings.ColumnFieldFromFieldImage;
            }

            if (value != null)
                Value.Text = Page.NormalizeText((string)value);
        }

        Template.Field.OcrText field;
    }
}
