@echo off
CLS
ECHO.
ECHO =============================
ECHO Fusee Setup per User
ECHO =============================
"%~dp0bin/Debug/Tools/fusee.exe" install
REM keep cmd open after execution to show installation messages
cmd /k
