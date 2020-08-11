#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.AsmdefEx
{
    internal class Menus : AssetPostprocessor
    {
        const string kEnableLoggingText = "Asmdef Ex/Enable Logging";
        const string kEnableLoggingSymbol = "ASMDEF_EX_LOG";
        const string kDeleteCompilerText = "Asmdef Ex/Delete Compiler";
        const string kReloadText = "Asmdef Ex/Reload AsmdefEx.cs For Tests";

        void OnPreprocessAsset()
        {
            // When AsmdefEx.cs is changed, sync the AsmdefEx.cs in tests.
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("Coffee.AsmdefEx");
            var sourcePath = Path.GetDirectoryName(asmdefPath).Replace('\\', '/') + "/AsmdefEx.cs";

            if (sourcePath != assetImporter.assetPath) return;

            Menus.ReloadTests();
        }

        [MenuItem(kEnableLoggingText, false)]
        static void EnableLogging()
        {
            SwitchSymbol(kEnableLoggingSymbol);
        }

        [MenuItem(kEnableLoggingText, true)]
        static bool EnableLogging_Valid()
        {
            Menu.SetChecked(kEnableLoggingText, HasSymbol(kEnableLoggingSymbol));
            return true;
        }

        [MenuItem(kDeleteCompilerText, false)]
        static void DeleteCompiler()
        {
            var path = CustomCompiler.GetInstalledPath();
            if (!string.IsNullOrEmpty(path))
                FileUtil.DeleteFileOrDirectory(path);
        }

        [MenuItem(kReloadText, false)]
        public static void ReloadTests()
        {
            var editorTests = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("Coffee.AsmdefEx.EditorTests");
            Coffee.AsmdefEx.InspectorGUI.SetExtensionEnabled(editorTests, true);
            var runtimeTests = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("Coffee.AsmdefEx.RuntimeTests");
            Coffee.AsmdefEx.InspectorGUI.SetExtensionEnabled(runtimeTests, true);
        }

        static string[] GetSymbols()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';', ',');
        }

        static void SetSymbols(string[] symbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols));
        }

        static bool HasSymbol(string symbol)
        {
            return GetSymbols().Any(x => x == symbol);
        }

        static void SwitchSymbol(string symbol)
        {
            var symbols = GetSymbols();
            SetSymbols(symbols.Any(x => x == symbol)
                ? symbols.Where(x => x != symbol).ToArray()
                : symbols.Concat(new[] {symbol}).ToArray()
            );
        }
    }
}
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
