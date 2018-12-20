//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Web;

namespace Cliver
{
    /// <summary>
    /// Sleep not freezing the app
    /// </summary>
    public static class ThreadRoutines
    {
        public static Thread Start(ThreadStart code, bool background = true, ApartmentState state = ApartmentState.Unknown)
        {
            Thread t = new Thread(code);
            if (state != ApartmentState.Unknown)
                t.SetApartmentState(state);
            t.IsBackground = background;
            t.Start();
            return t;
        }

        public delegate void ErrorHandler(Exception e);

        public static Thread StartTry(MethodInvoker code, ErrorHandler on_error = null, MethodInvoker on_finally = null, bool background = true, ApartmentState state = ApartmentState.Unknown)
        {
            Thread t = new Thread(
                () =>
                {
                    try
                    {
                        code.Invoke();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            if (on_error != null)
                                on_error.Invoke(e);
                            else
                                Message.Error(e);
                        }
                        catch (ThreadAbortException)
                        {
                            Thread.ResetAbort();
                        }
                    }
                    finally
                    {
                        on_finally?.Invoke();
                    }
                }
            );
            if (state != ApartmentState.Unknown)
                t.SetApartmentState(state);
            t.IsBackground = background;
            t.Start();
            return t;
        }

        //static HashSet<Thread> threads = new HashSet<Thread>();

        //public static Thread GetThread(ThreadStart st)
        //{
        //    lock (threads)
        //    {
        //        Thread t = new Thread(st);
        //        threads.Add(t);
        //        return t;
        //    }
        //}

        //public static void Shutdown()
        //{
        //    lock (threads)
        //    {
        //        foreach (Thread t in threads)
        //            if (t != Thread.CurrentThread)
        //                t.Abort();
        //    }
        //}

        //public static void Kill(Thread t)
        //{
        //    lock (threads)
        //    {
        //        threads.Remove(t);
        //        t.Abort();
        //    }
        //}

        //public static void Wait(long milliseconds, int poll_interval_in_mss = 20)
        //{
        //    if (milliseconds / 2 < poll_interval_in_mss) poll_interval_in_mss /= 2;
        //    DateTime t = DateTime.Now.AddMilliseconds(milliseconds);
        //    while (t > DateTime.Now)
        //    {
        //        Application.DoEvents();
        //        Thread.Yield();
        //        if(poll_interval_in_mss > 0)
        //            Thread.Sleep(poll_interval_in_mss);
        //    }
        //}

        ///// <summary>
        ///// Waiting not freezing the app
        ///// </summary>
        //public static object WaitForObject(Func<object> check_condition, int timeout_in_mss, int poll_interval_in_mss = 20)
        //{
        //    object o = null;
        //    DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
        //    while (dt > DateTime.Now)
        //    {
        //        o = check_condition();
        //        if (o != null)
        //            break;
        //        Application.DoEvents();
        //        Thread.Sleep(poll_interval_in_mss);
        //    }
        //    return o;
        //}

        ///// <summary>
        ///// Waiting not freezing the app
        ///// </summary>
        //public static object WaitForCondition(Func<bool> check_condition, int timeout_in_mss, int poll_interval_in_mss = 20)
        //{
        //    bool o = false;
        //    DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
        //    while (dt > DateTime.Now)
        //    {
        //        o = check_condition();
        //        if (o)
        //            break;
        //        Application.DoEvents();
        //        Thread.Sleep(poll_interval_in_mss);
        //    }
        //    return o;
        //}   
    }
}

