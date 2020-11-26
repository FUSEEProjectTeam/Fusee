@echo off
setlocal

dotnet >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORDOTNET

msbuild -ver >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORMSBUILD

git >NUL 2>NUL
IF %ERRORLEVEL% EQU 9009 GOTO ERRORGIT

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf
mkdir bin\Release\nuget

echo Building distribution.
dotnet publish src\Engine\Player\Desktop\Fusee.Engine.Player.Desktop.csproj -c Release

dotnet pack Fusee.sln -c Release-NuGet
msbuild src\Base\Imp\Android\Fusee.Base.Imp.Android.csproj -t:restore,pack -p:Configuration=Release
msbuild src\Engine\Imp\Graphics\Android\Fusee.Engine.Imp.Graphics.Android.csproj -t:restore,pack -p:Configuration=Release
dotnet pack dis\NuGet\Core\Core.csproj -c Release -o bin\Release\nuget
dotnet pack dis\NuGet\Desktop\Desktop.csproj -c Release -o bin\Release\nuget
msbuild dis\NuGet\Android\Android.csproj -t:restore,pack -p:Configuration=Release

dotnet pack dis\DnTemplate\DnTemplate.csproj -c Release -o bin\Release\nuget

msbuild dis\VSTemplate\VSTemplate.sln -t:restore,pack -p:Configuration=Release
copy /Y dis\VSTemplate\VSTemplate\bin\Release\ProjectTemplates\CSharp\1033\Fusee.Template.VS.zip bin\Release\nuget\
copy /Y dis\VSTemplate\VSIX\bin\Release\Fusee.Template.VS.vsix bin\Release\nuget\

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