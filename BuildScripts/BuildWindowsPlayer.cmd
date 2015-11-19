@echo off
echo Building Windows Player...

SET PROJECT=%CD%\..\
IF "%UNITY_EXE%"=="" SET "UNITY_EXE=C:\Program Files\Unity\Editor\Unity.exe"

mkdir %PROJECT%\Clients\Windows\
"%UNITY_EXE%" -batchmode -quit -projectPath %PROJECT% -buildWindowsPlayer %PROJECT%\Clients\Windows\UnityTest.exe
IF NOT "%ERRORLEVEL%"=="0" goto error

:success
echo Build COMPLETED!
exit /B 0
:error
echo Build FAILED!
exit /B 1

