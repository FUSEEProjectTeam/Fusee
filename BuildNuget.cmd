@echo off
setlocal

SET NUGET_BUILD=true

dotnet >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORDOTNET

msbuild -ver >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORMSBUILD

git >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORGIT

rem dotnet workload install macos
rem dotnet workload install wasm-tools

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf
mkdir bin\Release\nuget

echo Building distribution.
dotnet publish -c Release -p:PublishProfile=win-x64-release src\Engine\Player\Desktop\Fusee.Engine.Player.Desktop.csproj

rem dotnet publish -c Release -p:PublishProfile=FolderProfileRelease src/Engine/Player/Blazor/Fusee.Engine.Player.Blazor.csproj
rem dotnet build -c Release src/Tools/Build/Blazorpatch/Fusee.Tools.Build.Blazorpatch.csproj
rem dotnet bin/Release/Tools/Build/Blazorpatch/net6.0/Fusee.Tools.Build.Blazorpatch.dll -p bin/Release/Player/Blazor/net6.0/publish/wwwroot -t All

dotnet pack Fusee.sln -c Release-NuGet
msbuild src\Base\Imp\Android\Fusee.Base.Imp.Android.csproj -t:restore,pack -p:Configuration=Release
msbuild src\Engine\Imp\Graphics\Android\Fusee.Engine.Imp.Graphics.Android.csproj -t:restore,pack -p:Configuration=Release
dotnet pack dis\NuGet\Core\Core.csproj -c Release -o bin\Release\nuget
dotnet pack dis\NuGet\Desktop\Desktop.csproj -c Release -o bin\Release\nuget
msbuild dis\NuGet\Android\Android.csproj -t:restore,pack -p:Configuration=Release

dotnet pack dis\DnTemplate\DnTemplate.csproj -c Release -o bin\Release\nuget

msbuild dis\VSTemplate\VSTemplate.sln -t:restore,build -p:Configuration=Release
copy /Y dis\VSTemplate\VSTemplate\bin\Release\ProjectTemplates\CSharp\1033\Fusee.Template.VS.zip bin\Release\nuget\ >nul
copy /Y dis\VSTemplate\VSIX\bin\Release\Fusee.Template.VS.vsix bin\Release\nuget\ >nul
tar -c -a -f bin\Release\nuget\io_export_fus.zip -C bin\Release\Tools\CmdLine\net8.0\BlenderScripts\addons *
goto END

:ERRORDOTNET
echo Error: dotnet not found in path
goto END

:ERRORMSBUILD
echo Error: msbuild not found in path
goto END

:ERRORGIT
echo Error: git not found in path
goto END

:END
endlocal