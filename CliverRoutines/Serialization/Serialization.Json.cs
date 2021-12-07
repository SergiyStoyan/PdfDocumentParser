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
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Cliver
{
    /// <summary>
    /// Serialization helpers.
    /// </summary>
    public static partial class Serialization
    {
        /// <summary>
        /// Serialization to JSON.
        /// </summary>
        public static class Json
        {
            //public enum Mode
            //{
            //    NotIndented = 1,//0001
            //    NotPolymorphic = 2, //0010
            //    IncludeNullValues = 4,//0100
            //    IncludeDefaultValues = 8//1000
            //}
            //public static string Serialize(object o, Mode mode = 0)
            //{
            //    if (o == null)
            //        return null;
            //    return JsonConvert.SerializeObject(o,
            //     mode.HasFlag(Mode.NotIndented) ? Formatting.None : Formatting.Indented,
            //       new JsonSerializerSettings
            //       {
            //           TypeNameHandling = mode.HasFlag(Mode.NotPolymorphic) ? TypeNameHandling.None : TypeNameHandling.Auto,
            //           NullValueHandling = mode.HasFlag(Mode.IncludeNullValues) ? NullValueHandling.Include : NullValueHandling.Ignore,
            //           DefaultValueHandling = mode.HasFlag(Mode.IncludeDefaultValues) ? DefaultValueHandling.Include : DefaultValueHandling.Ignore
            //       }
            //        );
            //}

            public static string Serialize(object o, bool indented = true, bool polymorphic = true, bool ignoreNullValues = true, bool ignoreDefaultValues = false)
            {
                if (o == null)
                    return null;
                return JsonConvert.SerializeObject(o,
                    indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                   new JsonSerializerSettings
                   {
                       TypeNameHandling = polymorphic ? TypeNameHandling.Auto : TypeNameHandling.None,
                       NullValueHandling = ignoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
                       DefaultValueHandling = ignoreDefaultValues ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
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

            public static void Save(string file, object o, bool indented = true, bool polymorphic = true, bool ignoreNullValues = true, bool ignoreDefaultValues = false)
            {
                FileSystemRoutines.CreateDirectory(PathRoutines.GetFileDir(file));
                File.WriteAllText(file, Serialize(o, indented, polymorphic, ignoreNullValues, ignoreDefaultValues));
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

            public static IEnumerable<string> GetMemberNames(object o)
            {
                JObject jo = (JObject)JToken.FromObject(o);
                return jo.Properties().Select(x => x.Name);
            }

            /// <summary>
            /// Usage: [Newtonsoft.Json.JsonConverter(typeof(Serialization.Json.NoIndentConverter))]
            /// !!!Issue: does not work on types
            /// </summary>
            public class NoIndentConverter : JsonConverter
            {
                public override bool CanConvert(Type objectType)
                {
                    return true;
                }

                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    //if (reader.TokenType == JsonToken.Null)
                    //    return existingValue;
                    //var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);
                    //return jObject.ToObject(objectType);
                    object o = serializer.Deserialize(reader, objectType);
                    if (o == null)
                        return existingValue;
                    return o;
                }

                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
                }
            }
        }
    }
}