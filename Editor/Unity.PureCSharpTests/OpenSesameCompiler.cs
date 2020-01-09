using System.Text;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;
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
        string[] references { get { return assembly.References; } }
        public OpenSesameCompiler(ScriptAssembly scriptAssembly, EditorScriptCompilationOptions options, string tempOutputDirectory) : base(scriptAssembly, options, tempOutputDirectory)
        {
        }
#else
        string[] references { get { return m_Island._references; } }
        public OpenSesameCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }
#endif

#if UNITY_2020_1_OR_NEWER
        public override void BeginCompiling()
#else
        protected override Program StartCompiler()
#endif
        {
            // Install modified compiler. 
            string installedCsc = OpenSesameInstaller.Install();

#if UNITY_2020_1_OR_NEWER
            base.BeginCompiling();
#else
            Program p = base.StartCompiler();
#endif
            if (string.IsNullOrEmpty(installedCsc))
            {
                UnityEngine.Debug.LogWarning("<b>[OpenSesame]</b><color=orange>[Compiler]</color> OpenSesameCompiler is not installed. Instead, the default compiler will be used.");
                return p;
            }

            // Kill previous process.
            p.Kill();

            var psi = p.GetProcessStartInfo();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                psi.FileName = Path.GetFullPath(installedCsc);
                psi.Arguments = psi.Arguments.Replace("@Temp/", "@Temp\\");
            }
            else
            {
                psi.FileName = Paths.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono").Replace('\\', '/');
                psi.Arguments = installedCsc + " " + psi.Arguments.Replace("/shared ", "");
            }

            Debug.LogFormat("<b>[OpenSesame]</b><color=orange>[Compiler]</color> {0} {1}", psi.FileName, psi.Arguments);

            var program = new Program(psi);
            program.Start();

#if UNITY_2020_1_OR_NEWER
            process = program;
#else
            return program;
#endif
        }
    }
}
