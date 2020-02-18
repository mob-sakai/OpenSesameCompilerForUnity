using System.Linq;
using UnityEditor;

namespace Coffee.AsmdefEx
{
    class Menus
    {
        const string kEnableText = "Assets/Asmdef Ex/Enable";
        const string kDisableSymbol = "ASMDEF_EX_DISABLE";

        const string kEnableLoggingText = "Assets/Asmdef Ex/Enable Logging";
        const string kEnableLoggingSymbol = "ASMDEF_EX_LOG";

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
                : symbols.Concat(new[] { symbol }).ToArray()
            );
        }
    }
}
