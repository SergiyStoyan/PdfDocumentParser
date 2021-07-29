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
        public FieldOcrCharBoxsControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            this.textAutoInsertSpace = textAutoInsertSpace;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.OcrCharBoxs();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            field.TextAutoInsertSpace = SpecialTextAutoInsertSpace.Checked ? templateForm.GetTextAutoInsertSpaceFromGUI() : null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            SpecialTextAutoInsertSpace.Enabled = false;

            field = (Template.Field.OcrCharBoxs)row.Tag;
            if (field == null)
                field = new Template.Field.OcrCharBoxs();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialOcrSettings.Checked = field.OcrSettings != null;
            SpecialOcrSettings.Checked = field.OcrSettings != null;

            if (value != null)
            {
                List<Page.Line<Ocr.CharBox>> cbss = Page.GetLines((List<Ocr.CharBox>)value, textAutoInsertSpace);
                List<string> ls = new List<string>();
                foreach (var cbs in cbss)
                    ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                Value.Text = string.Join("\r\n", ls);
            }
        }

        Template.Field.OcrCharBoxs field;
    }
}
