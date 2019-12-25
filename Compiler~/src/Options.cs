using CommandLine;
using CommandLine.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace OpenSesameCompiler
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Output path.
        /// </summary>
        [Option('o', "out", Required = false, HelpText = "Output path.")]
        public string Out { get; set; }

        /// <summary>
        /// Logfile path.
        /// </summary>
        [Option('l', "logfile", Required = false, Default = "compile.log", HelpText = "Logfile path")]
        public string Logfile { get; set; }

        /// <summary>
        /// Input source code path (*.cs)
        /// </summary>
        [Option('s', "source", Required = false, Separator = ',', HelpText = "Input source code path (*.cs) separated by comma")]
        public IEnumerable<string> InputPaths { get; set; }

        /// <summary>
        /// Referenced assembly paths
        /// </summary>
        [Option('r', "reference", Required = false, Separator = ',', HelpText = "Referenced assemblies separated by comma")]
        public IEnumerable<string> References { get; set; }

        /// <summary>
        /// Define symbols.
        /// </summary>
        [Option('d', "define", Required = false, Separator = ',', HelpText = "Define symbols separated by comma")]
        public IEnumerable<string> Defines { get; set; }

        /// <summary>
        /// Allow unsafe code.
        /// </summary>
        [Option("unsafe", Required = false, Default = false, HelpText = "Allow unsafe code")]
        public bool Unsafe { get; set; }

        /// <summary>
        /// Optimize compile.
        /// </summary>
        [Option("optimize", Required = false, Default = false, HelpText = "Optimize compile")]
        public bool Optimize { get; set; }

        /// <summary>
        /// Target output kind.
        /// </summary>
        [Option('t', "target", Required = false, Default = OutputKind.DynamicallyLinkedLibrary, HelpText = "Target output kind. <DynamicallyLinkedLibrary|ConsoleApplication|NetModule>")]
        public OutputKind Target { get; set; }

        /// <summary>
        /// C# language version.
        /// </summary>
        [Option("langage", Required = false, Default = LanguageVersion.Latest, HelpText = "C# language version. <CSharp1|...|CSharp7|CSharp7_1|CSharp7_2|CSharp7_3|CSharp8|Latest>")]
        public LanguageVersion LanguageVersion { get; set; }

        /// <summary>
        /// Response file
        /// </summary>
        [Value(1, Required = false, HelpText = "Response file")]
        public string ResponseFile { get; set; }

        /// <summary>
        /// Usages.
        /// </summary>
        [Usage(ApplicationAlias = "OpenSesameCompiler")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("To compile", new Options { Out = "your.dll", InputPaths = new []{"scriptA.cs", "scriptB.cs" }, References = new []{"mscorlib.dll", "external.dll" }, Defines = new []{"DEBUG", "TRACE" }}),
                    new Example("To compile with response file", new Options { Out = "your.dll", ResponseFile = "responseFile" }),
                };
            }
        }
    }
}