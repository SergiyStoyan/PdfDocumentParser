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
using System.Diagnostics;

namespace Cliver
{
    public partial class Config
    {
        static public void ExportSettingsFiles(string toDirectory)
        {
            lock (fieldFullNames2settingsObject)
            {
                string d = FileSystemRoutines.CreateDirectory(toDirectory + System.IO.Path.DirectorySeparatorChar + CONFIG_FOLDER_NAME);
                foreach (Serializable s in fieldFullNames2settingsObject.Values)
                    if (File.Exists(s.__File))//it can be absent if default settings are used still
                        File.Copy(s.__File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
            }
        }

        #region Routines for individual Settings objects in the config scope.

        /// <summary>
        /// Returns the Settings object identified by the full name of the field to which the object belongs.
        /// The object is not newly created but must already exist in the config scope.
        /// </summary>
        /// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        /// <returns>The Settings object which was previously created by Config</returns>
        static public Settings GetSettings(string fullName)
        {
            lock (fieldFullNames2settingsObject)
            {
                Settings s = null;
                fieldFullNames2settingsObject.TryGetValue(fullName, out s);
                return s;
            }
        }

        /// <summary>
        /// Can be called from code when ordered load is required due to dependencies.
        /// </summary>
        /// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        static public void ReloadField(string fullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            lock (fieldFullNames2settingsObject)
            {
                foreach (IEnumerable<FieldInfo> settingsTypeFieldInfos in enumSettingsTypesFieldInfos())
                {
                    FieldInfo settingsTypeFieldInfo = settingsTypeFieldInfos.Where(a => (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
                    if (settingsTypeFieldInfo != null)
                    {
                        Serializable serializable = getSerializable(settingsTypeFieldInfo.FieldType, fullName, false, throwExceptionIfCouldNotLoadFromStorageFile);
                        settingsTypeFieldInfo.SetValue(null, serializable);
                        return;
                    }
                }
                throw new Exception("Field '" + fullName + "' was not found.");
            }
        }

        /// <summary>
        /// Returns the file path of the Settings object before the Settings object has been created. 
        /// </summary>
        /// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        /// <returns>Settings object's file path</returns>
        public static string GetFieldFile(string fullName)
        {
            lock (fieldFullNames2settingsObject)
            {
                foreach (IEnumerable<FieldInfo> settingsTypeFieldInfos in enumSettingsTypesFieldInfos())
                {
                    FieldInfo settingsTypeFieldInfo = settingsTypeFieldInfos.Where(a => (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
                    if (settingsTypeFieldInfo == null)
                        continue;
                    return Settings.GetConfigStorageDir(settingsTypeFieldInfo.FieldType) + System.IO.Path.DirectorySeparatorChar + fullName + "." + FILE_EXTENSION;
                }
                throw new Exception("Field '" + fullName + "' was not found.");
            }
        }
        #endregion
    }
}