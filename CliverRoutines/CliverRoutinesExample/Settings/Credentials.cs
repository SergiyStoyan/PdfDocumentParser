using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    class CredentialsSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
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
