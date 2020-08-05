//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
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

        public static void Initialize(Mode mode, List<string> primaryBaseDirs = null, Level defaultLevel = Level.ALL, int deleteLogsOlderDays = 10, int defaultMaxFileSize = -1, string timePattern = "[dd-MM-yy HH:mm:ss] ")
        {
            lock (lockObject)
            {
                Log.CloseAll();
                Log.mode = mode;
                Log.primaryBaseDirs = primaryBaseDirs;
                Log.deleteLogsOlderDays = deleteLogsOlderDays;
                Log.defaultLevel = defaultLevel;
                Log.defaultMaxFileSize = defaultMaxFileSize;
                Log.timePattern = timePattern;
            }
        }
        static List<string> primaryBaseDirs = null;
        static int deleteLogsOlderDays = 10;
        static Mode mode = Mode.ALL_LOGS_ARE_IN_SAME_FOLDER;
        static Level defaultLevel = Level.ALL;
        static int defaultMaxFileSize = -1;
        static string timePattern= "[dd-MM-yy HH:mm:ss] ";

        public static bool ReuseThreadLogIndexes = false;
        public static string FileExtension = "log";

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
        /// Head session which is created by default.
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
            LOG,
            DEBUG,
            INFORM,
            WARNING,
            ERROR,
            EXIT,
            TRACE,
            //INFORM2 = 11,
            //WARNING2 = 21,
            //ERROR2 = 31,
            //EXIT2 = 41,
        }

        /// <summary>
        /// Clear all existing sessions and close all the logs.
        /// </summary>
        public static void CloseAll()
        {
            lock (lockObject)
            {
                Session.CloseAll();
                workDir = null;
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
                    setWorkDir(defaultLevel > Level.NONE);
                return workDir;
            }
        }
        static string workDir = null;
        public const string WorkDirNameSuffix = @"_Sessions";
        static Thread deletingOldLogsThread = null;
        public static Func<string, bool> DeleteOldLogsDialog = null;

        static void setWorkDir(bool create)
        {
            lock (lockObject)
            {
                if (workDir != null)
                {
                    if (!create)
                        return;
                    if (Directory.Exists(workDir))
                        return;
                }
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
                    if (create)
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
                if (Directory.Exists(workDir) && deleteLogsOlderDays >= 0)
                {
                    if (deletingOldLogsThread?.TryAbort(1000) == false)
                        throw new Exception("Could not abort deletingOldLogsThread");
                    deletingOldLogsThread = ThreadRoutines.Start(() => { Log.DeleteOldLogs(deleteLogsOlderDays, DeleteOldLogsDialog); });//to avoid a concurrent loop while accessing the log file from the same thread 
                }
                else
                    throw new Exception("Could not create log folder!");
            }
            // deletingOldLogsThread?.Join();      
        }
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

