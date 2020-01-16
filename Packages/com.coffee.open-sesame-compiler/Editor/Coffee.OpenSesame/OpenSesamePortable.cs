#if !OPEN_SESAME
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace Coffee.OpenSesamePortable
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

    internal class Core
    {
        const string kLogHeader = "<b>[OpenSesame]</b><color=#9a4089>[Core]</color> ";
        static readonly string[] kOpenSesameAssemblies = new []
        {
            "Coffee.OpenSesame",
            "Coffee.OpenSesame.OSC",
            "Coffee.OpenSesame.Bootstrap",
            "Coffee.OpenSesame.Bootstrap.OSC",
            "Unity.PureCSharpTests",
            "Unity.PureCSharpTests.OSC",
        };

        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(kLogHeader + format, args);
        }

        public static string GetInstalledCompiler()
        {
            return InstallCompiler("3.4.0-beta.4");
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
            Log("Download {0} from nuget: {1}", packageId, url);
            EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Download {0} from nuget", packageId), 0.2f);
            try
            {
                using (var client = new WebClient())
                    client.DownloadFile(url, dowloadPath);
            }
            catch
            {
                Log("Download {0} with server certificate validation", packageId);
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
            EditorUtility.DisplayProgressBar("Open Sesame Portable", string.Format("Extract {0}", dowloadPath), 0.4f);
            string args = string.Format("x {0} -o{1}", dowloadPath, installPath);
            string exe = Path.Combine(EditorApplication.applicationContentsPath,
                Application.platform == RuntimePlatform.WindowsEditor ? "Tools\\7z.exe" : "Tools/7za");
            Log("Extract {0} to {1} with 7z command: {2} {3}", dowloadPath, installPath, exe, args);
            Process.Start(exe, args).WaitForExit();

            if (File.Exists(cscToolExe))
                return cscToolExe;

            throw new Exception(kLogHeader + "Open Sesame compiler is not found at " + cscToolExe);
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }

        public static bool IsInternalAssembly(string assemblyName)
        {
            return kOpenSesameAssemblies.Contains(assemblyName);
        }

        public static object CreateCompiler(string assemblyName, out string outputPath)
        {
            Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
            Type tCSharpLanguage = Type.GetType("UnityEditor.Scripting.Compilers.CSharpLanguage, UnityEditor");
            Type tMicrosoftCSharpCompiler = Type.GetType("UnityEditor.Scripting.Compilers.MicrosoftCSharpCompiler, UnityEditor");
            Type tEditorScriptCompilationOptions = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorScriptCompilationOptions, UnityEditor");
            Type tScriptAssembly = Type.GetType("UnityEditor.Scripting.ScriptCompilation.ScriptAssembly, UnityEditor");
            object options = Enum.Parse(tEditorScriptCompilationOptions, "BuildingForEditor");

            //  -> EditorCompilationInterface.GetScriptAssemblyForLanguage<CSharpLanguage>(assemblyName);
            var scriptAssembly = tEditorCompilationInterface.Call(new[] { tCSharpLanguage }, "GetScriptAssemblyForLanguage", assemblyName);
            if (scriptAssembly == null)
                throw new Exception(string.Format("[OpenSesamePortable] ScriptAssembly '{0}' is not exist.", assemblyName));

            // This assembly is internal assembly of OpenSesame. 
            //   - No 'OPEN_SESAME' define symbol.
            //   - No '.OSC' suffix.
            bool isInternal = IsInternalAssembly(assemblyName);

            scriptAssembly.Set("ScriptAssemblyReferences", Array.CreateInstance(tScriptAssembly, 0));
            var defines = ModifyDefines(scriptAssembly.Get("Defines") as string[], !isInternal);
            scriptAssembly.Set("Defines", defines);
            scriptAssembly.Set("Filename", isInternal ? assemblyName + ".dll" : assemblyName + ".OSC.dll");

            outputPath = Path.Combine("Temp", scriptAssembly.Get("Filename") as string);

            Log("Create compiler for {0}", outputPath);

            // << Unity 2020.1 or later >>
            //  -> return new MicrosoftCSharpCompiler(scriptAssembly, "Temp");
            if (string.Compare("2020.1", Application.unityVersion) < 0)
                return tMicrosoftCSharpCompiler.New(scriptAssembly, "Temp");

            // << Unity 2019.3 or later >>
            //  -> return new MicrosoftCSharpCompiler(scriptAssembly, EditorScriptCompilationOptions.BuildingForEditor, "Temp");
            if (string.Compare("2019.3", Application.unityVersion) < 0)
                return tMicrosoftCSharpCompiler.New(scriptAssembly, options, "Temp");

            // << Unity 2019.2 or earlier >>
            //  -> var island = scriptAssembly.ToMonoIsland(EditorScriptCompilationOptions.BuildingForEditor, "Temp", null);
            //  -> return new MicrosoftCSharpCompiler(island, true);
            var island = scriptAssembly.Call("ToMonoIsland", options, "Temp", "");
            return tMicrosoftCSharpCompiler.New(island, true);
        }

        public static string[] ModifyDefines(IEnumerable<string> defines, bool openSesame, IEnumerable<string> add = null, IEnumerable<string> remove = null)
        {
            return defines
                .Union(add ?? Enumerable.Empty<string>())
                .Except(remove ?? Enumerable.Empty<string>())
                .Union(openSesame ? new[] { "OPEN_SESAME" } : Enumerable.Empty<string>())
                .Distinct()
                .ToArray();
        }

        public static string CompileAssembly(string assemblyName, string cscToolExe = null)
        {
            Type tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            Type tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            FieldInfo fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

            string outputPath;
            EditorUtility.DisplayProgressBar("Open Sesame Pre-compile", "Compile assembly with OpenSesame compiler", 0.5f);
            using (var compiler = CreateCompiler(assemblyName, out outputPath) as IDisposable)
            {
                Log("Compile assembly for {0}", Path.GetFileName(outputPath));
                compiler.Call("BeginCompiling");
                ChangeCompiler(compiler, cscToolExe);
                compiler.Call("WaitForCompilationToFinish");
                var messages = compiler.Call("GetCompilerMessages") as IEnumerable;
                foreach (var m in messages)
                {
                    if ((int)m.Get("type") == 0)
                        UnityEngine.Debug.LogError(m.Get("message"));
                    else
                        UnityEngine.Debug.LogWarning(m.Get("message"));
                }

                Log("Compilation is completed: success={0}, outputPath={1}",
                    !messages.Cast<object>().Any(x => (int)x.Get("type") == 0),
                    outputPath);
            }
            return outputPath;
        }

        public static object ChangeCompiler(object compiler, string cscToolExe = null)
        {
            Type tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            Type tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            FieldInfo fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

            if (string.IsNullOrEmpty(cscToolExe))
                return compiler.Get("process", fiProcess);

            Log("Change csc tool exe to {0}", cscToolExe);
            var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
            compiler.Call("Dispose");

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                psi.FileName = Path.GetFullPath(cscToolExe);
                psi.Arguments = psi.Arguments.Replace("@Temp/", "@Temp\\");
            }
            else
            {
                psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono").Replace('\\', '/');
                psi.Arguments = cscToolExe + " " + psi.Arguments.Replace("/shared ", "");
            }

            Log("Restart compile process: {0} {1}", psi.FileName, psi.Arguments);
            var program = tProgram.New(psi);
            program.Call("Start");
            compiler.Set("process", program, fiProcess);

            return program;
        }

        public static void InitializeAssemblyOnLoad(Assembly assembly)
        {
            EditorUtility.DisplayProgressBar("Open Sesame Portable", "Initialize assembly on load " + assembly.GetName().Name, 0.9f);
            Log("Initialize assembly on load: {0}", assembly.FullName);
            var types = assembly.GetTypes();
            var InitializeOnLoadTypes = types
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false).Length);
            foreach (var type in InitializeOnLoadTypes)
            {
                try
                {
                    Log("Initialize on load: {0}", type.FullName);
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            var methods = types
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadMethodAttribute), false).Length);
            foreach (var method in methods)
            {
                try
                {
                    Log("Initialize on load method: {0}.{1}", method.DeclaringType, method.Name);
                    method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }
    }

    [InitializeOnLoad]
    internal class Bootstrap
    {
        static string kLogHeader = "";

        [Conditional("OPEN_SESAME_LOG")]
        static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(kLogHeader + format, args);
        }

        static Bootstrap()
        {
            var assemblyName = typeof(Bootstrap).Assembly.GetName().Name;
            kLogHeader = string.Format("<b><color=#c7634c>[PortableBootstrap ({0})]</color></b> ", assemblyName);
            if (Core.IsInternalAssembly(assemblyName))
            {
                Log("This assembly is internal in OpenSesameCompiler. Skip portable bootstrap task");
                return;
            }

            Log("Start portable bootstrap task");
            try
            {
                // Install OpenSesame compiler to project.
                var cscToolExe = Core.GetInstalledCompiler();

                // Compile this assembly with OpenSesame compiler.
                var outputAssemblyPath = Core.CompileAssembly(assemblyName, cscToolExe);

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
#endif