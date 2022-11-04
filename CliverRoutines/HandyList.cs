/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
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
    /// - auto-disposing IDisposable values which have left the list;
    /// </summary>
    /// <typeparam name="VT"></typeparam>
    public class HandyList<VT> : IDisposable, IEnumerable<VT> //where VT: class
    {
        public HandyList()
        {

        }

        public HandyList(IEnumerable<VT> list)
        {
            values = list.ToList();
        }

        ~HandyList()
        {
            Dispose();
        }

        virtual public void Dispose()
        {
            lock (this)
            {
                if (values != null)
                {
                    Clear();
                    values = null;
                }
            }
        }

        virtual public void Clear()
        {
            lock (this)
            {
                foreach (VT v in values)
                    if (v != null && v is IDisposable)
                        ((IDisposable)v).Dispose();
                values.Clear();
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {                
                    dispose(values[index]);
                values.RemoveAt(index);
            }
        }

        void dispose(VT value)
        {
            lock (this)
            {
                if (value == null || !(value is IDisposable))
                    return;
                int vKeyCount = 0;
                values.Where(a => a.Equals(value)).TakeWhile(a => ++vKeyCount < 2);
                if (vKeyCount < 2)//make sure it is the only inclusion of the object
                    ((IDisposable)value).Dispose();
            }
        }

        /// <summary>
        /// It is safe: returns default if does not exists.
        /// To check for existance, use TryGetValue().
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public VT this[int index]
        {
            get
            {
                lock (this)
                {
                    return values[index];
                }
            }
            set
            {
                lock (this)
                {
                    VT v = values[index];
                    if (v != null && !v.Equals(value))
                    {
                        int vKeyCount = 0;
                        values.Where(a => a.Equals(v)).TakeWhile(a => ++vKeyCount < 2);
                        if (vKeyCount < 2)//make sure it is the only inclusion of the object
                            dispose(v);
                    }
                    values[index] = value;
                }
            }
        }
        List<VT> values = new List<VT>();

        public IEnumerator<VT> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(VT value)
        {
           values.Add(value);
        }

        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public List<VT> GetRange(int index, int count)
        {
            return values.GetRange(index, count);
        }

        public List<VT> AsList()
        {
            return values;
        }
    }
}