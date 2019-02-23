//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

//#define UseNetJsonSerialization //for legacy apps

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

            public static T Deserialize<T>(string json, bool polymorphic = true)
            {
                return JsonConvert.DeserializeObject<T>(json,
                    new JsonSerializerSettings { TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None }
                    );
            }

            public static object Deserialize(Type type, string json, bool polymorphic = true)
            {
                return JsonConvert.DeserializeObject(json,
                    type,
                    new JsonSerializerSettings { TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None }
                    );
            }

            public static void Save(string file, object o, bool polymorphic = true, bool indented = true)
            {
                FileSystemRoutines.CreateDirectory(PathRoutines.GetDirFromPath(file));
                File.WriteAllText(file, Serialize(o, indented, polymorphic));
            }

            public static T Load<T>(string file, bool polymorphic = true)
            {
                return Deserialize<T>(File.ReadAllText(file), polymorphic);
            }

            public static object Load(Type type, string file, bool polymorphic = true)
            {
                return Deserialize(type, File.ReadAllText(file), polymorphic);
            }

            public static T Clone<T>(T o)
            {
                return Deserialize<T>(Serialize(o, false, true));
            }

            public static object Clone(Type type, object o)
            {
                return Deserialize(type, Serialize(o, false, true));
            }
        }
    }
}