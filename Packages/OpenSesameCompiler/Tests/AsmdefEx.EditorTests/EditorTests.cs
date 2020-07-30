#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.AsmdefEx.EditorTests
{
#if UNITY_EDITOR
    class UnityEditorTest
    {
        [UnityTest]
        public IEnumerator InternalMethod()
        {
            LogAssert.Expect(LogType.Log, "delayed");
            EditorApplication.CallDelayed(() => Debug.Log("delayed"), 1);
            yield return null;

            float startTime = Time.realtimeSinceStartup;
            while ((Time.realtimeSinceStartup - startTime) < 1.1f)
            {
                yield return null;
            }
        }

        [Test]
        public void PrivateCreateInstance()
        {
            var tracker = new EditorGUIUtility.EditorLockTracker();
            Assert.IsNotNull(tracker);
        }


        [Test]
        public void PrivateField()
        {
            var tracker = new EditorGUIUtility.EditorLockTracker();
            tracker.m_IsLocked = true;
            Assert.AreEqual(tracker.isLocked, true);
        }

        [Test]
        public void PrivateProperty()
        {
            var tracker = new EditorGUIUtility.EditorLockTracker();
            tracker.isLocked = true;
            Assert.AreEqual(tracker.isLocked, true);
        }

        [Test]
        public void PrivateEvent()
        {
            var tracker = new EditorGUIUtility.EditorLockTracker();
            tracker.lockStateChanged.AddListener(locked => { Debug.Log($"locked: {locked}"); });
            LogAssert.Expect(LogType.Log, "locked: True");
            tracker.isLocked = true;
        }

        [Test]
        public void PrivateConstant()
        {
            Assert.AreEqual(EditorGUIUtility.EditorLockTracker.k_LockMenuText, "Lock");
        }

        [Test]
        public void GenericAdd()
        {
            var list = new List<EditorGUIUtility.EditorLockTracker>();
            list.Add(new EditorGUIUtility.EditorLockTracker());

            Assert.AreEqual(list.Count, 1);
        }
    }
#endif

    class ScriptAssemblyTests
    {
        [Test]
        public void GetScriptAssembly()
        {
            var assemblyName = typeof(ScriptAssemblyTests).Assembly.GetName().Name;
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
            var defines = new[] {"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "");
            var expected = defines.Union(new[] {"IGNORE_ACCESS_CHECKS", "ASMDEF_EX"});

            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void AddSymbols()
        {
            var defines = new[] {"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "SYM_ADD1;SYM_ADD2");
            var expected = new[] {"SYM_A", "SYM_B", "SYM_C", "SYM_ADD1", "SYM_ADD2", "IGNORE_ACCESS_CHECKS", "ASMDEF_EX"};

            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void SubSymbols()
        {
            var defines = new[] {"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, false, "!SYM_B");
            var expected = new[] {"SYM_A", "SYM_C", "ASMDEF_EX"};

            CollectionAssert.AreEqual(expected, modified);
        }

        [Test]
        public void ModifySymbols()
        {
            var defines = new[] {"SYM_A", "SYM_B", "SYM_C"};
            var modified = Core.ModifyDefines(defines, true, "SYM_ADD1;!SYM_B");
            var expected = new[] {"SYM_A", "SYM_C", "SYM_ADD1", "IGNORE_ACCESS_CHECKS", "ASMDEF_EX"};

            CollectionAssert.AreEqual(expected, modified);
        }
    }
}
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
