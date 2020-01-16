#if OPEN_SESAME
using UnityEditor;
namespace Coffee.OpenSesame.Tests
{
    [InitializeOnLoad]
    internal class PortableTest
    {
        static PortableTest()
        {
            UnityEngine.Debug.Log("[PortableTest] InitializeOnLoad");
        }

        [InitializeOnLoadMethod]
        static void PortableTestMethod()
        {
            UnityEngine.Debug.Log("[PortableTest] InitializeOnLoadMethod");
        }
    }
}
#endif