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
using System.Threading.Tasks;
using System.Data.Linq;
using System.Linq;


namespace Cliver.InvoiceParser
{
    public partial class Settings
    {
        [Cliver.Settings.Obligatory]
        public static readonly GeneralSettings General;

        public class GeneralSettings : Cliver.Settings
        {
            public string InputFolder = @"d:\_d\_projects\PdfDocumentParser\_test_files";
            public string OutputFolder;
            public bool IgnoreHidddenFiles = true;
            public bool ReadInputFolderRecursively = false;

            public System.Drawing.Color StampColor = System.Drawing.Color.Red;

            public string SynchronizedFolder = Config.StorageDir + "/_synchronised";
            public bool Synchronize = true;
            public string SynchronizedUploadedFolder { get; private set; }

            public List<string> OrderedOutputFieldNames = new List<string>();

            public override void Loaded()
            {
                if (string.IsNullOrWhiteSpace(InputFolder))
                    InputFolder = ProgramRoutines.GetAppDirectory();
                if (string.IsNullOrWhiteSpace(OutputFolder))
                    OutputFolder = InputFolder + "\\Output";// + DateTime.Now.ToString("yyMMddHHmmss"); 

                switchSynchronization();
            }

            public override void Saving()
            {
                switchSynchronization();
            }

            void switchSynchronization()
            {
                try
                {
                    if (Synchronize && !string.IsNullOrWhiteSpace(SynchronizedFolder))
                    {
                        SynchronizedUploadedFolder = FileSystemRoutines.CreateDirectory(SynchronizedFolder + "\\uploaded");

                        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher()
                        {
                            Path = FileSystemRoutines.CreateDirectory(SynchronizedFolder + "\\downloaded"),
                            NotifyFilter = NotifyFilters.LastWrite,
                            Filter = "*.*",
                            EnableRaisingEvents = true,
                        };
                        FileSystemEventHandler fileSystemEventHandler = delegate (object sender, FileSystemEventArgs e)
                        {
                            try
                            {
                                if (File.Exists(e.FullPath))
                                {
                                    string file2 = PathRoutines.GetPathMirroredInDir(e.FullPath, fileSystemWatcher.Path, Config.StorageDir);
                                    File.Copy(e.FullPath, file2, true);
                                    if (file2 == Settings.Templates.__File)
                                    {
                                        Message.Inform("A newer templates have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
                                        MainForm.This.LoadTemplates();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Message.Error(ex);
                            }
                        };
                        fileSystemWatcher.Created += fileSystemEventHandler;
                        fileSystemWatcher.Changed += fileSystemEventHandler;
                    }
                    else
                    {
                        if (fileSystemWatcher != null)
                        {
                            fileSystemWatcher.Dispose();
                            fileSystemWatcher = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    Message.Error(e);
                }
            }
            FileSystemWatcher fileSystemWatcher = null;
        }
    }
}