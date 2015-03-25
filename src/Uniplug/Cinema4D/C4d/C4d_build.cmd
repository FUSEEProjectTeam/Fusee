@echo off &setlocal
for /f "tokens=2*" %%a in ('reg.exe query "HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath') do set "msbpath=%%b"
if "%msbpath%"=="0x1" goto :errorMsbuildPath

%msbpath%\msbuild.exe %3\C4d.csproj /t:Clean,Build /p:Configuration=%1 /p:Platform=%2

goto :exit

rem C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe D:\DataX\GitHub\Fusee\src\Uniplug\Cinema4D\C4d\C4d.csproj /p:Configuration=Debug /p:Platform=x64

:errorMsbuildPath
echo Error: Could not find MSBuild.exe

:exit