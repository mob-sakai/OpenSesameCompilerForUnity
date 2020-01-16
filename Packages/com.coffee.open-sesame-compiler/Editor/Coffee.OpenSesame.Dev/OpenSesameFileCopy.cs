using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

using UnityEditor.Compilation;

namespace Coffee.OpenSesame.Dev
{
	[InitializeOnLoad]
	class OpenSesameFileCopy : AssetPostprocessor
	{
        const string packagePath = "Packages/com.coffee.open-sesame-compiler";
        const string openSesamePath = packagePath + "/Editor/Coffee.OpenSesame";
        const string bootstrapPath = packagePath + "/Editor/Coffee.OpenSesame.Bootstrap";
        const string portableTestPath = "Assets/Tests/Portable";

        static OpenSesameFileCopy()
		{
			CompilationPipeline.assemblyCompilationFinished += (name, messages) =>
			{
				if (Path.GetFileName(name) != "Coffee.OpenSesame.Bootstrap.dll")
					return;

				Debug.LogFormat("{0}", name);
				Debug.Log("ここでdllコピー");
                FileUtil.CopyFileOrDirectory(name, packagePath + "/Editor/Coffee.OpenSesame.Bootstrap.dll");
			};

			CompilationPipeline.assemblyCompilationStarted += name =>
			{
				if (Path.GetFileNameWithoutExtension(name) != "Coffee.OpenSesame")
					return;

				Debug.LogFormat("{0}", name);
				Debug.Log("ここでcsコピー");
                FileUtil.CopyFileOrDirectory(openSesamePath + "/OpenSesamePortable.cs", bootstrapPath + "/OpenSesamePortable.cs");

            };
		}
	}
}
