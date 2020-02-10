using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Coffee.OpenSesame;
using UnityEditor.Scripting.ScriptCompilation;

namespace OpenSesameAsmdeflTests
{
    class Tests
    {
        [Test]
        public void GetScriptAssembly()
        {
            var assemblyName = nameof(OpenSesameAsmdeflTests);
            ScriptAssembly assembly = Core.GetScriptAssembly(assemblyName) as ScriptAssembly;
            Assert.IsNotNull(assembly);
            Assert.AreEqual(assembly.Filename, $"{assemblyName}.dll");
        }

        [Test]
        public void OpenSesameSymbol()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "");
            var expected = defines.Union(new []{"OPEN_SESAME"});
            
            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void AddSymbols()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "SYM_ADD1;SYM_ADD2");
            var expected = new []{"SYM_A", "SYM_B", "SYM_C", "SYM_ADD1", "SYM_ADD2", "OPEN_SESAME"};
            
            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void SubSymbols()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, false, "!SYM_B");
            var expected = new []{"SYM_A", "SYM_C"};
            
            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void ModifySymbols()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "SYM_ADD1;!SYM_B");
            var expected = new []{"SYM_A", "SYM_C", "SYM_ADD1", "OPEN_SESAME"};
            
            CollectionAssert.AreEqual(expected, modified);
        }
    }
}