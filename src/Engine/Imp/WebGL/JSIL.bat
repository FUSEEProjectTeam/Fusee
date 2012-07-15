echo -----------------------------------------------------------------------
echo JJSIL Cross compiling Example.Simple.exe and referenced DLLs
REM echo %JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" -i="Fusee.Engine.Imp.OpenTK" -p="%1Fusee.Engine.Imp.WebGL.Proxies.dll" %1Examples.Simple.exe
echo %JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" %1Examples.Simple.exe
%JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" %1Examples.Simple.exe

echo -----------------------------------------------------------------------
echo Copying needed JScript files to output directory
echo xcopy "Scripts\*.*" "%1..\Web"
xcopy /Y "Scripts\*.*" "%1..\Web"
