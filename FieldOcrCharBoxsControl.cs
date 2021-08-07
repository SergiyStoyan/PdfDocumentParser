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
    public partial class FieldOcrCharBoxsControl : FieldControl
    {
        public FieldOcrCharBoxsControl(TextAutoInsertSpace textAutoInsertSpace, Template.Field.OcrSettings ocrSettings)
        {
            InitializeComponent();

            SpecialOcrSettings.CheckedChanged += delegate { synchronizeControls(); };
            ColumnOfTable.SelectedIndexChanged += delegate { synchronizeControls(); };
            synchronizeControls();

            TesseractPageSegMode.DataSource = Enum.GetValues(typeof(Tesseract.PageSegMode));
            TesseractPageSegMode.SelectedItem = ocrSettings.TesseractPageSegMode;
            SingleFieldFromFieldImage.Checked = ocrSettings.SingleFieldFromFieldImage;
            ColumnCellFromCellImage.Checked = ocrSettings.ColumnCellFromCellImage;
            //=ocrSettings.IgnoreCharsBiggerThan;

            this.textAutoInsertSpace = textAutoInsertSpace;
            this.ocrSettings = ocrSettings;
        }
        TextAutoInsertSpace textAutoInsertSpace;
        Template.Field.OcrSettings ocrSettings;

        void synchronizeControls()
        {
            gOcr.Visible = SpecialOcrSettings.Checked;
            SingleFieldFromFieldImage.Enabled = ColumnOfTable.SelectedIndex < 0;
            ColumnCellFromCellImage.Enabled = !SingleFieldFromFieldImage.Enabled;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrCharBoxs();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (SpecialOcrSettings.Checked)
            {
                if (field.OcrSettings == null)
                    field.OcrSettings = ocrSettings.CreateCloneByJson();
                field.OcrSettings.SingleFieldFromFieldImage = SingleFieldFromFieldImage.Checked;
                field.OcrSettings.ColumnCellFromCellImage = ColumnCellFromCellImage.Checked;
                field.OcrSettings.TesseractPageSegMode = (Tesseract.PageSegMode)TesseractPageSegMode.SelectedItem;
            }
            else
                field.OcrSettings = null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.OcrCharBoxs)row.Tag;
            if (field == null)
                field = new Template.Field.OcrCharBoxs();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialOcrSettings.Checked = field.OcrSettings != null;
            if (field.OcrSettings != null)
            {
                SingleFieldFromFieldImage.Checked = field.OcrSettings.SingleFieldFromFieldImage;
                ColumnCellFromCellImage.Checked = field.OcrSettings.ColumnCellFromCellImage;
                TesseractPageSegMode.SelectedItem = field.OcrSettings.TesseractPageSegMode;
                //=field.OcrSettings.IgnoreCharsBiggerThan;
            }

            if (value != null)
            {
                List<Page.Line<Ocr.CharBox>> cbss = Page.GetLines((List<Ocr.CharBox>)value, textAutoInsertSpace, (field.OcrSettings == null ? ocrSettings : field.OcrSettings).CharFilter);
                List<string> ls = new List<string>();
                foreach (var cbs in cbss)
                    ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                Value.Text = string.Join("\r\n", ls);
            }
        }

        Template.Field.OcrCharBoxs field;
    }
}
