/********************************************************************************************
        Author: Sergiy Stoyan
        systoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
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
    abstract public class Endec : Cliver.Endec
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

        public class ProtectedData : Endec
        {
            //static public DataProtectionScope DefaultDataProtectionScope = DataProtectionScope.CurrentUser;

            public ProtectedData(byte[] key, DataProtectionScope dataProtectionScope = DataProtectionScope.CurrentUser)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                this.key = key;
                this.dataProtectionScope = dataProtectionScope;
            }
            readonly byte[] key = null;
            readonly DataProtectionScope dataProtectionScope;

            public ProtectedData(string key, DataProtectionScope dataProtectionScope = DataProtectionScope.CurrentUser) : this(Encoding.UTF8.GetBytes(key), dataProtectionScope)
            {
            }

            /// <summary>
            /// Initialize cryption with the given computer's system info.
            /// </summary>
            public ProtectedData()
            {
                key = Encoding.UTF8.GetBytes(GetKeyFromComputerSystemInfo());
            }

            override public byte[] Encrypt(byte[] bytes)
            {
                if (bytes == null)
                    throw new ArgumentNullException("bytes");
                return System.Security.Cryptography.ProtectedData.Protect(bytes, key, dataProtectionScope);
            }

            override public byte[] Decrypt(byte[] bytes)
            {
                if (bytes == null)
                    throw new ArgumentNullException("bytes");
                return System.Security.Cryptography.ProtectedData.Unprotect(bytes, key, dataProtectionScope);
            }
        }
    }
}