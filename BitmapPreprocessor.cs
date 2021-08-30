//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    public abstract class BitmapPreprocessor
    {
        public abstract Bitmap GetProcessed(Bitmap bitmap);

        public static Type CompileBitmapPreprocessorType(string bitmapPreprocessorClassDefinition)
        {
            Type t = Compiler.FindSubTypes(typeof(BitmapPreprocessor), Compiler.Compile(bitmapPreprocessorClassDefinition, Assembly.GetExecutingAssembly())).FirstOrDefault();
            if (t == null)
                throw new Exception("No sub-type of '" + typeof(BitmapPreprocessor).Name + "' was found in the hot-compiled type definition.");
            return t;
        }

        public static BitmapPreprocessor CreateBitmapPreprocessor(Template template)
        {
            Log.Inform("Compiling '" + template.Name + "' DocumentParser...");
            Type bitmapPreprocessorType = CompileBitmapPreprocessorType(template.BitmapPreprocessorClassDefinition);
            //object obj = Activator.CreateInstance(type);
            //var f =  type.InvokeMember("test",
            //      BindingFlags.Default | BindingFlags.InvokeMethod,
            //      null,
            //      obj,
            //      new object[] { "test" });
            return (BitmapPreprocessor)Activator.CreateInstance(bitmapPreprocessorType);
        }
    }
}