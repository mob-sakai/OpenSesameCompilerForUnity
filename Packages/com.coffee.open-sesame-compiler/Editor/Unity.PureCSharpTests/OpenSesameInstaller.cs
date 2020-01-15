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
using System.Diagnostics;

namespace Coffee.OpenSesameCompilers
{
    [InitializeOnLoad]
    internal class OpenSesameInstaller
    {

        const string oscVersion = "3.4.0-beta.4";
        const string oscName = "OpenSesameCompiler";
        const string oscPackageId = oscName + "." + oscVersion;
        static readonly string oscDownloadUrl = "https://globalcdn.nuget.org/packages/" + oscPackageId.ToLower() + ".nupkg";
        //static readonly string oscDownloadPath = ("Temp/" + oscPackageId.ToLower() + ".zip").Replace('/', Path.DirectorySeparatorChar);
        static readonly string oscInstallPath = ("Library/" + oscPackageId).Replace('/', Path.DirectorySeparatorChar);
        static readonly string csc = (oscInstallPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);


        //const string version = "3.4.0-beta.4";
        //const string package = "OpenSesameCompiler";
        //const string packageId = package + "." + version;
        //static readonly string url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
        //static readonly string downloadPath = ("Temp/" + packageId + ".zip").Replace('/', Path.DirectorySeparatorChar);
        //static readonly string extractPath = ("Library/" + packageId).Replace('/', Path.DirectorySeparatorChar);
        //static readonly string csc = (extractPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);
#if UNITY_EDITOR_WIN
        static readonly string exe7z = EditorApplication.applicationContentsPath + "\\Tools\\7z.exe";
#else
        static readonly string exe7z = EditorApplication.applicationContentsPath + "/Tools/7za";
#endif

        static OpenSesameInstaller()
        {
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
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> {0} is already installed: {1}", oscPackageId, csc);
                return csc;
            }

            try
            {
                var oscDownloadPath = Path.GetTempFileName() + ".nuget";
                if (Directory.Exists(oscInstallPath))
                    Directory.Delete(oscInstallPath, true);

                // Download csc from nuget.
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Download {0} from nuget: {1}", oscPackageId, oscDownloadUrl);
                EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Download {0} from nuget", oscPackageId), 0.5f);
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(oscDownloadUrl, oscDownloadPath);
                    }
                }
                catch
                {
                    Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Download {0} with server certificate validation", oscPackageId);
                    using (var client = new WebClient())
                    {
                        ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidation;
                        client.DownloadFile(oscDownloadUrl, oscDownloadPath);
                    }
                }

                // Extract zip.
                Debug.LogFormat("<b>[OpenSesame]</b><color=magenta>[Installer]</color> Extract {0} to {1}", oscDownloadPath, oscInstallPath);
                EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Extract {0}", oscDownloadPath), 0.8f);
                Process.Start(exe7z, string.Format("x {0} -o{1}", oscDownloadPath, oscInstallPath)).WaitForExit();

                return csc;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;
            }
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }
    }
}
