//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver
{
    public partial class Config
    {
        /// <summary>
        /// Creates a new instance of the given Settings field initiated with default values.
        /// Tries to load values from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// The new instance shares the same __Info object with the original instance.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static S CreateResetClone<S>(this S settings) where S : Settings, new()
        {
            if (settings.__Info == null)
                throw new Exception("This method cannot be performed on a Settings object which has __Info not defined.");
            return (S)Settings.Create(settings.__Info, true, true);
        }

        /// <summary>
        /// Creates a new instance of the given Settings field initiated with stored values.
        /// Tries to load values from the storage file.
        /// If this file does not exist, it tries to load values from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// The new instance shares the same __Info object with the original instance.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        /// <returns></returns>
        public static S CreateReloadedClone<S>(this S settings, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            if (settings.__Info == null)
                throw new Exception("This method cannot be performed on a Settings object which has __Info not defined.");
            return (S)Settings.Create(settings.__Info, false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Creates a new instance of the given Settings field with cloned values.
        /// The new instance shares the same __Info object with the original instance.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static S CreateClone<S>(this S settings) where S : Settings, new()
        {
            S s = Serialization.Json.Clone(settings);
            if (settings.__Info != null)
                s.__Info = settings.__Info;
            return s;
        }

        /// <summary>
        /// Copies storage files of all the Settings fields in the application to the specified directory.
        /// </summary>
        /// <param name="toDirectory">folder where files are to be copied</param>
        static public void ExportStorageFiles(string toDirectory)
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                foreach (SettingsMemberInfo sfi in EnumSettingsFieldInfos())
                //foreach (SettingsMemberInfo sfi in settingsFieldFullNames2SettingsFieldInfo.Values)
                {
                    string file2 = toDirectory + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(sfi.File);
                    if (File.Exists(sfi.File))//it can be absent if default settings are used still
                        File.Copy(sfi.File, file2);
                    else if (File.Exists(sfi.InitFile))
                        File.Copy(sfi.InitFile, file2);
                    else
                    {
                        Settings s = Settings.Create(sfi, true, true);
                        s.Save(sfi);
                        File.Move(sfi.File, file2);
                    }
                }
            }
        }

        //// ???what would it be needed for?
        //public static S CreateResetInstance<S>(string settingsFieldFullName) where S : Settings, new()
        //{
        //    return (S)Settings.Create(GetSettingsFieldInfo(settingsFieldFullName), true, true);
        //}

        //// ???what would it be needed for?
        //public static S CreateReloadedInstance<S>(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        //{
        //    return (S)Settings.Create(GetSettingsFieldInfo(settingsFieldFullName), false, throwExceptionIfCouldNotLoadFromStorageFile);
        //}

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        public static void Reset(string settingsFieldFullName)
        {
            SettingsMemberInfo sfi = GetSettingsFieldInfo(settingsFieldFullName);
            Settings s = Settings.Create(sfi, true, true);
            sfi.SetObject(s);
        }

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        public static void Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            SettingsMemberInfo sfi = GetSettingsFieldInfo(settingsFieldFullName);
            Settings s = Settings.Create(sfi, false, throwExceptionIfCouldNotLoadFromStorageFile);
            sfi.SetObject(s);
        }

        ///// <summary>
        //// ???what would it be needed for?
        ///// Returns the Settings object which is set to the field identified by the field's full name.
        ///// </summary>
        ///// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its file without extention</param>
        ///// <returns>The Settings object which is set to the field</returns>
        //static public Settings GetSettings(string settingsFieldFullName)
        //{
        //    SettingsMemberInfo sf = GetSettingsFieldInfo(settingsFieldFullName);
        //    return sf.GetObject();
        //}

    }
}