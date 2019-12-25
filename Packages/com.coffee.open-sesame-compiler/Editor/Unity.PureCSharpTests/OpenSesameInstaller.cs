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
            var customLanguage = new CSharpLanguageForOpenSesame();

            // Remove old custom compilers.
            ScriptCompilers.SupportedLanguages.RemoveAll(x => typeof(CSharpLanguage).IsAssignableFrom(x.GetType()));
            ScriptCompilers.SupportedLanguages.Insert(0, customLanguage);

            // Use reflection to overwrite 'readonly field'.
            typeof(ScriptCompilers)
                .GetField("CSharpSupportedLanguage", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, customLanguage);

            // Overwrite target assembly for c#.
            EditorBuildRules.GetPredefinedTargetAssemblies()
                .Where(x => x != null && x.Language != null)
                .First(x => x.Language.GetType() == typeof(CSharpLanguage))
                .Language = customLanguage;
        }
    }
}
