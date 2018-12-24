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

namespace Cliver.SampleParser
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

            string output_records_file = Settings.General.InputFolder + "\\_output.csv";
            if (File.Exists(output_records_file))
                File.Delete(output_records_file);
            TextWriter tw = new StreamWriter(output_records_file, false);

            List<Template2> active_templates = Settings.Template2s.Template2s.Where(x => x.Active).OrderBy(x => x.OrderWeight).ThenByDescending(x =>
            {
                return Settings.TemplateLocalInfo.GetInfo(x.Template.Name).UsedTime;
            }).ToList();
            if (active_templates.Count < 1)
            {
                LogMessage.Error("There is no active template!");
                return;
            }

            List<string> orderedOutputFieldNames = active_templates[0].Template.Fields.Select(x => x.Name).ToList();
            List<string> headers = new List<string> { "File" };
            headers.AddRange(orderedOutputFieldNames);
            headers.AddRange(new List<string> { "Template", "First Page" });

            tw.WriteLine(FieldPreparation.GetCsvHeaderLine(headers, FieldPreparation.FieldSeparator.COMMA));

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
                    bool? result = PdfProcessor.Process(f, active_templates, (templateName, page_i, fieldNames2texts) =>
                    {
                        List<string> values = new List<string>() { PathRoutines.GetFileNameFromPath(f) };
                        foreach (string fn in orderedOutputFieldNames)
                        {
                            fieldNames2texts.TryGetValue(fn, out string t);
                            values.Add(t);
                        }
                        values.AddRange(new List<string> { templateName, page_i.ToString() });
                        tw.WriteLine(FieldPreparation.GetCsvLine(values, FieldPreparation.FieldSeparator.COMMA));
                    });

                    if (result != true)
                        failed_files.Add(f);
                }
                catch (Exception e)
                {
                    Log.Main.Error("Processing file '" + f + "'", e);
                    failed_files.Add(f);
                }
                progress(++processed_files, total_files);
            }
            tw.Close();

            try
            {
                System.Diagnostics.Process.Start(output_records_file);
            }
            catch { }

            Log.Main.Inform("COMPLETED:\r\nTotal files: " + processed_files + "\r\nSuccess files: " + (processed_files - failed_files.Count) + "\r\nFailed files: " + failed_files.Count + "\r\n" + string.Join("\r\n", failed_files));
            if (failed_files.Count > 0)
                Message.Error("There were " + failed_files.Count + " failed files.\r\nSee details in the log.");
            //progress(0, 0);
        }
    }
}