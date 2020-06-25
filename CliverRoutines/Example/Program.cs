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
                Log_usage_example();
                Config_usage_example();
            }
            catch (Exception e)
            {
                Log.Exit(e);
            }
        }
        static void Log_usage_example()
        {
            try
            {
                Log.Initialize(Log.Mode.EACH_SESSION_IS_IN_OWN_FORLDER);

                Log.Inform("test");
                ThreadRoutines.StartTry(() =>
                {
                    Log.Inform0("to default log");
                    Log.Thread.Inform0("to thread log");
                    throw new Exception2("test exception2");
                },
                (Exception e) =>
                {
                    Log.Thread.Error(e);
                }
                );

                Log.Session s1 = Log.Session.Get("Name1");//open if not open session "Name1"
                Log.Writer nl = s1["Name"];//open if not open log "Name"
                nl.Error("to log 'Name'");
                s1.Trace("to the main log of session 'Name1'");
                s1.Thread.Inform("to the thread log of session 'Name1'");
                s1.Rename("Name2");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        static void Config_usage_example()
        {
            Config.Reload();

            //the usual routine is direct manipulating with settings data
            Settings.Smtp.Port = 10;
            //if the data is OK
            Settings.Smtp.Save();
            //else
            Settings.Smtp.Reload();

            //a more advanced routine which usually is not used
            SmtpSettings smtp2 = Config.CreateResetInstance(Settings.Smtp);
            //pass smtp2 somewhere for editing...
            smtp2.Port = 11;
            //if the data is OK
            Settings.Smtp = smtp2;
            Settings.Smtp.Save();
        }
    }
}
