//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    /// <summary>
    /// (!)Deprecated. Replaced with Endec class.
    /// </summary>
    public abstract class Crypto
    {
        abstract public byte[] Encrypt(byte[] bytes);
        abstract public byte[] Decrypt(byte[] bytes);

        public virtual string Encrypt2String(string s)
        {
            return Encrypt2String(Encoding.Unicode.GetBytes(s));
        }

        public virtual string Decrypt2String(string s)
        {
            return Encoding.Unicode.GetString(Decrypt2Bytes(s));
        }

        public virtual string Encrypt2String(byte[] bytes)
        {
            return Convert.ToBase64String(Encrypt(bytes));
        }

        public virtual byte[] Decrypt2Bytes(string s)
        {
            s = s.Replace(" ", "+");
            return Decrypt(Convert.FromBase64String(s));
        }

        public virtual string Encrypt2String<T>(T o)
        {
            return Encrypt2String(Serialization.Json.Serialize(o, false, true));
        }

        public virtual T Decrypt2Object<T>(string s)
        {
            return Serialization.Json.Deserialize<T>(Decrypt2String(s));
        }

        public class Rijndael : Crypto
        {
            //readonly string key;
            readonly byte[] vector = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

            public Rijndael(string key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, vector);
                // rijndael.Padding = PaddingMode.Zeros;
                rijndael.Key = pdb.GetBytes(32);
                rijndael.IV = pdb.GetBytes(16);
            }
            System.Security.Cryptography.Rijndael rijndael = System.Security.Cryptography.Rijndael.Create();

            override public byte[] Encrypt(byte[] bytes)
            {
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    //cs.FlushFinalBlock();
                    cs.Close();
                    return ms.ToArray();
                }
            }

            override public byte[] Decrypt(byte[] bytes)
            {
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, rijndael.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    //cs.FlushFinalBlock();
                    cs.Close();
                    return ms.ToArray();
                }
            }
        }

        public class Aes : Crypto
        {
            readonly string key = null;

            public Aes(string key)
            {
                this.key = key ?? throw new ArgumentNullException("key");
            }

            public override byte[] Encrypt(byte[] bytes)
            {
                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, 8);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.Close();
                        }
                        return ms.ToArray();
                    }
                }
            }

            public override byte[] Decrypt(byte[] bytes)
            {
                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, 8);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.Close();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        public class Halojoy
        {
            readonly string key;

            public Halojoy(string key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                this.key = Regex.Replace(key, @"\s+", "", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (key.Length < 3)
                    throw new Exception("Key is too short.");
            }

            public string Encrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(convert_by_halojoy(str)));
            }

            public string Decrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                return convert_by_halojoy(Encoding.UTF8.GetString(Convert.FromBase64String(str)));
            }

            /// <summary>
            /// EnCrypt to/from DeCrypt
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            string convert_by_halojoy(string str)
            {
                int kl = key.Length;
                if (kl > 32)
                    kl = 32;

                int[] k = new int[kl];

                for (int i = 0; i < kl; i++)
                    k[i] = (int)(key[i]) & 0x1F;

                int j = 0;
                StringBuilder ss = new StringBuilder(str);
                for (int i = 0; i < str.Length; i++)
                {
                    int e = (int)str[i];
                    ss[i] = (e & 0xE0) != 0 ? (char)(e ^ k[j]) : (char)e;
                    if (++j == kl)
                        j = 0;
                }
                return ss.ToString();
            }
        }
    }
}