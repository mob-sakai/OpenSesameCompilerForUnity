using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal static class OpenSesameInstaller
    {
        static bool sIsInstallFailed;
        public const string Version = "3.4.0";
        static string kLogHeader = "<color=#c34062><b>[OpenSesameInstaller]</b></color> ";

        static void Log(string format, params object[] args)
        {
            if (Core.LogEnabled)
                UnityEngine.Debug.LogFormat(kLogHeader + format, args);
        }

        public static string GetInstalledCompiler()
        {
            if (sIsInstallFailed)
                return "";

            try
            {
                return InstallCompiler(Version);
            }
            catch (Exception ex)
            {
                sIsInstallFailed = true;
                UnityEngine.Debug.LogException(new Exception(kLogHeader + ex.Message, ex.InnerException));
                return "";
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        static string InstallCompiler(string version)
        {
            string packageId = "OpenSesameCompiler." + version;
            string url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
            string dowloadPath = Path.GetTempFileName() + ".nuget";
            string installPath = ("Library/" + packageId).Replace('/', Path.DirectorySeparatorChar);
            string cscToolExe = (installPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);


            // OpenSesame compiler is already installed.
            if (File.Exists(cscToolExe))
            {
                Log("{0} is already installed at {1}", packageId, cscToolExe);
                return cscToolExe;
            }

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            // Download csc from nuget.
            UnityEngine.Debug.LogFormat(kLogHeader + "Download {0} from nuget: {1}", packageId, url);
            EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Download {0} from nuget", packageId), 0.2f);
            try
            {
                using (var client = new WebClient())
                    client.DownloadFile(url, dowloadPath);
            }
            catch
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidation;
                    client.DownloadFile(url, dowloadPath);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;
            }

            // Extract zip.
            string args = string.Format("x {0} -o{1}", dowloadPath, installPath);
            string exe = Path.Combine(EditorApplication.applicationContentsPath,
                Application.platform == RuntimePlatform.WindowsEditor ? "Tools\\7z.exe" : "Tools/7za");
            UnityEngine.Debug.LogFormat(kLogHeader + "Extract {0} to {1} with 7z command: {2} {3}", dowloadPath, installPath, exe, args);
            EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Extract {0}", dowloadPath), 0.4f);
            Process.Start(exe, args).WaitForExit();

            if (File.Exists(cscToolExe))
                return cscToolExe;

            throw new FileNotFoundException("Open Sesame compiler is not found at " + cscToolExe);
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }
    }
}
