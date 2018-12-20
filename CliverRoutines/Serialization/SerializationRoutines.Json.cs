//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

//#define UseNetJsonSerialization //for legacy apps

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
#if UseNetJsonSerialization
using System.Web.Script.Serialization;
#else
using Newtonsoft.Json;
#endif
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Cliver
{
    static public partial class SerializationRoutines
    {
        static public class Json
        {
            static public string Serialize(object o, bool indented = true, bool polymorphic = true)
            {
                if (o == null)
                    return null;
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(o);
#else
                return JsonConvert.SerializeObject(o,
                    indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                    polymorphic ? jsonSerializerSettings : null);
#endif
            }

            static public T Deserialize<T>(string json, bool polymorphic = true)
            {
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
#else
                return JsonConvert.DeserializeObject<T>(json,
                    polymorphic ? jsonSerializerSettings : null);
#endif
            }

            static public object Deserialize(Type type, string json, bool polymorphic = true)
            {
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize(json, type);
#else
                return JsonConvert.DeserializeObject(json, 
                    type,
                    polymorphic ? jsonSerializerSettings : null);
#endif
            }

            static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

            static public void Save(string file, object o, bool polymorphic = true, bool indented = true)
            {
                FileSystemRoutines.CreateDirectory(PathRoutines.GetDirFromPath(file));
                File.WriteAllText(file, Serialize(o, indented, polymorphic));
            }

            static public T Load<T>(string file, bool polymorphic = true)
            {
                return Deserialize<T>(File.ReadAllText(file), polymorphic);
            }

            static public object Load(Type type, string file, bool polymorphic = true)
            {
                return Deserialize(type, File.ReadAllText(file), polymorphic);
            }

            static public T Clone<T>(T o)
            {
                return Deserialize<T>(Serialize(o, false, true));
            }
        }
    }
}

