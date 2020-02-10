using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.OpenSesame.Dev
{
    [InitializeOnLoad]
    class OpenSesameFileDuplicator : AssetPostprocessor
    {
        const string packagePath = "Packages/com.coffee.open-sesame-compiler";
        const string openSesamePath = packagePath + "/Editor/Coffee.OpenSesame";
        const string portableTestPath = "Assets/Tests/PortableOpenSesameTests";

        static OpenSesameFileDuplicator()
        {
#if OPEN_SESAME_DEV
            //Duplicate OpenSesamePortable.cs
            CompilationPipeline.assemblyCompilationFinished += (name, _) =>
            {
                if (Path.GetFileNameWithoutExtension(name) != "Coffee.OpenSesame")
                    return;
                
                const string originDll = "Packages/com.coffee.open-sesame-compiler/Editor/Coffee.OpenSesame.dll";
                CopyFileIfUpdated(name, Path.GetFullPath(originDll));
            };
#endif
        }

        static void CopyFileIfUpdated(string src, string dst)
        {
            if (!File.Exists(src))
                return;

            if (File.Exists(dst))
            {
                using (var srcFs = new FileStream(src, FileMode.Open))
                using (var dstFs = new FileStream(dst, FileMode.Open))
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    if (md5.ComputeHash(srcFs).SequenceEqual(md5.ComputeHash(dstFs)))
                    {
                        return;
                    }
                }
            }

            File.Copy(src, dst, true);
        }
    }
}
