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
    internal class OpenSesameInstaller
    {
        const string kCompilerVersion = "3.4.0";
        static string kLogHeader = "";

        static void Log(string format, params object[] args)
        {
            if (Core.LogEnabled)
                UnityEngine.Debug.LogFormat("<color=#0063b1><b>[OpenSesameInstaller]</b></color> " + format, args);
        }

        public static string GetInstalledCompiler()
        {
            return InstallCompiler(kCompilerVersion);
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
                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;
            }

            // Extract zip.
            string args = string.Format("x {0} -o{1}", dowloadPath, installPath);
            string exe = Path.Combine(EditorApplication.applicationContentsPath,
                Application.platform == RuntimePlatform.WindowsEditor ? "Tools\\7z.exe" : "Tools/7za");
            UnityEngine.Debug.LogFormat(kLogHeader + "Extract {0} to {1} with 7z command: {2} {3}", dowloadPath, installPath, exe, args);
            Process.Start(exe, args).WaitForExit();

            if (File.Exists(cscToolExe))
                return cscToolExe;

            throw new Exception(kLogHeader + "Open Sesame compiler is not found at " + cscToolExe);
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }
    }
}
