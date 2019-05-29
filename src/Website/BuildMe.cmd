@echo off

SET BUILDPATH="..\..\docs"

where /q hugo
IF ERRORLEVEL 1 (
    ECHO hugo.exe not found. Ensure it is installed and placed in your PATH.
    EXIT /B
)

@echo on

rd %BUILDPATH% /S /Q
hugo --destination %BUILDPATH% --theme=beautifulhugo --baseURL https://fusee3d.org/
