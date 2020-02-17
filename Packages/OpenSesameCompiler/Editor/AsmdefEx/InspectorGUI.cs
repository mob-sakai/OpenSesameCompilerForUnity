using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor.Compilation;
using System;
using System.Security.Cryptography;
using System.Linq;

namespace Coffee.AsmdefEx
{
    [InitializeOnLoad]
    internal static class InspectorGUI
    {
        static string s_AssemblyNameToPublish;
        static GUIContent s_IgnoreAccessCheckText;
        static GUIContent s_EnableText;
        static GUIContent s_ModifySymbolsText;
        static GUIContent s_SettingsText;
        static GUIContent s_PublishText;
        static GUIContent s_HelpText;
        static bool s_OpenSettings = false;
        static Dictionary<string, bool> s_Portables = new Dictionary<string, bool>();
        static Dictionary<string, bool> s_Ignores = new Dictionary<string, bool>();

        static void OnAssemblyCompilationFinished(string name, CompilerMessage[] messages)
        {
            try
            {
                // This assembly is requested to publish?
                string assemblyName = Path.GetFileNameWithoutExtension(name);
                if (s_AssemblyNameToPublish != assemblyName)
                    return;

                s_AssemblyNameToPublish = null;
                Core.LogEx("Assembly compilation finished: <b>{0} is requested to publish.</b>", assemblyName);

                // No compilation error?
                if (messages.Any(x => x.type == CompilerMessageType.Error))
                    return;

                var scriptAssembly = Core.GetScriptAssembly(assemblyName);
                var originPath = scriptAssembly.Get("OriginPath") as string;

                // Publish a dll to parent directory.
                var dst = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(originPath)), assemblyName + ".dll");
                var src = "Library/ScriptAssemblies/" + Path.GetFileName(dst);
                Core.LogEx("<b>Publish assembly as dll:</b> " + dst);
                CopyFileIfUpdated(Path.GetFullPath(src), Path.GetFullPath(dst));

                EditorApplication.delayCall += () => AssetDatabase.ImportAsset(dst);
            }
            catch (Exception e)
            {
                Core.Error(e);
            }
        }

        public static void CopyFileIfUpdated(string src, string dst)
        {
            src = Path.GetFullPath(src);
            if (!File.Exists(src))
                return;

            dst = Path.GetFullPath(dst);
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

        static InspectorGUI()
        {
            s_IgnoreAccessCheckText = new GUIContent("Ignore Access Checks", "Ignore accessibility checks on compiling to allow access to internals and privates in other assemblies.");
            s_ModifySymbolsText = new GUIContent("Modify Symbols", "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");
            s_EnableText = new GUIContent("Enable Asmdef Extension", "Enable asmdef extension for this assembly.");
            s_SettingsText = new GUIContent("Asmdef Extension", "Show extension settings for this assembly definition file.");
            s_PublishText = new GUIContent("Publish", "Publish this assembly as dll to the parent directory.");
            s_HelpText = new GUIContent("Help", "Open AsmdefEx help page on browser.");

            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
            s_OpenSettings = EditorPrefs.GetBool("Coffee.AsmdefEx.InspectorGUI_OpenSettings", false);
            // CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        static void OnPostHeaderGUI(Editor editor)
        {
            var importer = editor.target as AssemblyDefinitionImporter;
            if (!importer)
                return;

            GUILayout.Space(-EditorGUIUtility.singleLineHeight);

            bool settingChanged = false;
            Settings setting = Settings.CreateFromJson(importer.userData);
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Space(30);

                // Open settings.
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    s_OpenSettings = GUILayout.Toggle(s_OpenSettings, s_SettingsText, EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false));
                    if (ccs.changed)
                        EditorPrefs.SetBool("Coffee.AsmdefEx.InspectorGUI_OpenSettings", s_OpenSettings);
                }

                // Publish assembly as a dll.
                if (GUILayout.Button(s_PublishText, EditorStyles.miniButtonMid, GUILayout.ExpandWidth(false)))
                {
                    s_AssemblyNameToPublish = GetAssemblyName(importer.assetPath);
                    Core.LogEx("<b>Request to publish the assembly as dll:</b> " + s_AssemblyNameToPublish);
                    settingChanged = true;
                }

                // Open help.
                if (GUILayout.Button(s_HelpText, EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL("https://github.com/mob-sakai/OpenSesameCompilerForUnity");
                }
            }

            EditorGUI.indentLevel++;
            if (s_OpenSettings)
            {
                // Enable.
                bool enabled = GetExtensionEnabled(importer.assetPath);
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    enabled = EditorGUILayout.ToggleLeft(s_EnableText, enabled);
                    if (ccs.changed)
                    {
                        EditorApplication.delayCall += () => SetExtensionEnabled(importer.assetPath, enabled);
                    }
                }

                // Ignore Accessibility Checks.
                using (new EditorGUI.DisabledScope(!enabled))
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    setting.IgnoreAccessChecks = EditorGUILayout.ToggleLeft(s_IgnoreAccessCheckText, setting.IgnoreAccessChecks);
                    settingChanged |= ccs.changed;
                }

                // Modify symbols.
                using (new EditorGUI.DisabledScope(!enabled))
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    setting.ModifySymbols = EditorGUILayout.DelayedTextField(s_ModifySymbolsText, setting.ModifySymbols);
                    settingChanged |= ccs.changed;
                }
            }
            EditorGUI.indentLevel--;

            if (settingChanged)
            {
                importer.userData = setting.ToJson();
                importer.SaveAndReimport();
            }
        }

        static readonly string s_If = "#if IGNORE_ACCESS_CHECKS // DO NOT REMOVE THIS LINE MANUALLY.";
        static readonly string s_EndIf = "#endif // IGNORE_ACCESS_CHECKS: DO NOT REMOVE THIS LINE MANUALLY.";

        static bool GetExtensionEnabled(string asmdefPath)
        {
            bool enabled;
            if (!s_Portables.TryGetValue(asmdefPath, out enabled))
            {
                string dst = Path.GetDirectoryName(asmdefPath) + "/AsmdefEx.cs";
                enabled = File.Exists(dst);
                s_Portables[asmdefPath] = enabled;
            }
            return enabled;
        }

        static void SetExtensionEnabled(string asmdefPath, bool enabled)
        {
            s_Portables[asmdefPath] = enabled;
            string dst = Path.GetDirectoryName(asmdefPath) + "/AsmdefEx.cs";
            if (enabled)
            {
                // Copy AsmdefEx.cs to assembly.
                const string src = "Packages/com.coffee.open-sesame-compiler/Editor/AsmdefEx/AsmdefEx.cs";
                AssetDatabase.CopyAsset(src, dst);
            }
            else
            {
                // Delete AsmdefEx.cs from assembly.
                AssetDatabase.DeleteAsset(dst);
            }
        }


        static void EnablePortableMode(string asmdefPath)
        {
            const string src = "Packages/com.coffee.open-sesame-compiler/Editor/AsmdefEx/AsmdefEx.cs";
            string dst = Path.GetDirectoryName(asmdefPath) + "/AsmdefEx.cs";

            var dir = Path.GetDirectoryName(asmdefPath);
            var fullDir = Path.GetFullPath(dir);

            var assemblyName = GetAssemblyName(asmdefPath);
            var files = Core.GetScriptAssembly(assemblyName).Get("Files") as string[];

            AssetDatabase.StartAssetEditing();

            // Copy AsmdefEx.cs to assembly.
            AssetDatabase.CopyAsset(src, dst);

            // Add #if and #endif to all source files.
            foreach (var file in files)
            {
                var assetPath = file.Replace(fullDir, dir).Replace('\\', '/');

                var text = File.ReadAllText(file);
                var m = Regex.Match(text, "[\r\n]+");
                if (!m.Success)
                    continue;

                var nl = m.Value;
                text = s_If + nl + text + nl + s_EndIf;
                File.WriteAllText(file, text);

                AssetDatabase.ImportAsset(assetPath);
            }

            AssetDatabase.StopAssetEditing();
        }

        static void DisablePortableMode(string asmdefPath)
        {
            var dir = Path.GetDirectoryName(asmdefPath);
            var fullDir = Path.GetFullPath(dir);
            var assemblyName = GetAssemblyName(asmdefPath);
            var files = Core.GetScriptAssembly(assemblyName).Get("Files") as string[];

            AssetDatabase.StartAssetEditing();

            // Add #if and #endif to all source files.
            foreach (var file in files)
            {
                var assetPath = file.Replace(fullDir, dir).Replace('\\', '/');

                // Delete AsmdefEx.cs
                if (Path.GetFileName(file) == "AsmdefEx.cs")
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    continue;
                }

                // Remove #if and #endif from all source files.
                var text = File.ReadAllText(file);
                text = Regex.Replace(text, s_If + "[\r\n]+", "");
                text = Regex.Replace(text, "[\r\n]+" + s_EndIf, "");
                File.WriteAllText(file, text);

                AssetDatabase.ImportAsset(assetPath);
            }
            AssetDatabase.StopAssetEditing();
        }

        static string GetAssemblyName(string asmdefPath = "")
        {
            var m = Regex.Match(File.ReadAllText(asmdefPath), "\"name\":\\s*\"([^\"]*)\"");
            return m.Success ? m.Groups[1].Value : "";
        }
    }
}
