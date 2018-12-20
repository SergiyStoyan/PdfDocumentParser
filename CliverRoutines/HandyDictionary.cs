/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cliver
{
    public class HandyDictionary<KT, VT> : IDisposable, IEnumerable<KeyValuePair<KT, VT>> //where VT: class
    {
        public HandyDictionary(Func<KT, VT> get_value)
        {
            getValue = get_value;
        }
        protected Func<KT, VT> getValue;

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

        static bool IsDisposable(Type t)
        {
            return typeof(IDisposable).IsAssignableFrom(t);
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

        public void Unset(KT k)
        {
            lock (this)
            {
                if (IsDisposable(typeof(VT)))
                {
                    VT v;
                    if (keys2value.TryGetValue(k, out v))
                        if (v != null)
                            ((IDisposable)v).Dispose();
                }
                keys2value.Remove(k);
            }
        }

        public VT this[KT k]
        {
            get
            {
                lock (this)
                {
                    VT v;
                    if (!keys2value.TryGetValue(k, out v))
                    {
                        v = getValue(k);
                        keys2value[k] = v;
                    }
                    return v;
                }
            }
            protected set
            {
                lock (this)
                {
                    keys2value[k] = value;
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
    }
}

///********************************************************************************************
//        Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Cliver
//{
//    public class HandyDictionary<KT, VT> : Dictionary<KT, VT>, IDisposable  //where VT: class
//    {
//        public HandyDictionary(Func<KT, VT> get_object)
//        {
//            getObject = get_object;
//        }
//        Func<KT, VT> getObject;

//        ~HandyDictionary()
//        {
//            Dispose();
//        }

//        public Enumerator GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        virtual public void Dispose()
//        {
//            Clear();
//        }

//        static bool IsDisposable(Type t)
//        {
//            return typeof(IDisposable).IsAssignableFrom(t);
//        }

//        virtual public void Clear()
//        {
//            lock (this)
//            {
//                if (IsDisposable(typeof(VT)))
//                    foreach (VT v in base.Values)
//                        ((IDisposable)v).Dispose();
//                base.Clear();
//            }
//        }

//        public VT this[KT k]
//        {
//            get
//            {
//                lock (this)
//                {
//                    VT v;
//                    if (!base.TryGetValue(k, out v))
//                    {
//                        v = getObject(k);
//                        base[k] = v;
//                    }
//                    return v;
//                }
//            }
//        }
//    }
//}