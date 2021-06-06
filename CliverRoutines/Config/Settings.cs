//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

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
        /// All __Info instances are paired one-to-one with all Settings fields in the application. 
        /// Cloned Settings objects share the same __Info instance which means that while multiple Settings objects can reference the same Settings field, the field references only one of them (attached object).
        /// For some rare needs (for instance when a Settings object was created by deserialization/cloning and so has empty __Info), setting __Info from an application is allowed (with caution!).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public SettingsMemberInfo __Info
        {
            get
            {
                return settingsFieldInfo;
            }
            set
            {
                if (value == null)
                    throw new Exception("SettingsMemberInfo cannot be set to NULL.");//to ensure that no __Info object can be lost in the custom application scope
                if (value.Type != GetType())
                    throw new Exception("Disaccording SettingsMemberInfo Type field. It must be: " + GetType().FullName + " but set: " + value.Type.FullName);
                settingsFieldInfo = value;
            }
        }
        SettingsMemberInfo settingsFieldInfo = null;

        internal static Settings Create(SettingsMemberInfo settingsFieldInfo, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            Settings settings = create(settingsFieldInfo, reset, throwExceptionIfCouldNotLoadFromStorageFile);
            settings.__Info = settingsFieldInfo;
            settings.Loaded();
            return settings;
        }
        static Settings create(SettingsMemberInfo settingsFieldInfo, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            if (!reset && File.Exists(settingsFieldInfo.File))
                try
                {
                    return (Settings)Cliver.Serialization.Json.Load(settingsFieldInfo.Type, settingsFieldInfo.File, true, true);
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
                    return (Settings)Cliver.Serialization.Json.Load(settingsFieldInfo.Type, settingsFieldInfo.InitFile, true, true);
                }
                catch (Exception e)
                {
                    throw new Exception("Error while loading settings " + settingsFieldInfo.FullName + " from initial file " + settingsFieldInfo.InitFile, e);
                }
            }
            return (Settings)Activator.CreateInstance(settingsFieldInfo.Type);
        }

        /// <summary>
        /// Indicates whether this Settings object is value of the Settings field defined in __Info.
        /// </summary>
        public bool IsAttached()
        {
            return __Info != null
                && Config.GetSettingsFieldInfo(__Info.FullName).GetObject() == this;//is referenced by the field
                                                                                    //&& __Info.GetObject() == this;//is referenced by the field//!!!if Config was reloaded and __Info was recreated, it still would work which is wrong
        }

        /// <summary>
        /// Serializes this Settings object to the storage file.
        /// (!)Calling this method on a detached Settings object throws an exception because otherwise it would lead to a confusing effect. 
        /// </summary>
        public void Save()
        {
            lock (this)
            {
                if (!IsAttached())//while technically it is possible, it can lead to a confusion.
                    throw new Exception("This method cannot be performed on this Settings object because it is not attached to its Settings field (" + __Info?.Type + ")");
                save();
            }
        }
        void save()
        {
            Saving();
            Cliver.Serialization.Json.Save(__Info.File, this, __Info.Indented, true);
            Saved();
        }
        internal void Save(SettingsMemberInfo settingsFieldInfo)//avoids a redundant check and provides an appropriate exception message
        {
            lock (this)
            {
                if (__Info != settingsFieldInfo)//which can only happen if there are several settings fields of the same type
                    throw new Exception("The value of Settings field '" + settingsFieldInfo.FullName + "' is not attached to it.");
                save();
            }
        }

        //for custom decryption
        //virtual protected void Deserializing(ref string json) { }  TBD

        virtual protected void Loaded() { }

        virtual protected void Saving() { }

        //for custom encryption
        //virtual protected void Serialized(ref string json) { }  TBD

        virtual protected void Saved() { }

        //void importValues(Settings s)!!!Declined because such copying may bring to a mess in the object's state (if any)
        //{
        //    foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        //        settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        //}

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with the default values. 
        /// Tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// (!)Calling this method on a detached Settings object throws an exception because otherwise it would lead to a confusing effect. 
        /// </summary>
        public void Reset(/*bool ignoreInitFile = false*/)
        {
            if (!IsAttached())//while technically it is possible, it is a way of confusion: called on one object it would replace another one!
                throw new Exception("This method cannot be performed on this Settings object because it is not attached to its Settings field (" + __Info?.Type + ")");
            __Info.SetObject(Create(__Info, true, true));
        }

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with the stored values.
        /// Tries to load it from the storage file.
        /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// (!)Calling this method on a detached Settings object throws an exception because otherwise it would lead to a confusing effect. 
        /// </summary>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            if (!IsAttached())//while technically it is possible, it is a way of confusion: called on one object it would replace another one!
                throw new Exception("This method cannot be performed on this Settings object because it is not attached to its Settings field (" + __Info?.Type + ")");
            __Info.SetObject(Create(__Info, false, throwExceptionIfCouldNotLoadFromStorageFile));
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
                    throw new Exception("This method cannot be performed on this Settings object because its __Info is not defined.");
                return !Serialization.Json.IsEqual(this, Create(__Info, false, false));
            }
        }

        /*//version with static __StorageDir
        /// <summary>
        /// (!)A Settings derivative or some of its ancestors must hide this public static getter with a new definition to change the storage directory.
        /// It specifies the storage folder for the type which defines this property. 
        /// </summary>
        public static string __StorageDir { get { return Log.AppCompanyUserDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME; } }
        */

        /// <summary>
        /// Storage folder for this Settings derivative.
        /// Each Settings derived class must have it defined. 
        /// Despite of the fact it is not static, actually it is instance independent as only the initial value is used.
        /// (It is not static as C# has no static polymorphism.)
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public abstract string __StorageDir { get; protected set; }
    }

    /// <summary>
    /// Settings field attribute.
    /// </summary>
    public class SettingsAttribute : Attribute
    {
        /// <summary>
        /// Indicates that the Settings field will be stored with indention.
        /// /// </summary>
        readonly public bool Indented;
        /// <summary>
        /// Indicates that the Settings field should not be initiated by Config by default.
        /// Such a field should be initiated explicitly when needed by Config.Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        /// </summary>
        readonly public bool Optional;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indented">Indicates that the Settings field be stored with indention</param>
        /// <param name="optional">Indicates that the Settings field should not be initiated by Config by default.
        /// Such a field should be initiated explicitly when needed by Config.Reload(string settingsFieldFullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)</param>
        public SettingsAttribute(bool indented = true, bool optional = false)
        {
            Indented = indented;
            Optional = optional;
        }
    }

    /// <summary>
    /// Instances of this class are to be stored in CommonApplicationData folder.
    /// CliverWinRoutines lib contains AppSettings adapted for Windows.
    /// </summary>
    public class AppSettings : Settings
    {
        /*//version with static __StorageDir
        /// <summary>
        /// (!)A Settings derivative or some of its ancestors must define this public static getter to specify the storage directory.
        /// </summary>
        new public static string __StorageDir { get; private set; } = Log.AppCompanyCommonDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME; 
        */

        sealed public override string __StorageDir { get; protected set; } = StorageDir;
        public static readonly string StorageDir = Log.AppCompanyCommonDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    /// <summary>
    /// Instances of this class are to be stored in LocalApplicationData folder.
    /// </summary>
    public class UserSettings : Settings
    {
        /*//version with static __StorageDir
        /// <summary>
        /// (!)A Settings derivative or some of its ancestors must define this public static getter to specify the storage directory.
        /// </summary>
        new public static string __StorageDir { get; private set; } = Log.AppCompanyUserDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
        */

        sealed public override string __StorageDir { get; protected set; } = StorageDir;
        public static readonly string StorageDir = Log.AppCompanyUserDataDir + Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }
}