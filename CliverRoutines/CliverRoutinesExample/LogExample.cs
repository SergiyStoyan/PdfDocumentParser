using System;
using System.Threading;
using Cliver;

namespace Example
{
    class LogExample
    {
        public static void Run()
        {
            //default configuration: everything is written to the same file in the same folder:
            Log.Inform("write out of box");
            Log.Thread.Inform("write out of box2");
            Log.Head["test"].Inform("write out of box3");
            Log.Session.Get("GAME")["client"].Inform("write out of box4");
            Log.Session.Get("GAME").Rename("Game");

            //a session-less log which will be continued with each app's start
            Log.Get("throughout").Inform("session-less log");

            //optional initialization. You will like to perform it at the very beginning of the app.            
            Log.Initialize(Log.Mode.FOLDER_PER_SESSION);

            //trivial usage: everything is written to the same file
            Log.Inform("write to the default log of the default session");
            Log.Inform("Log folder: " + Log.Dir);

            //more sophisticated usage
            Log.Head["Action1"].Inform0("write to log 'Action1' of the default session");

            //writing thread logs to the default session
            ThreadRoutines.Start(task);
            ThreadRoutines.Start(task);

            //writing to an explicitly created session
            Log.Session logSession_Task = Log.Session.Get("TASK");//create if not exists
            logSession_Task.Inform("write to the default log of the session '" + logSession_Task.Name + "'");
            Log.Writer log_Task_Subtask = logSession_Task["Subtask"];//create if not exists
            log_Task_Subtask.Error("write to log '" + log_Task_Subtask.Name + "' of session '" + logSession_Task.Name + "''");
            logSession_Task.Trace("write to the default log of the session '" + logSession_Task.Name + "'");
            logSession_Task.Thread.Inform("write to the thread log " + Log.Thread.Id + " of the session '" + logSession_Task.Name + "'");
            //sometimes you may need to rename a log session:
            logSession_Task.Rename("renamed_TASK");
            //optional; close the handlers and free memory
            logSession_Task.Close(false);

            //writing thread logs to explicitly created sessions
            Task.Start("TASK1");
            Task.Start("TASK2");
        }

        static void task()
        {
            try
            {
                Log.Inform0("write to default log of the default session");
                Log.Thread.Inform0("write to thread log " + Log.Thread.Id + " of the default session");
                throw new Exception("test exception2");
            }
            catch (Exception e)
            {
                Log.Thread.Error("write to thread log " + Log.Thread.Id + " of the default session", e);
            }
        }


        class Task
        {
            public static void Start(string name)
            {
                Task task = new Task();
                task.logSession = Log.Session.Get(name);
                ThreadRoutines.Start(task.download);
                ThreadRoutines.Start(task.download);
            }
            Log.Session logSession;

            // bogus task
            void download()
            {
                try
                {
                    logSession.Inform0("write to the default log of session '" + logSession.Name + "'");
                    logSession.Thread.Inform0("write to thread log " + logSession.Thread.Id + " of session '" + logSession.Name + "'");
                    throw new Exception2("test exception");
                }
                catch (Exception e)
                {
                    logSession.Thread.Error("write to thread log " + logSession.Thread.Id + " of session '" + logSession.Name + "'", e);
                }
            }
        }
    }
}
