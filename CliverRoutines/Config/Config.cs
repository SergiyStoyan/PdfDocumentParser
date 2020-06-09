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
using System.Diagnostics;

namespace Cliver
{
    /// <summary>
    /// Config manages serializable Settings objects set to static fields of this type declared in the application.
    /// It provides:
    /// - detecting static fields of Settings types declared in the application and initiating them with Settings objects;
    /// - serializing/deserializing the Settings objects;
    /// Every Settings field in the application has it own storage file which is defined by the Settings type and the field's full name. 
    /// Usually it's that only one field is declared per Settings type, but generally there can be any number of fields of the same Settings type.
    /// </summary>
    public partial class Config
    {
        /// <summary>
        /// Tells Config which optional (i.e. attributed with [Settings.Optional]) Settings fields are to be initialized. 
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
            lock (fieldFullNames2SettingsField)
            {
                fieldFullNames2SettingsField.Clear(); 
                foreach (SettingsField settingsField in enumSettingsFields())
                {
                    if (null != settingsField.Info.GetCustomAttributes<Settings.Optional>(false).FirstOrDefault() && (RequiredOptionalFieldFullNamesRegex == null || !RequiredOptionalFieldFullNamesRegex.IsMatch(settingsField.FullName)))
                        continue;
                    Settings settings = Settings.Create(settingsField, reset, throwExceptionIfCouldNotLoadFromStorageFile);
                    settingsField.SetObject(settings);
                    fieldFullNames2SettingsField[settingsField.FullName] = settingsField;
                }
            }
        }
        static Dictionary<string, SettingsField> fieldFullNames2SettingsField = new Dictionary<string, SettingsField>();

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
        static IEnumerable<SettingsField> enumSettingsFields()
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
                    {//usually it is only 1 FieldInfo per Settings type
                        yield return new SettingsField(settingsTypeFieldInfo);
                    }
            }
        }

        /// <summary>
        /// Reloads all the Settings fields. 
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
        /// Resets all the Settings fields.
        /// First it tries to load each Settings object from the initial settings file in app's directory. 
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        static public void Reset()
        {
            loadOrReset(true, true);
        }

        /// <summary>
        /// Serializes all the Settings fields to their storage files.
        /// </summary>
        static public void Save()
        {
            lock (fieldFullNames2SettingsField)
            {
                foreach (SettingsField settingsField in fieldFullNames2SettingsField.Values)
                    settingsField.GetObject()?.Save();
            }
        }

        /// <summary>
        /// Returns the Settings object which is set to the field identified by the field's full name.
        /// The object must be previously created by Reload() or Reset().
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its file without extention</param>
        /// <returns>The Settings object which is set to the field</returns>
        static public Settings GetSettings(string settingsFieldFullName)
        {//!!! before altering this method, pay attention that it is used by Settings !!!
            lock (fieldFullNames2SettingsField)
            {
                if (fieldFullNames2SettingsField.TryGetValue(settingsFieldFullName, out SettingsField settingsField))
                    return settingsField.GetObject();
                return null;
            }
        }
    }
}