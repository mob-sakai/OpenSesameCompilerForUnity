#if OPEN_SESAME // This line is added by Open Sesame Portable. DO NOT remov manually.using System.Collections;
using UnityEditor;

namespace OpenSesamePortableTests
{
    [InitializeOnLoad]
    public class InitializeOnLoad
    {
        static InitializeOnLoad()
        {
            UnityEngine.Debug.Log("[OpenSesamePortableTests] InitializeOnLoad");
        }

        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            UnityEngine.Debug.Log("[OpenSesamePortableTests] InitializeOnLoadMethod");
        }
    }
}
#endif // This line is added by Open Sesame Portable. DO NOT remov manually.
