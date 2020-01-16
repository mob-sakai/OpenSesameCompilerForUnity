Open Sesame Compiler For Unity
===

**NOTE: This branch is for development purposes only.**
**To use a released package, [default branch](https://github.com/mob-sakai/OpenSesameCompilerForUnity).**

## How to develop

1. Fork this repo
1. Clone `develop` branch
1. Create pull request

1. プロジェクトを開いて、OpenSesame > Dev Modeを選択すると開発モード
2. Coffee.OpenSesame/Portable.csを変更すると、他のPortable.csにも変更が反映される
3. Coffee.OpenSesame.Bootstrapが変更されると、自動パブリッシュ

テストを通して、コミットすればok


## How to release

When you push to `preview` or `master` branch, this package is automatically released by GitHub Actions.

* Update version in `package.json` 
* Update CHANGELOG.md
* Commit documents and push
* Update and tag upm branch
* Release on GitHub
* (Publish npm registory)
