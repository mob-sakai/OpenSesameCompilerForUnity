## Demo

1. Clone demo branch in this repo and open it with Unity 2018.3 or later
```
git clone -b demo https://github.com/mob-sakai/OpenSesameCompilerForUnity.git
```
2. The project has some inaccessible compilation errors  
![](https://user-images.githubusercontent.com/12690315/71837690-6274cb00-30fa-11ea-949f-5f60b1a1dbcd.png)
3. Do not worry, they are proper errors.  
The demo project access to internals/privates:
```cs
// EditorApplication.CallDelayed is an internal-static method in UnityEditor assembly.
EditorApplication.CallDelayed(() => Debug.Log("delayed"), 1);
```
4. Select a `Assets/Tests/Coffee.OpenSesame.Test.asmdef` in project view and activate 'Open Sesame' in inspector view  
![](https://user-images.githubusercontent.com/12690315/71837979-255d0880-30fb-11ea-99bc-3bb96b77cfa6.gif)
5. Run all edit mode tests in test runner view (`Windows > General > Test Runner`).  
The compilation error is gone, but some tests do not pass.  
![](https://user-images.githubusercontent.com/12690315/71838489-483bec80-30fc-11ea-9af4-83e2ddd7d894.png)
```cs
[Test]
public void DefineSymbols()
{
    const string log = "OSC_TEST is defined.";
    LogAssert.Expect(LogType.Log, log);
#if OSC_TEST // <- not defined!
    Debug.Log(log);
#endif
}

[Test]
public void RemoveSymbols()
{
    const string log = "TRACE is not defined.";
    LogAssert.Expect(LogType.Log, log);
#if !TRACE // <- defined automatically by Unity!
    Debug.Log(log);
#endif
}
```
6. Enable `symbols` to modify scripting define symbols for this assembly.  
Then edit `Modify Symbols` to `OSC_TEST;!TRACE`. This means "add `OSC_TEST` symbol and remove `TRACE` symbol for this assembly."  
![](https://user-images.githubusercontent.com/12690315/71839029-9a314200-30fd-11ea-8596-d1a6ea188741.png)
1. All tests pass!  
![](https://user-images.githubusercontent.com/12690315/71839100-c2b93c00-30fd-11ea-86a7-a2f1aac0a4cc.png)

For more details, see [the article 1 (Japanese)](https://qiita.com/mob-sakai/items/f3bbc0c45abc31ea7ac0) and [the article 2 (Japanese)](https://qiita.com/mob-sakai/items/a24780d68a6133be338f).
