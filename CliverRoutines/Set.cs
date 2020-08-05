/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cliver
{
    public class Set<V, T> where V : Enum<T>
    {
        public override string ToString()
        {
            return string.Join(",", Values.Select(a => a.ToString()));
        }

        protected Set(IEnumerable<V> values)
        {
            if (values == null)
                Values = new HashSet<V>();
            else
                Values = new HashSet<V>(values);
        }

        public HashSet<V> Values { get; protected set; }

        public S Append<S>(S set) where S : Set<V, T>
        {
            return Append<S>(set.Values.ToArray());
        }

        public S Append<S>(params V[] values) where S : Set<V, T>
        {
            foreach (V v in values)
                Values.Add(v);
            return (S)this;
        }

        public S Remove<S>(S set) where S : Set<V, T>
        {
            return Remove<S>(set.Values.ToArray());
        }

        public S Remove<S>(params V[] values) where S : Set<V, T>
        {
            foreach (V v in values)
                Values.Remove(v);
            return (S)this;
        }

        public static bool operator ==(Set<V, T> a, Set<V, T> b)
        {
            if (a.Values.Count != b.Values.Count)
                return false;
            foreach (V v in b.Values)
                if (!a.Values.Contains(v))
                    return false;
            return true;
        }

        public static bool operator !=(Set<V, T> a, Set<V, T> b)
        {
            return !(a == b);
        }

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

        public bool Contains<S>(params V[] values) where S : Set<V, T>
        {
            return AcontainsB<S>(Values, values);
        }

        public bool Contained<S>(params V[] values) where S : Set<V, T>
        {
            return AcontainsB<S>(values, Values);
        }

        static bool AcontainsB<S>(IEnumerable<V> aValues, IEnumerable<V> bValues) where S : Set<V, T>
        {
            if (bValues == null)
                return true;
            if (aValues == null)
                return false;
            foreach (V v in bValues)
                if (!aValues.Contains(v))
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
            SetExample v2 = new SetExample(EnumExample.VALUE2, EnumExample.VALUE3);
            SetExample v123 = new SetExample(EnumExample.VALUE1, EnumExample.VALUE2, EnumExample.VALUE3);
            SetExample se = empty.Append(v123).Remove<SetExample>(EnumExample.VALUE3);
        }
    }
}