//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    public abstract class BitmapPreprocessor
    {
        public virtual Bitmap GetProcessed(Bitmap bitmap)
        {
            using (bitmap)
            {
                return bitmap;
            }
        }

        //internal protected Template Template { get; set; }

        [Serializable]
        public class CompilationError
        {
            public int P1;
            public int P2;
            public string Message;
        }

        public class CompilationException : Exception
        {
            public CompilationException(List<CompilationError> compilationErrors) : base(string.Join("\r\n", compilationErrors.Select(a => a.Message)))
            {
                for (int i = 0; i < compilationErrors.Count; i++)
                    Data[i] = compilationErrors[i];
            }
        }

        static HashSet<string> assemblyPaths = new HashSet<string>
        {
            typeof(object).Assembly.Location, //mscorlib
            //MetadataReference.CreateFromFile(typeof(DynamicObject).Assembly.Location), //System.Core
            //MetadataReference.CreateFromFile(typeof(RuntimeBinderException).Assembly.Location),//Microsoft.CSharp
            //MetadataReference.CreateFromFile(typeof(Action).Assembly.Location), //System.Runtime
            //MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
            //MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            //MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location),
            typeof(System.Drawing.Size).Assembly.Location,
            typeof(System.Drawing.Point).Assembly.Location,
            typeof(System.Drawing.Bitmap).Assembly.Location,
            //MetadataReference.CreateFromFile(typeof(Cliver.DateTimeRoutines).Assembly.Location),
            typeof(Emgu.CV.Structure.Gray).Assembly.Location,
            typeof(Emgu.CV.IColor).Assembly.Location,
            typeof(Emgu.CV.CvInvoke).Assembly.Location,
            typeof(Emgu.CV.CvEnum.AdaptiveThresholdType).Assembly.Location,
            Assembly.GetExecutingAssembly().Location,
        };
        static List<MetadataReference> references = new List<MetadataReference>();
        static BitmapPreprocessor()
        {
            //    foreach (string ap in assemblyPaths)
            //        references.Add(MetadataReference.CreateFromFile(ap));

            //foreach (AssemblyName an in typeof(Emgu.CV.Structure.Gray).Assembly.GetReferencedAssemblies())
            //    assemblyPaths.Add(Assembly.Load(an.FullName).Location);
            //references.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)); //netstandard
            //references.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)); //netstandard

            Dictionary<string, Assembly> assemblNames2assemby = new Dictionary<string, Assembly>();
            getAllAssemblies(assemblNames2assemby, Assembly.GetExecutingAssembly());
            foreach (Assembly a in assemblNames2assemby.Values)
                references.Add(MetadataReference.CreateFromFile(a.Location));
        }
        static void getAllAssemblies(Dictionary<string, Assembly> assemblNames2assemby, Assembly assembly)
        {
            assemblNames2assemby[assembly.FullName] = assembly;
            foreach (AssemblyName an in assembly.GetReferencedAssemblies())
                if (!assemblNames2assemby.ContainsKey(an.FullName))
                {
                    Assembly a = Assembly.Load(an);
                    getAllAssemblies(assemblNames2assemby, a);
                }
        }

        public static Type GetCompiledBitmapPreprocessorType(string bitmapPreprocessorClassDefinition)
        {
            SyntaxTree st = SyntaxFactory.ParseSyntaxTree(bitmapPreprocessorClassDefinition);
            CSharpCompilation compilation = CSharpCompilation.Create("emitted.dll",//the file seems not to be really created
                syntaxTrees: new SyntaxTree[] { st },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );
            Assembly assembly;
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    List<CompilationError> compilationErrors = new List<CompilationError>();
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                    foreach (Diagnostic diagnostic in failures)
                        compilationErrors.Add(new CompilationError { Message = diagnostic.GetMessage(), P1 = diagnostic.Location.SourceSpan.Start, P2 = diagnostic.Location.SourceSpan.End });
                    throw new CompilationException(compilationErrors);
                }
                ms.Seek(0, SeekOrigin.Begin);
                assembly = Assembly.Load(ms.ToArray());
            }
            Type[] types = assembly.GetTypes();
            Type documentParserType = types.Where(t => !t.IsAbstract && t.BaseType.FullName.Contains("BitmapPreprocessor")).FirstOrDefault();
            if (documentParserType == null)
                throw new Exception("No BitmapPreprocessor subclass is found in the hot-compiled assembly.");
            return documentParserType;
        }

        public static BitmapPreprocessor CompileBitmapPreprocessor(Template template)
        {
            Log.Inform("Compiling '" + template.Name + "' DocumentParser...");
            Type bitmapPreprocessorType = GetCompiledBitmapPreprocessorType(template.BitmapPreprocessorClassDefinition);
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