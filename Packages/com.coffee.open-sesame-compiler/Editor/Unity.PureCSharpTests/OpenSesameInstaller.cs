using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;

namespace Coffee.OpenSesameCompilers
{
    [InitializeOnLoad]
    internal class OpenSesameInstaller
    {
        static OpenSesameInstaller()
        {
            var customLanguage = new OpenSesameCSharpLanguage();

            // Remove old custom compilers.
            int removed = ScriptCompilers.SupportedLanguages.RemoveAll(x => typeof(CSharpLanguage).IsAssignableFrom(x.GetType()));
            ScriptCompilers.SupportedLanguages.Insert(0, customLanguage);
            Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> {0} langages has been removed.", removed);

            // Use reflection to overwrite 'readonly field'.
            typeof(ScriptCompilers)
                .GetField("CSharpSupportedLanguage", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, customLanguage);

            // Overwrite target assembly for c#.
            foreach (var ta in EditorBuildRules.GetPredefinedTargetAssemblies()
                .Where(x => x != null && x.Language != null)
                .Where(x => x.Language.GetType() == typeof(CSharpLanguage)))
            {
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> {0} will be replaced.", ta.Language.GetLanguageName());
                ta.Language = customLanguage;
            }

            Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> {0} has been installed.", typeof(OpenSesameCSharpLanguage).Name);
        }
    }
}
