//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Cliver
{
    public partial class Settings
    {
        internal static Settings Create(Config. SettingsField settingsField, bool reset, bool throwExceptionIfCouldNotLoadFromStorageFile)
        {
            Settings settings;
            if (!reset && File.Exists(settingsField.File))
                try
                {
                    settings = (Settings)Cliver.Serialization.Json.Load(settingsField.Type, settingsField.File);
                }
                catch (Exception e)
                {
                    if (throwExceptionIfCouldNotLoadFromStorageFile)
                        throw new Exception("Error while loading settings " + settingsField.FullName + " from file " + settingsField.File, e);
                }
            if (File.Exists(settingsField.InitFile))
            {
                FileSystemRoutines.CopyFile(settingsField.InitFile, settingsField.File, true);
                settings = (Settings)Cliver.Serialization.Json.Load(settingsField.Type, settingsField.InitFile);
            }
            settings = (Settings)Activator.CreateInstance(settingsField.Type);
            settings.Field = settingsField;

            settings.Loaded();

            return settings;
        }

        public enum InitMode
        {
            LOAD,
            LOAD_OR_NEW,
            NEW
        }

        public void Save()
        {
            lock (this)
            {
                Saving();
                Cliver.Serialization.Json.Save(__File, this, __Indented, true);
                Saved();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public bool __Indented = true;

        virtual public void Loaded()
        {

        }

        virtual public void Saving()
        {

        }

        virtual public void Saved()
        {

        }
    }
}