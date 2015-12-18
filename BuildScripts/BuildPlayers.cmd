@echo off

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET "UNITY_EXE=C:\Program Files\Unity\Editor\Unity.exe"

CALL %CD%\BuildWindowsPlayer.cmd
IF NOT "%ERRORLEVEL%"=="0" goto error

CALL %CD%\BuildOSXPlayer.cmd
IF NOT "%ERRORLEVEL%"=="0" goto error


:success
echo Build COMPLETED!
exit /B 0
:error
echo Build FAILED!
exit /B 1

