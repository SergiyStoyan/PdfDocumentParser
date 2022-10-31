//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using Newtonsoft.Json;
using System.Text;

namespace Cliver
{
    /// <summary>
    /// A field/property of this type is implicitly encrypted when it is a member of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Encrypted<T> where T : class
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
                    return endec.Decrypt<T>(_Value);
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
                    _Value = endec.Encrypt(Value);
            }
        }

        public void Initialize(Endec2String endec)
        {
            if (endec != null)
                throw new Exception("Endec instance is already set and cannot be re-set.");
            _endec = endec;
        }
        Endec2String endec
        {
            get
            {
                if (_endec != null)
                    return _endec;
                if (defaultEndec != null)
                {
                    _endec = defaultEndec;
                    return _endec;
                }
                throw new Exception("Endec instance is not set. It can be done by either Initialize() or InitializeDefault() of Cliver.Encrypted class.");
            }
        }
        Endec2String _endec;

        static public void InitializeDefault(Endec2String endec)
        {
            if (defaultEndec != null)
                throw new Exception("Default Endec instance is already set and cannot be re-set.");
            defaultEndec = endec;
        }
        static Endec2String defaultEndec;
    }

    ///// <summary>
    ///// (!)Only intended for use in Encrypted<T>.
    ///// Provides a simplified syntax where Endec2String class is the general way to go.
    ///// </summary>
    //public abstract class ObjectEndec : Endec2String
    //{
    //    protected ObjectEndec(Endec endec) : base(endec)
    //    {
    //    }

    //    public class Rijndael : ObjectEndec
    //    {
    //        public Rijndael(string key) : base(new Endec.Rijndael(key))
    //        {
    //        }
    //    }
    //}

    /// <summary>
    /// (!)Deprecated. Exists for backward compatibility. Only intended for use in Settings.EncryptedAttribute.
    /// </summary>
    public abstract class StringEndec : Endec2String/*<string>*/
    {
        protected StringEndec(Endec endec) : base(endec)
        {
        }

        public class Rijndael : StringEndec
        {
            public Rijndael(string key) : base(new Endec.Rijndael(key))
            {
            }
        }
    }
}