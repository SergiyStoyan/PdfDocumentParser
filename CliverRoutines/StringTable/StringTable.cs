//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

//#define USE_HEADER_HASH

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;

namespace Cliver
{
    public abstract partial class StringTable
    {
        public enum ReadingMode
        {
            NULL = 0,
            IgnoreEmptyRows = 1,
            HeadersNumberEqualsColumnsNumber = 2,
            CellsNumberEqualsColumnsNumber = 4,
        }

        protected abstract List<string> getRowValues(string line);

        public void Read(StreamReader streamReader, ReadingMode mode = ReadingMode.IgnoreEmptyRows)
        {
            read(mode, () => { return getRowValues(streamReader.ReadLine()); });
        }

        void read(ReadingMode mode, Func<List<string>> getRowValues)
        {
            Headers = getRowValues();
            if (Headers == null)
                throw new Exception("There is no header.");
#if USE_HEADER_HASH
            for (int i = 0; i < Headers.Count; i++)
                Headers2I[Headers[i]] = i;
#endif

            Rows = new List<Row>();
            int lineNumber = 1;
            for (List<string> vs = getRowValues(); vs != null; vs = getRowValues())
            {
                lineNumber++;
                //if (vs.Count == 1 && string.IsNullOrEmpty(vs[0]) && Headers.Count > 1)
                if (mode.HasFlag(ReadingMode.IgnoreEmptyRows) && null == vs.Find(a => !string.IsNullOrEmpty(a)))
                    continue;
                if (vs.Count > Headers.Count)
                {
                    if (mode.HasFlag(ReadingMode.HeadersNumberEqualsColumnsNumber))
                        throw new Exception("The line " + lineNumber + " has more columns than headers: " + vs.Count + " > " + Headers.Count);
                }
                else if (vs.Count < Headers.Count)
                {
                    if (mode.HasFlag(ReadingMode.CellsNumberEqualsColumnsNumber))
                        throw new Exception("The line " + lineNumber + " has less columns than headers: " + vs.Count + " < " + Headers.Count);
                }
                Rows.Add(new Row(lineNumber, vs, Rows.Count + 1, this));
            }
            ColumnCount = Rows.Select(a => a.Values.Count).Max();
        }

        public List<string> Headers { get; private set; }

#if USE_HEADER_HASH
        readonly internal Dictionary<string, int> Headers2I = new Dictionary<string, int>();
#endif

        public List<Row> Rows { get; private set; }

        public int ColumnCount { get; private set; } = 0;

        public int RowCount { get { return Rows.Count; } }

        public string this[int y, string header]
        {
            get
            {
                return Rows[y][header];
            }
        }

        public string this[int y, int x]
        {
            get
            {
                return Rows[y - 1][x];
            }
        }

        public Row this[int y]
        {
            get
            {
                return Rows[y - 1];
            }
        }

        public class Row
        {
            public string this[int x]
            {
                get
                {
                    if (x > Values.Count)
                    {
                        if (x > table.ColumnCount)
                            throw new Exception("X is out of the column count: " + x + " > " + table.ColumnCount);
                        return null;
                    }
                    return Values[x - 1];
                }
            }

            public string this[string header]
            {
                get
                {
#if USE_HEADER_HASH
                    if (!table.Headers2I.TryGetValue(header, out int x0))
                        throw new Exception("No such header: '" + header + "'");
#else
                    int x0 = table.Headers.IndexOf(header);
                    if (x0 < 0)
                        throw new Exception("No such header: '" + header + "'");
#endif
                    return this[x0 + 1];
                }
            }

            public List<string> Values { get; private set; }

            /// <summary>
            /// 1-based number of the row in rows.
            /// </summary>
            public int Y { get; }

            /// <summary>
            /// 1-based number of line in the input stream.
            /// </summary>
            public int LineNumber { get; }

            internal Row(int lineNumber, List<string> values, int y, StringTable table)
            {
                LineNumber = lineNumber;
                Values = values;
                Y = y;
                this.table = table;
            }
            StringTable table;
        }

        protected abstract string getLine(Row row);

        public void Write(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(getLine(new Row(0, Headers, 0, this)));
            foreach (Row row in Rows)
                streamWriter.WriteLine(getLine(row));
            streamWriter.Flush();
        }
    }
}