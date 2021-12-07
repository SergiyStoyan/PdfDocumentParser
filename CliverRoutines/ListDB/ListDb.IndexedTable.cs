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
using Newtonsoft.Json;

namespace Cliver
{
    public partial class ListDb
    {
        public class IndexedDocument
        {
            public long ID;
        }

        public static IndexedTable<DocumentType> GetIndexedTable<DocumentType>(string directory = null) where DocumentType : IndexedDocument, new()
        {
            return IndexedTable<DocumentType>.Get(directory);
        }

        public class IndexedTable<DocumentType> : Table<DocumentType>, IDisposable where DocumentType : IndexedDocument, new()
        {
            new public static IndexedTable<DocumentType> Get(string directory = null)
            {
                directory = getNormalizedDirectory(directory);
                WeakReference wr;
                string key = directory + System.IO.Path.DirectorySeparatorChar + typeof(DocumentType).Name;
                if (!tableKeys2table.TryGetValue(key, out wr)
                    || !wr.IsAlive
                    )
                {
                    IndexedTable<DocumentType> t = new IndexedTable<DocumentType>(directory, key);
                    wr = new WeakReference(t);
                    tableKeys2table[key] = wr;
                }
                return (IndexedTable<DocumentType>)wr.Target;
            }

            IndexedTable(string directory, string key) : base(directory, key)
            {
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            override public Results Save(DocumentType document)
            {
                int i = ((List<DocumentType>)this).IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    logWriter.WriteLine("replaced: " + i);
                    return Results.UPDATED;
                }
                else
                {
                    setNewId(document);
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<DocumentType>)this).Add(document);
                    logWriter.WriteLine("added: " + (base.Count - 1));
                    return Results.ADDED;
                }
            }

            void setNewId(DocumentType document)
            {
                System.Reflection.PropertyInfo pi = typeof(DocumentType).GetProperty("ID");
                pi.SetValue(document, DateTime.Now.Ticks);
            }

            public DocumentType GetById(int document_id)
            {
                return this.Where(x => x.ID == document_id).FirstOrDefault();
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            override public Results Add(DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<DocumentType>)this).RemoveAt(i);
                    ((List<DocumentType>)this).Add(document);
                    logWriter.WriteLine("deleted: " + i + "\r\nadded: " + (base.Count - 1));
                    return Results.MOVED2TOP;
                }
                else
                {
                    setNewId(document);
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<DocumentType>)this).Add(document);
                    logWriter.WriteLine("added: " + (base.Count - 1));
                    return Results.ADDED;
                }
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="index"></param>
            /// <param name="document"></param>
            override public Results Insert(int index, DocumentType document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<DocumentType>)this).RemoveAt(i);
                    ((List<DocumentType>)this).Insert(index, document);
                    logWriter.WriteLine("replaced: " + i);
                    return Results.MOVED;
                }
                else
                {
                    setNewId(document);
                    fileWriter.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<DocumentType>)this).Insert(index, document);
                    logWriter.WriteLine("inserted: " + index);
                    return Results.INSERTED;
                }
            }
        }
    }
}