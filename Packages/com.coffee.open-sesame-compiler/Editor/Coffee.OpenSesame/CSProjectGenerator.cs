using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.OpenSesamePortable;
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.OpenSesame
{
    internal class CSProjectGenerator : AssetPostprocessor
    {
        static readonly Regex s_DefineConstants = new Regex("<DefineConstants>(.*)</DefineConstants>", RegexOptions.Compiled);

        static string OnGeneratedCSProject(string path, string content)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(path);
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            var setting = OpenSesameSetting.GetAtPathOrDefault(asmdefPath);
            if (string.IsNullOrEmpty(setting.ModifySymbols) && !setting.OpenSesame)
                return content;

            var modifySymbols = setting.ModifySymbols.Split(';');
            var add = modifySymbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
            var remove = modifySymbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));

            var defines = s_DefineConstants.Match(content).Groups[1].Value.Split(';');
            var isInternal = Core.IsInternalAssembly(assemblyName);

            defines = Core.ModifyDefines(defines, setting.OpenSesame && !isInternal, add, remove);

            return s_DefineConstants.Replace(content, string.Format("<DefineConstants>{0}</DefineConstants>", string.Join(";", defines)));
        }
    }
}
