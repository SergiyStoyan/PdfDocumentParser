//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
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
        public static List<string> RequiredOptionalFieldFullNames = null;

        ///// <summary>
        ///// Tells Config which optional (i.e. attributed with [Settings.Optional]) Settings types are to be initialized. 
        ///// It must be set before calling Reload() or Reset().
        ///// </summary>
        //public static List<Type> RequiredOptionalSettingsTypes = null;

        /// <summary>
        /// Tells Config in which order Settings fields ordered by their types are to be initalized.        
        /// It may be necessary due to dependencies between Settings types.
        /// Types listed here will be initialized first in the provided order.
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static List<Type> InitializationOrderedSettingsTypes
        {
            get
            {
                return initializationOrderedSettingsTypes;
            }
            set
            {
                Type t = value?.Find(a => !a.IsSubclassOf(typeof(Settings)));
                if (t != null)
                    throw new Exception("InitializationOrderedSettingsTypes cannot accept a non-Settings type: " + t.FullName);
                initializationOrderedSettingsTypes = value?.Distinct().ToList();
            }
        }
        static List<Type> initializationOrderedSettingsTypes = null;

        /// <summary>
        /// Tells Config in which assemblies to look for Settings fields.
        /// If not set, the default assemblies are processed.      
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static List<Assembly> ExplicitlyTrackedAssemblies = null;

        /// <summary>
        /// Tells Config how long to go through referenced assemblies to look for Settings fields.
        /// The search scope is limited by it in order not to load all the possible assemblies which is unavoidable while enumerating through them.
        /// It is 0-based.
        /// It must be set before calling Reload() or Reset().
        /// </summary>
        public static int AssemblyChainMaxLength = 1;

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";

        static void set_settingsFieldFullNames2SettingsFieldInfo()
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                ExplicitlyTrackedAssemblies = ExplicitlyTrackedAssemblies?.Distinct().ToList();
                if (AssemblyChainMaxLength < 1)
                    throw new Exception("AssemblyChainMaxLength cannot be < 1");

                if (settingsFieldFullNames2SettingsFieldInfo_is_set)
                {
                    //ExplicitlyTrackedAssemblies is the parameter that can change SettingsFieldInfo collection
                    if (lastExplicitlyTrackedAssemblies?.Count == ExplicitlyTrackedAssemblies?.Count)
                    {
                        if (ExplicitlyTrackedAssemblies == null)
                            return;
                        if (!lastExplicitlyTrackedAssemblies.Except(ExplicitlyTrackedAssemblies).Any() && !ExplicitlyTrackedAssemblies.Except(lastExplicitlyTrackedAssemblies).Any())
                            return;
                    }

                    //AssemblyChainMaxLength is the parameter that can change SettingsFieldInfo collection
                    if (lastAssemblyChainMaxLength == AssemblyChainMaxLength)
                        return;
                }
                settingsFieldFullNames2SettingsFieldInfo_is_set = true;
                lastExplicitlyTrackedAssemblies = ExplicitlyTrackedAssemblies;
                lastAssemblyChainMaxLength = AssemblyChainMaxLength;

                foreach (SettingsFieldInfo settingsFieldInfo in getSettingsFieldInfos())
                {//SettingsFieldInfo's parameters for a Settings field are expected to be unchangable so no need to re-create it.
                    //!!!Exposing of SettingsFieldInfo to the custom code is one more reason not to re-create it.
                    if (!settingsFieldFullNames2SettingsFieldInfo.ContainsKey(settingsFieldInfo.FullName))//!!!do not replace existing SettingsFieldInfo's
                        settingsFieldFullNames2SettingsFieldInfo[settingsFieldInfo.FullName] = settingsFieldInfo;
                }
            }
        }
        static bool settingsFieldFullNames2SettingsFieldInfo_is_set = false;
        static List<Assembly> lastExplicitlyTrackedAssemblies = null;
        static int lastAssemblyChainMaxLength = 0;
        static Dictionary<string, SettingsFieldInfo> settingsFieldFullNames2SettingsFieldInfo = new Dictionary<string, SettingsFieldInfo>();
        //static IEnumerable<SettingsFieldInfo> getSettingsFieldInfos()
        //{
        //    List<Assembly> assemblies;
        //    if (ExplicitlyTrackedAssemblies != null)
        //        assemblies = new List<Assembly>(ExplicitlyTrackedAssemblies);
        //    else
        //    {
        //        Assembly configAssembly = Assembly.GetExecutingAssembly();
        //        StackTrace stackTrace = new StackTrace();
        //        Assembly callingAssembly = stackTrace.GetFrames().Select(f => f.GetMethod().DeclaringType.Assembly).Where(a => a != configAssembly).FirstOrDefault();
        //        if (callingAssembly == null)
        //            callingAssembly = Assembly.GetEntryAssembly();
        //        assemblies = new List<Assembly> { callingAssembly };
        //        string configAssemblyFullName = configAssembly.FullName;
        //        //!!!The search scope is limited as it is in order not to load more assemblies which is unavoidable while enumerating them.
        //        foreach (AssemblyName assemblyName in callingAssembly.GetReferencedAssemblies())
        //        {
        //            Assembly a = Assembly.Load(assemblyName);
        //            if (null != a.GetReferencedAssemblies().Where(an => an.FullName == configAssemblyFullName).FirstOrDefault())
        //                assemblies.Add(a);
        //        }
        //    }
        //    foreach (Assembly assembly in assemblies.Where(a => a != null).Distinct())
        //    {
        //        Type[] types = assembly.GetTypes();
        //        IEnumerable<Type> settingsTypes = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings)))/*.Distinct()*/;
        //        if (settingsTypes.Any())
        //            foreach (Type type in types)
        //                //!!!while FieldInfo can see property, it loses its attributes if any. So we need to retrieve by GetMembers().
        //                foreach (MemberInfo settingsTypeMemberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)/*.Where(f => settingsTypes.Contains(f.Member)*/ /* && f.FieldType.IsAssignableFrom(settingsType)*/)
        //                {
        //                    FieldInfo fi = settingsTypeMemberInfo as FieldInfo;
        //                    if (fi != null)
        //                    {
        //                        if (!fi.Name.StartsWith("<")/*it is a compiled property*/ && settingsTypes.Contains(fi.FieldType))
        //                            yield return new SettingsFieldFieldInfo(fi);
        //                    }
        //                    else
        //                    {
        //                        PropertyInfo pi = settingsTypeMemberInfo as PropertyInfo;
        //                        if (pi != null && settingsTypes.Contains(pi.PropertyType))
        //                            yield return new SettingsFieldPropertyInfo(pi);
        //                    }
        //                }
        //    }

        //    //List<Type> types = new List<Type>();
        //    //List<Type> settingsTypes = new List<Type>();
        //    //foreach (Assembly assembly in assemblies.Where(a => a != null).Distinct())
        //    //    types.AddRange(assembly.GetTypes());
        //    //settingsTypes.AddRange(types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings)))/*.Distinct()*/);
        //    //foreach (Type type in types)
        //    //    //!!!while FieldInfo can see property, it loses its attributes if any. So we need to retrieve by GetMembers().
        //    //    foreach (MemberInfo settingsTypeMemberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)/*.Where(f => settingsTypes.Contains(f.Member)*/ /* && f.FieldType.IsAssignableFrom(settingsType)*/)
        //    //    {
        //    //        FieldInfo fi = settingsTypeMemberInfo as FieldInfo;
        //    //        if (fi != null)
        //    //        {
        //    //            if (!fi.Name.StartsWith("<")/*it is a compiled property*/ && settingsTypes.Contains(fi.FieldType))
        //    //                yield return new SettingsFieldFieldInfo(fi);
        //    //        }
        //    //        else
        //    //        {
        //    //            PropertyInfo pi = settingsTypeMemberInfo as PropertyInfo;
        //    //            if (pi != null && settingsTypes.Contains(pi.PropertyType))
        //    //                yield return new SettingsFieldPropertyInfo(pi);
        //    //        }
        //    //    }
        //}
        class ReferenceAssembly
        {
            readonly int level;
            readonly Assembly assembly;
            readonly List<ReferenceAssembly> referenceAssemblies = new List<ReferenceAssembly>();
            readonly Type[] types;
            readonly List<Type> settingsTypes;

            public ReferenceAssembly(Assembly assembly, string configAssemblyFullName, int maxLevel) : this(assembly, configAssemblyFullName, 0, maxLevel)
            {
            }
            ReferenceAssembly(Assembly assembly, string configAssemblyFullName, int level, int maxLevel)
            {
                this.level = level;
                this.assembly = assembly;
                if (level <= maxLevel)
                    foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                    {
                        Assembly a = Assembly.Load(assemblyName);
                        if (null != a.GetReferencedAssemblies().Where(an => an.FullName == configAssemblyFullName).FirstOrDefault())
                            referenceAssemblies.Add(new ReferenceAssembly(a, configAssemblyFullName, level + 1, maxLevel));
                    }
                types = assembly.GetTypes();
                settingsTypes = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Settings)))/*.Distinct()*/.ToList();
            }

            public IEnumerable<SettingsFieldInfo> GetSettingsFieldInfos()
            {
                foreach (SettingsFieldInfo sfi in getSettingsFieldInfosForSettingsTypes(settingsTypes))
                    yield return sfi;
                foreach (ReferenceAssembly ra in referenceAssemblies)
                {
                    foreach (SettingsFieldInfo sfi in getSettingsFieldInfosForSettingsTypes(ra.settingsTypes))
                        yield return sfi;
                    foreach (SettingsFieldInfo sfi in ra.GetSettingsFieldInfos())
                        yield return sfi;
                }
            }
            IEnumerable<SettingsFieldInfo> getSettingsFieldInfosForSettingsTypes(List<Type> settingsTypes)
            {
                if (settingsTypes.Count < 1)
                    yield break;
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
        static IEnumerable<SettingsFieldInfo> getSettingsFieldInfos()
        {
            List<ReferenceAssembly> referenceAssembly0s = new List<ReferenceAssembly>();
            Assembly configAssembly = Assembly.GetExecutingAssembly();
            if (ExplicitlyTrackedAssemblies != null)
                foreach (Assembly a in ExplicitlyTrackedAssemblies)
                    referenceAssembly0s.Add(new ReferenceAssembly(a, configAssembly.FullName,
                    /*The search scope is limited as it is in order not to load more assemblies which is unavoidable while enumerating them.*/AssemblyChainMaxLength));
            else
            {
                StackTrace stackTrace = new StackTrace();
                Assembly callingAssembly = stackTrace.GetFrames().Select(f => f.GetMethod().DeclaringType.Assembly).Where(a => a != configAssembly).FirstOrDefault();
                if (callingAssembly == null)
                    callingAssembly = Assembly.GetEntryAssembly();
                referenceAssembly0s.Add(new ReferenceAssembly(callingAssembly, configAssembly.FullName,
                    /*The search scope is limited as it is in order not to load more assemblies which is unavoidable while enumerating them.*/AssemblyChainMaxLength));
            }
            foreach (ReferenceAssembly ra in referenceAssembly0s)
                foreach (SettingsFieldInfo sfi in ra.GetSettingsFieldInfos())
                    yield return sfi;
        }

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

        static void loadOrReset(bool reset)
        {
            lock (settingsFieldFullNames2SettingsFieldInfo)
            {
                set_settingsFieldFullNames2SettingsFieldInfo();
                IEnumerable<SettingsFieldInfo> settingsFieldInfos = settingsFieldFullNames2SettingsFieldInfo.Values;
                if (InitializationOrderedSettingsTypes != null)
                {
                    SettingsTypeComparer settingsTypeComparer = new SettingsTypeComparer(InitializationOrderedSettingsTypes);
                    settingsFieldInfos = settingsFieldInfos.OrderBy(a => a.Type, settingsTypeComparer);
                }
                HashSet<string> requiredOptionalFieldFullNames = RequiredOptionalFieldFullNames == null ? null : new HashSet<string>(RequiredOptionalFieldFullNames);
                foreach (SettingsFieldInfo settingsFieldInfo in settingsFieldInfos)
                    if (!settingsFieldInfo.Optional /*|| RequiredOptionalSettingsTypes?.Contains(settingsFieldInfo.Type) == true*/ || requiredOptionalFieldFullNames?.Remove(settingsFieldInfo.FullName) == true)
                        settingsFieldInfo.SetObject(Settings.Create(settingsFieldInfo, reset));
                if (requiredOptionalFieldFullNames?.Count > 0)
                    throw new Exception("RequiredOptionalFieldFullNames contains name which was not found: '" + RequiredOptionalFieldFullNames[0] + "'");
            }
        }

        /// <summary>
        /// Reloads all the Settings fields in the application.
        /// It's the usual method to be called in the beginning of an application to initialize Config.
        /// First it tries to load each Settings object from its default storage directory. 
        /// If this file does not exist, it tries to load from the initial settings file in app's directory.
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        static public void Reload()
        {
            loadOrReset(false);
        }

        /// <summary>
        /// Resets all the Settings fields in the application.
        /// First it tries to load each Settings object from the initial settings file in app's directory. 
        /// Only if this file does not exist, it resets to the hardcoded values.
        /// </summary>
        static public void Reset()
        {
            loadOrReset(true);
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
                if (!settingsFieldFullNames2SettingsFieldInfo.TryGetValue(settingsFieldFullName, out SettingsFieldInfo settingsFieldInfo))
                {
                    set_settingsFieldFullNames2SettingsFieldInfo();
                    if (!settingsFieldFullNames2SettingsFieldInfo.TryGetValue(settingsFieldFullName, out settingsFieldInfo))
                        throw new Exception("Settings field with full name '" + settingsFieldFullName + "' was not found.");
                }
                return settingsFieldInfo;
            }
        }
    }
}
