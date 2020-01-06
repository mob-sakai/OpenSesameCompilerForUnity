Internal Accessable Compiler
===

**NOTE: This branch is for development purposes only.**
**To use a released package, see [Releases page](https://github.com/mob-sakai/InternalAccessibleCompilerForUnity/releases) or [default branch](https://github.com/mob-sakai/InternalAccessibleCompilerForUnity).**

## How to release

When you push to `preview` or `master` branch, this package is automatically released by GitHubAction.

* Update version in `package.json` 
* Update changelog.md
* Commit documents and push
* Update and tag upm branch
* Release on GitHub
* (Publish npm registory)

Alternatively, you can release it manually with the following command:

```bash
$ npx upm-release --pkg-root Packages/com.coffee.internal-accessible-compiler --debug
```
