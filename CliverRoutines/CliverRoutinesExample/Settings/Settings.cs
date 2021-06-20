using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    class Settings
    {
        /// <summary>
        ///Settings type field can be declared anywhere in the code. It must be static to be processed by Config.
        ///It can be declared readonly which is optional because the logic of the app may require replacing the value (!!!this is not supported by some versions of .NET)
        /// </summary>
        internal static GeneralSettings General;



        [SettingsFieldAttribute.Storage(indented: false)]//the data is serialized without indention
        public static ServerSettings Server { get; set; }




        //a Crypto instance needed by SettingsFieldAttribute.Crypto attribute
        public static IStringCrypto.Rijndael Crypto { get; } = new IStringCrypto.Rijndael("123");
        //an alternative by CliverWinRoutines 
        //public static Win.StringCrypto Crypto { get; } = new Win.StringCrypto();   

        //specify the class and the property that expose your IStringCrypto instance which is used for encryption
        [SettingsFieldAttribute.Crypto(iStringCryptoGetterHostingType: typeof(Settings), iStringCryptoGetterName: nameof(Crypto))]
        //this data is serialized as encrypted string
        internal static CredentialsSettings Credentials;
    }
}
