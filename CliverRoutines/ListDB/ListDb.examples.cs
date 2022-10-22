/********************************************************************************************
        Author: Sergiy Stoyan
        systoyan@gmail.com
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
        class testDocument //: ListDb.Document
        {
            public string A = DateTime.Now.ToString() + "\r\n" + DateTime.Now.Ticks.ToString();
            public string B = "test";
            public long C = DateTime.Now.Ticks;
            public DateTime DocumentType = DateTime.Now;

            static public void Test()
            {
                ListDb.Table<testDocument> t = ListDb.Table<testDocument>.Get(Log.AppCompanyUserDataDir);
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

        class testIndexedDocument : ListDb.IndexedDocument
        {
            public string A = DateTime.Now.ToString() + "\r\n" + DateTime.Now.Ticks.ToString();
            public string B = "test";
            public long C = DateTime.Now.Ticks;
            public DateTime DocumentType = DateTime.Now;

            static public void Test()
            {
                ListDb.IndexedTable<testIndexedDocument> t = ListDb.IndexedTable<testIndexedDocument>.Get(Log.AppCompanyUserDataDir);
                var d = new testIndexedDocument();
                t.Save(d);
                long id = d.ID;
                t.Flush();
            }
        }
    }
}