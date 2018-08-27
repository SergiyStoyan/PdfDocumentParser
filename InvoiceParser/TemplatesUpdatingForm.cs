using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Dropbox.Api;
using Dropbox.Api.Common;
using Dropbox.Api.Files;
using Dropbox.Api.Team;

namespace Cliver.InvoiceParser
{
    public partial class TemplatesUpdatingForm : Form
    {
        public TemplatesUpdatingForm()
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName;
        }

        static TemplatesUpdatingForm startShowDialog(Form owner, Action cancel)
        {
            TemplatesUpdatingForm pf = new TemplatesUpdatingForm();
            owner.BeginInvoke(() =>
            {
                pf.ShowDialog(owner);
            });
            return pf;
        }
        Action cancel = null;

        void showProgress(int p)
        {
            this.Invoke(() =>
            {
                progressBar.Value = p;
            });
        }

        void stopShowDialog()
        {
            this.Invoke(() =>
            {
                Close();
            });
        }

        private void abort_Click(object sender, EventArgs e)
        {
            //if (!Message.YesNo("The template collection is being updated. Would you like to abort the operation?"))
            //    return;
            cancel?.Invoke();
            stopShowDialog();
        }

        //static private async Task<ListFolderResult> ListFolder(DropboxClient client, string path)
        //{
        //    Console.WriteLine("--- Files ---");
        //    var list = await client.Files.ListFolderAsync(path);

        //    // show folders then files
        //    foreach (var item in list.Entries.Where(i => i.IsFolder))
        //    {
        //        Console.WriteLine("D  {0}/", item.Name);
        //    }

        //    foreach (var item in list.Entries.Where(i => i.IsFile))
        //    {
        //        var file = item.AsFile;

        //        Console.WriteLine("F{0,8} {1}",
        //            file.Size,
        //            item.Name);
        //    }

        //    if (list.HasMore)
        //    {
        //        Console.WriteLine("   ...");
        //    }
        //    return list;
        //}

        //static private async Task RunUserTests()
        //{
        //    DropboxClient client = new DropboxClient(Settings.General.RemoteAccessToken);
        //    //await GetCurrentAccount(client);

        //    //var path = "/home/Apps/PdfDocumentParser";
        //    var path = "";
        //    var list = await ListFolder(client, path);

        //    var firstFile = list.Entries.FirstOrDefault(i => i.IsFile);
        //    if (firstFile != null)
        //    {///Templates.Cliver.InvoiceParser.Settings+TemplatesSettings.json
        //        await Download(client, path, firstFile.AsFile);
        //    }
        //}
        //static private async Task Download(DropboxClient client, string folder, FileMetadata file)
        //{
        //    Console.WriteLine("Download file...");

        //    using (var response = await client.Files.DownloadAsync(folder + "/" + file.Name))
        //    {
        //        Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Rev);
        //        Console.WriteLine("------------------------------");
        //        Console.WriteLine(await response.GetContentAsStringAsync());
        //        Console.WriteLine("------------------------------");
        //    }
        //}

        async static Task<DateTime> getRemoteTemplatesTimestamp(WebClient wc)
        {
            wc.Headers["Authorization"] = "Bearer " + Settings.General.RemoteAccessToken;
            object dropboxAPIArg = new
            {
                path = Settings.General.RemoteTemplatesPath,
                include_media_info = false,
                include_deleted = false,
                include_has_explicit_shared_members = false,
            };
            string body = SerializationRoutines.Json.Serialize(dropboxAPIArg, false);
            wc.Headers["Content-Type"] = "application/json";
            string data = await wc.UploadStringTaskAsync(new Uri("https://api.dropboxapi.com/2/files/get_metadata"), "POST", body);
            Dictionary<string, string> keys2value = SerializationRoutines.Json.Deserialize<Dictionary<string, string>>(data);
            string client_modified;
            if (!keys2value.TryGetValue("client_modified", out client_modified))
                throw (new Exception("Response does not have key 'client_modified'."));
            DateTime lastModifiedTimestamp;
            if (!DateTime.TryParse(client_modified, out lastModifiedTimestamp))
                throw (new Exception("Could not convert '" + client_modified + "' to DateTime."));
            return lastModifiedTimestamp;
        }

        async static public void StartUpdatingTemplates(bool automaticlyLookingForNewerTemplates, Form owner, Action onFinished)
        {
            try
            {
                TemplatesUpdatingForm pf = null;
                WebClient wc = null;
                DateTime lastModifiedTimestamp = DateTime.MinValue;
                Action cancel = () =>
                {
                    try
                    {
                        wc?.CancelAsync();
                    }
                    catch { }//if disposed
                };

                if (!automaticlyLookingForNewerTemplates)
                    pf = TemplatesUpdatingForm.startShowDialog(owner, cancel);

                using (wc = new WebClient())
                {
                    lastModifiedTimestamp = await getRemoteTemplatesTimestamp(wc);
                }
                if (!string.IsNullOrWhiteSpace(Settings.Templates.__File) && File.Exists(Settings.Templates.__File))
                {
                    if (lastModifiedTimestamp <= Settings.General.LastDownloadedTemplatesTimestamp)
                    {
                        if (!automaticlyLookingForNewerTemplates)
                            Message.Inform("There is no newer templates collection to download.", owner);
                        if (pf != null)
                            pf.stopShowDialog();
                        return;
                    }
                    if (!Message.YesNo("A newer template collection can be downloaded. The existing template collection will be moved to folder '_old' where the changes you've done if any, can be restored from. Proceed with downloading the templates from the internet?", owner, Message.Icons.Exclamation))
                    {
                        if (pf != null)
                            pf.stopShowDialog();
                        return;
                    }
                }
                if (pf == null)
                    pf = startShowDialog(owner, cancel);

                using (wc = new WebClient())
                {
                    wc.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e)
                    {
                        try
                        {
                            if (pf != null)
                            {
                                pf.showProgress(e.ProgressPercentage);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Main.Error(ex);
                        }
                    };

                    wc.UploadDataCompleted += delegate (object sender, UploadDataCompletedEventArgs e)
                    {
                        try
                        {
                            if (e.Cancelled)
                                return;

                            //SleepRoutines.Wait(10000);

                            string file2 = Settings.Templates.__File + "_";
                            File.WriteAllBytes(file2, e.Result);
                            if (!string.IsNullOrWhiteSpace(Settings.Templates.__File) && File.Exists(Settings.Templates.__File))
                            {
                                string oldDir = FileSystemRoutines.CreateDirectory(PathRoutines.GetDirFromPath(Settings.Templates.__File) + "\\_old");
                                DirectoryInfo di = new DirectoryInfo(oldDir);
                                FileInfo[] fis = di.GetFiles(PathRoutines.GetFileNameWithoutExtentionFromPath(Settings.Templates.__File) + "*");
                                if (fis.Where(x => x.LastWriteTime == File.GetLastWriteTime(Settings.Templates.__File)).FirstOrDefault() == null)
                                    File.Move(Settings.Templates.__File, oldDir + "\\" + PathRoutines.InsertSuffixBeforeFileExtension(PathRoutines.GetFileNameFromPath(Settings.Templates.__File), DateTime.Now.ToString("_yyyy-MM-dd-hh-mm-ss")));
                            }
                            File.Move(file2, Settings.Templates.__File);
                            Settings.General.LastDownloadedTemplatesTimestamp = lastModifiedTimestamp;
                            Settings.General.Save();
                            Settings.Templates.Reload();
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex, owner);
                        }
                        finally
                        {
                            onFinished();
                            if (pf != null)
                                pf.stopShowDialog();
                        }
                    };
                    wc.Headers["Authorization"] = "Bearer " + Settings.General.RemoteAccessToken;
                    object dropboxAPIArg = new
                    {
                        path = Settings.General.RemoteTemplatesPath,
                    };
                    string parameters = SerializationRoutines.Json.Serialize(dropboxAPIArg, false);
                    wc.Headers["Dropbox-API-Arg"] = parameters;// "{\"path\": \"" + Settings.General.RemoteTemplatesPath + "\"}";
                    wc.Headers["Content-Type"] = "";
                    //byte[] data = await wc.UploadDataTaskAsync(new Uri("https://content.dropboxapi.com/2/files/download"), "POST", new byte[0]);
                    wc.UploadDataAsync(new Uri("https://content.dropboxapi.com/2/files/download"), "POST", new byte[0]);
                    //wc.Headers["Content-Type"] = "application/json";
                    //wc.UploadDataAsync(new Uri("https://api.dropboxapi.com/2/files/download"), "POST", Encoding.ASCII.GetBytes(body));
                }
            }
            catch (Exception e)
            {
                LogMessage.Error("Downloading templates collection: \r\n" + Log.GetExceptionMessage(e), owner);
            }
        }

        async static public void StartUploadingTemplates(Action onFinished, Form owner)
        {
            try
            {
                WebClient wc = null;
                Action cancel = () =>
                {
                    try
                    {
                        wc?.CancelAsync();
                    }
                    catch { }//if disposed
                };
                
                using (wc = new WebClient())
                {
                    wc.UploadProgressChanged += delegate (object sender, UploadProgressChangedEventArgs e)
                    {
                        try
                        {
                            //if (pf != null)
                            //{
                            //    pf.showProgress(e.ProgressPercentage);
                            //    return;
                            //}
                        }
                        catch (Exception ex)
                        {
                            Log.Main.Error(ex);
                        }
                    };

                    wc.UploadDataCompleted += delegate (object sender, UploadDataCompletedEventArgs e)
                    {
                        try
                        {
                            if (e.Cancelled)
                                return;

                            //Settings.General.LastDownloadedTemplatesTimestamp = lastModifiedTimestamp;
                            //Settings.General.Save();
                            //Settings.Templates.Reload();
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex, owner);
                        }
                        finally
                        {
                            onFinished();
                            //if (pf != null)
                            //    pf.stopShowDialog();
                        }
                    };
                    if (string.IsNullOrWhiteSpace(Settings.Templates.__File) || !File.Exists(Settings.Templates.__File))
                    {
                        Message.Inform("There is no template collection file to upload:\r\n" + Settings.Templates.__File);
                        return;
                    }
                    object dropboxAPIArg = new
                    {
                        path = Settings.General.RemoteTemplatesPath,
                        mode = "overwrite",
                        autorename = false,
                        mute = false,
                        strict_conflict = false,
                    };
                    string parameters = SerializationRoutines.Json.Serialize(dropboxAPIArg, false);
                    wc.Headers["Dropbox-API-Arg"] = parameters;
                    wc.Headers["Content-Type"] = "application/octet-stream";
                    wc.UploadDataAsync(new Uri("https://content.dropboxapi.com/2/files/upload"), "POST", File.ReadAllBytes(Settings.Templates.__File));
                }
            }
            catch (Exception e)
            {
                LogMessage.Error("Downloading templates collection: \r\n" + Log.GetExceptionMessage(e), owner);
            }
        }
    }
}