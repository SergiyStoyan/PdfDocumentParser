//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
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
    public abstract class Endec
    {
        abstract public byte[] Encrypt(byte[] bytes);
        abstract public byte[] Decrypt(byte[] bytes);

        public class Rijndael : Endec
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

        public class Aes : Endec
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
    }

    public class Endec2String
    {
        public Endec2String(Endec endec)
        {
            this.endec = endec;
        }
        protected readonly Endec endec;

        virtual public string Encrypt<T>(T o) where T : class
        {
            if (o == null)
                return null;

            byte[] bytes;
            if (typeof(T) == typeof(byte[]))
                bytes = o as byte[];
            else
            {
                string s;
                if (typeof(T) == typeof(string))
                    s = o as string;
                else
                    s = Serialization.Json.Serialize(o, false, true);
                bytes = Encoding.Unicode.GetBytes(s);
            }
            return Convert.ToBase64String(endec.Encrypt(bytes));
        }

        virtual public T Decrypt<T>(string s) where T : class
        {
            if (s == null)
                return null;

            s = s.Replace(" ", "+");
            byte[] bytes = Convert.FromBase64String(s);
            bytes = endec.Decrypt(bytes);
            if (typeof(T) == typeof(byte[]))
                return bytes as T;
            if (typeof(T) == typeof(string))
                return Encoding.Unicode.GetString(bytes) as T;
            return Serialization.Json.Deserialize<T>(s);
        }

        public class Rijndael : Endec2String
        {
            public Rijndael(string key) : base(new Endec.Rijndael(key))
            {
            }
        }
    }

    public class Endec2String<T> : Endec2String where T : class
    {
        public Endec2String(Endec endec) : base(endec) { }

        virtual public string Encrypt(T o)
        {
            return Encrypt<T>(o);
        }

        virtual public T Decrypt(string s)
        {
            return Decrypt<T>(s);
        }

        public class Rijndael : Endec2String<T>
        {
            public Rijndael(string key) : base(new Endec.Rijndael(key))
            {
            }
        }
    }

    //public class EndecBytes2String : Endec2String
    //{
    //    public EndecBytes2String(Endec endec) : base(endec)
    //    { }

    //    public string Encrypt(byte[] bytes)
    //    {
    //        return Convert.ToBase64String(endec.Encrypt(bytes));
    //    }

    //    public byte[] Decrypt(string s)
    //    {
    //        s = s.Replace(" ", "+");
    //        return endec.Decrypt(Convert.FromBase64String(s));
    //    }
    //}

    //public class EndecString2String : Endec2String
    //{
    //    public EndecString2String(Endec endec) : base(endec)
    //    { }

    //    public string Encrypt(string s)
    //    {
    //        byte[] bytes = Encoding.Unicode.GetBytes(s);
    //        return Convert.ToBase64String(endec.Encrypt(bytes));
    //    }

    //    public string Decrypt(string s)
    //    {
    //        s = s.Replace(" ", "+");
    //        byte[] bytes = endec.Decrypt(Convert.FromBase64String(s));
    //        return Encoding.Unicode.GetString(bytes);
    //    }
    //}

    //public class EndecObject2String<T>: Endec2String where T : class
    //{
    //    public EndecObject2String(Endec endec) : base(endec)
    //    { }

    //    public string Encrypt(T o)
    //    {
    //        string s = Serialization.Json.Serialize(o, false, true);
    //        byte[] bytes = Encoding.Unicode.GetBytes(s);
    //        return Convert.ToBase64String(endec.Encrypt(bytes));
    //    }

    //    public T Decrypt(string s)
    //    {
    //        s = s.Replace(" ", "+");
    //        byte[] bytes = endec.Decrypt(Convert.FromBase64String(s));
    //        s = Encoding.Unicode.GetString(bytes);
    //        return Serialization.Json.Deserialize<T>(s);
    //    }
    //}

}