#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
using UnityEngine;
using UnityEngine.UI;

public class RuntimeTests : MonoBehaviour
{
    [SerializeField] private Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_Text.text = string.Format(
            "InternalClass.PublicStaticMethod\n -> {0}\n" +
            "InternalClass.PrivateStaticMethod\n -> {1}\n" +
            "InternalClass.privateStaticStringField\n -> {2}\n" +
            "InternalClass.PublicMethod()\n -> {3}\n" +
            "InternalClass.PrivateMethod()\n -> {4}\n" +
            "InternalClass.privateStringField()\n -> {5}\n",
            Coffee.AsmdefEx.InternalLibrary.InternalClass.PublicStaticMethod(),
            Coffee.AsmdefEx.InternalLibrary.InternalClass.PrivateStaticMethod(),
            Coffee.AsmdefEx.InternalLibrary.InternalClass.privateStaticStringField,
            new Coffee.AsmdefEx.InternalLibrary.InternalClass().PublicMethod(),
            new Coffee.AsmdefEx.InternalLibrary.InternalClass().PrivateMethod(),
            new Coffee.AsmdefEx.InternalLibrary.InternalClass().privateStringField
        );
    }
}
#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.
