Open Sesame Compiler For Unity
===

A Roslyn compiler to access internals/privates for Unity.
In other words, you can access to **any internals/privates** in other assemblies, **without reflection**.
Let's say, "Open sesame!"

![](https://user-images.githubusercontent.com/12690315/70728190-4b81c980-1d44-11ea-856c-b05332d88ca0.png)
![](https://user-images.githubusercontent.com/12690315/70616819-a804bc00-1c52-11ea-8ea3-e24f94f6467d.gif)

[![](https://img.shields.io/github/release/mob-sakai/OpenSesameCompilerForUnity.svg?label=latest%20version)](https://github.com/mob-sakai/OpenSesameCompilerForUnity/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/OpenSesameCompilerForUnity.svg)](https://github.com/mob-sakai/OpenSesameCompilerForUnity/releases)
![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/OpenSesameCompilerForUnity.svg)](https://github.com/mob-sakai/OpenSesameCompilerForUnity/blob/upm/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [Description](#description) | [Install](#install) | [Usage](#usage) >>

### What's new? [See changelog ![](https://img.shields.io/github/release-date/mob-sakai/OpenSesameCompilerForUnity.svg?label=last%20updated)](https://github.com/mob-sakai/OpenSesameCompilerForUnity/blob/upm/CHANGELOG.md)
### Do you want to receive notifications for new releases? [Watch this repo ![](https://img.shields.io/github/watchers/mob-sakai/OpenSesameCompilerForUnity.svg?style=social&label=Watch)](https://github.com/mob-sakai/OpenSesameCompilerForUnity/subscription)
### Support me on GitHub!  
[![become_a_sponsor_on_github](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)


<br><br><br><br>
## Description

About `IgnoresAccessChecksToAttribute`  
[No InternalsVisibleTo, no problem – bypassing C# visibility rules with Roslyn](https://www.strathweb.com/2018/10/no-internalvisibleto-no-problem-bypassing-c-visibility-rules-with-roslyn/)


#### Features

* Easy to use: this package is out of the box
* you can access to any internal/private elements (types/members) in other assemblies, without reflection
  * Create instance
  * Get/set fields or properties
  * Call method
  * Create extension method that contains private access
* Processes only `AssemblyDefinitionFile` you configured
* Support C#8
* Support exporting as 'portable dll'
  * Publish a dll that works without this compiler

#### NOTE: Unsupported Features

* inheritance of internal/private classes
  * Same for interfaces
  * Try `InternalsVisibleToAttribute` if possible
* Set value into readonly field
  * Use reflection
* IDE support
  * Try `InternalsVisibleToAttribute` if possible


<br><br><br><br>
## Install

Find `Packages/manifest.json` in your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.coffee.open-sesame-compiler": "https://github.com/mob-sakai/OpenSesameCompilerForUnity.git",
    ...
  },
}
```

To update the package, add/change prefix `#version` to the target version.  
Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension).


### Requirement

* Unity 2018.3 or later
* Dot Net 3.0 or later



<br><br><br><br>
## Usage

### Compile AssemblyDefinitionFile to an 'internal accessible' dll

1. Select any `*.asmdef` in project view.
2. Click Right button and select `OpenSesame Compiler > Setting` in context menu.  
![](https://user-images.githubusercontent.com/12690315/70728182-49b80600-1d44-11ea-9ef7-9f2709702b81.png)
3. Open `OpenSesame Compiler Setting` and configure compile setting.  
![](https://user-images.githubusercontent.com/12690315/70728190-4b81c980-1d44-11ea-856c-b05332d88ca0.png)
   * **Assembly Names To Access:** Target assembly names separated by semicolons to access internally (eg. UnityEditor;UnityEditor.UI) 
   * **OutputDllPath:** Output dll path (eg. Assets/Editor/SomeAssembly.dll)  
4. Press `Compile` button to start compiling. After compilation, the dll will be automatically imported.
5. Enjoy!



<br><br><br><br>
## Demo

A demo project that dynamically changes the text displayed in UnityEditor's title bar.　(This package is used in Solution 3.)
https://github.com/mob-sakai/MainWindowTitleModifierForUnity

In [this class](https://github.com/mob-sakai/MainWindowTitleModifierForUnity/blob/master/Assets/Editor/Solution3.IgnoresAccessChecksToAttribute/Solution3.IgnoresAccessChecksToAttribute.cs), `ApplicationTitleDescriptor`, `EditorApplication.updateMainWindowTitle` and `EditorApplication.UpdateMainWindowTitle` are internals.
However, the class accesses to them **without reflection**.

For more details, see [the article 1 (Japanese)](https://qiita.com/mob-sakai/items/f3bbc0c45abc31ea7ac0) and [the article 2 (Japanese)](https://qiita.com/mob-sakai/items/a24780d68a6133be338f).



<br><br><br><br>
## License

* MIT



## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)  
[![become_a_sponsor_on_github](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)



## See Also

* GitHub page : https://github.com/mob-sakai/OpenSesameCompilerForUnity
* Releases : https://github.com/mob-sakai/OpenSesameCompilerForUnity/releases
* Issue tracker : https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues
* Change log : https://github.com/mob-sakai/OpenSesameCompilerForUnity/blob/upm/CHANGELOG.md
* [No InternalsVisibleTo, no problem – bypassing C# visibility rules with Roslyn](https://www.strathweb.com/2018/10/no-internalvisibleto-no-problem-bypassing-c-visibility-rules-with-roslyn/)
* Nuget tool version: https://www.nuget.org/packages/InternalAccessibleCompiler