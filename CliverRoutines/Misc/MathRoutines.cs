//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Threading;
using System.Collections.Generic;


namespace Cliver
{
    public static class MathRoutines
    {
        public static T Min<T>(params T[] values) where T : IComparable
        {
            if (values.Length < 1)
                throw new Exception("Nothing to compare");
            T v = values[0];
            for (int i = 1; i < values.Length; i++)
                if (v.CompareTo(values[i]) > 0)
                    v = values[i];
            return v;
        }

        public static T Max<T>(params T[] values) where T : IComparable
        {
            if (values.Length < 1)
                throw new Exception("Nothing to compare");
            T v = values[0];
            for (int i = 1; i < values.Length; i++)
                if (v.CompareTo(values[i]) < 0)
                    v = values[i];
            return v;
        }

        //public static T GetExtremum<T>(bool maxElseMin, params T[] values) where T : IComparable
        //{
        //    if (values.Length < 1)
        //        throw new Exception("Nothing to compare");
        //    T v = values[0];
        //    for (int i = 1; i < values.Length; i++)
        //        if (v.CompareTo(values[i]) < 0)
        //            v = values[i];
        //    return v;
        //}

        //public static T Ceiling<T>(T value) 
        //{           
        //    return v;
        //}

        public static T Truncate<T>(T value, T maximum) where T : IComparable
        {
            if (value.CompareTo(maximum) > 0)
                return maximum;
            return value;
        }
    }
}

