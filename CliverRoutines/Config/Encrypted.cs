//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver
{
    /// <summary>
    /// A property of this type is implicitly encrypted when it is a memeber of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Encrypted<T> where T : class
    {
        /// <summary>
        /// Encypted value used while serializing. 
        /// It must not be called from the custom code.
        /// </summary>
        public string _Value { get; set; } = null;

        /// <summary>
        /// Decrypted value which is to be used by the custom code.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public T Value
        {
            get
            {
                if (_Value == null)
                    return null;
                try
                {
                    string s = crypto.Decrypt(_Value);
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
                        _Value = crypto.Encrypt(value as string);
                    else
                        _Value = crypto.Encrypt(Serialization.Json.Serialize(value));
                }
            }
        }

        public void Initialize(StringCrypto crypto)
        {
            if (crypto != null)
                throw new Exception("Crypto engine is already initialized and cannot be re-set.");
            _crypto = crypto;
        }
        StringCrypto crypto
        {
            get { return _crypto != null ? _crypto : defaultCrypto;}
        }
        StringCrypto _crypto;

        static public void InitializeDefault(StringCrypto crypto)
        {
            if (defaultCrypto != null)
                throw new Exception("Default Crypto engine is already initialized and cannot be re-set.");
            defaultCrypto = crypto;
        }
        static StringCrypto defaultCrypto;
    }

    ///// <summary>
    ///// A property of this type is implicitly encrypted when it is a memeber of a Settings class.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class EncryptedByRijndael<T> : Encrypted<T> where T : class
    //{
    //    static public void Initialize(string key)
    //    {
    //        Initialize(new RijndaelCrypto(key));
    //    }

    //    class RijndaelCrypto : IStringCrypto
    //    {
    //        public RijndaelCrypto(string key)
    //        {
    //            crypto = new Cliver.Crypto.Rijndael(key);
    //        }
    //        Crypto.Rijndael crypto;

    //        public string Encrypt(string s)
    //        {
    //            return crypto.Encrypt(s);
    //        }

    //        public string Decrypt(string s)
    //        {
    //            return crypto.Decrypt(s);
    //        }
    //    }
    //}
}