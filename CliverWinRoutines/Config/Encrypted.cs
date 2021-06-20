//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;

namespace Cliver.Win
{
    /// <summary>
    /// A property of this type is implicitly encrypted when it is a member of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Encrypted<T> : Cliver.Encrypted<T> where T : class
    {
        static Encrypted()
        {
            InitializeDefault(new StringCrypto());
        }

        public Encrypted(T value = null) : base(value) { }
    }

    public class StringCrypto : Cliver.IStringCrypto
    {
        public StringCrypto()
        {
            crypto = new Win.Crypto.ProtectedData();
        }
        Win.Crypto.ProtectedData crypto;

        override public string Encrypt(string s)
        {
            return crypto.Encrypt(s);
        }

        override public string Decrypt(string s)
        {
            return crypto.Decrypt(s);
        }
    }
}