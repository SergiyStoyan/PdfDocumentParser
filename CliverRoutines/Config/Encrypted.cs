//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;
using Newtonsoft.Json;

namespace Cliver
{
    /// <summary>
    /// A field/property of this type is implicitly encrypted when it is a member of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[JsonConverter(typeof(EncryptedConverter))]
    public class Encrypted<T> /*: EncryptedBase*/ where T : class
    {
        public Encrypted()
        {
        }

        public Encrypted(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Encypted value. It must not be called from the custom code.
        /// </summary>
        [JsonProperty]//forces serialization for private 
        string _Value { get; set; } = null;

        /// <summary>
        /// Decrypted value to be used in the custom code.
        /// </summary>
        [JsonIgnore]
        public T Value
        {
            get
            {
                if (_Value == null)
                    return null;
                try
                {
                    string s = endec.Decrypt(_Value);
                    if (typeof(T) == typeof(string))
                        return s as T;
                    return Serialization.Json.Deserialize<T>(s);
                }
                catch (Exception e)
                {
                    Log.Error("Could not decrypt _Value.", e);
                    return null;
                }
            }
            set
            {
                if (value == null)
                    _Value = null;
                else
                {
                    if (typeof(T) == typeof(string))
                        _Value = endec.Encrypt(value as string);
                    else
                        _Value = endec.Encrypt(Serialization.Json.Serialize(value));
                }
            }
        }

        public void Initialize(StringEndec endec)
        {
            if (endec != null)
                throw new Exception("StringEndec instance is already set and cannot be re-set.");
            _endec = endec;
        }
        StringEndec endec
        {
            get
            {
                StringEndec c = _endec != null ? _endec : defaultEndec;
                if (c == null)
                    throw new Exception("StringEndec instance is not set. It can be done by either Initialize() or InitializeDefault() of Cliver.Encrypted class.");
                return c;
            }
        }
        StringEndec _endec;

        static public void InitializeDefault(StringEndec endec)
        {
            if (defaultEndec != null)
                throw new Exception("Default StringEndec instance is already set and cannot be re-set.");
            defaultEndec = endec;
        }
        static StringEndec defaultEndec;
    }
    /*public abstract class EncryptedBase
    {
        internal string _Value { get; set; } = null;
    }
    public class EncryptedConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            EncryptedBase e = (EncryptedBase)value;
            writer.WriteValue(e._Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object e = existingValue != null ? existingValue : Activator.CreateInstance(objectType);
            ((EncryptedBase)e)._Value = (string)reader.Value;
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Encrypted<>);
        }
    }*/

    /// <summary>
    /// Abstract string encrypting/decrypting class
    /// </summary>
    public abstract class StringEndec
    {
        public abstract string Encrypt(string s);
        public abstract string Decrypt(string s);

        public class Rijndael : StringEndec
        {
            public Rijndael(string key)
            {
                endec = new Cliver.Crypto.Rijndael(key);
            }
            Crypto.Rijndael endec;

            override public string Encrypt(string s)
            {
                return endec.Encrypt(s);
            }

            override public string Decrypt(string s)
            {
                return endec.Decrypt(s);
            }
        }
    }
}