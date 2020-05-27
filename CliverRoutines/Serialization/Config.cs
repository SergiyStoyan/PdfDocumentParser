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
    /// Alternative to .NET settings. Inheritors of this class are automatically managed by Config.
    /// </summary>
    abstract public class Settings : Serializable
    {
        public void Reset()
        {
            Serializable s = Create(GetType(), __File);
            foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        }

        public void Reload()
        {
            Serializable s = LoadOrCreate(GetType(), __File);
            foreach (FieldInfo settingsTypeFieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                settingsTypeFieldInfo.SetValue(this, settingsTypeFieldInfo.GetValue(s));
        }

        public S GetResetInstance<S>() where S : Settings, new()
        {
            return Create<S>(__File);
        }

        public S GetReloadedInstance<S>() where S : Settings, new()
        {
            return LoadOrCreate<S>(__File);
        }

        public bool IsChanged()
        {
            return !Serialization.Json.IsEqual(LoadOrCreate(GetType(), __File), this);
        }

        /// <summary>
        /// this object is ever to be loaded
        /// </summary>
        public class Obligatory : Attribute
        { }

        /// <summary>
        /// this object is located in user domain
        /// </summary>
        //public class UserDependent : Attribute
        //{ }

        /// <summary>
        /// if a custom Settings class is not direct descendant then it must to specify its Settings type explicitly
        /// </summary>
        //public class SeettingsType : Attribute
        //{
        //    public Type SettingsType;
        //}
    }

    abstract public class AppSettings : Settings
    {
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    abstract public class UserSettings : Settings
    {
        public static readonly string StorageDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + Log.CompanyName + System.IO.Path.DirectorySeparatorChar + Log.ProcessName + System.IO.Path.DirectorySeparatorChar + Config.CONFIG_FOLDER_NAME;
    }

    /// <summary>
    /// Manages Serializable settings.
    /// </summary>
    public class Config
    {
        static Config()
        {
            UnknownTypeStorageDir = UserSettings.StorageDir;
        }

        /// <summary>
        /// It allows to load only certain settings objects, while ignoring unneeded ones.
        /// However, objects attributed with [Settings.Obligatory] will be loaded in any way.
        /// </summary>
        /// <param name="assemblyNameRegexPattern"></param>
        /// <param name="obligatoryObjectNamesRegexPattern"></param>
        public static void Initialize(string assemblyNameRegexPattern = @"^Cliver", string obligatoryObjectNamesRegexPattern = null)
        {
            Config.assemblyNameRegexPattern = assemblyNameRegexPattern;
            obligatoryObjectNamesRegex = obligatoryObjectNamesRegexPattern == null ? null : new Regex(obligatoryObjectNamesRegexPattern);
        }
        static Regex obligatoryObjectNamesRegex = null;
        static string assemblyNameRegexPattern = null;

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";
        public static string UnknownTypeStorageDir { get; private set; }


        static void get(bool reset)
        {
            lock (objectFullNames2serializable)
            {
                objectFullNames2serializable.Clear();
                List<Assembly> assemblies = new List<Assembly>();
                assemblies.Add(Assembly.GetEntryAssembly());
                foreach (AssemblyName assemblyNames in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assemblyNames => assemblyNameRegexPattern != null ? Regex.IsMatch(assemblyNames.Name, assemblyNameRegexPattern) : true))
                    assemblies.Add(Assembly.Load(assemblyNames));
                HashSet<FieldInfo> settingsTypeFieldInfos = new HashSet<FieldInfo>();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type settingsType in types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))))
                    {
                        foreach (Type type in types)
                            foreach (FieldInfo settingsTypeFieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType.IsAssignableFrom(settingsType)))
                                settingsTypeFieldInfos.Add(settingsTypeFieldInfo);
                    }
                }
                foreach (FieldInfo settingsTypeFieldInfo in settingsTypeFieldInfos)
                {
                    string fullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;

                    if (null == settingsTypeFieldInfo.GetCustomAttributes<Settings.Obligatory>(false).FirstOrDefault() && (obligatoryObjectNamesRegex == null || !obligatoryObjectNamesRegex.IsMatch(fullName)))
                        continue;

                    Serializable serializable;

                    string fileName = fullName + "." + FILE_EXTENSION;
                    string file = (settingsTypeFieldInfo.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (settingsTypeFieldInfo.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : UnknownTypeStorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
                    if (reset)
                    {
                        string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + fileName;
                        if (File.Exists(initFile))
                        {
                            FileSystemRoutines.CopyFile(initFile, file, true);
                            serializable = Serializable.LoadOrCreate(settingsTypeFieldInfo.FieldType, file);
                        }
                        else
                            serializable = Serializable.Create(settingsTypeFieldInfo.FieldType, file);
                    }
                    else
                    {
                        try
                        {
                            serializable = Serializable.Load(settingsTypeFieldInfo.FieldType, file);
                        }
                        catch (Exception e)
                        {
                            //if (!Message.YesNo("Error while loading config file " + file + "\r\n\r\n" + e.Message + "\r\n\r\nWould you like to proceed with restoring the initial config?", null, Message.Icons.Error))
                            //    Environment.Exit(0);
                            //if (!ignore_load_error && !Directory.Exists(StorageDir))//it is newly installed and so files are not expected to be there
                            //    ignore_load_error = true;
                            //if (!ignore_load_error)
                            //    LogMessage.Error2(e);
                            string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + fileName;
                            if (File.Exists(initFile))
                            {
                                FileSystemRoutines.CopyFile(initFile, file, true);
                                serializable = Serializable.LoadOrCreate(settingsTypeFieldInfo.FieldType, file);
                            }
                            else
                                serializable = Serializable.Create(settingsTypeFieldInfo.FieldType, file);
                        }
                    }

                    settingsTypeFieldInfo.SetValue(null, serializable);
                    objectFullNames2serializable[fullName] = serializable;
                }
            }
        }
        static Dictionary<string, Serializable> objectFullNames2serializable = new Dictionary<string, Serializable>();

        /// <summary>
        /// Can be called from code when ordered load is required due to dependencies.
        /// </summary>
        static public void ReloadField(string fullName)
        {
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetEntryAssembly());
            foreach (AssemblyName assemblyNames in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assemblyNames => assemblyNameRegexPattern != null ? Regex.IsMatch(assemblyNames.Name, assemblyNameRegexPattern) : true))
                assemblies.Add(Assembly.Load(assemblyNames));
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type settingsType in types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))))
                {
                    foreach (Type type in types)
                    {
                        FieldInfo settingsTypeFieldInfo = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType == settingsType && (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
                        if (settingsTypeFieldInfo != null)
                        {
                            Serializable serializable;
                            string fileName = fullName + "." + FILE_EXTENSION;
                            string file = (settingsTypeFieldInfo.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (settingsTypeFieldInfo.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : UnknownTypeStorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
                            try
                            {
                                serializable = Serializable.Load(settingsTypeFieldInfo.FieldType, file);
                            }
                            catch //(Exception e)
                            {
                                //if (!Message.YesNo("Error while loading config file " + file + "\r\n\r\n" + e.Message + "\r\n\r\nWould you like to proceed with restoring the initial config?", null, Message.Icons.Error))
                                //    Environment.Exit(0);
                                //if (!ignore_load_error && !Directory.Exists(StorageDir))//it is newly installed and so files are not expected to be there
                                //    ignore_load_error = true;
                                //if (!ignore_load_error)
                                //    LogMessage.Error2(e);
                                string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + fileName;
                                if (File.Exists(initFile))
                                {
                                    FileSystemRoutines.CopyFile(initFile, file, true);
                                    serializable = Serializable.LoadOrCreate(settingsTypeFieldInfo.FieldType, file);
                                }
                                else
                                    serializable = Serializable.Create(settingsTypeFieldInfo.FieldType, file);
                            }

                            settingsTypeFieldInfo.SetValue(null, serializable);
                            return;
                        }
                    }
                }
            }
            throw new Exception("Field '" + fullName + "' was not found.");
        }

        static public void Reload(string unknownTypeStorageDir = null, bool readOnly = false)
        {
            if (unknownTypeStorageDir != null)
                UnknownTypeStorageDir = unknownTypeStorageDir;
            ReadOnly = readOnly;
            get(false);
        }

        static public bool ReadOnly { get; private set; }

        static public void Reset(string unknownTypeStorageDir = null)
        {
            if (unknownTypeStorageDir != null)
                UnknownTypeStorageDir = unknownTypeStorageDir;
            get(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unknownTypeStorageDir">only unknown Serializable types will be saved to a new location</param>
        //static public void Save(string unknownTypeStorageDir = null)
        //{
        //    if (unknownTypeStorageDir != null)
        //        if (ReadOnly && PathRoutines.ArePathsEqual(unknownTypeStorageDir, UnknownTypeStorageDir))
        //            throw new Exception("Config is read-only and cannot be saved to the same location: " + unknownTypeStorageDir);
        //    UnknownTypeStorageDir = unknownTypeStorageDir;
        //    lock (objectFullNames2serializable)
        //    {
        //        foreach (Serializable s in objectFullNames2serializable.Values)
        //        {
        //            if (s is AppSettings)
        //                s.Save(AppSettings.StorageDir + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
        //            else if (s is UserSettings)
        //                s.Save(UserSettings.StorageDir + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
        //            else
        //                s.Save(UnknownTypeStorageDir + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
        //        }
        //    }
        //}
        static public void Save()
        {
            lock (objectFullNames2serializable)
            {
                foreach (Serializable s in objectFullNames2serializable.Values)
                    s.Save();
            }
        }

        static public Serializable GetInstance(string fullName)
        {
            lock (objectFullNames2serializable)
            {
                Serializable s = null;
                objectFullNames2serializable.TryGetValue(fullName, out s);
                return s;
            }
        }

        static public void CopyFiles(string toDirectory)
        {
            lock (objectFullNames2serializable)
            {
                string d = FileSystemRoutines.CreateDirectory(toDirectory + System.IO.Path.DirectorySeparatorChar + CONFIG_FOLDER_NAME);
                foreach (Serializable s in objectFullNames2serializable.Values)
                    if (File.Exists(s.__File))//it can be absent if default settings are used still
                        File.Copy(s.__File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
            }
        }
    }
}