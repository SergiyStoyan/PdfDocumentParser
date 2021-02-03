//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
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
    abstract public class SettingsMemberInfo
    {
        /// <summary>
        /// Settings' full name is the string that is used in code to refer to this field/property. 
        /// It defines exactly the Settings field/property in code but has nothing to do with the one's type. 
        /// </summary>
        public readonly string FullName;

        /// <summary>
        /// Path of the storage file. It consists of a directory which defined by the Settings based type and the file name which is the field's full name in the code.
        /// </summary>
        public readonly string File;

        /// <summary>
        /// Path of the init file. It consists of the directory where the entry assembly is located and the file name which is the field's full name in the code.
        /// </summary>
        public readonly string InitFile;

        /// <summary>
        /// Whether serialization to string is to be done with indention.
        /// </summary>
        public bool Indented;

        /// <summary>
        /// Settings derived type.
        /// </summary>
        public readonly Type Type;

        internal Settings GetObject()
        {
            lock (this)
            {
                return getObject();
            }
        }
        abstract protected Settings getObject();

        internal void SetObject(Settings settings)
        {
            lock (this)
            {
                setObject(settings);
            }
        }
        abstract protected void setObject(Settings settings);

        internal readonly SettingsAttribute Attribute;

        protected SettingsMemberInfo(MemberInfo settingsTypeMemberInfo, Type type)
        {
            Type = type;
            FullName = settingsTypeMemberInfo.DeclaringType.FullName + "." + settingsTypeMemberInfo.Name;
            Settings s = (Settings)Activator.CreateInstance(Type);
            File = s.__StorageDir + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            InitFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + FullName + "." + Config.FILE_EXTENSION;
            Attribute = settingsTypeMemberInfo.GetCustomAttributes<SettingsAttribute>(false).FirstOrDefault();
            Indented = Attribute == null ? true : Attribute.Indented;
        }
    }

    public class SettingsFieldInfo : SettingsMemberInfo
    {
        override protected Settings getObject()
        {
            return (Settings)FieldInfo.GetValue(null);
        }

        override protected void setObject(Settings settings)
        {
            FieldInfo.SetValue(null, settings);
        }

        readonly FieldInfo FieldInfo;

        internal SettingsFieldInfo(FieldInfo settingsTypeFieldInfo) : base(settingsTypeFieldInfo, settingsTypeFieldInfo.FieldType)
        {
            FieldInfo = settingsTypeFieldInfo;
        }
    }

    public class SettingsPropertyInfo : SettingsMemberInfo
    {
        override protected Settings getObject()
        {
            return (Settings)PropertyInfo.GetValue(null);
        }

        override protected void setObject(Settings settings)
        {
            PropertyInfo.SetValue(null, settings);
        }

        readonly PropertyInfo PropertyInfo;

        internal SettingsPropertyInfo(PropertyInfo settingsTypePropertyInfo) : base(settingsTypePropertyInfo, settingsTypePropertyInfo.PropertyType)
        {
            PropertyInfo = settingsTypePropertyInfo;
        }
    }
}