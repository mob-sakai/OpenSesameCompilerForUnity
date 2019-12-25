using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Compilers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.OpenSesame
{
    class UnityEditorTest
    {
        [MenuItem("Assets/OpenSesame Compiler/Develop Mode", false)]
        static void SwitchSymbol()
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.IsNullOrEmpty(symbols) ? "OPEN_SESAME_DEV" : "");
        }

        [UnityTest]
        public IEnumerator InternalMethod()
        {
            LogAssert.Expect(LogType.Log, "delayed");
            EditorApplication.CallDelayed(() => Debug.Log("delayed"), 1);
            yield return null;

            float startTime = Time.realtimeSinceStartup;
            while ((Time.realtimeSinceStartup - startTime) < 1f)
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
            tracker.lockStateChanged.AddListener(locked =>
            {
                Debug.Log("locked: " + locked);
            });
            LogAssert.Expect(LogType.Log, "locked: True");
            tracker.isLocked = true;
        }

        [Test]
        public void PrivateConstant()
        {
            Assert.AreEqual(EditorGUIUtility.EditorLockTracker.k_LockMenuText, "Lock");
        }
    }
}
