using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{

    class SmtpSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public string Host = "";
        public int Port = 25;
        public string _EncryptedPassword = null;

        internal string Password//it is not serialized because it is non-public
        {
            get
            {
                if (string.IsNullOrEmpty(_EncryptedPassword))
                    return null;
                try
                {
                    return crypto.Decrypt(_EncryptedPassword);
                }
                catch (Exception e)
                {
                    Log.Error("Could not decrypt the password.", e);
                    return null;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _EncryptedPassword = null;
                else
                    _EncryptedPassword = crypto.Encrypt(value);
            }
        }
        Crypto.Aes crypto = new Crypto.Aes("motherboard ID");

        protected override void Loaded()
        {
            ConfigExample.Log.Inform("Settings loaded: " + __Info.FullName);
        }

        protected override void Saved()
        {
            ConfigExample.Log.Inform("Settings saved: " + __Info.FullName);
        }
    }
}
