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
        /// Alternative to .NET settings. Inheritors of this class in the application are automatically managed by Config.
        /// From practical point of view, usually only one field is to be declared per certain Settings type, but generally there can be any number of fields of the same Settings type.
        /// A Settings object always belongs to one Settings field even it is not the value of this field (i.e. it is detached). 
        /// </summary>
        abstract public partial class Settings
        {
            [Newtonsoft.Json.JsonIgnore]
            internal SettingsField Field { get; private set; }

            internal static Settings Create(SettingsField settingsField, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
            {
                Settings settings = create(settingsField, reset, throwExceptionIfCouldNotLoadFromStorageFile);
                settings.Field = settingsField;
                settings.Loaded();
                return settings;
            }
            static Settings create(SettingsField settingsField, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
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

            /// <summary>
            /// Field full name is the string that is used in the code to refer to this field. It is the type path of the field. 
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __FullName { get { return Field.FullName; } }

            /// <summary>
            /// Path of the storage file. It consists of a directory which defined by Settings type and file name which is the field's full name.
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __File { get { return Field.File; } }

            /// <summary>
            /// Path of the init file. It consists of the directory where the entry assembly is located and file name which is the field's full name.
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string __InitFile { get { return Field.InitFile; } }

            /// <summary>
            /// Indicates whether this Settings object is value of some Settings field or it is not.
            /// If Settings object copies are created in the application then this method allows to indicate them before calling Reload(), Reset() and Save()
            /// </summary>
            public bool IsDetached()
            {
                return Config.GetSettings(Field.FullName) != this;
            }

            /// <summary>
            /// Serializes this Settings object to the storage file.
            /// </summary>
            public void Save()
            {
                //if (IsDetached())
                //    throw new Exception("This method cannot be performed because this Settings object it is not value of the field " + __FieldFullName);
                lock (this)
                {
                    Saving();
                    Cliver.Serialization.Json.Save(__File, this, __Indented, true);
                    Saved();
                }
            }

            [Newtonsoft.Json.JsonIgnore]
            public bool __Indented = true;

            virtual public void Loaded() { }

            virtual public void Saving() { }

            virtual public void Saved() { }

            //void importValues(Settings s)!!!Declined because such copying may bring to the mess in a object's state (if any)
            //{
            //    foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            //        settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
            //}

            /// <summary>
            /// Replaces the value of the field defined by __FieldFullName with a new object initiated with default values. 
            /// Tries to load it from the initial file located in the app's directory. 
            /// If this file does not exist, it creates an object with the hardcoded values.
            /// Avoid calling this method on a detached Settings object as it leads to a confusing effect. 
            /// </summary>
            public void Reset(/*bool ignoreInitFile = false*/)
            {
                //if (IsDetached())
                //    throw new Exception("This method cannot be performed because this Settings object it is not value of the field " + __FieldFullName);
                Field.SetObject(Create(Field, true, true));
            }

            /// <summary>
            /// Replaces the value of the field defined by __FieldFullName with a new object initiated with stored values.
            /// Tries to load it from the storage file.
            /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
            /// If this file does not exist, it creates an object with the hardcoded values.
            /// Avoid calling this method on a detached Settings object as it leads to a confusing effect. 
            /// </summary>
            /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
            public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
            {
                //if (IsDetached())
                //    throw new Exception("This method cannot be performed because this Settings object it is not value of the field " + __FieldFullName);
                Field.SetObject(Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile));
            }

            /// <summary>
            /// Creates a new instance of this Settings type initiated with default values.
            /// Tries to load it from the initial file located in the app's directory. 
            /// If this file does not exist, it creates an object with the hardcoded values.
            /// </summary>
            /// <typeparam name="S"></typeparam>
            /// <returns></returns>
            public S CreateResetInstance<S>() where S : Settings, new()
            {
                return (S)Create(Field, true, true);
            }

            /// <summary>
            /// Creates a new instance of this Settings type initiated with stored values.
            /// Tries to load it from the storage file.
            /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
            /// If this file does not exist, it creates an object with the hardcoded values.
            /// </summary>
            /// <typeparam name="S"></typeparam>
            /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
            /// <returns></returns>
            public S CreateReloadedInstance<S>(bool throwExceptionIfCouldNotLoadFromStorageFile = false) where S : Settings, new()
            {
                return (S)Create(Field, false, throwExceptionIfCouldNotLoadFromStorageFile);
            }

            /// <summary>
            /// Compares serializable properties of this object with the ones stored in the file or the default ones.
            /// </summary>
            /// <returns>False if the values are the identical.</returns>
            public bool IsChanged()
            {
                lock (this)
                {
                    return !Serialization.Json.IsEqual(this, Create(Field, false, false));
                }
            }

            /// <summary>
            /// Indicates that the attributed Settings field should not be initiated by Config by default.
            /// </summary>
            public class Optional : Attribute { }

            /// <summary>
            /// Folder where storage files of this Settings type are to be saved by Config.
            /// Each Settings type has to define it.
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public abstract string __StorageDir { get; }

            /// <summary>
            /// Folder where storage files of this Settings type are saved by Config.
            /// </summary>
            public static string GetStorageDir(Type settingsType)
            {
                Settings s = (Settings)Activator.CreateInstance(settingsType);
                return s.__StorageDir;
            }
        }
    }
}