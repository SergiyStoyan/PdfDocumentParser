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
using System.Runtime.Caching;


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
            public DateTime _DownloadedTemplatesFileLastWriteTime;
            public DateTime _UploadedTemplatesFileLastWriteTime;

            public override void Loaded()
            {
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
                        if (_memCache == null)
                            _memCache = MemoryCache.Default;

                        string downloadFolder = FileSystemRoutines.CreateDirectory(SynchronizedFolder + "\\download");
                        if (downloadFileSystemWatcher != null)
                        {
                            if (downloadFileSystemWatcher.Path == downloadFolder)
                                return;
                            downloadFileSystemWatcher.Dispose();
                        }
                        downloadFileSystemWatcher = new FileSystemWatcher()
                        {
                            Path = downloadFolder,
                            NotifyFilter = NotifyFilters.LastWrite,
                            Filter = "*.*",
                        };
                        CacheEntryRemovedCallback downloadFileModificationCompleted = delegate (CacheEntryRemovedArguments args)
                        {
                            try
                            {
                                string file = args.CacheItem.Key;
                                //if (!PathRoutines.ArePathsEqual(e.FullPath, Settings.Templates.__File))
                                //    return;
                                if (!File.Exists(file))//directory
                                    return;
                                string file2 = PathRoutines.GetPathMirroredInDir(file, downloadFileSystemWatcher.Path, Config.StorageDir);
                                for (int i = 0; ; i++)
                                    try
                                    {
                                        FileSystemRoutines.CopyFile(file, file2, true);
                                        if (file2 == Settings.Templates.__File)
                                        {
                                            Message.Inform("A newer templates have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
                                            MainForm.This.LoadTemplates();
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
                            catch (Exception ex)
                            {
                                LogMessage.Error(ex);
                            }
                        };
                        FileSystemEventHandler downloadfileSystemEventHandler = delegate (object sender, FileSystemEventArgs e)
                        {
                            _memCache.AddOrGetExisting(e.FullPath, e, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(100), RemovedCallback = downloadFileModificationCompleted });
                        };
                        //downloadFileSystemWatcher.Created += downloadfileSystemEventHandler;
                        downloadFileSystemWatcher.Changed += downloadfileSystemEventHandler;
                        downloadFileSystemWatcher.EnableRaisingEvents = true;



                        if (uploadFileSystemWatcher != null)
                            return;
                        uploadFileSystemWatcher = new FileSystemWatcher()
                        {
                            Path = Config.StorageDir,
                            NotifyFilter = NotifyFilters.LastWrite,
                            Filter = "*.*",
                            IncludeSubdirectories = false,
                        };
                        string synchronizedUploadFolder = FileSystemRoutines.CreateDirectory(SynchronizedFolder + "\\upload");
                        CacheEntryRemovedCallback uploadFileModificationCompleted = delegate (CacheEntryRemovedArguments args)
                        {
                            try
                            {
                                string file = args.CacheItem.Key;
                                //if (!PathRoutines.ArePathsEqual(e.FullPath, Settings.Templates.__File))
                                //    return;
                                if (!File.Exists(file))//directory
                                    return;
                                string file2 = PathRoutines.GetPathMirroredInDir(file, Config.StorageDir, synchronizedUploadFolder);
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
                            catch (Exception ex)
                            {
                                LogMessage.Error(ex);
                            }
                        };
                        FileSystemEventHandler uploadfileSystemEventHandler = delegate (object sender, FileSystemEventArgs e)
                        {
                            _memCache.AddOrGetExisting(e.FullPath, e, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(10), RemovedCallback = uploadFileModificationCompleted });
                        };
                        //uploadFileSystemWatcher.Created += uploadfileSystemEventHandler;
                        uploadFileSystemWatcher.Changed += uploadfileSystemEventHandler;
                        uploadFileSystemWatcher.EnableRaisingEvents = true;
                    }
                    else
                    {
                        if (downloadFileSystemWatcher != null)
                        {
                            downloadFileSystemWatcher.Dispose();
                            downloadFileSystemWatcher = null;
                        }
                        if (uploadFileSystemWatcher != null)
                        {
                            uploadFileSystemWatcher.Dispose();
                            uploadFileSystemWatcher = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    Message.Error(e);
                }
            }
            FileSystemWatcher downloadFileSystemWatcher = null;
            FileSystemWatcher uploadFileSystemWatcher = null;
            MemoryCache _memCache;

            ~SynchronizationSettings()
            {
                if (downloadFileSystemWatcher != null)
                    downloadFileSystemWatcher.Dispose();
                if (uploadFileSystemWatcher != null)
                    uploadFileSystemWatcher.Dispose();
                if (_memCache != null)
                    _memCache.Dispose();
            }
        }
    }
}