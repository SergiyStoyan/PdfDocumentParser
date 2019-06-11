using System;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using Cliver.Win;

namespace Cliver.Wpf
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(string caption, Icon icon, string message, string[] buttons, int default_button, Window owner)
        {
            InitializeComponent();

            Icon = Win.AssemblyRoutines.GetAppIconImageSource();
            Title = caption;
            Owner = owner;

            if (icon == null)
                image.Width = 0;
            else
                image.Source = icon.ToImageSource();

            System.Drawing.Size s = Cliver.Win.SystemInfo.GetCurrentScreenSize(new System.Drawing.Point((int)Left, (int)Top), true);
            MaxWidth = s.Width * 3 / 4;
            MaxHeight = s.Height * 3 / 4;

            this.message.Text = message;

            //if (!yesNo)
            //{
            //    b1.Content = "OK";
            //    b2.Visibility = Visibility.Collapsed;
            //}

            if (buttons != null)
            {
                for (int i = buttons.Length - 1; i >= 0; i--)
                {
                    Button b = new Button();
                    b.Tag = i;
                    b.Content = buttons[i];
                    //b.AutoSize = true;
                    b.Click += b_Click;
                    buttonPanel.Children.Add(b);
                    if (i == default_button)
                        b.Focus();
                }
            }

            Loaded += delegate
            {
                if (ActualWidth == MaxWidth)
                    this.message.Width = this.canvas.ActualWidth;
            };
        }

        void b_Click(object sender, EventArgs e)
        {
            ClickedButton = (int)((Button)sender).Tag;
            Close();
        }

        public int? ClickedButton { get; private set; } = null;

        new public int? ShowDialog()
        {
            base.ShowDialog();
            return ClickedButton;
        }

        public new void Close()
        {
            try
            {
                Dispatcher.Invoke(() =>
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