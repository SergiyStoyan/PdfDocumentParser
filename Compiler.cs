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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// Tool for hot-compiling assembly from C# code string
    /// </summary>
    public class Compiler
    {
        [Serializable]
        public class Error
        {
            public int P1;
            public int P2;
            public string Message;
        }

        public class Exception : System.Exception
        {
            public Exception(List<Error> compilationErrors) : base(string.Join("\r\n", compilationErrors.Select(a => a.Message)))
            {
                for (int i = 0; i < compilationErrors.Count; i++)
                    Data[i] = compilationErrors[i];
            }
        }

        //static HashSet<string> assemblyPaths = new HashSet<string>
        //{
        //    typeof(object).Assembly.Location, //mscorlib
        //    //MetadataReference.CreateFromFile(typeof(DynamicObject).Assembly.Location), //System.Core
        //    //MetadataReference.CreateFromFile(typeof(RuntimeBinderException).Assembly.Location),//Microsoft.CSharp
        //    //MetadataReference.CreateFromFile(typeof(Action).Assembly.Location), //System.Runtime
        //    //MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
        //    //MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
        //    //MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location),
        //    typeof(System.Drawing.Size).Assembly.Location,
        //    typeof(System.Drawing.Point).Assembly.Location,
        //    typeof(System.Drawing.Bitmap).Assembly.Location,
        //    //MetadataReference.CreateFromFile(typeof(Cliver.DateTimeRoutines).Assembly.Location),
        //    typeof(Emgu.CV.Structure.Gray).Assembly.Location,
        //    typeof(Emgu.CV.IColor).Assembly.Location,
        //    typeof(Emgu.CV.CvInvoke).Assembly.Location,
        //    typeof(Emgu.CV.CvEnum.AdaptiveThresholdType).Assembly.Location,
        //    Assembly.GetExecutingAssembly().Location,
        //};
        //static List<MetadataReference> _references = new List<MetadataReference>();
        //static Compiler()
        //{
        //    //    foreach (string ap in assemblyPaths)
        //    //        references.Add(MetadataReference.CreateFromFile(ap));

        //    //foreach (AssemblyName an in typeof(Emgu.CV.Structure.Gray).Assembly.GetReferencedAssemblies())
        //    //    assemblyPaths.Add(Assembly.Load(an.FullName).Location);
        //    //references.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)); //netstandard
        //    //references.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location)); //netstandard
        //}

        //public static void Test()
        //{
        //    Stopwatch sw0 = new Stopwatch();
        //    sw0.Start();
        //    var f = references;
        //    sw0.Stop();
        //    Stopwatch sw1 = new Stopwatch();
        //    sw1.Start();
        //    var r = references;
        //    sw1.Stop();
        //    Stopwatch sw3 = new Stopwatch();
        //    sw3.Start();
        //    var y = references;
        //    sw3.Stop();
        //}
        static public List<MetadataReference> GetAllReferences()
        {
            List<MetadataReference> references = new List<MetadataReference>();
            Dictionary<string, Assembly> assemblNames2assemby = new Dictionary<string, Assembly>();
            getAllAssemblies(assemblNames2assemby, Assembly.GetCallingAssembly());
            foreach (Assembly a in assemblNames2assemby.Values)
                references.Add(MetadataReference.CreateFromFile(a.Location));
            return references;
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

        /// <summary>
        /// Compile 
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="references"></param>
        /// <returns></returns>
        public static Type[] Compile(string typesDefinition, IEnumerable<MetadataReference> references = null)
        {
            SyntaxTree st = SyntaxFactory.ParseSyntaxTree(typesDefinition);
            CSharpCompilation compilation = CSharpCompilation.Create("emitted.dll",//no file seems to be really created
                syntaxTrees: new SyntaxTree[] { st },
                references: references != null ? references : GetAllReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );
            Assembly assembly;
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    List<Error> compilationErrors = new List<Error>();
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                    foreach (Diagnostic diagnostic in failures)
                        compilationErrors.Add(new Error { Message = diagnostic.GetMessage(), P1 = diagnostic.Location.SourceSpan.Start, P2 = diagnostic.Location.SourceSpan.End });
                    throw new Exception(compilationErrors);
                }
                ms.Seek(0, SeekOrigin.Begin);
                assembly = Assembly.Load(ms.ToArray());
            }
            return assembly.GetTypes();
        }
        //static Dictionary<string, Type[]> compiledTypesDefinitions2Types = new Dictionary<string, Type[]>();

        public static IEnumerable<Type> FindSubTypes(string baseTypeName, Type[] types)
        {
            return types.Where(t => !t.IsAbstract && t.BaseType.FullName.Contains(baseTypeName));
        }

        public static IEnumerable<Type> FindSubTypes(Type baseType, Type[] types)
        {
            return types.Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t));
        }

        public static string RemoveComments(string typesDefinition)
        {
            if (typesDefinition == null)
                return null;
            typesDefinition = Regex.Replace(typesDefinition, @"^\s*//.*", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return Regex.Replace(typesDefinition, @"/\*.*?\*/", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //return Regex.Replace(typesDefinition, @"\s+", "", RegexOptions.Singleline);
        }
    }
}