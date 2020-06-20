using System;
using System.Threading;
using Cliver;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Initialize(Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER);
                Config.Reload();

                Log.Inform("test");
                ThreadRoutines.StartTry(() =>
                {
                    Log.Inform0("to default log");
                    Log.Thread.Inform0("to thread log");
                    throw new Exception("test exception");
                },
                (Exception e) =>
                {
                    Log.Thread.Error(e);
                }
                );

                Log.Session s1 = Log.Session.Get("Name1");
                Log.Writer nl = s1["Name"];
                nl.Error("test error");
                s1.Trace("test");
                s1.Thread.Inform("test");
                s1.Rename("Name2");

                throw new Exception2("test exception2");
            }
            catch(Exception e)
            {
                Log.Exit(e);
            }            
        }
    }
}
