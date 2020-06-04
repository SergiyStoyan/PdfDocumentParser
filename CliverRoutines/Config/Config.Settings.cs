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
        /// Alternative to .NET settings. Inheritors of this class are automatically managed by Config.
        /// Practically in the application only one field is expected to be declared per cetain Settings type, but generally there can be any number of fields of the same Settings type.
        /// By defaul a Settings type object is attached to the field to which it was set. 
        /// A copy of a Settings type object, if created, still keeps the information of the field of its origin despite of the fact it is not the value of this field.
        /// </summary>
        abstract public partial class Settings
        {
            #region engine internal API

            internal Config.SettingsField Field { get; private set; }

            internal static Settings Create(Config.SettingsField settingsField, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
            {
                Settings settings = create(settingsField, reset, throwExceptionIfCouldNotLoadFromStorageFile);
                settings.Field = settingsField;
                settings.Loaded();
                return settings;
            }
            static Settings create(Config.SettingsField settingsField, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
            {
                if (!reset && File.Exists(settingsField.File))
                    try
                    {
                        return (Settings)Cliver.Serialization.Json.Load(settingsField.Type, settingsField.File);
                    }
                    catch (Exception e)
                    {
                        if (throwExceptionIfCouldNotLoadFromStorageFile)
                            throw new Exception("Error while loading settings " + settingsField.FullName + " from file " + settingsField.File, e);
                    }
                if (File.Exists(settingsField.InitFile))
                {
                    FileSystemRoutines.CopyFile(settingsField.InitFile, settingsField.File, true);
                    return (Settings)Cliver.Serialization.Json.Load(settingsField.Type, settingsField.InitFile);
                }
                return (Settings)Activator.CreateInstance(settingsField.Type);
            }
            internal enum InitMode
            {
                LOAD,
                LOAD_OR_NEW,
                NEW
            }

            #endregion

            //public void ChangeField(FieldInfo newFieldInfo)
            //{
            //    throw new Exception("TBD");
            //    if (Field.Type != newFieldInfo.FieldType)
            //        throw new Exception("New field type differs from ");
            //}

            /// <summary>
            /// Field full name is the string that is used in the code to refer to this field. It is the type path of the field. 
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __FieldFullName { get { return Field.FullName; } }

            /// <summary>
            /// Path of the storage file. It consists of a directory which defined by Settings type and file name which is the field's full name.
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __File { get { return Field.File; } }

            /// <summary>
            /// Path of the init file. It consists of the directory where the entry assembly is loacted and file name which is the field's full name.
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __InitFile { get { return Field.InitFile; } }

            /// <summary>
            /// Indicates if this Settings type object is managed by Congif (e.g. it's set to some Settings type field) 
            /// or it was copied and is not the value of a Settings type field managed by Config.
            /// </summary>
            public bool __IsCopy()
            {
                return Config.FindSettingsField(this).GetObject() != this;
            }

            public void Save()
            {
                lock (this)
                {
                    Saving();
                    Cliver.Serialization.Json.Save(__File, this, __Indented, true);
                    Saved();
                }
            }

            [Newtonsoft.Json.JsonIgnore]
            public bool __Indented = true;

            virtual public void Loaded()
            {

            }

            virtual public void Saving()
            {

            }

            virtual public void Saved()
            {

            }

            //void importValues(Settings s)!!!Declined because such copying may bring to the mess in a object's state (if any)
            //{
            //    foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            //        settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
            //}

            /// <summary>
            /// Reset this object to its default values.
            /// First it tries to load from the initial file located in the app's directory. 
            /// If this file does not exist, it resets to the hardcoded values.
            /// </summary>
            public void Reset(/*bool ignoreInitFile = false*/)
            {
                Field.SetObject(Create(Field, true, true));
            }

            /// <summary>
            /// First it tries to load from the storage file.
            /// If this file does not exist, it tries to reset from the initial file located in the app's directory. 
            /// If this file does not exist, it resets to the hardcoded values.
            /// </summary>
            /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
            public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
            {
                Field.SetObject(Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile));
            }

            /// <summary>
            /// Creates a new instance of this Settings type with default values.
            /// First it tries to load a Settings object from the initial file located in the app's directory. 
            /// If this file does not exist, it creates a Settings object with the hardcoded values.
            /// </summary>
            /// <typeparam name="S"></typeparam>
            /// <returns></returns>
            public S CreateResetInstance<S>() where S : Settings, new()
            {
                return (S)Create(Field, true, true);
            }

            /// <summary>
            /// First tries to load a new instance of this Settings type from its storage file.
            /// If the file does not exist, it tries to load a Settings object from the initial file located in the app's directory. 
            /// If this file does not exist, it creates a Settings object with the hardcoded values.
            /// </summary>
            /// <typeparam name="S"></typeparam>
            /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
            /// <returns></returns>
            public S CreateReloadedInstance<S>(bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
            {
                return (S)Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile);
            }

            /// <summary>
            /// Compares actual values of this object with the values in the storage file or default values.
            /// </summary>
            /// <returns>False if the values are the identical.</returns>
            public bool __IsChanged()
            {
                return Serialization.Json.IsEqual(this, Create(Field, false, false));
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
            [Newtonsoft.Json.JsonIgnore]
            public abstract string __StorageDir { get; }

            /// <summary>
            /// Folder where storage files of this Settings type are stored by Config engine.
            /// </summary>
            public static string GetStorageDir(Type settingsType)
            {
                Settings s = (Settings)Activator.CreateInstance(settingsType);
                return s.__StorageDir;
            }
        }
    }
}