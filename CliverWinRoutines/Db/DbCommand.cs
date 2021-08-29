////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        sergey.stoyan@hotmail.com
////        http://www.cliversoft.com
////********************************************************************************************

//using System;
//using System.Data;
//using System.Collections.Generic;
//using System.Data.SqlClient;
////using MySql.Data.MySqlClient;

//namespace Cliver.Db
//{
//    public abstract class Command
//    {
//        public readonly string Sql;
//        protected readonly DbConnection dc;

//        protected Command(string sql, DbConnection db_connection)
//        {
//            this.dc = db_connection;
//            this.Sql = sql;
//        }

//        public int Execute(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                return Execute_(key_value_pairs);
//            }
//        }
//        abstract protected int Execute_(params object[] key_value_pairs);

//        public object GetSingleValue(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                return GetSingleValue_(key_value_pairs);
//            }
//        }
//        abstract protected object GetSingleValue_(params object[] key_value_pairs);

//        public IDataReader GetReader(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                return GetReader_(key_value_pairs);
//            }
//        }
//        abstract protected IDataReader GetReader_(params object[] key_value_pairs);

//        public Record GetFirstRecord(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                return GetFirstRecord_(key_value_pairs);
//            }
//        }
//        abstract protected Record GetFirstRecord_(params object[] key_value_pairs);

//        public void Set(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                Set_(key_value_pairs);
//            }
//        }
//        abstract protected void Set_(params object[] key_value_pairs);

//        public Recordset GetRecordset(params object[] key_value_pairs)
//        {
//            lock (dc.NativeConnection)
//            {
//                return GetRecordset_(key_value_pairs);
//            }
//        }
//        abstract protected Recordset GetRecordset_(params object[] key_value_pairs);

//        public string GetCommandText()
//        {
//            lock (dc.NativeConnection)
//            {
//                return GetCommandText_();
//            }
//        }
//        abstract protected string GetCommandText_();
//    }

//    public class MsSqlCommand : Command
//    {
//        SqlCommand c;

//        internal MsSqlCommand(string sql, DbConnection db_connection)
//            : base(sql, db_connection)
//        {
//            c = new SqlCommand(sql, (SqlConnection)dc.NativeConnection);
//            c.Prepare();
//        }

//        override protected int Execute_(params object[] key_value_pairs)
//        {
//            int rc = 0;
//            try
//            {
//                Set(key_value_pairs);
//                rc = c.ExecuteNonQuery();                
//            }
//            finally
//            {
//                c.Parameters.Clear();
//            }
//            return rc;
//        }

//        override protected object GetSingleValue_(params object[] key_value_pairs)
//        {
//            object o = null;
//            try
//            {
//                Set(key_value_pairs);
//                o = c.ExecuteScalar();
//            }
//            finally
//            {
//                c.Parameters.Clear();
//            }
//            return o;
//        }

//        override protected IDataReader GetReader_(params object[] key_value_pairs)
//        {
//            SqlDataReader r = null;
//            try
//            {
//                this.Set(key_value_pairs);
//                r = c.ExecuteReader();
//            }
//            finally
//            {
//                c.Parameters.Clear();
//            }
//            return r;
//        }

//        override protected Record GetFirstRecord_(params object[] key_value_pairs)
//        {
//            Record a = null;
//            SqlDataReader r = null;
//            try
//            {
//                this.Set(key_value_pairs);
//                r = c.ExecuteReader();
//            }
//            finally
//            {
//                c.Parameters.Clear();
//            }
//            try
//            {
//                if (r.Read())
//                {
//                    a = new Record();
//                    //object[] os = new object[r.FieldCount];
//                    //int t = r.GetValues(os);
//                    for (int i = 0; i < r.FieldCount; i++)
//                        a[r.GetName(i)] = r.IsDBNull(i) ? null : r[i];
//                }
//            }
//            finally
//            {
//                r.Close();
//            }
//            return a;
//        }

//        override protected void Set_(params object[] key_value_pairs)
//        {
//            if (key_value_pairs == null)
//                return;
//            for (int i = 0; i < key_value_pairs.Length; i += 2)
//            {
//                if (key_value_pairs[i + 1] == null)
//                    key_value_pairs[i + 1] = DBNull.Value;
//                c.Parameters.AddWithValue((string)key_value_pairs[i], key_value_pairs[i + 1]);
//            }
//        }

//        override protected Recordset GetRecordset_(params object[] key_value_pairs)
//        {
//            this.Set(key_value_pairs);
//            Recordset rs = new Recordset(c.ExecuteReader());
//            return rs;
//        }

//        override protected string GetCommandText_()
//        {
//            return c.CommandText;
//        }
//    }

//    public class Record : Dictionary<string, object>
//    {
//        public new object this[string name]
//        {
//            get
//            {
//                object o;
//                if (this.TryGetValue(name, out o))
//                    return o;
//                return null;                
//            }
//            set
//            {
//                this.Add(name, value);
//            }
//        }
//    }

//    public class Recordset : IEnumerable<Record>
//    {
//        public Recordset(IDataReader dr)
//        {
//            while (dr.Read())
//            {
//                Record r = new Record();
//                for (int i = 0; i < dr.FieldCount; i++)
//                    r[dr.GetName(i)] = dr.IsDBNull(i) ? null : dr[i];
//                rows.Add(r);
//            }
//            dr.Close();
//        }

//        List<Record> rows = new List<Record>();

//        public Record this[int row_index]
//        {
//            get
//            {
//                return rows[row_index];
//            }
//        }

//        protected void Add(Record record)
//        {
//            rows.Add(record);
//        }

//        public IEnumerator<Record> GetEnumerator()
//        {
//            return rows.GetEnumerator();
//        }

//        public int Count
//        {
//            get
//            {
//                return rows.Count;
//            }
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }
//    }
//}

