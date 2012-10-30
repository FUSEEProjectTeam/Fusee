echo.
echo -----------------------------------------------------------------------
echo creating an output directory for the cross-compiled js parts
echo if not exist "%1..\Web\nul" md "%1..\Web"
if not exist "%1..\Web\nul" md "%1..\Web"
echo if not exist "%1..\Web\SampleObj\nul" md "%1..\Web\SampleObj"
if not exist "%1..\Web\SampleObj\nul" md "%1..\Web\SampleObj"
echo.
echo -----------------------------------------------------------------------
echo JJSIL Cross compiling Example.Simple.exe and referenced DLLs
echo %JSIL_DIR%\bin\JSILc.exe -o="%1..\Web" %1Examples.Simple.exe
"%JSIL_DIR%\bin\JSILc.exe" -o="%1..\Web" %1Examples.Simple.exe
echo.
echo JJSIL Cross compiling Example.CubeAndTiles.exe and referenced DLLs
echo "%JSIL_DIR%\bin\JSILc.exe" -o="%1..\Web" %1Examples.CubeAndTiles.exe
"%JSIL_DIR%\bin\JSILc.exe" -o="%1..\Web" %1Examples.CubeAndTiles.exe
echo.
echo -----------------------------------------------------------------------
echo Copying needed JScript files to output directory
echo xcopy "Scripts\*.*" "%1..\Web"
xcopy /Y "Scripts\*.*" "%1..\Web"
echo xcopy /Y "%1SampleObj\*.*" "%1..\Web\SampleObj"
xcopy /Y "%1SampleObj\*.*" "%1..\Web\SampleObj"
