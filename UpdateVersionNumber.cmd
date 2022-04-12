@echo off
setlocal

if '%1'=='' goto :ERRORVERSION

set VERSIONUPDATE="bin\Release\Tools\Build\Versionupdate\net6.0\Fusee.Tools.Build.Versionupdate.dll"

dotnet build -c Release src\Tools\Build\Versionupdate\Fusee.Tools.Build.Versionupdate.csproj
IF %ERRORLEVEL% NEQ 0 goto :ERRORBUILD

dotnet %VERSIONUPDATE% -f Directory.Build.props -v %1 -t Props
dotnet %VERSIONUPDATE% -f src\Tools\BlenderScripts\addons\io_export_fus\__init__.py -v %1 -t Blender
dotnet %VERSIONUPDATE% -f dis\DnTemplate\template\Fusee_App.csproj -v %1 -t Csproj
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSIX\Properties\AssemblyInfo.cs -v %1 -t AssemblyInfo
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSTemplate\Properties\AssemblyInfo.cs -v %1 -t AssemblyInfo
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSTemplate\Android\Android.csproj -v %1 -t Csproj
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSTemplate\Core\Core.csproj -v %1 -t Csproj
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSTemplate\Desktop\Desktop.csproj -v %1 -t Csproj
dotnet %VERSIONUPDATE% -f dis\VSTemplate\VSIX\source.extension.vsixmanifest -v %1 -t VsixManifest

goto :END

:ERRORBUILD
echo "Versionupdate build unsuccessfull"
goto :END

:ERRORVERSION
echo "Please provide a version number in the format major.minor.patch[-note]"
goto :END


:END