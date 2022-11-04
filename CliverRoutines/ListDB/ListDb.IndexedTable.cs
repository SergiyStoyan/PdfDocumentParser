/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
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

        public class IndexedTable<DocumentT> : Table<DocumentT>, IDisposable where DocumentT : IndexedDocument, new()
        {
            new public static IndexedTable<DocumentT> Get(string directory, Modes? mode = null)
            {
                return (IndexedTable<DocumentT>)get(directory, (string file) => { return new IndexedTable<DocumentT>(file, mode); });
            }

            IndexedTable(string file, Modes? mode = null) : base(file, mode)
            {
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            override public Result Save(DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        writeOperation(Action.replaced, i, document);
                        //documents.Find(d=>d.ID==);
                        invokeSaved(document, false);
                        return Result.UPDATED;
                    }
                    else
                    {
                        setNewId(document);
                        writeAdded(document);
                        documents.Add(document);
                        invokeSaved(document, true);
                        return Result.ADDED;
                    }
                }
            }

            void setNewId(DocumentT document)
            {
                lock (this)
                {
                    System.Reflection.PropertyInfo pi = typeof(DocumentT).GetProperty("ID");
                    pi.SetValue(document, DateTime.Now.Ticks);
                }
            }

            public DocumentT GetById(int documentId)
            {
                lock (this)
                {
                    return documents.Where(x => x.ID == documentId).FirstOrDefault();
                }
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            override public Result Add(DocumentT document)
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
                        invokeSaved(document, false);
                        return Result.MOVED2TOP;
                    }
                    else
                    {
                        setNewId(document);
                        documents.Add(document);
                        writeAdded(document);
                        invokeSaved(document, true);
                        return Result.ADDED;
                    }
                }
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="index"></param>
            /// <param name="document"></param>
            override public Result Insert(int index, DocumentT document)
            {
                lock (this)
                {
                    int i = documents.IndexOf(document);
                    if (i >= 0)
                    {
                        documents.RemoveAt(i);
                        documents.Insert(index, document);
                        writeOperation(Action.replaced, i, document);
                        invokeSaved(document, false);
                        return Result.MOVED;
                    }
                    else
                    {
                        setNewId(document);
                        documents.Insert(index, document);
                        writeOperation(Action.inserted, index, document);
                        invokeSaved(document, true);
                        return Result.INSERTED;
                    }
                }
            }
        }
    }
}