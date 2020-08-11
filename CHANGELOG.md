# [1.0.0-preview.26](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.25...v1.0.0-preview.26) (2020-08-11)


### Bug Fixes

* Not recompiling on first launch ([88c05b7](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/88c05b7a7d4b777cd952adbbcfacb7848515f50d))

# [1.0.0-preview.25](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.24...v1.0.0-preview.25) (2020-08-11)


### Features

* support windows ([511bc47](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/511bc47ae34021eb78720dea2a5b36e4ad208c25))

# [1.0.0-preview.24](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.23...v1.0.0-preview.24) (2020-08-07)


### Bug Fixes

* AsmdefEx.cs in package was unexpectedly deleted ([4ffd27a](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/4ffd27a17c4855d67154d444b097a1b9dc346b9e))
* assembly is repeatedly compiled when IgnoreAccessChecks=false ([592d81c](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/592d81c9207d4f124fa9f5e97309b2a70be03193))


### Features

* hide custom compiler option ([7eb27e5](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/7eb27e51d32f40a400f338cb275d0124866615ad))
* install custom compiler with nuget package id ([4b754cd](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/4b754cda3e78a8fc295063574010b0cd1b36c159))
* Reload only when there are changes ([9bb6af8](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/9bb6af8c522d12ff45378da765e4d2cd17861b11))
* runtime support ([cbea4b5](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/cbea4b52c8bbee90eba0834cc7180f47b43fb1d7))
* skip the initial compile request if not needed ([35845c9](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/35845c9d9b165ab7a099545e4429c60421f000f1))
* support linux ([3477df2](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/3477df260418b766ec4c736d1b3c3fe23072e860))
* update OpenSesameCompiler to 3.4.0-beta.3 ([3eb9aea](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/3eb9aeaa747d3fffe8b5dfe9c36c2a5f4d4c900c))


### BREAKING CHANGES

* `IgnoresAccessChecksTo` attribute is required to access internals/privates in other assemblies as following:
```cs
[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("<TargetAssemblyName>")]
```

# [1.0.0-preview.23](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.22...v1.0.0-preview.23) (2020-07-26)


### Bug Fixes

* exe path ([afd3b70](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/afd3b700a5ebd9a0901eba755e11f8c62a2b18ae))
* fix typo ([528b821](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/528b821f43006fb4ea5d844fd8b8ae9e94f31fd3))
* Windows support ([2dd9086](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/2dd90865157d21c0039aad41ad6edb28f6959d3e))


### Features

* Add button to reload AsmdefEx.cs ([cb5f05d](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/cb5f05da28a4c0fe0c259b87134e458d30b26a44))
* Add menu to delete compiler ([da70e3e](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/da70e3e93fda9f98a812e7887daabfcb79bf5eef))
* Add menu to install compiler ([5e56b46](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/5e56b468ad434b57044e0b6a89e16f29022f0705))
* support TLS 1.2 or later to install compiler (OS native) ([802151f](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/802151fa0f476bba4523fa018e2f175ad1bbb613))

# [1.0.0-preview.22](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.21...v1.0.0-preview.22) (2020-05-20)


### Bug Fixes

* update docs ([65e7fd5](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/65e7fd57b940b31a0109b787fbdb41b60bc51cdd)), closes [#9](https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues/9)

# [1.0.0-preview.21](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.20...v1.0.0-preview.21) (2020-02-18)


### Bug Fixes

* log format ([0a253f8](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/0a253f8d7754a0085570ecf998781be93a882a21))
* multiple compile issue ([3ae0425](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/3ae042579f07357d2aafac8a96e88de477117bab))
* publish feature ([fff7dcf](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/fff7dcf4903c4d026a9069fe2d4641502a865b8a))


### Features

* new architecture ([ad58072](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/ad580728dee68637d0c2d63814967e2fc964f374))
* redesign portable mode ([6386e62](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/6386e624b1ba2c37bacb80384d6779a484df6f8b))
* remove portable mode ([e00ecdc](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/e00ecdc33e7e8ce81b42c838311bf1b7d1be6382))

# [1.0.0-preview.20](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.19...v1.0.0-preview.20) (2020-02-10)


### Bug Fixes

* fix copying portable dll ([889c84f](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/889c84fd8ff4438c0215ab65a7ed943642ceac55))
* fix for windows ([4eaab54](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/4eaab54175a8425ceec1cf2682b743f0b8931dc2))

# [1.0.0-preview.19](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.18...v1.0.0-preview.19) (2020-02-10)


### Bug Fixes

* resolve asmdef path correctly ([9461dad](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/9461dad3954418d963658dbd0d927e69c65e3104))


### Features

* set language version in csproj to latest ([07bbda4](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/07bbda41f2836face6f1b0a50dda86c359d4c9b2))

# [1.0.0-preview.19](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.18...v1.0.0-preview.19) (2020-02-10)


### Bug Fixes

* resolve asmdef path correctly ([9461dad](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/9461dad3954418d963658dbd0d927e69c65e3104))


### Features

* set language version in csproj to latest ([07bbda4](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/07bbda41f2836face6f1b0a50dda86c359d4c9b2))

# [1.0.0-preview.19](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.18...v1.0.0-preview.19) (2020-02-10)


### Bug Fixes

* resolve asmdef path correctly ([9461dad](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/9461dad3954418d963658dbd0d927e69c65e3104))


### Features

* set language version in csproj to latest ([07bbda4](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/07bbda41f2836face6f1b0a50dda86c359d4c9b2))

# [1.0.0-preview.18](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.17...v1.0.0-preview.18) (2020-02-06)


### Bug Fixes

* fix file path problems ([8d9b88b](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/8d9b88b83dacb2584b05c3559e8056d51d80ff1a))

# [1.0.0-preview.17](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.16...v1.0.0-preview.17) (2020-02-06)


### Bug Fixes

* fix file path on copying ([6c554ea](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/6c554ea57262b3f2bc482d49b36ffc460a54a582))

# [1.0.0-preview.16](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.15...v1.0.0-preview.16) (2020-02-06)


### Bug Fixes

* guid conflict ([a2a33de](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/a2a33de7e644755fc0e85a8082655cf754bbe93d))

# [1.0.0-preview.15](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.14...v1.0.0-preview.15) (2020-01-30)


### Bug Fixes

* support windows line-ending ([98f4299](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/98f4299129746c0a80b5fe9d890bff040eac5e38))

# [1.0.0-preview.14](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.13...v1.0.0-preview.14) (2020-01-30)


### Bug Fixes

* update release workflow ([9026765](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/9026765c1d89801c5bedbbd03a2a0faaf984c331))

# [1.0.0-preview.13](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.12...v1.0.0-preview.13) (2020-01-30)


### Bug Fixes

* fix asmdef settings for development ([ecda4e9](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/ecda4e95ee174e7828b6bbf738d6f423ae87bc87))
* fix inspector GUI ([69aaffe](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/69aaffe1649ff528cda6b9a2a707879ed874d14f))
* fix portable dll name ([a75d9bd](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/a75d9bd23d171d3c001362f1b487af08f8f8bafe))
* ignore unnecessary file copying ([a5acd99](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/a5acd990fbfe5041e74af33a9bf4afa93a099a7b))


### Features

* add develop menu ([0856e78](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/0856e78d7d8f35e9e8aec96c628eaae94040018e))
* add portable mode ([f72f635](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/f72f635d9ee7d188a121a9de8d9dfa80b53fafc5))
* install compiler on load ([7cee469](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/7cee4694f120c1b4338d4fcac5d821a5b0579fa9))
* reload published dll ([1391255](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/1391255bcc0cacb2448d940ea97dc2eeadcd2f4d))
* remove bootstrap ([69cf366](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/69cf3662bbbe0ed5b0a536c52041c4ee8dfe4e3d))
* remove portable script ([d4a9325](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/d4a9325cc6849088866b86b3d2b2802ca04dbe25))
* show portable version ([4121e4e](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/4121e4eae4bca07b21bfe059d5dfa3a81493ff2a))

# [1.0.0-preview.12](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.11...v1.0.0-preview.12) (2020-01-16)


### Bug Fixes

* fix .npmignore ([cdde18c](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/cdde18cf9981ab8e55ffc9acf501b61f836bdae2))

# [1.0.0-preview.11](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.10...v1.0.0-preview.11) (2020-01-16)


### Bug Fixes

* add development log ([bb7c365](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/bb7c365376f37117cb26c38b0a8e432528f22583))
* fix installer ([b707e84](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/b707e84409a72b5985438e8892014264fe74841a))
* fix publish command ([df7bdd1](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/df7bdd1718f12a5ae9735c838f000ff3b1c1a67f))
* unzip with 7z ([0db6bbd](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/0db6bbde2828d1ac608110b78ac85ec32ad5b37d))


### Features

* add 'OPEN_SESAME' symbol automatically ([8718833](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/8718833dedf81e4712b220b30e4be2017e38a750))
* add develop menu ([8893a1f](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/8893a1fa7423ec7887a51b3a0521ddd223b3c81f))
* portable mode ([2cfd2e0](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/2cfd2e0b376525a0fe8438b0bd022acaa3c19585))
* update OpenSesameCompiler to 3.4.0-beta.3 ([f8d9a1b](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/f8d9a1bc18324631c59cb5fea62d76a0c700f37c))

# [1.0.0-preview.10](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.9...v1.0.0-preview.10) (2020-01-09)


### Bug Fixes

* cannot publish on 2019.3 ([29f1974](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/29f1974be0991aea6999733430d43ed4e52844de))
* cannot publish on windows ([bfb6801](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/bfb680185b4702561c9778f12b7e28c8a453606d))
* fail to download compiler from nuget ([f579a05](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/f579a05cecf33dcc34c8ab9ebd67b9c5b223bfe8))
* not found filename ([a4cf2a5](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/a4cf2a5dbdb22390c2e5f3757e335d2f659297ac))
* TlsException: Invalid certificate received from server. ([48dbff0](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/48dbff02899e2a319b9fa6a439e5e39731c4e346)), closes [#13](https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues/13)


### Features

* add pre-compiler to init ([062892d](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/062892dc60b4f8ba3afc33f5099f99cc41a22be5))
* support 2019.3 ([134f304](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/134f3043f854111d9f8bed822b49a4dc4db5d690))
* support 2020.1 ([d2589b3](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/d2589b379e253fb0fbcbdc656706bb579cf66a12))
* support Windows ([de3bccb](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/de3bccb603ff4d5781610671463cc26bd028dd4b)), closes [#8](https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues/8)

# [1.0.0-preview.9](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.8...v1.0.0-preview.9) (2020-01-06)


### Bug Fixes

* fix special thanks ([c64bcc0](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/c64bcc0ee538f18c152b3af348fd6ee9f1884c97))

# [1.0.0-preview.8](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.7...v1.0.0-preview.8) (2020-01-06)


### Bug Fixes

* update readme ([385241b](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/385241bb09eee284c5792e2fd337440b9ad7348c))

# [1.0.0-preview.7](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.6...v1.0.0-preview.7) (2020-01-06)


### Bug Fixes

* update dll ([08f9e8e](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/08f9e8ed7a2caec065794c9c0aaff2a04f69d0fa))

# [1.0.0-preview.6](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.5...v1.0.0-preview.6) (2020-01-06)


### Bug Fixes

* remove extra logging ([7093851](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/709385132cef6d543a9843fa5f0112d8b8c76c69))

# [1.0.0-preview.5](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.4...v1.0.0-preview.5) (2020-01-06)


### Bug Fixes

* update dll for package ([d27a4d0](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/d27a4d0a564cdbd05c4a87f8f569e5c8874e1113))

# [1.0.0-preview.4](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.3...v1.0.0-preview.4) (2020-01-06)


### Features

* add/remove scripting define symbols each assemblies ([101b587](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/101b587e0f70ebb0d655071d0b07d6d81d68c475)), closes [#12](https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues/12)
* modify symbols in csproj ([47eef0e](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/47eef0e174881e4f841e2bb9080714f87a0cf231))
* refactor setting ([5e5e869](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/5e5e8696315694346cf2fcea4716eaa78ca756fd))
* use Microsoft.Net.Compilers to compile ([d9b4648](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/d9b464876df8308899e57297aebe2a0457de88f0)), closes [#11](https://github.com/mob-sakai/OpenSesameCompilerForUnity/issues/11)

# [1.0.0-preview.3](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.2...v1.0.0-preview.3) (2019-12-25)


### Bug Fixes

* overwrite predefined target assemblies ([e062393](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/e0623934c7740a69467553738d072cc1428808ff))


### Features

* support 2019.3 ([d7fab1a](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/d7fab1a069df43f002c14a1de7471d39a193fe55))

# [1.0.0-preview.2](https://github.com/mob-sakai/OpenSesameCompilerForUnity/compare/v1.0.0-preview.1...v1.0.0-preview.2) (2019-12-25)


### Bug Fixes

* default symbol is empty ([f951f15](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/f951f158f89786c06c9ad0effd2e2d73a143a7a3))
* fix readme links ([24d7680](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/24d7680c6fdaabecfb07aadce900104e3517258f))
* ignore Tests.meta to publish ([dac1aa6](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/dac1aa6ce0cbe62bbcb70b69945a52103d8fabb4))
* remove old languages on reload ([e5a04a4](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/e5a04a47a249d50f3fc511d67a6357e7c12e7bd5))

# 1.0.0-preview.1 (2019-12-25)


### Features

* implement ([ac0360c](https://github.com/mob-sakai/OpenSesameCompilerForUnity/commit/ac0360c9cf9b47c66f2a21d1e65721472ede0be9))
