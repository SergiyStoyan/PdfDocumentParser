using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //An example of upgrading of a Settings type 
    partial class Settings
    {
        internal static TemplatesSettings Templates;
    }

    //Example how to check the type version and migrate to the current type if needed.
    //When this attribute is set, Config checks if the version set by the attribute accords with the version saved in the storage file.
    [SettingsTypeAttribute.TypeVersion(value: 210601, minSupportedTypeVersion: 210601)]
    class TemplatesSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public List<Template> Templates = new List<Template> { new Template { Name = "test", Words = new List<string> { "apple", "box" } } };

        //Here is your chance to upgrade the data to the current format.
        override protected UnsupportedTypeVersionHandlerCommand UnsupportedTypeVersionHandler()
        {
            //different approaches are considered below:
            if (__TypeVersion < 200301)
            {
                Newtonsoft.Json.Linq.JObject o = __Info.ReadStorageFileAsJObject();
                //edit the old data as a JObject
                //...
                //set the current version
                o["__TypeVersion"] = __Info.TypeVersion.Value;
                //save
                __Info.WriteStorageFileAsJObject(o);
                return UnsupportedTypeVersionHandlerCommand.Reload;
            }
            if (__TypeVersion < 200901)
            {
                string s = __Info.ReadStorageFileAsString();
                //edit the old data as a serialized string
                //...
                //set the current version
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(?<=\""__TypeVersion\""\:\s*)\d+", __Info.TypeVersion.Value.ToString(), System.Text.RegularExpressions.RegexOptions.Singleline);
                //save
                __Info.WriteStorageFileAsString(s);
                return UnsupportedTypeVersionHandlerCommand.Reload;
            }
            if (__TypeVersion < __Info.TypeVersion.Value)
            {
                //alter the data in the object itself
                //...
                //save
                Save();//(!)when saving, the current type version is set
                return UnsupportedTypeVersionHandlerCommand.Proceed;
            }
            //__TypeVersion > __Info.TypeVersion.Value
            Console.WriteLine("WARNING: The application might not support properly the newer type version " + __TypeVersion + " data stored in " + __Info.File + ".");
            Console.WriteLine("The expected version: " + __Info.TypeVersion.Value);
            while (true)
            {
                Console.WriteLine("\r\nProceed? [Y/N]");
                ConsoleKey k = Console.ReadKey().Key;
                if (k == ConsoleKey.Y)
                    return UnsupportedTypeVersionHandlerCommand.Proceed;
                if (k == ConsoleKey.N)
                    Log.Exit("Too new type version " + __TypeVersion + " detected in " + __Info.File + ".");
            }
        }
    }

    public class Template
    {
        public string Name;
        public List<string> Words = new List<string>();
    }
}
