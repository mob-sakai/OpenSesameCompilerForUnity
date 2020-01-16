using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Coffee.OpenSesamePortable;
using UnityEditor;

namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal class Bootstrap
    {
        const string kAssemblyName = "Coffee.OpenSesame";
        const string kInstallerFullName = "Coffee.OpenSesame.OpenSesameLanguageInstaller, " + kAssemblyName;

        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat("<color=#c34062><b>[OpenSesameBootstrap]</b></color> " + format, args);
        }

        [Conditional("OPEN_SESAME_LOG")]
        static void Warning(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat("<color=#c34062><b>[OpenSesameBootstrap]</b></color> " + format, args);
        }

        static Bootstrap()
        {
            // OpenSesameLanguageInstaller has been loaded.
            if (Type.GetType(kInstallerFullName) != null || Type.GetType(kInstallerFullName + ".OSC") != null)
            {
                Warning("OpenSesameLanguageInstaller has been loaded. Skip bootstrap task.", kInstallerFullName);
                return;
            }

            Log("Start bootstrap task");
            try
            {
                // Install OpenSesame compiler to project.
                var cscToolExe = Core.GetInstalledCompiler();

                // Compile the assembly with OpenSesame compiler.
                var outputAssemblyPath = Core.CompileAssembly(kAssemblyName, cscToolExe);

                // Load and Initialize the compiled assembly.
                var tmp = Path.GetTempFileName() + ".dll";
                File.Move(outputAssemblyPath, tmp);
                Core.InitializeAssemblyOnLoad(Assembly.LoadFrom(tmp));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
