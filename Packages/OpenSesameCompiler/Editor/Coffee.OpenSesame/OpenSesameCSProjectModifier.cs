using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.OpenSesame
{
    internal class OpenSesameCSProjectModifier : AssetPostprocessor
    {
        static void Log(string format, params object[] args)
        {
            if (Core.LogEnabled)
                UnityEngine.Debug.LogFormat("<color=#0063b1><b>[OpenSesameCSProjectModifier]</b></color> " + format, args);
        }

        static string OnGeneratedCSProject(string path, string content)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(path);
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            var setting = OpenSesameSetting.GetAtPath(asmdefPath);
            if (string.IsNullOrEmpty(setting.ModifySymbols) && !setting.OpenSesame)
                return content;

            var defines = Regex.Match(content, "<DefineConstants>(.*)</DefineConstants>").Groups[1].Value.Split(';', ',');
            defines = Core.ModifyDefines(defines, setting.OpenSesame, setting.ModifySymbols);
            var defineText = string.Join(";", defines);

            Log("Script defines in {0}.csproj are modified:\n{1}", assemblyName, defineText);
            content = Regex.Replace(content, "<DefineConstants>(.*)</DefineConstants>", string.Format("<DefineConstants>{0}</DefineConstants>", defineText), RegexOptions.Multiline);

            // Use latest language version.
            if(setting.OpenSesame)
            {
                content = Regex.Replace(content, "<LangVersion>.*</LangVersion>", "<LangVersion>latest</LangVersion>", RegexOptions.Multiline);
            }

            return content;
        }
    }
}
