//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;

namespace Cliver
{
    public static class ReflectionRoutines
    {
        static public bool IsSubclassOfRawGeneric(this Type type, Type generic_type)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic_type == cur)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}

