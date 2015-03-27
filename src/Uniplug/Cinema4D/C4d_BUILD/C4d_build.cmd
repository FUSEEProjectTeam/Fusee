@echo off &setlocal

rem Checking the registry for the location of msbuild v4.
for /f "tokens=2*" %%a in ('reg.exe query "HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath') do set "msbpath=%%b"
if "%msbpath%"=="0x1" goto :errorMsbuildPath

rem Building the C4d project
%msbpath%\msbuild.exe %3\C4d.csproj /t:Clean,Build /p:Configuration=%1 /p:Platform=%2

goto :exit

:errorMsbuildPath
echo Error: Could not find MSBuild.exe

:exit