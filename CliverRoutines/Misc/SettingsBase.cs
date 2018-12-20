using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Cliver
{

    //    public class Settings
    //    {
    //        static Settings()
    //        {

    //        }

    //        static readonly string Directory = Log.GetAppCommonDataDir();

    //        static T Get<T>() where T : Serializable, new()
    //        {
    //            string name = typeof(T).Name;
    //            Serializable s;
    //            if (type_names2object.TryGetValue(name, out s))
    //                return (T)s;
    //            T t = Serializable.Load<T>(Directory + "\\" + name + ".setting");
    //            type_names2object[name] = t;
    //            return t;
    //        }
    //        static Dictionary<string, Serializable> type_names2object = new Dictionary<string, Serializable>();

    //        static public void Save()
    //        {
    //            foreach (Serializable s in type_names2object.Values)
    //                s.Save();
    //        }

    //        static public void Reload()
    //        {
    //            //Serializable[] ss = new Serializable[type_names2object.Count];
    //            //type_names2object.Values.CopyTo(ss, 0);
    //            //type_names2object.Clear();
    //            //foreach (Serializable s in ss)
    //            //{
    //            //    T t = Serializable.Load<T>(s.GetFile());
    //            //    type_names2object[name] = t;
    //            //}
    //        }
    //    }
}

