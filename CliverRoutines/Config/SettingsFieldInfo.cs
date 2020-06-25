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
    /// Settings attributes which are defined by a Settings field.
    /// </summary>
    public class SettingsFieldInfo
    {
        /// <summary>
        /// Settings' full name is the string that is used in code to refer to this field/property. 
        /// It defines exactly Settings field/property in the code but has nothing to do with the type of it. 
        /// </summary>
        public readonly string FullName;

        internal readonly FieldInfo FieldInfo;

        /// <summary>
        /// Path of the storage file. It consists of a directory which defined by the Settings based type and the file name which is the field's full path in the code.
        /// </summary>
        public readonly string File;

        /// <summary>
        /// Path of the init file. It consists of the directory where the entry assembly is located and the file name which is the field's full name in the code.
        /// </summary>
        public readonly string InitFile;

        /// <summary>
        /// Whether serialization to string is done with indention.
        /// </summary>
        public bool Indented;

        /// <summary>
        /// Settings based type.
        /// </summary>
        internal readonly Type Type;

        internal Settings GetObject()
        {
            lock (this)
            {
                return (Settings)FieldInfo.GetValue(null);
            }
        }

        internal void SetObject(Settings settings)
        {
            lock (this)
            {
                FieldInfo.SetValue(null, settings);
            }
        }

        internal SettingsFieldInfo(FieldInfo settingsTypeFieldInfo)
        {
            FullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;
            FieldInfo = settingsTypeFieldInfo;
            Settings s = (Settings)Activator.CreateInstance(settingsTypeFieldInfo.FieldType);
            File = s.__StorageDir + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            InitFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            Type = settingsTypeFieldInfo.FieldType;
        }
    }
}