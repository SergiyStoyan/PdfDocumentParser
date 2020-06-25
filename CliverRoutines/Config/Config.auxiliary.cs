//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Cliver
{
    public partial class Config
    {
        /// <summary>
        /// Creates a new instance of the given Settings field initiated with default values.
        /// Tries to load values from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static S CreateResetInstance<S>(S settings) where S : Settings, new()
        {
            if (settings.__Info == null)
                throw new Exception("This method cannot be performed on a Settings object which has __Info undefined.");
            return (S)Settings.Create(settings.__Info, true, true);
        }

        /// <summary>
        /// Creates a new instance of the given Settings field initiated with stored values.
        /// Tries to load values from the storage file.
        /// If this file does not exist, it tries to load values from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        /// <returns></returns>
        public static S CreateReloadedInstance<S>(S settings, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            if (settings.__Info == null)
                throw new Exception("This method cannot be performed on a Settings object which has __Info undefined.");
            return (S)Settings.Create(settings.__Info, false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Copies storage files of all the Settings fields initiated by Config, to the specified directory.
        /// So this method makes effect only if called after Reload() or Reset(). 
        /// </summary>
        /// <param name="toDirectory">folder where files are to be copied</param>
        static public void ExportStorageFiles(string toDirectory)
        {
            lock (settingsFullNames2SettingsFieldInfo)
            {
                string d = FileSystemRoutines.CreateDirectory(toDirectory + System.IO.Path.DirectorySeparatorChar + CONFIG_FOLDER_NAME);
                foreach (SettingsFieldInfo settingsFieldInfo in enumSettingsFieldInfos())
                {
                    if (File.Exists(settingsFieldInfo.File))//it can be absent if default settings are used still
                        File.Copy(settingsFieldInfo.File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(settingsFieldInfo.File));
                    else if (File.Exists(settingsFieldInfo.InitFile))
                        File.Copy(settingsFieldInfo.InitFile, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(settingsFieldInfo.InitFile));
                    else
                    {
                        Settings s = Settings.Create(settingsFieldInfo, true, true);
                        s.Save();
                        File.Move(settingsFieldInfo.File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(settingsFieldInfo.File));
                    }
                }
            }
        }

        //// ???what would it be needed for?
        //public static S CreateResetInstance<S>(string settingsFullName) where S : Settings, new()
        //{
        //    return (S)Settings.Create(GetSettingsFieldInfo(settingsFullName), true, true);
        //}

        //// ???what would it be needed for?
        //public static S CreateReloadedInstance<S>(string settingsFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        //{
        //    return (S)Settings.Create(GetSettingsFieldInfo(settingsFullName), false, throwExceptionIfCouldNotLoadFromStorageFile);
        //}

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        public static void Reset(string settingsFullName) 
        {
            SettingsFieldInfo sf = GetSettingsFieldInfo(settingsFullName);
            Settings s = Settings.Create(sf, true, true);
            sf.SetObject(s);
        }

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        public static void Reload(string settingsFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            SettingsFieldInfo sf = GetSettingsFieldInfo(settingsFullName);
            Settings s = Settings.Create(GetSettingsFieldInfo(settingsFullName), false, throwExceptionIfCouldNotLoadFromStorageFile);
            sf.SetObject(s);
        }

        ///// <summary>
        //// ???what would it be needed for?
        ///// Returns the Settings object which is set to the field identified by the field's full name.
        ///// </summary>
        ///// <param name="settingsFullName">full name of Settings field; it equals to the name of its file without extention</param>
        ///// <returns>The Settings object which is set to the field</returns>
        //static public Settings GetSettings(string settingsFullName)
        //{
        //    SettingsFieldInfo sf = GetSettingsFieldInfo(settingsFullName);
        //    return sf.GetObject();
        //}
    }
}