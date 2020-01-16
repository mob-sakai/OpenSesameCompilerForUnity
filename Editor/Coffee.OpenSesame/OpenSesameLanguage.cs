using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Coffee.OpenSesamePortable;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;

namespace Coffee.OpenSesame
{
	internal class OpenSesameLanguage : CSharpLanguage
    {
        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat("<b>[OpenSesame]</b><color=#10893e>[Language]</color> " + format, args);
        }

#if UNITY_2020_1_OR_NEWER
        public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, string tempOutputDirectory)
        {
            Func<ScriptCompilerBase> CreateBaseCompiler = () => base.CreateCompiler(scriptAssembly, tempOutputDirectory);
#elif UNITY_2019_3_OR_NEWER
        public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, EditorScriptCompilationOptions options, string tempOutputDirectory)
        {
            Func<ScriptCompilerBase> CreateBaseCompiler = () => base.CreateCompiler(scriptAssembly, options, tempOutputDirectory);
#else
		public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            Func<ScriptCompilerBase> CreateBaseCompiler = () => base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);
#endif

            // No asmdef: Use default compiler
            if (string.IsNullOrEmpty(scriptAssembly.OriginPath))
                return CreateBaseCompiler();

            // If publish is set, change output directory.
            var path = OpenSesameSetting.PublishOrigin;
            if (path == scriptAssembly.OriginPath)
            {
                OpenSesameSetting.PublishAssemblyName = scriptAssembly.Filename;
                //scriptAssembly.OutputDirectory = Path.GetDirectoryName(scriptAssembly.OriginPath.TrimEnd('/'));
                UnityEngine.Debug.LogFormat("<b>[OpenSesame]</b> Publish assembly as dll: {0}", scriptAssembly.FullPath);
            }

            // Get OpenSesameSetting from asmdef meta.
            var setting = OpenSesameSetting.GetAtPathOrDefault(
                Directory.GetFiles(scriptAssembly.OriginPath, "*.asmdef")
                .Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, ""))
                .FirstOrDefault()
            );

            // Modify define symbols.
            if (!string.IsNullOrEmpty(setting.ModifySymbols) || setting.OpenSesame)
            {
                var symbols = setting.ModifySymbols.Split(';');
                var add = symbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
                var remove = symbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));
                var assemblyName = Path.GetFileNameWithoutExtension(scriptAssembly.Filename);
                var isInternal = Core.IsInternalAssembly(assemblyName);

                scriptAssembly.Defines = Core.ModifyDefines(scriptAssembly.Defines, setting.OpenSesame && !isInternal, add, remove);
                Log("Modified define symbols: {0}", string.Join(";", scriptAssembly.Defines));
            }

            // OpenSesame is disable: Use default compiler.
            if (!setting.OpenSesame)
                return CreateBaseCompiler();

            // Use OpenSesameCompiler.
            Log("Compile with saying 'Open sesame!': {0}", scriptAssembly.Filename);
#if UNITY_2020_1_OR_NEWER
            return new OpenSesameCompiler(scriptAssembly, tempOutputDirectory);
#elif UNITY_2019_3_OR_NEWER
			var options = buildingForEditor ? EditorScriptCompilationOptions.BuildingForEditor : EditorScriptCompilationOptions.BuildingEmpty;
            return new OpenSesameCompiler(scriptAssembly, options, tempOutputDirectory);
#else
            var options = buildingForEditor ? EditorScriptCompilationOptions.BuildingForEditor : EditorScriptCompilationOptions.BuildingEmpty;
            return new OpenSesameCompiler(scriptAssembly.ToMonoIsland(options, "Temp", ""), runUpdater);
#endif
        }
    }
}
