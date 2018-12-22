//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cliver;
using System.Text.RegularExpressions;
using Cliver.PdfDocumentParser;

namespace Cliver.InvoiceParser
{
    class Processor
    {
        static public void Run(Action<int, int> progress)
        {
            Log.Main.Inform("STARTED");
            progress(0, 0);
            if (string.IsNullOrWhiteSpace(Settings.General.InputFolder))
            {
                LogMessage.Error("Input Folder is not specified.");
                return;
            }
            if (!Directory.Exists(Settings.General.InputFolder))
            {
                LogMessage.Error("Input folder '" + Settings.General.InputFolder + "' does not exist.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Settings.General.OutputFolder))
            {
                LogMessage.Error("Output Folder is not specified.");
                return;
            }
            if (PathRoutines.GetNormalizedPath(Settings.General.InputFolder) == PathRoutines.GetNormalizedPath(Settings.General.OutputFolder))
            {
                LogMessage.Error("Output Folder cannot be Input Folder.");
                return;
            }
            if (!Directory.Exists(Settings.General.OutputFolder))
            {
                Settings.General.OutputFolder = FileSystemRoutines.CreateDirectory(Settings.General.OutputFolder, true);
                if (!Directory.Exists(Settings.General.OutputFolder))
                {
                    LogMessage.Error("Output folder '" + Settings.General.OutputFolder + "' could not be created.");
                    return;
                }
            }
            else
            {
                if ((Directory.GetFiles(Settings.General.OutputFolder).FirstOrDefault() != null || Directory.GetDirectories(Settings.General.OutputFolder).FirstOrDefault() != null) && !Message.YesNo("Output folder '" + Settings.General.OutputFolder + "' is not empty and will be cleaned up. Proceed?"))
                    return;
                FileSystemRoutines.ClearDirectory(Settings.General.OutputFolder, true);
            }

            string output_records_file = Settings.General.OutputFolder + "\\output.xlsx";
            if (File.Exists(output_records_file))
                File.Delete(output_records_file);
            Excel xls = new Excel(output_records_file, DateTime.Now.ToString());

            List<Template2> active_templates = Settings.Template2s.Template2s.Where(x => x.Active).OrderBy(x => x.OrderWeight).ThenByDescending(x =>
            {
                return Settings.TemplateLocalInfo.GetInfo(x.Template.Name).UsedTime;
            }).ToList();
            if (active_templates.Count < 1)
            {
                LogMessage.Error("There is no active template!");
                return;
            }

            List<string> orderedOutputFieldNames = OutputConfigForm.GetOrderedOutputFieldNames();

            List<string> headers = active_templates[0].Template.Fields.Select(x => x.Name).ToList();
            List<string> hs0 = headers.OrderBy(a => a).ToList();
            for (int i = 1; i < active_templates.Count; i++)
            {
                Template2 t = active_templates[i];
                List<string> hs = t.Template.Fields.Select(x => x.Name).ToList();
                if (!hs.OrderBy(a => a).SequenceEqual(hs0))
                {
                    if (!LogMessage.AskYesNo("Templates '" + active_templates[0].Template.Name + "' and '" + active_templates[i].Template.Name + "' have different headers!\r\nProceed?", true))
                        return;
                }
            }
            headers.Clear();
            headers.AddRange(new List<string> { "Original File", "Stamped File" });
            headers.AddRange(orderedOutputFieldNames);
            headers.AddRange(new List<string> { "Template", "First Page" });

            xls.WriteLine(headers);

            List<string> files = FileSystemRoutines.GetFiles(Settings.General.InputFolder, Settings.General.ReadInputFolderRecursively);
            if (Settings.General.IgnoreHiddenFiles)
                files = files.Select(f => new FileInfo(f)).Where(fi => !fi.Attributes.HasFlag(FileAttributes.Hidden)).Select(fi => fi.FullName).ToList();

            int processed_files = 0;
            int total_files = files.Count;
            List<string> failed_files = new List<string>();
            progress(processed_files, total_files);
            foreach (string f in files)
            {
                try
                {
                    string of;
                    //if (Settings.General.ReadInputFolderRecursively)
                    //{
                        string od = PathRoutines.GetPathMirroredInDir(PathRoutines.GetDirFromPath(f), Settings.General.InputFolder, Settings.General.OutputFolder);
                        od = FileSystemRoutines.CreateDirectory(od, false);
                        of = PathRoutines.InsertSuffixBeforeFileExtension(od + "\\" + PathRoutines.GetFileNameFromPath(f), ".stamped");
                    //}
                    //else
                    //    of = PathRoutines.InsertSuffixBeforeFileExtension(Settings.General.OutputFolder + "\\" + PathRoutines.GetFileNameFromPath(f), ".stamped");
                    bool? result = PdfProcessor.Process(f, active_templates, of, (templateName, page_i, fieldNames2texts) =>
                    {
                        List<string> values = new List<string>() { PathRoutines.GetFileNameFromPath(f), PathRoutines.GetFileNameFromPath(of) };
                        foreach (string fn in orderedOutputFieldNames)
                        {
                            fieldNames2texts.TryGetValue(fn, out string t);
                            values.Add(t);
                        }
                        values.AddRange(new List<string> { templateName, page_i.ToString() });
                        int j = xls.WriteLine(values);
                        xls.SetLink(j, 1, new Uri(f, UriKind.Absolute));
                        xls.SetLink(j, 2, new Uri(of, UriKind.Relative));
                        int i = orderedOutputFieldNames.IndexOf(Template2.FieldNames.INVOICE);
                        if (i >= 0)
                            xls.SetLink(j, 2 + i + 1, new Uri(f, UriKind.Absolute));
                    });

                    if (result == true)
                        xls.Save();
                    else
                        failed_files.Add(f);
                }
                catch (Exception e)
                {
                    Log.Main.Error("Processing file '" + f + "'", e);
                    failed_files.Add(f);
                }
                progress(++processed_files, total_files);
            }

            xls.Dispose();
            //tw.Close();

            Log.Main.Inform("COMPLETED:\r\nTotal files: " + processed_files + "\r\nSuccess files: " + (processed_files - failed_files.Count) + "\r\nFailed files: " + failed_files.Count + "\r\n" + string.Join("\r\n", failed_files));
            if (failed_files.Count > 0)
                Message.Error("There were " + failed_files.Count + " failed files.\r\nSee details in the log.");
            //progress(0, 0);
        }
    }
}