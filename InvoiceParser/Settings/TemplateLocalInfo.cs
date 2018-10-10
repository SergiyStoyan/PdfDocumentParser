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
                GetInfo(templateName).LastTestFile = lastTestFile;
            }

            public void SetUsedTime(string templateName)
            {
                GetInfo(templateName).UsedTime = DateTime.Now;
            }

            public Info GetInfo(string templateName)
            {
                //if (templateName == null)
                //    return null;
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
            }

            public override void Saving()
            {
            }
        }
    }
}