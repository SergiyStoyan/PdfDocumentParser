﻿/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cliver
{
    /// <summary>
    /// Features:
    /// - auto-generating values;
    /// - auto-disposing IDisposable values which have left the dictionary;
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
                    Clear();
                    keys2value = null;
                }
            }
        }

        virtual public void Clear()
        {
            lock (this)
            {
                foreach (VT v in keys2value.Values)
                    if (v != null && v is IDisposable)
                        ((IDisposable)v).Dispose();
                keys2value.Clear();
            }
        }

        public void Unset(KT key)
        {
            lock (this)
            {
                if (keys2value.TryGetValue(key, out VT v) && v != null && v is IDisposable)
                {
                    int vKeyCount = 0;
                    keys2value.Values.Where(a => a.Equals(v)).TakeWhile(a => ++vKeyCount < 2);
                    if (vKeyCount < 2)//make sure it is the only inclusion of the object
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
                    Unset(key);
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