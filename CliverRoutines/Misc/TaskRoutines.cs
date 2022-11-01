using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliver
{
    /// <summary>
    /// Credit to: https://social.msdn.microsoft.com/Forums/en-US/163ef755-ff7b-4ea5-b226-bbe8ef5f4796/is-there-a-pattern-for-calling-an-async-method-synchronously?forum=async
    /// https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
    /// </summary>
    public static class TaskRoutines
    {
        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSynchronously(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSynchronously<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool run;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<(SendOrPostCallback sendOrPostCallback, object state)> items = new Queue<(SendOrPostCallback sendOrPostCallback, object state)>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback sendOrPostCallback, object state)
            {
                lock (items)
                {
                    items.Enqueue((sendOrPostCallback, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => run = false, null);
            }

            public void BeginMessageLoop()
            {
                run = true;
                while (run)
                {
                    (SendOrPostCallback sendOrPostCallback, object state) task = (sendOrPostCallback: null, state: null);
                    lock (items)
                    {
                        if (items.Count > 0)
                            task = items.Dequeue();
                    }
                    if (task.sendOrPostCallback != null)
                    {
                        task.sendOrPostCallback(task.state);
                        if (InnerException != null)
                            throw new Exception(nameof(TaskRoutines.RunSynchronously) + " method threw an exception.", InnerException);
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
