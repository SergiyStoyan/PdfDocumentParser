using System;
using System.Threading;
using Cliver;

namespace Example
{
    class ConfigExample
    {
        static public Log.Session Log = Cliver.Log.Session.Get("ConfigExample");

        public static void Run()
        {
            //mandatory; initialize settings
            Config.Reload();

            //modify
            Settings.Smtp.Port = 10;
            //save on disk
            Settings.Smtp.Save(); //settings are saved here:     Settings.Smtp.__Info.File
            //or, decline changes
            Settings.Smtp.Reload();
            //or, reset to initial values
            Settings.Smtp.Reset();

            editSmtpInDialog();

            User user = new User { Name = "Tom2", Email = "tom@company.com" };
            user.Notify("test");
            Settings.General.Users[user.Name] = user;

            Config.Save();
        }

        // bogus editor
        static void editSmtpInDialog()
        {
            try
            {
                SmtpSettings smtp2 = Config.CreateReloadedClone(Settings.Smtp);

                //expose smtp2 in an editing dialog and get new values
                smtp2.Host = "smtp.server.com";
                smtp2.Port = 29;

                if (!isValid(smtp2))
                    return;

                Settings.Smtp = smtp2;
                Settings.Smtp.Save();   //settings are saved here:     Settings.Smtp.__Info.File
            }
            catch (Exception e)
            {
                Log.Error2(e);
            }
        }

        // bogus validator
        static bool isValid(SmtpSettings smtp)
        {
            return true;
        }
        
        // bogus mailer
        public static void Email(string host, int port, string password, string message)
        {
            Log.Inform("sent message:\r\n" + message);
        }
    }
}
