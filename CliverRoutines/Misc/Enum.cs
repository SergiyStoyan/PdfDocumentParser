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
using System.Linq;
using System.Reflection;


namespace Cliver
{
    public class Enum<T>
    {
        public override string ToString()
        {
            return Value.ToString();
        }

        protected Enum(T value)
        {
            Value = value;
        }

        public T Value { get; protected set; }

        static public Dictionary<string, E> ToDictionary<E>() where E : Enum<T>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<T>))).ToDictionary(x => x.Name, x => ((E)x.GetValue(null)));
        }

        static public List<E> ToList<E>() where E : Enum<T>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<T>))).Select(x => ((E)x.GetValue(null))).ToList();
        }

        static public E Parse<E>(string value, bool ignoreCase = false) where E : Enum<T>
        {
            value = value.Trim();
            foreach (E e in ToList<E>())
            {
                if (0 == string.Compare(e.Value.ToString(), value, ignoreCase))
                    return e;
            }
            return null;
        }

        public bool IsAmong<E>(params E[] values) where E : Enum<T>
        {
            return values.FirstOrDefault(a => a == (E)this) != null;
        }
    }

    public class NullableEnum<T> : Enum<T> where T : class
    {
        //Example! You have to define this field in your class
        //public static readonly NullableEnum<T> NULL = new NullableEnum<T>(null);

        protected NullableEnum(T value) : base(value)
        {
        }

        new static public E Parse<E>(string value, bool ignoreCase = false) where E : Enum<T>
        {
            if (value != null)
                value = value.Trim();
            foreach (E e in ToList<E>())
            {
                if (e.Value == null)
                    if (value == null)
                        return e;
                    else
                        continue;
                if (value == null)
                    continue;
                if (0 == string.Compare(e.Value.ToString(), value, ignoreCase))
                    return e;
            }
            return null;
        }
    }

    internal class EnumExample : Cliver.Enum<string>
    {
        public static readonly EnumExample EMPTY = new EnumExample("");
        public static readonly EnumExample VALUE1 = new EnumExample("VALUE1");
        public static readonly EnumExample VALUE2 = new EnumExample("VALUE2");
        public static readonly EnumExample VALUE3 = new EnumExample("VALUE3");

        internal EnumExample(string value) : base(value) { }
    }

    /// <summary>
    /// TBD!!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Set<T>
    {
        public override string ToString()
        {
            return string.Join(",", Values.Select(a => a.ToString()));
        }

        protected Set(IEnumerable<T> values)
        {
            Values = values.ToList();
        }

        public List<T> Values { get; protected set; }

        static public Dictionary<string, S> ToDictionary<S>() where S : Set<T>
        {
            return typeof(S).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Set<T>))).ToDictionary(x => x.Name, x => ((S)x.GetValue(null)));
        }

        static public List<S> ToList<S>() where S : Set<T>
        {
            return typeof(S).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Set<T>))).Select(x => ((S)x.GetValue(null))).ToList();
        }

        //static public S Parse<S>(string values, string separator=",", bool ignoreCase = false) where S : Set<T>
        //{
        //    value = value.Trim();
        //    foreach (T v in ToList<E>())
        //    {
        //        if (0 == string.Compare(v.ToString(), value, ignoreCase))
        //            return (E)Activator.CreateInstance(typeof(E), v);
        //    }
        //    return null;
        //}

        //public bool ContainsAll(IEnumerable<T> values)
        //{

        //}
    }

    public static class StringValues
    {
        public static Dictionary<string, string> ToDictionary()
        {
            return typeof(StringValues).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(string)).ToDictionary(x => x.Name, x => (string)x.GetValue(null));
        }

        public static List<string> ToList()
        {
            return typeof(StringValues).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(string)).Select(x => (string)x.GetValue(null)).ToList();
        }
    }
}