//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
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
            return Compiler.FindFirstSubType("BitmapPreprocessor", Compiler.Compile(bitmapPreprocessorClassDefinition));
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