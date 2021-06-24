﻿using System;
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
        static ServerSettings()
        {
            //if you are not on Windows and cannot use CliverWinRoutines, you have to initialize Encrypted<> explicitly with a key
            Cliver.Encrypted<string>.InitializeDefault(new Cliver.StringEndec.Rijndael("123"));
        }

        public string Host = "";
        public int Port = 123;
        //This field is encrypted. It decrypts its value only when explicitly called.
        public Cliver.Encrypted<string> Password = new Encrypted<string>();
        //Windows alternative provided by CliverWinRoutines:
        //public Cliver.Win.Encrypted<string> Password = new Cliver.Win.Encrypted<string>();  
    }
}
