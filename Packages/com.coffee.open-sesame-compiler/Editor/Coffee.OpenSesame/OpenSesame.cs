using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.Security.Cryptography;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal static class Core
    {
        public static bool LogEnabled;
        public static string kLogHeader = "<b><color=#9a4089>[OpenSesame]</color></b> ";

        static void Log(string format, params object[] args)
        {
            if (LogEnabled)
                UnityEngine.Debug.LogFormat(kLogHeader + format, args);
        }

        static void Error(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(kLogHeader + format, args);
        }

        public static object GetScriptAssembly(string assemblyName)
        {
            Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
            Type tCSharpLanguage = Type.GetType("UnityEditor.Scripting.Compilers.CSharpLanguage, UnityEditor");
            return tEditorCompilationInterface.Call(new[] { tCSharpLanguage }, "GetScriptAssemblyForLanguage", assemblyName);
        }

        public static string[] ModifyDefines(IEnumerable<string> defines, bool openSesame, string modifySymbols)
        {
            var symbols = modifySymbols.Split(';', ',');
            var add = symbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
            var remove = symbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));
            return defines
                .Union(add ?? Enumerable.Empty<string>())
                .Except(remove ?? Enumerable.Empty<string>())
                .Union(openSesame ? new[] { "OPEN_SESAME" } : Enumerable.Empty<string>())
                .Distinct()
                .ToArray();
        }

        public static void ChangeCompilerProcess(object compiler, OpenSesameSetting setting)
        {
            Type tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            Type tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            FieldInfo fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

            Log("Kill previous compiler process");
            var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
            compiler.Call("Dispose");

            // Convert response file for Mono to .Net.
            //   - Add preferreduilang option (en-US)
            //   - Change language version to 'latest'
            //   - Change 'debug' to 'debug:portable'
            //   - Change compiler switch prefix '-' to '/'
            string responseFile = Regex.Replace(psi.Arguments, "^.*@(.+)$", "$1");
            if (compiler.GetType().Name == "MonoCSharpCompiler")
            {
                Log("Convert response file for Mono to .Net: {0}", responseFile);
                var text = File.ReadAllText(responseFile);
                text = "/preferreduilang:en-US\n" + text;
                text = Regex.Replace(text, "^-", "/", RegexOptions.Multiline);
                text = Regex.Replace(text, "^/langversion:\\d+$", "/langversion:latest", RegexOptions.Multiline);
                text = Regex.Replace(text, "^/debug$", "/debug:portable", RegexOptions.Multiline);
                File.WriteAllText(responseFile, text);
            }

            // Modify scripting define symbols.
            if (setting.SholdModifyDefines)
            {
                Log("Modify scripting define symbols: {0}", responseFile);
                var text = File.ReadAllText(responseFile);
                var defines = Regex.Matches(text, "^/define:([^\r\n]+)", RegexOptions.Multiline)
                        .Cast<Match>()
                        .Select(x => x.Groups[1].Value);

                text = Regex.Replace(text, "[\r\n]+/define:[^\r\n]+", "");
                foreach (var d in ModifyDefines(defines, setting.OpenSesame, setting.ModifySymbols))
                {
                    text += System.Environment.NewLine + "/define:" + d;
                }
                File.WriteAllText(responseFile, text);
            }

            if (setting.OpenSesame)
            {
                var cscToolExe = OpenSesameInstaller.GetInstalledCompilerPath();
                Log("Change csc tool exe to {0}", cscToolExe);
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    psi.FileName = Path.GetFullPath(cscToolExe);
                    psi.Arguments = "/shared /noconfig @" + responseFile;
                }
                else
                {
                    psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono");
                    psi.Arguments = cscToolExe + " /noconfig @" + responseFile;
                }
            }

            Log("Restart compiler process: {0} {1}", psi.FileName, psi.Arguments);
            var program = tProgram.New(psi);
            program.Call("Start");
            compiler.Set("process", program, fiProcess);
        }

        static void OnAssemblyCompilationStarted(string name)
        {
            try
            {
                string assemblyName = Path.GetFileNameWithoutExtension(name);
                string assemblyFilename = assemblyName + ".dll";

                Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
                var compilerTasks = tEditorCompilationInterface.Get("Instance").Get("compilationTask").Get("compilerTasks") as IDictionary;
                var scriptAssembly = compilerTasks.Keys.Cast<object>().FirstOrDefault(x => (x.Get("Filename") as string) == assemblyFilename);

                // Should change compiler process for the assembly?
                var setting = OpenSesameSetting.GetAtPath(scriptAssembly.Get("OriginPath") as string);
                if (!setting.SholdChangeCompilerProcess)
                    return;

                // Create new compiler to recompile.
                Log("Assembly compilation started: <b>{0} should be recompiled.</b>", assemblyName);
                Core.ChangeCompilerProcess(compilerTasks[scriptAssembly], setting);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(new Exception(kLogHeader + e.Message, e.InnerException));
            }
        }

        static void OnAssemblyCompilationFinished(string name, CompilerMessage[] messages)
        {
            try
            {
                // This assembly is requested to publish?
                string assemblyName = Path.GetFileNameWithoutExtension(name);
                if (OpenSesameSetting.AssemblyNameToPublish != assemblyName)
                    return;

                OpenSesameSetting.AssemblyNameToPublish = null;
                Log("Assembly compilation finished: <b>{0} is requested to publish.</b>", assemblyName);

                // No compilation error?
                if (messages.Any(x => x.type == CompilerMessageType.Error))
                    return;

                var scriptAssembly = Core.GetScriptAssembly(assemblyName);
                var originPath = scriptAssembly.Get("OriginPath") as string;

                // Publish a dll to parent directory.
                var dst = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(originPath)), assemblyName + ".dll");
                var src = "Library/ScriptAssemblies/" + Path.GetFileName(dst);
                UnityEngine.Debug.Log(kLogHeader + "<b>Publish assembly as dll:</b> " + dst);
                CopyFileIfUpdated(src, dst);

                EditorApplication.delayCall += () => AssetDatabase.ImportAsset(dst);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(new Exception(kLogHeader + e.Message, e.InnerException));
            }
        }

        public static void CopyFileIfUpdated(string src, string dst)
        {
            if (!File.Exists(src))
                return;

            if (File.Exists(dst))
            {
                using (var srcFs = new FileStream(src, FileMode.Open))
                using (var dstFs = new FileStream(dst, FileMode.Open))
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    if (md5.ComputeHash(srcFs).SequenceEqual(md5.ComputeHash(dstFs)))
                        return;
                }
            }

            var dir = Path.GetDirectoryName(dst);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(src, dst, true);
        }

        static Core()
        {
            LogEnabled = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';', ',')
                .Any(x => x == "OPEN_SESAME_LOG");

            var assembly = typeof(Core).Assembly;
            var asmInfo = string.Format("{0} ({1})", FileUtil.GetProjectRelativePath(assembly.Location), assembly.GetName().Version);
            Log("Activate OpenSesame assembly: <b>{0}</b>", asmInfo);
            kLogHeader = string.Format("<b><color=#9a4089>[OpenSesame ({0})]</color></b> ", assembly.GetName().Version);

            var cscPath = OpenSesameInstaller.GetInstalledCompilerPath();
            if (string.IsNullOrEmpty(cscPath))
            {
                Error("Failed to install Open Sesame Compiler ver {0}. OpenSesame assembly: {1}.", OpenSesameInstaller.Version, asmInfo);
                return;
            }
            Log("Installed OpenSesame compiler: <b>{0}</b>", cscPath);

            Log("Start watching assembly compilation...");
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }
    }
}