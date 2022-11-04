/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cliver
{
    /// <summary>
    /// Base class for Set implementation.
    /// </summary>
    /// <typeparam name="V">Cliver.Enum type</typeparam>
    /// <typeparam name="T">Cliver.Enum value type</typeparam>
    public class Set<V, T> where V : Enum<T>
    {
        public override string ToString()
        {
            return string.Join(",", Values.Select(a => a.ToString()));
        }

        /// <summary>
        /// Create a Set from a collection of values.
        /// </summary>
        /// <param name="values"></param>
        protected Set(IEnumerable<V> values)
        {
            if (values == null)
                Values = new HashSet<V>();
            else
                Values = new HashSet<V>(values);
        }

        /// <summary>
        /// Collection of Enum's contained by this Set
        /// </summary>
        public HashSet<V> Values { get; protected set; }

        /// <summary>
        /// Enhance this Set with another one.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public S Add<S>(S set) where S : Set<V, T>
        {
            return Add<S>(set.Values.ToArray());
        }

        /// <summary>
        /// Enhance this Set with a collection of Enum's.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public S Add<S>(params V[] values) where S : Set<V, T>
        {
            foreach (V v in values)
                Values.Add(v);
            return (S)this;
        }

        /// <summary>
        /// Diminish this Set by another Set.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public S Subtract<S>(S set) where S : Set<V, T>
        {
            return Subtract<S>(set.Values.ToArray());
        }

        /// <summary>
        /// Diminish this Set by a collection of Enum's.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public S Subtract<S>(params V[] values) where S : Set<V, T>
        {
            foreach (V v in values)
                Values.Remove(v);
            return (S)this;
        }

        /// <summary>
        /// 2 Set's comprise of the same Enum's.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Set<V, T> a, Set<V, T> b)
        {
            if (a.Values.Count != b.Values.Count)
                return false;
            foreach (V v in b.Values)
                if (!a.Values.Contains(v))
                    return false;
            return true;
        }

        //public override bool Equals(object obj)
        //{
        //    Set<V, T> b = obj as Set<V, T>;
        //    if (b == null)
        //        return false;
        //    return this == b;
        //}

        /// <summary>
        /// 2 Set's do not comprise of the same Enum's.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Set<V, T> a, Set<V, T> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Convert a values string into a Set.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="valuesStr"></param>
        /// <param name="separator"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        static public S Parse<S>(string valuesStr, string separator = ",", bool ignoreCase = false) where S : Set<V, T>
        {
            if (valuesStr == null)
                valuesStr = string.Empty;
            HashSet<V> allValues = new HashSet<V>((List<V>)typeof(V).GetMethod("ToList").Invoke(null, null));
            HashSet<V> foundValues = new HashSet<V>();
            string[] vss = System.Text.RegularExpressions.Regex.Split(valuesStr, @"\s*" + separator + @"\s*");
            foreach (string vs in vss)
            {
                V v = allValues.FirstOrDefault(a => 0 == string.Compare(vs, a.ToString(), ignoreCase));
                if (v == null)
                    return null;
                foundValues.Add(v);
            }
            S s = (S)Activator.CreateInstance(typeof(S));
            s.Values = foundValues;
            return s;
        }

        /// <summary>
        /// Checks if this Set includes all given values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Contains<S>(params V[] values) where S : Set<V, T>
        {
            return Contains<S>(values);
        }

        /// <summary>
        /// Checks if this Set includes all given values.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Contains<S>(IEnumerable<V> values) where S : Set<V, T>
        {
            if (values == null)
                return true;
            foreach (V v in values)
                if (!Values.Contains(v))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if this Set includes all the values of a given Set.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public bool Contains<S>(S set) where S : Set<V, T>
        {
            foreach (V v in set.Values)
                if (!Values.Contains(v))
                    return false;
            return true;
        }
    }

    internal class SetExample : Cliver.Set<EnumExample, string>
    {
        internal SetExample(IEnumerable<EnumExample> values) : base(values) { }
        internal SetExample(params EnumExample[] values) : base(values) { }

        static void test()
        {
            SetExample empty = new SetExample(EnumExample.EMPTY);
            SetExample v1 = new SetExample(EnumExample.VALUE1);
            SetExample v23 = new SetExample(EnumExample.VALUE2, EnumExample.VALUE3);
            SetExample v123 = new SetExample(EnumExample.VALUE1, EnumExample.VALUE2, EnumExample.VALUE3);
            SetExample se = empty.Add(v123).Subtract<SetExample>(EnumExample.VALUE3);
        }
    }
}