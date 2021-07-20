using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //An example of encrypting a Settings type/field
    partial class Settings
    {
        //Alternatively, the Encrypted attribute can be added directly to the field: 
        //[SettingsAttributes.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Settings.Endec))]
        //This object will be serialized as encrypted string
        internal static CredentialsSettings Credentials;

        //A StringEndec object passed into SettingsFieldAttribute.EncryptedAttribute
        internal static StringEndec.Rijndael Endec { get; } = new StringEndec.Rijndael("111");
        //An alternative by CliverWinRoutines which does not require a key:
        //internal static Cliver.Win.StringEndec Endec { get; } = new Cliver.Win.StringEndec();   
    }

    //This attribute can be applied to either a Settings type or a Settings field. Being applied to a type, it causes any field of the type to be encrypted.
    //Specify the class and the property that expose a StringEndec object to be used for encryption.
    [SettingsAttributes.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Settings.Endec))]
    class CredentialsSettings : Cliver.UserSettings
    {
        public string Key = "test";
        public string Token = "123";
    }
}

