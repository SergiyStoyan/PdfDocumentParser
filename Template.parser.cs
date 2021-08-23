////********************************************************************************************
////Author: Sergey Stoyan
////        sergey.stoyan@gmail.com
////        sergey.stoyan@hotmail.com
////        http://www.cliversoft.com
////********************************************************************************************
//using System;
//using System.Collections.Generic;

//namespace Cliver.PdfDocumentParser
//{
//    //!!!TRIAL
//    public partial class Template
//    {
//        [Newtonsoft.Json.JsonIgnore]
//        public Type ParserType;

//        public ParserT GetParser<ParserT>(Page page) where ParserT : Parser
//        {
//            //parser = (Parser)Activator.CreateInstance(ParserType);
//            //parser.Page = page;
//            parser = new Parser { Page = page, Template = this };
//            parser.Page.PageCollection.ActiveTemplate = this;
//            return (ParserT)parser;
//        }
//        Parser parser;
//    }

//    public class Parser
//    {
//        public Page Page { get; set; }
//        public Template Template { get; internal set; }

//        //example field
//        public List<string> TableLines
//        {
//            get
//            {
//                return Page.GetTextLines("TableLines");
//            }
//        }
//    }
//}