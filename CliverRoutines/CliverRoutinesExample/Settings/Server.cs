using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //An example of encypting of a serializable field
    partial class Settings
    {
        public static ServerSettings Server { get; set; }
    }

    class ServerSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        //optional static initialization
        static ServerSettings()
        {
            //if you are not on Windows and cannot use CliverWinRoutines, you have to initialize Encrypted<T> explicitly with a key
            //which can be done by the static way.
            //(!)Otheriwse Endec must be set directly in the instance before the first calling Value.
            Cliver.Encrypted<string>.InitializeDefault(new Cliver.StringEndec.Rijndael("123"));//recommended way
            //Alternatives:
            //Cliver.Encrypted<string>.InitializeDefault(new Cliver.StringEndec(new Endec.Rijndael("123")));//general way
        }

        public string Host = "";
        public int Port = 123;
        //This field is encrypted. It decrypts its value only when it is explicitly called.
        public Cliver.Encrypted<string> Password = new Cliver.Encrypted<string>("test");
        //Windows alternative provided by CliverWinRoutines that does not require a key:
        //public Cliver.Encrypted<string> Password = new Cliver.Win.Encrypted<string>("test");
    }
}
