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
    /// A property of this type is implicitly encrypted when it is a memeber of a Settings class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Encrypted<T> : Cliver.Encrypted<T> where T : class
    {
        static Encrypted()
        {
            crypto = new Crypto();
        }

        class Crypto : ICrypto
        {
            public Crypto()
            {
                crypto = new Win.Crypto.ProtectedData();
            }
            Win.Crypto.ProtectedData crypto;

            public string Encrypt(string s)
            {
                return crypto.Encrypt(s);
            }

            public string Decrypt(string s)
            {
                return crypto.Decrypt(s);
            }
        }
    }
}