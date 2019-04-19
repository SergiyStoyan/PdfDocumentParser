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
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                fi.SetValue(this, fi.GetValue(s));
        }

        public void Reload()
        {
            Serializable s = LoadOrCreate(GetType(), __File);
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                fi.SetValue(this, fi.GetValue(s));
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
                List<Assembly> sas = new List<Assembly>();
                sas.Add(Assembly.GetEntryAssembly());
                foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => assemblyNameRegexPattern != null ? Regex.IsMatch(an.Name, assemblyNameRegexPattern) : true))
                    sas.Add(Assembly.Load(an));
                //bool ignore_load_error = false;
                foreach (Assembly sa in sas)
                {
                    Type[] ets = sa.GetTypes();
                    foreach (Type st in ets.Where(t => t.IsSubclassOf(typeof(Settings))))
                    {
                        List<FieldInfo> fis = new List<FieldInfo>();
                        foreach (Type et in ets)
                            fis.AddRange(et.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => st.IsAssignableFrom(a.FieldType)));

                        if (fis.Count < 1)
                            //    throw new Exception("No field of type '" + st.FullName + "' was found.");
                            continue;
                        //if (fis.Count > 1)
                        //    throw new Exception("More then 1 field of type '" + st.FullName + "' was found.");
                        foreach (FieldInfo fi in fis)
                        {
                            string fullName = fi.DeclaringType.FullName + "." + fi.Name;

                            if (null == fi.GetCustomAttributes<Settings.Obligatory>(false).FirstOrDefault() && (obligatoryObjectNamesRegex == null || !obligatoryObjectNamesRegex.IsMatch(fullName)))
                                continue;

                            Serializable t;

                            string fileName = fullName + "." + FILE_EXTENSION;
                            string file = (fi.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (fi.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : StorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
                            if (reset)
                            {
                                string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + fileName;
                                if (File.Exists(initFile))
                                {
                                    FileSystemRoutines.CopyFile(initFile, file, true);
                                    t = Serializable.LoadOrCreate(fi.FieldType, file);
                                }
                                else
                                    t = Serializable.Create(fi.FieldType, file);
                            }
                            else
                            {
                                try
                                {
                                    t = Serializable.Load(fi.FieldType, file);
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
                                        t = Serializable.LoadOrCreate(fi.FieldType, file);
                                    }
                                    else
                                        t = Serializable.Create(fi.FieldType, file);
                                }
                            }

                            fi.SetValue(null, t);
                            objectFullNames2serializable[fullName] = t;
                        }
                    }
                }
            }
        }
        static Dictionary<string, Serializable> objectFullNames2serializable = new Dictionary<string, Serializable>();

        /// <summary>
        /// Can be called from code when ordered load is required due to dependencies.
        /// </summary>
        static public void ReloadField(string fullName)
        {
            List<Assembly> sas = new List<Assembly>();
            sas.Add(Assembly.GetEntryAssembly());
            foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => assemblyNameRegexPattern != null ? Regex.IsMatch(an.Name, assemblyNameRegexPattern) : true))
                sas.Add(Assembly.Load(an));
            foreach (Assembly sa in sas)
            {
                Type[] ets = sa.GetTypes();
                foreach (Type st in ets.Where(t => t.IsSubclassOf(typeof(Settings))))
                {
                    foreach (Type et in ets)
                    {
                        FieldInfo fi = et.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType == st && (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
                        if (fi != null)
                        {
                            Serializable t;
                            string fileName = fullName + "." + FILE_EXTENSION;
                            string file = (fi.FieldType.BaseType == typeof(UserSettings) ? UserSettings.StorageDir : (fi.FieldType.BaseType == typeof(AppSettings) ? AppSettings.StorageDir : StorageDir)) + System.IO.Path.DirectorySeparatorChar + fileName;
                            try
                            {
                                t = Serializable.Load(fi.FieldType, file);
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
                                    t = Serializable.LoadOrCreate(fi.FieldType, file);
                                }
                                else
                                    t = Serializable.Create(fi.FieldType, file);
                            }

                            fi.SetValue(null, t);
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