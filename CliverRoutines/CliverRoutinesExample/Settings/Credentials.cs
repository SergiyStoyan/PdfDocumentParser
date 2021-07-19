using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //An example of encryption of a Settings type/field
    partial class Settings
    {
        //Specify the class and the property that expose a StringEndec object to be used for encryption.
        //This attribute can be applied to either a Settings type or a Settings field.
        [SettingsAttributes.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Settings.Endec))]
        //This object will be serialized as encrypted string
        internal static CredentialsSettings Credentials;

        //A StringEndec object passed into SettingsFieldAttribute.EncryptedAttribute
        public static StringEndec.Rijndael Endec { get; } = new StringEndec.Rijndael("111");
        //An alternative by CliverWinRoutines which does not require a key:
        //public static Cliver.Win.StringEndec Endec { get; } = new Cliver.Win.StringEndec();   
    }

    //Alternatively, the Encrypted attribute can be added to the type which will cause any field of this type to be serialized as encrypted string
    //[SettingsAttributes.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Settings.Endec))]
    class CredentialsSettings : Cliver.UserSettings
    {
        public string Key = "test";
        public string Token = "123";
    }
}

