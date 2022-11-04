//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cliver
{
    public class Trier
    {
        virtual public int RetryMaxCount { get; set; } = 3;

        virtual protected bool proceedOnException(Exception e)
        {
            List<Type> ignoredExceptionTypes = new List<Type>();
            return ignoredExceptionTypes?.Find(a => e.GetType() == a) != null;
        }

        virtual protected void onRetry(Exception e)
        {
            int retryDelayInSecs = 30;
            System.Threading.Thread.Sleep(retryDelayInSecs * 1000);
        }

        //public T Perform<T>()
        //{
        //    return Perform(Function);
        //}

        protected int retryCount;

        public T Perform<T>(Func<T> function, Func<Exception, bool> proceedOnException = null, Action<Exception> onRetry = null)
        {
            if (proceedOnException == null)
                proceedOnException = this.proceedOnException;
            if (onRetry == null)
                onRetry = this.onRetry;
            for (retryCount = 0; ; retryCount++)
                try
                {
                    return function();
                }
                catch (Exception e)
                {
                    if (!proceedOnException(e))
                    {
                        //throw new Exception("Caller stack: " + Log.GetStackString(1, -1), e);
                        //Log.Warning2("Caller stack for the following error: " + Log.GetStackString(1, -1));
                        throw;
                    }
                    if (retryCount >= RetryMaxCount)
                        throw new Exception("Try count exeeded: " + (retryCount + 1), e);
                    onRetry(e);
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

