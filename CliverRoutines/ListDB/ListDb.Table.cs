/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace Cliver
{
    public partial class ListDb
    {
        public class testDocument //: ListDb.Document
        {
            public string A = DateTime.Now.ToString() + "\r\n" + DateTime.Now.Ticks.ToString();
            public string B = "test";
            public long C = DateTime.Now.Ticks;
            public DateTime DocumentType = DateTime.Now;

            static public void Test()
            {
                ListDb.Table<testDocument> t = ListDb.Table<testDocument>.Get();
                //t.Drop();
                t.Save(new testDocument());
                t.Save(new testDocument());
                t.Insert(0, new testDocument());
                testDocument d = t.Last();
                d.A = @"changed";
                t.Save(d);
                t.Insert(t.Count - 1, t.First());
                t.Flush();
            }
        }

        //public class Document
        //{
        //[Newtonsoft.Json.JsonIgnoreAttribute]
        //public string test;
        //}

        //public class IgnoredField : Attribute
        //{
        //}

        public static Table<DocumentType> GetTable<DocumentType>(string directory = null) where DocumentType : new()
        {
            return Table<DocumentType>.Get(directory);
        }

        public class Table<DocumentType> : List<DocumentType>, IDisposable where DocumentType : new()
        {
            public static Table<DocumentType> Get(string directory = null)
            {
                directory = getNormalizedDirectory(directory);
                WeakReference wr;
                string key = directory + System.IO.Path.DirectorySeparatorChar + typeof(DocumentType).Name;
                if (!tableKeys2table.TryGetValue(key, out wr)
                    || !wr.IsAlive
                    )
                {
                    Table<DocumentType> t = new Table<DocumentType>(directory, key);
                    wr = new WeakReference(t);
                    tableKeys2table[key] = wr;
                }
                return (Table<DocumentType>)wr.Target;
            }
            protected static Dictionary<string, WeakReference> tableKeys2table = new Dictionary<string, WeakReference>();

            protected static string getNormalizedDirectory(string directory = null)
            {
                if (directory == null)
                    directory = Cliver.Log.AppCompanyCommonDataDir;
                return PathRoutines.GetNormalizedPath(directory, true);
            }

            public readonly string Log = null;
            protected TextWriter logWriter;
            public readonly string File = null;
            protected TextWriter fileWriter;
            readonly string newFile;
            public Modes Mode = Modes.FLUSH_TABLE_ON_CLOSE;
            public readonly string Name;
            protected readonly string key;

            public enum Modes
            {
                NULL = 0,
                KEEP_OPEN_TABLE_FOREVER = 1,//requires explicite call Close()
                FLUSH_TABLE_ON_CLOSE = 2,
                FLUSH_TABLE_ON_START = 4,
            }

            public delegate void SavedHandler(DocumentType document, bool asNew);
            public event SavedHandler Saved = null;

            public delegate void RemovedHandler(DocumentType document, bool sucess);
            public event RemovedHandler Removed = null;

            protected Table(string directory, string key)
            {
                directory = getNormalizedDirectory(directory);
                this.key = key;

                Name = typeof(DocumentType).Name + "s";

                File = directory + System.IO.Path.DirectorySeparatorChar + Name + ".listdb";
                newFile = File + ".new";
                Log = directory + System.IO.Path.DirectorySeparatorChar + Name + ".listdb.log";

                if (System.IO.File.Exists(newFile))
                {
                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(File);
                    System.IO.File.Move(newFile, File);
                    if (System.IO.File.Exists(Log))
                        System.IO.File.Delete(Log);
                }

                if (System.IO.File.Exists(File))
                {
                    using (TextReader fr = new StreamReader(File))
                    {
                        if (System.IO.File.Exists(Log))
                        {
                            foreach (string l in System.IO.File.ReadLines(Log))
                            {
                                Match m = Regex.Match(l, @"flushed:\s+\[(\d+)\]");
                                if (m.Success)
                                {
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    for (int i = 0; i < p1; i++)
                                    {
                                        DocumentType d = JsonConvert.DeserializeObject<DocumentType>(fr.ReadLine());
                                        base.Add(d);
                                    }
                                    continue;
                                }
                                m = Regex.Match(l, @"deleted:\s+(\d+)");
                                if (m.Success)
                                {
                                    base.RemoveAt(int.Parse(m.Groups[1].Value));
                                    continue;
                                }
                                m = Regex.Match(l, @"replaced:\s+(\d+)");
                                if (m.Success)
                                {
                                    DocumentType d = JsonConvert.DeserializeObject<DocumentType>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 >= base.Count)
                                        throw new Exception("Log file broken.");
                                    base.RemoveAt(p1);
                                    base.Insert(p1, d);
                                    continue;
                                }
                                m = Regex.Match(l, @"added:\s+(\d+)");
                                if (m.Success)
                                {
                                    DocumentType d = JsonConvert.DeserializeObject<DocumentType>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 != base.Count)
                                        throw new Exception("Log file broken.");
                                    base.Add(d);
                                    continue;
                                }
                                m = Regex.Match(l, @"inserted:\s+(\d+)");
                                if (m.Success)
                                {
                                    DocumentType d = JsonConvert.DeserializeObject<DocumentType>(fr.ReadLine());
                                    int p1 = int.Parse(m.Groups[1].Value);
                                    if (p1 >= base.Count)
                                        throw new Exception("Log file broken.");
                                    base.Insert(p1, d);
                                    continue;
                                }
                            }
                            if ((Mode & Modes.FLUSH_TABLE_ON_START) == Modes.FLUSH_TABLE_ON_START)
                            {
                                foreach (string l in System.IO.File.ReadLines(Log))
                                {
                                    if (!Regex.IsMatch(l, @"flushed:\s+\[(\d+)\]"))
                                    {
                                        Flush();
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (string s = fr.ReadLine(); s != null; s = fr.ReadLine())
                                base.Add(JsonConvert.DeserializeObject<DocumentType>(s));
                        }
                        if (fr.ReadLine() != null)
                            throw new Exception("Log file broken.");
                    }
                }

                fileWriter = new StreamWriter(File, true);
                ((StreamWriter)fileWriter).AutoFlush = true;
                logWriter = new StreamWriter(Log, true);
                ((StreamWriter)logWriter).AutoFlush = true;
            }

            ~Table()
            {
                Dispose();
            }

            public void Dispose()
            {
                lock (this)
                {
                    try
                    {
                        if (disposed)
                            return;
                        disposed = true;

                        if (tableKeys2table.ContainsKey(key))
                            tableKeys2table.Remove(key);

                        if ((Mode & Modes.FLUSH_TABLE_ON_CLOSE) == Modes.FLUSH_TABLE_ON_CLOSE)
                            Flush();
                        if (fileWriter != null)
                            fileWriter.Dispose();
                        if (logWriter != null)
                            logWriter.Dispose();
                    }
                    catch
                    {
                        //when Dispose is called from finalizer, files may be already closed and so exception thrown
                    }
                }
            }
            protected bool disposed = false;

            public void Flush()
            {
                logWriter.WriteLine("flushing");

                using (TextWriter newFileWriter = new StreamWriter(newFile, false))
                {
                    foreach (DocumentType d in this)
                        newFileWriter.WriteLine(JsonConvert.SerializeObject(d, Formatting.None));
                    newFileWriter.Flush();
                }

                if (fileWriter != null)
                    fileWriter.Dispose();
                if (System.IO.File.Exists(File))
                    System.IO.File.Delete(File);
                System.IO.File.Move(newFile, File);
                fileWriter = new StreamWriter(File, true);
                ((StreamWriter)fileWriter).AutoFlush = true;

                if (logWriter != null)
                    logWriter.Dispose();
                logWriter = new StreamWriter(Log, false);
                ((StreamWriter)logWriter).AutoFlush = true;
                logWriter.WriteLine("flushed: [" + base.Count + "]");
            }

            public void Drop()
            {
                base.Clear();

                if (fileWriter != null)
                    fileWriter.Dispose();
                if (System.IO.File.Exists(File))
                    System.IO.File.Delete(File);

                if (logWriter != null)
                    logWriter.Dispose();
                if (System.IO.File.Exists(Log))
                    System.IO.File.Delete(Log);
            }

            new public void Clear()
            {
                base.Clear();

                if (fileWriter != null)
                    fileWriter.Dispose();
                fileWriter = new StreamWriter(File, false);

                if (logWriter != null)
                    logWriter.Dispose();
                logWriter = new StreamWriter(Log, false);
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            virtual public Results Save(DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    logWriter.WriteLine("replaced: " + i);
                    Saved?.Invoke(document, false);
                    return Results.UPDATED;
                }
                else
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.Add(document);
                    logWriter.WriteLine("added: " + (base.Count - 1));
                    Saved?.Invoke(document, true);
                    return Results.ADDED;
                }
            }
            public enum Results
            {
                ADDED,
                UPDATED,
                MOVED2TOP,
                MOVED,
                INSERTED,
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            new virtual public Results Add(DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.RemoveAt(i);
                    base.Add(document);
                    logWriter.WriteLine("deleted: " + i + "\r\nadded: " + (base.Count - 1));
                    Saved?.Invoke(document, false);
                    return Results.MOVED2TOP;
                }
                else
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.Add(document);
                    logWriter.WriteLine("added: " + (base.Count - 1));
                    Saved?.Invoke(document, true);
                    return Results.ADDED;
                }
            }

            new public void AddRange(IEnumerable<DocumentType> documents)
            {
                throw new Exception("TBD");
                //base.AddRange(documents);
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            new virtual public Results Insert(int index, DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.RemoveAt(i);
                    base.Insert(index, document);
                    logWriter.WriteLine("replaced: " + i);
                    Saved?.Invoke(document, false);
                    return Results.MOVED;
                }
                else
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    base.Insert(index, document);
                    logWriter.WriteLine("inserted: " + index);
                    Saved?.Invoke(document, true);
                    return Results.INSERTED;
                }
            }

            new public void InsertRange(int index, IEnumerable<DocumentType> documents)
            {
                throw new Exception("TBD");
                //base.InsertRange(index, documents);
            }

            new public bool Remove(DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    base.RemoveAt(i);
                    logWriter.WriteLine("deleted: " + i);
                    Removed?.Invoke(document, true);
                    return true;
                }
                Removed?.Invoke(document, false);
                return false;
            }

            new public int RemoveAll(Predicate<DocumentType> match)
            {
                throw new Exception("TBD");
                //return base.RemoveAll(match);
            }

            new public void RemoveAt(int index)
            {
                base.RemoveAt(index);
                logWriter.WriteLine("deleted: " + index);
            }

            new public void RemoveRange(int index, int count)
            {
                for (int i = index; i < count; i++)
                {
                    base.RemoveAt(i);
                    logWriter.WriteLine("deleted: " + i);
                }
            }

            //public DocumentType? GetPrevious(DocumentType document)
            //{
            //    if (document == null)
            //        return null;
            //    int i = IndexOf(document);
            //    if (i < 1)
            //        return null;
            //    return (DocumentType?)this[i - 1];
            //}

            //public DocumentType? GetNext(DocumentType document)
            //{
            //    if (document == null)
            //        return null;
            //    int i = this.IndexOf(document);
            //    if (i + 1 >= this.Count)
            //        return null;
            //    return (DocumentType?)this[i + 1];
            //}
        }
    }
}