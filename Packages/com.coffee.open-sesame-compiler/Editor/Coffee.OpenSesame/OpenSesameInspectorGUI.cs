using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal static class OpenSesameInspectorGUI
    {
        static GUIContent s_OpenSesameText;
        static GUIContent s_PortableModeText;
        static GUIContent s_ModifySymbolsText;
        static GUIContent s_SettingsText;
        static GUIContent s_PublishText;
        static GUIContent s_HelpText;
        static bool s_OpenSettings = false;
        static Dictionary<string, string> s_PortableVersions = new Dictionary<string, string>();


        static OpenSesameInspectorGUI()
        {
            s_OpenSesameText = new GUIContent("Open Sesame", "Use OpenSesameCompiler instead of default csc. In other words, allow access to internals and privates in other assemblies.");
            s_ModifySymbolsText = new GUIContent("Modify Symbols", "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");
            s_PortableModeText = new GUIContent("Portable Mode", "Make this assembly available to other projects that do not have 'Open Sesame Compiler' package installed.");
            s_SettingsText = new GUIContent("Settings", "Show other settings for this assembly.");
            s_PublishText = new GUIContent("Publish", "Publish this assembly as dll to the parent directory.");
            s_HelpText = new GUIContent("Help", "Open help page on browser.");

            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
            s_OpenSettings = EditorPrefs.GetBool("OpenSesameInspectorGUI_OpenSettings", false);
        }

        static void OnPostHeaderGUI(Editor editor)
        {
            var importer = editor.target as AssemblyDefinitionImporter;
            if (!importer)
                return;

            bool settingChanged = false;
            OpenSesameSetting setting = OpenSesameSetting.CreateFromJson(importer.userData);
            using (new EditorGUILayout.HorizontalScope())
            {
                // Enable OpenSesame.
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    setting.OpenSesame = EditorGUILayout.ToggleLeft(s_OpenSesameText, setting.OpenSesame, GUILayout.MaxWidth(100));
                    settingChanged |= ccs.changed;
                }

                GUILayout.FlexibleSpace();

                // Open settings.
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    s_OpenSettings = GUILayout.Toggle(s_OpenSettings, s_SettingsText, EditorStyles.miniButtonLeft);
                    if (ccs.changed)
                        EditorPrefs.SetBool("OpenSesameInspectorGUI_OpenSettings", s_OpenSettings);
                }

                // Publish assembly as a dll.
                if (GUILayout.Button(s_PublishText, EditorStyles.miniButtonMid))
                {
                    OpenSesameSetting.AssemblyNameToPublish = GetAssemblyName(importer.assetPath);
                    UnityEngine.Debug.Log(Core.kLogHeader + "<b>Request to publish the assembly as dll:</b> " + OpenSesameSetting.AssemblyNameToPublish);
                    settingChanged = true;
                }

                // Open help.
                if (GUILayout.Button(s_HelpText, EditorStyles.miniButtonRight))
                {
                    Application.OpenURL("https://github.com/mob-sakai/OpenSesameCompilerForUnity");
                }
            }

            EditorGUI.indentLevel++;
            if (s_OpenSettings)
            {
                // Modify symbols.
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    setting.ModifySymbols = EditorGUILayout.DelayedTextField(s_ModifySymbolsText, setting.ModifySymbols);
                    settingChanged |= ccs.changed;
                }

                // Portable mode.
                using (new EditorGUI.DisabledScope(!setting.OpenSesame))
                using (new EditorGUILayout.HorizontalScope())
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    var assemblyName = GetAssemblyName(importer.assetPath);
                    var dllPath = Path.Combine(Path.GetDirectoryName(importer.assetPath), "OpenSesamePortable." + assemblyName +".dll");
                    EditorGUILayout.ToggleLeft(s_PortableModeText, File.Exists(dllPath));
                    if (ccs.changed)
                        EditorApplication.delayCall += () => SwitchPortableMode(dllPath);

                    string version = "";
                    if (File.Exists(dllPath) && !s_PortableVersions.TryGetValue(dllPath, out version))
                    {
                        var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                        s_PortableVersions[dllPath] = version = info != null ? info.FileVersion : "Not available";
                    }
                    
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(version);
                }
            }
            EditorGUI.indentLevel--;

            if (settingChanged)
            {
                importer.userData = JsonUtility.ToJson(setting);
                importer.SaveAndReimport();
            }
        }

        static void SwitchPortableMode(string dllPath)
        {
            var originPath = typeof(OpenSesameInspectorGUI).Assembly.Location;
            var dllFullPath = Path.GetFullPath(dllPath);

            if (File.Exists(dllFullPath))
                AssetDatabase.DeleteAsset(dllPath);
            else
            {
                Core.CopyFileIfUpdated(originPath, dllFullPath);
                Core.CopyFileIfUpdated(originPath + ".meta", dllFullPath + ".meta");
                EditorApplication.delayCall += () => AssetDatabase.ImportAsset(dllPath);
            }
        }

        static string GetAssemblyName(string asmdefPath = "")
        {
            var m = Regex.Match(File.ReadAllText(asmdefPath), "\"name\":\\s*\"([^\"]*)\"");
            return m.Success ? m.Groups[1].Value : "";
        }
    }
}
