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
        /// Copies storage files of all the Settings fields initiated by Config, to the specified directory.
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
        
        /// <summary>
        /// Allows to get the Settings field's properties before its value has been created (i.e. before the Settings field has been initialized).
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        /// <returns>Settings field's properties</returns>
        public static SettingsField GetSettingsField(string settingsFieldFullName)
        {
            return enumSettingsFields().FirstOrDefault(a => a.FullName == settingsFieldFullName);
        }
    }
}