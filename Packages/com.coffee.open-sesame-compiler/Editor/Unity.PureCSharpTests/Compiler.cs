using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditor.Utils;
using UnityEngine;

namespace Coffee.OpenSesameCompilers
{
    internal class OpenSesameCompiler : MicrosoftCSharpCompiler
    {
#if UNITY_2019_3_OR_NEWER
		public OpenSesameCompiler(ScriptAssembly scriptAssembly, EditorScriptCompilationOptions options, string tempOutputDirectory) : base(scriptAssembly, options, tempOutputDirectory)
        {
        }
#else
        ScriptAssembly scriptAssembly;

        public OpenSesameCompiler(ScriptAssembly scriptAssembly, MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
            this.scriptAssembly = scriptAssembly;
        }
#endif

        protected override Program StartCompiler()
        {
            // Kill previous process.
            var p = base.StartCompiler();
            p.Kill();

#if UNITY_2019_3_OR_NEWER
            var responsefile = assembly.GeneratedResponseFile;
            var outputPath = assembly.FullPath;
#else
            // Get last responsefile.
            var outopt = string.Format("/out:\"{0}\"", m_Island._output);
            var responsefile = Directory.GetFiles("Temp", "UnityTempFile*")
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .First(path => File.ReadAllLines(path).Any(line => line.Contains(outopt)));
            var outputPath = scriptAssembly.FullPath;
#endif

            // Start compiling with dotnet app
            const string compiler = "Packages/com.coffee.open-sesame-compiler/Compiler~";
            var psi = new ProcessStartInfo()
            {
                Arguments = string.Format("run -p {0} -- -l {1}.log {1}", compiler, responsefile),
                FileName = "dotnet",
                CreateNoWindow = true
            };

            // On MacOS or Linux, PATH environmant is not correct.
            if (Application.platform != RuntimePlatform.WindowsEditor)
                psi.FileName = "/usr/local/share/dotnet/dotnet";

            var program = new Program(psi);
            program.Start((s, e) =>
            // Exit callback
            {
                if (program.ExitCode == 0)
                {
                    // If dll published, reimport assembly.
                    if (!Path.GetDirectoryName(outputPath).StartsWith("Library/ScriptAssemblies"))
                    {
                        EditorApplication.delayCall += () =>
                            AssetDatabase.ImportAsset(outputPath);
                    }
                    return;
                }

                // Show error logs.
                foreach (var l in File.ReadAllLines(responsefile + ".log"))
                    UnityEngine.Debug.LogError(l);
            });

            return program;
        }
    }
}
