//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace Cliver
{
    /// <summary>
    /// Dynamic dialog box with many answer cases
    /// </summary>
    public partial class MessageForm : Form
    {
        public MessageForm(string caption, Icon icon, string message, string[] buttons, int default_button, Form owner, bool button_auto_size = false)
        {
            InitializeComponent();
            
            CreateHandle();

            this.Icon = AssemblyRoutines.GetAppIcon();

            this.MaximizeBox = true;

            Owner = owner;

            if (icon != null)
            {
                int w = icon.Width - image_box.Width;
                image_box.Image = (Image)icon.ToBitmap();
                if (w > 0)
                {
                    this.Width += w;
                    this.message.Left += w;
                }
            }

            this.Text = caption;
            this.message.Text = message;

            if (buttons != null)
            {
                for (int i = buttons.Length - 1; i >= 0; i--)
                {
                    Button b = new Button();
                    b.Tag = i;
                    b.Text = buttons[i];
                    b.AutoSize = true;
                    b.Click += b_Click;
                    flowLayoutPanel1.Controls.Add(b);
                    if (i == default_button)
                        b.Select();
                }

                if (!button_auto_size)
                {
                    Size max_size = new Size(0, 0);
                    foreach (Button b in flowLayoutPanel1.Controls)
                    {
                        if (b.Width > max_size.Width)
                            max_size.Width = b.Width;
                        if (b.Height > max_size.Height)
                            max_size.Height = b.Height;
                    }
                    foreach (Button b in flowLayoutPanel1.Controls)
                    {
                        b.AutoSize = false;
                        b.Size = max_size;
                    }
                }
            }

            //Size s = this.message.GetPreferredSize(new Size(Screen.PrimaryScreen.WorkingArea.Width * 3 / 4, Screen.PrimaryScreen.WorkingArea.Height * 3 / 4));
            //this.Width = this.Width + s.Width - this.message.Width;
            //this.Height = this.Height + s.Height - this.message.Height;
        }

        void b_Click(object sender, EventArgs e)
        {
            clicked_button = (int)((Button)sender).Tag;
            this.Close();
        }

        int clicked_button = -1;

        public int ClickedButton
        {
            get { return clicked_button; }
        }

        new public int ShowDialog()
        {
            System.Windows.Forms.DialogResult r = base.ShowDialog();
            return clicked_button;
        }

        private void Message_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            var rtb = (RichTextBox)sender;
            Size s = this.Size;
            {
                int h = e.NewRectangle.Height - rtb.Height;
                if (h > 0)
                {
                    int h2 = Screen.PrimaryScreen.WorkingArea.Height * 3 / 4 - this.Height;
                    s.Height += h2 < h ? h2 : h;
                }
            }
            {
                int w = e.NewRectangle.Width - rtb.Width;
                if (w > 0)
                {
                    int w2 = Screen.PrimaryScreen.WorkingArea.Width * 3 / 4 - this.Width;
                    s.Width += w2 < w ? w2 : w;
                }
            }
            {
                if (s.Height > s.Width)
                {
                    s.Height -= 100;
                    s.Width += 100;
                }
            }
            this.Size = s;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                switch ((Int32)m.WParam)
                {
                    case 0xF030: // Maximize event - SC_MAXIMIZE from Winuser.h
                        restored_size = this.Size;
                        break;
                    case 0xF120: // Restore event - SC_RESTORE from Winuser.h
                        this.Size = restored_size;
                        break;
                }
            }
            base.WndProc(ref m);
        }
        Size restored_size;

        public new void Close()
        {
            try
            {
                this.Invoke(() =>
                {
                    try
                    {
                        base.Close();
                    }
                    catch { }//if closed already
                });
            }
            catch { }//if closed already
        }
    }
}