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
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    public partial class FieldPdfTextControl : FieldControl
    {
        public FieldPdfTextControl()
        {
            InitializeComponent();

            SpecialTextAutoInsertSpace.CheckedChanged += delegate { synchronizeControls(); };
            synchronizeControls();
        }

        void synchronizeControls()
        {
            gSpacing.Visible = SpecialTextAutoInsertSpace.Checked;
        }

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.PdfText();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (SpecialTextAutoInsertSpace.Checked)
                field.TextAutoInsertSpace = new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpaceThreshold.Value, Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text), IgnoreSourceSpaces = textAutoInsertSpaceIgnoreSourceSpaces.Checked };
            else
                field.TextAutoInsertSpace = null;
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
                textAutoInsertSpaceRepresentative.Text = Regex.Escape(field.TextAutoInsertSpace.Representative);
                textAutoInsertSpaceIgnoreSourceSpaces.Checked = field.TextAutoInsertSpace.IgnoreSourceSpaces;
            }

            if (value != null)
                Value.Text = Page.NormalizeText((string)value);
        }

        Template.Field.PdfText field;
    }
}
