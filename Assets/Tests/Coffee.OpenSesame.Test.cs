using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Coffee.OpenSesame
{
    class OpenSesameTest
    {
        [Test]
        public void DefineSymbols()
        {
            const string log = "OSC_TEST is defined.";
            LogAssert.Expect(LogType.Log, log);
#if OSC_TEST
            Debug.Log(log);
#endif
        }

        [Test]
        public void RemoveSymbols()
        {
            const string log = "TRACE is not defined.";
            LogAssert.Expect(LogType.Log, log);
#if !TRACE
            Debug.Log(log);
#endif
        }
    }

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
