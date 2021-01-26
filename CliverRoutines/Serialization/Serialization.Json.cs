//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************


using System;
using Newtonsoft.Json;
using System.IO;

namespace Cliver
{
    public static partial class Serialization
    {
        public static class Json
        {
            public static string Serialize(object o, bool indented = true, bool polymorphic = true, bool ignoreNullProperties = true)
            {
                if (o == null)
                    return null;
                return JsonConvert.SerializeObject(o,
                    indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                   new JsonSerializerSettings
                   {
                       TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None,
                       NullValueHandling = ignoreNullProperties ? NullValueHandling.Ignore : NullValueHandling.Include
                   }
                    );
            }

            public static T Deserialize<T>(string json, bool polymorphic = true, bool createNewObjects = true)
            {
                return JsonConvert.DeserializeObject<T>(json,
                    new JsonSerializerSettings { TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None, ObjectCreationHandling = createNewObjects ? ObjectCreationHandling.Replace : ObjectCreationHandling.Auto }
                    );
            }

            public static object Deserialize(Type type, string json, bool polymorphic = true, bool createNewObjects = true)
            {
                return JsonConvert.DeserializeObject(json,
                    type,
                    new JsonSerializerSettings { TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None, ObjectCreationHandling = createNewObjects ? ObjectCreationHandling.Replace : ObjectCreationHandling.Auto }
                    );
            }

            public static void Save(string file, object o, bool indented = true, bool polymorphic = true, bool ignoreNullProperties = true)
            {
                FileSystemRoutines.CreateDirectory(PathRoutines.GetFileDir(file));
                File.WriteAllText(file, Serialize(o, indented, polymorphic, ignoreNullProperties));
            }

            public static T Load<T>(string file, bool polymorphic = true, bool createNewObjects = true)
            {
                return Deserialize<T>(File.ReadAllText(file), polymorphic, createNewObjects);
            }

            public static object Load(Type type, string file, bool polymorphic = true, bool createNewObjects = true)
            {
                return Deserialize(type, File.ReadAllText(file), polymorphic, createNewObjects);
            }

            public static O Clone<O>(O o)
            {
                return Deserialize<O>(Serialize(o, false, true));
            }

            public static object Clone(Type type, object o)
            {
                return Deserialize(type, Serialize(o, false, true));
            }

            public static object Clone2(object o)
            {
                return Deserialize(o.GetType(), Serialize(o, false, true));
            }

            public static bool IsEqual(object a, object b)
            {
                return Serialize(a, false, true) == Serialize(b, false, true);
            }
        }

        public static O CreateCloneByJson<O>(this O o)
        {
            return Json.Clone<O>(o);
        }

        public static object CreateCloneByJson(this object o, Type type)
        {
            return Json.Clone(type, o);
        }

        public static bool IsEqualByJson(this object a, object b)
        {
            return Json.IsEqual(a, b);
        }

        public static string ToStringByJson(this object o, bool indented = true, bool polymorphic = false, bool ignoreNullProperties = true)
        {
            return Json.Serialize(o, indented, polymorphic, ignoreNullProperties);
        }
    }
}