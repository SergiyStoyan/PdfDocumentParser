//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Cliver
{
    public static partial class Log
    {
        static readonly object lockObject = new object();

        public static void Initialize(Mode mode, List<string> primaryBaseDirs = null, Level level = Level.ALL, bool writeLog = true, int deleteLogsOlderDays = 10)
        {
            lock (lockObject)
            {
                Log.CloseAll();
                Log.mode = mode;
                Log.primaryBaseDirs = primaryBaseDirs;
                Log.writeLog = writeLog;
                Log.deleteLogsOlderDays = deleteLogsOlderDays;
                Log.level = level;
            }
        }
        static List<string> primaryBaseDirs = null;
        static int deleteLogsOlderDays = 10;
        static bool writeLog = true;
        static Mode mode = Mode.ALL_LOGS_ARE_IN_SAME_FOLDER;
        static Level level = Level.ALL;

        public static bool ReuseThreadLogIndexes = false;

        public enum Mode
        {
            /// <summary>
            /// Each session creates its own folder.
            /// </summary>
            EACH_SESSION_IS_IN_OWN_FORLDER,
            /// <summary>
            /// <summary>
            /// Writes only log files without creating session folder.
            /// </summary>
            ALL_LOGS_ARE_IN_SAME_FOLDER
        }

        /// <summary>
        /// Head nname session created by default.
        /// </summary>
        public static Session Head
        {
            get
            {
                if (headSession == null)
                    headSession = Session.Get(HEAD_SESSION_NAME);
                return headSession;
            }
        }
        static Session headSession = null;
        public const string HEAD_SESSION_NAME = "";

        /// <summary>
        /// Default log of the head session. 
        /// Depending on condition THREAD_LOG_IS_DEFAULT, it is either Main log or Thread log.
        /// </summary>
        public static Writer Default
        {
            get
            {
                return Head.Default;
            }
        }

        /// <summary>
        /// Main log of the head session.
        /// </summary>
        public static Session.NamedWriter Main
        {
            get
            {
                return Head.Main;
            }
        }

        /// <summary>
        /// Thread log of the head session.
        /// </summary>
        public static Session.ThreadWriter Thread
        {
            get
            {
                return Head.Thread;
            }
        }

        /// <summary>
        /// Log only messages of the respective types
        /// </summary>
        public enum Level
        {
            NONE,
            ERROR,
            WARNING,
            INFORM,
            ALL
        }

        public enum MessageType
        {
            LOG = 0,
            INFORM = 1,
            WARNING = 2,
            ERROR = 3,
            EXIT = 4,
            TRACE = 5,
            //INFORM2 = 11,
            //WARNING2 = 21,
            //ERROR2 = 31,
            //EXIT2 = 41,
        }

        /// <summary>
        /// Clear all sessions and close all the log files.
        /// </summary>
        public static void CloseAll()
        {
            lock (lockObject)
            {
                Session.CloseAll();

                workDir = null;
                if (headSession != null)
                    headSession.Close(false);
                headSession = null;

                GC.Collect();
            }
        }

        /// <summary>
        ///Parent log directory.
        /// </summary>
        public static string WorkDir
        {
            get
            {
                if (workDir == null)
                {
                    while (deletingOldLogsThread != null && deletingOldLogsThread.IsAlive)
                    {
                        deletingOldLogsThread.Abort();
                        System.Threading.Thread.Sleep(100);
                    }
                    lock (lockObject)
                    {
                        List<string> baseDirs = new List<string> {
                                Log.AppDir,
                                CompanyUserDataDir,
                                CompanyCommonDataDir,
                                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                                System.IO.Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + CompanyName + System.IO.Path.DirectorySeparatorChar,
                                };
                        if (Log.primaryBaseDirs != null)
                            baseDirs.InsertRange(0, Log.primaryBaseDirs);
                        foreach (string baseDir in baseDirs)
                        {
                            workDir = baseDir + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + WorkDirNameSuffix;
                            if (writeLog)
                                try
                                {
                                    if (!Directory.Exists(workDir))
                                        FileSystemRoutines.CreateDirectory(workDir);
                                    string testFile = workDir + System.IO.Path.DirectorySeparatorChar + "test";
                                    File.WriteAllText(testFile, "test");
                                    File.Delete(testFile);
                                    break;
                                }
                                catch //(Exception e)
                                {
                                    workDir = null;
                                }
                        }
                        if (workDir == null)
                            throw new Exception("Could not access any log directory.");
                        workDir = PathRoutines.GetNormalizedPath(workDir, false);
                        if (writeLog)
                            if (Directory.Exists(workDir) && deleteLogsOlderDays >= 0)
                                deletingOldLogsThread = ThreadRoutines.Start(() => { Log.DeleteOldLogs(deleteLogsOlderDays, DeleteOldLogsDialog); });//to avoid a concurrent loop while accessing the log file from the same thread 
                            else
                                throw new Exception("Could not create log folder!");
                    }
                    // deletingOldLogsThread?.Join();
                }
                return workDir;
            }
        }
        static string workDir = null;
        public const string WorkDirNameSuffix = @"_Sessions";
        static Thread deletingOldLogsThread = null;
        public static Func<string, bool> DeleteOldLogsDialog = null;
    }

    //public class TerminatingException : Exception
    //{
    //    public TerminatingException(string message)
    //        : base(message)
    //    {
    //        LogMessage.Exit(message);
    //    }
    //}

    /// <summary>
    /// Trace info for such Exception is not logged. Used for foreseen errors.
    /// </summary>
    public class Exception2 : Exception
    {
        public Exception2(string message)
            : base(message)
        {
        }

        public Exception2(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

