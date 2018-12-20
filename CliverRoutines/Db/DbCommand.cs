//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

namespace Cliver.Db
{
    public abstract class Command
    {
        public readonly string Sql;
        protected readonly DbConnection dc;

        protected Command(string sql, DbConnection db_connection)
        {
            this.dc = db_connection;
            this.Sql = sql;
        }

        public int Execute(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                return Execute_(key_value_pairs);
            }
        }
        abstract protected int Execute_(params object[] key_value_pairs);

        public object GetSingleValue(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                return GetSingleValue_(key_value_pairs);
            }
        }
        abstract protected object GetSingleValue_(params object[] key_value_pairs);

        public IDataReader GetReader(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                return GetReader_(key_value_pairs);
            }
        }
        abstract protected IDataReader GetReader_(params object[] key_value_pairs);

        public Record GetFirstRecord(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                return GetFirstRecord_(key_value_pairs);
            }
        }
        abstract protected Record GetFirstRecord_(params object[] key_value_pairs);

        public void Set(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                Set_(key_value_pairs);
            }
        }
        abstract protected void Set_(params object[] key_value_pairs);

        public Recordset GetRecordset(params object[] key_value_pairs)
        {
            lock (dc.NativeConnection)
            {
                return GetRecordset_(key_value_pairs);
            }
        }
        abstract protected Recordset GetRecordset_(params object[] key_value_pairs);

        public string GetCommandText()
        {
            lock (dc.NativeConnection)
            {
                return GetCommandText_();
            }
        }
        abstract protected string GetCommandText_();
    }

    public class MsSqlCommand : Command
    {
        SqlCommand c;

        internal MsSqlCommand(string sql, DbConnection db_connection)
            : base(sql, db_connection)
        {
            c = new SqlCommand(sql, (SqlConnection)dc.NativeConnection);
            c.Prepare();
        }

        override protected int Execute_(params object[] key_value_pairs)
        {
            int rc = 0;
            try
            {
                Set(key_value_pairs);
                rc = c.ExecuteNonQuery();                
            }
            finally
            {
                c.Parameters.Clear();
            }
            return rc;
        }

        override protected object GetSingleValue_(params object[] key_value_pairs)
        {
            object o = null;
            try
            {
                Set(key_value_pairs);
                o = c.ExecuteScalar();
            }
            finally
            {
                c.Parameters.Clear();
            }
            return o;
        }

        override protected IDataReader GetReader_(params object[] key_value_pairs)
        {
            SqlDataReader r = null;
            try
            {
                this.Set(key_value_pairs);
                r = c.ExecuteReader();
            }
            finally
            {
                c.Parameters.Clear();
            }
            return r;
        }

        override protected Record GetFirstRecord_(params object[] key_value_pairs)
        {
            Record a = null;
            SqlDataReader r = null;
            try
            {
                this.Set(key_value_pairs);
                r = c.ExecuteReader();
            }
            finally
            {
                c.Parameters.Clear();
            }
            try
            {
                if (r.Read())
                {
                    a = new Record();
                    //object[] os = new object[r.FieldCount];
                    //int t = r.GetValues(os);
                    for (int i = 0; i < r.FieldCount; i++)
                        a[r.GetName(i)] = r.IsDBNull(i) ? null : r[i];
                }
            }
            finally
            {
                r.Close();
            }
            return a;
        }

        override protected void Set_(params object[] key_value_pairs)
        {
            if (key_value_pairs == null)
                return;
            for (int i = 0; i < key_value_pairs.Length; i += 2)
            {
                if (key_value_pairs[i + 1] == null)
                    key_value_pairs[i + 1] = DBNull.Value;
                c.Parameters.AddWithValue((string)key_value_pairs[i], key_value_pairs[i + 1]);
            }
        }

        override protected Recordset GetRecordset_(params object[] key_value_pairs)
        {
            this.Set(key_value_pairs);
            Recordset rs = new Recordset(c.ExecuteReader());
            return rs;
        }

        override protected string GetCommandText_()
        {
            return c.CommandText;
        }
    }

    public class Record : Dictionary<string, object>
    {
        public new object this[string name]
        {
            get
            {
                object o;
                if (this.TryGetValue(name, out o))
                    return o;
                return null;                
            }
            set
            {
                this.Add(name, value);
            }
        }
    }

    public class Recordset : IEnumerable<Record>
    {
        public Recordset(IDataReader dr)
        {
            while (dr.Read())
            {
                Record r = new Record();
                for (int i = 0; i < dr.FieldCount; i++)
                    r[dr.GetName(i)] = dr.IsDBNull(i) ? null : dr[i];
                rows.Add(r);
            }
            dr.Close();
        }

        List<Record> rows = new List<Record>();

        public Record this[int row_index]
        {
            get
            {
                return rows[row_index];
            }
        }

        protected void Add(Record record)
        {
            rows.Add(record);
        }

        public IEnumerator<Record> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return rows.Count;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
    //    /// <summary>
    //    /// Thread-safe OdbcCommand implementation
    //    /// </summary>
    //    public class Odbc
    //    {
    //        static object static_lock_variable = new object();
    //        string sql;
    //        OdbcCommand c;
    //        DbConnection db_connection;

    //        static Hashtable sql2commands = new Hashtable();

    //        public Odbc(string sql, DbConnection db_connection)
    //        {
    //            _Odbc(sql, db_connection);
    //        }

    //        public Odbc(string sql)
    //        {
    //            _Odbc(sql, DbConnection.This);
    //        }

    //        void _Odbc(string sql, DbConnection db_connection)
    //        {
    //            lock (db_connection)
    //            {
    //                this.sql = sql;

    //                OdbcCommand _oc = (OdbcCommand)sql2commands[sql];
    //                if (_oc == null)
    //                {
    //                    _oc = new OdbcCommand(sql, db_connection.Odbc);
    //                    _oc.Prepare();
    //                    sql2commands[sql] = _oc;
    //                }
    //                this.c = _oc;
    //                this.db_connection = db_connection;
    //            }
    //        }

    //        public int ExecuteNonQuery()
    //        {
    //            return ExecuteNonQuery(null);
    //        }

    //        public int ExecuteNonQuery(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                AddWithValues(values);
    //                int rc = c.ExecuteNonQuery();
    //                c.Parameters.Clear();
    //                return rc;
    //            }
    //        }

    //        public object ExecuteScalar()
    //        {
    //            return ExecuteScalar(null);
    //        }

    //        public object ExecuteScalar(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                object o = null;
    //                AddWithValues(values);
    //                o = c.ExecuteScalar();
    //                c.Parameters.Clear();
    //                return o;
    //            }
    //        }

    //        public OdbcDataReader ExecuteReader()
    //        {
    //            return ExecuteReader(null);
    //        }

    //        public OdbcDataReader ExecuteReader(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                this.AddWithValues(values);
    //                OdbcDataReader r = c.ExecuteReader();
    //                c.Parameters.Clear();
    //                return r;
    //            }
    //        }

    //        static public string GetString(OdbcDataReader ord, string column_name)
    //        {
    //            lock (ord)
    //            {
    //                object o = ord.GetValue(ord.GetOrdinal(column_name));
    //                if (o == System.DBNull.Value)
    //                    return null;
    //                return o.ToString();
    //            }
    //        }

    //        public Hashtable GetFirstRecord()
    //        {
    //            return GetFirstRecord(null);
    //        }

    //        public Hashtable GetFirstRecord(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                Hashtable a = null;
    //                OdbcDataReader r = null;
    //                this.AddWithValues(values);
    //                r = c.ExecuteReader();
    //                c.Parameters.Clear();
    //                if (r.Read())
    //                {
    //                    a = new Hashtable();
    //                    //object[] os = new object[r.FieldCount];
    //                    //int t = r.GetValues(os);
    //                    for (int i = 0; i < r.FieldCount; i++)
    //                        a[r.GetName(i)] = r.IsDBNull(i) ? null : r[i];
    //                }
    //                r.Close();
    //                return a;
    //            }
    //        }

    //        public void AddWithValue(string name, object value)
    //        {
    //            lock (db_connection)
    //            {
    //                c.Parameters.AddWithValue(name, value);
    //            }
    //        }

    //        public void AddWithValues(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                if (values == null)
    //                    return;
    //                foreach (object o in values)
    //                    c.Parameters.AddWithValue("", o);
    //            }
    //        }

    //        public void AddWithPreparedValues(params string[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                if (values == null)
    //                    return;
    //                foreach (string o in values)
    //                    if (o != null)
    //                        c.Parameters.AddWithValue("", (object)FileWriter.PrepareField(o));
    //                    else
    //                        //c.Parameters.AddWithValue("", o);
    //                        c.Parameters.AddWithValue("", "");
    //            }
    //        }

    //        public void ExecuteExecuteNonQueryWithPreparedValues(params string[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                AddWithPreparedValues(values);
    //                this.ExecuteNonQuery();
    //            }
    //        }

    //        ~Odbc()
    //        {
    //            try
    //            {
    //            }
    //            catch
    //            {
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Thread-safe OleDbCommand implementation
    //    /// </summary>
    //    public class OleDb
    //    {
    //        static object static_lock_variable = new object();
    //        string sql;
    //        OleDbCommand c;
    //        DbConnection db_connection;

    //        static Hashtable sql2commands = new Hashtable();

    //        public OleDb(string sql, DbConnection db_connection)
    //        {
    //            _OleDb(sql, db_connection);
    //        }

    //        public OleDb(string sql)
    //        {
    //            _OleDb(sql, DbConnection.This);
    //        }

    //        void _OleDb(string sql, DbConnection db_connection)
    //        {
    //            lock (db_connection)
    //            {
    //                this.sql = sql;

    //                OleDbCommand _oc = (OleDbCommand)sql2commands[sql];
    //                if (_oc == null)
    //                {
    //                    _oc = new OleDbCommand(sql, db_connection.OleDb);
    //                    _oc.Prepare();
    //                    sql2commands[sql] = _oc;
    //                }
    //                this.c = _oc;
    //                this.db_connection = db_connection;
    //            }
    //        }

    //        public int ExecuteNonQuery()
    //        {
    //            return ExecuteNonQuery(null);
    //        }

    //        public int ExecuteNonQuery(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                AddWithValues(values);
    //                int rc = c.ExecuteNonQuery();
    //                c.Parameters.Clear();
    //                return rc;
    //            }
    //        }

    //        public object ExecuteScalar()
    //        {
    //            return ExecuteScalar(null);
    //        }

    //        public object ExecuteScalar(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                object o = null;
    //                AddWithValues(values);
    //                o = c.ExecuteScalar();
    //                c.Parameters.Clear();
    //                return o;
    //            }
    //        }

    //        public OleDbDataReader ExecuteReader()
    //        {
    //            return ExecuteReader(null);
    //        }

    //        public OleDbDataReader ExecuteReader(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                this.AddWithValues(values);
    //                OleDbDataReader r = c.ExecuteReader();
    //                c.Parameters.Clear();
    //                return r;
    //            }
    //        }

    //        static public string GetString(OleDbDataReader ord, string column_name)
    //        {
    //            lock (ord)
    //            {
    //                object o = ord.GetValue(ord.GetOrdinal(column_name));
    //                if (o == System.DBNull.Value)
    //                    return null;
    //                return o.ToString();
    //            }
    //        }

    //        public Hashtable GetFirstRecord()
    //        {
    //            return GetFirstRecord(null);
    //        }

    //        public Hashtable GetFirstRecord(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                Hashtable a = null;
    //                OleDbDataReader r = null;
    //                this.AddWithValues(values);
    //                r = c.ExecuteReader();
    //                if (r.Read())
    //                {
    //                    a = new Hashtable();
    //                    for (int i = 0; i < r.FieldCount; i++)
    //                    {
    //                        if (r.IsDBNull(i))
    //                            a[r.GetName(i)] = null;
    //                        else
    //                            a[r.GetName(i)] = r.GetValue(i);
    //                    }
    //                }
    //                r.Close();
    //                return a;
    //            }
    //        }

    //        public void AddWithValue(string name, object value)
    //        {
    //            lock (db_connection)
    //            {
    //                c.Parameters.AddWithValue(name, value);
    //            }
    //        }

    //        public void AddWithValues(params object[] values)
    //        {
    //            lock (db_connection)
    //            {
    //                if (values == null)
    //                    return;
    //                foreach (object o in values)
    //                    c.Parameters.AddWithValue("", o);
    //            }
    //        }

    //        //public void AddWithPreparedValues(params object[] values)
    //        //{
    //        //    lock (db_connection)
    //        //    {
    //        //        if (values == null)
    //        //            return;
    //        //        foreach (object o in values)
    //        //            if (o != null)
    //        //                c.Parameters.AddWithValue("", (object)FileWriter.PrepareField(o.ToString(), null));
    //        //            else
    //        //                //c.Parameters.AddWithValue("", o);
    //        //                c.Parameters.AddWithValue("", "");
    //        //    }
    //        //}

    //        //public void ExecuteExecuteNonQueryWithPreparedValues(params object[] values)
    //        //{
    //        //    lock (db_connection)
    //        //    {
    //        //        AddWithPreparedValues(values);
    //        //        this.ExecuteNonQuery();
    //        //    }
    //        //}

    //        ~OleDb()
    //        {
    //            try
    //            {
    //            }
    //            catch
    //            {
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Thread-safe MySqlCommand implementation
    //    /// </summary>
    //    //public class MySql
    //    //{
    //    //    static object static_lock_variable = new object();
    //    //    string sql;
    //    //    MySqlCommand c;
    //    //    DbConnection db_connection;

    //    //    static Hashtable sql2commands = new Hashtable();

    //    //    public MySql(string sql, DbConnection db_connection)
    //    //    {
    //    //        _MySql(sql, db_connection);
    //    //    }

    //    //    public MySql(string sql)
    //    //    {
    //    //        _MySql(sql, DbConnection.This);
    //    //    }

    //    //    void _MySql(string sql, DbConnection db_connection)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            this.sql = sql;

    //    //            MySqlCommand _oc = (MySqlCommand)sql2commands[sql];
    //    //            if (_oc == null)
    //    //            {
    //    //                _oc = new MySqlCommand(sql, db_connection.MySql);
    //    //                _oc.Prepare();
    //    //                sql2commands[sql] = _oc;
    //    //            }
    //    //            this.c = _oc;
    //    //            this.db_connection = db_connection;
    //    //        }
    //    //    }

    //    //    public int ExecuteNonQuery()
    //    //    {
    //    //        return ExecuteNonQuery(null);
    //    //    }

    //    //    public int ExecuteNonQuery(params object[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            AddWithValues(name_value_pairs);
    //    //            int rc = c.ExecuteNonQuery();
    //    //            c.Parameters.Clear();
    //    //            return rc;
    //    //        }
    //    //    }

    //    //    public object ExecuteScalar()
    //    //    {
    //    //        return ExecuteScalar(null);
    //    //    }

    //    //    public object ExecuteScalar(params object[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            object o = null;
    //    //            AddWithValues(name_value_pairs);
    //    //            o = c.ExecuteScalar();
    //    //            c.Parameters.Clear();
    //    //            return o;
    //    //        }
    //    //    }

    //    //    public MySqlDataReader ExecuteReader()
    //    //    {
    //    //        return ExecuteReader(null);
    //    //    }

    //    //    public MySqlDataReader ExecuteReader(params object[] values)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            this.AddWithValues(values);
    //    //            MySqlDataReader r = c.ExecuteReader();
    //    //            c.Parameters.Clear();
    //    //            return r;
    //    //        }
    //    //    }

    //    //    public Recordset ExecuteRecordset()
    //    //    {
    //    //        return ExecuteRecordset(null);
    //    //    }

    //    //    public Recordset ExecuteRecordset(params object[] values)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            this.AddWithValues(values);
    //    //            Recordset rs = new Recordset(c.ExecuteReader());
    //    //            return rs;
    //    //        }
    //    //    }

    //    //    static public string GetString(MySqlDataReader ord, string column_name)
    //    //    {
    //    //        lock (ord)
    //    //        {
    //    //            object o = ord.GetValue(ord.GetOrdinal(column_name));
    //    //            if (o == System.DBNull.Value)
    //    //                return null;
    //    //            return o.ToString();
    //    //        }
    //    //    }

    //    //    public Hashtable GetFirstRecord()
    //    //    {
    //    //        return GetFirstRecord(null);
    //    //    }

    //    //    public Hashtable GetFirstRecord(params object[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            Hashtable a = null;
    //    //            MySqlDataReader r = null;
    //    //            this.AddWithValues(name_value_pairs);
    //    //            r = c.ExecuteReader();
    //    //            c.Parameters.Clear();
    //    //            if (r.Read())
    //    //            {
    //    //                a = new Hashtable();
    //    //                for (int i = 0; i < r.FieldCount; i++)
    //    //                    a[r.GetName(i)] = r.IsDBNull(i) ? null : r[i];
    //    //            }
    //    //            r.Close();
    //    //            return a;
    //    //        }
    //    //    }

    //    //    public void AddWithValue(string name, object value)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            c.Parameters.AddWithValue(name, value);
    //    //        }
    //    //    }

    //    //    public void AddWithValues(params object[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            if (name_value_pairs == null)
    //    //                return;
    //    //            for (int i = 0; i < name_value_pairs.Length; i += 2)
    //    //                c.Parameters.AddWithValue((string)name_value_pairs[i], name_value_pairs[i + 1]);
    //    //        }
    //    //    }

    //    //    public void AddWithPreparedValues(params string[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            if (name_value_pairs == null)
    //    //                return;
    //    //            for (int i = 1; i < name_value_pairs.Length; i += 2)
    //    //                name_value_pairs[i] = FileWriter.PrepareField(name_value_pairs[i]);
    //    //            AddWithValues(name_value_pairs);
    //    //        }
    //    //    }

    //    //    public void ExecuteExecuteNonQueryWithPreparedValues(params string[] name_value_pairs)
    //    //    {
    //    //        lock (db_connection)
    //    //        {
    //    //            AddWithPreparedValues(name_value_pairs);
    //    //            this.ExecuteNonQuery();
    //    //        }
    //    //    }

    //    //    ~MySql()
    //    //    {
    //    //        try
    //    //        {
    //    //        }
    //    //        catch
    //    //        {
    //    //        }
    //    //    }
    //    //}

