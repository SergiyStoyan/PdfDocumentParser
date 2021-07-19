using System;
using System.Collections.Generic;
using System.Text;
using Cliver;
using System.Linq;

namespace Example
{
    //An example of upgrading of a Settings type 
    partial class Settings
    {
        internal static TemplatesSettings Templates;
    }

    //Example how to check the type version and migrate to the current type if needed.
    //When this attribute is set, Config checks if the version set by the attribute accords with the version saved in the storage file.
    [SettingsAttributes.TypeVersion(value: 210601, minSupportedTypeVersion: 210601)]
    class TemplatesSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public List<Template> Templates = new List<Template> { new Template { Name = "test", Words = new List<string> { "apple", "box" } } };

        //Here is your chance to upgrade the data to the current format.
        override protected UnsupportedFormatHandlerCommand UnsupportedFormatHandler(Exception deserializingException)
        {//different approaches are considered below.
            //if deserializingException==NULL then the file could be deserialized but its __TypeVersion is not acceptable.
            if (deserializingException?.Message.Contains("Could not create an instance of type Example.Template+Field") == true || __TypeVersion < 200601)
            {//remove property Fields which does not exist anymore
                Newtonsoft.Json.Linq.JObject o = __Info.ReadStorageFileAsJObject();
                for (int i = o["Templates"].Count() - 1; i >= 0; i--)
                    o["Templates"][i]["Fields"].Remove();
                o["__TypeVersion"] = 210701;
                //save
                __Info.WriteStorageFileAsJObject(o);
                return UnsupportedFormatHandlerCommand.Reload;
            }

            if (__TypeVersion < 210301)
            {
                string s = __Info.ReadStorageFileAsString();
                //edit the old data as a serialized string
                //...
                //set the current version
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(?<=\""__TypeVersion\""\:\s*)\d+", __Info.TypeVersion.Value.ToString(), System.Text.RegularExpressions.RegexOptions.Singleline);
                //save
                __Info.WriteStorageFileAsString(s);
                return UnsupportedFormatHandlerCommand.Reload;
            }

            if (deserializingException == null && __TypeVersion < __Info.TypeVersion.Value)
            {
                //alter the data in the object itself
                //...
                //save
                Save();//(!)when saving, the current type version is set
                return UnsupportedFormatHandlerCommand.Proceed;
            }

            if (deserializingException != null)
                Console.WriteLine("EXCEPTION while deserializing " + __Info.File + ": " + Log.GetExceptionMessage(deserializingException));
            else            //__TypeVersion > __Info.TypeVersion.Value     
            {
                Console.WriteLine("WARNING: The application might not support properly the newer type version " + __TypeVersion + " data stored in " + __Info.File + ".");
                Console.WriteLine("The expected version: " + __Info.TypeVersion.Value);
            }
            while (true)
            {
                Console.WriteLine("\r\nPlease choose the option:\r\nExit - [E]\r\nReset the settings - [R]\r\nProceed as is - [P]");
                ConsoleKey k = Console.ReadKey().Key;
                switch (k)
                {
                    case ConsoleKey.E:
                        Log.Exit("Settings " + __Info.FullName + " could not be loaded.");
                        break;
                    case ConsoleKey.R:
                        return UnsupportedFormatHandlerCommand.Reset;
                    case ConsoleKey.P:
                        return UnsupportedFormatHandlerCommand.Proceed;
                    default:
                        continue;
                }
            }
        }
    }

    public class Template
    {
        public string Name;
        public List<string> Words = new List<string>();
    }
}
