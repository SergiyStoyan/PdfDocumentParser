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
    /// <summary>
    /// Alternative to .NET settings. Inheritors of this class are automatically managed by Config engine.
    /// Practically only one field of this type in the application is expected to be declared per Settings type, but generally there can be any number of fields of the same Settings type.
    /// So Settings object is unaware of the field(s) it belongs to. The linking between Settings objects and fields is managed by Config.
    /// Settings type, however, defines its storage directory.
    /// </summary>
    abstract public partial class Settings
    {
        //public void ChangeField(FieldInfo newFieldInfo)
        //{
        //    throw new Exception("TBD");
        //    if (Field.Type != newFieldInfo.FieldType)
        //        throw new Exception("New field type differs from ");
        //}
        internal Config.SettingsField Field { get; private set; }
        [Newtonsoft.Json.JsonIgnore]
        public string __FullName { get { return Field.FullName; } }
        [Newtonsoft.Json.JsonIgnore]
        public string __File { get { return Field.File; } }
        [Newtonsoft.Json.JsonIgnore]
        public string __InitFile { get { return Field.InitFile; } }
        public bool __IsCopy()
        {
            return Config.GetSettingsField(this).GetObject() != this;
        }
        
        void importValues(Settings s)
        {
            foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        }

        /// <summary>
        /// Reset this object to its hardcoded values.
        /// </summary>
        public void Reset(/*bool ignoreInitFile = false*/)
        {
            importValues(Create(Field, true, true));
        }

        /// <summary>
        /// First tries to reset this object to the values stored in its storage file.
        /// Only if the file does not exist, it resets this object to its hardcoded values.
        /// </summary>
        public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            importValues(Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile));
        }

        /// <summary>
        /// Creates a new instance of this Settings type with hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public S CreateResetInstance<S>() where S : Settings, new()
        {
            return (S)Create(Field, true, true);
        }

        /// <summary>
        /// First tries to load a new instance of this Settings type from its storage file.
        /// Only if the file does not exist, it creates a new instance with hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public S CreateReloadedInstance<S>(bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
        {
            return (S)Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Compares actual values of this object with the values in the storage file.
        /// </summary>
        /// <returns>False if the values are the identical.</returns>
        public bool IsChanged()
        {
            return !Serialization.Json.IsEqual(Create(Field, false, true), this);
        }

        /// <summary>
        /// Indicates that this Settings type field should not be initiated by Config by default.
        /// </summary>
        public class Optional : Attribute
        { }

        /// <summary>
        /// Folder where storage files of this Settings type are stored by Config engine.
        /// Each Settings type has to define this path.
        /// </summary>
        public abstract string ConfigStorageDir { get; }

        /// <summary>
        /// Folder where storage files of this Settings type are stored by Config engine.
        /// </summary>
        public static string GetConfigStorageDir(Type settingsType)
        {
            Settings s = (Settings)Activator.CreateInstance(settingsType);
            return s.ConfigStorageDir;
        }
    }

    /// <summary>
    /// Inheritor of this class is stored in CommonApplicationData folder.
    /// CliverWinRoutines lib contains AppSettings adapted for Windows.
    /// </summary>
    public class AppSettings : Settings
    {
        sealed public override string ConfigStorageDir { get { return StorageDir; } }
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    /// <summary>
    /// Inheritor of this class is stored in LocalApplicationData folder.
    /// </summary>
    public class UserSettings : Settings
    {
        sealed public override string ConfigStorageDir { get { return StorageDir; } }
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }
}