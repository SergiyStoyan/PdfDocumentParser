using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    partial class Settings
    {
        //Settings type field/property can be declared anywhere in the code. It must be static to be processed by Config.
        //It can be declared readonly which is optional because the logic of the app may require replacing the value (!!!this is not supported by some versions of .NET)
        [SettingsFieldAttribute.Indented(false)]//the data is serialized without indention
        internal static GeneralSettings General;
    }

    class GeneralSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public Dictionary<string, User> Users = new Dictionary<string, User>();

        protected override void Loaded()
        {
            ConfigExample.Log.Inform0("Settings loaded: " + __Info.FullName);
        }

        protected override void Saving()
        {
            ConfigExample.Log.Inform0("Settings saving...: " + __Info.FullName);
        }
    }

    public class User
    {
        public string Name;//serialazable
        public string Email;//serialazable
        public bool Active = true;//serialazable

        public void Notify(string message)
        {
            ConfigExample.Message(Settings.Server.Host, Settings.Server.Port, Settings.Server.Password?.Value, message);
        }
    }
}
