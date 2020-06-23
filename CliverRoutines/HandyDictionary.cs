/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cliver
{
    /// <summary>
    /// Features:
    /// - auto-generating values;
    /// - auto-disposing IDisposable values;
    /// </summary>
    /// <typeparam name="KT"></typeparam>
    /// <typeparam name="VT"></typeparam>
    public class HandyDictionary<KT, VT> : IDisposable, IEnumerable<KeyValuePair<KT, VT>> //where VT: class
    {
        /// <summary>
        /// Create HandyDictionary with auto-generating value function.
        /// </summary>
        /// <param name="getValue"></param>
        public HandyDictionary(GetValue getValue)
        {
            this.getValue = getValue;
        }
        public delegate VT GetValue(KT key);
        protected GetValue getValue;

        /// <summary>
        /// Create HandyDictionary without auto-generating value function.
        /// </summary>
        /// <param name="defaultValue"></param>
        public HandyDictionary(VT defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        protected VT defaultValue;

        public HandyDictionary()
        {
            defaultValue = default;
        }

        ~HandyDictionary()
        {
            Dispose();
        }

        virtual public void Dispose()
        {
            lock (this)
            {
                if (keys2value != null)
                {
                    if (IsDisposable(typeof(VT)))
                        foreach (VT v in keys2value.Values)
                            if (v != null)
                                ((IDisposable)v).Dispose();
                    keys2value = null;
                }
            }
        }

        static bool IsDisposable(Type type)
        {
            return typeof(IDisposable).IsAssignableFrom(type);
        }

        virtual public void Clear()
        {
            lock (this)
            {
                if (IsDisposable(typeof(VT)))
                    foreach (VT v in keys2value.Values)
                        if (v != null)
                            ((IDisposable)v).Dispose();
                keys2value.Clear();
            }
        }

        public void Unset(KT key)
        {
            lock (this)
            {
                if (IsDisposable(typeof(VT)))
                {
                    VT v;
                    if (keys2value.TryGetValue(key, out v))
                        if (v != null)
                            ((IDisposable)v).Dispose();
                }
                keys2value.Remove(key);
            }
        }

        public VT this[KT key]
        {
            get
            {
                lock (this)
                {
                    VT v;
                    if (!keys2value.TryGetValue(key, out v))
                    {
                        if (getValue == null)
                            return defaultValue;
                        v = getValue(key);
                        keys2value[key] = v;
                    }
                    return v;
                }
            }
            set
            {
                lock (this)
                {
                    keys2value[key] = value;
                }
            }
        }
        Dictionary<KT, VT> keys2value = new Dictionary<KT, VT>();

        public IEnumerator<KeyValuePair<KT, VT>> GetEnumerator()
        {
            return keys2value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Dictionary<KT, VT>.KeyCollection Keys
        {
            get
            {
                return keys2value.Keys;
            }
        }

        public Dictionary<KT, VT>.ValueCollection Values
        {
            get
            {
                return keys2value.Values;
            }
        }

        public bool TryGetValue(KT key, out VT value)
        {
            if (!keys2value.TryGetValue(key, out value))
            {
                if (getValue == null)
                {
                    value = defaultValue;
                    return false;
                }
                value = getValue(key);
                keys2value[key] = value;
            }
            return true;
        }
    }
}