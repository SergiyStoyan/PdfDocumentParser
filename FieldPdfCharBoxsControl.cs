//********************************************************************************************
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
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    public partial class FieldPdfCharBoxsControl : FieldControl
    {
        public FieldPdfCharBoxsControl()
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
                field = new Template.Field.PdfCharBoxs();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            if (SpecialTextAutoInsertSpace.Checked)
                field.TextAutoInsertSpace = new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpace_Threshold.Value, Representative = Regex.Unescape(textAutoInsertSpaceRepresentative.Text), IgnoreSourceSpaces = textAutoInsertSpaceIgnoreSourceSpaces.Checked };
            else
                field.TextAutoInsertSpace = null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.PdfCharBoxs)row.Tag;
            if (field == null)
                field = new Template.Field.PdfCharBoxs();

            List<string> fieldNames = template.Fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialTextAutoInsertSpace.Checked = field.TextAutoInsertSpace != null;
            if (field.TextAutoInsertSpace != null)
            {
                textAutoInsertSpace_Threshold.Value = (decimal)field.TextAutoInsertSpace.Threshold;
                textAutoInsertSpaceRepresentative.Text = Regex.Escape(field.TextAutoInsertSpace.Representative);
                textAutoInsertSpaceIgnoreSourceSpaces.Checked = field.TextAutoInsertSpace.IgnoreSourceSpaces;
            }
            else
            {
                textAutoInsertSpace_Threshold.Value = (decimal)template.TextAutoInsertSpace.Threshold;
                textAutoInsertSpaceRepresentative.Text = template.TextAutoInsertSpace.Representative;
                textAutoInsertSpaceIgnoreSourceSpaces.Checked = template.TextAutoInsertSpace.IgnoreSourceSpaces;
            }

            if (value != null)
            {
                List<Page.Line<Pdf.CharBox>> cbss = Page.GetLines((List<Pdf.CharBox>)value,
                   SpecialTextAutoInsertSpace.Checked ? new TextAutoInsertSpace { Threshold = (float)textAutoInsertSpace_Threshold.Value, IgnoreSourceSpaces = textAutoInsertSpaceIgnoreSourceSpaces.Checked, Representative = textAutoInsertSpaceRepresentative.Text } : template.TextAutoInsertSpace,
                   null
                );
                List<string> ls = new List<string>();
                foreach (var cbs in cbss)
                    ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                Value.Text = string.Join("\r\n", ls);
            }
        }

        Template.Field.PdfCharBoxs field;
    }
}
