//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cliver
{
    public partial class Settings
    {
        ///// <summary>
        ///// Returns the value of a serializable field identified by its name.
        ///// </summary>
        ///// <param name="fieldName">name of serializable field</param>
        ///// <returns>The value of the serializable field</returns>
        //public object GetFieldValue(string fieldName)
        //{
        //    //!!!while FieldInfo can see property, it loses its attributes if any.
        //    FieldInfo fi = GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(a => a.Name == fieldName);
        //    return fi?.GetValue(this);
        //}
    }
}