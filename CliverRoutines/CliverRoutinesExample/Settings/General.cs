using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{

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
            ConfigExample.Email(Settings.Smtp.Host, Settings.Smtp.Port, Settings.Smtp.Password, message);
        }
    }
}
