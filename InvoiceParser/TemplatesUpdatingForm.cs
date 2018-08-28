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

namespace Cliver.InvoiceParser
{
    public partial class TemplatesUpdatingForm : Form
    {
        public TemplatesUpdatingForm()
        {
            InitializeComponent();

            this.Icon = AssemblyRoutines.GetAppIcon();
            Text = Application.ProductName + ": upadating templates";
        }

        static TemplatesUpdatingForm startShowDialog(Form owner, Action cancel)
        {
            TemplatesUpdatingForm pf = new TemplatesUpdatingForm();
            pf.CreateHandle();
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
                try
                {
                    progressBar.Value = p;
                }
                catch
                {
                }//it gives negative in the end for some reason
            });
        }

        void showState(string s)
        {
            this.Invoke(() =>
            {
                state.Text = s;
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

        async static Task<DateTime> getRemoteTemplatesTimestamp(WebClient wc)
        {
            wc.Headers["Authorization"] = "Bearer " + Settings.Remote.AccessToken;
            object dropboxAPIArg = new
            {
                path = Settings.Remote.TemplatesPath,
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

        async static public void StartUpdatingTemplatesFromRemoteLocation(bool automaticlyLookingForNewerTemplates, Form owner, Action onFinished)
        {
            TemplatesUpdatingForm pf = null;
            try
            {
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
                    string m2 = "The current template collection will be copied to the remote location and also to the local folder '_old' from where it can be restored when needed.";
                    if (lastModifiedTimestamp <= Settings.Remote.LastDownloadedTemplatesTimestamp)
                    {
                        if (automaticlyLookingForNewerTemplates
                            || !Message.YesNo("There is no newer template collection to download. Do you want to download the remote version anyway?\r\n\r\n" + m2, pf, Message.Icons.Question, false))
                        {
                            if (pf != null)
                                pf.stopShowDialog();
                            return;
                        }
                    }
                    else if (!Message.YesNo("A newer template collection can be downloaded. " + m2 + "\r\nProceed with updating the templates from the remote location?", pf))
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
                    if (!await pf.uploadLocalTemplates(wc))
                    {
                        if (pf != null)
                            pf.stopShowDialog();
                        return;
                    }
                }

                pf.showState("Downloading the template collection from the remote location...");
                pf.showProgress(0);

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
                            Settings.Remote.LastDownloadedTemplatesTimestamp = lastModifiedTimestamp;
                            Settings.Remote.Save();
                            Settings.Templates.Reload();
                            Message.Inform("The template collection has been updated successfully. The previous version can be found in folder 'old'.", pf);
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex, pf);
                        }
                        finally
                        {
                            onFinished();
                            if (pf != null)
                                pf.stopShowDialog();
                        }
                    };
                    wc.Headers["Authorization"] = "Bearer " + Settings.Remote.AccessToken;
                    object dropboxAPIArg = new
                    {
                        path = Settings.Remote.TemplatesPath,
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
                LogMessage.Error("Downloading template collection: \r\n" + Log.GetExceptionMessage2(e), pf);
            }
        }

        async public Task<bool> uploadLocalTemplates(WebClient wc)
        {
            UploadProgressChangedEventHandler uploadProgressChangedEventHandler = delegate (object sender, UploadProgressChangedEventArgs e)
            {
                try
                {
                    showProgress(e.ProgressPercentage);
                }
                catch (Exception ex)
                {
                    Log.Main.Error(ex);
                }
            };

            UploadDataCompletedEventHandler uploadDataCompletedEventHandler = delegate (object sender, UploadDataCompletedEventArgs e)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    LogMessage.Error(ex, this);
                }
                finally
                {
                }
            };

            try
            {
                showState("Uploading the current template collection to the remote location...");

                wc.UploadProgressChanged += uploadProgressChangedEventHandler;
                wc.UploadDataCompleted += uploadDataCompletedEventHandler;
                if (string.IsNullOrWhiteSpace(Settings.Templates.__File) || !File.Exists(Settings.Templates.__File))
                {
                    Log.Main.Inform("There is no template collection file to upload:\r\n" + Settings.Templates.__File);
                    return true;
                }
                //ddQ5o69t6GAAAAAAAAAFG0oLO5Oq3Xhs_j_W0hIhJazCpkoKQJ6mvKLMACURYv1C
                wc.Headers["Authorization"] = "Bearer " + Settings.Remote.AccessToken;
                object dropboxAPIArg = new
                {
                    path = "/_last" + Settings.Remote.TemplatesPath,
                    mode = "overwrite",
                    autorename = false,
                    mute = false,
                    strict_conflict = false,
                };
                string parameters = SerializationRoutines.Json.Serialize(dropboxAPIArg, false);
                wc.Headers["Dropbox-API-Arg"] = parameters;
                wc.Headers["Content-Type"] = "application/octet-stream";
                byte[] r = await wc.UploadDataTaskAsync(new Uri("https://content.dropboxapi.com/2/files/upload"), "POST", File.ReadAllBytes(Settings.Templates.__File));
                Dictionary<string, string> keys2value = SerializationRoutines.Json.Deserialize<Dictionary<string, string>>(Encoding.UTF8.GetString(r));
                if(keys2value.ContainsKey("rev"))
                    return true;
            }
            catch (Exception e)
            {
                LogMessage.Error("Uploading template collection: \r\n" + Log.GetExceptionMessage2(e), this);
            }
            finally
            {
                wc.UploadProgressChanged -= uploadProgressChangedEventHandler;
                wc.UploadDataCompleted -= uploadDataCompletedEventHandler;
            }
            return false;
        }
    }
}