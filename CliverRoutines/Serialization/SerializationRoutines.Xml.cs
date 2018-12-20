//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Cliver
{
    static public partial class SerializationRoutines
    {
        static public class Xml
        {
            /// <summary>
            /// Serialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="o"></param>
            /// <returns></returns>
            static public string Serialize2<T>(T o)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                serializer.Serialize(ms, o);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="xml"></param>
            /// <returns></returns>
            static public T Deserialize2<T>(string xml)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                StringReader s = new StringReader(xml);
                return (T)serializer.Deserialize(s);
            }

            /// <summary>
            /// Serialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="o"></param>
            /// <returns></returns>
            public static string Serialize<T>(T o)
            {
                var serializer = new DataContractSerializer(typeof(T));
                using (var writer = new StringWriter())
                using (var stm = new XmlTextWriter(writer))
                {
                    serializer.WriteObject(stm, o);
                    return writer.ToString();
                }
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="xml"></param>
            /// <returns></returns>
            public static T Deserialize<T>(string xml)
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

