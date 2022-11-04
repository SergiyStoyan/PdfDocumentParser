////********************************************************************************************
////Author: Sergiy Stoyan
////        s.y.stoyan@gmail.com
////        http://www.cliversoft.com
////********************************************************************************************
//using System;
//using System.Reflection;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;

//namespace Cliver
//{
//    /// <summary>
//    /// Write to a file. Thread-safe.
//    /// </summary>
//    public partial class FileWriter : IDisposable
//    {
//        public void Dispose()
//        {
//            lock (this)
//            {
//                if (tw != null)
//                {
//                    tw.Close();
//                    tw = null;
//                }
//            }
//        }

//        public FileWriter(string file, bool append = true, Encoding encoding = null)
//        {
//            if (encoding == null)
//                encoding = Encoding.Unicode;
//            File = file;
//            FileSystemRoutines.CreateDirectory(PathRoutines.GetFileDir(file));
//            tw = new StreamWriter(file, append, encoding);//System.Text.Encoding.GetEncoding("ISO-8859-1")
//        }
//        TextWriter tw = null;

//        /// <summary>
//        /// Absolute path of the file.
//        /// </summary>
//        public string File { get; private set; }

//        public void Write(string str)
//        {
//            lock (this)
//            {
//                tw.Write(str);
//                tw.Flush();
//            }
//        }

//        /// <summary>
//        /// Strip string from new lines and write it to file
//        /// </summary>
//        /// <param name="str">string</param>
//        public void WriteLine(string str)
//        {
//            lock (this)
//            {
//                if (str == null)
//                    return;
//                str = Regex.Replace(str, "[\r\n]+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
//                Write(str + "\r\n");
//            }
//        }

//        public class Tsv<DocumentT> : FileWriter where DocumentT : new()
//        {
//            public Tsv(string file, OrderedDictionary headerKeys2header, bool append = true, Encoding encoding = null) : base(file, append, encoding)
//            {
//                headers = new string[headerKeys2header.Count];
//                headerKeys2header.Keys.CopyTo(headers, 0);
//                WriteLine(FieldPreparation.Tsv.GetLine(headerKeys2header.Values, false));
//            }
//            readonly string[] headers = null;

//            public Tsv(string file, bool append = true, params string[] headers) : base(file, append)
//            {
//                //if (this.headers != null)
//                //    throw new Exception("Headers were already recorded to the file.");
//                this.headers = headers;
//                WriteLine(FieldPreparation.Tsv.GetLine(headers, false));
//            }

//            public Tsv(string file, bool append = true, Encoding encoding = null) : base(file, append, encoding)
//            {
//                pis = typeof(DocumentT).GetProperties();
//                List<string> hs = new List<string>();
//                foreach (PropertyInfo pi in pis)
//                {
//                    if (pi.GetCustomAttribute<FieldPreparation.IgnoredField>() != null)
//                        continue;
//                    hs.Add(pi.Name);
//                }
//                headers = hs.ToArray();
//                WriteLine(FieldPreparation.Tsv.GetLine(headers, false));
//            }
//            readonly PropertyInfo[] pis;

//            public void WriteLine(DocumentT document)
//            {
//                lock (this)
//                {
//                    List<string> values = new List<string>();
//                    foreach (string h in headers)
//                    {
//                        PropertyInfo pi = pis.First(a => a.Name == h);
//                        if (pi == null)
//                            throw new Exception("Header '" + h + "' is absent in the object:\r\n" + document.ToStringByJson());
//                        string s;

//                        object p = pi.GetValue(document);
//                        if (pi.PropertyType == typeof(string))
//                            s = (string)p;
//                        else
//                            s = p?.ToString();
//                        values.Add(s);
//                    }
//                    base.WriteLine(FieldPreparation.Tsv.GetLine(values, false));
//                }
//            }

//            //public void WriteLine(IEnumerable<object> values)
//            //{
//            //    lock (this)
//            //    {
//            //        base.WriteLine(FieldPreparation.Tsv.GetLine(values, false));
//            //    }
//            //}

//            //public void WriteLine(params string[] values)
//            //{
//            //    lock (this)
//            //    {
//            //        base.WriteLine(FieldPreparation.Tsv.GetLine(values, false));
//            //    }
//            //}
//        }

//        public class Json<DocumentT> : FileWriter where DocumentT : new()
//        {
//            public Json(string file, bool append = true, Encoding encoding = null) : base(file, append, encoding)
//            {
//            }

//            public void WriteLine(DocumentT document)
//            {
//                lock (this)
//                {
//                    WriteLine(Serialization.Json.Serialize(document, false));
//                }
//            }
//        }
//    }
//}
