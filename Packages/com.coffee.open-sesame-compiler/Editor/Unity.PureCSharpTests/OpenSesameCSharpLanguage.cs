using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;

namespace Coffee.OpenSesameCompilers
{
    internal class OpenSesameCSharpLanguage : CSharpLanguage
    {
#if UNITY_2019_3_OR_NEWER
        public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, EditorScriptCompilationOptions options, string tempOutputDirectory)
#else
        public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
#endif
        {
#if UNITY_2019_3_OR_NEWER
            Func<ScriptCompilerBase> CreateBaseCompiler = () => base.CreateCompiler(scriptAssembly, options, tempOutputDirectory);
#else
            Func<ScriptCompilerBase> CreateBaseCompiler = () => base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);
#endif

            // No asmdef: Use default compiler
            if (string.IsNullOrEmpty(scriptAssembly.OriginPath))
                return CreateBaseCompiler();

            // If publish is set, change output directory.
            var path = OpenSesameSetting.PublishOrigin;
            if (path == scriptAssembly.OriginPath)
            {
                OpenSesameSetting.PublishOrigin = null;
                scriptAssembly.OutputDirectory = Path.GetDirectoryName(scriptAssembly.OriginPath.TrimEnd('/'));
                UnityEngine.Debug.LogFormat("<b>Publish assembly as dll:</b> {0}", scriptAssembly.FullPath);
            }

            // Get OpenSesameSetting from asmdef meta.
            var setting = OpenSesameSetting.GetAtPathOrDefault(
                Directory.GetFiles(scriptAssembly.OriginPath, "*.asmdef")
                .Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, ""))
                .FirstOrDefault()
            );

            }

            // OpenSesame is disable: Use default compiler.
            if (!setting.OpenSesame)
                return CreateBaseCompiler();

            // Use OpenSesameCompiler.
            Debug.LogFormat("<b>[OpenSesame]</b><color=green>[Language]</color> Say open sesame: {0}", scriptAssembly.Filename);
#if UNITY_2019_3_OR_NEWER
            return new OpenSesameCompiler(scriptAssembly, options, tempOutputDirectory);
#else
            return new OpenSesameCompiler(island, runUpdater);
#endif
        }
    }
}
