# Open Sesame Compiler

A Roslyn compiler to access internals/privates.
In other words, you can access to **any internals/privates** in other assemblies, **without reflection**.
Let's say, "Open sesame!"

![icon](https://user-images.githubusercontent.com/12690315/69955042-1dc8a380-1540-11ea-9d38-fa7fa77b22d9.png)

[![Nuget](https://img.shields.io/nuget/v/OpenSesameCompiler)](https://www.nuget.org/packages/OpenSesameCompiler)
![GitHub](https://img.shields.io/github/license/mob-sakai/OpenSesameCompiler)
![Nuget](https://img.shields.io/nuget/dt/OpenSesameCompiler)
![release](https://github.com/mob-sakai/OpenSesameCompiler/workflows/Release/badge.svg)

## Summary

`OpenSesameCompiler` is a Roslyn compiler to access internals/privates.


(Sample code)

## Installation

```bash
$ dotnet tool install --global OpenSesameCompiler
```

Or, download from [release page](https://github.com/mob-sakai/OpenSesameCompiler/releases)

## Usage

```sh
OpenSesameCompiler --output your.dll your.csproj
  -o, --output            (Default: ) Output path｡ If it is empty, a dll is generated in the same path as csproj.
  -a, --assemblyNames     (Default: ) Target assembly names separated by semicolons to access internally
  -c, --configuration     (Default: Release) Configuration
  -l, --logfile           (Default: compile.log) Logfile path
  --help                  Display this help screen.
  --version               Display version information.
  ProjectPath (pos. 1)    Input .csproj path
```

## License

MIT

## See Also

- GitHub page : https://github.com/mob-sakai/OpenSesameCompiler
- Nuget page : https://www.nuget.org/packages/OpenSesameCompiler
- For Unity version : https://www.nuget.org/packages/OpenSesameCompilerForUnity

[![become_a_sponsor_on_github](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)
