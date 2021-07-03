//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Cliver
{
    abstract public class Synchronization
    {
        abstract protected IEnumerable<Cliver.Settings> synchronizedSettingss { get; }

        virtual protected void onNewerSettingsFile(Settings settings)
        {
            throw new Exception("TBD: A newer settings " + settings.__Info.FullName + " have been downloaded from the remote storage. Upon closing this message they will be updated in the application.");
        }

        abstract protected Regex appSetupFileFilter { get; }// = new Regex(Program.Name + @"\.Setup\-(\d+\.\d+\.\d+)");

        abstract protected Version programVersion { get; }// = Program.Version;

        virtual protected void onNewerAppVersion(string appSetupFile)
        {
            throw new Exception("TBD: A newer app version has been downloaded: " + appSetupFile + "\r\nWould you like to install it now?");
        }

        abstract protected void ErrorHandler(Exception e);

        public class Parameters
        {
            public string SynchronizationFolder = null;
            public string DownloadFolderName = "_download";
            public string UploadFolderName = "_upload";
            public bool Synchronize = false;
            public int PollingPeriodMss = 10000;
        }

        public void Switch(Parameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parameters.SynchronizationFolder))
                    throw new Exception("SynchronizationFolder is not set.");
                if (string.IsNullOrWhiteSpace(parameters.SynchronizationFolder))
                    throw new Exception("SynchronizationFolder is not set.");
                this.parameters = parameters;
                downloadFolder = FileSystemRoutines.CreateDirectory(parameters.SynchronizationFolder + "\\" + parameters.DownloadFolderName);
                uploadFolder = FileSystemRoutines.CreateDirectory(parameters.SynchronizationFolder + "\\" + parameters.UploadFolderName);
                appSetupFolder = FileSystemRoutines.CreateDirectory(parameters.SynchronizationFolder);

                if (parameters.Synchronize && pollingThread?.IsAlive != true)
                    pollingThread = ThreadRoutines.Start(polling);
            }
            catch (Exception e)
            {
                ErrorHandler(e);
            }
        }

        Parameters parameters = null;
        string downloadFolder = null;
        string uploadFolder = null;
        string appSetupFolder = null;
        Thread pollingThread = null;

        void polling()
        {
            try
            {
                while (parameters.Synchronize)
                {
                    foreach (Settings settings in synchronizedSettingss)
                    {
                        //if (settings.__Info.File == null)//at the start it is null
                        //    return;
                        pollUploadSettingsFile(settings);
                        pollDownloadSettingsFile(settings);
                    }

                    string appSetupFile = null;
                    foreach (string file in Directory.GetFiles(appSetupFolder))
                    {
                        Match m = appSetupFileFilter?.Match(file);
                        if (m?.Success != true)
                            continue;
                        if (!Version.TryParse(m.Groups[1].Value, out Version v))
                            continue;
                        if (v > programVersion && v > lastAppVersion)
                        {
                            lastAppVersion = v;
                            appSetupFile = file;
                        }
                    }
                    if (appSetupFile != null)
                    {
                        AppNewSetupFile = appSetupFile;
                        onNewerAppVersion(appSetupFile);
                        return;
                    }

                    Thread.Sleep(parameters.PollingPeriodMss);
                }
            }
            catch (Exception e)
            {
                ErrorHandler(new Exception("Synchronization thread exited due to exception.", e));
            }
        }
        Version lastAppVersion = new Version(0, 0, 0);

        public string AppNewSetupFile { get; private set; } = null;

        void pollUploadSettingsFile(Settings settings)
        {
            try
            {
                if (!File.Exists(settings.__Info.File))
                    return;
                DateTime uploadLWT = File.GetLastWriteTime(settings.__Info.File);
                if (uploadLWT.AddSeconds(10) > DateTime.Now)//it is being written
                    return;
                string file2 = uploadFolder + "\\" + PathRoutines.GetFileName(settings.__Info.File);
                if (File.Exists(file2) && uploadLWT <= File.GetLastWriteTime(file2))
                    return;
                copy(settings.__Info.File, file2);
            }
            catch (Exception e)
            {
                ErrorHandler(e);
            }
        }

        void pollDownloadSettingsFile(Settings settings)
        {
            try
            {
                string file = downloadFolder + "\\" + PathRoutines.GetFileName(settings.__Info.File);
                if (!File.Exists(file))
                    return;
                DateTime downloadLWT = File.GetLastWriteTime(file);
                if (downloadLWT.AddSeconds(100) > DateTime.Now)//it is being written
                    return;
                if (File.Exists(settings.__Info.File) && downloadLWT <= File.GetLastWriteTime(settings.__Info.File))
                    return;
                copy(file, settings.__Info.File);
                onNewerSettingsFile(settings);
            }
            catch (Exception e)
            {
                ErrorHandler(e);
            }
        }

        static void copy(string file, string file2)
        {
            for (int i = 0; ; i++)
                try
                {
                    Log.Inform("Copying " + file + " to " + file2);
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
    }
}
