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
    /// </summary>
    abstract public class Settings : Serializable
    {
        //public bool ReadOnly { get; private set; }

        Serializable getSerializable(bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            Type settingsType = GetType();
            string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(__File);
            if (!reset && File.Exists(__File))
                try
                {
                    return Serializable.Load(settingsType, __File);
                }
                catch (Exception e)
                {
                    if (throwExceptionIfCouldNotLoadFromStorageFile)
                        throw new Exception("Error while loading settings from file " + __File, e);
                }
            if (File.Exists(initFile))
            {
                FileSystemRoutines.CopyFile(initFile, __File, true);
                return Serializable.LoadOrCreate(settingsType, __File);
            }
            return Serializable.Create(settingsType, __File);
        }

        void importValues(Serializable s)
        {
            foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        }

        /// <summary>
        /// Reset this object to its hardcoded values.
        /// </summary>
        public void Reset()
        {
            importValues(getSerializable(true, true));
        }

        /// <summary>
        /// First tries to reset this object to the values stored in its storage file.
        /// Only if the file does not exist, it resets this object to its hardcoded values.
        /// </summary>
        public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            importValues(getSerializable(false, throwExceptionIfCouldNotLoadFromStorageFile));
        }

        /// <summary>
        /// Creates a new instance of this Settings type with hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public S CreateResetInstance<S>() where S : Settings, new()
        {
            return (S)getSerializable(true, true);
        }

        /// <summary>
        /// First tries to load a new instance of this Settings type from its storage file.
        /// Only if the file does not exist, it creates a new instance with hardcoded values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public S CreateReloadedInstance<S>(bool throwExceptionIfCouldNotLoadFromStorageFile) where S : Settings, new()
        {
            return (S)getSerializable(false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Compares actual values of this object with the values in the storage file.
        /// </summary>
        /// <returns>False if the values are the identical.</returns>
        public bool IsChanged()
        {
            return !Serialization.Json.IsEqual(LoadOrCreate(GetType(), __File), this);
        }

        /// <summary>
        /// Indicates that this object is always initiated by Config.
        /// </summary>
        public class Obligatory : Attribute
        { }

        /// <summary>
        /// Folder where storage files of this Settings type are stored by Config engine.
        /// Each custom Settings type has to define this path.
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

        /// <summary>
        /// Folder where storage files of this Settings type are stored by Config engine.
        /// </summary>
        public static string GetConfigStorageDir<S>() where S : Settings, new()
        {
            S s = Activator.CreateInstance<S>();
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