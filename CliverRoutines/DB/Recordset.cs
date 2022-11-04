//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************

//################################
// 
// (!!!) When using a certain database type include the respective implementation files into the project and add the required reference.
//
//################################
using System;
using System.Data;
using System.Collections.Generic;

namespace Cliver.Db
{
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