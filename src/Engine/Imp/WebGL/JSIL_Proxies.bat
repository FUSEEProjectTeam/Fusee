echo -----------------------------------------------------------------------
echo JSIL Cross compiling 1Fusee.Engine.Imp.OpenTK.dll without further dependencies
echo use this as a list of method stubs when extending Fusee.Engine.Imp.WebGL.js
echo %JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" --nd %1Fusee.Engine.Imp.OpenTK.dll
%JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" --nodeps --stub="Fusee.Engine.Imp.OpenTK" %1Fusee.Engine.Imp.OpenTK.dll