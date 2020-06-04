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
        /// Copies storage files of all the Settings type fields initiated by Config, to the specified directory.
        /// So this method makes effect only if called after Reload() or Reset(). 
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

        #region Routines for individual Settings type fields.

        ///// <summary>
        ///// Returns full names of the fields of the given Settings type which are stored in Config. 
        ///// So this method must be called only after Reload() or Reset(). 
        ///// Practically, it is expected to be only one field per Settings type but in general it can be any number of them.
        ///// Also, Settings object might be not set to any field at all in which case the empty list is returned.
        ///// </summary>
        ///// <param name="settings">Settings type object</param>
        ///// <returns>full name of the Settings type field</returns>
        //public List<string> FindFieldFullNames(Settings settings)
        //{
        //    lock (fieldFullNames2SettingsField)
        //    {
        //        return fieldFullNames2SettingsField.Where(kv => kv.Value.GetObject() == settings).Select(kv => kv.Value.FullName).ToList();
        //    }
        //    //return hostingClassType.GetField(settingsFieldName) + "." + settingsFieldName;
        //    //return settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;
        //}

        /// <summary>
        /// Returns the file path of the Settings object before the object has been created (i.e. the Settings field has been initialized).
        /// So this method can be called anytime. 
        /// </summary>
        /// <param name="settingsTypeFieldFullName">full name of Settings type field; it equals to the name of its file without extention</param>
        /// <returns>Settings object's storage file path</returns>
        public static string GetSettingsFile(string settingsTypeFieldFullName)
        {
            SettingsField settingsField = enumSettingsTypeFieldInfos().FirstOrDefault(a => a.FullName == settingsTypeFieldFullName);
            if (settingsField == null)
                throw new Exception("Field '" + settingsTypeFieldFullName + "' was not found.");
            return settingsField.File;
        }

        #endregion
    }
}