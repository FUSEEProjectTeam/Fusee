@echo off

where /q code
IF ERRORLEVEL 1 (
    ECHO Visual Studio Code is missing. Ensure it is installed and placed in your PATH.
    EXIT /B
)

where /q hugo
IF ERRORLEVEL 1 (
    ECHO hugo.exe not found. Ensure it is installed and placed in your PATH.
    EXIT /B
)

@echo on

start "" "code" .
start "" "http://localhost:1313/"
hugo server -D --theme=beautifulhugo
