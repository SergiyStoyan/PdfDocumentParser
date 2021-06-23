using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    partial class Settings
    {
        //This field will be serialized as an encrypted string because of 
        internal static CredentialsSettings Credentials;

        //A StringEndec object passed into SettingsFieldAttribute.EncryptedAttribute
        public static StringEndec.Rijndael Endec { get; } = new StringEndec.Rijndael("123");
        //An alternative by CliverWinRoutines which does not require a key:
        //public static Cliver.Win.StringEndec Endec { get; } = new Cliver.Win.StringEndec();   
    }

    //Specify the class and the property that expose a StringEndec object which is to be used for encryption.
    //This attribute can be applied to either a Settings type or a Settings field.
    [SettingsAttribute.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Settings.Endec))]
    class CredentialsSettings : Cliver.UserSettings//This type will be serialized as an encrypted string
    {
        public string Key = "test";
        public string Token = "123";

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

