cd ..\..
"%ProgramFiles%\7-Zip\7z" a Fusee_v0.7.0.exe Fusee\LICENSE.txt Fusee\SetupMachineWide.bat Fusee\SetupPerUser.bat Fusee\bin\Debug Fusee\dis Fusee\ext -sfx
move /y Fusee_v0.7.0.exe %FuseeRoot%bin\
echo Done generating %FuseeRoot%bin\Fusee_v0.7.0.exe binary distribution.
