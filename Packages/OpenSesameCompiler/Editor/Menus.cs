using System.Linq;
using UnityEditor;

namespace Coffee.AsmdefEx
{
    internal static class Menus
    {
        const string kEnableText = "Assets/Asmdef Ex/Enable";
        const string kDisableSymbol = "ASMDEF_EX_DISABLE";

        const string kEnableLoggingText = "Assets/Asmdef Ex/Enable Logging";
        const string kEnableLoggingSymbol = "ASMDEF_EX_LOG";

        const string kDeleteCompilerText = "Assets/Asmdef Ex/Delete Compiler";
        const string kInstallCompilerText = "Assets/Asmdef Ex/Install Compiler";

        [MenuItem(kEnableText, false)]
        static void Enable()
        {
            SwitchSymbol(kDisableSymbol);
        }

        [MenuItem(kEnableText, true)]
        static bool Enable_Valid()
        {
            Menu.SetChecked(kEnableText, !HasSymbol(kDisableSymbol));
            return true;
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

        [MenuItem(kInstallCompilerText, false, 30)]
        static void InstallCompiler()
        {
            CustomCompiler.GetInstalledPath();
        }

        [MenuItem(kDeleteCompilerText, false, 31)]
        static void DeleteCompiler()
        {
            var path = CustomCompiler.GetInstalledPath();
            if (!string.IsNullOrEmpty(path))
                FileUtil.DeleteFileOrDirectory(path);
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
