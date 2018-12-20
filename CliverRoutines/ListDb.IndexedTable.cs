/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace Cliver
{
    public partial class ListDb
    {
        public class IndexedDocument
        {
            public long ID;
        }

        public static IndexedTable<D> GetIndexedTable<D>(string directory = null) where D : IndexedDocument, new()
        {
            return IndexedTable<D>.Get(directory);
        }

        public class IndexedTable<D> : Table<D>, IDisposable where D : IndexedDocument, new()
        {
            public static IndexedTable<D> Get(string directory = null)
            {
                directory = get_normalized_directory(directory);
                WeakReference wr;
                string key = directory + "\\" + typeof(D).Name;
                if (!table_keys2table.TryGetValue(key, out wr)
                    || !wr.IsAlive
                    )
                {
                    IndexedTable<D> t = new IndexedTable<D>(directory, key);
                    wr = new WeakReference(t);
                    table_keys2table[key] = wr;
                }
                return (IndexedTable<D>)wr.Target;
            }

            IndexedTable(string directory, string key) : base(directory, key)
            {
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            override public Results Save(D document)
            {
                int i = ((List<D>)this).IndexOf(document);
                if (i >= 0)
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    log_writer.WriteLine("replaced: " + i);
                    return Results.UPDATED;
                }
                else
                {
                    set_new_id(document);
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<D>)this).Add(document);
                    log_writer.WriteLine("added: " + (base.Count - 1));
                    return Results.ADDED;
                }
            }

            void set_new_id(D document)
            {
                System.Reflection.PropertyInfo pi = typeof(D).GetProperty("ID");
                pi.SetValue(document, DateTime.Now.Ticks);
            }

            public D GetById(int document_id)
            {
                return this.Where(x => x.ID == document_id).FirstOrDefault();
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            override public Results Add(D document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<D>)this).RemoveAt(i);
                    ((List<D>)this).Add(document);
                    log_writer.WriteLine("deleted: " + i + "\r\nadded: " + (base.Count - 1));
                    return Results.MOVED2TOP;
                }
                else
                {
                    set_new_id(document);
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<D>)this).Add(document);
                    log_writer.WriteLine("added: " + (base.Count - 1));
                    return Results.ADDED;
                }
            }

            /// <summary>
            /// Table works as an ordered HashSet
            /// </summary>
            /// <param name="document"></param>
            override public Results Insert(int index, D document)
            {
                int i = base.IndexOf(document);
                if (i >= 0)
                {
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<D>)this).RemoveAt(i);
                    ((List<D>)this).Insert(index, document);
                    log_writer.WriteLine("replaced: " + i);
                    return Results.MOVED;
                }
                else
                {
                    set_new_id(document);
                    file_writer.WriteLine(JsonConvert.SerializeObject(document, Formatting.None));
                    ((List<D>)this).Insert(index, document);
                    log_writer.WriteLine("inserted: " + index);
                    return Results.INSERTED;
                }
            }
        }
    }
}