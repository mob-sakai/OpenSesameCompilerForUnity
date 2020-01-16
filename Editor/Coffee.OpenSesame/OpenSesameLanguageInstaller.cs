using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Coffee.OpenSesamePortable;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal class OpenSesameLanguageInstaller
    {
        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat("<b>[OpenSesame]</b><color=#10893e>[LanguageInstaller]</color> " + format, args);
        }

        static OpenSesameLanguageInstaller()
        {
            Log("Start install OpenSesame to project.");
            var csc = Core.GetInstalledCompiler();
            Log("OpenSesame Compiler has been installed at {0}", csc);

            // 
            Log("Start install OpenSesameLanguage to Unity.");
            var language = new OpenSesameLanguage();

            // Remove old custom compilers.
            int removed = ScriptCompilers.SupportedLanguages.RemoveAll(x => typeof(CSharpLanguage).IsAssignableFrom(x.GetType()));
            ScriptCompilers.SupportedLanguages.Insert(0, language);
            Log("{0} langages has been removed.", removed);

            // Use reflection to overwrite 'readonly field'.
            typeof(ScriptCompilers)
                .GetField("CSharpSupportedLanguage", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, language);

            // Overwrite target assembly for c#.
            foreach (var ta in EditorBuildRules.GetPredefinedTargetAssemblies()
                .Where(x => x != null && x.Language != null)
                .Where(x => x.Language.GetType() == typeof(CSharpLanguage)))
            {
                Log("{0} will be replaced.", ta.Language.GetLanguageName());
                ta.Language = language;
            }

            Log("{0} has been installed.", typeof(OpenSesameLanguage).Name);
        }
    }
}
