echo off
where /q hugo
IF ERRORLEVEL 1 (
    ECHO hugo.exe not found. Ensure it is installed and placed in your PATH.
    EXIT /B
)
echo on
rd public /s/q
hugo --destination ../../docs --theme=beautifulhugo --baseURL http://fusee3d.org/
REM xcopy /i /s /y _site\*.* public
