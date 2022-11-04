/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
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
        public class Table<DocumentT> : IDisposable where DocumentT : new()
        {
            public static Table<DocumentT> Get(string directory, Modes? mode = null)
            {
                return get(directory, (string file) => { return new Table<DocumentT>(file, mode); });
            }
            protected delegate Table<DocumentT> NewTable(string file);
            protected static Table<DocumentT> get(string directory, NewTable newTable)
            {
                string file = PathRoutines.GetNormalizedPath(directory, true) + Path.DirectorySeparatorChar + typeof(DocumentT).Name + "s" + ".listdb";
                lock (tableFiles2table)
                {
                    if (!tableFiles2table.TryGetValue(file, out Table<DocumentT> table)
                        || table.Disposed
                        )
                    {
                        //table = (Table<DocumentT>)Activator.CreateInstance(typeof(Table<DocumentT>), file, mode);
                        table = newTable(file);
                        tableFiles2table[file] = table;
                    }
                    return table;
                }
            }
            static Dictionary<string, Table<DocumentT>> tableFiles2table = new Dictionary<string, Table<DocumentT>>();

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
                        if (fileWriter == null)
                            return;

                        lock (tableFiles2table)
                        {
                            if (tableFiles2table.ContainsKey(File))
                                tableFiles2table.Remove(File);
                        }

                        if (Mode.HasFlag(Modes.FLUSH_ON_CLOSE))
                        {
                            bool flush = false;
                            using (TextReader tr = new StreamReader(File))
                            {
                                for (string l = tr.ReadLine(); l != null; l = tr.ReadLine())
                                    if (l[0] != '{' && l[0] != '[')
                                    {
                                        flush = true;
                                        break;
                                    }
                            }
                            if (flush)
                                Flush();
                        }
                        if (fileWriter != null)
                        {
                            fileWriter.Close();
                            fileWriter = null;
                        }
                    }
                    catch
                    {
                        //when Dispose is called from finalizer, files may be already closed and so exception thrown
                    }
                }
            }

            public bool Disposed
            {
                get
                {
                    lock (this) return fileWriter == null;
                }
            }

            public readonly string File = null;
            protected TextWriter fileWriter;
            readonly public Modes Mode = Modes.FLUSH_ON_CLOSE | Modes.IGNORE_RESTORING_ERROR;
            public readonly string Name;
            protected readonly List<DocumentT> documents = new List<DocumentT>();

            public enum Modes
            {
                NULL = 0,
                //KEEP_EVER_OPEN = 1,//requires explicitly calling Close()
                FLUSH_ON_CLOSE = 2,
                FLUSH_ON_START = 4,
                IGNORE_RESTORING_ERROR = 8,
            }

            public delegate void SavedHandler(DocumentT document, bool asNew);
            public event SavedHandler Saved = null;
            protected void invokeSaved(DocumentT document, bool asNew)
            {
                Saved?.Invoke(document, asNew);
            }

            public delegate void RemovedHandler(DocumentT document, bool sucess);
            public event RemovedHandler Removed = null;

            protected Table(string file, Modes? mode = null)
            {
                File = file;
                Name = PathRoutines.GetFileNameWithoutExtention(file);
                if (mode != null)
                    Mode = mode.Value;
                newFile = file + ".new";

                if (System.IO.File.Exists(newFile))
                {
                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(newFile);
                    else
                        System.IO.File.Move(newFile, File);
                }

                int index = -1;
                if (System.IO.File.Exists(File))
                {
                    using (TextReader tr = new StreamReader(File))
                    {
                        try
                        {
                            for (string l = tr.ReadLine(); l != null; l = tr.ReadLine())
                            {
                                DocumentT document;
                                if (l[0] == '{' || l[0] == '[')
                                {//a serialized record
                                    document = JsonConvert.DeserializeObject<DocumentT>(l);
                                    documents.Add(document);
                                }
                                else
                                {//a record operation which is followed by the record
                                    Match m = actionReadingRegex.Match(l);
                                    if (!Enum.TryParse(m.Groups[1].Value, out Action action))
                                        throw new Exception("Unknown action in the ListDb file: '" + m.Groups[1].Value + "'");
                                    index = int.Parse(m.Groups[2].Value);

                                    switch (action)
                                    {
                                        case Action.deleted:
                                            if (documents.Count <= index)
                                                throw new RestoreException("There are less documents in the table '" + Name + "' than a deleted element index (" + index + ")");
                                            documents.RemoveAt(index);
                                            continue;
                                        case Action.replaced:
                                            {
                                                l = tr.ReadLine();
                                                if (l == null)
                                                    throw new RestoreException("There is no document in the table '" + Name + "' for a replaced element index (" + index + ")");
                                                document = JsonConvert.DeserializeObject<DocumentT>(l);
                                                if (index >= documents.Count)
                                                    throw new RestoreException("There are less documents in the table '" + Name + "' than a replaced element index (" + index + ")");
                                                documents.RemoveAt(index);
                                                documents.Insert(index, document);
                                            }
                                            continue;
                                        case Action.inserted:
                                            {
                                                l = tr.ReadLine();
                                                if (l == null)
                                                    throw new RestoreException("There is no document in the table '" + Name + "' for an inserted element index (" + index + ")");
                                                document = JsonConvert.DeserializeObject<DocumentT>(l);
                                                if (index >= documents.Count)
                                                    throw new RestoreException("There are less documents in the table '" + Name + "' (" + documents.Count + ") than an inserted element index (" + index + ")");
                                                documents.Insert(index, document);
                                            }
                                            continue;
                                        default:
                                            throw new Exception("Unknown action: " + action);
                                    }
                                }
                            }
                        }
                        catch (RestoreException e)
                        {
                            FirstRestoreException = e;
                            if (!Mode.HasFlag(Modes.IGNORE_RESTORING_ERROR))
                                throw;
                        }
                    }
                }
                if (Mode.HasFlag(Modes.FLUSH_ON_START) && index >= 0)
                    Flush();
                else
                {
                    fileWriter = new StreamWriter(File, true);
                    ((StreamWriter)fileWriter).AutoFlush = true;
                }
            }
            readonly Regex actionReadingRegex = new Regex("(" + string.Join("|", Enum.GetNames(typeof(Action))) + @")(?:\:\s*(\d+))?");
            readonly string newFile;

            public readonly RestoreException FirstRestoreException;

            public class RestoreException : Exception
            {
                public RestoreException(string message) : base(message)
                { }
            }

            protected enum Action
            {
                replaced,
                inserted,
                deleted,
            }

            protected void writeOperation(Action action, int index, DocumentT document)
            {
                fileWriter.WriteLine(action.ToString() + ": " + index);
                fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
            }
            protected void writeAdded(DocumentT document)
            {
                fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
            }
            protected void writeDeleted(int index)
            {
                fileWriter.WriteLine(Action.deleted + ": " + index);
            }

            /// <summary>
            /// Write data to the file, cleaning opearations if any.
            /// </summary>
            public void Flush()
            {
                lock (this)
                {
                    if (fileWriter != null)
                        fileWriter.Close();

                    using (TextWriter newFileWriter = new StreamWriter(newFile, false))
                    {
                        foreach (DocumentT d in documents)
                            newFileWriter.WriteLine(JsonConvert.SerializeObject(d, Formatting.None));
                        newFileWriter.Flush();
                    }

                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(File);
                    System.IO.File.Move(newFile, File);

                    fileWriter = new StreamWriter(File, true);
                    ((StreamWriter)fileWriter).AutoFlush = true;
                }
            }

            public void Close()
            {
                Dispose();
            }

            /// <summary>
            /// Delete the table.
            /// </summary>
            public void Drop()
            {
                lock (this)
                {
                    documents.Clear();

                    if (fileWriter != null)
                        fileWriter.Close();
                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(File);
                }
            }

            /// <summary>
            /// Remove all the data from the table.
            /// </summary>
            public void Clear()
            {
                lock (this)
                {
                    Drop();

                    fileWriter = new StreamWriter(File, false);
                    ((StreamWriter)fileWriter).AutoFlush = true;
                }
            }

            /// <summary>
            /// Adds a document to the table if it does not exists. Otherwise, overwrites it.
            /// Table works as an ordered HashSet.
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            virtual public Result Save(DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        writeOperation(Action.replaced, i, document);
                        Saved?.Invoke(document, false);
                        return Result.UPDATED;
                    }
                    else
                    {
                        documents.Add(document);
                        writeAdded(document);
                        Saved?.Invoke(document, true);
                        return Result.ADDED;
                    }
                }
            }

            public enum Result
            {
                ADDED,
                UPDATED,
                MOVED2TOP,
                MOVED,
                INSERTED,
            }

            /// <summary>
            /// Adds a document to the end. If it exists then it is moved to the end. 
            /// Table works as an ordered HashSet.
            /// </summary>
            /// <param name="document"></param>
            virtual public Result Add(DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        documents.RemoveAt(i);
                        documents.Add(document);
                        writeDeleted(i);
                        writeAdded(document);
                        Saved?.Invoke(document, false);
                        return Result.MOVED2TOP;
                    }
                    else
                    {
                        documents.Add(document);
                        writeAdded(document);
                        Saved?.Invoke(document, true);
                        return Result.ADDED;
                    }
                }
            }

            public void AddRange(IEnumerable<DocumentT> documents)
            {
                lock (this)
                {
                    foreach (DocumentT document in documents)
                        Add(document);
                }
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            virtual public Result Insert(int index, DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        documents.RemoveAt(i);
                        documents.Insert(index, document);
                        writeOperation(Action.replaced, i, document);
                        Saved?.Invoke(document, false);
                        return Result.MOVED;
                    }
                    else
                    {
                        documents.Insert(index, document);
                        writeOperation(Action.inserted, index, document);
                        Saved?.Invoke(document, true);
                        return Result.INSERTED;
                    }
                }
            }

            public IEnumerable<DocumentT> Find(Predicate<DocumentT> match)
            {
                lock (this)
                {
                    List<DocumentT> matchedDocuments = new List<DocumentT>();
                    foreach (DocumentT document in documents)
                        if (match(document))
                            yield return document;
                }
            }

            public void InsertRange(int index, IEnumerable<DocumentT> documents)
            {
                lock (this)
                {
                    throw new Exception("TBD");
                    //base.InsertRange(index, documents);
                }
            }

            public bool Remove(DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        documents.RemoveAt(i);
                        writeDeleted(i);
                        Removed?.Invoke(document, true);
                        return true;
                    }
                    Removed?.Invoke(document, false);
                    return false;
                }
            }

            public int RemoveAll(Predicate<DocumentT> match)
            {
                lock (this)
                {
                    List<DocumentT> documentsToDelete = new List<DocumentT>();
                    foreach (DocumentT document in documents)
                        if (match(document))
                            documentsToDelete.Add(document);
                    foreach (DocumentT document in documentsToDelete)
                        Remove(document);
                    return documentsToDelete.Count;
                }
            }

            public void RemoveAt(int index)
            {
                lock (this)
                {
                    documents.RemoveAt(index);
                    writeDeleted(index);
                }
            }

            public void RemoveRange(int index, int count)
            {
                lock (this)
                {
                    for (int i = index; i < count; i++)
                        RemoveAt(i);
                }
            }

            public int Count
            {
                get
                {
                    lock (this)
                    {
                        return documents.Count;
                    }
                }
            }

            public DocumentT First(Predicate<DocumentT> match = null)
            {
                lock (this)
                {
                    if (match == null)
                        return documents.First();
                    return documents.First(new Func<DocumentT, bool>(match));
                }
            }

            public DocumentT Last(Predicate<DocumentT> match = null)
            {
                lock (this)
                {
                    if (match == null)
                        return documents.Last();
                    return documents.Last(new Func<DocumentT, bool>(match));
                }
            }

            //public DocumentT? GetPrevious(DocumentT document)
            //{
            //    if (document == null)
            //        return null;
            //    int i = IndexOf(document);
            //    if (i < 1)
            //        return null;
            //    return (DocumentT?)this[i - 1];
            //}

            //public DocumentT? GetNext(DocumentT document)
            //{
            //    if (document == null)
            //        return null;
            //    int i = this.IndexOf(document);
            //    if (i + 1 >= this.Count)
            //        return null;
            //    return (DocumentT?)this[i + 1];
            //}
        }
    }
}