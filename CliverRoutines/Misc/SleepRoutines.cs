//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
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

    public class Tryer
    {
        public Tryer(int tryMaxCount, int retryDelayInSecs, List<Type> exceptionTypes = null)
        {
            TryMaxCount = tryMaxCount;
            RetryDelayInSecs = retryDelayInSecs;
            ExceptionTypes = exceptionTypes;
        }
        public int TryMaxCount;
        public int RetryDelayInSecs;
        public List<Type> ExceptionTypes;
        //public Func<T> Function;

        public delegate string OnRetryDelegate(Exception exception, int tryCount);
        public OnRetryDelegate OnRetry;

        //public T Perform<T>()
        //{
        //    return Perform(Function);
        //}

        public T Perform<T>(Func<T> function)
        {
            for (int tryCount = 1; ; tryCount++)
                try
                {
                    return function();
                }
                catch (Exception e)
                {
                    if (ExceptionTypes != null && ExceptionTypes.Find(a => e.GetType() == a) == null)
                    {
                        //throw new Exception("Caller stack: " + Log.GetStackString(1, -1), e);
                        //Log.Warning2("Caller stack for the following error: " + Log.GetStackString(1, -1));
                        throw;
                    }
                    if (tryCount >= TryMaxCount)
                        throw new Exception("Try count exeeded: " + tryCount, e);
                    if (OnRetry != null)
                        OnRetry(e, tryCount);
                    System.Threading.Thread.Sleep(RetryDelayInSecs * 1000);
                }
        }

        //public static T Perform<T>(Func<T> function, int tryMaxCount, int retryDelayInSecs, List<Type> exceptionTypes = null, OnRetryDelegate onRetry = null)
        //{
        //    for (int tryCount = 0; ; tryCount++)
        //        try
        //        {
        //            return function();
        //        }
        //        catch (Exception e)
        //        {
        //            if (exceptionTypes != null && exceptionTypes.Find(a => e.GetType() == a) == null)
        //                throw;
        //            if (tryCount++ >= tryMaxCount)
        //                throw new Exception("Try count exeeded: " + tryCount, e);
        //            if (onRetry != null)
        //                onRetry(e, tryCount);
        //            System.Threading.Thread.Sleep(retryDelayInSecs * 1000);
        //        }
        //}
    }
}

