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
using System.Linq;

namespace Cliver.Win
{
    public class Crypto
    {
        public class ProtectedData
        {
            public static DataProtectionScope DataProtectionScope = DataProtectionScope.CurrentUser;
            readonly byte[] key = null;

            public ProtectedData(byte[] key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                this.key = key;
            }

            public ProtectedData(string key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                this.key = Encoding.UTF8.GetBytes(key);
            }

            /// <summary>
            /// Initialize cryption with the given computer's system info.
            /// </summary>
            public ProtectedData()
            {
                string key = SystemInfo.GetMotherboardIds().FirstOrDefault();
                if (key == null)
                {
                    var pi = SystemInfo.GetProcessorInfos().FirstOrDefault();
                    key = pi != null ? pi.Id : throw new Exception("Could not create the default key");
                }

                this.key = Encoding.UTF8.GetBytes(key);
            }

            public string Encrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                var data = Encoding.Unicode.GetBytes(str);
                byte[] encrypted = System.Security.Cryptography.ProtectedData.Protect(data, key, DataProtectionScope);
                return Convert.ToBase64String(encrypted);
            }

            public string Decrypt(string str)
            {
                if (str == null)
                    throw new ArgumentNullException("str");
                byte[] data = Convert.FromBase64String(str);
                byte[] decrypted = System.Security.Cryptography.ProtectedData.Unprotect(data, key, DataProtectionScope);
                return Encoding.Unicode.GetString(decrypted);
            }
        }
    }
}