//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
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
        /// Copies storage files of all the Settings type fields initiated in Config.
        /// So this method must be called only after Reload() or Reset(). 
        /// </summary>
        /// <param name="toDirectory">folder where files are to be copied</param>
        static public void ExportSettingsFiles(string toDirectory)
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
        /// Returns the Settings object identified by the full name of the field to which the object belongs.
        /// The object is not newly created but must exist in Config.
        /// So this method must be called only after Reload() or Reset(). 
        /// </summary>
        /// <param name="settingsTypeFieldFullName">full name of Settings type field; it equals to the name of its file without extention</param>
        /// <returns>The Settings object which is stored in Config</returns>
        static public Settings GetSettings(string settingsTypeFieldFullName)
        {
            lock (fieldFullNames2SettingsField)
            {
               if(! fieldFullNames2SettingsField.TryGetValue(settingsTypeFieldFullName, out SettingsField settingsField))
                return null;
                return settingsField.GetObject();
            }
        }

        #region Routines for individual Settings type fields.

        /// <summary>
        /// Returns full names of the fields of the given Settings type which are stored in Config. 
        /// So this method must be called only after Reload() or Reset(). 
        /// Practically, it is expected to be only one field per Settings type but in general it can be any number of them.
        /// The name of the storage file without extension is the same as the field full name.
        /// </summary>
        /// <param name="settings">Settings type object</param>
        /// <returns>full name of the Settings type field</returns>
        public List<string> FindFieldFullNames(Settings settings)
        {
            lock (fieldFullNames2SettingsField)
            {
                return fieldFullNames2SettingsField.Where(kv => kv.Value.GetObject() == settings).Select(kv => kv.Value.FullName).ToList();
            }
            //return hostingClassType.GetField(settingsFieldName) + "." + settingsFieldName;
            //return settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;
        }

        ///// <summary>
        ///// !!!Deprecated. Use InitialzingOrderedSettingsTypes instead of it.
        ///// Can be called when ordered load is required due to dependencies.
        ///// </summary>
        ///// <param name="settingsTypeFieldFullName">Settings field's full name which is the name of its file without extention</param>
        ///// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        //static public void ReloadField(string settingsTypeFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        //{
        //    lock (fieldFullNames2SettingsField)
        //    {
        //        foreach (IEnumerable<FieldInfo> settingsTypeFieldInfos in enumSettingsTypesFieldInfos())
        //        {
        //            FieldInfo settingsTypeFieldInfo = settingsTypeFieldInfos.Where(a => (a.DeclaringType.FullName + "." + a.Name) == settingsTypeFieldFullName).FirstOrDefault();
        //            if (settingsTypeFieldInfo != null)
        //            {
        //                Serializable serializable = getSerializable(settingsTypeFieldInfo.FieldType, settingsTypeFieldFullName, false, throwExceptionIfCouldNotLoadFromStorageFile);
        //                settingsTypeFieldInfo.SetValue(null, serializable);
        //                return;
        //            }
        //        }
        //        throw new Exception("Field '" + settingsTypeFieldFullName + "' was not found.");
        //    }
        //}

        /// <summary>
        /// Returns the file path of the Settings object before the object has been created (i.e. the Settings field has been initialized).
        /// So this method can be called anytime. 
        /// </summary>
        /// <param name="settingsTypeFieldFullName">full name of Settings type field; it equals to the name of its file without extention</param>
        /// <returns>Settings object's storage file path</returns>
        public static string GetSettingsFile(string settingsTypeFieldFullName)
        {
            lock (fieldFullNames2SettingsField)
            {
                SettingsField settingsField = enumSettingsTypeFieldInfos().Where(a => (a.FieldInfo.DeclaringType.FullName + "." + a.FieldInfo.Name) == settingsTypeFieldFullName).FirstOrDefault();
                if (settingsField == null)
                    throw new Exception("Field '" + settingsTypeFieldFullName + "' was not found.");
                return settingsField.File;
            }
        }

        /// <summary>
        /// Returns the file path of the Settings object before the object has been created (i.e. the Settings field has been initialized). 
        /// So this method can be called anytime. 
        /// </summary>
        /// <param name="settingsTypeFieldInfo">FieldInfo of Settings type field</param>
        /// <returns>Settings object's storage file path</returns>
        public static string GetSettingsFile(FieldInfo settingsTypeFieldInfo)
        {
            return new SettingsField(settingsTypeFieldInfo).File;
        }


        #endregion
    }
}