# WebAssembly and our current build pipeline

## HowTo: csproj
Every executable WebAsm *.csproj file needs the following two imports (This is the old standard [not dotnet core] but is used troughout Fusee due to our custom build.targets and build.props):

- `<Import Project="Sdk.props" Sdk="Mono.WebAssembly.Sdk" />` <br>(After the first property group)
- `<Import Project="Sdk.targets" Sdk="Mono.WebAssembly.Sdk" />` <br>(At the end)

The global SDK version for WebAssembly is specified within the `$(FuseeRoot)/global.json` and can thus be changed for every subfolder with a single file.

The SDK import can be obtained through either the nuget package `Mono.WebAssembly.Sdk.0.1.0-preview1.nupkg` within the `$(FuseeRoot)/packages` folder or via [Github: Mono-SDKs](https://github.com/mono/mono/tree/master/sdks/wasm/sdk/Mono.WebAssembly.Sdk).<br>The nuget package should install itself to the right directory otherwise - and for the Github version - one has to copy the whole SDK folder to: `%programfiles%\dotnet\sdk\$(UsedDotnetSDKVersion)\Sdks\`.

## HowTo: Nuget packages
The file `$(FuseeRoot)\NuGet.config` adds the new package source `wasm` and the corresponding source folder `$(FuseeRoot)\packages\*.*`<br>
This should suffice for VS 2019 and the corresponding *.csproj files to find and obtain the correct nuget packages (as of today: `0.1.0-preview1`).

## HowTo: Custom build.props and build.targets
One can use the default build.props and build.targets within every Fusee example folder with one exception:
`$(BaseIntermediateOutputPath)` and the `$(IntermediateOutputPath)` must not be set, otherwise the whole dotnet system namespace is gone (including object, int, etc.)!

_Further investigation needed!_

## Conclusion
For a working example see: `$(FuseeRoot)\Examples\RocketOnly`




