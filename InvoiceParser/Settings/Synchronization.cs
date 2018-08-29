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
            public string SynchronizedFolder = null;
            public bool Synchronize = false;
            public HashSet<string> SynchronizeFileNames = null;

            public override void Loaded()
            {
                if (SynchronizeFileNames == null)
                {
                    Config.PreloadField("Templates");
                    SynchronizeFileNames = new HashSet<string> { PathRoutines.GetFileNameFromPath(Settings.Templates.__File) };
                }
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
                    if (synchronization.Synchronize && !string.IsNullOrWhiteSpace(synchronization.SynchronizedFolder))
                    {
                        if (pollingThread != null && pollingThread.IsAlive)
                            return;
                        pollingThread = ThreadRoutines.StartTry(()=> { polling(synchronization); });
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
            static void polling(SynchronizationSettings synchronization)
            {
                while (synchronization.Synchronize)
                {
                    string downloadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizedFolder + "\\download");
                    string uploadFolder = FileSystemRoutines.CreateDirectory(synchronization.SynchronizedFolder + "\\upload");
                    foreach (string fn in synchronization.SynchronizeFileNames)
                    {
                        try
                        {
                            string file = Config.StorageDir + "\\" + fn;
                            if (File.Exists(file))
                            {
                                DateTime uploadLWT = File.GetLastWriteTime(file);
                                if (uploadLWT.AddSeconds(10) < DateTime.Now)//it is not being written
                                {
                                    string file2 = uploadFolder + "\\" + fn;
                                    if (!File.Exists(file2) || uploadLWT > File.GetLastWriteTime(file2))
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
                            }
                        }
                        catch (Exception e)
                        {
                            LogMessage.Error(e);
                        }
                        try
                        {
                            string file = downloadFolder + "\\" + fn;
                            if (File.Exists(file))
                            {
                                DateTime downloadLWT = File.GetLastWriteTime(file);
                                if (downloadLWT.AddSeconds(100) < DateTime.Now)//it is not being written
                                {
                                    string file2 = Config.StorageDir + "\\" + fn;
                                    if (!File.Exists(file2) || downloadLWT > File.GetLastWriteTime(file2))
                                    {
                                        for (int i = 0; ; i++)
                                            try
                                            {
                                                FileSystemRoutines.CopyFile(file, file2, true);
                                                if (file2 == Settings.Templates.__File)
                                                {
                                                    Message.Inform("A newer templates have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
                                                    MainForm.This.BeginInvoke(() => { MainForm.This.LoadTemplates(); });
                                                }
                                                break;
                                            }
                                            catch (IOException ex)//no access while locked by sycnhronizing app
                                            {
                                                if (i >= 100)
                                                    throw new Exception("Could not copy file '" + file + "' to '" + file2 + "'", ex);
                                                Thread.Sleep(1000);
                                            }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogMessage.Error(e);
                        }
                    }
                    Thread.Sleep(100000);
                }
            }
            static Thread pollingThread = null;
        }
    }
}