using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditorInternal;

namespace Coffee.OpenSesameCompilers
{
    internal class CSharpLanguageForOpenSesame : CSharpLanguage
    {
        public override ScriptCompilerBase CreateCompiler(ScriptAssembly scriptAssembly, MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
        {
            // No asmdef -> default compiler
            if (string.IsNullOrEmpty(scriptAssembly.OriginPath))
                return base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);

            // Do not use OpenSesameCompiler -> default compiler
            var asmdefPath = Directory.GetFiles(scriptAssembly.OriginPath, "*.asmdef").Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, "")).FirstOrDefault();
            var setting = OpenSesameSetting.CreateFromAsmdef(asmdefPath);

            // No OpenSesameCompiler setting in meta.
            if (setting == null || !setting.OpenSesameCompiler)
                return base.CreateCompiler(scriptAssembly, island, buildingForEditor, targetPlatform, runUpdater);

            // If publish is set, change output path.
            var path = OpenSesameSetting.PublishOrigin;
            if (path == scriptAssembly.OriginPath)
            {
                OpenSesameSetting.PublishOrigin = null;

                //scriptAssembly.Filename = Path.GetFileName(setting.PublishFolder);
                if (Directory.Exists(setting.PublishFolder))
                    scriptAssembly.OutputDirectory = setting.PublishFolder;
                else
                {
                    var asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
                    UnityEngine.Debug.LogWarningFormat(asmdef, "Directory '{0}' is not found", setting.PublishFolder);
                }
            }

            // Use OpenSesameCompiler.
            return new OpenSesameCompiler(scriptAssembly, island, runUpdater);
        }
    }
}
