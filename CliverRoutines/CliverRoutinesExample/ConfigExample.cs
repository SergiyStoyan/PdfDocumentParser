﻿using System;
using System.Threading;
using Cliver;

namespace Example
{
    class ConfigExample
    {
        static public Log.Session Log = Cliver.Log.Session.Get("ConfigExample");

        public static void Run()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            
            //(!)mandatory; initialize settings
            Config.Reload();

            //modify
            Settings.Server.Host = "1.1.1.1";
            Settings.Server.Port = 10;
            //save on disk
            Settings.Server.Save();
            Log.Inform("The settings are saved to: " + Settings.Server.__Info.File);
            //or, restore the previously saved values
            Settings.Server.Reload();
            //or, reset to initial values
            Settings.Server.Reset();

            editServerByDialog();

            User user = new User { Name = "Tom3", Email = "tom@company.com" };
            user.Notify("test");
            Settings.General.Users[user.Name] = user;

            Log.Inform("Settings.Credentials.Key: " + Settings.Credentials.Key);
            Settings.Credentials.Key = "test2";
            Log.Inform("Settings.Credentials.Key: " + Settings.Credentials.Key);

            Log.Inform("Settings.Templates.__TypeVersion: " + Settings.Templates.__TypeVersion);

            Config.Save();

            watch.Stop();
        }

        // bogus editor
        static void editServerByDialog()
        {
            try
            {
                ServerSettings server2 = Config.CreateReloadedClone(Settings.Server);

                //expose server2 in an editing dialog and get new values
                server2.Host = "ftp.server.com";
                server2.Port = 30;
                if (server2.Password == null)
                    server2.Password = new Encrypted<string>();
                server2.Password.Value = "test";

                if (!isValid(server2))
                    return;

                Settings.Server = server2;
                Settings.Server.Save();
                Log.Inform("The settings are saved to: " + Settings.Server.__Info.File);
            }
            catch (Exception e)
            {
                Log.Error2(e);
            }
        }

        // bogus validator
        static bool isValid(ServerSettings server)
        {
            return true;
        }

        // bogus messenger
        public static void Message(string host, int port, string password, string message)
        {
            Log.Inform("sent message:\r\n" + message);
        }
    }
}
