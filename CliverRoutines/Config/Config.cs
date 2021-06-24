//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
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
    /// Config manages values of the static public fields/properties of Settings-derived types that are declared anywhere in the application.
    /// It provides:
    /// - detecting static public fields of Settings types declared in the application and initiating them with values;
    /// - serializing/deserializing those Settings fields/properties;
    /// Every Settings field/property has it own storage file which is defined by its type and its full name in code. 
    /// Usually it's that only one field is declared per Settings type, but generally there can be any number of fields of the same Settings type.
    /// </summary>
    public static partial class Config
    {
        /// <summary>
        /// Tells Config which optional (i.e. attributed with [Settings.Optional]) Settings fields are to be initialized. 
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static Regex RequiredOptionalFieldFullNamesRegex = null;

        /// <summary>
        /// Tells Config in which order Settings fields ordered by their types are to be initalized.        
        /// It may be necessary due to dependencies between Settings types.
        /// Types listed here will be initialized first in the provided order.
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static List<Type> InitializationOrderedSettingsTypes = null;

        /// <summary>
        /// Tells Config in which assemblies to look for Settings fields.
        /// If not set, the default assemblies are processed.      
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static List<Assembly> ExplicitlyTrackedAssemblies = null;

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";

        static void loadOrReset(bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                settingsFieldFullNames2SettingsFieldInfo.Clear();
                foreach (SettingsFieldInfo settingsFieldInfo in EnumSettingsFieldInfos())
                {
                    if (settingsFieldInfo.Optional == true && RequiredOptionalFieldFullNamesRegex?.IsMatch(settingsFieldInfo.FullName) == false)
                        continue;
                    settingsFieldInfo.SetObject(Settings.Create(settingsFieldInfo, reset, throwExceptionIfCouldNotLoadFromStorageFile));
                    settingsFieldFullNames2SettingsFieldInfo[settingsFieldInfo.FullName] = settingsFieldInfo;
                }
            }
        }
        static Dictionary<string, SettingsFieldInfo> settingsFieldFullNames2SettingsFieldInfo = new Dictionary<string, SettingsFieldInfo>();

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

        /// <summary>
        /// Enumerates through all the Settings fields available in the application.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SettingsFieldInfo> EnumSettingsFieldInfos()
        {
            List<Assembly> assemblies;
            if (ExplicitlyTrackedAssemblies != null)
                assemblies = new List<Assembly>(ExplicitlyTrackedAssemblies);
            else
            {
                Assembly configAssembly = Assembly.GetExecutingAssembly();
                StackTrace stackTrace = new StackTrace();
                Assembly callingAssembly = stackTrace.GetFrames().Select(f => f.GetMethod().DeclaringType.Assembly).Where(a => a != configAssembly).FirstOrDefault();
                if (callingAssembly == null)
                    callingAssembly = Assembly.GetEntryAssembly();
                assemblies = new List<Assembly> { callingAssembly };
                string configAssemblyFullName = configAssembly.FullName;
                //!!!The search scope is limited as it is in order not to load more assemblies which is unavoidable while enumerating them.
                foreach (AssemblyName assemblyName in callingAssembly.GetReferencedAssemblies())
                {
                    Assembly a = Assembly.Load(assemblyName);
                    if (null != a.GetReferencedAssemblies().Where(an => an.FullName == configAssemblyFullName).FirstOrDefault())
                        assemblies.Add(a);
                }
            }
            assemblies.RemoveAll(a => a == null);
            assemblies.Distinct();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> settingsTypes = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings))).Distinct();
                if (InitializationOrderedSettingsTypes != null)
                {
                    SettingsTypeComparer settingsTypeComparer = new SettingsTypeComparer(InitializationOrderedSettingsTypes);
                    settingsTypes = settingsTypes.OrderBy(t => t, settingsTypeComparer);
                }
                foreach (Type type in types)
                    //!!!while FieldInfo can see property, it loses its attributes if any. So we need to retrieve by GetMembers().
                    foreach (MemberInfo settingsTypeMemberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)/*.Where(f => settingsTypes.Contains(f.Member)*/ /* && f.FieldType.IsAssignableFrom(settingsType)*/)
                    {
                        FieldInfo fi = settingsTypeMemberInfo as FieldInfo;
                        if (fi != null)
                        {
                            if (!fi.Name.StartsWith("<")/*it is a compiled property*/ && settingsTypes.Contains(fi.FieldType))
                                yield return new SettingsFieldFieldInfo(fi);
                        }
                        else
                        {
                            PropertyInfo pi = settingsTypeMemberInfo as PropertyInfo;
                            if (pi != null && settingsTypes.Contains(pi.PropertyType))
                                yield return new SettingsFieldPropertyInfo(pi);
                        }
                    }
            }
        }

        /// <summary>
        /// Reloads all the Settings fields in the application.
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
        /// Resets all the Settings fields in the application.
        /// First it tries to load each Settings object from the initial settings file in app's directory. 
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        static public void Reset()
        {
            loadOrReset(true, true);
        }

        /// <summary>
        /// Serializes all the Settings fields initialized in the application to their storage files.
        /// </summary>
        static public void Save()
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                foreach (SettingsFieldInfo settingsFieldInfo in settingsFieldFullNames2SettingsFieldInfo.Values)
                    settingsFieldInfo.GetObject()?.Save(settingsFieldInfo);
            }
        }

        /// <summary>
        /// Allows to get the Settings field's properties before its value has been created (i.e. before the Settings field has been initialized).
        /// </summary>
        /// <param name="settingsFieldFullName">full name of Settings field; it equals to the name of its storage file without extention</param>
        /// <returns>Settings field's properties</returns>
        public static SettingsFieldInfo GetSettingsFieldInfo(string settingsFieldFullName)
        {//!!! before altering this method, pay attention that it is used by the engine !!!
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                if (!settingsFieldFullNames2SettingsFieldInfo.TryGetValue(settingsFieldFullName, out SettingsFieldInfo sfi))
                {
                    sfi = EnumSettingsFieldInfos().FirstOrDefault(a => a.FullName == settingsFieldFullName);
                    if (sfi == null)
                        throw new Exception("Settings field with full name: '" + settingsFieldFullName + "' does not exist.");
                    settingsFieldFullNames2SettingsFieldInfo[settingsFieldFullName] = sfi;
                }
                return sfi;
            }
        }
    }
}