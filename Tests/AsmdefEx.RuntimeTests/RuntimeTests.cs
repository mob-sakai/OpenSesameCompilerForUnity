#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
using System.Collections.Generic;
using NUnit.Framework;
using Coffee.AsmdefEx.RuntimeTests.InternalLibrary;

namespace Coffee.AsmdefEx.RuntimeTests
{
    class ScriptDefineSymbolTest
    {
        [Test]
        public void DefineSymbols()
        {
#if OSC_TEST
            string log = "OSC_TEST is defined.";
#else
            string log = "OSC_TEST is not defined.";
#endif
            Assert.AreEqual(log, "OSC_TEST is defined.");
        }

        [Test]
        public void RemoveSymbols()
        {
#if UNITY_5_3_OR_NEWER
            string log = "UNITY_5_3_OR_NEWER is defined.";
#else
            string log = "UNITY_5_3_OR_NEWER is not defined.";
#endif
            Assert.AreEqual(log, "UNITY_5_3_OR_NEWER is not defined.");
        }
    }

    class RuntimeTest
    {
        [Test]
        public void ListPool()
        {
            List<int> list = UnityEngine.UI.ListPool<int>.Get();
            list.Add(1);
            list.Clear();
            UnityEngine.UI.ListPool<int>.Release(list);
        }
    }

    public class AnotherInternalLibrary
    {
        [Test]
        public void InternalClass_privateStringField()
        {
            Assert.AreEqual(new InternalClass().privateStringField, "privateStringField");
        }

        [Test]
        public void InternalClass_PublicMethod()
        {
            Assert.AreEqual(new InternalClass().PublicMethod(), "PublicMethod");
        }

        [Test]
        public void InternalClass_PrivateMethod()
        {
            Assert.AreEqual(new InternalClass().PrivateMethod(), "PrivateMethod");
        }

        [Test]
        public void InternalClass_privateStaticStringField()
        {
            Assert.AreEqual(InternalClass.privateStaticStringField, "privateStaticStringField");
        }

        [Test]
        public void InternalClass_PublicStaticMethod()
        {
            Assert.AreEqual(InternalClass.PublicStaticMethod(), "PublicStaticMethod");
        }

        [Test]
        public void InternalClass_PrivateStaticMethod()
        {
            Assert.AreEqual(InternalClass.PrivateStaticMethod(), "PrivateStaticMethod");
        }
    }
}
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
