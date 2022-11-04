//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Cliver
{
    /// <summary>
    /// Base class for Enum implementation.
    /// </summary>
    /// <typeparam name="V">value type</typeparam>
    public class Enum<V>
    {
        public override string ToString()
        {
            return Value?.ToString();
        }

        /// <summary>
        /// Create a Enum from a value.
        /// </summary>
        /// <param name="value"></param>
        protected Enum(V value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of this Enum.
        /// </summary>
        public V Value { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        static public Dictionary<string, E> ToDictionary<E>() where E : Enum<V>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).ToDictionary(x => x.Name, x => ((E)x.GetValue(null)));
        }
        //static public Dictionary<string, V> ToDictionary<V>()
        //{
        //    return typeof(V).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).ToDictionary(x => x.Name, x => (V)x.GetValue(null));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        static public List<E> ToList<E>() where E : Enum<V>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).Select(x => ((E)x.GetValue(null))).ToList();
        }

        static public List<V> GetValues<V>()
        {
            return typeof(V).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).Select(x => (V)x.GetValue(null)).ToList();
        }

        /// <summary>
        /// Convert a value string into a Enum.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="valueStr"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        static public E Parse<E>(string valueStr, bool ignoreCase = true) where E : Enum<V>
        {
            if (valueStr != null)
                valueStr = valueStr.Trim();
            foreach (E e in ToList<E>())
            {
                if (e.Value == null)
                {
                    if (valueStr == null)
                        return e;
                    continue;
                }
                if (valueStr == null)
                    continue;
                if (0 == string.Compare(e.Value.ToString(), valueStr, ignoreCase))
                    return e;
            }
            return null;
        }

        /// <summary>
        /// Check if this Enum is included in a given collection.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public bool IsAmong<E>(params E[] objects) where E : Enum<V>
        {
            return IsAmong(objects);
        }

        /// <summary>
        /// Check if this Enum is included in a given collection.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public bool IsAmong<E>(IEnumerable< E> objects) where E : Enum<V>
        {
            return objects.FirstOrDefault(a => a == (E)this) != null;
        }
    }

    internal class EnumExample : Cliver.Enum<string>
    {
        public static readonly EnumExample NULL = new EnumExample(null);
        public static readonly EnumExample EMPTY = new EnumExample(string.Empty);
        public static readonly EnumExample VALUE1 = new EnumExample("VALUE1");
        public static readonly EnumExample VALUE2 = new EnumExample("VALUE2");
        public static readonly EnumExample VALUE3 = new EnumExample("VALUE3");

        EnumExample(string value) : base(value) { }

        public static implicit operator string(EnumExample o) => o?.Value;
        public static implicit operator EnumExample(string s) => Parse<EnumExample>(s);
    }

    //internal class StringEnum : Cliver.Enum<string>
    //{
    //    StringEnum(string value) : base(value) { }

    //    public bool IsMatch(params StringEnum[] objects) 
    //    {
    //        return objects.FirstOrDefault(a => a == System.Text.RegularExpressions.Regex.IsMatch( (E)this) != null;
    //    }
    //}

    //public static class StringValues
    //{
    //    public static Dictionary<string, string> ToDictionary()
    //    {
    //        return typeof(StringValues).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(string)).ToDictionary(x => x.Name, x => (string)x.GetValue(null));
    //    }

    //    public static List<string> ToList()
    //    {
    //        return typeof(StringValues).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(string)).Select(x => (string)x.GetValue(null)).ToList();
    //    }
    //}
}