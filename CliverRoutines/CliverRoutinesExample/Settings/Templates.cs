using System;
using System.Collections.Generic;
using System.Text;
using Cliver;
using System.Linq;
using System.Text.RegularExpressions;

namespace Example
{
    //An example of upgrading a Settings type 
    partial class Settings
    {
        internal static TemplatesSettings Templates;
    }

    public class Template
    {
        public string Name;
        public List<string> Words = new List<string>();
    }

    //Example how to check the type version and migrate to the current type if needed.
    //When this attribute is set, Config checks if the version set by the attribute accords with the version saved in the storage file.
    [SettingsAttributes.TypeVersion(210701)]
    class TemplatesSettings : Cliver.UserSettings//UserSettings based class is serialized in the user directory
    {
        public List<Template> Templates = new List<Template> { new Template { Name = "test", Words = new List<string> { "apple", "box" } } };

        //Here is your chance to upgrade the data to the current format.
        override protected void UnsupportedFormatHandler(Exception deserializingException)
        {
            try
            {
                if (deserializingException != null)
                    throw deserializingException;

                //the object was deserialized but its __TypeVersion is not acceptable
                //successive upgrading from version to version using different approaches:
                if (__TypeVersion < 200601)
                {//editing the data as JSON object
                    //remove property Field which does not exist anymore
                    Newtonsoft.Json.Linq.JObject o = __Info.ReadStorageFileAsJObject();
                    for (int i = o["Templates"].Count() - 1; i >= 0; i--)
                        o["Templates"][i]["Field"]?.Remove();
                    //set the respective version
                    o["__TypeVersion"] = 200601;
                    //save
                    __Info.WriteStorageFileAsJObject(o);
                    Reload();//UnsupportedFormatHandler() will be called again because __TypeVersion is still obsolete
                    return;
                }
                if (__TypeVersion < 210301)
                {//editing the data as string
                    string s = __Info.ReadStorageFileAsString();
                    //edit the old data as a serialized string. It is the most low-level option of data altering.
                    //...
                    //set the respective version                
                    __Info.UpdateTypeVersionInStorageFileString(210301, ref s);
                    //save
                    __Info.WriteStorageFileAsString(s);
                    Reload();//UnsupportedFormatHandler() will be called again because __TypeVersion is still obsolete
                    return;
                }
                if (__TypeVersion < __Info.TypeVersion)
                {//altering this object itself
                    foreach (Template t in Templates)
                        t.Name = Regex.Replace(t.Name, @"^test", "_TEST_");
                    //...
                    //save
                    Save();//(!)when saving, the current type version is set
                    return;
                }

                //the type version in the file is newer than supported by this method
                throw new Exception("Unsupported version of " + GetType().FullName + ": " + __TypeVersion + ". Accepted version: " + __Info.TypeVersion, deserializingException);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Console.WriteLine("Error while loading " + __Info.File);
                Console.WriteLine("The application will exit now. If you still want to run it, remove the file or, to preserve it, move it to a safe location."
                    + "The file's data will be reset by this application to their default state.");
                Console.WriteLine("Press a key...");
                Console.ReadKey();
                Log.Exit("Settings " + __Info.FullName + " could not be loaded.");
            }
        }
    }
}
