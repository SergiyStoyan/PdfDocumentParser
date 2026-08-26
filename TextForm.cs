//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    public partial class TextForm : Form
    {
        public TextForm(string caption, string t, bool edit, List<(int start, int length, iTextSharp.text.pdf.DocumentFont font)> fonts = null)
        {
            InitializeComponent();
            Icon = Win.AssemblyRoutines.GetAppIcon();
            Text = AboutBox.AssemblyProduct + ": " + caption;// Application.ProductName;

            textBox.ReadOnly = !edit;
            panel.Visible = edit;
            textBox.Text = t;

            HashSet<string> errors = new HashSet<string>();

            if (fonts != null)
            {
                Dictionary<string, string> fsns2fn = new Dictionary<string, string>();
                using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
                    fsns2fn = fontsCollection.Families.ToDictionary(a => Regex.Replace(a.Name, @"\s+", ""), a => a.Name);

                var ms = Regex.Matches(t, "\r"); //textBox eats \r's so we need to adjust positions
                int rCounter = 0;
                int i = 0;
                foreach ((int start, int length, iTextSharp.text.pdf.DocumentFont font) in fonts)
                {
                    int start1 = start - rCounter;
                    int length1 = length;
                    int end = start + length;
                    for (; i < ms.Count; i++)
                    {
                        var m = ms[i];
                        if (m.Index <= start)
                        {
                            rCounter++;
                            start1--;
                        }
                        else if (m.Index < end)
                        {
                            rCounter++;
                            length1--;
                        }
                        else
                            break;
                    }

                    var fsn = Regex.Replace(font.PostscriptFontName, @".*\+|\,.*", "");
                    if (!fsns2fn.TryGetValue(fsn, out string fn))
                    {
                        errors.Add("Font " + fsn + " is not found.");
                        continue;
                    }
                    textBox.Select(start1, length1);
                    textBox.SelectionFont = new System.Drawing.Font(fn, textBox.Font.Size);
                }
            }

            Load += (s, e) =>
            {
                if (errors.Count > 0)
                    this.Error(string.Join("\r\n", errors));
            };
        }


        //public TextForm(string caption, List<(string Text, iTextSharp.text.pdf.DocumentFont Font)> t, bool edit)
        //{
        //    InitializeComponent();
        //    Icon = Win.AssemblyRoutines.GetAppIcon();
        //    Text = AboutBox.AssemblyProduct + ": " + caption;// Application.ProductName;

        //    textBox.ReadOnly = !edit;
        //    panel.Visible = edit;
        //    textBox.Text = t;

        //        foreach ((string Text, iTextSharp.text.pdf.DocumentFont Font) in t)
        //        {
        //            textBox.Select(start, end - start);
        //            textBox.SelectionFont = new System.Drawing.Font(f.Familyname, textBox.Font.Size);
        //        }
        //}

        private void bSave_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public string Content
        {
            get
            {
                return textBox.Text;
            }
        }

        private void bCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
