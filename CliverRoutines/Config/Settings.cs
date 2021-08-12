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
        /// Cloned Settings objects share the same __Info instance which means that while multiple Settings objects can reference the same Settings field, the field can reference only one of them (which is called as 'attached object').
        /// For some rare needs (for instance when a Settings object was created by deserialization/cloning and therefore has empty __Info), setting __Info from an application's code is allowed (with caution!).
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
                //if (value == null)
                //    throw new Exception("SettingsFieldInfo cannot be set to NULL.");//to ensure that no __Info object can be lost in the custom application scope !!!it can be lost anyway
                if (value != null)
                {
                    if (value.Type != GetType())
                        throw new Exception("Disaccording SettingsFieldInfo Type field. It must be '" + GetType().FullName + "' while trying '" + value.Type.FullName + "'");
                    if (Config.GetSettingsFieldInfo(value.FullName) != value)
                        throw new Exception("This SettingsFieldInfo object is not registered in Config. Probably it was created before the re-initialization.");
                }
                settingsFieldInfo = value;
            }
        }
        SettingsFieldInfo settingsFieldInfo = null;

        internal static Settings Create(SettingsFieldInfo settingsFieldInfo, bool reset)
        {
            Settings settings = create(settingsFieldInfo, reset);
            settings.__Info = settingsFieldInfo;
            settings.Loaded();
            return settings;
        }
        static Settings create(SettingsFieldInfo settingsFieldInfo, bool reset)
        {
            if (!reset && File.Exists(settingsFieldInfo.File))
                return loadFromFile(settingsFieldInfo);
            if (File.Exists(settingsFieldInfo.InitFile))
            {
                FileSystemRoutines.CopyFile(settingsFieldInfo.InitFile, settingsFieldInfo.File, true);
                return loadFromFile(settingsFieldInfo);
            }
            Settings settings = (Settings)Activator.CreateInstance(settingsFieldInfo.Type);
            settings.__TypeVersion = settingsFieldInfo.TypeVersion;
            return settings;
        }
        static Settings loadFromFile(SettingsFieldInfo settingsFieldInfo)
        {
            string s = File.ReadAllText(settingsFieldInfo.File);
            if (settingsFieldInfo.Endec != null)
                s = settingsFieldInfo.Endec.Decrypt(s);
            Settings settings;
            Exception exception = null;
            try
            {
                settings = (Settings)Serialization.Json.Deserialize(settingsFieldInfo.Type, s, true, true);
            }
            catch (Exception e)
            {
                settings = (Settings)Activator.CreateInstance(settingsFieldInfo.Type);
                exception = e;
            }
            if (exception != null || settingsFieldInfo.TypeVersion != settings.__TypeVersion)
            {
                settings.__Info = settingsFieldInfo;
                UnsupportedFormatHandlerCommand mode = settings.UnsupportedFormatHandler(exception);
                switch (mode)
                {
                    case UnsupportedFormatHandlerCommand.Reload:
                        settings = create(settingsFieldInfo, false);
                        break;
                    case UnsupportedFormatHandlerCommand.Reset:
                        settings = create(settingsFieldInfo, true);
                        break;
                    case UnsupportedFormatHandlerCommand.Proceed:
                        break;
                    default:
                        throw new Exception("Unknown option: " + mode);
                }
            }
            return settings;
        }

        /// <summary>
        /// Indicates whether this Settings object is the value of the Settings field defined by __Info.
        /// </summary>
        public bool IsAttached()
        {
            return __Info != null
                //&& __Info.GetObject() == this;!!!if Config was reloaded and __Info was recreated, it still would work which is wrong
                && Config.GetSettingsFieldInfo(__Info.FullName)?.GetObject() == this;//is referenced by the field
        }

        /// <summary>
        /// Serializes this Settings object to its storage file.
        /// </summary>
        public void Save()
        {
            lock (this)
            {
                if (__Info == null)//it can be called on a detached instance in order to be used by UnsupportedFormatHandler(). Also, saving a detached object is not confusing.
                    throw new Exception("This method cannot be performed on this Settings object because its __Info is not set.");
                save();
            }
        }
        void save()
        {
            __TypeVersion = __Info.TypeVersion;
            Saving();
            string s = Serialization.Json.Serialize(this, __Info.Indented, true, !__Info.NullSerialized, false/*!!!default values always must be stored*/);
            if (__Info.Endec != null)
                s = __Info.Endec.Encrypt(s);
            FileSystemRoutines.CreateDirectory(PathRoutines.GetFileDir(__Info.File));
            File.WriteAllText(__Info.File, s);
            Saved();
        }
        internal void Save(SettingsFieldInfo correctSettingsFieldInfo)//checks that __Info was not replaced and provides an appropriate exception message
        {
            lock (this)
            {
                if (__Info != correctSettingsFieldInfo)
                    //it can happen when:
                    //- there are several settings fields of the same type and their __Info's were exchanged from the custom code;
                    //- Config was reloaded while an old object was preserved;
                    //All this will lead to a confusion.
                    throw new Exception("Settings field '" + correctSettingsFieldInfo.FullName + "' cannot be saved because its __Info has been altered.");
                save();
            }
        }

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with the default values. 
        /// Tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// (!)Calling this method on a detached Settings object throws an exception because otherwise it could lead to a confusing effect. 
        /// </summary>
        public void Reset(/*bool ignoreInitFile = false*/)
        {
            if (!IsAttached())//while technically it is possible, it could lead to a confusion: called on one object it might replace an other one!
                throw new Exception("This method cannot be performed on this Settings object because it is not attached to its Settings field (" + __Info?.FullName + ").");
            __Info.ResetObject();
        }

        /// <summary>
        /// Replaces the value of the field defined by __Info with a new object initiated with the stored values.
        /// Tries to load it from the storage file.
        /// If this file does not exist, it tries to load it from the initial file located in the app's directory. 
        /// If this file does not exist, it creates an object with the hardcoded values.
        /// (!)Calling this method on a detached Settings object throws an exception because otherwise it could lead to a confusing effect. 
        /// </summary>
        public void Reload()
        {
            if (!IsAttached())//while technically it is possible, it could lead to a confusion: called on one object it might replace an other one!
                throw new Exception("This method cannot be performed on this Settings object because it is not attached to its Settings field (" + __Info?.FullName + ").");
            __Info.ReloadObject();
        }

        /// <summary>
        /// Compares serializable fields/properties of this object with the ones stored in the file or the default ones.
        /// </summary>
        /// <returns>False if the values are identical.</returns>
        public bool IsChanged()
        {
            lock (this)
            {
                if (__Info == null)//was created outside Config
                    throw new Exception("This method cannot be performed on this Settings object because its __Info is not set.");
                Settings settings = create(__Info, false);
                return !Serialization.Json.IsEqual(this, settings);
            }
        }

        #region Type Version support

        /// <summary>
        /// Actual version of the Settings type as it is restored from the storage file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]//it serves 2 aims: - ignore when 0; - forces setting through the private setter (yes, it does!)
        public uint __TypeVersion { get; private set; } = 0;
        //public Version __TypeVersion { get; private set; } = null;

        /// <summary>
        /// Called by Config if either: 
        /// - the storage/init file content does not match the Settings type version;
        /// - the storage/init file could not be deserialized;
        /// Here is your chance to amend the data to migrate to the current version.
        /// </summary>
        virtual protected UnsupportedFormatHandlerCommand UnsupportedFormatHandler(Exception deserializingException)
        {
            if (deserializingException != null)
                throw new Exception("Error while deserializing settings " + settingsFieldInfo.FullName + " from file " + settingsFieldInfo.InitFile, deserializingException);
            throw new Exception("Unsupported type version of " + __Info.FullName + ": " + __TypeVersion + "\r\nin the file: " + __Info.File);
        }
        public enum UnsupportedFormatHandlerCommand
        {
            /// <summary>
            /// Read the storage file if it exists, otherwise reads the initial file if it exists, otherwise creates a default instance.
            /// </summary>
            Reload,
            /// <summary>
            /// Read the initial file if it exists, otherwise creates a default instance.
            /// </summary>
            Reset,
            /// <summary>
            /// The application proceeds with the cussrent instance as is.
            /// </summary>
            Proceed
        }

        #endregion

        virtual protected void Loaded() { }

        virtual protected void Saving() { }

        virtual protected void Saved() { }

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
        /// (It is not static because C# does not support static polymorphism.)
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public abstract string __StorageDir { get; protected set; }
    }
}