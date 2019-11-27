@echo off

echo This will be cleaning up the repository (git clean -xdf), this will DELETE all untracked files. (Ctrl-C to abort)
pause

echo Cleaning up build directory.
git clean -xdf

echo Building distribution.
dotnet build Fusee.sln -c Debug-Desktop

set startdir=%CD%
cd ..

echo Zipping it all up.
"%ProgramFiles%\7-Zip\7z" a Fusee_bin.exe Fusee\LICENSE.txt Fusee\SetupMachineWide.bat Fusee\SetupPerUser.bat Fusee\bin\Debug Fusee\dis Fusee\ext -sfx
move /y Fusee_bin.exe %FuseeRoot%\bin\
echo Done generating %FuseeRoot%\bin\Fusee_bin.exe binary distribution.

cd %startdir%