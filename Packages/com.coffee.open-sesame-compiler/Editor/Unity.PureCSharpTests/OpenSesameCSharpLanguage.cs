using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditorInternal;

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
            // No asmdef -> default compiler
            if (string.IsNullOrEmpty(scriptAssembly.OriginPath))
#if UNITY_2019_3_OR_NEWER
                return base.CreateCompiler(scriptAssembly, options, tempOutputDirectory);
#else
                return base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);
#endif

            // Do not use OpenSesameCompiler -> default compiler
            var asmdefPath = Directory.GetFiles(scriptAssembly.OriginPath, "*.asmdef")
                .Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, ""))
                .FirstOrDefault();
            var setting = OpenSesameSetting.CreateFromAsmdef(asmdefPath);

            // No OpenSesameCompiler setting in meta.
            if (setting == null || !setting.OpenSesameCompiler)
#if UNITY_2019_3_OR_NEWER
                return base.CreateCompiler(scriptAssembly, options, tempOutputDirectory);
#else
                return base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);
#endif

            var asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
            Debug.LogFormat("<b>[OpenSesame]</b><color=green>[Language]</color> Say open sesame: {0}", asmdef.name);

            // If publish is set, change output path.
            var path = OpenSesameSetting.PublishOrigin;
            if (path == scriptAssembly.OriginPath)
            {
                Debug.LogFormat("<b>[OpenSesame]</b><color=green>[Language]</color> Publish to: {0}", setting.PublishFolder);
                OpenSesameSetting.PublishOrigin = null;

                //scriptAssembly.Filename = Path.GetFileName(setting.PublishFolder);
                if (Directory.Exists(setting.PublishFolder))
                    scriptAssembly.OutputDirectory = setting.PublishFolder;
                else
                {
                    UnityEngine.Debug.LogWarningFormat(asmdef, "Directory '{0}' is not found", setting.PublishFolder);
                }
            }

            scriptAssembly.Filename = asmdef.name + ".dll";
            Debug.LogFormat("<b>[OpenSesame]</b><color=green>[Language]</color> Ready to open sesame: {0}", asmdef.name);

            // Use OpenSesameCompiler.
#if UNITY_2019_3_OR_NEWER
            return new OpenSesameCompiler(scriptAssembly, options, tempOutputDirectory);
#else
            ValueType v = island;
            typeof(MonoIsland)
                .GetField("_output", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(v, "Temp/" + scriptAssembly.Filename);
            island = (MonoIsland)v;

            return new OpenSesameCompiler(scriptAssembly, island, runUpdater);
#endif
        }

    }
}
