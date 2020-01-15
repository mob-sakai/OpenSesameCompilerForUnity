using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.Compilation;

namespace Coffee.OpenSesameCompilers
{
    public class CustomAssetPostprocessor : AssetPostprocessor
    {
        static readonly Regex s_DefineConstants = new Regex("<DefineConstants>(.*)</DefineConstants>", RegexOptions.Compiled);

        static string OnGeneratedCSProject(string path, string content)
        {
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(Path.GetFileName(path).Replace(".csproj", ""));
            var setting = OpenSesameSetting.GetAtPathOrDefault(asmdefPath);
            if (string.IsNullOrEmpty(setting.ModifySymbols) && !setting.OpenSesame)
                return content;

            var symbols = setting.ModifySymbols.Split(';');
            var toAdd = symbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
            var toRemove = symbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));
            var modified = s_DefineConstants.Match(content).Groups[1].Value.Split(';')
                .Union(toAdd)
                .Except(toRemove)
                .Union(setting.OpenSesame ? new[] { "OPEN_SESAME" } : Enumerable.Empty<string>())
                .Distinct()
                .ToArray();

            return s_DefineConstants.Replace(content, string.Format("<DefineConstants>{0}</DefineConstants>", string.Join(";", modified)));
        }
    }
}
