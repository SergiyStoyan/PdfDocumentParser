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
        /// Tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static S CreateResetCopy<S>(S settings) where S : Settings, new()
        {
            return (S)Settings.Create(settings.Field, true, true);
        }

        /// <summary>
        /// Creates a new instance of the given Settings field initiated with stored values.
        /// Tries to load it from the storage file.
        /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        /// <returns></returns>
        public static S CreateReloadedCopy<S>(S settings, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            return (S)Settings.Create(settings.Field, false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Copies storage files of all the Settings fields initiated by Config, to the specified directory.
        /// So this method makes effect only if called after Reload() or Reset(). 
        /// </summary>
        /// <param name="toDirectory">folder where files are to be copied</param>
        static public void ExportStorageFiles(string toDirectory)
        {
            lock (fieldFullNames2SettingsField)
            {
                string d = FileSystemRoutines.CreateDirectory(toDirectory + System.IO.Path.DirectorySeparatorChar + CONFIG_FOLDER_NAME);
                foreach (SettingsField settingsField in fieldFullNames2SettingsField.Values)
                    if (File.Exists(settingsField.File))//it can be absent if default settings are used still
                        File.Copy(settingsField.File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(settingsField.File));
            }
        }

        /// <summary>
        /// Allows to get the Settings field's properties before its value has been created (i.e. before the Settings field has been initialized).
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        /// <returns>Settings field's properties</returns>
        public static SettingsField GetSettingsField(string settingsFieldFullName)
        {
            lock (fieldFullNames2SettingsField)
            {
                if (!fieldFullNames2SettingsField.TryGetValue(settingsFieldFullName, out SettingsField sf))
                {
                    sf = enumSettingsFields().FirstOrDefault(a => a.FullName == settingsFieldFullName);
                    fieldFullNames2SettingsField[settingsFieldFullName] = sf;
                }
                return sf;
            }
        }

        // ???what would it be needed for?
        public static S CreateResetCopy<S>(string settingsFieldFullName) where S : Settings, new()
        {
            return (S)Settings.Create(GetSettingsField(settingsFieldFullName), true, true);
        }

        // ???what would it be needed for?
        public static S CreateReloadedCopy<S>(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            return (S)Settings.Create(GetSettingsField(settingsFieldFullName), false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        // ???what would it be needed for?
        public static void Reset<S>(string settingsFieldFullName) where S : Settings, new()
        {
            SettingsField sf = GetSettingsField(settingsFieldFullName);
            S s = (S)Settings.Create(sf, true, true);
            sf.SetObject(s);
        }

        // ???what would it be needed for?
        public static void Reload<S>(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            SettingsField sf = GetSettingsField(settingsFieldFullName);
            S s = (S)Settings.Create(GetSettingsField(settingsFieldFullName), false, throwExceptionIfCouldNotLoadFromStorageFile);
            sf.SetObject(s);
        }
    }
}