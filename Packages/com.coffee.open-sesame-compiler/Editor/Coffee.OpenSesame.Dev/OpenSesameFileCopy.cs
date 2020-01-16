using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Coffee.OpenSesame.Dev
{
    [InitializeOnLoad]
    class OpenSesameFileDuplicator : AssetPostprocessor
    {
        const string packagePath = "Packages/com.coffee.open-sesame-compiler";
        const string openSesamePath = packagePath + "/Editor/Coffee.OpenSesame";
        const string bootstrapPath = packagePath + "/Editor/Coffee.OpenSesame.Bootstrap";
        const string portableTestPath = "Assets/Tests/Portable";

        static OpenSesameFileDuplicator()
        {
            // Duplicate Coffee.OpenSesame.Bootstrap.dll
            CompilationPipeline.assemblyCompilationFinished += (name, messages) =>
            {
                if (Path.GetFileName(name) != "Coffee.OpenSesame.Bootstrap.dll")
                    return;

                Debug.Log("[FileDuplicator] Coffee.OpenSesame.Bootstrap.dll");
                File.Copy(name, packagePath + "/Editor/Coffee.OpenSesame.Bootstrap.dll", true);
            };

            // Duplicate OpenSesamePortable.cs
            CompilationPipeline.assemblyCompilationStarted += name =>
            {
                if (Path.GetFileNameWithoutExtension(name) != "Coffee.OpenSesame")
                    return;

                Debug.Log("[FileDuplicator] OpenSesamePortable.cs");
                File.Copy(openSesamePath + "/OpenSesamePortable.cs", bootstrapPath + "/OpenSesamePortable.cs", true);
                File.Copy(openSesamePath + "/OpenSesamePortable.cs", portableTestPath + "/OpenSesamePortable.cs", true);

            };
        }
    }
}
