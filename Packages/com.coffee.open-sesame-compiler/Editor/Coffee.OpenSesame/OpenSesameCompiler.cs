using System;
using System.Diagnostics;
using System.Reflection;
using Coffee.OpenSesamePortable;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;

namespace Coffee.OpenSesame
{
    internal class OpenSesameCompiler : MicrosoftCSharpCompiler
    {
        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat("<b>[OpenSesame]</b><color=#0063b1>[Compiler]</color> " + format, args);
        }

#if UNITY_2020_1_OR_NEWER
        public OpenSesameCompiler(ScriptAssembly scriptAssembly, string tempOutputDirectory) : base(scriptAssembly, tempOutputDirectory)
        {
        }
        
#elif UNITY_2019_3_OR_NEWER
        public OpenSesameCompiler(ScriptAssembly scriptAssembly, EditorScriptCompilationOptions options, string tempOutputDirectory) : base(scriptAssembly, options, tempOutputDirectory)
        {
        }
#else
        public OpenSesameCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }
#endif

#if UNITY_2020_1_OR_NEWER
        public override void BeginCompiling()
        {
            string cscToolExe = Bootstrap.GetInstalledCompiler();
            base.BeginCompiling();
            ChangeCompiler(this, cscToolExe);
        }
#else
        protected override Program StartCompiler()
        {
            FieldInfo fiProcess = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor")
                    .GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

            string cscToolExe = Core.GetInstalledCompiler();
            var process = base.StartCompiler();
            this.Set("process", process, fiProcess);
            return Core.ChangeCompiler(this, cscToolExe) as Program;
        }
#endif
    }
}
