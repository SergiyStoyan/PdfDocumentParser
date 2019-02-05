﻿//********************************************************************************************
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

        new public virtual string ToString()
        {
            return Value.ToString();
        }

        protected NullableEnum(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public Dictionary<string, T> ToDictionary()
        {
            return this.GetType().GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => ((Enum<T>)x.GetValue(this)).Value);
        }
    }

    public class Enum<T>
    {
        new public virtual string ToString()
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
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x => ((Enum<T>)x.GetValue(this)).Value);
        }

        public List<T> ToList()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => ((Enum<T>)x.GetValue(this)).Value).ToList();
        }
    }
}

