//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Cliver
{
    public class Crypto
    {
        public class Rijndael
        {
            //readonly string key;
            readonly byte[] vector = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

            public Rijndael(string key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, vector);
                rijndael.Key = pdb.GetBytes(32);
                rijndael.IV = pdb.GetBytes(16);
            }
            System.Security.Cryptography.Rijndael rijndael = System.Security.Cryptography.Rijndael.Create();

            public string Encrypt(string str)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(str);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.Close();
                bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }

            public string Decrypt(string str)
            {
                byte[] bytes = Convert.FromBase64String(str);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.Close();
                bytes = ms.ToArray();
                return Encoding.Unicode.GetString(bytes);
            }
        }

        public class Aes
        {
            readonly string key = null;

            public Aes(string key)
            {
                this.key = key ?? throw new ArgumentNullException("key");
            }

            public string Encrypt(string str)
            {
                using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, 8);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] bytes = Encoding.Unicode.GetBytes(str);
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytes, 0, bytes.Length);
                            cs.Close();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            public string Decrypt(string str)
            {
                str = str.Replace(" ", "+");
                byte[] bytes = Convert.FromBase64String(str);
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
                        return Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
        }

        public class ProtectedData
        {
            readonly byte[] key = null;

            public ProtectedData(byte[] key)
            {
                this.key = key;
            }

            public ProtectedData(string key)
            {
                if (key != null)
                    this.key = Encoding.UTF8.GetBytes(key);
            }

            public string Encrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                var data = Encoding.Unicode.GetBytes(str);
                byte[] encrypted = System.Security.Cryptography.ProtectedData.Protect(data, key, DataProtectionScope.LocalMachine);
                return Convert.ToBase64String(encrypted);
            }

            public string Decrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                byte[] data = Convert.FromBase64String(str);
                byte[] decrypted = System.Security.Cryptography.ProtectedData.Unprotect(data, key, DataProtectionScope.LocalMachine);
                return Encoding.Unicode.GetString(decrypted);
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
            /// EnCrypt <-> DeCrypt
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