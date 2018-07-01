using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    public partial class FileFilterForm : Form
    {
        public FileFilterForm(string folder, Regex r)
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = "Filtered Files";
            files.ReadOnly = true;

            Load += delegate
            {
                foreach(string f in FileSystemRoutines.GetFiles(folder))
                {
                    int i = files.Items.Add(f);
                    if (r.IsMatch(f))
                        files.SelectedIndex = i;
                }
            };

        }
    }

    public class ReadOnlyListBox : ListBox
    {
        private bool _readOnly = false;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            // If ReadOnly is set to true, then block any messages 
            // to the selection area from the mouse or keyboard. 
            // Let all other messages pass through to the 
            // Windows default implementation of DefWndProc.
            if (!_readOnly || ((m.Msg <= 0x0200 || m.Msg >= 0x020E)
            && (m.Msg <= 0x0100 || m.Msg >= 0x0109)
            && m.Msg != 0x2111
            && m.Msg != 0x87))
            {
                base.DefWndProc(ref m);
            }
        }
    }
}
