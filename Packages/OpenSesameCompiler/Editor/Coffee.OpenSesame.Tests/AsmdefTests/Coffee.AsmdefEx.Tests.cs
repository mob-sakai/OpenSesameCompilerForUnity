#if IGNORE_ACCESS_CHECKS // DO NOT REMOVE THIS LINE MANUALLY.
using System.Linq;
using NUnit.Framework;
using UnityEditor.Scripting.ScriptCompilation;

namespace Coffee.AsmdefEx.Tests
{
    class ScriptAssemblyTests
    {   
        [Test]
        public void GetScriptAssembly()
        {
            var assemblyName = nameof(ScriptAssemblyTests);
            ScriptAssembly assembly = Core.GetScriptAssembly(assemblyName) as ScriptAssembly;
            Assert.IsNotNull(assembly);
            Assert.AreEqual(assembly.Filename, $"{assemblyName}.dll");
        }
    }

    class SymbolTests
    {  
        [Test]
        public void IgnoreAccessChecksSymbol()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "");
            var expected = defines.Union(new []{"IGNORE_ACCESS_CHECKS"});
            
            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void AddSymbols()
        {
            var defines = new []{"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "SYM_ADD1;SYM_ADD2");
            var expected = new []{"SYM_A", "SYM_B", "SYM_C", "SYM_ADD1", "SYM_ADD2", "IGNORE_ACCESS_CHECKS"};
            
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
            var expected = new []{"SYM_A", "SYM_C", "SYM_ADD1", "IGNORE_ACCESS_CHECKS"};
            
            CollectionAssert.AreEqual(expected, modified);
        }
    }
}
#endif // IGNORE_ACCESS_CHECKS: DO NOT REMOVE THIS LINE MANUALLY.