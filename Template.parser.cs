////********************************************************************************************
////Author: Sergiy Stoyan
////        systoyan@gmail.com
////        sergiy.stoyan@outlook.com
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
//        public Template.Field TableLines;

//        public string Get(Template.Field.Text f)
//        {
//            return Page.GetText("TableLines");
//        }

//        public List<string> Get(Template.Field.TextLines f)
//        {
//            return Page.GetTextLines("TableLines");
//        }

//        public List<Page.CharBox> Get(Template.Field.CharBoxs f)
//        {
//            return Page.GetCharBoxes("TableLines");
//        }

//        public System.Drawing.Bitmap GetImage(Template.Field f)
//        {
//            return Page.GetImage("TableLines");
//        }
//    }
//}