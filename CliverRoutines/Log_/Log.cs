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

        /// <summary>
        /// Shuts down the log engine and re-initializes it. Optional.
        /// </summary>
        /// <param name="mode">log configuration</param>
        /// <param name="baseDirs">directories for logging, ordered by preference. When NULL, the built-in directory list is used.</param>
        /// <param name="deleteLogsOlderThanDays">old logs that are older than the number of days will be deleted. When negative, no clean-up is performed.</param>
        public static void Initialize(Mode? mode = null, List<string> baseDirs = null, int deleteLogsOlderThanDays = 10)
        {
            lock (lockObject)
            {
                Log.CloseAll();
                if (mode != null)
                    Log.mode = (Mode)mode;
                Log.baseDirs = baseDirs;
                Log.deleteLogsOlderThanDays = deleteLogsOlderThanDays;
            }
        }
        static List<string> baseDirs = null;
        static int deleteLogsOlderThanDays = 10;
        static Mode mode = Mode.ONE_FOLDER | Mode.DEFAULT_NAMED_LOG;

        /// <summary>
        /// Log level which is passed to each log as default.
        /// </summary>
        public static Level DefaultLevel = Level.ALL;

        /// <summary>
        /// Maximum log file length in bytes which is passed to each log as default.
        /// If negative than no effect.
        /// </summary>
        public static int DefaultMaxFileSize = -1;

        /// <summary>
        /// Pattern of time recorded before a log message. See DateTime.ToString() format.
        /// </summary>
        public static string TimePattern = "[dd-MM-yy HH:mm:ss] ";

        /// <summary>
        /// Whether thread log indexes of closed logs can be reused.
        /// </summary>
        public static bool ReuseThreadLogIndexes = false;

        /// <summary>
        /// Extension of log files.
        /// </summary>
        public static string FileExtension = "log";

        /// <summary>
        /// Suffix to the RootDir folder name.
        /// </summary>
        public static string RootDirNameSuffix = @"_Logs";

        /// <summary>
        /// Log configuration.
        /// </summary>
        public enum Mode : uint
        {
            /// <summary>
            /// No session folder is created. Log files are in one folder.
            /// It is default option if not FOLDER_PER_SESSION, otherwise, ignored.
            /// </summary>
            ONE_FOLDER = 1,//0001
            /// <summary>
            /// Each session creates its own folder.
            /// </summary>
            FOLDER_PER_SESSION = 2,//0010
            /// <summary>
            /// Default log is named log.
            /// It is default option if not THREAD_DEFAULT_LOG, otherwise, ignored.
            /// </summary>
            DEFAULT_NAMED_LOG = 4,//0100
            /// <summary>
            /// Default log is thread log.
            /// </summary>
            DEFAULT_THREAD_LOG = 8,//1000
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

        ///// <summary>
        ///// The head session's directory.
        ///// </summary>
        //public static string Dir
        //{
        //    get
        //    {
        //        return Head.Dir;
        //    }
        //}

        /// <summary>
        /// Message importance levels.
        /// </summary>
        public enum Level
        {
            NONE,
            ERROR,
            WARNING,
            INFORM,
            ALL
        }

        /// <summary>
        /// Message types.
        /// </summary>
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
                NamedWriter.CloseAll();
                rootDir = null;
                headSession = null;
            }
        }

        /// <summary>
        ///Directory where logs and log sessions are written.
        /// </summary>
        public static string RootDir 
        {
            get
            {
                if (rootDir == null)
                    setRootDir(DefaultLevel > Level.NONE);
                return rootDir;
            }
        }
        static string rootDir = null;
        static Thread deletingOldLogsThread = null;
        public static Func<string, bool> DeleteOldLogsDialog = null;

        static void setRootDir(bool create)
        {
            lock (lockObject)
            {
                if (rootDir != null)
                {
                    if (!create)
                        return;
                    if (Directory.Exists(rootDir))
                        return;
                }
                List<string> baseDirs = new List<string> {
                                CompanyUserDataDir,
                                CompanyCommonDataDir,
                                Log.AppDir,
                                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                                Path.GetTempPath() + Path.DirectorySeparatorChar + CompanyName + Path.DirectorySeparatorChar,
                                };
                if (Log.baseDirs != null)
                    baseDirs.InsertRange(0, Log.baseDirs);
                foreach (string baseDir in baseDirs)
                {
                    BaseDir = baseDir;
                    rootDir = BaseDir + Path.DirectorySeparatorChar + Log.ProgramName + RootDirNameSuffix;
                    if (create)
                        try
                        {
                            if (!Directory.Exists(rootDir))
                                FileSystemRoutines.CreateDirectory(rootDir);
                            string testFile = rootDir + Path.DirectorySeparatorChar + "test";
                            File.WriteAllText(testFile, "test");
                            File.Delete(testFile);
                            break;
                        }
                        catch //(Exception e)
                        {
                            rootDir = null;
                        }
                }
                if (rootDir == null)
                    throw new Exception("Could not access any log directory.");
                rootDir = PathRoutines.GetNormalizedPath(rootDir, false);
                if (Directory.Exists(rootDir) && deleteLogsOlderThanDays >= 0)
                    deletingOldLogsThread = ThreadRoutines.Start(() => { Log.DeleteOldLogs(deleteLogsOlderThanDays, DeleteOldLogsDialog); });//to avoid a concurrent loop while accessing the log file from the same thread 
            }
        }

        /// <summary>
        ///Actual base directory where RootDir is created.
        /// </summary>
        public static string BaseDir { get; private set; }

        /// <summary>
        /// Creates or retrieves a session-less log writer which allows continuous writing to the same log file in Log.RootDir. 
        /// </summary>
        /// <param name="name">log name</param>
        /// <returns>wirter</returns>
        static public NamedWriter Get(string name)
        {
            return NamedWriter.Get(name);
        }
    }

    /// <summary>
    /// Trace info for such Exception is not logged. 
    /// It is intended for foreseen errors.
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

