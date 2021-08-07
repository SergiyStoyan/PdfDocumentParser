//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************


//!!! requires installing NUGET package 'TaskScheduler' and adding the respective reference


//using System;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.Win32.TaskScheduler;//requires installing NUGET package 'TaskScheduler' and adding the respective reference

//namespace Cliver
//{
//    public static class TaskScheduler
//    {
//        public static readonly Task DefaultTask = new Task
//        {
//            Name = AssemblyRoutines.GetExecutingAssemblyName(),// ProgramRoutines.GetAppName();
//            RepetitionPattern = new RepetitionPattern(new TimeSpan(0, 0, 15, 0), TimeSpan.Zero, false),
//            Executable = ProgramRoutines.GetAppPath(),
//            Parameters = ""
//        };
//        public class Task
//        {
//            public string Name;// = AssemblyRoutines.GetExecutingAssemblyName();// ProgramRoutines.GetAppName();
//            public RepetitionPattern RepetitionPattern;
//            public string Executable;// = ProgramRoutines.GetAppPath();
//            public string Parameters;
//        }

//        /// <summary>
//        /// (!)An existing task with this name will be overwritten!
//        /// </summary>
//        /// <param name="enabled"></param>
//        /// <param name="task"></param>
//        /// <returns></returns>
//        public static Microsoft.Win32.TaskScheduler.Task CreateTask(bool enabled, Task task = null)
//        {
//            //try
//            //{
//            if (task == null)
//                task = DefaultTask;

//            var t = TaskService.Instance.GetTask(task.Name);
//            TaskDefinition td = t != null ? t.Definition : TaskService.Instance.NewTask();
//            td.RegistrationInfo.Description = "";
//            td.Triggers.Clear();
//            td.Triggers.Add(new TimeTrigger()
//            {
//                StartBoundary = DateTime.Now,
//                Enabled = true,
//                Repetition = task.RepetitionPattern
//            });
//            td.Actions.Clear();
//            td.Actions.Add(new ExecAction(task.Executable, task.Parameters));
//            td.Settings.Enabled = enabled;
//            return TaskService.Instance.RootFolder.RegisterTaskDefinition(task.Name, td);
//            //}
//            //catch (System.UnauthorizedAccessException ex)
//            //{
//            //    Message.Error2("To deal with the scheduled task, the application must run As Administrator. Therefore it will ask to restart with elevated rights.", ex);
//            //    Win.ProcessRoutines.Restart(true, null);
//            //}
//        }

//        public static Microsoft.Win32.TaskScheduler.Task GetTask(string taskName = null)
//        {
//            if (taskName == null)
//                taskName = DefaultTask.Name;
//            var t = TaskService.Instance.GetTask(taskName);
//            if (t == null)
//                return null;
//            return t;
//        }

//        public static void UpdateTask(Microsoft.Win32.TaskScheduler.Task task)
//        {
//            task.RegisterChanges();
//        }

//        public static void DeleteTask(string taskName = null)
//        {
//            if (!Win.UserRoutines.CurrentUserHasElevatedPrivileges())
//            {
//                Message.Exclaim("To deal with the scheduled task, the application must run As Administrator. Therefore it will ask to restart with elevated rights.");
//                Win.ProcessRoutines.Restart(true, null);
//            }
//            if (taskName == null)
//                taskName = DefaultTask.Name;
//            TaskService.Instance.RootFolder.DeleteTask(taskName);
//        }

//        public static void EnableTask(bool enable, string taskName = null)
//        {
//            var t = GetTask(taskName);
//            if (t == null)
//                throw new Exception("A scheduled task '" + taskName + "' does not exist.");
//            t.Enabled = enable;
//            t.RegisterChanges();
//        }

//        public static bool IsTaskEnabled(string taskName = null)
//        {
//            var t = GetTask(taskName);
//            if (t == null)
//                return false;
//            return t.Enabled;
//        }

//        public static List<string> GetTaskExecutables(string taskName = null)
//        {
//            if (taskName == null)
//                taskName = DefaultTask.Name;
//            var t = TaskService.Instance.GetTask(taskName);
//            if (t == null)
//                return null;
//            return t.Definition.Actions.Where(a => a is ExecAction).Select(a => ((ExecAction)a).Path).ToList();
//        }

//        //public static Microsoft.Win32.TaskScheduler.Task GetTaskStatus(out string log, out string error, string taskName = null)
//        //{
//        //    try
//        //    {
//        //        if (taskName == null)
//        //            taskName = DefaultTask.Name;
//        //        var t = TaskService.Instance.GetTask(taskName);
//        //        if (t == null)
//        //        {
//        //            log = null;
//        //            error = "The task '" + taskName + "' does not exist.";
//        //            return null;
//        //        }
//        //        log = "Schedule: last run: " + t.LastRunTime + "; next run: " + t.NextRunTime;
//        //        if (!t.Definition.Settings.Enabled)
//        //            error = "The task '" + taskName + "' is disabled.";
//        //        else if (((ExecAction)t.Definition.Actions[0]).Path != ProgramRoutines.GetAppPath())
//        //            error = "The executable path of task '" + taskName + "' differs from the path of this application instance.";
//        //        else
//        //            error = null;
//        //        return t;
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        throw new Exception("Error while getting status of a scheduled task '" + taskName + "'.", e);
//        //    }
//        //}
//    }
//}