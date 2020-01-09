using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coffee.OpenSesameCompilers
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
            try
            {
                var argTypes = args.Select(x => x != null ? x.GetType() : null).ToArray();
                foreach (var cotr in self.GetConstructors())
                {
                    var pis = cotr.GetParameters();
                    if (pis.Length != argTypes.Length)
                        continue;

                    for (int i = 0; i < pis.Length; i++)
                    {
                        if (argTypes[i] != null && !pis[i].ParameterType.IsAssignableFrom(argTypes[i]))
                            break;

                        if (i == pis.Length - 1)
                            return cotr.Invoke(args);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} constructor is not assignable. {1}", self.Name, ex.Message));
            }

            throw new Exception(string.Format("{0} constructor is not assignable.", self.Name));
        }

        public static object Call(this object self, string methodName, params object[] args)
        {
            try
            {
                return self.Type().GetMethod(methodName).Invoke(self.Inst(), args);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("<{0}> {1} is not assignable. {2}", methodName, self.Type().GetMethod(methodName), ex.Message));
            }
        }

        public static object Call(this object self, Type[] genericTypes, string methodName, params object[] args)
        {
            try
            {
                return self.Type().GetMethod(methodName).MakeGenericMethod(genericTypes).Invoke(self.Inst(), args);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("<{0}> {1} is not assignable. {2}", methodName, self.Type().GetMethod(methodName), ex.Message));
            }
        }

        public static object Get(this object self, string memberName)
        {
            try
            {
                Type t = self.Type();
                if (self is Type && t.IsEnum)
                    return Enum.Parse(t, memberName);

                var pi = t.GetProperty(memberName);
                return pi != null
                    ? pi.GetValue(self.Inst(), new object[0])
                    : t.GetField(memberName).GetValue(self.Inst());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} is not assignable. {1}", memberName, ex.Message));
            }
        }

        public static void Set(this object self, string memberName, object value)
        {
            try
            {
                Type t = self.Type();
                var pi = t.GetProperty(memberName);
                if (pi != null)
                    pi.SetValue(self.Inst(), value, new object[0]);
                else
                    t.GetField(memberName).SetValue(self.Inst(), value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} is not assignable. {1}", memberName, ex.Message));
            }
        }
    }

    [InitializeOnLoad]
    internal class PreCompiler
    {
        static readonly Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
        static readonly Type tCSharpLanguage = Type.GetType("UnityEditor.Scripting.Compilers.CSharpLanguage, UnityEditor");
        static readonly Type tMicrosoftCSharpCompiler = Type.GetType("UnityEditor.Scripting.Compilers.MicrosoftCSharpCompiler, UnityEditor");
        static readonly Type tEditorScriptCompilationOptions = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorScriptCompilationOptions, UnityEditor");
        static readonly Type tScriptAssembly = Type.GetType("UnityEditor.Scripting.ScriptCompilation.ScriptAssembly, UnityEditor");
        static readonly object BuildingForEditor = tEditorScriptCompilationOptions.Get("BuildingForEditor");

        const string assemblName = "Unity.PureCSharpTests";
        const string assemblyPath = "Temp/" + assemblName + ".dll";
        const string installerFullName = "Coffee.OpenSesameCompilers.OpenSesameInstaller, " + assemblName;

        static object CreateCompiler(string assemblyName)
        {
            //  -> EditorCompilationInterface.GetScriptAssemblyForLanguage<CSharpLanguage>(assemblyName);
            object scriptAssembly = tEditorCompilationInterface.Call(new[] { tCSharpLanguage }, "GetScriptAssemblyForLanguage", assemblyName);
            if (scriptAssembly == null)
                throw new Exception(string.Format("ScriptAssembly '{0}' is not exist.", assemblyName));

            scriptAssembly.Set("ScriptAssemblyReferences", Array.CreateInstance(tScriptAssembly, 0));

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
            object island = scriptAssembly.Call("ToMonoIsland", BuildingForEditor, "Temp", null);
            return tMicrosoftCSharpCompiler.New(island, true);
        }

        static void CompileAssembly(string assemblyName)
        {
            using (var compiler = CreateCompiler(assemblyName) as IDisposable)
            {
                compiler.Call("BeginCompiling");
                compiler.Call("WaitForCompilationToFinish");
                var messages = compiler.Call("GetCompilerMessages") as IEnumerable;
                bool error = false;
                foreach (var m in messages)
                {
                    if ((int)m.Get("type") == 0)
                    {
                        error |= true;
                        Debug.LogError(m.Get("message"));
                    }
                    else
                    {
                        Debug.LogWarning(m.Get("message"));
                    }
                }
            }
        }

        static PreCompiler()
        {
            try
            {
                // OpenSesame is installed already.
                if (Type.GetType(installerFullName) != null)
                    return;

                EditorUtility.DisplayProgressBar("Open Sesame Installer", "Pre-compile OpenSesame package", 0.1f);

                // Compile OpenSesame
                CompileAssembly(assemblName);

                // Load OpenSesame and install.
                var tmp = Path.GetTempFileName() + ".dll";
                File.Move(assemblyPath.Replace('/', Path.DirectorySeparatorChar), tmp);
                Assembly.LoadFrom(tmp);
                RuntimeHelpers.RunClassConstructor(Type.GetType(installerFullName).TypeHandle);
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
