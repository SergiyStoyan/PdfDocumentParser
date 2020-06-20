using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    /// <summary>
    /// A UserSettings based class which is serialized into the user specific directory
    /// </summary>
    class SmtpSettings : Cliver.UserSettings
    {
        public string SmtpHost = "";
        public int SmtpPort = 25;
        public string SmtpEncryptedPassword = null;

        /// <summary>
        /// 
        /// </summary>
        internal string SmtpPassword
        {
            get
            {
                if (string.IsNullOrEmpty(SmtpEncryptedPassword))
                    return null;
                try
                {
                    return crypto.Decrypt(SmtpEncryptedPassword);
                }
                catch
                {
                    Cliver.Log.Main.Error("Could not decrypt the password.");
                    return null;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    SmtpEncryptedPassword = null;
                else
                    SmtpEncryptedPassword = crypto.Encrypt(value);
            }
        }
        Cliver.Crypto.Aes crypto = new Cliver.Crypto.Aes("motherboard ID");

        public override void Loaded()
        {
            Cliver.Log.Main.Inform("Settings loaded.");
        }

        public override void Saved()
        {
            Cliver.Log.Main.Inform("Settings saved.");
        }
    }
    
    class Settings
    {
        /// <summary>
        ///Settings type field can be declared anywhere in the code. It must be public and static to be processed by Config.
        ///Also, it can be declared readonly which is optional because sometimes the logic of the app may require replacing the value.
        /// </summary>
        public static SmtpSettings Smtp;
    }
}
