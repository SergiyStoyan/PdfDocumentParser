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
using System.Runtime.Serialization.Formatters.Binary;

namespace Cliver
{
    static public partial class SerializationRoutines
    {
        static public class Binary
        {
            /// <summary>
            /// Serialize object
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            static public byte[] Serialize(object o)
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(ms, o);
                return ms.GetBuffer();
            }

            static public T Deserialize<T>(byte[] bytes)
            {
                MemoryStream ms = new MemoryStream(bytes);
                BinaryFormatter f = new BinaryFormatter();
                return (T)f.Deserialize(ms);
            }

            static public object Deserialize(byte[] bytes)
            {
                MemoryStream ms = new MemoryStream(bytes);
                BinaryFormatter f = new BinaryFormatter();
                return f.Deserialize(ms);
            }
        }
    }
}

