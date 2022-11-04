//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Cliver
{
    /// <summary>
    /// Read/write a record-per-line file. Thread-safe.
    /// </summary>
    abstract public class StorageFile<RecordT> where RecordT : new()
    {
        protected StorageFile(string file)
        {
            File = file;
            FileSystemRoutines.CreateDirectory(PathRoutines.GetFileDir(File));
        }
        public readonly string File;

        public void Write(IEnumerable<RecordT> records, bool append = true)
        {
            lock (this)
            {
                write(records, append);
            }
        }
        abstract protected void write(IEnumerable<RecordT> records, bool append = true);

        public IEnumerable<RecordT> Read()
        {
            lock (this)
            {
                return read();
            }
        }
        abstract protected IEnumerable<RecordT> read();

        public class Tsv : StorageFile<RecordT>
        {
            public Tsv(string file) : base(file)
            {
                foreach (FieldInfo fi in typeof(RecordT).GetFields())
                    if (fi.GetCustomAttribute<FieldPreparation.IgnoredField>() == null)
                        fis.Add(fi);
                if (fis.Count == 0)
                    throw new Exception("Record type " + typeof(RecordT).Name + " has no serilazable fields.");
            }
            readonly List<FieldInfo> fis = new List<FieldInfo>();

            override protected void write(IEnumerable<RecordT> records, bool append = true)
            {
                using (TextWriter tw = new StreamWriter(File, append))
                {
                    FileInfo fileInfo = new FileInfo(File);
                    if (!append || !fileInfo.Exists || fileInfo.Length == 0)
                    {
                        List<string> hs = new List<string>();
                        foreach (FieldInfo fi in fis)
                            hs.Add(fi.Name);
                        tw.WriteLine(getLine(hs));
                    }
                    foreach (RecordT r in records)
                        tw.WriteLine(getLine(r));
                }
            }
            string getLine(RecordT record)
            {
                IEnumerable<object> getValues()
                {
                    foreach (FieldInfo fi in fis)
                        yield return fi.GetValue(record);
                }
                return getLine(getValues());
            }
            string getLine(IEnumerable<object> values)
            {
                List<string> ss = new List<string>();
                foreach (object v in values)
                {
                    string s = v == null ? "" : Regex.Replace(v.ToString(), @"\t+", " ");
                    ss.Add(s);
                }
                return string.Join("\t", ss);
            }

            override protected IEnumerable<RecordT> read()
            {
                if (System.IO.File.Exists(File))
                    using (TextReader tr = new StreamReader(File))
                    {
                        string l = tr.ReadLine();//pass off the header
                        for (l = tr.ReadLine(); l != null; l = tr.ReadLine())
                        {
                            string[] vs = l.Split('\t');
                            //RecordT d = Activator.CreateInstance<RecordT>();
                            RecordT r = new RecordT();
                            for (int i = 0; i < fis.Count; i++)
                                fis[i].SetValue(r, Convert.ChangeType(vs[i], fis[i].FieldType));
                            yield return r;
                        }
                    }
            }
        }

        public class Json : StorageFile<RecordT>
        {
            public Json(string file) : base(file)
            { }

            override protected void write(IEnumerable<RecordT> records, bool append = true)
            {
                using (TextWriter tw = new StreamWriter(File, append))
                    foreach (RecordT r in records)
                        tw.WriteLine(Serialization.Json.Serialize(r, false));
            }

            override protected IEnumerable<RecordT> read()
            {
                if (System.IO.File.Exists(File))
                    using (TextReader tr = new StreamReader(File))
                        for (string l = tr.ReadLine(); l != null; l = tr.ReadLine())
                            yield return Serialization.Json.Deserialize<RecordT>(l);
            }
        }
    }
}
