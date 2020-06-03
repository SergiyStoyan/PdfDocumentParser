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
    /// Config manages serializable objects pertaining to Settings type fields.
    /// It provides linking between each Settings type object and fields of this type in the application. 
    /// Every Settings type field in the application has it own storage file whcich is defined by the Settings type and full name of the field.
    /// Usually it's that only one field is expected to be declared per Settings type, but generally there can be any number of fields of the same Settings type.
    /// </summary>
    public partial class Config
    {
        /// <summary>
        /// Tells Config which optional (i.e. attributed with [Settings.Optional]) Settings type fields are to be initialized. 
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static Regex RequiredOptionalFieldFullNamesRegex = null;

        /// <summary>
        /// Tells Config in which order Settings types are to be inialized.        
        /// It may be necessary due to dependencies between Settings types.
        /// Types listed here will be initialized first in the provided order.
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static List<Type> InitialzingOrderedSettingsTypes = null;

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";

        static void loadOrReset(bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            lock (fieldFullNames2settingsObject)
            {
                fieldFullNames2settingsObject.Clear();
                foreach (FieldInfo settingsTypeFieldInfo in enumSettingsTypeFieldInfos())
                {
                    fieldInfos.Add(settingsTypeFieldInfo);
                    string fullName = settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name;

                    if (null != settingsTypeFieldInfo.GetCustomAttributes<Settings.Optional>(false).FirstOrDefault() && (RequiredOptionalFieldFullNamesRegex == null || !RequiredOptionalFieldFullNamesRegex.IsMatch(fullName)))
                        continue;

                    Serializable serializable = getSerializable(settingsTypeFieldInfo.FieldType, fullName, reset, throwExceptionIfCouldNotLoadFromStorageFile);

                    settingsTypeFieldInfo.SetValue(null, serializable);
                    fieldFullNames2settingsObject[fullName] = (Settings)serializable;
                }
            }
        }
        static Dictionary<string, Settings> fieldFullNames2settingsObject = new Dictionary<string, Settings>();

        class SettingsTypeComparer : IComparer<Type>
        {
            public SettingsTypeComparer(List<Type> orderedTypes)
            {
                this.orderedTypes = orderedTypes;
            }
            readonly List<Type> orderedTypes;
            public int Compare(Type a, Type b)
            {
                int ai = orderedTypes.IndexOf(a);
                int bi = orderedTypes.IndexOf(b);
                if (ai < 0)
                    if (bi < 0)
                        return 0;
                    else
                        return 1;
                if (bi < 0)
                    return -1;
                if (a == b)
                    return 0;
                return ai < bi ? -1 : 1;
            }
        }
        static IEnumerable<FieldInfo> enumSettingsTypeFieldInfos()
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
                IEnumerable<Type> settingsTypes = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))).Distinct();
                if (InitialzingOrderedSettingsTypes != null)
                {
                    SettingsTypeComparer settingsTypeComparer = new SettingsTypeComparer(InitialzingOrderedSettingsTypes);
                    settingsTypes = settingsTypes.OrderBy(t => t, settingsTypeComparer);
                }
                foreach (Type type in types)
                    foreach (FieldInfo settingsTypeFieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(f => settingsTypes.Contains(f.FieldType) /* && f.FieldType.IsAssignableFrom(settingsType)*/))
                    {//usually it should be only 1 FieldInfo per Settings type
                        if (!settingsTypeFieldInfo.IsInitOnly)
                            throw new Exception("Settings type field " + settingsTypeFieldInfo.DeclaringType.FullName + "." + settingsTypeFieldInfo.Name + " must be readonly to ensure the proper work of Cliver.Config");
                        yield return settingsTypeFieldInfo;
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
        /// Reloads all the Settings type fields. 
        /// It's the usual method to be called in the beginning of an application to initiate Config.
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
    }
}