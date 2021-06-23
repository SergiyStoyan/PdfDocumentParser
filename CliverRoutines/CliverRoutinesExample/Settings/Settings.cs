using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    class Settings
    {
        /// <summary>
        ///Settings type field/property can be declared anywhere in the code. It must be static to be processed by Config.
        ///It can be declared readonly which is optional because the logic of the app may require replacing the value (!!!this is not supported by some versions of .NET)
        /// </summary>
        [SettingsFieldAttribute.Indented(false)]//the data is serialized without indention
        internal static GeneralSettings General;



        public static ServerSettings Server { get; set; }




        //a StringEndec object passed into SettingsFieldAttribute.EncryptedAttribute
        public static StringEndec.Rijndael Endec { get; } = new StringEndec.Rijndael("123");
        //an alternative by CliverWinRoutines which does not require a key:
        //public static Cliver.Win.StringEndec Endec { get; } = new Win.StringEndec();   

        //specify the class and the property that expose a StringEndec object which is to be used for encryption
        [SettingsAttribute.Encrypted(stringEndecGetterHostingType: typeof(Settings), stringEndecGetterName: nameof(Endec))]
        //Settings field to be serialized as encrypted string
        internal static CredentialsSettings Credentials;




        internal static TemplatesSettings Templates;
    }
}
