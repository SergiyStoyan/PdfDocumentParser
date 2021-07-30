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
    public partial class FieldPdfTextControl : FieldControl
    {
        public FieldPdfTextControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            SpecialTextAutoInsertSpace.CheckedChanged += delegate { synchronizeControls(); };
            synchronizeControls();

            this.textAutoInsertSpace = textAutoInsertSpace;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        void synchronizeControls()
        {
            panel2.Visible = SpecialTextAutoInsertSpace.Checked;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.PdfText();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (field.TextAutoInsertSpace == null)
                field.TextAutoInsertSpace = new TextAutoInsertSpace();
            field.TextAutoInsertSpace = SpecialTextAutoInsertSpace.Checked ? new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpaceThreshold.Value, Representative = textAutoInsertSpaceRepresentative.Text, IgnoreSourceSpaces = textAutoInsertSpaceIgnoreSourceSpaces.Checked } : null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.PdfText)row.Tag;
            if (field == null)
                field = new Template.Field.PdfText();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialTextAutoInsertSpace.Checked = field.TextAutoInsertSpace != null;
            if (field.TextAutoInsertSpace != null)
            {
                textAutoInsertSpaceThreshold.Value = (decimal)field.TextAutoInsertSpace.Threshold;
                textAutoInsertSpaceRepresentative.Text = field.TextAutoInsertSpace.Representative;
                textAutoInsertSpaceIgnoreSourceSpaces.Checked = field.TextAutoInsertSpace.IgnoreSourceSpaces;
            }

            if (value != null)
                Value.Text = Page.NormalizeText((string)value);
        }

        Template.Field.PdfText field;
    }
}
