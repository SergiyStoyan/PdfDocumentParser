/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace Cliver.Win
{
    /// <summary>
    /// (!)Deprecated. Replaced with Endec class.
    /// </summary>
    public class Crypto
    {
        public static string GetKeyFromComputerSystemInfo()
        {
            string key = SystemInfo.GetMotherboardIds().FirstOrDefault();
            if (key != null)
                return key;
            var pi = SystemInfo.GetProcessorInfos().FirstOrDefault();
            if (pi != null)
                return pi.Id;
            key = SystemInfo.GetMachineGuid();
            if (key != null)
                return key;
            key = SystemInfo.GetMACs().FirstOrDefault();
            if (key != null)
                return key;
            throw new Exception("Could not create the default key");
        }

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
                key = Encoding.UTF8.GetBytes(GetKeyFromComputerSystemInfo());
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