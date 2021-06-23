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
        [SettingsFieldAttribute.NotIndented]//the data is serialized without indention
        internal static GeneralSettings General;



        public static ServerSettings Server { get; set; }




        //a Crypto object used by SettingsFieldAttribute.Crypto attribute
        public static StringCrypto.Rijndael Crypto { get; } = new StringCrypto.Rijndael("123");
        //an alternative by CliverWinRoutines which does not require a key:
        //public static Cliver.Win.StringCrypto Crypto { get; } = new Win.StringCrypto();   

        //specify the class and the property that expose a StringCrypto object which will be used for encryption
        [SettingsFieldAttribute.Crypto(stringCryptoGetterHostingType: typeof(Settings), stringCryptoGetterName: nameof(Crypto))]
        //Settings field to be serialized as encrypted string
        internal static CredentialsSettings Credentials;




        internal static TemplatesSettings Templates;
    }
}
