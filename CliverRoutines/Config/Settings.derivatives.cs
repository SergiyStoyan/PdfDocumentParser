//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver
{
    /// <summary>
    /// Instances of this class are to be stored in CommonApplicationData folder.
    /// CliverWinRoutines lib contains AppSettings adapted for Windows.
    /// </summary>
    public class AppSettings : Settings
    {
        /*//version with static __StorageDir
        /// <summary>
        /// (!)A Settings derivative or some of its ancestors must define this public static getter to specify the storage directory.
        /// </summary>
        new public static string __StorageDir { get; private set; } = Log.AppCompanyCommonDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME; 
        */

        /// <summary>
        /// Storage folder for this Settings located in CommonApplicationData.
        /// </summary>
        sealed public override string __StorageDir { get; protected set; } = StorageDir;
        /// <summary>
        /// Storage folder for this Settings located in CommonApplicationData.
        /// </summary>
        public static readonly string StorageDir = Log.AppCompanyCommonDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    /// <summary>
    /// Instances of this class are to be stored in LocalApplicationData folder.
    /// </summary>
    public class UserSettings : Settings
    {
        /*//version with static __StorageDir
        /// <summary>
        /// (!)A Settings derivative or some of its ancestors must define this public static getter to specify the storage directory.
        /// </summary>
        new public static string __StorageDir { get; private set; } = Log.AppCompanyUserDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
        */

        /// <summary>
        /// Storage folder for this Settings located in LocalApplicationData.
        /// </summary>
        sealed public override string __StorageDir { get; protected set; } = StorageDir;
        /// <summary>
        /// Storage folder for this Settings located in LocalApplicationData.
        /// </summary>
        public static readonly string StorageDir = Log.AppCompanyUserDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }
}