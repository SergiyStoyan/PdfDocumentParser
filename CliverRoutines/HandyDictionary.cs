/********************************************************************************************
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

        public void Remove(KT key)
        {
            lock (this)
            {
                if (keys2value.TryGetValue(key, out VT v))
                    dispose(v);
                keys2value.Remove(key);
            }
        }

        void dispose(VT value)
        {
            lock (this)
            {
                if (value == null || !(value is IDisposable))
                    return;
                int vKeyCount = 0;
                keys2value.Values.Where(a => a.Equals(value)).TakeWhile(a => ++vKeyCount < 2);
                if (vKeyCount < 2)//make sure it is the only inclusion of the object
                    ((IDisposable)value).Dispose();
            }
        }

        /// <summary>
        /// It is safe: returns default if does not exists.
        /// To check for existance, use TryGetValue().
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public VT this[KT key]
        {
            get
            {
                lock (this)
                {
                    TryGetValue(key, out VT v);
                    return v;
                }
            }
            set
            {
                lock (this)
                {
                    if (keys2value.TryGetValue(key, out VT v) && v != null && !v.Equals(value))
                        dispose(v);
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

        public void Add(KT key, VT value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Attention: the count can be implicitly changed due to auto-generating value function
        /// </summary>
        public int Count
        {
            get
            {
                return keys2value.Count;
            }
        }
    }
}