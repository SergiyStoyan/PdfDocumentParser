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
    /// <summary>
    /// Alternative to .NET settings. Fields/properties of Settings based types in the application are automatically managed by Config. 
    /// Those Settings field to be managed must be static and public.
    /// In practice, usually only one field is declared per a Settings derived type, but generally there can be any number of fields of the same Settings type.
    /// </summary>
    abstract public partial class Settings
    {
        /// <summary>
        /// This info identifies a certain Settings field/property in the application to which this object belongs. 
        /// For some rare hypothetical need, changing/setting it from the application is allowed (with caution!).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public SettingsFieldInfo __Info
        {
            get
            { 
                return settingsFieldInfo;
            }            
            set
            {
                if (value.Type != GetType())
                    throw new Exception("Disaccording SettingsFieldInfo Type field. Expected: " + GetType());
                settingsFieldInfo = value;
            }
        }
        public SettingsFieldInfo settingsFieldInfo = null;

        internal static Settings Create(SettingsFieldInfo settingsFieldInfo, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            Settings settings = create(settingsFieldInfo, reset, throwExceptionIfCouldNotLoadFromStorageFile);
            settings.__Info = settingsFieldInfo;
            settings.Loaded();
            return settings;
        }
        static Settings create(SettingsFieldInfo settingsFieldInfo, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            if (!reset && File.Exists(settingsFieldInfo.File))
                try
                {
                    return (Settings)Cliver.Serialization.Json.Load(settingsFieldInfo.Type, settingsFieldInfo.File);
                }
                catch (Exception e)
                {
                    if (throwExceptionIfCouldNotLoadFromStorageFile)
                        throw new Exception("Error while loading settings " + settingsFieldInfo.FullName + " from file " + settingsFieldInfo.File, e);
                }
            if (File.Exists(settingsFieldInfo.InitFile))
            {
                FileSystemRoutines.CopyFile(settingsFieldInfo.InitFile, settingsFieldInfo.File, true);
                try
                {
                    return (Settings)Cliver.Serialization.Json.Load(settingsFieldInfo.Type, settingsFieldInfo.InitFile);
                }
                catch (Exception e)
                {
                    throw new Exception("Error while loading settings " + settingsFieldInfo.FullName + " from initial file " + settingsFieldInfo.InitFile, e);
                }
            }
            return (Settings)Activator.CreateInstance(settingsFieldInfo.Type);
        }

        /// <summary>
        /// Indicates whether this Settings object is value of the Settings field indicated by __Info.
        /// </summary>
        public bool IsDetached()
        {
            return __Info == null//was created outside Config
                || Config.GetSettingsFieldInfo(__Info.FullName).GetObject() != this;//is not referenced by the field
        }

        /// <summary>
        /// Serializes this Settings object to the storage file according to __Info.
        /// </summary>
        public void Save()
        {
            lock (this)
            {
                //!!!Performing on a detached object must be allowed. Consider the real case: 
                //save an edited detached object while the attached object remains unchanged in use until the end of the process so that the changes will come into game after restart.
                if (__Info == null)
                    throw new Exception("This method cannot be performed on a Settings object which has __Info not defined.");
                Saving();
                Cliver.Serialization.Json.Save(__Info.File, this, __Info.Indented, true);
                Saved();
            }
        }

        virtual public void Loaded() { }

        virtual public void Saving() { }

        virtual public void Saved() { }

        //void importValues(Settings s)!!!Declined because such copying may bring to a mess in the object's state (if any)
        //{
        //    foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        //        settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        //}

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with default values. 
        /// Tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// Calling this method on a detached Settings object makes no effect because otherwise it would lead to a confusing effect. 
        /// </summary>
        public bool Reset(/*bool ignoreInitFile = false*/)
        {
            if (IsDetached())//while technically it is possible, it seems to contradict the idea of Settings if this method was performed outside the Settings field.
                return false;
            __Info.SetObject(Create(__Info, true, true));
            return true;
        }

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with stored values.
        /// Tries to load it from the storage file.
        /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// Calling this method on a detached Settings object makes no effect because otherwise it would lead to a confusing effect. 
        /// </summary>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        public bool Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            if (IsDetached())//while technically it is possible, it seems to contradict the idea of Settings if this method was performed outside the Settings field.
                return false;
            __Info.SetObject(Create(__Info, false, throwExceptionIfCouldNotLoadFromStorageFile));
            return true;
        }

        /// <summary>
        /// Compares serializable properties of this object with the ones stored in the file or the default ones.
        /// </summary>
        /// <returns>False if the values are identical.</returns>
        public bool IsChanged()
        {
            lock (this)
            {
                if (__Info == null)//was created outside Config
                    throw new Exception("This method cannot be performed on a Settings object which has __Info not defined.");
                return !Serialization.Json.IsEqual(this, Create(__Info, false, false));
            }
        }

        /// <summary>
        /// Indicates that the attributed Settings field should not be initiated by Config by default.
        /// </summary>
        public class Optional : Attribute { }

        /// <summary>
        /// Folder where storage files for this Settings derived type are to be saved by Config.
        /// Each Settings derived class must have it defined. 
        /// Despite of the fact it is not static, it is instance independent actually as only default value of it is used.
        /// (It is not static due to badly awkwardness of C#.)
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public abstract string __StorageDir { get; }
    }

    /// <summary>
    /// Instances of this class are to be stored in CommonApplicationData folder.
    /// CliverWinRoutines lib contains AppSettings adapted for Windows.
    /// </summary>
    public class AppSettings : Settings
    {
        sealed public override string __StorageDir { get { return StorageDir; } }
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    /// <summary>
    /// Instances of this class are to be stored in LocalApplicationData folder.
    /// </summary>
    public class UserSettings : Settings
    {
        sealed public override string __StorageDir { get { return StorageDir; } }
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }
}