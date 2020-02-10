using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace Coffee.OpenSesame
{
    [System.Serializable]
    internal class OpenSesameSetting
    {
        public static string AssemblyNameToPublish;
        public bool OpenSesame;
        public string ModifySymbols = "";

        public bool SholdChangeCompilerProcess { get { return OpenSesame || !string.IsNullOrEmpty(ModifySymbols); } }
        public bool SholdModifyDefines { get { return OpenSesame || !string.IsNullOrEmpty(ModifySymbols); } }

        public static OpenSesameSetting GetAtPath(string path)
        {
            var setting = new OpenSesameSetting();
            if (string.IsNullOrEmpty(path))
                return setting;

            // If input path is directory, find asmdef file.
            if (Directory.Exists(path))
                path = Directory.GetFiles(path, "*.asmdef")
                    .Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, ""))
                    .FirstOrDefault();

            // Not find asmdef file.
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || !File.Exists(path + ".meta"))
                return setting;

            try
            {
                var json = AssetImporter.GetAtPath(path).userData;
                GetOpenSesameSettingsFromJson(json, out setting.OpenSesame, out setting.ModifySymbols);
            }
            catch { }
            return setting;
        }

        public static OpenSesameSetting CreateFromJson(string json = "")
        {
            var setting = new OpenSesameSetting();
            GetOpenSesameSettingsFromJson(json, out setting.OpenSesame, out setting.ModifySymbols);
            return setting;
        }

        static void GetOpenSesameSettingsFromJson(string json, out bool openSesame, out string modifySymbols)
        {
            openSesame = false;
            modifySymbols = "";
            if (string.IsNullOrEmpty(json))
                return;

            openSesame = Regex.Match(json, "\"OpenSesame\":\\s*(true|false)").Groups[1].Value == "true";
            modifySymbols = Regex.Match(json, "\"ModifySymbols\":\\s*\"([^\"]*)\"").Groups[1].Value;
        }
    }
}
