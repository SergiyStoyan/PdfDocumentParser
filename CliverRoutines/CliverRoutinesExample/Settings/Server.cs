using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{

    class ServerSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        static ServerSettings()
        {
            //if you are not on Windows and cannot use CliverWinRoutines, you have to initialize IStringCrypto with a key
            Cliver.Encrypted<string>.InitializeDefault(new Cliver.IStringCrypto.Rijndael("123"));
        }

        public string Host = "";
        public int Port = 123;
        public Cliver.Encrypted<string> Password = new Encrypted<string>();
        //Windows alternative provided by CliverWinRoutines:
        //public Cliver.Win.Encrypted<string> Password = new Cliver.Win.Encrypted<string>();  

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
