#if !OPEN_SESAME
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coffee.OpenSesame
{
    internal static class ReflectionExtensions
    {
        static object Inst(this object self)
        {
            return (self is Type) ? null : self;
        }

        static Type Type(this object self)
        {
            return (self as Type) ?? self.GetType();
        }

        public static object New(this Type self, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetConstructor(types)
                .Invoke(null, args);
        }

        public static object Call(this object self, string methodName, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetMethod(methodName, types)
                .Invoke(self.Inst(), args);
        }

        public static object Call(this object self, Type[] genericTypes, string methodName, params object[] args)
        {
            return self.Type().GetMethod(methodName)
                .MakeGenericMethod(genericTypes)
                .Invoke(self.Inst(), args);
        }

        public static object Get(this object self, string memberName, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName)[0];
            return mi is PropertyInfo
                ? (mi as PropertyInfo).GetValue(self.Inst(), new object[0])
                : (mi as FieldInfo).GetValue(self.Inst());
        }

        public static void Set(this object self, string memberName, object value, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName)[0];
            if (mi is PropertyInfo)
                (mi as PropertyInfo).SetValue(self.Inst(), value, new object[0]);
            else
                (mi as FieldInfo).SetValue(self.Inst(), value);
        }
    }

    [InitializeOnLoad]
    internal class PreCompiler
    {
        static readonly Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
        static readonly Type tCSharpLanguage = Type.GetType("UnityEditor.Scripting.Compilers.CSharpLanguage, UnityEditor");
        static readonly Type tMicrosoftCSharpCompiler = Type.GetType("UnityEditor.Scripting.Compilers.MicrosoftCSharpCompiler, UnityEditor");
        static readonly Type tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
        static readonly Type tEditorScriptCompilationOptions = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorScriptCompilationOptions, UnityEditor");
        static readonly Type tScriptAssembly = Type.GetType("UnityEditor.Scripting.ScriptCompilation.ScriptAssembly, UnityEditor");
        static readonly Type tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
        static readonly object BuildingForEditor = Enum.Parse(tEditorScriptCompilationOptions, "BuildingForEditor");
        static readonly FieldInfo fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

        static string assemblyName = "Coffee.OpenSesame.Portable";
        static string outputPath = "Temp/" + assemblyName + ".OSC.dll";
        //static string installerFullName = "Coffee.OpenSesame.Portable, " + assemblyName + ".OSC";

        const string oscVersion = "3.4.0-beta.4";
        const string oscName = "OpenSesameCompiler";
        const string oscPackageId = oscName + "." + oscVersion;
        static readonly string oscDownloadUrl = "https://globalcdn.nuget.org/packages/" + oscPackageId.ToLower() + ".nupkg";
        //static readonly string oscDownloadPath = ("Temp/" + oscPackageId.ToLower() + ".zip").Replace('/', Path.DirectorySeparatorChar);
        static readonly string oscInstallPath = ("Library/" + oscPackageId).Replace('/', Path.DirectorySeparatorChar);
        static readonly string csc = (oscInstallPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);

#if UNITY_EDITOR_WIN
        static readonly string exe7z = EditorApplication.applicationContentsPath + "\\Tools\\7z.exe";
#else
        static readonly string exe7z = EditorApplication.applicationContentsPath + "/Tools/7za";
#endif

        static void InstallCompiler()
        {
            // Modified compiler is already installed.
            if (File.Exists(csc))
                return;

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
                Debug.LogFormat("<b>[OpenSesamePortable]</b><color=magenta>[InstallCompiler]</color> Extract {0} to {1}", oscDownloadPath, oscInstallPath);
                EditorUtility.DisplayProgressBar("Open Sesame Portable", string.Format("Extract {0}", oscDownloadPath), 0.4f);
                Process.Start(exe7z, string.Format("x {0} -o{1}", oscDownloadPath, oscInstallPath)).WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;
            }
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }

        static object CreateCompiler(string assemblyName)
        {
            //  -> EditorCompilationInterface.GetScriptAssemblyForLanguage<CSharpLanguage>(assemblyName);
            var scriptAssembly = tEditorCompilationInterface.Call(new[] { tCSharpLanguage }, "GetScriptAssemblyForLanguage", assemblyName);
            if (scriptAssembly == null)
                throw new Exception(string.Format("ScriptAssembly '{0}' is not exist.", assemblyName));

            scriptAssembly.Set("Defines", (scriptAssembly.Get("Defines") as string[]).Concat(new[] { "OPEN_SESAME" }).ToArray());
            scriptAssembly.Set("ScriptAssemblyReferences", Array.CreateInstance(tScriptAssembly, 0));
            scriptAssembly.Set("Filename", assemblyName + ".OSC.dll");

            // << Unity 2020.1 or later >>
            //  -> return new MicrosoftCSharpCompiler(scriptAssembly, "Temp");
            if (string.Compare("2020.1", Application.unityVersion) < 0)
                return tMicrosoftCSharpCompiler.New(scriptAssembly, "Temp");

            // << Unity 2019.3 or later >>
            //  -> return new MicrosoftCSharpCompiler(scriptAssembly, EditorScriptCompilationOptions.BuildingForEditor, "Temp");
            if (string.Compare("2019.3", Application.unityVersion) < 0)
                return tMicrosoftCSharpCompiler.New(scriptAssembly, BuildingForEditor, "Temp");

            // << Unity 2019.2 or earlier >>
            //  -> var island = scriptAssembly.ToMonoIsland(EditorScriptCompilationOptions.BuildingForEditor, "Temp", null);
            //  -> return new MicrosoftCSharpCompiler(island, true);
            var island = scriptAssembly.Call("ToMonoIsland", BuildingForEditor, "Temp", "");
            return tMicrosoftCSharpCompiler.New(island, true);
        }

        static void CompileAssembly(string assemblyName)
        {
            using (var compiler = CreateCompiler(assemblyName) as IDisposable)
            {
                compiler.Call("BeginCompiling");
                var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
                compiler.Call("Dispose");

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    psi.FileName = Path.GetFullPath(csc);
                    psi.Arguments = psi.Arguments.Replace("@Temp/", "@Temp\\");
                }
                else
                {
                    psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono").Replace('\\', '/');
                    psi.Arguments = csc + " " + psi.Arguments.Replace("/shared ", "");
                }

                var program = tProgram.New(psi);
                program.Call("Start");
                compiler.Set("process", program, fiProcess);

                compiler.Call("WaitForCompilationToFinish");
                var messages = compiler.Call("GetCompilerMessages") as IEnumerable;
                foreach (var m in messages)
                {
                    if ((int)m.Get("type") == 0)
                        Debug.LogError(m.Get("message"));
                    else
                        Debug.LogWarning(m.Get("message"));
                }
            }
        }

        static void InitializeOnLoad(Assembly assembly)
        {
            var types = assembly.GetTypes();
            var InitializeOnLoadTypes = types
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false).Length);
            foreach(var type in InitializeOnLoadTypes)
            {
                try
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            var methods = types
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadMethodAttribute), false).Length);
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        static PreCompiler()
        {
            try
            {
                // Install OpenSesame compiler. 
                EditorUtility.DisplayProgressBar("Open Sesame Portable", "Install OpenSesame package", 0f);
                InstallCompiler();

                // Compile same assembly with OpenSesame compiler.
                assemblyName = typeof(PreCompiler).Assembly.GetName().Name;
                outputPath = Path.Combine("Temp", assemblyName + ".OSC.dll");
                EditorUtility.DisplayProgressBar("Open Sesame Portable", "Compile assembly with OpenSesame compiler", 0.5f);
                CompileAssembly(assemblyName);

                // Load OpenSesame and install.
                EditorUtility.DisplayProgressBar("Open Sesame Portable", "Load assembly " + Path.GetFileName(outputPath), 0.9f);
                var tmp = Path.GetTempFileName() + ".dll";
                File.Move(outputPath, tmp);

                InitializeOnLoad(Assembly.LoadFrom(tmp));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
#else
using UnityEditor;
namespace Coffee.OpenSesame
{
    [InitializeOnLoad]
    internal class Portable
    {
        static Portable()
        {
            UnityEngine.Debug.Log("<b>うおおおおおおおおおおお</b>");
        }
    }
}
#endif