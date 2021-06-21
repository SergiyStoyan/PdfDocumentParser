using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //Example how to check the type version and migrate to the current type if needed.
    //When this attribute is set, Config engine checks if the version set by the attribute corresponds to the version in the storage file.
    [SettingsTypeAttribute.TypeVersion(value: 210601, minSupportedTypeVersion: 210601)]
    class TemplatesSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public Dictionary<string, Template> Names2Template = new Dictionary<string, Template>();

        //Here is your chance to upgrade the data to the current format.
        override protected UnsupportedTypeVersionHandlerCommand UnsupportedTypeVersionHandler()
        {
            //some approaches are considered below:
            if (__TypeVersion < 200301)
            {
                Newtonsoft.Json.Linq.JObject o = GetJObjectFromStorageFile();
                //edit the old data as a JObject
                //...
                //set the current version
                o["__TypeVersion"] = __Info.TypeVersion.Value;
                //save
                System.IO.File.WriteAllText(__Info.File, o.ToString());
                return UnsupportedTypeVersionHandlerCommand.Reload;
            }
            if (__TypeVersion < 200901)
            {
                string s = System.IO.File.ReadAllText(__Info.File);
                //edit the old data as a serialized string
                //...
                //set the current version
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(?<=\""__TypeVersion\""\:\s*)\d+", __Info.TypeVersion.Value.ToString(), System.Text.RegularExpressions.RegexOptions.Singleline);
                //save
                System.IO.File.WriteAllText(__Info.File, s);
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
            Console.WriteLine("The expected version is " + __Info.TypeVersion.Value);
            Console.WriteLine("Consider to update the application.");
            return UnsupportedTypeVersionHandlerCommand.Proceed;
        }
    }

    public class Template
    {
        public string Name;
        public List<string> Words = new List<string>();
    }
}
