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
using System.Diagnostics;

namespace Cliver
{
    /// <summary>
    /// Manages serializable objects pertaining to Settings type fields.
    /// </summary>
    public partial class Config
    {
        static Config()
        {
        }

        /// <summary>
        /// Allows to load only certain settings objects, while ignoring unneeded ones.
        /// However, objects attributed with [Settings.Obligatory] will be loaded in any way.
        /// </summary>
        public static Regex ObligatoryObjectNamesRegex = null;

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";

        static void loadOrReset(bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            lock (fieldFullNames2settingsObject)
            {
                fieldFullNames2settingsObject.Clear();
                foreach (IEnumerable<FieldInfo> settingsTypeFieldInfos in enumSettingsTypesFieldInfos())
                    foreach (FieldInfo settingsTypeFieldInfo in settingsTypeFieldInfos)
                    {
                        fieldInfos.Add(settingsTypeFieldInfo);
                        string fullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;

                        if (null == settingsTypeFieldInfo.GetCustomAttributes<Settings.Obligatory>(false).FirstOrDefault() && (ObligatoryObjectNamesRegex == null || !ObligatoryObjectNamesRegex.IsMatch(fullName)))
                            continue;

                        Serializable serializable = getSerializable(settingsTypeFieldInfo.FieldType, fullName, reset, throwExceptionIfCouldNotLoadFromStorageFile);

                        settingsTypeFieldInfo.SetValue(null, serializable);
                        fieldFullNames2settingsObject[fullName] = (Settings)serializable;
                    }
            }
        }
        static Dictionary<string, Settings> fieldFullNames2settingsObject = new Dictionary<string, Settings>();

        static IEnumerable<IEnumerable<FieldInfo>> enumSettingsTypesFieldInfos()
        {
            string configAssemblyFullName = Assembly.GetExecutingAssembly().FullName;
            StackTrace stackTrace = new StackTrace();
            Assembly callingAssembly = stackTrace.GetFrames().Where(f => f.GetMethod().DeclaringType.Assembly.FullName != configAssemblyFullName).Select(f => f.GetMethod().DeclaringType.Assembly).FirstOrDefault();
            if (callingAssembly == null)
                callingAssembly = Assembly.GetEntryAssembly();
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(callingAssembly);
            foreach (AssemblyName assemblyName in callingAssembly.GetReferencedAssemblies())
            {
                Assembly a = Assembly.Load(assemblyName);
                if (null != a.GetReferencedAssemblies().Where(an => an.FullName == configAssemblyFullName).FirstOrDefault())
                    assemblies.Add(a);
            }
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type settingsType in types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))))
                {
                    foreach (Type type in types)
                        yield return type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(f => f.FieldType == settingsType/* && f.FieldType.IsAssignableFrom(settingsType)*/);
                }
            }
        }

        static Serializable getSerializable(Type settingsType, string fullName, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            string fileName = fullName + "." + FILE_EXTENSION;
            string file = Settings.GetConfigStorageDir(settingsType) + System.IO.Path.DirectorySeparatorChar + fileName;
            string initFile = Log.AppDir + System.IO.Path.DirectorySeparatorChar + fileName;
            if (!reset && File.Exists(file))
                try
                {
                    return Serializable.Load(settingsType, file);
                }
                catch (Exception e)
                {
                    if (throwExceptionIfCouldNotLoadFromStorageFile)
                        throw new Exception("Error while loading settings " + fullName + " from file " + file, e);
                }
            if (File.Exists(initFile))
            {
                FileSystemRoutines.CopyFile(initFile, file, true);
                return Serializable.LoadOrCreate(settingsType, file);
            }
            return Serializable.Create(settingsType, file);
        }

        /// <summary>
        /// Reloads all the Settings type fields. It the usual method to be called in the beginning of an application to initiate Config scope.
        /// First it tries to load each Settings object from its default storage directory. 
        /// If this file does not exist, it tries to load from the initial settings file in app's directory.
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        static public void Reload(bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            loadOrReset(false, throwExceptionIfCouldNotLoadFromStorageFile);
        }

        /// <summary>
        /// Resets all the Settings type fields.
        /// First it tries to load each Settings object from the initial settings file in app's directory. 
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        static public void Reset()
        {
            loadOrReset(true, true);
        }

        /// <summary>
        /// Serializes all the Settings type fields to their files.
        /// </summary>
        static public void Save()
        {
            lock (fieldFullNames2settingsObject)
            {
                foreach (Serializable s in fieldFullNames2settingsObject.Values)
                    s.Save();
            }
        }

        static public void ExportSettingsFiles(string toDirectory)
        {
            lock (fieldFullNames2settingsObject)
            {
                string d = FileSystemRoutines.CreateDirectory(toDirectory + System.IO.Path.DirectorySeparatorChar + CONFIG_FOLDER_NAME);
                foreach (Serializable s in fieldFullNames2settingsObject.Values)
                    if (File.Exists(s.__File))//it can be absent if default settings are used still
                        File.Copy(s.__File, d + System.IO.Path.DirectorySeparatorChar + PathRoutines.GetFileName(s.__File));
            }
        }

        #region Routines for individual Settings objects in the config scope.

        /// <summary>
        /// Returns the Settings object identified by the full name of the field to which the object belongs.
        /// The object is not newly created but must already exist in the config scope.
        /// </summary>
        /// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        /// <returns>The Settings object which was previously created by Config</returns>
        static public Settings GetSettings(string fullName)
        {
            lock (fieldFullNames2settingsObject)
            {
                Settings s = null;
                fieldFullNames2settingsObject.TryGetValue(fullName, out s);
                return s;
            }
        }

        /// <summary>
        /// Can be called from code when ordered load is required due to dependencies.
        /// </summary>
        /// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        /// <param name="throwExceptionIfCouldNotLoadFromStorageFile"></param>
        static public void ReloadField(string fullName, bool throwExceptionIfCouldNotLoadFromStorageFile = false)
        {
            lock (fieldFullNames2settingsObject)
            {
                foreach (IEnumerable<FieldInfo> settingsTypeFieldInfos in enumSettingsTypesFieldInfos())
                {
                    FieldInfo settingsTypeFieldInfo = settingsTypeFieldInfos.Where(a => (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
                    if (settingsTypeFieldInfo != null)
                    {
                        Serializable serializable = getSerializable(settingsTypeFieldInfo.FieldType, fullName, false, throwExceptionIfCouldNotLoadFromStorageFile);
                        settingsTypeFieldInfo.SetValue(null, serializable);
                        return;
                    }
                }
                throw new Exception("Field '" + fullName + "' was not found.");
            }
        }

        ///// <summary>
        ///// Returns the file path of the Settings object before the Settings object has been created. 
        ///// It allows to delete/replace the file before loading config which might be need when the object's definition was changed and smooth transition to the new format is required.
        ///// </summary>
        ///// <param name="fullName">Settings field's full name which is the name of its file without extention</param>
        ///// <returns>Settings object's file path</returns>
        //public static string GetFieldFile(string fullName)
        //{
        //lock (fieldFullNames2settingsObject)
        //{
        //    HashSet<Assembly> assemblies = new HashSet<Assembly>();
        //    assemblies.Add(Assembly.GetEntryAssembly());
        //    foreach (AssemblyName assemblyNames in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(assemblyNames => assemblyNameRegexPattern != null ? Regex.IsMatch(assemblyNames.Name, assemblyNameRegexPattern) : true))
        //        assemblies.Add(Assembly.Load(assemblyNames));
        //    foreach (Assembly assembly in assemblies)
        //    {
        //        Type[] types = assembly.GetTypes();
        //        foreach (Type settingsType in types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))))
        //        {
        //            foreach (Type type in types)
        //            {
        //                FieldInfo settingsTypeFieldInfo = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType == settingsType && (a.DeclaringType.FullName + "." + a.Name) == fullName).FirstOrDefault();
        //                if (settingsTypeFieldInfo == null)
        //                    continue;
        //                return Settings.GetDefaultStorageDir(settingsTypeFieldInfo.FieldType) + System.IO.Path.DirectorySeparatorChar + fullName + "." + FILE_EXTENSION;
        //            }
        //        }
        //    }
        //    throw new Exception("Field '" + fullName + "' was not found.");
        //}
    //}
        #endregion
    }
}