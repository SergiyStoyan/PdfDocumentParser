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
    static public class SerializationRoutines
    {
        static public class Json
        {
            static public string Get(object o)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(o);
            }

            static public T Get<T>(string json)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
            }
        }

        static public class Xml
        {
            static public string Get2<T>(T o)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                serializer.Serialize(ms, o);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }

            static public T Get2<T>(string xml)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                StringReader s = new StringReader(xml);
                return (T)serializer.Deserialize(s);
            }

            public static string Get<T>(T o)
            {
                var serializer = new DataContractSerializer(typeof(T));
                using (var writer = new StringWriter())
                using (var stm = new XmlTextWriter(writer))
                {
                    serializer.WriteObject(stm, o);
                    return writer.ToString();
                }
            }

            public static T Get<T>(string xml)
            {
                var serializer = new DataContractSerializer(typeof(T));
                using (var reader = new StringReader(xml))
                using (var stm = new XmlTextReader(reader))
                {
                    return (T)serializer.ReadObject(stm);
                }
            }
        }
    }
}

