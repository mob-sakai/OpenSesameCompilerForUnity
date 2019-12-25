using CommandLine;

namespace OpenSesameCompiler
{
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            int exitCode = 0;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opt => exitCode = Compiler.Compile(opt));
            return exitCode;
        }
    }
}