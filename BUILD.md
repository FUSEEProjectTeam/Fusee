How FUSEE builds
================

The FUSEE build system is built on top of msbuild, which is the build engine running inside Visual Studio. msbuild can also be run stand-alone, e.g. in automated build environments. Due to FUSEE's multiplatform nature, its integration with cross-compiled Java-Script and the need to control and transform digital assets with tools other than the standard compilers shipped with Visual Studio, some hand-coding in the msbuild project files (.csproj) was necessary to setup a consistent, usable, maintainable and extendable build system.

    