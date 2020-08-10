using Cliver.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliver.PdfDocumentParser
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        void run()
        {
            //capturedFields.Columns.Add("",);

        }

        private void cRun_Click(object sender, EventArgs e)
        {
            if (cRun.Checked)
            {
                if (t != null && !t.TryAbort(1000))
                {
                    Message.Error("Could not stop the debugging thread.");
                    return;
                }
                t = null;
                cRun.Checked = false;
                return;
            }
            cRun.Checked = true;
            t = ThreadRoutines.StartTry(
                () =>
                {



                },
                (Exception ex) =>
                {
                    Log.Error(ex);
                    Message.Error(ex);
                },
                () =>
                {
                    cRun.Checked = false;
                }
            );
        }
        Thread t = null;

        private void bSave_Click(object sender, EventArgs e)
        {

        }
    }
}
