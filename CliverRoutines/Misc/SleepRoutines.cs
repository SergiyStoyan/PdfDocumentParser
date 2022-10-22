//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Threading;
using System.Collections.Generic;


namespace Cliver
{
    public static class SleepRoutines
    {
        public static T WaitForObject<T>(Func<T> get_object, int mss, int spin_sleep_in_mss = 10) where T : class
        {
            T o = null;
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

        public static bool WaitForCondition(Func<bool> condition, int timeoutMss, int pollTimeSpanMss = 10)
        {
            for (DateTime timeout = DateTime.Now.AddMilliseconds(timeoutMss); DateTime.Now < timeout; Thread.Sleep(pollTimeSpanMss))
            {
                if (condition())
                    return true;
                //Application.DoEvents();
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

