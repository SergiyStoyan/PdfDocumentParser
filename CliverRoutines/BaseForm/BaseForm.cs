//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace Cliver
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
        }

        public static Size GetRecommendedWindowSize(double factor = 0.8)
        {
            Size s = SystemInfo.GetPrimaryScreenSize(false);
            return new Size((int)((float)s.Width * factor), (int)((float)s.Height * factor));
        }
    }

    public static class ControlRoutines
    {
        public static void EnumControls(this Control control, Action<Control> function, bool recoursive)
        {
            foreach (Control c in control.Controls)
            {
                function(c);
                if (recoursive)
                    EnumControls(c, function, true);
            }
        }

        /// <summary>
        /// Set text to the control thread-safely.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="m"></param>
        public static void SetText(this Control c, string m)
        {
            if (c.InvokeRequired)
                c.BeginInvoke(new MethodInvoker(() => { c.Text = m; }));
            else
                c.Text = m;
        }

        public static object Invoke(this Control c, Func<object> function)
        {
            return c.Invoke(function);
        }

        public static void Invoke(this Control c, MethodInvoker code)
        {
            c.Invoke(code);
        }

        public static void Invoke2(this Control c, MethodInvoker code)
        {
            if (c.InvokeRequired)
                c.Invoke(code);
        }

        public static void BeginInvoke(this Control c, MethodInvoker code)
        {
            c.BeginInvoke(code);
        }

        public static void BeginInvoke2(this Control c, MethodInvoker code)
        {
            if (c.InvokeRequired)
                c.BeginInvoke(code);
        }

        public static object InvokeFromUiThread(Delegate d)
        {
            try
            {
                return Application.OpenForms[0].Invoke(d);
            }
            catch (ThreadAbortException) { }
            return null;
        }

        public static object InvokeFromUiThread(MethodInvoker code)
        {
            return InvokeFromUiThread((Delegate)code);
        }

        public static Thread SlideVertically(this Control c, double pixelsPerMss, int position2, int step = 1, MethodInvoker finished = null)
        {
            lock (c)
            {
                Thread t = null;
                //if (controls2sliding_thread.TryGetValue(c, out t) && t.IsAlive)
                //    return t;

                step = c.Top > position2 ? -step : step;
                double total_mss = Math.Abs(position2 - c.Top) / pixelsPerMss;
                int sleep = (int)(total_mss / ((position2 - c.Top) / step));
                t = ThreadRoutines.Start(() =>
                {
                    try
                    {
                        while (c.Visible && !(bool)ControlRoutines.Invoke(c, () =>
                            {
                                c.Top = c.Top + step;
                                return step < 0 ? c.Top <= position2 : c.Top >= position2;
                            })
                        )
                            System.Threading.Thread.Sleep(sleep);
                        ControlRoutines.Invoke(c, () =>
                            {
                                finished?.Invoke();
                            });
                    }
                    catch(Exception e)//control disposed
                    {
                    }
                });
                //controls2sliding_thread[c] = t;
                return t;
            }
        }
        //static readonly  Dictionary<Control, Thread> controls2sliding_thread = new Dictionary<Control, Thread>();

        public static Thread Condense(this Form f, double opacityPerMss, double opacity2, double step = 0.05, MethodInvoker finished = null)
        {
            lock (f)
            {
                Thread t = null;
                //if (controls2condensing_thread.TryGetValue(f, out t) && t.IsAlive)
                //    return t;

                step = f.Opacity < opacity2 ? step : -step;
                double total_mss = Math.Abs(opacity2 - f.Opacity) / opacityPerMss;
                int sleep = (int)(total_mss / ((opacity2 - f.Opacity) / step));
                t = ThreadRoutines.Start(() =>
                {
                    try
                    {
                        while (!(bool)ControlRoutines.Invoke(f, () =>
                            {
                                f.Opacity = f.Opacity + step;
                                return step > 0 ? f.Opacity >= opacity2 : f.Opacity <= opacity2;
                            })
                        )
                            System.Threading.Thread.Sleep(sleep);
                        ControlRoutines.Invoke(f, () =>
                        {
                            finished?.Invoke();
                        });
                    }
                    catch (Exception e)//control disposed
                    {
                    }
                });
                //controls2condensing_thread[f] = t;
                return t;
            }
        }
        //static readonly Dictionary<Form, Thread> controls2condensing_thread = new Dictionary<Form, Thread>();
    }
}

