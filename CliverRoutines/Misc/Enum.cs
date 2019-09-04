//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Cliver
{
    public class NullableEnum<T> where T : class
    {
        public static readonly NullableEnum<T> NULL = new NullableEnum<T>(null);

        public override string ToString()
        {
            return Value.ToString();
        }

        protected NullableEnum(T value)
        {
            Value = value;
        }

        public NullableEnum()
        {
        }

        public T Value { get; private set; }

        public Dictionary<string, T> ToDictionary()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(NullableEnum<T>))).ToDictionary(x => x.Name, x => ((NullableEnum<T>)x.GetValue(this)).Value);
        }

        public List<T> ToList()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(NullableEnum<T>))).Select(x => ((NullableEnum<T>)x.GetValue(this)).Value).ToList();
        }
    }

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

        public Enum()
        {
        }

        public T Value { get; private set; }

        public Dictionary<string, T> ToDictionary()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<T>))).ToDictionary(x => x.Name, x => ((Enum<T>)x.GetValue(this)).Value);
        }

        public List<T> ToList()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType.IsSubclassOf(typeof(Enum<T>))).Select(x => ((Enum<T>)x.GetValue(this)).Value).ToList();
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