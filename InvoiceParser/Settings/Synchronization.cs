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
        public static readonly SynchronizationSettings Synchronization;

        public class SynchronizationSettings : Cliver.Settings
        {
            public string SynchronizationFolder = null;
            public bool Synchronize = false;
            Regex SynchronizeFileFilter = new Regex(@"Template2s|TemplateLocalInfo");

            public override void Loaded()
            {
                switchSynchronization(this);
            }

            public override void Saving()
            {
                switchSynchronization(this);
            }

            static void switchSynchronization(SynchronizationSettings synchronization)
            {
                try
                {
                    synchronize = synchronization.Synchronize;
                    synchronizeFileFilter = synchronization.SynchronizeFileFilter;
                    downloadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizationFolder + "\\_download");
                    uploadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizationFolder + "\\_upload");

                    if (synchronization.Synchronize && !string.IsNullOrWhiteSpace(synchronization.SynchronizationFolder))
                    {
                        if (pollingThread != null && pollingThread.IsAlive)
                            return;
                        pollingThread = ThreadRoutines.StartTry(polling);
                    }
                    else
                    {
                    }
                }
                catch (Exception e)
                {
                    Message.Error(e);
                }
            }
            static void polling()
            {
                while (synchronize)
                {
                    foreach (string file in Directory.GetFiles(Config.StorageDir))
                    {
                        if (synchronizeFileFilter == null || !synchronizeFileFilter.IsMatch(file))
                            continue;
                        pollUploadFile(file);
                    }
                    foreach (string file in Directory.GetFiles(downloadFolder))
                    {
                        if (synchronizeFileFilter == null || !synchronizeFileFilter.IsMatch(file))
                            continue;
                        pollDownloadFile(file);
                    }
                    Thread.Sleep(10000);
                }
            }
            static void pollUploadFile(string file)
            {
                try
                {
                    DateTime uploadLWT = File.GetLastWriteTime(file);
                    if (uploadLWT.AddSeconds(10) > DateTime.Now)//it is being written
                        return;
                    string file2 = PathRoutines.GetPathMirroredInDir(file, Config.StorageDir, uploadFolder);
                    if (File.Exists(file2) && uploadLWT <= File.GetLastWriteTime(file2))
                        return;
                    copy(file, file2);
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
            static void copy(string file, string file2)
            {
                for (int i = 0; ; i++)
                    try
                    {
                        FileSystemRoutines.CopyFile(file, file2, true);
                        return;
                    }
                    catch (IOException ex)//no access while locked by sycnhronizing app
                    {
                        if (i >= 100)
                            throw new Exception("Could not copy file '" + file + "' to '" + file2 + "'", ex);
                        Thread.Sleep(1000);
                    }
            }
            static void pollDownloadFile(string file)
            {
                try
                {
                    DateTime downloadLWT = File.GetLastWriteTime(file);
                    if (downloadLWT.AddSeconds(100) > DateTime.Now)//it is being written
                        return;
                    string file2 = PathRoutines.GetPathMirroredInDir(file, downloadFolder, Config.StorageDir);
                    if (downloadLWT <= File.GetLastWriteTime(file2))
                        return;
                    copy(file, file2);
                    if (Settings.Template2s == null || Settings.Template2s.__File == null)//at the start it is null
                        return;
                    if (PathRoutines.ArePathsEqual(file2, Settings.Template2s.__File))
                    {
                        Message.Inform("A newer templates have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
                        Settings.Template2s.Reload();
                        MainForm.This.BeginInvoke(() => { MainForm.This.LoadTemplates(); });
                    }
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
            static Thread pollingThread = null;
            static bool synchronize = false;
            static Regex synchronizeFileFilter = null;
            static string downloadFolder = null;
            static string uploadFolder = null;

            public static void SynchronizeUploadFile(string file)
            {
                try
                {
                    copy(file, PathRoutines.GetPathMirroredInDir(file, Config.StorageDir, uploadFolder));
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }
    }
}