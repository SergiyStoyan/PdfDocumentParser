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
using System.Diagnostics;

namespace Cliver
{
    /// <summary>
    /// Settings type field's properties.
    /// </summary>
    public class SettingsField
    {
        /// <summary>
        /// Field full name is the string that is used in the code to refer to this field. It is the type path of the field. 
        /// </summary>
        public readonly string FullName;

        internal readonly FieldInfo Info;

        /// <summary>
        /// Path of the storage file. It consists of a directory which defined by Settings type and file name which is the field's full name.
        /// </summary>
        public readonly string File;

        /// <summary>
        /// Path of the init file. It consists of the directory where the entry assembly is located and file name which is the field's full name.
        /// </summary>
        public readonly string InitFile;

        internal readonly Type Type;

        internal Settings GetObject()
        {
            lock (this)
            {
                return (Settings)Info.GetValue(null);
            }
        }

        internal void SetObject(Settings settings)
        {
            lock (this)
            {
                Info.SetValue(null, settings);
            }
        }

        internal SettingsField(FieldInfo settingsTypeFieldInfo)
        {
            FullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;
            Info = settingsTypeFieldInfo;
            File = Settings.GetStorageDir(settingsTypeFieldInfo.FieldType) + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            InitFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            Type = settingsTypeFieldInfo.FieldType;
        }
    }
}