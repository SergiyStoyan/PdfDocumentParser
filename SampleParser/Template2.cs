//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Linq;
using System.Drawing;
using System.Collections.Specialized;
using Cliver.PdfDocumentParser;
using System.Net;

namespace Cliver.SampleParser
{
    public class Template2
    {
        public class ConditionNames
        {
            public const string DocumentFirstPage = "FirstPageOfDocument";
            public const string DocumentLastPage = "LastPageOfDocument";
        }

        public class FieldNames
        {
            public const string InvoiceId = "InvoiceId";
            public const string TotalAmount = "TotalAmount";
            public const string ProductTable = "ProductTable";
            public const string ProductNames = "ProductNames";
            public const string ProductCosts = "ProductCosts";
        }

        public Template Template;

        public bool Active = true;
        public DateTime ModifiedTime;
        public string Comment;
        public float OrderWeight = 1f;
        public Regex FileFilterRegex = null;

        public string GetModifiedTimeAsString()
        {
            return ModifiedTime.ToString("yy-MM-dd HH:mm:ss");
        }
    }
}