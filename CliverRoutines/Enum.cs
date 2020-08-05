//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Cliver
{
    public class Enum<V>
    {
        public override string ToString()
        {
            return Value.ToString();
        }

        protected Enum(V value)
        {
            Value = value;
        }

        public V Value { get; protected set; }

        static public Dictionary<string, E> ToDictionary<E>() where E : Enum<V>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).ToDictionary(x => x.Name, x => ((E)x.GetValue(null)));
        }

        static public List<E> ToList<E>() where E : Enum<V>
        {
            return typeof(E).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<V>))).Select(x => ((E)x.GetValue(null))).ToList();
        }

        static public E Parse<E>(string valueStr, bool ignoreCase = false) where E : Enum<V>
        {
            if (valueStr != null)
                valueStr = valueStr.Trim();
            foreach (E e in ToList<E>())
            {
                if (e.Value == null)
                    if (valueStr == null)
                        return e;
                    else
                        continue;
                if (valueStr == null)
                    continue;
                if (0 == string.Compare(e.Value.ToString(), valueStr, ignoreCase))
                    return e;
            }
            return null;
        }

        public bool IsAmong<E>(params E[] objects) where E : Enum<V>
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
    }

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