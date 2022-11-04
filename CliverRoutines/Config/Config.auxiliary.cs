//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
            return (S)Settings.Create(settings.__Info, true);
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
        /// <returns></returns>
        public static S CreateReloadedClone<S>(this S settings) where S : Settings, new()
        {
            if (settings.__Info == null)
                throw new Exception("This method cannot be performed on a Settings object which has __Info not defined.");
            return (S)Settings.Create(settings.__Info, false);
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
                set_settingsFieldFullNames2SettingsFieldInfo();
                foreach (SettingsFieldInfo sfi in settingsFieldFullNames2SettingsFieldInfo.Values)
                {
                    string file2 = toDirectory + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(sfi.File);
                    if (File.Exists(sfi.File))//it can be absent if default settings are used still
                        File.Copy(sfi.File, file2);
                    else if (File.Exists(sfi.InitFile))
                        File.Copy(sfi.InitFile, file2);
                    else
                    {
                        Settings s = Settings.Create(sfi, true);
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
        //public static S CreateReloadedInstance<S>(string settingsFieldFullName) where S : Settings, new()
        //{
        //    return (S)Settings.Create(GetSettingsFieldInfo(settingsFieldFullName), false);
        //}

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        public static void Reset(string settingsFieldFullName)
        {
            GetSettingsFieldInfo(settingsFieldFullName).ResetObject();
        }

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldHostingType">full type name of the class hosting the Settings field</param>
        /// <param name="settingsFieldName">name of the Settings field</param>
        public static void Reset(Type settingsFieldHostingType, string settingsFieldName)
        {
            Reset(settingsFieldHostingType.FullName + "." + settingsFieldName);
        }

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        public static void Reload(string settingsFieldFullName)
        {
            GetSettingsFieldInfo(settingsFieldFullName).ReloadObject();
        }

        /// <summary>
        /// Can be used to initialize an optional Settings field.
        /// </summary>
        /// <param name="settingsFieldHostingType">full type name of the class hosting the Settings field</param>
        /// <param name="settingsFieldName">name of the Settings field</param>
        public static void Reload(Type settingsFieldHostingType, string settingsFieldName)
        {
            Reload(settingsFieldHostingType.FullName + "." + settingsFieldName);
        }

        /// <summary>
        /// Allows to get the Settings field's properties before its value has been created (i.e. before the Settings field has been initialized).
        /// </summary>
        /// <param name="settingsFieldHostingType">full type name of the class hosting the Settings field</param>
        /// <param name="settingsFieldName">name of the Settings field</param>
        /// <returns>Settings field's properties</returns>
        public static SettingsFieldInfo GetSettingsFieldInfo(Type settingsFieldHostingType, string settingsFieldName)
        {
            return GetSettingsFieldInfo(settingsFieldHostingType.FullName + "." + settingsFieldName);
        }

        //// ???what would it be needed for?
        ///// <summary>
        ///// Returns the Settings object which is set to the field identified by the field's full name.
        ///// </summary>
        ///// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its file without extention</param>
        ///// <returns>The Settings object which is set to the field</returns>
        //static public Settings GetSettings(string settingsFieldFullName)
        //{
        //    SettingsFieldInfo sf = GetSettingsFieldInfo(settingsFieldFullName);
        //    return sf.GetObject();
        //}  

        ///// <summary>
        ///// Get SettingsFieldInfos for the Settings type.
        ///// ATTENTION: potentially a SettingsFieldInfo object can become out of game so be careful operating with it.
        ///// </summary>
        ///// <param name="settingsType">Settings type</param>
        ///// <param name="fresh">if TRUE then the app is re-parsed looking up for the required Settings type</param>
        ///// <returns>usually it would be only 1 element in the list</returns>
        //public static List<SettingsFieldInfo> GetSettingsFieldInfos(Type settingsType, bool fresh = false)
        //{
        //    if (fresh)
        //        return getSettingsFieldInfos().Where(a => a.Type == settingsType).ToList();
        //    lock (settingsFieldFullNames2SettingsFieldInfo)
        //    {
        //        return settingsFieldFullNames2SettingsFieldInfo.Values.Where(a => a.Type == settingsType).ToList();
        //    }
        //}

        /// <summary>
        /// Get all the SettingsFieldInfo's in the app.
        /// ATTENTION: potentially, SettingsFieldInfo objects may become out of game so be careful while operating with them.
        /// </summary>
        /// <returns>SettingsFieldInfo ennumerator</returns>
        static public IEnumerable<SettingsFieldInfo> GetSettingsFieldInfos()
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                return settingsFieldFullNames2SettingsFieldInfo.Values;
            }
        }
    }
}