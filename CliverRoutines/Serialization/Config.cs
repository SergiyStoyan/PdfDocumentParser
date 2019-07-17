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
            return Load<S>(__File);
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
            DefaultStorageDir = UserSettings.StorageDir;
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
        public static readonly string DefaultStorageDir;
        public static string StorageDir { get; private set; }


        static void get(bool reset)
        {
            lock (objectFullNames2serializable)
            {
                objectFullNames2serializable.Clear();
                List<Assembly> assemblies = new List<Assembly>();
                assemblies.Add(Assembly.GetEntryAssembly());
                foreach (AssemblyName assemblyNames in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assemblyNames => assemblyNameRegexPattern != null ? Regex.IsMatch(assemblyNames.Name, assemblyNameRegexPattern) : true))
                    assemblies.Add(Assembly.Load(assemblyNames));
                List<FieldInfo> settingsTypeFieldInfos = new List<FieldInfo>();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type settingsType in types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))))
                    {
                        foreach (Type type in types)
                        {
                            List<FieldInfo> fis = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType.IsAssignableFrom(settingsType)).ToList();
                            if (fis.Count > 1)
                                throw new Exception("There are more than one fields of Settings type " + settingsType.FullName + ":\r\n" + string.Join(",\r\n", fis.Select(a => a.DeclaringType.FullName + "." + a.Name).ToList()));
                            settingsTypeFieldInfos.AddRange(fis);
                        }
                    }
                }
                foreach (FieldInfo settingsTypeFieldInfo in settingsTypeFieldInfos)
                {
                    string fullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;

                    if (null == settingsTypeFieldInfo.GetCustomAttributes<Settings.Obligatory>(false).FirstOrDefault() && (obligatoryObjectNamesRegex == null || !obligatoryObjectNamesRegex.IsMatch(fullName)))
                        continue;

                    Serializable serializable;

                    string fileName = fullName + "." + FILE_EXTENSION;
                    string file = (settingsTypeFieldInfo.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (settingsTypeFieldInfo.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : StorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
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
                            string file = (settingsTypeFieldInfo.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (settingsTypeFieldInfo.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : StorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
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

                            settingsTypeFieldInfo.SetValue(null, serializable);
                            return;
                        }
                    }
                }
            }
            throw new Exception("Field '" + fullName + "' was not found.");
        }

        static public void Reload(string storageDir = null, bool readOnly = false)
        {
            StorageDir = storageDir != null ? storageDir : DefaultStorageDir;
            ReadOnly = readOnly;
            get(false);
        }

        static public bool ReadOnly { get; private set; }

        static public void Reset(string storageDir = null)
        {
            StorageDir = storageDir != null ? storageDir : DefaultStorageDir;
            get(true);
        }

        static public void Save(string storageDir = null)
        {
            storageDir = storageDir != null ? storageDir : DefaultStorageDir;
            if (ReadOnly && PathRoutines.ArePathsEqual(storageDir, StorageDir))
                throw new Exception("Config is read-only and cannot be saved to the same location.");
            StorageDir = storageDir;
            lock (objectFullNames2serializable)
            {
                foreach (Serializable s in objectFullNames2serializable.Values)
                    s.Save(StorageDir + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
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
                    if (File.Exists(s.__File))//it can be absent if default settings used still
                        File.Copy(s.__File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
            }
        }
    }
}