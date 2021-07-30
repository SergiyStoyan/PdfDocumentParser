﻿//********************************************************************************************
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
    public partial class FieldPdfCharBoxsControl : FieldControl
    {
        public FieldPdfCharBoxsControl(TextAutoInsertSpace textAutoInsertSpace)
        {
            InitializeComponent();

            this.textAutoInsertSpace = textAutoInsertSpace;
        }
        TextAutoInsertSpace textAutoInsertSpace;

        override protected object getObject()
        {
            if (field == null)
                field = new Template.Field.PdfCharBoxs();
            field.ColumnOfTable = (string)ColumnOfTable.SelectedItem;
            //field.TextAutoInsertSpace = SpecialTextAutoInsertSpace.Checked ? templateForm.GetTextAutoInsertSpaceFromGUI() : null;
            return field;
        }

        protected override void initialize(DataGridViewRow row, object value)
        {
            field = (Template.Field.PdfCharBoxs)row.Tag;
            if (field == null)
                field = new Template.Field.PdfCharBoxs();

            List<string> fieldNames = fields.Where(a => a.ColumnOfTable == null).Select(a => a.Name).Distinct().ToList();
            fieldNames.Remove(field.Name);
            fieldNames.Insert(0, "");
            ColumnOfTable.DataSource = fieldNames;

            ColumnOfTable.SelectedItem = field.ColumnOfTable;

            SpecialTextAutoInsertSpace.Checked = field.TextAutoInsertSpace != null;

            if (value != null)
            {
                List<Page.Line<Pdf.CharBox>> cbss = Page.GetLines((List<Pdf.CharBox>)value, textAutoInsertSpace);
                List<string> ls = new List<string>();
                foreach (var cbs in cbss)
                    ls.Add(Serialization.Json.Serialize(cbs.CharBoxs));
                Value.Text = string.Join("\r\n", ls);
            }
        }

        Template.Field.PdfCharBoxs field;
    }
}
