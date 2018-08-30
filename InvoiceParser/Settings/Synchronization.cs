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

            public override void Loaded()
            {
                //if (SynchronizedFileNames == null)
                //{
                //    Config.PreloadField("Templates");
                //    SynchronizedFileNames = new HashSet<string> { PathRoutines.GetFileNameFromPath(Settings.Templates.__File) };
                //}
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
                    //synchronizedFileNames = synchronization.SynchronizedFileNames;
                    downloadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizationFolder + "\\download");
                    uploadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizationFolder + "\\upload");

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
                        pollUploadFile(file);
                        pollDownloadFile(file);
                    }
                    Thread.Sleep(100000);
                }
            }
            static void pollUploadFile(string file)
            {
                try
                {
                    DateTime uploadLWT = File.GetLastWriteTime(file);
                    if (uploadLWT.AddSeconds(10) > DateTime.Now)//it is not being written
                        return;
                    string file2 = uploadFolder + "\\" + PathRoutines.GetFileNameFromPath(file);
                    if (File.Exists(file2) && uploadLWT <= File.GetLastWriteTime(file2))
                        return;
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
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
            static void pollDownloadFile(string file2)
            {
                try
                {
                    string file = downloadFolder + "\\" + PathRoutines.GetFileNameFromPath(file2);
                    if (!File.Exists(file))
                        return;
                    DateTime downloadLWT = File.GetLastWriteTime(file);
                    if (downloadLWT.AddSeconds(100) > DateTime.Now)//it is not being written
                        return;
                    if (downloadLWT <= File.GetLastWriteTime(file2))
                        return;
                    for (int i = 0; ; i++)
                        try
                        {
                            FileSystemRoutines.CopyFile(file, file2, true);
                            if (file2 == Settings.Templates.__File)
                            {
                                Message.Inform("A newer templates have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
                                Settings.Templates.Reload();
                                MainForm.This.BeginInvoke(() => { MainForm.This.LoadTemplates(); });
                            }
                            return;
                        }
                        catch (IOException ex)//no access while locked by sycnhronizing app
                        {
                            if (i >= 100)
                                throw new Exception("Could not copy file '" + file + "' to '" + file2 + "'", ex);
                            Thread.Sleep(1000);
                        }
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
            static Thread pollingThread = null;
            static bool synchronize = false;
            static string downloadFolder = null;
            static string uploadFolder = null;

            public static void SynchronizeUploadFile(string file)
            {
                try
                {
                    string file2 = uploadFolder + "\\" + PathRoutines.GetFileNameFromPath(file);
                    for (int i = 0; ; i++)
                        try
                        {
                            FileSystemRoutines.CopyFile(file, file2, true);
                            break;
                        }
                        catch (IOException ex)//no access while locked by sycnhronizing app
                        {
                            if (i >= 100)
                                throw new Exception("Could not copy file '" + file + "' to '" + file2 + "'", ex);
                            Thread.Sleep(1000);
                        }
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }
    }
}