using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEditor;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.ScriptCompilation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Coffee.OpenSesameCompilers
{
    [InitializeOnLoad]
    internal class OpenSesameInstaller
    {
        const string version = "3.4.0-beta.2";
        const string package = "OpenSesameCompiler";
        const string packageId = package + "." + version;
        static readonly string url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
        static readonly string downloadPath = ("Temp/" + packageId + ".zip").Replace('/', Path.DirectorySeparatorChar);
        static readonly string extractPath = ("Library/" + packageId).Replace('/', Path.DirectorySeparatorChar);
        static readonly string csc = (extractPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);

        static OpenSesameInstaller()
        {
			//

			var PublishOrigin = OpenSesameSetting.PublishOrigin;
			var PublishAssemblyName = OpenSesameSetting.PublishAssemblyName;
			OpenSesameSetting.PublishOrigin = null;
			OpenSesameSetting.PublishAssemblyName = null;
			if(!string.IsNullOrEmpty(PublishOrigin) && !string.IsNullOrEmpty(PublishAssemblyName))
			{
				Debug.Log("PUBLISH");
				Debug.Log("Library/ScriptAssemblies/" + PublishAssemblyName);
				Debug.Log(Path.Combine(Path.GetDirectoryName(PublishOrigin.TrimEnd('/')), PublishAssemblyName));
				FileUtil.UnityFileCopy("Library/ScriptAssemblies/" + PublishAssemblyName, Path.Combine(Path.GetDirectoryName(PublishOrigin.TrimEnd('/')), PublishAssemblyName), true);
			}
			

				//
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

        public static string Install()
        {
            // Modified compiler is already installed.
            if (File.Exists(csc))
            {
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> {0} is already installed: {1}", packageId, csc);
                return csc;
            }

            try
            {
                if (File.Exists(downloadPath))
                    File.Delete(downloadPath);

                // Download csc from nuget.
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Download {0} from nuget: {1}", packageId, url);
                EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Download {0} from nuget", packageId), 0.5f);
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(url, downloadPath);
                    }
                }
                catch
                {
                    Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Download {0} with server certificate validation", packageId);
                    using (var client = new WebClient())
                    {
                        ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidation;
                        client.DownloadFile(url, downloadPath);
                    }
                }

                // Extract zip.
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Extract {0} to {1}", downloadPath, extractPath);
                EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Extract {0}", downloadPath), 0.8f);
                using (var unzip = new Unzip(downloadPath))
                {
                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);

                    unzip.ExtractToDirectory(extractPath);
                }

                return csc;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);

                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);

                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;

                if (File.Exists(downloadPath))
                    File.Delete(downloadPath);
            }
        }

        private static bool OnServerCertificateValidation(Object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }
    }
}
