//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using Cliver.PdfDocumentParser;
using System.Linq;

namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly TemplateLocalInfoSettings TemplateLocalInfo;

        public class TemplateLocalInfoSettings : Cliver.Settings
        {
            public Dictionary<string, Info> TemplateNames2Info;

            public class Info
            {
                public string LastTestFile;
                public DateTime UsedTime;

                public string GetUsedTimeAsString()
                {
                    return UsedTime.ToString("yy-MM-dd HH:mm:ss");
                }
            }

            public void SetLastTestFile(string templateName, string lastTestFile)
            {
                Info i = GetInfo(templateName, true);
                if (i.LastTestFile == lastTestFile)
                    return;
                i.LastTestFile = lastTestFile;
            }

            public void SetUsedTime(string templateName)
            {
                GetInfo(templateName, true).UsedTime = DateTime.Now;
            }

            public Info GetInfo(string templateName, bool createIfNotExists)
            {
                if (templateName == null)
                    return null;
                Info tai;
                if (!TemplateNames2Info.TryGetValue(templateName, out tai))
                {
                    tai = new Info();
                    TemplateNames2Info[templateName] = tai;
                }
                return tai;
            }

            public void Clear_Save()
            {
                var deletedTNs = TemplateNames2Info.Keys.Where(n => Template2s.Template2s.Where(a => a.Template.Name == n).FirstOrDefault() == null).ToList();
                foreach (string n in deletedTNs)
                    TemplateNames2Info.Remove(n);
                Save();
            }

            public override void Loaded()
            {
                if (TemplateNames2Info == null)
                    TemplateNames2Info = new Dictionary<string, Info>();

                //!!!TO BE REMOVED AFTER UPGRADE
                if (TemplateNames2Info.Count < 1)
                {
                    if (Settings.TestFiles == null)
                        Config.ReloadField("TestFiles");
                    foreach (string tm in Settings.TestFiles.TemplateNames2TestFile.Keys)
                        TemplateNames2Info[tm] = new Info
                        {
                            LastTestFile = Settings.TestFiles.TemplateNames2TestFile[tm],
                            UsedTime = DateTime.MinValue,
                        };
                    Save();
                    Reload();
                    if (TemplateNames2Info.Count > 0)
                        if (System.IO.File.Exists(Settings.TestFiles.__File))
                            System.IO.File.Delete(Settings.TestFiles.__File);
                }
            }

            public override void Saving()
            {
            }
        }
    }
}