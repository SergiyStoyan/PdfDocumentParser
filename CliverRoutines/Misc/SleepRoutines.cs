//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;


namespace Cliver
{
    public static class SleepRoutines
    {
        public static object WaitForObject(Func<object> get_object, int mss, int spin_sleep_in_mss = 10)
        {
            object o = null;
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                o = get_object();
                if (o != null)
                    break;
                //Application.DoEvents();
                Thread.Sleep(spin_sleep_in_mss);
            }
            return o;
        }

        public static bool WaitForCondition(Func<bool> check_condition, int mss, int spin_sleep_in_mss = 10)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                if (check_condition())
                    return true;
                //Application.DoEvents();
                Thread.Sleep(spin_sleep_in_mss);
            }
            return false;
        }

        public static void Wait(int mss, int spin_sleep_in_mss = 10)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                //Application.DoEvents();
                Thread.Sleep(spin_sleep_in_mss);
            }
        }        
    }
}

