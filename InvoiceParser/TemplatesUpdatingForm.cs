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
            owner.BeginInvoke2(() =>
            {
                pf.ShowDialog(owner);
                pf.showProgress(0);
            });
            return pf;
        }
        Action cancel = null;

        void showProgress(int p)
        {
            this.BeginInvoke2(() =>
            {
                progressBar.Value = p;
            });
        }

        void stopShowDialog()
        {
            this.BeginInvoke2(() =>
            {
                Close();
            });
        }

        private void abort_Click(object sender, EventArgs e)
        {
            if (!Message.YesNo("The template collection is being updated. Would you like to abort the operation?"))
                return;
            cancel?.Invoke();
            stopShowDialog();
        }

        static public void StartUpdatingTemplates(bool automaticlyLookingForNewerTemplates, Form owner, Action onFinished)
        {
            return;
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.dropbox.com/s/mhpgr119gg4sqnx/Templates.Cliver.InvoiceParser.Settings%2BTemplatesSettings.json?dl=0");
            //webRequest.Method = "HEAD";
            //webRequest.IfModifiedSince = Settings.General.LastDownloadedTemplatesTimestamp;
            //HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            //var g =   webResponse.LastModified;
            //var Date = webResponse.Headers["Last-Modified"];



            DateTime lastModifiedTimestamp = DateTime.MinValue;
            using (WebClient wc = new WebClient())
            {
                //cancel = delegate ()
                //{
                //    wc.CancelAsync();
                //};

                TemplatesUpdatingForm pf = null;
                wc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    try
                    {
                        if (!wc.IsBusy)
                            return;
                        if (pf != null)
                        {
                            pf.showProgress(e.ProgressPercentage);
                            return;
                        }
                        if (!string.IsNullOrWhiteSpace(Settings.Templates.__File) && File.Exists(Settings.Templates.__File))
                        {
                            if (wc.ResponseHeaders == null)
                                throw (new Exception("No headers received."));
                            string lms = wc.ResponseHeaders["Last-Modified"];
                            if (lms == null || lms.Length <= 0)
                                throw (new Exception("No Last-Modified header in the response."));

                            if (!DateTime.TryParse(lms, out lastModifiedTimestamp))
                                throw (new Exception("Could not convert Last-Modified header '" + lms + "' to DateTime."));
                            if (lastModifiedTimestamp <= Settings.General.LastDownloadedTemplatesTimestamp)
                            {
                                wc.CancelAsync();
                                if (!automaticlyLookingForNewerTemplates)
                                    Message.Inform("There is no newer templates collection to download.", owner);
                            }
                            if (!Message.YesNo("A newer template collection can be downloaded. The existing template collection will be moved to folder '_old' where the changes you've done if any, can be restored from. Proceed with downloading the templates from the internet?", owner, Message.Icons.Exclamation))
                                return;
                            pf = TemplatesUpdatingForm.startShowDialog(owner, () =>
                            {
                                wc?.CancelAsync();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        wc.CancelAsync();
                        Message.Error("While updating templates:\n\n" + ex.Message, owner);
                    }
                };

                wc.DownloadDataCompleted += delegate (object sender, DownloadDataCompletedEventArgs e)
                {
                    try
                    {
                        if (e.Cancelled)
                            return;

                        if (!string.IsNullOrWhiteSpace(Settings.Templates.__File) && File.Exists(Settings.Templates.__File))
                        {
                            string oldDir = FileSystemRoutines.CreateDirectory(PathRoutines.GetDirFromPath(Settings.Templates.__File) + "\\_old");
                            DirectoryInfo di = new DirectoryInfo(oldDir);
                            FileInfo[] fis = di.GetFiles(PathRoutines.GetFileNameWithoutExtentionFromPath(Settings.Templates.__File) + "*");
                            if (fis.Where(x => x.LastWriteTime == File.GetLastWriteTime(Settings.Templates.__File)).FirstOrDefault() == null)
                                File.Move(Settings.Templates.__File, oldDir + "\\" + PathRoutines.InsertSuffixBeforeFileExtension(PathRoutines.GetFileNameFromPath(Settings.Templates.__File), DateTime.Now.ToString("_YYYY-MM-dd")));
                        }
                        File.WriteAllBytes(Settings.Templates.__File, e.Result);
                        Settings.General.LastDownloadedTemplatesTimestamp = lastModifiedTimestamp;
                        Settings.General.Save();
                        Settings.Templates.Reload();
                    }
                    catch (Exception ex)
                    {
                        Message.Error(ex);
                    }
                    finally
                    {                        
                        onFinished();
                        if (pf != null)
                            pf.stopShowDialog();
                    }
                };
                Settings.General.LastDownloadedTemplatesTimestamp = DateTime.Now;
                wc.Headers["If-Modified-Since"] = Settings.General.LastDownloadedTemplatesTimestamp.ToString("ddd, dd MMM yyyy HH:mm:ss GMT");
                wc.DownloadDataAsync(new Uri(Settings.General.TemplatesUrl));
            }
        }
    }
}