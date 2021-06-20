using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    //Example how to check type version and migrate to a newer type
    [SettingsTypeAttribute.TypeVersion(value: 210601, minSupportedTypeVersion: 210601)]
    class TemplateSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public Dictionary<string, Template> Names2Template = new Dictionary<string, Template>();

        //Here is your chance to upgrade the data to the current format.
        override protected UnsupportedTypeVersionHandlerResult UnsupportedTypeVersionHandler()
        {
            //Some approaches follow below.
            if (__TypeVersion < 200301)
            {
                Newtonsoft.Json.Linq.JObject o = GetJObjectFromStorageFile();
                //edit the old data as a JObject
                //...
                //save
                //set the current version
                o["__TypeVersion"] = 210601;
                //save
                System.IO.File.WriteAllText(__Info.File, o.ToString());
                return UnsupportedTypeVersionHandlerResult.NotSave_Reload;
            }
            if (__TypeVersion < 200901)
            {
                string s = System.IO.File.ReadAllText(__Info.File);
                //edit the old data as a serialized string
                //...
                //set the current version
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(?<=\""__TypeVersion\""\:\s*)\d+", "210601", System.Text.RegularExpressions.RegexOptions.Singleline);
                //save
                System.IO.File.WriteAllText(__Info.File, s);
                return UnsupportedTypeVersionHandlerResult.NotSave_Reload;
            }
            if (__TypeVersion < __Info.TypeVersion.Value)
            {
                //alter the old data directly in the object
                //...
                return UnsupportedTypeVersionHandlerResult.Save_Reload;
            }
            //__TypeVersion > __Info.TypeVersion.Value
                throw new Exception("This application version does not support the newer type version which is stored in " + __Info.File + "\r\nConsider to update your application version.");
        }
    }

    public class Template
    {
        public string Name;
        public List<string> Words = new List<string>();
    }
}
